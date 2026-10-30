let invoiceList = []; // Danh sách hóa đơn

// Load danh sách hóa đơn khi trang load
window.onload = function () {
    // Set mặc định 7 ngày trước
    let today = new Date();
    // let sevenDaysAgo = new Date(today);
    // sevenDaysAgo.setDate(today.getDate() - 7);
    let dateString = today.toISOString().split('T')[0]; // YYYY-MM-DD
    document.getElementById("filter-date").value = dateString;

    // Load dữ liệu
    LoadInvoiceList();
};

// Lấy tên sàn từ mã số
function GetECommerceName(ecommerce) {
    switch (ecommerce) {
        case 0: return "PlayWithMe";
        case 1: return "Tiki";
        case 2: return "Shopee";
        case 3: return "Lazada";
        default: return "Unknown";
    }
}

// Lấy tên trạng thái hóa đơn
function GetInvoiceStatusName(status) {
    switch (status) {
        case 0: return "Chưa xuất";
        case 1: return "Đã xuất";
        case 2: return "Lỗi";
        default: return "Unknown";
    }
}

// Lấy class CSS cho trạng thái
function GetInvoiceStatusClass(status) {
    return `status-${status}`;
}

// Load danh sách hóa đơn từ server
async function LoadInvoiceList() {
    const searchParams = new URLSearchParams();

    // Lấy ngày filter
    let filterDate = document.getElementById("filter-date").value;
    if (filterDate) {
        searchParams.append("fromDate", filterDate);
    }

    let query = "/EInvoice/GetList";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    if (responseDB.responseText == "null") {
        CreateMustClickOkModal("Không lấy được danh sách hóa đơn.", null);
        return;
    }

    invoiceList = JSON.parse(responseDB.responseText);
    DisplayInvoiceTable();
}

// Hiển thị bảng hóa đơn
function DisplayInvoiceTable() {
    let tbody = document.querySelector("#invoice-table tbody");
    tbody.innerHTML = "";

    // Lấy filter từ radio button
    let filterValue = document.querySelector('input[name="filter-need-invoice"]:checked').value;

    // Filter danh sách theo lựa chọn
    let filteredList = invoiceList;
    if (filterValue === "need") {
        // Chỉ đơn cần xuất hóa đơn
        filteredList = invoiceList.filter(invoice => invoice.NeedInvoice === 1);
    } else if (filterValue === "pending") {
        // Đơn cần xuất nhưng chưa xuất (InvoiceStatus = 0)
        filteredList = invoiceList.filter(invoice => invoice.NeedInvoice === 1 && invoice.InvoiceStatus === 0);
    }

    // Update count
    document.getElementById("count-invoice").innerHTML = filteredList.length + " hóa đơn";

    for (let i = 0; i < filteredList.length; i++) {
        let invoice = filteredList[i];
        let row = tbody.insertRow(-1);

        // ID
        let cell0 = row.insertCell(0);
        cell0.innerHTML = invoice.Id;

        // Sàn
        let cell1 = row.insertCell(1);
        cell1.innerHTML = GetECommerceName(invoice.ECommmerce);

        // Mã Đơn
        let cell2 = row.insertCell(2);
        cell2.innerHTML = invoice.Code;
        cell2.style.cursor = "pointer";
        cell2.style.color = "blue";
        cell2.style.textDecoration = "underline";
        cell2.onclick = function () {
            OpenOrderDetail(invoice);
        };

        // Trạng Thái Đơn
        let cell3 = row.insertCell(3);
        cell3.innerHTML = invoice.OrderStatus || "";

        // TG Cần Xuất HD
        let cell4 = row.insertCell(4);
        if (invoice.NeedInvoiceUpdateTime) {
            let date = new Date(invoice.NeedInvoiceUpdateTime);
            cell4.innerHTML = date.toLocaleString('vi-VN');
        }

        // Cần HĐ
        let cell5 = row.insertCell(5);
        cell5.innerHTML = invoice.NeedInvoice === 1 ? "Có" : "Không";
        cell5.style.fontWeight = "bold";
        cell5.style.color = invoice.NeedInvoice === 1 ? "red" : "gray";

        // Trạng Thái HĐ (chỉ hiển thị khi NeedInvoice = 1)
        let cell6 = row.insertCell(6);
        if (invoice.NeedInvoice === 1) {
            cell6.innerHTML = GetInvoiceStatusName(invoice.InvoiceStatus);
            cell6.className = GetInvoiceStatusClass(invoice.InvoiceStatus);
        } else {
            cell6.innerHTML = "";
        }

        // Ngày Xuất
        let cell7 = row.insertCell(7);
        if (invoice.InvoiceDate) {
            let date = new Date(invoice.InvoiceDate);
            cell7.innerHTML = date.toLocaleString('vi-VN');
        }

        // Mã HĐ
        let cell8 = row.insertCell(8);
        cell8.innerHTML = invoice.InvoiceCode || "";

        // Mã Tra Cứu
        let cell9 = row.insertCell(9);
        cell9.innerHTML = invoice.InvoiceLookupCode || "";

        // Invoice ID
        let cell10 = row.insertCell(10);
        cell10.innerHTML = invoice.InvoiceId || "";
    }
}

// Mở tab chi tiết đơn hàng
function OpenOrderDetail(invoice) {
    let eTypeName = GetECommerceName(invoice.ECommmerce).toUpperCase();
    let url = `/EInvoice/Order?ecommerce=${eTypeName}&orderCode=${invoice.Code}`;
    window.open(url, '_blank');
}

// Lấy ký tự bỏ qua từ database
async function LoadSkippingCharacters() {
    ShowCircleLoader();

    const searchParams = new URLSearchParams();
    let query = "/EInvoice/LoadSkippingCharacters";
    let responseDB = await RequestHttpPostPromise(searchParams, query);

    RemoveCircleLoader();

    try {
        let result = JSON.parse(responseDB.responseText);

        if (result.State === 0) {
            // Hiển thị ký tự đã load
            let displayElement = document.getElementById("skipping-characters-display");
            if (displayElement) {
                displayElement.innerHTML = `Ký tự bỏ qua: "${result.Characters}"`;
            }

            CreateMustClickOkModal(result.Message, null);
        } else {
            CreateMustClickOkModal(`Lỗi: ${result.Message}`, null);
        }
    } catch (e) {
        console.error("Error:", e);
        CreateMustClickOkModal("Lỗi: " + e.message, null);
    }
}
