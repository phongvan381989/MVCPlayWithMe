let currentFilter = 0; // null = all, 0 = chưa hoàn, 1 = đã hoàn, 2 = thất lạc
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
        searchParams.append("isReturn", currentFilter);
    }

    let query = "/ProductECommerce/GetOrderReturnShopList";
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
    let buttons = document.querySelectorAll('.status-filter button');
    buttons.forEach(btn => btn.classList.remove('active'));
    event.target.classList.add('active');

    // Load lại dữ liệu
    LoadData();
}

// Render bảng dữ liệu
function RenderTable(data) {
    let tbody = document.querySelector("#myTableReturnShop tbody");
    tbody.innerHTML = "";

    // Hiển thị số lượng
    document.getElementById("count-result").innerHTML = `Tổng số: ${data.length} đơn hàng`;

    if (data.length === 0) {
        let row = tbody.insertRow(-1);
        let cell = row.insertCell(0);
        cell.colSpan = 7;
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

        // Loại Đơn
        let cell3 = row.insertCell(3);
        cell3.innerHTML = GetOrderTypeBadge(item.IsBooking);

        // Thời Gian
        let cell4 = row.insertCell(4);
        if (item.UpdateTime) {
            let date = new Date(item.UpdateTime);
            cell4.innerHTML = date.toLocaleString('vi-VN');
        } else {
            cell4.innerHTML = "";
        }

        // Trạng Thái
        let cell5 = row.insertCell(5);
        cell5.innerHTML = GetStatusBadge(item.IsReturn);

        // Thao Tác
        let cell6 = row.insertCell(6);
        if (item.IsReturn === 0) {
            // Chưa hoàn - hiển thị 2 nút
            cell6.innerHTML = `
                <div class="action-buttons">
                    <button class="btn-returned" onclick="CreateMustClickOkModal('Ấn Hoàn Hàng bên trang Order', null)">
                        ✓ Đã Hoàn
                    </button>
                    <button class="btn-lost" onclick="UpdateStatus(${item.Id}, 2, '${item.Code}')">
                        ✗ Thất Lạc
                    </button>
                </div>
            `;
        } else {
            // Đã có trạng thái - không cho sửa
            cell6.innerHTML = `<span style="color: #6c757d; font-style: italic;">Đã xử lý</span>`;
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

// Lấy badge loại đơn
function GetOrderTypeBadge(isBooking) {
    switch (isBooking) {
        case 0:
            return '<span class="status-badge" style="background-color: #6c757d; color: white;">Bình thường</span>';
        case 1:
            return '<span class="status-badge" style="background-color: #17a2b8; color: white;">Booking</span>';
        default:
            return '<span class="status-badge">Không xác định</span>';
    }
}

// Lấy badge trạng thái
function GetStatusBadge(isReturn) {
    switch (isReturn) {
        case 0:
            return '<span class="status-badge status-pending">Chưa Hoàn</span>';
        case 1:
            return '<span class="status-badge status-returned">Đã Hoàn</span>';
        case 2:
            return '<span class="status-badge status-lost">Thất Lạc</span>';
        default:
            return '<span class="status-badge">Không xác định</span>';
    }
}

// Cập nhật trạng thái
async function UpdateStatus(id, isReturn, code) {
    let statusText = isReturn === 1 ? "ĐÃ HOÀN VỀ SHOP" : "THẤT LẠC";
    let confirmText = `Xác nhận đơn hàng "${code}" ${statusText}?`;

    if (!confirm(confirmText)) {
        return;
    }

    ShowCircleLoader();

    const searchParams = new URLSearchParams();
    searchParams.append("id", id);
    searchParams.append("isReturn", isReturn);

    let query = "/ProductECommerce/UpdateOrderReturnStatus";
    let responseDB = await RequestHttpPostPromise(searchParams, query);

    RemoveCircleLoader();

    try {
        let result = JSON.parse(responseDB.responseText);

        if (result.State === 0) {
            CreateMustClickOkModal(`Cập nhật thành công!\n\nĐơn hàng "${code}" đã được đánh dấu: ${statusText}`, function () {
                LoadData(); // Reload dữ liệu
            });
        } else {
            CreateMustClickOkModal(`Lỗi: ${result.Message}`, null);
        }
    } catch (e) {
        console.error("Error:", e);
        CreateMustClickOkModal("Lỗi: " + e.message, null);
    }
}
