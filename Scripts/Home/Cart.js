// Khi tích/bỏ tích chọn mua sẽ không được cập nhật vào real cart cookie,
// chỉ xóa sản phẩm, thay đổi số lượng chọn là được cập nhật nếu cookie có đối ứng
// listCartCookieObje: dùng xác định sản phẩm được mua khi click mua hàng
let listCartCookieObject; // list cart cookie object server trả về

function CreateSelectedModel() {
    // Lấy mẫu
    let sample = document.getElementsByClassName("sample-selected-model")[0].firstElementChild;
    let containerModel = document.getElementsByClassName("contianer-selected-model")[0];

    let length = listCartCookieObject.length;

    // Sinh bản sao
    for (let i = 0; i < length; i++) {
        let obj = listCartCookieObject[i];
        let clone = sample.cloneNode(true);
        clone.setAttribute("data-model-id", obj.sanPhamId.toString());
        // Cập nhật dữ liệu bản sao
        clone.getElementsByClassName("model-checkbox-input")[0].checked = Boolean(obj.real);
        clone.getElementsByClassName("WanNdG")[0].src = Get320VersionOfImageSrc(GetSanPhamMediaUrl(obj.sanPhamBasicInfo.Id, obj.sanPhamBasicInfo.CoverImageFileName));
        clone.getElementsByClassName("WanNdG")[0].alt = obj.sanPhamBasicInfo.Name;

        let listA = clone.getElementsByClassName("item-url");
        listA[0].title = obj.sanPhamBasicInfo.Name;
        listA[0].href = "/san-pham/" + GenerateSlug(obj.sanPhamBasicInfo.Name) + "-" + obj.sanPhamId;

        listA[1].title = obj.sanPhamBasicInfo.Name;
        listA[1].href = "/san-pham/" + GenerateSlug(obj.sanPhamBasicInfo.Name) + "-" + obj.sanPhamId;

        clone.getElementsByClassName("JB57cn")[0].innerHTML = obj.sanPhamBasicInfo.Name;
        //clone.getElementsByClassName("dcPz7Y")[0].innerHTML = obj.modelName;

        if (obj.sanPhamBasicInfo.BookCoverPrice > obj.sanPhamBasicInfo.SalePrice) {
            clone.getElementsByClassName("vWt6ZL")[0].style.display = "";
            clone.getElementsByClassName("vWt6ZL")[0].innerHTML =
                ConvertMoneyToTextWithIcon(obj.sanPhamBasicInfo.BookCoverPrice);
        }
        else {
            clone.getElementsByClassName("vWt6ZL")[0].style.display = "none";
        }
        clone.getElementsByClassName("M-AAFK")[0].innerHTML =
            ConvertMoneyToTextWithIcon(obj.sanPhamBasicInfo.SalePrice);

        clone.getElementsByClassName("v3H4Zf")[0].value = obj.quantity;

        clone.getElementsByClassName("max-quantity")[0].innerHTML = obj.sanPhamBasicInfo.Quantity + " sản phẩm có sẵn";

        clone.getElementsByClassName("ofQLuG")[0].innerHTML =
            ConvertMoneyToTextWithIcon(obj.quantity * obj.sanPhamBasicInfo.SalePrice);

        containerModel.appendChild(clone);
    }

    ShowSumMoney();
    ShowCheckboxAll();
}

async function LoadCart() {
    let responseText = await CartPageLoadCart();
    if (responseText != "null") {
        listCartCookieObject = JSON.parse(responseText);
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
        if (obj.quantity > obj.sanPhamBasicInfo.Quantity) {
            obj.quantity = obj.sanPhamBasicInfo.Quantity;
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
            sumMoney = sumMoney + listCartCookieObject[i].quantity * listCartCookieObject[i].sanPhamBasicInfo.SalePrice;
            count++;
        }
    }
    document.getElementsByClassName("A-CcKC-SanPham")[0].innerHTML = "(" + count + " sản phẩm):"
    document.getElementsByClassName("WC0us-")[0].innerHTML =
        ConvertMoneyToTextWithIcon(sumMoney);
}

// Hiển thị tiền thanh toán 1 sản phẩm
function ShowMoney(model) {
    let id = parseInt(model.getAttribute("data-model-id"));
    let obj = listCartCookieObject.find(item => item.sanPhamId === id);
    model.getElementsByClassName("ofQLuG")[0].innerHTML =
        ConvertMoneyToTextWithIcon(obj.quantity * obj.sanPhamBasicInfo.SalePrice);
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

// Khi click check box model, cập nhật vào listCartCookieObject
function ClickModelCheckBox(element) {
    let model = element.closest('.selected-model');
    isChecked = element.checked;

    let id = parseInt(model.getAttribute("data-model-id"));
    let obj = listCartCookieObject.find(item => item.sanPhamId === id);
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
    let obj = listCartCookieObject.find(item => item.sanPhamId === id);
    return obj.sanPhamBasicInfo.Quantity;
}

async function UpdateSanPhamQuantityOnCart(sanPhamId, quantity) {
    if (DEBUG) {
        console.log("UpdateSanPhamQuantityOnCart call");
    }
    const searchParams = new URLSearchParams();
    searchParams.append("sanPhamId", sanPhamId);
    searchParams.append("quantity", quantity);
    let query = "/Home/UpdateSanPhamQuantityOnCart";

    return await RequestHttpPostPromise(searchParams, query);
}

// Cập nhật listCartCookieObject, cart cookie khi thay đổi số lượng muốn mua
// element là input tag
async function UpdateWhenChangeInputQuantity(model, quan) {
    let id = parseInt(model.getAttribute("data-model-id"));
    let obj = listCartCookieObject.find(item => item.sanPhamId === id);
    obj.quantity = quan;

    // Là khách vãng lai
    if (CheckAnonymousCustomer()) {
        if (typeof CartManager !== 'undefined') {
            guestCart = CartManager.updateQuantity(id, quan);
        }
        // Trả về định dạng giống truy vấn httpPost
        return GetEasyPromise();
    }
    else {// Khách đăng nhập
        return await UpdateSanPhamQuantityOnCart(id, quan);
    }
}

async function ValidateInput(element) {
    let model = element.closest('.selected-model');
    let newInput = element.value;
    let iInput = 1;
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

    let res = await UpdateWhenChangeInputQuantity(model, iInput);

    let resObj = JSON.parse(res.responseText);
    if (resObj.State != 0) {
        await CreateMustClickOkModal("Có lỗi xảy ra. Vui lòng thử lại sau.", null);
        return;
    }

    ShowSumMoney();
    ShowMoney(model);
}

// element là button tăng / giảm 1 số lượng đặt hàng
async function Decrease(element) {
    let eInput = element.nextElementSibling;
    let iInput = ConvertToInt(eInput.value);
    if (iInput > 1) {
        iInput = iInput - 1;
        eInput.value = iInput.toString();
    }

    let model = element.closest('.selected-model');

    let res = await UpdateWhenChangeInputQuantity(model, iInput);
    let resObj = JSON.parse(res.responseText);
    if (resObj.State != 0) {
        await CreateMustClickOkModal("Có lỗi xảy ra. Vui lòng thử lại sau.", null);
        return;
    }

    ShowSumMoney();
    ShowMoney(model);
}

// element là button tăng / giảm 1 số lượng đặt hàng
async function Increase(element) {
    let eInput = element.previousElementSibling;
    let iInput = ConvertToInt(eInput.value);

    let model = element.closest('.selected-model');
    let maxQuantity = GetMaxQuantityInputInCartPage(model);

    if (iInput < maxQuantity) {
        iInput = iInput + 1;
        eInput.value = iInput.toString();
    }
    else {
        await CreateMustClickOkModal("Cửa hàng chỉ còn " + maxQuantity + " sản phẩm.", null);
    }

    let res = await UpdateWhenChangeInputQuantity(model, iInput);
    let resObj = JSON.parse(res.responseText);
    if (resObj.State != 0) {
        await CreateMustClickOkModal("Có lỗi xảy ra. Vui lòng thử lại sau.", null);
        return;
    }

    ShowSumMoney();
    ShowMoney(model);
}

async function DeleteSanPhamOnCart(sanPhamId) {
    const searchParams = new URLSearchParams();
    searchParams.append("sanPhamId", sanPhamId);
    let query = "/Home/DeleteSanPhamOnCart";

    return await RequestHttpPostPromise(searchParams, query);
}

// element là button xóa
async function DeleteSanPhamOnCartElement(element) {
    let model = element.closest('.selected-model');
    let id = parseInt(model.getAttribute("data-model-id"));
    let length = listCartCookieObject.length;
    for (let i = 0; i < length; i++) {
        if (listCartCookieObject[i].sanPhamId == id) {
            // Là khách vãng lai
            if (CheckAnonymousCustomer()) {
                if (typeof CartManager !== 'undefined') {
                    guestCart = CartManager.removeFromCart(id);
                }
            }
            else {// Khách đăng nhập
                let res = await DeleteSanPhamOnCart(id);
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

window.addEventListener('DOMContentLoaded',async function () {
    if (DEBUG) {
        console.log("window.innerWidth: " + window.innerWidth)
    }
    await LoadCart();
})
