let invoiceList = []; // Danh sách hóa đơn

// Load danh sách hóa đơn khi trang load
window.onload = function () {
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

    // Update count
    document.getElementById("count-invoice").innerHTML = invoiceList.length + " hóa đơn";

    for (let i = 0; i < invoiceList.length; i++) {
        let invoice = invoiceList[i];
        let row = tbody.insertRow(-1);

        // ID
        let cell0 = row.insertCell(0);
        cell0.innerHTML = invoice.Id;

        // Mã Đơn
        let cell1 = row.insertCell(1);
        cell1.innerHTML = invoice.Code;

        // Sàn
        let cell2 = row.insertCell(2);
        cell2.innerHTML = GetECommerceName(invoice.ECommmerce);

        // Trạng Thái Đơn
        let cell3 = row.insertCell(3);
        cell3.innerHTML = invoice.OrderStatus || "";

        // Tổng Tiền
        let cell4 = row.insertCell(4);
        cell4.innerHTML = ConvertMoneyToText(invoice.MoneySum);

        // Trạng Thái HĐ
        let cell5 = row.insertCell(5);
        cell5.innerHTML = GetInvoiceStatusName(invoice.InvoiceStatus);
        cell5.className = GetInvoiceStatusClass(invoice.InvoiceStatus);

        // Ngày Xuất
        let cell6 = row.insertCell(6);
        if (invoice.InvoiceDate) {
            let date = new Date(invoice.InvoiceDate);
            cell6.innerHTML = date.toLocaleString('vi-VN');
        }

        // Mã HĐ
        let cell7 = row.insertCell(7);
        cell7.innerHTML = invoice.InvoiceCode || "";

        // Ghi Chú
        let cell8 = row.insertCell(8);
        cell8.innerHTML = invoice.Note || "";

        // Thao Tác
        let cell9 = row.insertCell(9);

        // Nút Cập Nhật Trạng Thái
        let btnUpdate = document.createElement("button");
        btnUpdate.innerHTML = "Cập Nhật";
        btnUpdate.className = "btn-update";
        btnUpdate.onclick = function () {
            ShowUpdateStatusModal(invoice);
        };
        cell9.appendChild(btnUpdate);

        // Nút Xóa
        let btnDelete = document.createElement("button");
        btnDelete.innerHTML = "Xóa";
        btnDelete.className = "btn-update";
        btnDelete.style.backgroundColor = "#f44336";
        btnDelete.style.color = "white";
        btnDelete.onclick = function () {
            DeleteInvoice(invoice.Id);
        };
        cell9.appendChild(btnDelete);
    }
}

// Hiển thị modal thêm mới
function ShowAddModal() {
    document.getElementById("modal-title").innerHTML = "Thêm Hóa Đơn";
    document.getElementById("invoice-id").value = "-1";
    document.getElementById("invoice-code").value = "";
    document.getElementById("invoice-ecommerce").value = "1";
    document.getElementById("invoice-order-status").value = "";
    document.getElementById("invoice-money").value = "0";
    document.getElementById("invoice-status").value = "0";
    document.getElementById("invoice-code-einvoice").value = "";
    document.getElementById("invoice-note").value = "";

    document.getElementById("invoice-modal").style.display = "block";
}

// Hiển thị modal cập nhật trạng thái
function ShowUpdateStatusModal(invoice) {
    document.getElementById("modal-title").innerHTML = "Cập Nhật Trạng Thái";
    document.getElementById("invoice-id").value = invoice.Id;
    document.getElementById("invoice-code").value = invoice.Code;
    document.getElementById("invoice-code").disabled = true;
    document.getElementById("invoice-ecommerce").value = invoice.ECommmerce;
    document.getElementById("invoice-ecommerce").disabled = true;
    document.getElementById("invoice-order-status").value = invoice.OrderStatus || "";
    document.getElementById("invoice-money").value = invoice.MoneySum;
    document.getElementById("invoice-status").value = invoice.InvoiceStatus;
    document.getElementById("invoice-code-einvoice").value = invoice.InvoiceCode || "";
    document.getElementById("invoice-note").value = invoice.Note || "";

    document.getElementById("invoice-modal").style.display = "block";
}

// Đóng modal
function CloseModal() {
    document.getElementById("invoice-modal").style.display = "none";
    document.getElementById("invoice-code").disabled = false;
    document.getElementById("invoice-ecommerce").disabled = false;
}

// Lưu hóa đơn
async function SaveInvoice() {
    let id = parseInt(document.getElementById("invoice-id").value);
    let code = document.getElementById("invoice-code").value.trim();
    let ecommerce = parseInt(document.getElementById("invoice-ecommerce").value);
    let orderStatus = document.getElementById("invoice-order-status").value.trim();
    let moneySum = parseFloat(document.getElementById("invoice-money").value);
    let invoiceStatus = parseInt(document.getElementById("invoice-status").value);
    let invoiceCode = document.getElementById("invoice-code-einvoice").value.trim();
    let note = document.getElementById("invoice-note").value.trim();

    // Validate
    if (code === "") {
        CreateMustClickOkModal("Mã đơn hàng không được rỗng.", null);
        return;
    }

    const searchParams = new URLSearchParams();
    let query = "";

    if (id === -1) {
        // Thêm mới
        searchParams.append("code", code);
        searchParams.append("ecommerce", ecommerce);
        searchParams.append("orderStatus", orderStatus);
        searchParams.append("moneySum", moneySum);
        searchParams.append("invoiceStatus", invoiceStatus);
        searchParams.append("note", note);
        query = "/EInvoice/Insert";
    } else {
        // Cập nhật trạng thái
        searchParams.append("id", id);
        searchParams.append("invoiceStatus", invoiceStatus);
        searchParams.append("invoiceCode", invoiceCode);
        searchParams.append("note", note);
        query = "/EInvoice/UpdateStatus";
    }

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText,
        "Lưu hóa đơn thành công.",
        "Có lỗi xảy ra.");

    CloseModal();
    LoadInvoiceList();
}

// Xóa hóa đơn
async function DeleteInvoice(id) {
    if (!confirm("Bạn có chắc muốn xóa hóa đơn này?")) {
        return;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("id", id);
    let query = "/EInvoice/Delete";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText,
        "Xóa hóa đơn thành công.",
        "Có lỗi xảy ra.");

    LoadInvoiceList();
}
