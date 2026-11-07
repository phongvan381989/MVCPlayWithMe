// mảng đối tượng product
let listBasicInfo = [];

let listInsertTemp = [];

// Danh sách đối tượng lưu về db
let listSaveInfo = [];

let discount = 0;

// Lưu thông tin khách hàng từ modal
let currentCustomerInfor = null;

// Hàm cập nhật tổng kết đơn hàng
function UpdateOrderSummary() {
    // Tính tổng tiền sản phẩm từ bảng
    let table = document.getElementById("myTable");
    let totalProducts = 0;

    for (let i = 1; i < table.rows.length; i++) {
        let row = table.rows[i];
        // Cell 7 là cột "Tổng Tiền"
        let totalCell = row.cells[7];
        if (totalCell) {
            let totalText = totalCell.innerHTML.replace(/[,.]/g, ''); // Loại bỏ dấu phẩy
            let total = parseFloat(totalText) || 0;
            totalProducts += total;
        }
    }

    // Lấy phí ship
    let shippingFee = parseFloat(document.getElementById("shipping-fee").value) || 0;

    // Tính tổng thanh toán (không có giảm giá)
    let totalPayment = totalProducts + shippingFee;

    // Cập nhật hiển thị
    document.getElementById("total-products").innerHTML = totalProducts.toLocaleString('vi-VN') + ' ₫';
    document.getElementById("total-shipping").innerHTML = shippingFee.toLocaleString('vi-VN') + ' ₫';
    document.getElementById("total-payment").innerHTML = totalPayment.toLocaleString('vi-VN') + ' ₫';
}

// Event listener cho phí ship
window.addEventListener('load', async function() {
    document.getElementById("shipping-fee").addEventListener("input", UpdateOrderSummary);
    await GetListPublisher();
});

// Xử lý khi hoàn thành modal nhập thông tin khách hàng
function FinishCustomerInforModal() {
    if (!ValidCustomerInforInput()) {
        return;
    }

    // Lấy thông tin từ modal
    currentCustomerInfor = CreateAddressObjFromInput();

    // Hiển thị thông tin đã nhập
    document.getElementById("display-name").innerText = currentCustomerInfor.name;
    document.getElementById("display-phone").innerText = currentCustomerInfor.phone;
    document.getElementById("display-address").innerText =
        currentCustomerInfor.province + ", " +
        currentCustomerInfor.subdistrict + ", " +
        currentCustomerInfor.detail;

    document.getElementById("customer-infor-display").style.display = "block";

    // Đóng modal
    ReturnCustomerInforModal();
}

// Đóng modal
function ReturnCustomerInforModal() {
    document.getElementById("modal-customer-infor").style.display = "none";
}

function ComboIdName(id, name) {
    this.id = id;
    this.name = name;
}

function SetDataIdListOfCombo(list) {
    // list chứa dữ liệu comboId lặp lại, tạo list mới không lặp lại
    let listTemp = [];
    let isExist = false;
    for (let i = 0; i < list.length; i++) {
        isExist = false;
        for (let j= 0; j < listTemp.length; j++) {
            if (listTemp[j].id === list[i].comboId) {
                isExist = true;
                break;
            }
        }
        if (isExist === false) {
            listTemp.push(new ComboIdName(list[i].comboId, list[i].comboName));
        }
    }
    let ele = document.getElementById("list-combo");

    SetDataListOfIdName(ele, listTemp);
}

async function GetBasicInfoProduct() {
    let publisherId =
        GetDataIdFromPublisherDatalist(document.getElementById("publisher-id").value);
    if (publisherId === null) {
        CreateMustClickOkModal("Không lấy được thông tin nhà phát hành", null);
        return;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("publisherId", publisherId);
    let query = "/Product/GetProductIdCodeBarcodeNameBooCoverkPrice";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();
    if (responseDB.responseText != "null") {
        listBasicInfo = JSON.parse(responseDB.responseText);
    }
    else {
        listBasicInfo = [];
    }

    let ele = document.getElementById("list-product-name");
    SetDataListOfIdName(ele, listBasicInfo);
    SetDataIdListOfCombo(listBasicInfo);
    document.getElementById("code-or-isbn").value = "";
    document.getElementById("product-name-id").value = "";
    document.getElementById("combo-id").value = "";
}

// discount mặc định = 0 (không cần input nữa)

document.getElementById("code-or-isbn").addEventListener("keypress", function (event) {
    if (event.key === "Enter") {
        event.preventDefault();
        document.getElementById("add-head-list").click();
    }
});

document.getElementById("code-or-isbn").addEventListener("focus", function (event) {
    document.getElementById("product-name-id").value = "";
    document.getElementById("combo-id").value = "";
});

document.getElementById("product-name-id").addEventListener("keypress", function (event) {
    if (event.key === "Enter") {
        event.preventDefault();
        document.getElementById("add-head-list").click();
    }
});

document.getElementById("product-name-id").addEventListener("focus", function (event) {
    document.getElementById("code-or-isbn").value = "";
    document.getElementById("combo-id").value = "";
});

document.getElementById("combo-id").addEventListener("keypress", function (event) {
    if (event.key === "Enter") {
        event.preventDefault();
        document.getElementById("add-head-list").click();
    }
});

document.getElementById("combo-id").addEventListener("focus", function (event) {
    document.getElementById("code-or-isbn").value = "";
    document.getElementById("product-name-id").value = "";
});

function CheckProductExistInTable(obj, table) {
    let length = table.rows.length;
    let exist = false;
    for (let i = 1; i < length; i++) {
        if (table.rows[i].cells[0].innerHTML == obj.id) {
            exist = true;
            break;
        }
    }
    return exist;
}

function AddHeadList() {
    listInsertTemp = [];
    let length = listBasicInfo.length;
    if (length == 0) {
        CreateMustClickOkModal("Danh sách sản phẩm chưa được tải về. Vui lòng chọn nhà phát hành, để tải danh sách sản phẩm tương ứng. Hoặc thử lại sau nếu đã chọn nhà phát hành", null);
        return;
    }

    // Lấy chiết khấu và số lượng từ input
    let discountInput = parseFloat(document.getElementById("discount-input").value) || 0;
    let quantityInput = parseInt(document.getElementById("quantity-input").value) || 1;

    let code = document.getElementById("code-or-isbn").value;
    let nameId = GetDataIdFromProductNameDatalist(document.getElementById("product-name-id").value);
    let comboId = GetDataIdFromComboDatalist(document.getElementById("combo-id").value)

    if (!isEmptyOrSpaces(code)) {
        //if (code.length != 13) {
        //    CreateMustClickOkModal("Mã sản phẩm hoặc ISBN không chính xác", null);
        //    return;
        //}
        //if (code.substring(0, 2) != "89" &&
        //    code.substring(0, 6) != "978604") {
        //    CreateMustClickOkModal("Mã sản phẩm hoặc ISBN không chính xác", null);
        //    return;
        //}
        //if (code.substring(0, 2) == "89") {// Tìm theo mã sản phẩm
        //    for (let i = 0; i < length; i++) {
        //        if (listBasicInfo[i].code === code) {
        //            listInsertTemp.push(listBasicInfo[i]);
        //            break;
        //        }
        //    }
        //}
        //else {// Tìm theo isbn
        //    for (let i = 0; i < length; i++) {
        //        if (listBasicInfo[i].barcode === code) {
        //            listInsertTemp.push(listBasicInfo[i]);
        //            break;
        //        }
        //    }
        //}

        for (let i = 0; i < length; i++) {
            if (listBasicInfo[i].code === code ||
                listBasicInfo[i].barcode === code) {
                listInsertTemp.push(listBasicInfo[i]);
                break;
            }
        }
    }
    else if (nameId != null) { // Tìm theo tên
        for (let i = 0; i < length; i++) {
            if (listBasicInfo[i].id == nameId) {
                listInsertTemp.push(listBasicInfo[i]);
                break;
            }
        }
    }
    else if (comboId != null) {
        for (let i = 0; i < length; i++) {
            if (listBasicInfo[i].comboId == comboId) {
                listInsertTemp.push(listBasicInfo[i]);
            }
        }
    }

    if (listInsertTemp.length > 0) {
        // Find a <table> element with id="myTable":
        let table = document.getElementById("myTable");

        for (let i = 0; i < listInsertTemp.length; i++) { // Thêm vào hàng đầu tiên của table
            let objBasicInfo = listInsertTemp[i];
            // Kiểm tra sản phẩm đã xuất hiện trong danh sách hiển thị
            if (CheckProductExistInTable(objBasicInfo, table)) {
                continue;
            }

            // Create an empty <tr> element and add it to the 1st position of the table:
            let row = table.insertRow(1);

            // Insert new cells (<td> elements)
            let cell1 = row.insertCell(0);
            let cell2 = row.insertCell(1);
            let cell3 = row.insertCell(2);
            let cell4 = row.insertCell(3);
            let cell5 = row.insertCell(4);
            let cell6 = row.insertCell(5);
            let cell7 = row.insertCell(6);
            let cell8 = row.insertCell(7);
            let cell9 = row.insertCell(8);
            let cell10 = row.insertCell(9);

            // Id
            cell1.innerHTML = objBasicInfo.id;
            cell1.style.display = "none";

            // Image
            let img = document.createElement("img");
            if (objBasicInfo.imageSrc.length > 0) {
                img.setAttribute("src", Get320VersionOfImageSrc(objBasicInfo.imageSrc[0]));
            } else {
                img.setAttribute("src", srcNoImageThumbnail);
            }
            img.height = thumbnailHeight;
            img.width = thumbnailWidth;
            img.className = "go-to-detail-product";
            img.onclick = function () {
                window.open("UpdateDelete?id=" + objBasicInfo.id);
            };
            cell2.append(img);

            // Tên
            cell3.innerHTML = objBasicInfo.name;

            // Giá bìa
            cell4.innerHTML = objBasicInfo.bookCoverPrice;

            // Giá Bán (input để có thể sửa)
            let salePrice = (100 - discountInput) * objBasicInfo.bookCoverPrice / 100;

            // Chiết khấu - format ban đầu
            if (discountInput > 0) {
                cell5.innerHTML = `-${discountInput.toFixed(2)}%`;
                cell5.style.color = "red";
                cell5.style.fontWeight = "bold";
            } else {
                cell5.innerHTML = `0.00%`;
                cell5.style.color = "green";
                cell5.style.fontWeight = "bold";
            }
            let input6 = document.createElement("input");
            input6.type = "number";
            input6.value = salePrice;
            input6.step = "1000";
            cell6.appendChild(input6);

            // Số lượng nhập
            // Focus vào đây
            let input7 = document.createElement("input");
            input7.type = "number";
            input7.min = "1";
            cell7.appendChild(input7);
            input7.focus();
            input7.value = quantityInput; // Lấy từ input

            // Tổng Tiền
            let totalPrice = salePrice * input7.value;
            cell8.innerHTML = ConvertMoneyToText(totalPrice);

            // Tồn kho
            let stockQuantity = objBasicInfo.quantity || 0;
            cell9.innerHTML = stockQuantity;
            if (stockQuantity <= 0) {
                cell9.style.color = "red";
                cell9.style.fontWeight = "bold";
            }

            // Hàm cập nhật tổng tiền
            function updateTotal() {
                let sale = parseFloat(input6.value) || 0;
                let qty = parseFloat(input7.value) || 0;
                let total = sale * qty;
                cell8.innerHTML = ConvertMoneyToText(total);
                UpdateOrderSummary(); // Cập nhật tổng kết đơn hàng
            }

            // Hàm cập nhật chiết khấu khi sửa giá bán
            function updateDiscount() {
                let sale = parseFloat(input6.value) || 0;
                let coverPrice = objBasicInfo.bookCoverPrice;
                if (coverPrice > 0) {
                    let newDiscount = ((coverPrice - sale) / coverPrice) * 100;

                    // Format và đổi màu
                    if (sale < coverPrice) {
                        // Giá bán < Giá bìa → Màu đỏ, dấu "-", thêm "%"
                        cell5.innerHTML = `-${newDiscount.toFixed(2)}%`;
                        cell5.style.color = "red";
                        cell5.style.fontWeight = "bold";
                    } else {
                        // Giá bán >= Giá bìa → Màu xanh
                        let increase = ((sale - coverPrice) / coverPrice) * 100;
                        cell5.innerHTML = `+${increase.toFixed(2)}%`;
                        cell5.style.color = "green";
                        cell5.style.fontWeight = "bold";
                    }
                }
                updateTotal();
            }

            // Event listener cho Giá Bán
            input6.addEventListener("input", updateDiscount);

            // Event listener cho Số Lượng
            input7.addEventListener("input", updateTotal);
            input7.addEventListener("keypress", function (event) {
                if (event.key === "Enter") {
                    event.preventDefault();
                    let codeTemp = document.getElementById("code-or-isbn").value;
                    let nameTemp = document.getElementById("product-name-id").value;
                    let comboTemp = document.getElementById("combo-id").value;

                    if (!isEmptyOrSpaces(codeTemp)) {
                        //document.getElementById("product-name-id").value = "";
                        //document.getElementById("combo-id").value = "";
                        document.getElementById("code-or-isbn").focus();
                        document.getElementById("code-or-isbn").select();
                    }
                    else if (!isEmptyOrSpaces(nameTemp)) {
                        //document.getElementById("code-or-isbn").value = "";
                        document.getElementById("product-name-id").focus();
                        document.getElementById("product-name-id").select();
                    }
                    else if (!isEmptyOrSpaces(comboTemp)) {
                        document.getElementById("combo-id").focus();
                        document.getElementById("combo-id").select();
                    }
                }
            });

            // Nút xóa
            let btn = document.createElement("button");
            btn.onclick = function () {
                this.parentElement.parentElement.remove();
                UpdateOrderSummary(); // Cập nhật tổng kết sau khi xóa
            };
            btn.innerHTML = "Xóa";
            cell10.appendChild(btn);

            // Cập nhật tổng kết khi thêm sản phẩm mới
            UpdateOrderSummary();

            //alert(JSON.stringify(objBasicInfo));
        }
    }
    else {
        CreateMustClickOkModal("Không tìm thấy sản phẩm phù hợp", null);
    }
}

// Constructor function for objects
function objImport(productId, price, quantity, bookCoverPrice, discount) {
    this.productId = productId;
    this.price = price;
    this.quantity = quantity;
    this.bookCoverPrice = bookCoverPrice;
    this.discount = discount;
}

// Lưu db
async function CreateOrderManually() {
    let rows = document.getElementById("myTable").rows;
    let length = rows.length;

    if (length <= 1) {
        CreateMustClickOkModal("Chưa có sản phẩm nào trong đơn hàng", null);
        return;
    }

    // Kiểm tra số lượng và tồn kho
    for (let i = 1; i < length; i++) {
        let row = rows[i];
        let productName = row.cells[2].innerHTML;
        let quantity = Number(row.cells[6].childNodes[0].value);
        let stockQuantity = Number(row.cells[8].innerHTML);

        // Kiểm tra số lượng < 1
        if (quantity < 1) {
            CreateMustClickOkModal(`Sản phẩm "${productName}" có số lượng không hợp lệ (${quantity}). Số lượng phải >= 1.`, null);
            return;
        }

        // Kiểm tra tồn kho <= 0
        if (stockQuantity <= 0) {
            CreateMustClickOkModal(`Sản phẩm "${productName}" đã hết hàng (Tồn kho: ${stockQuantity}). Vui lòng xóa khỏi đơn hàng.`, null);
            return;
        }

        // Kiểm tra số lượng đặt mua > tồn kho
        if (quantity > stockQuantity) {
            CreateMustClickOkModal(`Sản phẩm "${productName}" không đủ hàng.\nSố lượng đặt: ${quantity}\nTồn kho: ${stockQuantity}\nVui lòng giảm số lượng hoặc xóa khỏi đơn hàng.`, null);
            return;
        }
    }

    // Tạo danh sách OrderDetail từ bảng
    let lsOrderDetail = [];
    for (let i = 1; i < length; i++) {
        let row = rows[i];
        let orderDetail = {
            id: -1,
            orderId: -1,
            sanPhamId: Number(row.cells[0].innerHTML),
            name: row.cells[2].innerHTML,
            CoverImageFileName: "",
            quantity: Number(row.cells[6].childNodes[0].value),
            bookCoverPrice: Number(row.cells[3].innerHTML),
            price: Number(row.cells[5].childNodes[0].value)
        };
        lsOrderDetail.push(orderDetail);
    }

    // Lấy thông tin từ order summary
    let totalProducts = 0;
    for (let i = 1; i < length; i++) {
        let row = rows[i];
        let totalCell = row.cells[7];
        if (totalCell) {
            let totalText = totalCell.innerHTML.replace(/[,.]/g, '');
            let total = parseFloat(totalText) || 0;
            totalProducts += total;
        }
    }

    let shippingFee = parseFloat(document.getElementById("shipping-fee").value) || 0;
    let totalPayment = totalProducts + shippingFee;

    // Tạo danh sách OrderPay
    let lsOrderPay = [
        {
            id: -1,
            orderId: -1,
            orderSimplePromoId: -1,
            orderSimplePromotion: null,
            type: 0, // TOTAL
            strType: "Tổng tiền hàng",
            value: totalProducts
        },
        {
            id: -1,
            orderId: -1,
            orderSimplePromoId: -1,
            orderSimplePromotion: null,
            type: 1, // SHIP
            strType: "Phí vận chuyển",
            value: shippingFee
        },
        {
            id: -1,
            orderId: -1,
            orderSimplePromoId: -1,
            orderSimplePromotion: null,
            type: 10, // FINAL
            strType: "Tổng thanh toán",
            value: totalPayment
        }
    ];

    // Tạo danh sách OrderTrack (trạng thái ban đầu: PROCESSING = 0)
    let lsOrderTrack = [
        {
            id: -1,
            orderId: -1,
            status: 0, // EOrderStatus.PROCESSING
            strStatus: "Đang xử lý",
            time: null
        }
    ];

    // Kiểm tra đã nhập thông tin người nhận chưa
    if (!currentCustomerInfor) {
        CreateMustClickOkModal("Vui lòng nhập thông tin người nhận", null);
        return;
    }

    // Lấy thông tin từ các dropdown
    let orderFrom = parseInt(document.getElementById("order-from").value) || 1;
    let orderPayStatus = parseInt(document.getElementById("order-pay-status").value) || 0;
    let paymentMethod = parseInt(document.getElementById("payment-method").value) || 1;

    // Tạo đối tượng Order
    let order = {
        id: -1,
        customerId: -1,
        lsOrderPay: lsOrderPay,
        lsOrderTrack: lsOrderTrack,
        lsOrderDetail: lsOrderDetail,
        name: currentCustomerInfor.name || "",
        phone: currentCustomerInfor.phone || "",
        province: currentCustomerInfor.province || "",
        subdistrict: currentCustomerInfor.subdistrict || "",
        detail: currentCustomerInfor.detail || "",
        code: "",
        from: orderFrom, // 0: Web, 1: Facebook, 3: Zalo
        note: document.getElementById("customer-message").value || "",
        time: null,
        OrderCode: "",
        OrderStatus: 0, // EOrderStatus.PROCESSING
        OrderPayStatus: orderPayStatus, // 0: PENDING, 1: PAID, 2: REFUNDED
        PaymentMethod: paymentMethod, // 0: CASH_ON_DELIVERY, 1: BANK_TRANSFER
        PaymentDeadline: null
    };

    // Gọi API với PostJSON
    let url = "/Order/CreateOrderManually";

    ShowCircleLoader();
    try {
        let responseText = await PostJSON(url, order);
        RemoveCircleLoader();

        if (responseText && responseText !== "null") {
            CheckStatusResponseAndShowPrompt(responseText, "Tạo đơn hàng thành công.", "Tạo đơn hàng thất bại.");

            // Parse để kiểm tra kết quả
            let result = JSON.parse(responseText);
            if (result.State === 0) {
                // Xóa toàn bộ nếu thành công
                DeleteRowsExcludeHead(document.getElementById("myTable"));

                // Reset form thông tin người nhận
                currentCustomerInfor = null;
                document.getElementById("customer-infor-display").style.display = "none";
                document.getElementById("customer-message").value = "";
                document.getElementById("shipping-fee").value = "0";

                // Cập nhật tổng kết
                UpdateOrderSummary();
            }
        }
        else {
            CreateMustClickOkModal("Tạo đơn hàng thất bại.", null);
        }
    } catch (error) {
        RemoveCircleLoader();
        CreateMustClickOkModal("Lỗi: " + error.message, null);
    }
}

function Delete() {
    let text = "Bạn chắc chắn muốn XÓA dữ liệu nhập kho đang hiển thị?";
    if (confirm(text) == false)
        return;

    DeleteRowsExcludeHead(document.getElementById("myTable"));
    UpdateOrderSummary(); // Cập nhật tổng kết sau khi xóa toàn bộ
}
