// Khi tích/bỏ tích chọn mua sẽ không được cập nhật vào real cart cookie,
// chỉ xóa sản phẩm, thay đổi số lượng chọn là được cập nhật nếu cookie có đối ứng
// listCartCookieObje: dùng xác định sản phẩm được mua khi click mua hàng
let listCartCookieObject; // list cart cookie object server trả về

ChangeLayoutForSmallScreen();

LoadCart();

// Thay đổi hiển thị khi hiển thị trên màn hình điện thoại
function ChangeLayoutForSmallScreen() {
    if (scrWidth >= 800)
        return;

    // Bỏ padding
    document.getElementsByClassName("main-container")[0].style.padding = "0px";
    document.getElementsByClassName("BjIo5w")[0].style.padding = "0px";
    document.getElementsByClassName("BjIo5w")[0].style.marginRight = "0px";
    document.getElementsByClassName("BjIo5w")[0].style.marginLeft = "0px";

    document.getElementsByClassName("contianer-selected-model")[0].style.padding = "0px";
    document.getElementsByClassName("contianer-selected-model")[0].style.marginRight = "0px";
    document.getElementsByClassName("contianer-selected-model")[0].style.marginLeft = "0px";


    document.getElementsByClassName("s1Gxkq")[0].style.paddingRight = "0px";
    document.getElementsByClassName("s1Gxkq")[0].style.paddingLeft = "0px";
    document.getElementsByClassName("s1Gxkq")[0].style.marginRight = "0px";
    document.getElementsByClassName("s1Gxkq")[0].style.marginLeft = "0px";

    // Ẩn cột sản phẩm hàng tiêu đề
    document.getElementsByClassName("checkbox-name-header")[0].style.display = "none";
    document.getElementsByClassName("price-quantity-sum-delete-header")[0].style.width = "100%";

    // Chia row thành 2 hàng
    document.getElementsByClassName("zoXdNN")[0].style.display = "block";
    document.getElementsByClassName("checkbox-name")[0].style.width = "100%";
    document.getElementsByClassName("price-quantity-sum-delete")[0].style.width = "100%";
    document.getElementsByClassName("price-quantity-sum-delete")[0].style.marginTop = "10px";

    // Set chiều rộng nút mua hàng về auto
    document.getElementsByClassName("shopee-button-solid")[0].style.width = "auto";
}

function CreateSelectedModel() {
    // Lấy mẫu
    let sample = document.getElementsByClassName("sample-selected-model")[0].firstElementChild;
    let containerModel = document.getElementsByClassName("contianer-selected-model")[0];

    let length = listCartCookieObject.length;

    // Sinh bản sao
    for (let i = 0; i < length; i++) {
        let obj = listCartCookieObject[i];
        let clone = sample.cloneNode(true);
        clone.setAttribute("data-model-id", obj.id.toString());
        // Cập nhật dữ liệu bản sao
        clone.getElementsByClassName("model-checkbox-input")[0].checked = Boolean(obj.real);
        clone.getElementsByClassName("WanNdG")[0].src = Get320VersionOfImageSrc(obj.imageSrc);

        let listA = clone.getElementsByClassName("item-url");
        listA[0].title = obj.itemName;
        listA[0].href = "/Item/" + GenerateSlug(obj.itemName) + "-" + obj.itemId;

        listA[1].title = obj.itemName;
        listA[1].href = "/Item/" + GenerateSlug(obj.itemName) + "-" + obj.itemId;

        clone.getElementsByClassName("JB57cn")[0].innerHTML = obj.itemName;
        clone.getElementsByClassName("dcPz7Y")[0].innerHTML = obj.modelName;

        clone.getElementsByClassName("vWt6ZL")[0].innerHTML =
            ConvertMoneyToTextWithIcon(obj.bookCoverPrice);
        clone.getElementsByClassName("M-AAFK")[1].innerHTML =
            ConvertMoneyToTextWithIcon(obj.price);

        clone.getElementsByClassName("v3H4Zf")[0].value = obj.q;

        clone.getElementsByClassName("max-quantity")[0].innerHTML = obj.quantity + " sản phẩm có sẵn";

        clone.getElementsByClassName("ofQLuG")[0].innerHTML =
            ConvertMoneyToTextWithIcon(obj.q * obj.price);

        containerModel.appendChild(clone);
    }

    ShowSumMoney();
    ShowCheckboxAll();
}

async function LoadCart() {
    let responseDB = await CartPageLoadCart();
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
    // Có những sản phẩm số lượng trong kho nhỏ hơn số lượng khách đã chọn, cập nhật lại cookie và list obj
    let length = listCartCookieObject.length;

    for (let i = 0; i < length; i++) {
        let obj = listCartCookieObject[i];
        if (obj.q > obj.quantity) {
            obj.q = obj.quantity;
        }
    }

    CreateSelectedModel();
}

// Hiển thị tổng tiền thanh toán
function ShowSumMoney() {
    let length = listCartCookieObject.length;
    let count = 0;
    let sumMoney = 0;
    for (let i = 0; i < length; i++) {
        if (listCartCookieObject[i].real == 1) {
            sumMoney = sumMoney + listCartCookieObject[i].q * listCartCookieObject[i].price;
            count++;
        }
    }
    document.getElementsByClassName("A-CcKC")[0].innerHTML = "Tổng thanh toán (" + count + " Sản phẩm):"
    document.getElementsByClassName("WC0us-")[0].innerHTML =
        ConvertMoneyToTextWithIcon(sumMoney);
}

// Hiển thị tiền thanh toán 1 sản phẩm
function ShowMoney(model) {
    let id = parseInt(model.getAttribute("data-model-id"));
    let obj = GetObjFromListAndId(id, listCartCookieObject);
    model.getElementsByClassName("ofQLuG")[0].innerHTML =
        ConvertMoneyToTextWithIcon(obj.q * obj.price);
}

// Hiển thị checkbox tất cả
function ShowCheckboxAll() {
    document.getElementsByClassName("iGlIrs")[0].innerHTML = "Chọn Tất Cả (" + listCartCookieObject.length + ")";

    // Nếu tất cả sản phẩm được tích
    let isAll = true;
    let list = document.getElementsByClassName("contianer-selected-model")[0].children;
    for (let i = 0; i < list.length; i++) {
        if (list[i].getElementsByClassName("model-checkbox-input")[0].checked == false) {
            isAll = false;
            break;
        }
    }
    document.getElementsByClassName("all-model-checkbox-input")[0].checked = isAll;
}

function ClickModelCheckBox(element) {
    let model = element.parentElement.parentElement.parentElement;
    UpdateModelCheckbox(model, element.checked)
}

// Khi click check box model, cập nhật vào listCartCookieObject
function UpdateModelCheckbox(model, isChecked) {
    let id = parseInt(model.getAttribute("data-model-id"));
    let obj = GetObjFromListAndId(id, listCartCookieObject);
    if (isChecked) {
        obj.real = 1;
    } else {
        obj.real = 0;
    }

    ShowCheckboxAll();
    ShowSumMoney();
}

function ClickAllModelCheckBox(element) {
    let isChecked = element.checked;

    let listModel = document.getElementsByClassName("contianer-selected-model")[0].children;
    for (let i = 0; i < listModel.length; i++) {
        listModel[i].getElementsByClassName("model-checkbox-input")[0].checked = isChecked;

        if (isChecked) {
            listCartCookieObject[i].real = 1;
        } else {
            listCartCookieObject[i].real = 0;
        }
    }

    ShowSumMoney();
}

// Lấy số lượng max khách hàng có thể chọn
function GetMaxQuantityInputInCartPage(model) {
    let id = parseInt(model.getAttribute("data-model-id"));
    let obj = GetObjFromListAndId(id, listCartCookieObject);
    return obj.quantity;
}

async function UpdateCartQuantity(modelId, quantity) {
    const searchParams = new URLSearchParams();
    searchParams.append("modelId", modelId);
    searchParams.append("quantity", quantity);
    let query = "/Home/UpdateCartQuantity";

    return await RequestHttpPostPromise(searchParams, query);
}

// Cập nhật listCartCookieObject, cart cookie khi thay đổi số lượng muốn mua
// element là input tag
async function UpdateWhenChangeInputQuantity(model, element) {
    let id = parseInt(model.getAttribute("data-model-id"));
    let obj = GetObjFromListAndId(id, listCartCookieObject);
    obj.q = ConvertToInt(element.value);

    // Là khách vãng lai
    if (CheckAnonymousCustomer()) {
        UpdateQuantityOfCookie(obj);
        // Trả về định dạng giống truy vấn httpPost
        return GetEasyPromise();
    }
    else {// Khách đăng nhập
        return await UpdateCartQuantity(id, ConvertToInt(element.value));
    }
}

async function ValidateInput(element) {
    let model = element.parentElement.parentElement.parentElement.parentElement;
    let newInput = element.value;
    let iInput = 0;
    if (IsNumeric(newInput)) {
        iInput = ConvertToInt(newInput);
        let maxQuantity = GetMaxQuantityInputInCartPage(model);
        if (iInput === 0) {
            iInput = 1;
            if (maxQuantity == 0) {
                iInput = 0;
            }
        }

        if (iInput > maxQuantity) {
            iInput = maxQuantity;
            await CreateMustClickOkModal("Cửa hàng chỉ còn " + maxQuantity + " sản phẩm.", null);
        }
    }
    element.value = iInput.toString();

    let res = await UpdateWhenChangeInputQuantity(model, element);

    let resObj = JSON.parse(res.responseText);
    if (resObj.State != 0) {
        await CreateMustClickOkModal("Có lỗi xảy ra. Vui lòng thử lại sau.", null);
        return;
    }

    ShowSumMoney();
    ShowMoney(model);
}

// element là button giảm 1 số lượng đặt hàng
async function Decrease(element) {
    let eInput = element.nextElementSibling;
    let iInput = ConvertToInt(eInput.value);
    if (iInput > 1) {
        iInput = iInput - 1;
        eInput.value = iInput.toString();
    }

    let model = element.parentElement.parentElement.parentElement.parentElement;

    let res = await UpdateWhenChangeInputQuantity(model, eInput);
    let resObj = JSON.parse(res.responseText);
    if (resObj.State != 0) {
        await CreateMustClickOkModal("Có lỗi xảy ra. Vui lòng thử lại sau.", null);
        return;
    }

    ShowSumMoney();
    ShowMoney(model);
}

// element là button giảm 1 số lượng đặt hàng
async function Increase(element) {
    let eInput = element.previousElementSibling;
    let iInput = ConvertToInt(eInput.value);

    let model = element.parentElement.parentElement.parentElement.parentElement;
    let maxQuantity = GetMaxQuantityInputInCartPage(model);

    if (iInput < maxQuantity) {
        iInput = iInput + 1;
        eInput.value = iInput.toString();
    }
    else {
        await CreateMustClickOkModal("Cửa hàng chỉ còn " + maxQuantity + " sản phẩm.", null);
    }

    let res = await UpdateWhenChangeInputQuantity(model, eInput);
    let resObj = JSON.parse(res.responseText);
    if (resObj.State != 0) {
        await CreateMustClickOkModal("Có lỗi xảy ra. Vui lòng thử lại sau.", null);
        return;
    }

    ShowSumMoney();
    ShowMoney(model);
}

async function DeleteOneCart(modelId) {
    const searchParams = new URLSearchParams();
    searchParams.append("modelId", modelId);
    let query = "/Home/DeleteOneCart";

    return await RequestHttpPostPromise(searchParams, query);
}

// element là button xóa
async function DeleteModel(element) {
    let model = element.parentElement.parentElement.parentElement;
    let id = parseInt(model.getAttribute("data-model-id"));
    let length = listCartCookieObject.length;
    for (let i = 0; i < length; i++) {
        if (listCartCookieObject[i].id == id) {
            // Là khách vãng lai
            if (CheckAnonymousCustomer()) {
                DeleteOneCartCookie(listCartCookieObject[i]);
            }
            else {// Khách đăng nhập
                let res = await DeleteOneCart(id);
                let resObj = JSON.parse(res.responseText);
                if (resObj.State != 0) {
                    await CreateMustClickOkModal("Có lỗi xảy ra. Vui lòng thử lại sau.", null);
                    return;
                }
            }
            listCartCookieObject.splice(i, 1);
            break;
        }
    }
    model.remove();

    ShowSumMoney();
    ShowCheckboxAll();

    // Cập nhật sản phẩm trong giỏ hàng góc trên trái
    document.getElementsByClassName("cart-count")[0].innerHTML = listCartCookieObject.length;

    // Giỏ hàng trống
    if (listCartCookieObject.length == 0) {
        document.getElementsByClassName("cart-empty")[0].style.display = "block";
        document.getElementsByClassName("main-container")[0].style.display = "none";
        return;
    }
}

async function BuyNow() {
    // Khách vãng lai
    // Chưa sản phẩm / model nào được chọn để mua
    let isSelected = false;
    let length = listCartCookieObject.length;
    for (let i = 0; i < length; i++) {
        if (listCartCookieObject[i].real == 1) {
            isSelected = true;
            break;
        }
    }
    if (!isSelected) {
        await CreateMustClickOkModal("Bạn chưa chọn sản phẩm nào để mua.", null)
        return;
    }
    // Tạo string dạng cookie của những sản phẩm chọn mua
    let listReal = [];
    for (let i = 0; i < length; i++) {
        if (listCartCookieObject[i].real == 1) {
            listReal.push(listCartCookieObject[i]);
        }
    }
    let cookie = GetCartCookieFromListCartCookie(listReal);
    let cookieBase64 = window.btoa(cookie);
    location.href = "/Home/Checkout?cart=" + cookieBase64;
}
