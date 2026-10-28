let orderData = null; // Dữ liệu CommonOrder

// Load chi tiết đơn hàng khi trang load
window.onload = function () {
    LoadOrderDetail();
};

// Lấy URL parameters
function GetUrlParameter(name) {
    name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
    let regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
    let results = regex.exec(location.search);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
}

// Load chi tiết đơn hàng từ server
async function LoadOrderDetail() {
    let ecommerce = GetUrlParameter('ecommerce');
    let orderCode = GetUrlParameter('orderCode');

    if (!ecommerce || !orderCode) {
        CreateMustClickOkModal("Thiếu thông tin đơn hàng.", null);
        return;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("ecommerce", ecommerce);
    searchParams.append("orderCode", orderCode);
    let query = "/ProductECommerce/GetOrderDetail";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    if (responseDB.responseText == "null") {
        CreateMustClickOkModal("Không lấy được thông tin đơn hàng.", null);
        return;
    }

    try {
        if (DEBUG_ADMIN) {
            console.log("LoadOrderDetail data: " + responseDB.responseText);
        }
        orderData = JSON.parse(responseDB.responseText);
        DisplayOrderInfo();
        DisplayProductList();
    } catch (e) {
        CreateMustClickOkModal("Lỗi parse dữ liệu: " + e.message, null);
    }
}

// Hiển thị thông tin đơn hàng
function DisplayOrderInfo() {
    if (!orderData) return;

    document.getElementById("order-ship-code").innerHTML = orderData.shipCode || "";
    document.getElementById("order-status").innerHTML = orderData.status || "";

    if (orderData.created_at) {
        let date = new Date(orderData.created_at);
        document.getElementById("order-created-at").innerHTML = date.toLocaleString('vi-VN');
    }

    if (orderData.update_time) {
        let date = new Date(orderData.update_time);
        document.getElementById("order-update-time").innerHTML = date.toLocaleString('vi-VN');
    }

    document.getElementById("order-message").innerHTML = orderData.messageToSeller || "";
}

// Hiển thị danh sách sản phẩm
function DisplayProductList() {
    if (!orderData) return;

    let tbody = document.querySelector("#product-table tbody");
    tbody.innerHTML = "";

    let itemCount = orderData.listItemName ? orderData.listItemName.length : 0;

    for (let i = 0; i < itemCount; i++) {
        let row = tbody.insertRow(-1);

        // STT
        let cell0 = row.insertCell(0);
        cell0.innerHTML = i + 1;

        // Hình Ảnh
        let cell1 = row.insertCell(1);
        if (orderData.listThumbnail && orderData.listThumbnail[i]) {
            let img = document.createElement("img");
            img.src = orderData.listThumbnail[i];
            img.className = "product-thumbnail";
            img.alt = "Thumbnail";
            cell1.appendChild(img);
        }

        // Tên Sản Phẩm
        let cell2 = row.insertCell(2);
        cell2.innerHTML = orderData.listItemName ? (orderData.listItemName[i] || "") : "";

        // Tên Model
        let cell3 = row.insertCell(3);
        cell3.innerHTML = orderData.listModelName ? (orderData.listModelName[i] || "") : "";

        // Số Lượng
        let cell4 = row.insertCell(4);
        let quantity = orderData.listQuantity ? (orderData.listQuantity[i] || 0) : 0;
        cell4.innerHTML = quantity;

        // Giá Gốc
        let cell5 = row.insertCell(5);
        let originalPrice = orderData.list_model_original_price ? (orderData.list_model_original_price[i] || 0) : 0;
        cell5.innerHTML = ConvertMoneyToText(originalPrice);

        // Giá Bán
        let cell6 = row.insertCell(6);
        let discountedPrice = orderData.list_model_discounted_price ? (orderData.list_model_discounted_price[i] || 0) : 0;
        cell6.innerHTML = ConvertMoneyToText(discountedPrice);

        // Thành Tiền
        let cell7 = row.insertCell(7);
        let total = discountedPrice * quantity;
        cell7.innerHTML = ConvertMoneyToText(total);
        cell7.style.fontWeight = "bold";
    }
}

// Tạo hóa đơn nháp
async function CreateDraftInvoice() {
    if (!orderData) {
        CreateMustClickOkModal("Chưa có dữ liệu đơn hàng.", null);
        return;
    }

    // if (!confirm("Bạn có chắc muốn tạo hóa đơn nháp cho đơn hàng này?")) {
    //     return;
    // }

    ShowCircleLoader();
    let responseText = await PostJSON('/EInvoice/CreateDraftInvoice', orderData);
    RemoveCircleLoader();

    try {
        let result = JSON.parse(responseText);

        if (result.State === 0) {
            CreateMustClickOkModal(
                `Tạo hóa đơn nháp thành công!\n` +
                `Mã tra cứu: ${result.myJson.invoiceId}\n` +
                `, Id hóa đơn trên casso: ${result.myJson.lookupCode}\n` +
                `, Số hóa đơn: ${result.myJson.invoiceNumber || 'Chưa có'}`,
                null
            );
        } else {
            CreateMustClickOkModal(
                `Lỗi tạo hóa đơn:\n${result.Message}`,
                null
            );
        }
    } catch (e) {
        CreateMustClickOkModal("Lỗi: " + e.Message, null);
    }
}

// Xuất hóa đơn (phát hành lên cơ quan thuế)
async function IssueInvoice() {
    if (!orderData) {
        CreateMustClickOkModal("Chưa có dữ liệu đơn hàng.", null);
        return;
    }

    if (!confirm("Bạn có chắc muốn XUẤT HÓA ĐƠN và gửi lên cơ quan thuế?\n\nSau khi xuất sẽ KHÔNG thể chỉnh sửa!")) {
        return;
    }

    ShowCircleLoader();
    let responseText = await PostJSON('/EInvoice/IssueInvoice', orderData);
    RemoveCircleLoader();

    try {
        let result = JSON.parse(responseText);

        if (result.State === 0) {
            CreateMustClickOkModal(
                `✅ Xuất hóa đơn thành công!\n\n` +
                `Mã tra cứu: ${result.myJson.lookupCode}\n` +
                `Số hóa đơn: ${result.myJson.invoiceNumber || 'Đang chờ CQT cấp'}\n` +
                `Trạng thái: ${result.myJson.status}\n` +
                `Mã cơ quan thuế: ${result.myJson.taxAuthorityCode || 'Đang xử lý'}`,
                null
            );
        } else {
            CreateMustClickOkModal(
                `❌ Lỗi xuất hóa đơn:\n\n${result.Message}`,
                null
            );
        }
    } catch (e) {
        CreateMustClickOkModal("❌ Lỗi: " + e.message, null);
    }
}
