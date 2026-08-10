// ========================================
// ADMIN ORDER DETAIL - LOAD SINGLE ORDER
// ========================================

/**
 * Lấy chi tiết 1 đơn hàng
 */
async function GetOrderDetail() {
    if (!ORDER_ID) {
        CreateMustClickOkModal('Không tìm thấy ID đơn hàng', null);
        return;
    }

    try {
        ShowCircleLoader();

        // Gọi API admin
        let data = { id: ORDER_ID };
        let responseText = await PostJSON('/Administrator/GetOrderDetailAdmin', data);
        let result = JSON.parse(responseText);

        RemoveCircleLoader();

        if (result.State !== 0) {
            await CreateMustClickOkModal(result.Message || 'Có lỗi xảy ra. Vui lòng thử lại sau.', null);
            return;
        }

        let order = result.myJson;

        if (!order) {
            await CreateMustClickOkModal('Không tìm thấy đơn hàng', null);
            return;
        }

        // Hiển thị đơn hàng
        ShowOrderDetail(order);

    } catch (error) {
        RemoveCircleLoader();
        console.error('GetOrderDetail error:', error);
        CreateMustClickOkModal('Lỗi kết nối server', null);
    }
}

/**
 * Hiển thị chi tiết đơn hàng (1 order)
 * @param {Object} orderObj - Order object
 */
function ShowOrderDetail(orderObj) {
    let container = document.getElementsByClassName("result-content-container")[0];
    container.innerHTML = "";

    let sample = document.getElementsByClassName("sample-order")[0];
    let sampleItem = document.getElementsByClassName("sample-item-container")[0];

    let clone = sample.cloneNode(true);

    // Mã đơn
    clone.getElementsByClassName("order-code")[0].innerHTML = "MÃ . " + orderObj.OrderCode;

    // Trạng thái đơn (lấy đầu tiên vì query đã sort)
    clone.getElementsByClassName("fxASnxvd")[0].innerHTML =
        orderObj.lsOrderTrack[0].strStatus;

    // Thông tin nhận hàng
    let cloneAddress = clone.getElementsByClassName("sample-address-container")[0];
    cloneAddress.style.display = "initial";

    let d = new Date(orderObj.time);
    cloneAddress.getElementsByClassName("address-date")[0].innerHTML = GetFormattedDate(d);

    cloneAddress.getElementsByClassName("address-name-phone")[0].innerHTML =
        orderObj.name + ", " + orderObj.phone;

    cloneAddress.getElementsByClassName("address-address")[0].innerHTML =
        orderObj.detail + ", " + orderObj.subdistrict + ", " + orderObj.province;

    let containerItem = clone.getElementsByClassName("order-item-container")[0];

    // Sản phẩm
    let num = orderObj.lsOrderDetail.length;
    for (let j = 0; j < num; j++) {
        let itemObj = orderObj.lsOrderDetail[j];
        let cloneItem = sampleItem.cloneNode(true);
        cloneItem.style.display = "flex";

        // Icon - Set src cho <img>
        let imageSrc = Get320VersionOfImageSrc(GetSanPhamMediaUrl(itemObj.sanPhamId, itemObj.CoverImageFileName));
        let imgElement = cloneItem.getElementsByClassName("model-icon")[0];
        imgElement.src = imageSrc;
        imgElement.alt = itemObj.name || "Product image";

        // Tên
        cloneItem.getElementsByClassName("ffZM87hf-name")[0].innerHTML = itemObj.name;

        // Giá bìa, giá bán số lượng
        if (itemObj.bookCoverPrice > itemObj.price) {
            cloneItem.getElementsByClassName("vWt6ZL")[0].style.display = "";
            cloneItem.getElementsByClassName("vWt6ZL")[0].innerHTML =
                ConvertMoneyToTextWithIcon(itemObj.bookCoverPrice);
        }
        else {
            cloneItem.getElementsByClassName("vWt6ZL")[0].style.display = "none";
        }
        cloneItem.getElementsByClassName("M-AAFK")[0].innerHTML =
            ConvertMoneyToTextWithIcon(itemObj.price);

        cloneItem.getElementsByClassName("quantity-model")[0].innerHTML = itemObj.quantity;

        containerItem.append(cloneItem);
    }

    // Lời nhắn cho shop
    if (isEmptyOrSpaces(orderObj.note)) {
        clone.getElementsByClassName("gQuJxM")[0].remove();
    }
    else {
        clone.getElementsByClassName("gQuJxM")[0].innerHTML = orderObj.note;
    }

    // Phương thức thanh toán
    if (orderObj.PaymentMethod === EPaymentMethod.BANK_TRANSFER) {
        clone.getElementsByClassName("payment-method-text")[0].innerHTML = "Chuyển khoản ngân hàng";
    }

    // Chi tiết thanh toán
    num = orderObj.lsOrderPay.length;
    for (let j = 0; j < num; j++) {
        let paymentObj = orderObj.lsOrderPay[j];

        // Tìm row tương ứng với strType
        if (paymentObj.type === EOrderPayType.TOTAL) {
            // Hiển thị tổng tiền hàng
            clone.getElementsByClassName("model-money-sum")[0].innerHTML =
                ConvertMoneyToTextWithIcon(paymentObj.value);
        }
        else if (paymentObj.type === EOrderPayType.SHIP) {
            // Hiển thị phí vận chuyển
            clone.getElementsByClassName("shipee-fee")[0].innerHTML =
                ConvertMoneyToTextWithIcon(paymentObj.value);
        }
        else if (paymentObj.type === EOrderPayType.PROMOTION) {
            if (paymentObj.orderSimplePromotion != null &&
                paymentObj.orderSimplePromotion.Type === EOrderSimplePromotionType.SHIP_DISCOUNT) {
                if (paymentObj.value != 0) {
                    clone.getElementsByClassName("free-shipee-fee")[0].innerHTML =
                        ConvertMoneyToTextWithIcon(paymentObj.value);
                }
                else {
                    clone.getElementsByClassName("free-shipee-fee")[0].parentElement.style.display = "none";
                }
            }
            else if (paymentObj.orderSimplePromotion != null &&
                paymentObj.orderSimplePromotion.Type === EOrderSimplePromotionType.TOTAL_DISCOUNT) {
                if (paymentObj.value != 0) {
                    clone.getElementsByClassName("discount-final-money")[0].innerHTML =
                        ConvertMoneyToTextWithIcon(paymentObj.value);
                }
                else {
                    clone.getElementsByClassName("discount-final-money")[0].parentElement.style.display = "none";
                }
            }
        }
        else if (paymentObj.type === EOrderPayType.FINAL) {
            // Hiển thị tổng thanh toán
            clone.getElementsByClassName("final-money")[0].innerHTML =
                ConvertMoneyToTextWithIcon(paymentObj.value);
        }
    }

    clone.style.display = "block";
    container.append(clone);
}

// =============================================
// Initial Load - Gọi GetOrderDetail() sau khi page load xong
// =============================================
window.addEventListener('DOMContentLoaded', async function () {
    await GetOrderDetail();
});
