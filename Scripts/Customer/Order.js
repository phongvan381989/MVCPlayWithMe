// Tham số tìm kiếm
let status;
let listOrder = null; // Danh sách tất cả đơn hàng người dùng

// Hiện tất cả hoặc 1 đơn trên 1 trang, không phân trang nên gọi hàm này ngay khi load trang
async function GetOrder() {
    const searchParams = new URLSearchParams();
    let query;
    if (GetValueFromUrlName("id") == null) {
        query = "/Customer/GetAllOrder";
    }
    else {
        searchParams.append("id", GetValueFromUrlName("id"));
        query = "/Customer/GetOrderFromId";
    }
    ShowCircleLoader();

    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();
    let resObj = JSON.parse(responseDB.responseText);
    if (resObj.State != 0) {
        await CreateMustClickOkModal("Có lỗi xảy ra. Vui lòng thử lại sau.", null);
        listOrder = null;
        return;
    }
    listOrder = resObj.myJson;

    ShowHideSomethingFirst();

    // Hiển thị đơn hàng
    ShowResult(document.getElementsByClassName("nvsflAB87")[0]);
}

async function SearchOrderForAnonymous() {
    let sdtNameForSearch = document.getElementById("input_sdt_name_for_search");
    if (isEmptyOrSpaces(sdtNameForSearch.value)) {
        CreateMustClickOkModal("Thông tin tìm kiếm chưa đúng. Vui lòng nhập tên hoặc vài số cuối SĐT người nhận.", null);
        sdtNameForSearch.value = "";
        sdtNameForSearch.focus();
        return;
    }
    else if (sdtNameForSearch.value.length < 3) {
        CreateMustClickOkModal("Nhập ít nhất 3 ký tự");
        sdtNameForSearch.focus();
        return;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("sdtNameForSearch", sdtNameForSearch.value);
    let query = "/Customer/SearchOrderForAnonymous";
    ShowCircleLoader();

    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();
    let resObj = JSON.parse(responseDB.responseText);
    if (resObj.State != 0) {
        await CreateMustClickOkModal("Có lỗi xảy ra. Vui lòng thử lại sau.", null);
        listOrder = null;
        return;
    }

    listOrder = resObj.myJson;

    // Hiển thị đơn hàng
    ShowResult(document.getElementsByClassName("nvsflAB87")[0]);
}

function ShowHideSomethingFirst() {
    if (GetValueFromUrlName("id") != null) {
        document.getElementsByClassName("status-container")[0].remove();
    }
    else {
        if (!CheckAnonymousCustomer()) {
            document.getElementById("search_order_for_anonymous").remove();
        }
        else {
            document.getElementById("search_order_for_anonymous").style.display = "initial";
        }
    }
}

function GetFormattedDate(date) {
    var year = date.getFullYear();

    var month = (1 + date.getMonth()).toString();
    month = month.length > 1 ? month : '0' + month;

    var day = date.getDate().toString();
    day = day.length > 1 ? day : '0' + day;

    return day + '/' + month + '/' + year;
}

function ShowResult(ele) {
    let status = parseInt(ele.getAttribute("data-status"));
    if (GetValueFromUrlName("id") == null) {
        // Bỏ màu bottom-border, chữ của button cũ
        let lsBtn = document.getElementsByClassName("nvsflAB87");
        for (let i = 0; i < lsBtn.length; i++) {
            if (lsBtn[i] != ele) {
                lsBtn[i].style.color = "initial";
                lsBtn[i].style.borderBottom = "none";
            }
            else {
                lsBtn[i].style.color = "#ee4d2d";
                lsBtn[i].style.borderBottom = "2px solid #ee4d2d";
            }
        }
    }

    // Làm trống
    let container = document.getElementsByClassName("result-content-container")[0];
    container.innerHTML = "";

    // Hiển thị order
    if (listOrder == null) {
        return;
    }

    let sample = document.getElementsByClassName("sample-order")[0];
    let sampleItem = document.getElementsByClassName("sample-item-container")[0];
    let sampleAdress = document.getElementsByClassName("sample-address-container")[0];
    let samplePayment = document.getElementsByClassName("sample-payment")[0];

    let length = listOrder.length;
    // Duyệt từ cuối danh sách để lấy theo thời gian gần nhất hiển thị hàng trên ở web
    for (let i = length - 1; i >= 0; i--) {
        let orderObj = listOrder[i];
        if (status != 10 && orderObj.lsOrderTrack[orderObj.lsOrderTrack.length - 1].status != status)
            continue;

        let clone = sample.cloneNode(true);
        clone.style.display = "block";
        // Trạng thái đơn
        clone.getElementsByClassName("fxASnxvd")[0].innerHTML =
            orderObj.lsOrderTrack[orderObj.lsOrderTrack.length - 1].strStatus;

        // Thông tin nhận hàng
        let cloneAddress = sampleAdress.cloneNode(true);
        cloneAddress.style.display = "initial";

        let d = new Date(orderObj.time);
        cloneAddress.getElementsByClassName("address-date")[0].innerHTML = GetFormattedDate(d);

        cloneAddress.getElementsByClassName("address-name-phone")[0].innerHTML =
            orderObj.address.name + ", " + orderObj.address.phone;

        // Là khách vãng lai
        if (CheckAnonymousCustomer()) {
            cloneAddress.getElementsByClassName("address-address")[0].innerHTML =
                orderObj.address.province + ", " + orderObj.address.district + ", ******";
        }
        else {
            cloneAddress.getElementsByClassName("address-address")[0].innerHTML =
            orderObj.address.detail + ", " + orderObj.address.province + ", "
                + orderObj.address.district + ", " + orderObj.address.subdistrict;
        }
        clone.getElementsByClassName("order-address-container")[0].append(cloneAddress);

        let containerItem = clone.getElementsByClassName("order-item-container")[0];
        let containerPayment = clone.getElementsByClassName("payment-infor-container")[0];
        let num = orderObj.lsOrderDetail.length;
        for (let j = 0; j < num; j++) {
            let itemObj = orderObj.lsOrderDetail[j];
            let cloneItem = sampleItem.cloneNode(true);
            cloneItem.style.display = "flex";
            // Icon
            cloneItem.getElementsByClassName("model-icon")[0].src = itemObj.imageSrc;

            // Tên item
            cloneItem.getElementsByClassName("ffZM87hf")[0].innerHTML = itemObj.itemName;
            // Tên phân loại
            cloneItem.getElementsByClassName("ffZM87hf")[1].innerHTML = itemObj.modelName;
            // Số lượng
            cloneItem.getElementsByClassName("ffZM87hf")[2].innerHTML = "x" + itemObj.quantity;

            // Tiền giá bìa
            cloneItem.getElementsByClassName("j2En5csA")[0].innerHTML =
                ConvertMoneyToTextWithIcon(itemObj.bookCoverPrice * itemObj.quantity);
            // Thành tiền
            cloneItem.getElementsByClassName("ndAKDn90")[0].innerHTML =
                ConvertMoneyToTextWithIcon(itemObj.price * itemObj.quantity);

            containerItem.append(cloneItem);
        }

        num = orderObj.lsOrderPay.length;
        for (let j = 0; j < num; j++) {
            let paymentObj = orderObj.lsOrderPay[j];

            let clonePayment = samplePayment.cloneNode(true);
            clonePayment.style.display = "flex";

            if (paymentObj.type == 10) {
                clonePayment.getElementsByClassName("money-padding")[0].className = "money-padding money-sum";
            }

            clonePayment.getElementsByClassName("money-title")[0].innerHTML = paymentObj.strType;
            clonePayment.getElementsByClassName("money-padding")[0].innerHTML =
                ConvertMoneyToTextWithIcon(paymentObj.value);


            containerPayment.append(clonePayment);

            // Tính tổng tiền, các loại phí, khuyến mại
        }
        container.append(clone);
    }
}
