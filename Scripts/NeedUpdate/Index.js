// NeedUpdate/Index.js
// Xử lý cập nhật giá & tồn kho lên các sàn TMĐT

/**
 * Khởi tạo trang - set radio "Tất cả" làm mặc định
 */
document.addEventListener('DOMContentLoaded', function () {
    // Mặc định chọn "Tất cả"
    let allRadio = document.getElementById('all-e-ecommonerce-type');
    if (allRadio) {
        allRadio.checked = true;
    }
});

/**
 * Callback khi thay đổi lựa chọn sàn TMĐT
 * Hàm này được gọi từ ECommerceType.cshtml (onchange)
 */
function ECommerceTypeChange() {
    let eType = GetECommerceType(); // Từ web.play.with.me.common.js
    console.log(`Sàn được chọn: ${eType}`);
}

/**
 * Cập nhật giá sản phẩm có thay đổi
 */
async function UpdateChangedPrice() {
    let eType = GetECommerceType(); // ALL, TIKI, SHOPEE, LAZADA, PLAYWITHME

    if (!confirm(`Xác nhận cập nhật giá sản phẩm có thay đổi lên sàn: ${eType}?`)) {
        return;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("eType", eType);

    let query = "/NeedUpdate/UpdateChangedPrice";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật giá thành công.", "Có lỗi xảy ra.");
}

/**
 * Cập nhật giá toàn bộ sản phẩm
 */
async function UpdateAllPrice() {
    let eType = GetECommerceType(); // ALL, TIKI, SHOPEE, LAZADA, PLAYWITHME

    if (!confirm(`⚠️ XÁC NHẬN CẬP NHẬT GIÁ TOÀN BỘ SẢN PHẨM LÊN SÀN: ${eType}?\n\nThao tác này có thể mất nhiều thời gian.`)) {
        return;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("eType", eType);

    let query = "/NeedUpdate/UpdateAllPrice";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật giá toàn bộ thành công.", "Có lỗi xảy ra.");
}

/**
 * Cập nhật tồn kho sản phẩm có thay đổi
 */
async function UpdateChangedStock() {
    let eType = GetECommerceType(); // ALL, TIKI, SHOPEE, LAZADA, PLAYWITHME

    if (!confirm(`Xác nhận cập nhật tồn kho sản phẩm có thay đổi lên sàn: ${eType}?`)) {
        return;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("eType", eType);

    let query = "/NeedUpdate/UpdateChangedStock";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật tồn kho thành công.", "Có lỗi xảy ra.");
}

/**
 * Cập nhật tồn kho toàn bộ sản phẩm
 */
async function UpdateAllStock() {
    let eType = GetECommerceType(); // ALL, TIKI, SHOPEE, LAZADA, PLAYWITHME

    if (!confirm(`⚠️ XÁC NHẬN CẬP NHẬT TỒN KHO TOÀN BỘ SẢN PHẨM LÊN SÀN: ${eType}?\n\nThao tác này có thể mất nhiều thời gian.`)) {
        return;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("eType", eType);

    let query = "/NeedUpdate/UpdateAllStock";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật tồn kho toàn bộ thành công.", "Có lỗi xảy ra.");
}
