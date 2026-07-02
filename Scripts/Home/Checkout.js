let listCartCookieObject; // list cart cookie object server trả về, đã check real == 1 trên server

let listCustomerInforCookieObject; // list customer cookie object server trả về

let currentIndexInforObject = -1; // index địa chỉ nhận hàng hiện tại. Chưa chọn địa chỉ nhận hàng thì dùng thông tin mặc định khi load trang

let currentIndexInforUpdateObject = -1; // index địa chỉ nhận hàng cập nhật thông tin

let funcOfChangeAddress = null; // Hàm xử lý sự kiện click nút change-address-btn

LoadSomething();

// Sau khi load page, cần thêm thông tin
async function LoadSomething() {

    await LoadCustomerInfor(); // Load địa chỉ khách trước để tính phí ship

    LoadCart();
}

function CreateSelectedModel() {
    // Lấy mẫu
    let sample = document.getElementsByClassName("sample-model")[0].firstElementChild;
    let containerModel = document.getElementsByClassName("model-container")[0];

    let length = listCartCookieObject.length;

    // Sinh bản sao
    for (let i = 0; i < length; i++) {
        let obj = listCartCookieObject[i];

        let clone = sample.cloneNode(true);
        clone.setAttribute("data-model-id", obj.id.toString());

        // Cập nhật dữ liệu bản sao
        clone.getElementsByClassName("rTOisL")[0].src = Get320VersionOfImageSrc(obj.imageSrc);
        clone.getElementsByClassName("item-name")[0].innerHTML = obj.itemName;
        clone.getElementsByClassName("model-name")[0].innerHTML = obj.modelName;
        clone.getElementsByClassName("price-model")[0].innerHTML =
            ConvertMoneyToTextWithIcon(obj.price);
        clone.getElementsByClassName("quantity-model")[0].innerHTML = obj.q;
        clone.getElementsByClassName("money-model")[0].innerHTML =
            ConvertMoneyToTextWithIcon(obj.q * obj.price);

        containerModel.appendChild(clone);
    }

    ShowSumMoney();
}

async function LoadCart() {
    let responseDB = await CheckoutPageLoadCart(GetValueFromUrlName("cart"));
    if (responseDB.responseText != "null") {
        listCartCookieObject = JSON.parse(responseDB.responseText);
    }
    else {
        listCartCookieObject = null;
    }

    if (listCartCookieObject == null || listCartCookieObject.length == 0) {
        document.getElementsByClassName("cart-empty")[0].style.display = "block";
        document.getElementsByClassName("main-container")[0].style.display = "none";
        return;
    }
    // Có những sản phẩm số lượng trong kho nhỏ hơn số lượng khách đã chọn,
    // nhưng được tính lại phía server

    CreateSelectedModel();
}

// Hiển thị tổng tiền thanh toán
function ShowSumMoney() {
    let length = listCartCookieObject.length;
    let sumMoney = 0;
    for (let i = 0; i < length; i++) {
        sumMoney = sumMoney + listCartCookieObject[i].q * listCartCookieObject[i].price;
    }

    document.getElementsByClassName("model-money-sum")[0].innerHTML =
        ConvertMoneyToTextWithIcon(sumMoney);

    let shipFee = GetShipFee();
    sumMoney = shipFee + sumMoney;
    document.getElementsByClassName("shipee-fee")[0].innerHTML =
        ConvertMoneyToTextWithIcon(shipFee);
    document.getElementsByClassName("money-sum")[0].innerHTML =
        ConvertMoneyToTextWithIcon(sumMoney);
}

function GetShipFee() {
    let fee = 0;
    if (currentIndexInforObject != -1) {
        let obj = listCustomerInforCookieObject[currentIndexInforObject];
        if (obj.province == HaNoiCity) {
            fee = standardShipFeeInHaNoi;
        }
        else {
            fee = standardShipFeeOutHaNoi;
        }
    }
    return fee;
}

async function GetListAddress() {
    const searchParams = new URLSearchParams();
    let query = "/Customer/GetListAddress";

    return await RequestHttpPostPromise(searchParams, query);
}
// Chưa địa chỉ nào được chọn (lần đầu vào page chưa tạo địa chỉ, chưa chọn địa chỉ mặc định)
function ShowAddressDontSelected() {

}

// Thêm hàm xử lý sự kiện click nút change-address-btn
// Xóa hàm xử lý cũ nếu có
function ChangeAddressClickEvent(element) {
    if (element.getAttribute('listener') === 'true') {
        element.removeEventListener("click", funcOfChangeAddress);
    }
    element.addEventListener('click', funcOfChangeAddress);
}

async function LoadCustomerInfor() {

    if (CheckAnonymousCustomer()) {// Khách vãng lai
        // Lấy từ cookie
        listCustomerInforCookieObject = GetListCustomerInforCookieFromCookie();
    }
    else {
        let res = await GetListAddress();
        if (JSON.parse(res.responseText) == null) {
            await CreateMustClickOkModal("Không lấy được dữ liệu. Vui lòng thử lại sau.", null);
            // Trả về định dạng giống truy vấn httpPost
            return GetEasyPromise();
        }
        else {
            listCustomerInforCookieObject = JSON.parse(res.responseText);
        }
    }

    let isDefaultAdd = false;
    // Tìm địa chỉ mặc định
    for (let i = listCustomerInforCookieObject.length - 1; i >= 0; i--) {
        let obj = listCustomerInforCookieObject[i];
        if (obj.defaultAdd == 1) {
            currentIndexInforObject = i;
            ShowCustomerInforFromObj(obj);
            isDefaultAdd = true;
            break;
        }
    }

    if (!isDefaultAdd) {
        document.getElementsByClassName("address-name-phone")[0].style.display = "none";
        document.getElementsByClassName("address-address")[0].style.display = "none";
        document.getElementsByClassName("address-default")[0].style.display = "none";
        let changeAddressBtnEle = document.getElementsByClassName("change-address-btn")[0];
        changeAddressBtnEle.innerHTML = "Thêm địa chỉ mới";

        // Chưa địa chỉ nào được chọn là mặc định
        // Hiển thị danh sách địa chỉ trước

        if (funcOfChangeAddress !== null) {
            changeAddressBtnEle.removeEventListener("click", funcOfChangeAddress);
        }

        if (listCustomerInforCookieObject.length > 0) {
            funcOfChangeAddress = function () { ShowListCustomerInforModal(); };
        }
        else {
            funcOfChangeAddress = function () { ShowCustomerInforModal(true, false, null); };
        }
        changeAddressBtnEle.addEventListener("click", funcOfChangeAddress);
    }
    // Trả về định dạng giống truy vấn httpPost
    return GetEasyPromise();
}

// Từ obj hiển thị tên, phone, địa chỉ lên checkout page
function ShowCustomerInforFromObj(obj) {
    // Chưa chọn địa chỉ nhận hàng
    if (currentIndexInforObject == -1) {
        return;
    }

    document.getElementsByClassName("address-name-phone")[0].style.display = "block";
    document.getElementsByClassName("address-address")[0].style.display = "block";

    document.getElementsByClassName("address-name-phone")[0].innerHTML = obj.name + ", " + obj.phone;
    document.getElementsByClassName("address-address")[0].innerHTML =
        obj.detail + ", " + obj.province + ", " + obj.district + ", " + obj.subdistrict;
    if (obj.defaultAdd) {
        document.getElementsByClassName("address-default")[0].style.display = "block";
    }
    else {
        document.getElementsByClassName("address-default")[0].style.display = "none";
    }
    let changeAddressBtnEle = document.getElementsByClassName("change-address-btn")[0];
    changeAddressBtnEle.innerHTML = "Thay đổi";
    if (funcOfChangeAddress !== null) {
        changeAddressBtnEle.removeEventListener("click", funcOfChangeAddress);
    }
    funcOfChangeAddress = function () { ShowListCustomerInforModal(); };
    changeAddressBtnEle.addEventListener("click", funcOfChangeAddress);
}

function ShowListCustomerInforModal() {
    document.getElementById("modal-list-customer-infor").style.display = "block";

    // Hiển thị danh sách địa chỉ
    // Lấy mẫu
    let sample = document.getElementsByClassName("sample-customer-infor-container")[0];
    let container = document.getElementsByClassName("list-customer-infor-container")[0];

    let length = listCustomerInforCookieObject.length;

    for (let i = 0; i < length; i++) {
        let obj = listCustomerInforCookieObject[i];
        let clone = sample.cloneNode(true);
        clone.style.display = "flex";
        clone.setAttribute("data-index", i.toString());

        if (currentIndexInforObject == i) {
            clone.getElementsByClassName("address-radio")[0].checked = true;
        }

        clone.getElementsByClassName("name")[0].innerHTML = obj.name;
        clone.getElementsByClassName("phone")[0].innerHTML = obj.phone;

        clone.getElementsByClassName("detail")[0].innerHTML = obj.detail;
        clone.getElementsByClassName("province-district-subdistrict")[0].innerHTML =
            obj.subdistrict + ", " + obj.district + ", " + obj.province;

        if (!obj.defaultAdd) {
            clone.getElementsByClassName("default-address")[0].style.display = "none";
        }
        container.appendChild(clone);
    }
}

function ReturnCustomerInforModal() {
    RefreshModalCustomerInfor();
    currentIndexInforUpdateObject = -1;
}

async function FinishCustomerInforModal() {
    if (!ValidCustomerInforInput()) {
        return;
    }

    let obj = CreateCookieValueFromInput();
    if (currentIndexInforUpdateObject != -1) {// Cập nhật thông tin vào object
        obj.id = listCustomerInforCookieObject[currentIndexInforUpdateObject].id;
        listCustomerInforCookieObject[currentIndexInforUpdateObject] = obj;
        //Kiểm tra có đặt mặc định
        if (obj.defaultAdd) {
            // Bỏ mặc định cũ nếu có
            for (let i = listCustomerInforCookieObject.length - 1; i >= 0; i--) {
                if (listCustomerInforCookieObject[i].defaultAdd
                    && i != currentIndexInforUpdateObject) {
                    listCustomerInforCookieObject[i].defaultAdd = 0;
                }
            }
        }

        // Chọn địa chỉ vừa chỉnh sửa làm địa chỉ nhận hàng
        currentIndexInforObject = currentIndexInforUpdateObject;
    }
    else { // Thêm mới
        //Kiểm tra có đặt mặc đinh
        if (obj.defaultAdd) {
            // Bỏ mặc định cũ nếu có
            for (let i = listCustomerInforCookieObject.length - 1; i >= 0; i--) {
                if (listCustomerInforCookieObject[i].defaultAdd) {
                    listCustomerInforCookieObject[i].defaultAdd = 0;
                }
            }
        }
        listCustomerInforCookieObject.push(obj);
        // Chọn địa chỉ vừa thêm mới làm địa chỉ nhận hàng
        currentIndexInforObject = listCustomerInforCookieObject.length - 1;
    }

    if (CheckAnonymousCustomer()) {// Khách vãng lai
        SetCartCookieFromListCustomerInforCookieObject(listCustomerInforCookieObject);
    }
    else { // Khách đăng nhập
        if (currentIndexInforUpdateObject != -1) {// Cập nhật thông tin
            let res = await UpdateAddress(obj);
            let resObj = JSON.parse(res.responseText);
            if (resObj.State != 0) {
                await CreateMustClickOkModal("Có lỗi xảy ra. Vui lòng thử lại sau.", null);
                return;
            }
        }
        else {
            let res = await InsertAddress(obj);
            let resObj = JSON.parse(res.responseText);
            if (resObj.State != 0) {
                await CreateMustClickOkModal("Có lỗi xảy ra. Vui lòng thử lại sau.", null);
                return;
            }

            // Cập nhật lại id cho obj vừa thêm vào list
            listCustomerInforCookieObject[listCustomerInforCookieObject.length - 1].id = resObj.myAnything;
        }
    }

    currentIndexInforUpdateObject = -1;

    RefreshModalCustomerInfor();

    // Nếu bên dưới có modal hiển thị danh sách địa chỉ, ta cập nhật lại
    // Xóa địa chỉ khỏi danh sách
    document.getElementsByClassName("list-customer-infor-container")[0].innerHTML = "";
    ShowListCustomerInforModal();
}

function DestroyListCustomerInforModal() {
    // Xóa địa chỉ khỏi danh sách
    document.getElementsByClassName("list-customer-infor-container")[0].innerHTML = "";
    document.getElementById("modal-list-customer-infor").style.display = "none";
}

// Ý nghĩa: không thay đổi lựa chọn nhận hàng,
// nhưng lựa chọn cũ có thể đã cập nhật nên cần hiển thị ra checkout page
function DontConfirmListCustomerInforModal() {
    DestroyListCustomerInforModal();

    // Cập nhật thông tin lựa chọn cũ
    ShowCustomerInforFromObj(listCustomerInforCookieObject[currentIndexInforObject]);

    // Tính lại tiền
    ShowSumMoney();
}

// Xác nhận dùng thông tin nào nhận hàng
function ConfirmListCustomerInforModal() {
    let list = document.getElementsByClassName("list-customer-infor-container")[0].children;
    let length = list.length;
    for (let i = 0; i < length; i++) {
        let ele = list[i];

        if (ele.getElementsByClassName("address-radio")[0].checked) {
            currentIndexInforObject = parseInt(ele.getAttribute("data-index"));
            ShowCustomerInforFromObj(listCustomerInforCookieObject[currentIndexInforObject]);
            break;
        }
    }
    DestroyListCustomerInforModal();

    // Tính lại tiền
    ShowSumMoney();
}

function UpdateCustomerInfor(ele) {
    currentIndexInforUpdateObject = parseInt(ele.parentElement.parentElement.getAttribute("data-index"));
    ShowCustomerInforModal(false, true, listCustomerInforCookieObject[currentIndexInforUpdateObject]);
}

function objOrderPay(type, value) {
    this.type = type;
    this.value = value;
}

// Tạo list thanh toán: tổng tiền hàng, phí ship, tổng thanh toán
/// 0: Tổng tiền hàng
/// 1: Phí ship
/// 2: Khuyến mại khác
/// 3: Tổng thanh toán = Tổng tiền hàng + Phí ship - Khuyến mại khác
function GetListOrderPay() {
    let length = listCartCookieObject.length;
    let sumMoney = 0;
    for (let i = 0; i < length; i++) {
        sumMoney = sumMoney + listCartCookieObject[i].q * listCartCookieObject[i].price;
    }
    let listOrderPay = [];
    listOrderPay.push(new objOrderPay(0, sumMoney));

    let shipFee = GetShipFee();
    listOrderPay.push(new objOrderPay(1, shipFee));

    sumMoney = shipFee + sumMoney;
    listOrderPay.push(new objOrderPay(3, sumMoney));

    return listOrderPay;
}

// Check lại thông tin đơn hàng bên phía server: mã sản phẩm, số lượng có đủ
// (vì trong thời gian khách chọn có thể số lượng tồn kho thay đổi), tổng tiền hàng, tiền ship.
// Đồng thời gửi thông tin địa chỉ, lời nhắn shop
async function CheckOrderOnSever() {
    const searchParams = new URLSearchParams();
    searchParams.append("cart", JSON.stringify(listCartCookieObject));
    searchParams.append("customerInfor", JSON.stringify(listCustomerInforCookieObject[currentIndexInforObject]));
    let listOrderPay = GetListOrderPay();
    searchParams.append("listOrderPay", JSON.stringify(listOrderPay));
    searchParams.append("noteToShop", document.getElementsByClassName("gQuJxM")[0].value);

    let query = "/Home/CheckOrderOnSever";

    return await RequestHttpPostPromise(searchParams, query);

}

async function Order() {
    if (currentIndexInforObject == -1) {
        await CreateMustClickOkModal("Vui lòng cung cấp thông tin người nhận hàng.", null)
        return;
    }
    ShowCircleLoader();
    let responseDB = await CheckOrderOnSever();
    RemoveCircleLoader();

    let result = JSON.parse(responseDB.responseText);
    if (result == null) {
        await CreateMustClickOkModal("Có lỗi, vui lòng thử lại sau.");
        return;
    }
    if (result.State != 0) {
        await CreateMustClickOkModal(result.Message);
        return;
    }
    // Với khách vãng lai
    // Cập nhật lại cart cookie, xóa những sản phẩm đặt hàng thành công
    let length = listCartCookieObject.length;
    for (let i = length - 1; i >= 0; i--) {
        DeleteOneCartCookie(listCartCookieObject[i]);
    }

    await CreateMustClickOkModal("Đặt hàng thành công. Cảm ơn bạn đã mua hàng tại shop.", function () { location.replace("/"); });
}
