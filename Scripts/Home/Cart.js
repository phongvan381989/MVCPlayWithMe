let listCartObject; // list cart object server trả về
let lengthOfActive; // Số lượng đầu sản phẩm có thể mua = listCartObject.length - sản phẩm hết hàng, ngừng kinh doanh

// Batch debounce state
let pendingUpdates = {}; // { sanPhamId: quantity }
let batchTimer = null;
let isSending = false; // ✅ Flag để prevent concurrent requests
let debounceTime = 2000; // 2 giây debounce

function CreateSelectedModel() {
    // Lấy mẫu
    let sample = document.getElementsByClassName("sample-selected-model")[0].firstElementChild;
    let containerModel = document.getElementsByClassName("contianer-selected-model")[0];
    // if (DEBUG) {
    //     console.log("CreateSelectedModel containerModel: " + containerModel.tagName);
    // }

    let length = listCartObject.length;
    lengthOfActive = length;
    // Sinh bản sao
    for (let i = 0; i < length; i++) {
        let obj = listCartObject[i];
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

        // Hiển thị số lượng tồn kho hoặc "Hết hàng"
        let maxQuantityElement = clone.getElementsByClassName("max-quantity")[0];
        if (obj.sanPhamBasicInfo.Quantity > 0) {
            maxQuantityElement.innerHTML = obj.sanPhamBasicInfo.Quantity + " sản phẩm có sẵn";
            maxQuantityElement.style.color = ""; // Reset màu mặc định
        } else {
            maxQuantityElement.innerHTML = "HẾT HÀNG";
            maxQuantityElement.style.color = "#ee4d2d"; // Màu đỏ

            // Hết hàng → disable +- số lượng
            let quantityContainer = clone.getElementsByClassName("shopee-input-quantity")[0];
            //let checkBox = clone.getElementsByClassName("mcsiKT")[0];

            quantityContainer.classList.add("disabled");
            //checkBox.classList.add("disabled");
            clone.getElementsByClassName("model-checkbox-input")[0].disabled = true;

            lengthOfActive--;

            obj.real = 0; // cập nhật lại nếu
        }

        clone.getElementsByClassName("ofQLuG")[0].innerHTML =
            ConvertMoneyToTextWithIcon(obj.quantity * obj.sanPhamBasicInfo.SalePrice);

        containerModel.appendChild(clone);
    }

    ShowSumMoney();
    ShowCheckboxAll();
}

// Helper: Reset real = 0 trên server (cho logged-in users)
async function RefreshRealOfCartOnServer() {
    try {
        const searchParams = new URLSearchParams();
        searchParams.append("_", Date.now()); // Cache buster
        await RequestHttpPostPromise(searchParams, '/Home/RefreshRealOfCart');
    } catch (error) {
        console.error('❌ Failed to refresh real cart:', error);
    }
}

// Helper: Gửi batch updates lên server (SEQUENTIAL - đợi request trước xong)
async function SendBatchUpdate() {
    if (Object.keys(pendingUpdates).length === 0) {
        return; // Không có gì cần update
    }

    // ✅ Đợi nếu đang gửi request khác
    if (isSending) {
        if (DEBUG) {
            console.log('⏳ Already sending, will retry later...');
        }

        // Schedule lại
        if (batchTimer) {
            clearTimeout(batchTimer);
        }

        batchTimer = setTimeout(async () => {
            batchTimer = null;
            await SendBatchUpdate();
        }, debounceTime);

        return;
    }

    // ✅ Set flag để prevent concurrent
    isSending = true;

    if (DEBUG) {
        console.log('📦 Sending batch update:', pendingUpdates);
    }

    // Snapshot và clear (prevent duplicate sends)
    const toSend = { ...pendingUpdates };
    pendingUpdates = {};

    try {
        const responseText = await PostJSON('/Home/BatchUpdateCartQuantities', toSend);
        const result = JSON.parse(responseText);

        if (result.State !== 0) {
            console.error('❌ Batch update failed:', result.Message);

            // Restore nếu fail
            Object.assign(pendingUpdates, toSend);

            await CreateMustClickOkModal('Không thể cập nhật giỏ hàng. Vui lòng thử lại.', null);
        } else {
            // if (DEBUG) {
            //     console.log('✅ Batch update success:', result.Message);
            // }
        }
    } catch (error) {
        console.error('❌ Batch update error:', error);

        // Restore nếu error
        Object.assign(pendingUpdates, toSend);

        await CreateMustClickOkModal('Có lỗi xảy ra khi cập nhật giỏ hàng. Vui lòng thử lại sau.', null);
    } finally {
        // ✅ Clear flag
        isSending = false;

        // // ✅ Gửi tiếp nếu còn pending (accumulated trong lúc gửi)
        // if (Object.keys(pendingUpdates).length > 0) {
        //     if (DEBUG) {
        //         console.log('📦 Has pending updates, scheduling next batch...');
        //     }

        //     if (batchTimer) {
        //         clearTimeout(batchTimer);
        //     }

        //     batchTimer = setTimeout(async () => {
        //         batchTimer = null;
        //         await SendBatchUpdate();
        //     }, debounceTime);
        // }
    }
}

function ShowCartList() {
    if (DEBUG) {
        console.log('ShowCartList listCartObject: ' + JSON.stringify(listCartObject));
    }
    // Làm mới nội dung
    document.getElementsByClassName("contianer-selected-model")[0].innerHTML = "";
    document.getElementsByClassName("A-CcKC-SanPham")[0].innerHTML = "";
    document.getElementsByClassName("WC0us-")[0].innerHTML = "";

    if (listCartObject == null || listCartObject.length == 0) {
        document.getElementsByClassName("cart-empty")[0].style.display = "block";
        document.getElementsByClassName("main-container")[0].style.display = "none";
        return;
    }

    // Có những sản phẩm số lượng trong kho nhỏ hơn số lượng khách đã chọn, cập nhật lại cookie và list obj
    let length = listCartObject.length;

    for (let i = 0; i < length; i++) {
        let obj = listCartObject[i];
        if (obj.quantity > obj.sanPhamBasicInfo.Quantity) {
            obj.quantity = obj.sanPhamBasicInfo.Quantity;
        }
    }

    CreateSelectedModel();
    document.getElementsByClassName("cart-empty")[0].style.display = "none";
    document.getElementsByClassName("main-container")[0].style.display = "block";
}

function ShowErrorWhenLoadCart(error) {
    console.error('❌ Error in LoadCart:', error);

    // Hiển thị lỗi cho user
    document.getElementsByClassName("cart-empty")[0].style.display = "block";
    document.getElementsByClassName("main-container")[0].style.display = "none";

    CreateMustClickOkModal('Có lỗi khi tải giỏ hàng. Vui lòng thử lại sau.', null);
}

// Helper: Đợi pending batch sync hoàn thành (dùng trước BuyNow)
async function WaitForPendingBatchSync() {
    if (DEBUG) {
        console.log('⏳ Force syncing pending updates before checkout...');
    }

    // ✅ Đợi request đang gửi (nếu có)
    while (isSending) {
        if (DEBUG) {
            console.log('⏳ Waiting for current request to finish...');
        }
        await new Promise(resolve => setTimeout(resolve, 100)); // Poll mỗi 100ms
    }

    // ✅ Đợi pending updates (nếu có)
    if (Object.keys(pendingUpdates).length > 0) {
        // Cancel timer và sync ngay
        if (batchTimer) {
            clearTimeout(batchTimer);
            batchTimer = null;
        }

        await SendBatchUpdate();
    }

    if (DEBUG) {
        console.log('✅ All updates synced!');
    }
}

async function LoadCart() {
    try {
        let responseText = await CartPageLoadCart();

        // Sau khi load cart với dữ liệu đầy đủ, set real = 0 với khách vãng lai và xóa cart với khách đăng nhập
        if (typeof CartManager !== 'undefined') {
            CartManager.setRealZeroOrClear();
        }

        listCartObject = JSON.parse(responseText);
        ShowCartList();
    } catch (error) {
        ShowErrorWhenLoadCart(error);
    } finally {
    }
}

// Khi back từ checkout page, reload cart để tránh dữ liệu cũ và so sánh với cart được load từ bfcache
async function LoadCartBFCache() {
    if (DEBUG) {
        console.log("LoadCartBFCache called");
    }
    try {
        let responseText = await CartPageLoadCart();

        if (typeof CartManager !== 'undefined') {
            CartManager.setRealZeroOrClear();
        }

        let listCartObjectTem = listCartObject; // Lưu lại listCartObject cũ để so sánh
        listCartObject = JSON.parse(responseText);
        // so sánh với listCartObject cũ để tìm sản phẩm nào đang được chọn mua
        for (let i = 0; i < listCartObject.length; i++) {
            let obj = listCartObject[i];
            for (let j = 0; j < listCartObjectTem.length; j++) {
                let objTem = listCartObjectTem[j];
                if (obj.sanPhamId === objTem.sanPhamId && objTem.real === 1) {
                    obj.real = 1;
                    break;
                }
            }
        }

        ShowCartList();
    } catch (error) {
        ShowErrorWhenLoadCart(error);
    }
}


// Hiển thị tổng tiền thanh toán
function ShowSumMoney() {
    let length = listCartObject.length;
    let count = 0;
    let sumMoney = 0;
    for (let i = 0; i < length; i++) {
        if (listCartObject[i].real == 1) {
            sumMoney = sumMoney + listCartObject[i].quantity * listCartObject[i].sanPhamBasicInfo.SalePrice;
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
    let obj = listCartObject.find(item => item.sanPhamId === id);
    model.getElementsByClassName("ofQLuG")[0].innerHTML =
        ConvertMoneyToTextWithIcon(obj.quantity * obj.sanPhamBasicInfo.SalePrice);
}

// Hiển thị checkbox tất cả
function ShowCheckboxAll() {
    document.getElementsByClassName("iGlIrs")[0].innerHTML = "Chọn Tất Cả (" + lengthOfActive + ")";

    // Nếu tất cả sản phẩm được tích
    let isAll = true;
    let listModel = document.getElementsByClassName("contianer-selected-model")[0].children;
    for (let i = 0; i < listModel.length; i++) {
        let checkBoxInput = listModel[i].getElementsByClassName("model-checkbox-input")[0];
        if (checkBoxInput.checked == false && checkBoxInput.disabled == false) {
            isAll = false;
            break;
        }
    }
    document.getElementsByClassName("all-model-checkbox-input")[0].checked = isAll;
}

// Khi click check box model, cập nhật vào listCartCookieObject, localStorage
function ClickModelCheckBox(element) {
    let model = element.closest('.selected-model');
    let isChecked = element.checked;

    let id = parseInt(model.getAttribute("data-model-id"));
    let obj = listCartObject.find(item => item.sanPhamId === id);
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
    let real = isChecked ? 1 : 0;
    let listModel = document.getElementsByClassName("contianer-selected-model")[0].children;
    for (let i = 0; i < listModel.length; i++) {
        let checkBoxInput = listModel[i].getElementsByClassName("model-checkbox-input")[0];
        if (checkBoxInput.disabled == false) {
            checkBoxInput.checked = isChecked;
            listCartObject[i].real = real;
        }
    }

    ShowSumMoney();
}

// Lấy số lượng max khách hàng có thể chọn
function GetMaxQuantityInputInCartPage(model) {
    let id = parseInt(model.getAttribute("data-model-id"));
    let obj = listCartObject.find(item => item.sanPhamId === id);
    return obj.sanPhamBasicInfo.Quantity;
}

// // Cập nhật số lượng khi +/- 1 hoặc thay đổi input số lượng
// // Cập nhật localStorage và listCartCookieObject
// // Khách đăng nhập vẫn sẽ có cart ở localStorage
// async function UpdateSanPhamQuantityOnCart(sanPhamId, quantity) {
//     // if (DEBUG) {
//     //     console.log("UpdateSanPhamQuantityOnCart call");
//     // }
//     const searchParams = new URLSearchParams();
//     searchParams.append("sanPhamId", sanPhamId);
//     searchParams.append("quantity", quantity);
//     let query = "/Home/UpdateSanPhamQuantityOnCart";

//     return await RequestHttpPostPromise(searchParams, query);
// }

// Cập nhật listCartCookieObject, localStorage khi thay đổi số lượng muốn mua
// Batch debounce: Thu thập changes và gửi 1 request duy nhất
// element là input tag
function UpdateWhenChangeInputQuantity(model, quan) {
    let id = parseInt(model.getAttribute("data-model-id"));
    // let obj = listCartObject.find(item => item.sanPhamId === id);

    // // ✅ STEP 1: Update memory ngay (instant UI)
    // obj.quantity = quan;

    // ✅ STEP 2: Update localStorage (cho cả guest và logged-in)
    if (typeof CartManager !== 'undefined') {
        // let real = 0;
        // if (model.getElementsByClassName("model-checkbox-input")[0].checked) {
        //     real = 1;
        // }
        guestCart = CartManager.updateQuantity(id, quan);
    }

    // ✅ STEP 3: Logged-in users - Batch debounce sync to server
    if (!CheckAnonymousCustomer()) {
        // Add to pending batch
        pendingUpdates[id] = quan;

        // Cancel previous timer
        if (batchTimer) {
            clearTimeout(batchTimer);
        }

        // Schedule batch send (2 giây debounce)
        batchTimer = setTimeout(async () => {
            batchTimer = null; // ✅ Clear ngay khi timer fire
            await SendBatchUpdate();
        }, debounceTime); // 2 seconds debounce
    }
}

function ValidateInput(element) {
    let maxQuantity = GetMaxQuantityInputInCartPage(model);
    if (maxQuantity === 0) {
        CreateMustClickOkModal("Sản phẩm tạm hết hàng.", null);
        element.value = '0';
        return;
    }

    let model = element.closest('.selected-model');
    let newInput = element.value;
    let iInput = 1;

    if (IsNumeric(newInput)) {
        iInput = ConvertToInt(newInput);

        if (iInput === 0) {
            iInput = 1;
        }

        if (iInput > maxQuantity) {
            iInput = maxQuantity;
            CreateMustClickOkModal("Cửa hàng chỉ còn " + maxQuantity + " sản phẩm.", null);
        }
    }

    element.value = iInput.toString();

    UpdateWhenChangeInputQuantity(model, iInput);

    ShowSumMoney();
    ShowMoney(model);
}

// element là button tăng / giảm 1 số lượng đặt hàng
function Decrease(element) {
    let eInput = element.nextElementSibling;
    let iInput = ConvertToInt(eInput.value);
    if (iInput > 1) {
        iInput = iInput - 1;
    }
    else {
        iInput = 1;
    }
    eInput.value = iInput.toString();

    let model = element.closest('.selected-model');

    UpdateWhenChangeInputQuantity(model, iInput);

    ShowSumMoney();
    ShowMoney(model);
}

// element là button tăng / giảm 1 số lượng đặt hàng
function Increase(element) {
    let eInput = element.previousElementSibling;
    let iInput = ConvertToInt(eInput.value);

    let model = element.closest('.selected-model');
    let maxQuantity = GetMaxQuantityInputInCartPage(model);

    if (iInput < maxQuantity) {
        iInput = iInput + 1;
        eInput.value = iInput.toString();
    }
    else {
        CreateMustClickOkModal("Cửa hàng chỉ còn " + maxQuantity + " sản phẩm.", null);
    }

    UpdateWhenChangeInputQuantity(model, iInput);

    ShowSumMoney();
    ShowMoney(model);
}

// element là button xóa
async function DeleteSanPhamOnCartElement(element) {
    let model = element.closest('.selected-model');
    let id = parseInt(model.getAttribute("data-model-id"));
    let length = listCartObject.length;
    if (DEBUG) {
        console.log("DeleteSanPhamOnCartElement called id: " + id);
    }
    for (let i = 0; i < length; i++) {
        if (listCartObject[i].sanPhamId == id) {
            // Là khách vãng lai
            if (CheckAnonymousCustomer()) {
                if (typeof CartManager !== 'undefined') {
                    guestCart = CartManager.removeFromCart(id);
                }
            }
            else {// Khách đăng nhập
                try {
                    ShowCircleLoader();
                    const responseText = await PostJSON('/Home/DeleteSanPhamOnCart', { sanPhamId: id });
                    RemoveCircleLoader();
                    const result = JSON.parse(responseText);

                    if (result.State !== 0) {
                        await CreateMustClickOkModal('Không xóa sản phẩm thành công. Vui lòng thử lại.', null);
                    }
                } catch (error) {
                    RemoveCircleLoader();
                    await CreateMustClickOkModal('Có lỗi xảy ra khi xóa sản phẩm. Vui lòng thử lại sau.', null);
                }
            }
            listCartObject.splice(i, 1);
            break;
        }
    }
    model.remove();

    ShowSumMoney();
    ShowCheckboxAll();

    // Cập nhật sản phẩm trong giỏ hàng góc trên trái
    document.getElementsByClassName("cart-count")[0].innerHTML = listCartObject.length;

    // Giỏ hàng trống
    if (listCartObject.length == 0) {
        document.getElementsByClassName("cart-empty")[0].style.display = "block";
        document.getElementsByClassName("main-container")[0].style.display = "none";
        return;
    }
}

async function BuyNow() {
    // Lọc các sản phẩm được chọn mua (real = 1)
    const selectedItems = listCartObject.filter(item => item.real === 1);

    if (selectedItems.length === 0) {
        await CreateMustClickOkModal("Bạn chưa chọn sản phẩm nào để mua.", null);
        return;
    }

    try {
        if (!CheckAnonymousCustomer()) {
            // ✅ CRITICAL: Force sync pending quantity updates TRƯỚC KHI checkout
            try {
                await WaitForPendingBatchSync();
            } catch (error) {
                console.error('❌ Failed to sync quantities:', error);
                await CreateMustClickOkModal("Không thể đồng bộ giỏ hàng. Vui lòng thử lại.", null);
                return;
            }
            if (typeof CartManager !== 'undefined') {
                CartManager.clearAndCreateCartFromList(selectedItems); // real = 1)
            }

        } else {

            // Cần lưu real = 1 cho các sản phẩm được chọn vào localStorage (cho cả guest và logged-in)
            if (typeof CartManager !== 'undefined') {
                const cart = CartManager.getCart();
                cart.forEach(item => {
                    item.real = 0; // Reset real = 0 trước
                });


                // Update real field: 1 cho selected, 0 cho unselected
                cart.forEach(item => {
                    const isSelected = selectedItems.some(s => s.sanPhamId === item.sanPhamId);
                    if (isSelected) {
                        item.real = 1;
                        item.time = Date.now(); // Update timestamp cho items được chọn
                    }
                });

                CartManager.saveCart(cart);
            }
        }


        // Set flag để detect back từ checkout page
        sessionStorage.setItem('fromCheckout', 'pending');

        // Chuyển đến trang thanh toán
        window.location.href = '/Home/Checkout';

    } catch (error) {
        console.error('❌ Error in BuyNow:', error);
        await CreateMustClickOkModal('Có lỗi xảy ra. Vui lòng thử lại!', null);
    }
}

// Initial load
window.addEventListener('DOMContentLoaded', async function () {
    if (DEBUG) {
        console.log("DOMContentLoaded - Initial load");
        console.log("window.innerWidth: " + window.innerWidth);

        const vw = document.documentElement.clientWidth;
        const mq = window.matchMedia('(min-width: 800px)');

        console.log("Viewport:", vw);
        console.log("Media query matches:", mq.matches);

        if (vw >= 800 && !mq.matches) {
            console.error("❌ CÓ VẤN ĐỀ!");
        } else if (vw < 800) {
            console.log("ℹ️ Viewport nhỏ hơn 800px → Media query KHÔNG apply");
        }
    }
    await LoadCart();
});

// Back/Forward navigation (từ bfcache)
window.addEventListener('pageshow', async function (event) {
    // Chỉ chạy khi restore từ bfcache (back button)
    if (event.persisted) {
        const fromCheckout = sessionStorage.getItem('fromCheckout');

        if (fromCheckout === 'visited') {
            // ✅ Back từ Checkout page → Reload fresh data
            sessionStorage.removeItem('fromCheckout');

            if (DEBUG) {
                console.log("🔙 pageshow - Back từ Checkout page → Reload cart");
                console.log("listCartObject: " + JSON.stringify(listCartObject));
            }

            await LoadCartBFCache();
        } else {
            // Back từ page khác (product, search, etc.) → Dùng cached data (nhanh hơn)
            if (DEBUG) {
                console.log("🔙 pageshow - Back từ page khác → Dùng bfcache");
            }
            await LoadCart();
        }
    }
});
