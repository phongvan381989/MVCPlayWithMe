// Tham số tìm kiếm
let status;
let listOrder = null; // Danh sách tất cả đơn hàng người dùng

// Hiện tất cả hoặc 1 đơn trên 1 trang, không phân trang nên gọi hàm này ngay khi load trang
async function GetOrder() {
    const searchParams = new URLSearchParams();
    let query;

    if (GetValueFromUrlName("id") == null) {
        // Kiểm tra nếu là khách vãng lai → lấy OrderCodes từ localStorage
        if (CheckAnonymousCustomer()) {
            let orderCodes = GetOrderCodesFromLocalStorage();
            if (orderCodes && orderCodes.length > 0) {
                // Gọi API mới với OrderCodes
                query = "/Customer/GetOrdersByOrderCodes";
                searchParams.append("orderCodes", JSON.stringify(orderCodes));
            }
            else {
                return; // Không có đơn hàng nào → không gọi API
            }
        } else {
            // Khách đăng nhập → dùng API cũ
            query = "/Customer/GetAllOrder";
        }
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

    // Hiển thị đơn hàng - mặc định hiển thị "Tất cả"
    FilterOrderAll(document.getElementsByClassName("nvsflAB87")[0]);
}

/**
 * Lấy danh sách OrderCode từ localStorage (cho khách vãng lai)
 * localStorage key: "guestOrders" (theo format của SaveGuestOrderToLocalStorage trong Checkout.js)
 * Format: [{ orderCode: "260804-12345", orderDate: "2026-08-04T10:30:00.000Z", createdAt: 1722764400000 }, ...]
 * @returns {Array<string>} Danh sách OrderCode
 */
function GetOrderCodesFromLocalStorage() {
    try {
        const storageKey = 'guestOrders';
        let ordersJson = localStorage.getItem(storageKey);

        if (!ordersJson) {
            return [];
        }

        let orders = JSON.parse(ordersJson);

        // Validate
        if (!Array.isArray(orders)) {
            console.warn("guestOrders in localStorage is not an array");
            return [];
        }

        // Extract orderCode từ array of objects
        let orderCodes = orders
            .map(order => order.orderCode)
            .filter(code => code && code.trim() !== "");

        return orderCodes;
    } catch (e) {
        console.error("Error reading OrderCodes from localStorage:", e);
        return [];
    }
}

/**
 * Set placeholder cho search input dựa vào loại khách
 */
function InitializeSearchPlaceholder() {
    let searchInput = document.getElementById("search-input-text-order-id");
    if (!searchInput) return;

    if (CheckAnonymousCustomer()) {
        searchInput.placeholder = "Gõ 5 chữ cuối SDT, mã đơn, tên người nhận";
    } else {
        searchInput.placeholder = "Tìm theo SDT, mã đơn, tên người nhận";
    }
}

/**
 * Hàm tìm kiếm đơn hàng - dispatcher
 * - Khách đăng nhập: Tìm trong listOrder (client-side) - min 3 ký tự
 * - Khách vãng lai: Gửi request về backend - min 5 ký tự
 */
async function OrderSearch() {
    let searchInput = document.getElementById("search-input-text-order-id");
    let searchValue = searchInput.value.trim();

    // Validate input
    if (isEmptyOrSpaces(searchValue)) {
        CreateMustClickOkModal("Vui lòng nhập thông tin tìm kiếm.", null);
        searchInput.focus();
        return;
    }

    // Min length khác nhau: khách vãng lai 5 ký tự, khách đăng nhập 3 ký tự
    let isAnonymous = CheckAnonymousCustomer();
    let minLength = isAnonymous ? 5 : 1;

    if (searchValue.length < minLength) {
        CreateMustClickOkModal(`Nhập ít nhất ${minLength} ký tự`, null);
        searchInput.focus();
        return;
    }

    if (isAnonymous) {
        // Khách vãng lai -> gửi request backend
        await SearchOrderForAnonymous(searchValue);
    } else {
        // Khách đăng nhập -> tìm trong listOrder
        SearchOrderInList(searchValue);
    }
}

/**
 * Tìm kiếm trong listOrder (client-side search cho khách đăng nhập)
 * @param {string} searchValue - Từ khóa tìm kiếm
 */
function SearchOrderInList(searchValue) {
    if (!listOrder || listOrder.length === 0) {
        CreateMustClickOkModal("Không có đơn hàng để tìm kiếm.", null);
        return;
    }

    searchValue = searchValue.toLowerCase();

    let filteredOrders = listOrder.filter(order => {
        // Tìm theo mã đơn
        if (order.OrderCode && order.OrderCode.toLowerCase().includes(searchValue)) {
            return true;
        }

        // Tìm theo tên người nhận
        if (order.name && order.name.toLowerCase().includes(searchValue)) {
            return true;
        }

        // Tìm theo số điện thoại
        if (order.phone && order.phone.includes(searchValue)) {
            return true;
        }

        return false;
    });

    // Hiển thị kết quả tìm kiếm
    ShowResultOrder(filteredOrders);

    // Reset bộ lọc về "Tất cả"
    ResetFilterToAll();

    if (filteredOrders.length === 0) {
        CreateMustClickOkModal("Không tìm thấy đơn hàng phù hợp.", null);
        return;
    }
}

/**
 * Tìm kiếm đơn hàng cho khách vãng lai (backend search)
 * @param {string} searchValue - Từ khóa tìm kiếm
 */
async function SearchOrderForAnonymous(searchValue) {
    const searchParams = new URLSearchParams();
    searchParams.append("sdtNameForSearch", searchValue);
    let query = "/Customer/SearchOrderForAnonymous";
    ShowCircleLoader();

    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();
    let resObj = JSON.parse(responseDB.responseText);
    if (resObj.State != 0) {
        await CreateMustClickOkModal("Có lỗi xảy ra. Vui lòng thử lại sau.", null);
        return;
    }

    let searchResults = resObj.myJson;

    // Hiển thị kết quả

    ShowResultOrder(searchResults);

    // Reset bộ lọc về "Tất cả"
    ResetFilterToAll();

    if (!searchResults || searchResults.length === 0) {
        CreateMustClickOkModal("Không tìm thấy đơn hàng phù hợp.", null);
    }
}

/**
 * Lọc danh sách orders theo status
 * @param {number} status - Trạng thái cần lọc (EOrderFilterStatus.ALL = Tất cả)
 * @returns {Array} Danh sách orders đã lọc
 */
function FilterOrdersByStatus(status) {
    if (!listOrder || listOrder.length === 0) {
        return [];
    }

    // status = 10 -> Tất cả
    if (status === EOrderFilterStatus.ALL) {
        return listOrder;
    }

    // Lọc theo status cuối cùng trong lsOrderTrack
    return listOrder.filter(order => {
        let latestStatus = order.lsOrderTrack[order.lsOrderTrack.length - 1].status;
        if (status === EOrderFilterStatus.PENDING) {
            return order.OrderPayStatus === EOrderPayStatus.PENDING;
        }
        else if (status === EOrderFilterStatus.REFUNDED) {
            return order.OrderPayStatus === EOrderPayStatus.REFUNDED;
        }
        return order.OrderStatus === status;
    });
}

/**
 * Update UI của status buttons (highlight button đang active)
 * @param {HTMLElement} activeButton - Button được click
 */
function UpdateStatusButtonUI(activeButton) {
    if (GetValueFromUrlName("id") != null) {
        // Đang xem 1 đơn cụ thể -> không update button UI
        return;
    }

    let allButtons = document.getElementsByClassName("nvsflAB87");
    for (let btn of allButtons) {
        if (btn === activeButton) {
            btn.style.color = "#764ba2";
            btn.style.borderBottom = "2px solid #764ba2";
        } else {
            btn.style.color = "initial";
            btn.style.borderBottom = "none";
        }
    }
}

/**
 * Reset bộ lọc về "Tất cả" (highlight button đầu tiên)
 */
function ResetFilterToAll() {
    let allButtons = document.getElementsByClassName("nvsflAB87");
    if (allButtons.length > 0) {
        UpdateStatusButtonUI(allButtons[0]); // Button đầu tiên là "Tất cả"
    }
}

/**
 * Xóa nội dung search input
 */
function ClearSearchInput() {
    let searchInput = document.getElementById("search-input-text-order-id");
    if (searchInput) {
        searchInput.value = "";
    }
}

/**
 * Xử lý khi click button lọc theo status (internal helper)
 * @param {HTMLElement} buttonElement - Button được click
 * @param {number} status - Status code để lọc
 */
function OnStatusFilterClick(buttonElement, status) {
    // Xóa search input khi click filter
    ClearSearchInput();

    // Update UI button
    UpdateStatusButtonUI(buttonElement);

    // Lọc và hiển thị
    let filteredOrders = FilterOrdersByStatus(status);
    ShowResultOrder(filteredOrders);
}

// ============================================
// Các hàm filter theo từng trạng thái cụ thể
// ============================================

function FilterOrderAll(buttonElement) {
    OnStatusFilterClick(buttonElement, EOrderFilterStatus.ALL);
}

function FilterOrderPending(buttonElement) {
    OnStatusFilterClick(buttonElement, EOrderFilterStatus.PENDING);
}

function FilterOrderProcessing(buttonElement) {
    OnStatusFilterClick(buttonElement, EOrderFilterStatus.PROCESSING);
}

function FilterOrderShipping(buttonElement) {
    OnStatusFilterClick(buttonElement, EOrderFilterStatus.SHIPPING);
}

function FilterOrderReceived(buttonElement) {
    OnStatusFilterClick(buttonElement, EOrderFilterStatus.RECEIVED);
}

function FilterOrderCancelled(buttonElement) {
    OnStatusFilterClick(buttonElement, EOrderFilterStatus.CANCELLED);
}

function FilterOrderRefunded(buttonElement) {
    OnStatusFilterClick(buttonElement, EOrderFilterStatus.REFUNDED);
}

/**
 * Hiển thị danh sách orders (không lọc, chỉ render)
 * @param {Array} orders - Danh sách orders cần hiển thị
 */
function ShowResultOrder(orders) {
    // Làm trống container
    let container = document.getElementsByClassName("result-content-container")[0];
    container.innerHTML = "";

    // Kiểm tra danh sách rỗng
    if (!orders || orders.length === 0) {
        container.innerHTML = '<div class="empty-order-state">Danh sách trống</div>';
        return;
    }

    let sample = document.getElementsByClassName("sample-order")[0];
    let sampleItem = document.getElementsByClassName("sample-item-container")[0];
    let samplePayment = document.getElementsByClassName("sample-payment")[0];

    // Render từng order
    for (let i = 0; i < orders.length; i++) {
        let orderObj = orders[i];

        let clone = sample.cloneNode(true);

        // Trạng thái chờ thanh toán/ đã thanh toán/ hoàn tiền
        if (orderObj.OrderPayStatus === EOrderPayStatus.PENDING) {
            clone.getElementsByClassName("last-order-pay-status")[0].innerHTML = "Chờ thanh toán";
        }
        else if (orderObj.OrderPayStatus === EOrderPayStatus.PAID) {
            clone.getElementsByClassName("last-order-pay-status")[0].innerHTML = "Đã thanh toán";
        }
        else if (orderObj.OrderPayStatus === EOrderPayStatus.REFUNDED) {
            clone.getElementsByClassName("last-order-pay-status")[0].innerHTML = "Hoàn tiền";
        }

        // Mã đơn
        clone.getElementsByClassName("order-code")[0].innerHTML = orderObj.OrderCode;

        // Trạng thái đơn
        // lấy đầu tiên vì query đã sort
        clone.getElementsByClassName("last-order-status")[0].innerHTML =
            orderObj.lsOrderTrack[0].strStatus;

        // Thông tin nhận hàng
        let cloneAddress = clone.getElementsByClassName("sample-address-container")[0];
        cloneAddress.style.display = "initial";

        let d = new Date(orderObj.time);
        cloneAddress.getElementsByClassName("address-date")[0].innerHTML = GetFormattedDate(d);

        cloneAddress.getElementsByClassName("address-name-phone")[0].innerHTML =
            orderObj.name + ", " + orderObj.phone;

        // Là khách vãng lai
        if (CheckAnonymousCustomer()) {
            cloneAddress.getElementsByClassName("address-address")[0].innerHTML =
                orderObj.province + ", ******";
        }
        else {
            cloneAddress.getElementsByClassName("address-address")[0].innerHTML =
                orderObj.detail + ", " + orderObj.subdistrict + ", " + orderObj.province;
        }

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
        else{
            clone.getElementsByClassName("gQuJxM")[0].innerHTML = orderObj.note;
        }

        // Phương thức thanh toán
        if (orderObj.PaymentMethod == EPaymentMethod.BANK_TRANSFER) {
            let paymentMethodContainer = clone.getElementsByClassName("hjhgui88")[0];
            paymentMethodContainer.innerHTML = `
                <span class="KoRB7y payment-method-text">Chuyển khoản ngân hàng</span>
                <button type="button" class="btn-view-qr"
                    style="margin: 10px; padding: 6px 12px; background: #667eea; color: white; border: none; border-radius: 4px; cursor: pointer; font-size: 0.85rem;"
                    onclick="ShowPaymentQRCode('${orderObj.OrderCode}')">
                    📱 Xem QR thanh toán
                </button>
            `;
        }
        else {
            clone.getElementsByClassName("payment-method-text")[0].innerHTML = "Thanh toán khi nhận hàng";
        }

        // CHi tiết thanh toán
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
}

// =============================================
// Hiển thị QR thanh toán cho đơn hàng chuyển khoản
// =============================================

/**
 * Gọi API lấy thông tin QR code và hiển thị modal
 * @param {string} orderCode - Mã đơn hàng
 */
async function ShowPaymentQRCode(orderCode) {
    ShowCircleLoader();

    try {
        const responseText = await PostJSON('/Customer/GetOrderPaymentQRCode', {
            orderCode: orderCode
        });

        RemoveCircleLoader();

        const result = JSON.parse(responseText);

        if (result.State !== 0) {
            await CreateMustClickOkModal(result.Message || "Không thể lấy thông tin QR code", null);
            return;
        }

        // Hiển thị modal với QR code
        const paymentInfo = {
            orderCode: result.OrderCode,
            qrCodeUrl: result.QRCodeUrl,
            bankAccount: result.BankAccount,
            totalAmount: result.TotalAmount
        };

        await CreateBankTransferPaymentModal(paymentInfo);

    } catch (error) {
        RemoveCircleLoader();
        console.error("ShowPaymentQRCode error:", error);
        await CreateMustClickOkModal("Có lỗi xảy ra. Vui lòng thử lại sau.", null);
    }
}

/**
 * Hiển thị modal thanh toán chuyển khoản với QR code VietQR
 * @param {object} paymentInfo - Thông tin thanh toán {orderCode, qrCodeUrl, bankAccount, totalAmount}
 */
async function CreateBankTransferPaymentModal(paymentInfo) {
    return new Promise((resolve) => {
        const { orderCode, qrCodeUrl, bankAccount, totalAmount } = paymentInfo;

        // Tạo copy icons bằng CreateCopyIcon từ web.play.with.me.common.js
        const copyIconAccount = CreateCopyIcon({
            size: '16',
            color: '#007bff',
            title: 'Copy số tài khoản',
            className: 'copy-icon-account'
        });

        const copyIconContent = CreateCopyIcon({
            size: '16',
            color: '#007bff',
            title: 'Copy nội dung CK',
            className: 'copy-icon-content'
        });

        let container = document.createElement("div");
        container.className = "container-my-modal-must-click-ok";
        container.innerHTML = `
            <div tabindex='0' class='my-modal-must-click-ok'>
                <div class='modal-content-selected'>
                    <div style='text-align: center; margin-bottom: 20px;'>
                        <div style='font-size: 1.2rem; color: #333; margin-bottom: 10px;'>
                            📦 Thông tin thanh toán
                        </div>
                        <div style='font-size: 0.95rem; color: #666; margin-bottom: 5px;'>
                            Mã đơn hàng: <strong style='color: #007bff;'>${orderCode}</strong>
                        </div>
                    </div>

                    <!-- QR Code Section -->
                    <div style='text-align: center; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 20px; border-radius: 8px; margin-bottom: 20px;'>
                        <h4 style='color: white; margin: 0 0 15px 0; font-size: 1rem;'>
                            📱 Quét mã QR để thanh toán
                        </h4>
                        <div style='background: white; padding: 5px; border-radius: 6px; display: inline-block;'>
                            <img src='${qrCodeUrl}' alt='QR Code' style='max-width: 280px; width: 100%; height: auto;' />
                        </div>
                        <p style='color: white; font-size: 0.85rem; margin: 12px 0 0 0;'>
                            Mở app ngân hàng → Quét QR → Xác nhận
                        </p>
                    </div>

                    <!-- Bank Info -->
                    <div style='background: #f8f9fa; padding: 15px; border-radius: 6px; margin-bottom: 15px;'>
                        <h5 style='font-size: 0.95rem; margin: 0 0 12px 0; color: #495057;'>
                            🏦 Hoặc chuyển khoản thủ công:
                        </h5>
                        <div style='background: white; padding: 12px; border-radius: 4px;'>
                            <div style='display: flex; justify-content: space-between; padding: 6px 0; border-bottom: 1px solid #e9ecef; font-size: 0.85rem;'>
                                <span style='color: #6c757d;'>Ngân hàng:</span>
                                <span style='font-weight: 500;'>${bankAccount.BankName}</span>
                            </div>
                            <div style='display: flex; justify-content: space-between; align-items: center; padding: 6px 0; border-bottom: 1px solid #e9ecef; font-size: 0.85rem;'>
                                <span style='color: #6c757d;'>Số TK:</span>
                                <div style='display: flex; align-items: center; gap: 6px;'>
                                    <span style='font-weight: 600;'>${bankAccount.AccountNumber}</span>
                                    ${copyIconAccount}
                                </div>
                            </div>
                            <div style='display: flex; justify-content: space-between; padding: 6px 0; border-bottom: 1px solid #e9ecef; font-size: 0.85rem;'>
                                <span style='color: #6c757d;'>Chủ TK:</span>
                                <span style='font-weight: 500;'>${bankAccount.AccountHolder}</span>
                            </div>
                            <div style='display: flex; justify-content: space-between; padding: 6px 0; border-bottom: 1px solid #e9ecef; font-size: 0.85rem;'>
                                <span style='color: #6c757d;'>Số tiền:</span>
                                <span style='font-weight: 700; color: #28a745;'>${totalAmount.toLocaleString()}đ</span>
                            </div>
                            <div style='display: flex; justify-content: space-between; align-items: center; padding: 6px 0; font-size: 0.85rem;'>
                                <span style='color: #6c757d;'>Nội dung CK:</span>
                                <div style='display: flex; align-items: center; gap: 6px;'>
                                    <span style='font-weight: 700; color: #007bff;'>${orderCode}</span>
                                    ${copyIconContent}
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Warning -->
                    <div style='background: #fff3cd; border: 1px solid #ffc107; padding: 12px; border-radius: 4px; margin-bottom: 20px;'>
                        <div style='font-size: 0.8rem; color: #856404;'>
                            <strong>⚠️ Lưu ý:</strong>
                            <ul style='margin: 8px 0 0 18px; padding: 0;'>
                                <li>Chuyển <strong>đúng số tiền</strong>: ${totalAmount.toLocaleString()}đ</li>
                                <li>Nội dung CK: <strong>${orderCode}</strong></li>
                                <li>Đơn hàng sẽ được xử lý sau khi nhận được thanh toán</li>
                            </ul>
                        </div>
                    </div>

                    <!-- Close Button -->
                    <div style='text-align: center;'>
                        <button class='btn-close' style='
                            padding: 10px 20px;
                            background: #6c757d;
                            color: white;
                            border: none;
                            border-radius: 4px;
                            cursor: pointer;
                            font-size: 0.95rem;
                            display: inline-flex;
                            align-items: center;
                            justify-content: center;
                        '>Đóng</button>
                    </div>
                </div>
            </div>
        `;

        document.getElementsByTagName("body")[0].appendChild(container);

        let modal = container.getElementsByClassName("my-modal-must-click-ok")[0];
        modal.focus();

        // Add copy event listeners
        container.getElementsByClassName("copy-icon-account")[0].addEventListener("click", function() {
            CopyWithVisualFeedback(bankAccount.AccountNumber, this, {
                type: 'icon',
                successColor: '#28a745'
            });
        });

        container.getElementsByClassName("copy-icon-content")[0].addEventListener("click", function () {
            CopyWithVisualFeedback(orderCode, this, {
                type: 'icon',
                successColor: '#28a745'
            });
        });

        // Close button
        container.getElementsByClassName("btn-close")[0].addEventListener("click", function () {
            container.remove();
            resolve("closed");
        });
    });
}

// =============================================
// Initial Load - Gọi GetOrder() sau khi page load xong
// =============================================
window.addEventListener('DOMContentLoaded', async function () {
    // Set placeholder phù hợp với loại khách
    InitializeSearchPlaceholder();

    // Load danh sách đơn hàng
    await GetOrder();
});
