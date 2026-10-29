let currentFilter = 0; // null = all, 0 = cần check
let allData = []; // Cache dữ liệu

// Load dữ liệu khi trang load
window.onload = function () {
    LoadData();
};

// Load dữ liệu từ server
async function LoadData() {
    ShowCircleLoader();

    const searchParams = new URLSearchParams();
    if (currentFilter !== null) {
        searchParams.append("isDone", currentFilter);
    }

    let query = "/EInvoice/GetCheckInvoiceList";
    let responseDB = await RequestHttpPostPromise(searchParams, query);

    RemoveCircleLoader();

    if (responseDB.responseText) {
        try {
            allData = JSON.parse(responseDB.responseText);
            RenderTable(allData);
        } catch (e) {
            console.error("Error parsing data:", e);
            CreateMustClickOkModal("Lỗi parse dữ liệu: " + e.message, null);
        }
    } else {
        CreateMustClickOkModal("Không lấy được dữ liệu", null);
    }
}

// Lọc theo trạng thái
function FilterByStatus(status) {
    currentFilter = status;

    // Cập nhật active button
    let buttons = document.querySelectorAll('.filter-buttons button');
    buttons.forEach(btn => btn.classList.remove('active'));
    event.target.classList.add('active');

    // Load lại dữ liệu
    LoadData();
}

// Render bảng dữ liệu
function RenderTable(data) {
    let tbody = document.querySelector("#myTableCheckInvoice tbody");
    tbody.innerHTML = "";

    // Hiển thị số lượng
    let pendingCount = data.filter(item => item.IsDone === 0).length;
    let totalCount = data.length;
    document.getElementById("count-result").innerHTML =
        `Tổng số: ${totalCount} đơn | Cần check: ${pendingCount} đơn`;

    if (data.length === 0) {
        let row = tbody.insertRow(-1);
        let cell = row.insertCell(0);
        cell.colSpan = 6;
        cell.innerHTML = "Không có dữ liệu";
        cell.style.textAlign = "center";
        cell.style.padding = "20px";
        return;
    }

    data.forEach(item => {
        let row = tbody.insertRow(-1);

        // ID
        let cell0 = row.insertCell(0);
        cell0.innerHTML = item.Id;

        // Sàn TMĐT
        let cell1 = row.insertCell(1);
        cell1.innerHTML = GetECommerceName(item.ECommmerce);

        // Mã Đơn
        let cell2 = row.insertCell(2);
        cell2.innerHTML = item.Code || "";

        // Thời Gian
        let cell3 = row.insertCell(3);
        if (item.UpdateTime) {
            let date = new Date(item.UpdateTime);
            cell3.innerHTML = date.toLocaleString('vi-VN');
        } else {
            cell3.innerHTML = "";
        }

        // Trạng Thái
        let cell4 = row.insertCell(4);
        cell4.innerHTML = GetStatusBadge(item.IsDone);

        // Thao Tác
        let cell5 = row.insertCell(5);
        if (item.IsDone === 0) {
            // Chưa xong - hiển thị nút "Đã Xong"
            cell5.innerHTML = `
                <button class="btn-done" onclick="MarkAsDone(${item.Id}, '${item.Code}')">
                    ✓ Đã Xong
                </button>
            `;
        } else {
            // Đã xong - không cho sửa
            cell5.innerHTML = `<span style="color: #6c757d; font-style: italic;">Đã hoàn thành</span>`;
        }
    });
}

// Lấy tên sàn TMĐT
function GetECommerceName(ecommerce) {
    switch (ecommerce) {
        case 0: return "Play With Me";
        case 1: return "Tiki";
        case 2: return "Shopee";
        case 3: return "Lazada";
        default: return "Không xác định";
    }
}

// Lấy badge trạng thái
function GetStatusBadge(isDone) {
    switch (isDone) {
        case 0:
            return '<span class="status-badge status-pending">Cần Check</span>';
        case 1:
            return '<span class="status-badge status-done">Đã Xong</span>';
        default:
            return '<span class="status-badge">Không xác định</span>';
    }
}

// Đánh dấu đã kiểm tra xong
async function MarkAsDone(id, code) {
    let confirmText = `Xác nhận đã kiểm tra và xử lý xong đơn hàng "${code}"?`;

    if (!confirm(confirmText)) {
        return;
    }

    ShowCircleLoader();

    const searchParams = new URLSearchParams();
    searchParams.append("id", id);

    let query = "/EInvoice/UpdateCheckInvoiceStatus";
    let responseDB = await RequestHttpPostPromise(searchParams, query);

    RemoveCircleLoader();

    try {
        let result = JSON.parse(responseDB.responseText);

        if (result.State === 0) {
            CreateMustClickOkModal(`✓ Cập nhật thành công!\n\nĐơn hàng "${code}" đã được đánh dấu hoàn thành.`, function () {
                LoadData(); // Reload dữ liệu
            });
        } else {
            CreateMustClickOkModal(`❌ Lỗi: ${result.Message}`, null);
        }
    } catch (e) {
        console.error("Error:", e);
        CreateMustClickOkModal("❌ Lỗi: " + e.message, null);
    }
}
