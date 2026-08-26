let listCartObject;

let listCustomerInforObject; // list customer object server trả về

let currentIndexInforObject = -1; // index địa chỉ nhận hàng hiện tại. Chưa chọn địa chỉ nhận hàng thì dùng thông tin mặc định khi load trang

let currentIndexInforUpdateObject = -1; // index địa chỉ nhận hàng cập nhật thông tin

let funcOfChangeAddress = null; // Hàm xử lý sự kiện click nút change-address-btn

let listOrderSimplePromotionsObject = null; // Danh sách promotion đang hoạt động

let listOrderPay = null; // Danh sách các loại thanh toán: tổng tiền hàng, phí ship, tổng thanh toán,....

// Mark rằng user đã vào checkout page (để Cart page detect back navigation)
if (sessionStorage.getItem('fromCheckout') === 'pending') {
    sessionStorage.setItem('fromCheckout', 'visited');
}

// ========== GUEST ORDER STORAGE ==========

/**
 * Lưu mã đơn hàng và thời gian đặt đơn vào localStorage cho khách vãng lai
 * @param {string} orderCode - Mã đơn hàng (format: YYMMDD-XXXXX)
 */
function SaveGuestOrderToLocalStorage(orderCode) {
    try {
        const storageKey = 'guestOrders';

        // Lấy danh sách đơn hàng hiện tại
        let orders = [];
        const existingData = localStorage.getItem(storageKey);
        if (existingData) {
            orders = JSON.parse(existingData);
        }

        // Thêm đơn hàng mới (check trùng trước)
        const existingOrder = orders.find(o => o.orderCode === orderCode);
        if (!existingOrder) {
            orders.push({
                orderCode: orderCode,
                orderDate: new Date().toISOString(), // ISO format: "2026-08-04T10:30:00.000Z"
                createdAt: Date.now() // Timestamp for sorting
            });

            // Sắp xếp theo thời gian mới nhất trước
            orders.sort((a, b) => b.createdAt - a.createdAt);

            // Giới hạn số lượng đơn hàng lưu trữ (ví dụ: 100 đơn gần nhất)
            if (orders.length > 100) {
                orders = orders.slice(0, 100);
            }

            // Lưu vào localStorage
            localStorage.setItem(storageKey, JSON.stringify(orders));

            if (DEBUG) {
                console.log('✓ Đã lưu mã đơn hàng vào localStorage:', orderCode);
            }
        }
    } catch (error) {
        if (DEBUG) {
            console.error('Lỗi khi lưu mã đơn hàng:', error);
        }
    }
}

/**
 * Lấy danh sách đơn hàng của khách vãng lai từ localStorage
 * @returns {Array} Mảng các đơn hàng [{orderCode, orderDate, createdAt}]
 */
function GetGuestOrdersFromLocalStorage() {
    try {
        const storageKey = 'guestOrders';
        const existingData = localStorage.getItem(storageKey);
        if (existingData) {
            return JSON.parse(existingData);
        }
        return [];
    } catch (error) {
        console.error('Lỗi khi lấy danh sách đơn hàng:', error);
        return [];
    }
}

// ========== PROMOTION APIs ==========

/**
 * Lấy danh sách promotion đang hoạt động
 * @returns {Promise<Array>} Mảng OrderSimplePromotion
 */
async function GetActiveOrderSimplePromotions() {
    try {
        const searchParams = new URLSearchParams();
        const query = "/Home/GetActiveOrderSimplePromotions";

        const response = await RequestHttpPostPromise(searchParams, query);
        const promotions = JSON.parse(response.responseText);

        if (DEBUG) {
            console.log("✓ Load promotions:", promotions);
        }

        return promotions;
    } catch (error) {
        console.warn("Lỗi khi lấy promotions:", error);
        return [];
    }
}

// /**
//  * Tính tổng giảm giá cho đơn hàng
//  * @param {number} totalProductAmount - Tổng tiền hàng (không bao gồm ship)
//  * @returns {Promise<{discount: number, descriptions: string[]}>}
//  */
// async function CalculateDiscount(totalProductAmount) {
//     try {
//         const searchParams = new URLSearchParams();
//         searchParams.append("totalProductAmount", totalProductAmount);
//         const query = "/Home/CalculateDiscount";

//         const response = await RequestHttpPostPromise(searchParams, query);
//         const result = JSON.parse(response.responseText);

//         if (DEBUG) {
//             console.log(`✓ Giảm giá cho ${totalProductAmount}đ: ${result.discount}đ`);
//         }

//         return result;
//     } catch (error) {
//         console.warn("Lỗi khi tính giảm giá:", error);
//         return { discount: 0, descriptions: [] };
//     }
// }

/**
 * Load promotions khi trang load
 */
async function LoadPromotions() {
    listOrderSimplePromotionsObject = await GetActiveOrderSimplePromotions();
}

async function CheckoutPageLoadCart() {
    // Lấy guest cart từ localStorage
    let guestCart = CartManager.getCart();

    guestCart = guestCart.filter(item => item.real === 1); // Chỉ lấy những sản phẩm thực sự chọn mua (real=1)
    if (DEBUG) {
        console.log("CheckoutPageLoadCart guestCart: " + JSON.stringify(guestCart));
    }

    // Gửi cart data dưới dạng JSON body
    return await PostJSON('/Home/CheckoutPageLoadCart', guestCart);
}

/**
 * Copy text vào clipboard
 * @param {string} text - Text cần copy
 * @param {HTMLElement} iconElement - Icon element để hiển thị animation
 */
function copyToClipboard(text, iconElement) {
    navigator.clipboard.writeText(text).then(() => {
        // Hiển thị thông báo đã copy - đổi SVG thành checkmark
        const originalHTML = iconElement.innerHTML;
        const originalColor = iconElement.style.color;

        // Thay SVG bằng checkmark emoji
        iconElement.innerHTML = '✅';
        iconElement.style.color = '#28a745';
        iconElement.style.fontSize = '1.2rem';

        setTimeout(() => {
            iconElement.innerHTML = originalHTML;
            iconElement.style.color = originalColor || '#007bff';
            iconElement.style.fontSize = '';
        }, 1500);
    }).catch(err => {
        console.error('Lỗi khi copy:', err);
        alert('Không thể copy. Vui lòng copy thủ công.');
    });
}

/**
 * Hiển thị modal thanh toán chuyển khoản với QR code VietQR
 * @param {object} paymentInfo - Thông tin thanh toán {orderCode, qrCodeUrl, bankAccount, totalAmount}
 */
async function CreateBankTransferPaymentModal(paymentInfo) {
    return new Promise((resolve) => {
        const { orderCode, qrCodeUrl, bankAccount, totalAmount } = paymentInfo;

        let container = document.createElement("div");
        container.className = "container-my-modal-must-click-ok";
        container.innerHTML = `
            <div tabindex='0' class='my-modal-must-click-ok'>
                <div class='modal-content-selected'>
                    <div style='text-align: center; margin-bottom: 20px;'>
                        <div style='font-size: 1.3rem; color: #28a745; margin-bottom: 10px;'>
                            ✅ Đặt hàng thành công!
                        </div>
                        <div style='font-size: 1rem; color: #333; margin-bottom: 5px; display: flex; align-items: center; justify-content: center; gap: 8px;'>
                            📦 Mã đơn hàng: <strong style='color: #007bff;'>${orderCode}</strong>
                            <span class='copy-icon-ordercode' style='cursor: pointer; color: #007bff; user-select: none; display: inline-flex; align-items: center;' title='Copy mã đơn hàng'>
                                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 115.77 122.88" fill="currentColor">
                                    <path d="M89.62,13.96v7.73h12.19h0.01v0.02c3.85,0.01,7.34,1.57,9.86,4.1c2.5,2.51,4.06,5.98,4.07,9.82h0.02v0.02 v73.27v0.01h-0.02c-0.01,3.84-1.57,7.33-4.1,9.86c-2.51,2.5-5.98,4.06-9.82,4.07v0.02h-0.02h-61.7H40.1v-0.02 c-3.84-0.01-7.34-1.57-9.86-4.1c-2.5-2.51-4.06-5.98-4.07-9.82h-0.02v-0.02V92.51H13.96h-0.01v-0.02c-3.84-0.01-7.34-1.57-9.86-4.1 c-2.5-2.51-4.06-5.98-4.07-9.82H0v-0.02V13.96v-0.01h0.02c0.01-3.85,1.58-7.34,4.1-9.86c2.51-2.5,5.98-4.06,9.82-4.07V0h0.02h61.7 h0.01v0.02c3.85,0.01,7.34,1.57,9.86,4.1c2.5,2.51,4.06,5.98,4.07,9.82h0.02V13.96L89.62,13.96z M79.04,21.69v-7.73v-0.02h0.02 c0-0.91-0.39-1.75-1.01-2.37c-0.61-0.61-1.46-1-2.37-1v0.02h-0.01h-61.7h-0.02v-0.02c-0.91,0-1.75,0.39-2.37,1.01 c-0.61,0.61-1,1.46-1,2.37h0.02v0.01v64.59v0.02h-0.02c0,0.91,0.39,1.75,1.01,2.37c0.61,0.61,1.46,1,2.37,1v-0.02h0.01h12.19V35.65 v-0.01h0.02c0.01-3.85,1.58-7.34,4.1-9.86c2.51-2.5,5.98-4.06,9.82-4.07v-0.02h0.02H79.04L79.04,21.69z M105.18,108.92V35.65v-0.02 h0.02c0-0.91-0.39-1.75-1.01-2.37c-0.61-0.61-1.46-1-2.37-1v0.02h-0.01h-61.7h-0.02v-0.02c-0.91,0-1.75,0.39-2.37,1.01 c-0.61,0.61-1,1.46-1,2.37h0.02v0.01v73.27v0.02h-0.02c0,0.91,0.39,1.75,1.01,2.37c0.61,0.61,1.46,1,2.37,1v-0.02h0.01h61.7h0.02 v0.02c0.91,0,1.75-0.39,2.37-1.01c0.61-0.61,1-1.46,1-2.37h-0.02V108.92L105.18,108.92z"/>
                                </svg>
                            </span>
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
                                    <span class='copy-icon-account' style='cursor: pointer; color: #007bff; user-select: none; display: inline-flex; align-items: center;' title='Copy số tài khoản'>
                                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 115.77 122.88" fill="currentColor">
                                            <path d="M89.62,13.96v7.73h12.19h0.01v0.02c3.85,0.01,7.34,1.57,9.86,4.1c2.5,2.51,4.06,5.98,4.07,9.82h0.02v0.02 v73.27v0.01h-0.02c-0.01,3.84-1.57,7.33-4.1,9.86c-2.51,2.5-5.98,4.06-9.82,4.07v0.02h-0.02h-61.7H40.1v-0.02 c-3.84-0.01-7.34-1.57-9.86-4.1c-2.5-2.51-4.06-5.98-4.07-9.82h-0.02v-0.02V92.51H13.96h-0.01v-0.02c-3.84-0.01-7.34-1.57-9.86-4.1 c-2.5-2.51-4.06-5.98-4.07-9.82H0v-0.02V13.96v-0.01h0.02c0.01-3.85,1.58-7.34,4.1-9.86c2.51-2.5,5.98-4.06,9.82-4.07V0h0.02h61.7 h0.01v0.02c3.85,0.01,7.34,1.57,9.86,4.1c2.5,2.51,4.06,5.98,4.07,9.82h0.02V13.96L89.62,13.96z M79.04,21.69v-7.73v-0.02h0.02 c0-0.91-0.39-1.75-1.01-2.37c-0.61-0.61-1.46-1-2.37-1v0.02h-0.01h-61.7h-0.02v-0.02c-0.91,0-1.75,0.39-2.37,1.01 c-0.61,0.61-1,1.46-1,2.37h0.02v0.01v64.59v0.02h-0.02c0,0.91,0.39,1.75,1.01,2.37c0.61,0.61,1.46,1,2.37,1v-0.02h0.01h12.19V35.65 v-0.01h0.02c0.01-3.85,1.58-7.34,4.1-9.86c2.51-2.5,5.98-4.06,9.82-4.07v-0.02h0.02H79.04L79.04,21.69z M105.18,108.92V35.65v-0.02 h0.02c0-0.91-0.39-1.75-1.01-2.37c-0.61-0.61-1.46-1-2.37-1v0.02h-0.01h-61.7h-0.02v-0.02c-0.91,0-1.75,0.39-2.37,1.01 c-0.61,0.61-1,1.46-1,2.37h0.02v0.01v73.27v0.02h-0.02c0,0.91,0.39,1.75,1.01,2.37c0.61,0.61,1.46,1,2.37,1v-0.02h0.01h61.7h0.02 v0.02c0.91,0,1.75-0.39,2.37-1.01c0.61-0.61,1-1.46,1-2.37h-0.02V108.92L105.18,108.92z"/>
                                        </svg>
                                    </span>
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
                                    <span class='copy-icon-content' style='cursor: pointer; color: #007bff; user-select: none; display: inline-flex; align-items: center;' title='Copy nội dung CK'>
                                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 115.77 122.88" fill="currentColor">
                                            <path d="M89.62,13.96v7.73h12.19h0.01v0.02c3.85,0.01,7.34,1.57,9.86,4.1c2.5,2.51,4.06,5.98,4.07,9.82h0.02v0.02 v73.27v0.01h-0.02c-0.01,3.84-1.57,7.33-4.1,9.86c-2.51,2.5-5.98,4.06-9.82,4.07v0.02h-0.02h-61.7H40.1v-0.02 c-3.84-0.01-7.34-1.57-9.86-4.1c-2.5-2.51-4.06-5.98-4.07-9.82h-0.02v-0.02V92.51H13.96h-0.01v-0.02c-3.84-0.01-7.34-1.57-9.86-4.1 c-2.5-2.51-4.06-5.98-4.07-9.82H0v-0.02V13.96v-0.01h0.02c0.01-3.85,1.58-7.34,4.1-9.86c2.51-2.5,5.98-4.06,9.82-4.07V0h0.02h61.7 h0.01v0.02c3.85,0.01,7.34,1.57,9.86,4.1c2.5,2.51,4.06,5.98,4.07,9.82h0.02V13.96L89.62,13.96z M79.04,21.69v-7.73v-0.02h0.02 c0-0.91-0.39-1.75-1.01-2.37c-0.61-0.61-1.46-1-2.37-1v0.02h-0.01h-61.7h-0.02v-0.02c-0.91,0-1.75,0.39-2.37,1.01 c-0.61,0.61-1,1.46-1,2.37h0.02v0.01v64.59v0.02h-0.02c0,0.91,0.39,1.75,1.01,2.37c0.61,0.61,1.46,1,2.37,1v-0.02h0.01h12.19V35.65 v-0.01h0.02c0.01-3.85,1.58-7.34,4.1-9.86c2.51-2.5,5.98-4.06,9.82-4.07v-0.02h0.02H79.04L79.04,21.69z M105.18,108.92V35.65v-0.02 h0.02c0-0.91-0.39-1.75-1.01-2.37c-0.61-0.61-1.46-1-2.37-1v0.02h-0.01h-61.7h-0.02v-0.02c-0.91,0-1.75,0.39-2.37,1.01 c-0.61,0.61-1,1.46-1,2.37h0.02v0.01v73.27v0.02h-0.02c0,0.91,0.39,1.75,1.01,2.37c0.61,0.61,1.46,1,2.37,1v-0.02h0.01h61.7h0.02 v0.02c0.91,0,1.75-0.39,2.37-1.01c0.61-0.61,1-1.46,1-2.37h-0.02V108.92L105.18,108.92z"/>
                                        </svg>
                                    </span>
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
                                <li>Hạn thanh toán: <strong>${ORDER_DEADLINE_HOURS} giờ</strong></li>
                            </ul>
                        </div>
                    </div>

                    <!-- Buttons -->
                    <div style='display: flex; gap: 10px; justify-content: center;'>
                        <button class='btn-view-order' style='
                            padding: 10px 20px;
                            background: #007bff;
                            color: white;
                            border: none;
                            border-radius: 4px;
                            cursor: pointer;
                            font-size: 0.9rem;
                        '>Xem đơn hàng</button>
                        <button class='btn-go-home' style='
                            padding: 10px 20px;
                            background: #6c757d;
                            color: white;
                            border: none;
                            border-radius: 4px;
                            cursor: pointer;
                            font-size: 0.9rem;
                        '>Về trang chủ</button>
                    </div>
                </div>
            </div>
        `;

        document.getElementsByTagName("body")[0].appendChild(container);

        let modal = container.getElementsByClassName("my-modal-must-click-ok")[0];
        modal.focus();

        // Add copy event listeners
        container.getElementsByClassName("copy-icon-ordercode")[0].addEventListener("click", function() {
            copyToClipboard(orderCode, this);
        });

        container.getElementsByClassName("copy-icon-account")[0].addEventListener("click", function() {
            copyToClipboard(bankAccount.AccountNumber, this);
        });

        container.getElementsByClassName("copy-icon-content")[0].addEventListener("click", function() {
            copyToClipboard(orderCode, this);
        });

        container.getElementsByClassName("btn-view-order")[0].addEventListener("click", function () {
            container.remove();
            location.replace("/Customer/Order");
            resolve("view-order");
        });

        container.getElementsByClassName("btn-go-home")[0].addEventListener("click", function () {
            container.remove();
            location.replace("/");
            resolve("home");
        });
    });
}

/**
 * Hiển thị modal đặt hàng thành công với 2 lựa chọn
 * @param {string} orderCode - Mã đơn hàng (format: YYMMDD-XXXXX)
 */
async function CreateOrderSuccessModal(orderCode) {
    return new Promise((resolve) => {
        // Tạo modal container
        let container = document.createElement("div");
        container.className = "container-my-modal-must-click-ok";
        container.innerHTML = `
            <div tabindex='0' class='my-modal-must-click-ok'>
                <div class='modal-content-selected' style='padding: 30px 20px;'>
                    <div style='text-align: center; margin-bottom: 20px;'>
                        <div style='font-size: 1.5rem; color: #28a745; margin-bottom: 15px;'>
                            ✅ Đặt hàng thành công!
                        </div>
                        <div style='font-size: 1.1rem; color: #333; margin-bottom: 10px; display: flex; align-items: center; justify-content: center; gap: 8px;'>
                            📦 Mã đơn hàng: <strong>${orderCode}</strong>
                            <span class='copy-icon-ordercode-cod' style='cursor: pointer; color: #007bff; user-select: none; display: inline-flex; align-items: center;' title='Copy mã đơn hàng'>
                                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 115.77 122.88" fill="currentColor">
                                    <path d="M89.62,13.96v7.73h12.19h0.01v0.02c3.85,0.01,7.34,1.57,9.86,4.1c2.5,2.51,4.06,5.98,4.07,9.82h0.02v0.02 v73.27v0.01h-0.02c-0.01,3.84-1.57,7.33-4.1,9.86c-2.51,2.5-5.98,4.06-9.82,4.07v0.02h-0.02h-61.7H40.1v-0.02 c-3.84-0.01-7.34-1.57-9.86-4.1c-2.5-2.51-4.06-5.98-4.07-9.82h-0.02v-0.02V92.51H13.96h-0.01v-0.02c-3.84-0.01-7.34-1.57-9.86-4.1 c-2.5-2.51-4.06-5.98-4.07-9.82H0v-0.02V13.96v-0.01h0.02c0.01-3.85,1.58-7.34,4.1-9.86c2.51-2.5,5.98-4.06,9.82-4.07V0h0.02h61.7 h0.01v0.02c3.85,0.01,7.34,1.57,9.86,4.1c2.5,2.51,4.06,5.98,4.07,9.82h0.02V13.96L89.62,13.96z M79.04,21.69v-7.73v-0.02h0.02 c0-0.91-0.39-1.75-1.01-2.37c-0.61-0.61-1.46-1-2.37-1v0.02h-0.01h-61.7h-0.02v-0.02c-0.91,0-1.75,0.39-2.37,1.01 c-0.61,0.61-1,1.46-1,2.37h0.02v0.01v64.59v0.02h-0.02c0,0.91,0.39,1.75,1.01,2.37c0.61,0.61,1.46,1,2.37,1v-0.02h0.01h12.19V35.65 v-0.01h0.02c0.01-3.85,1.58-7.34,4.1-9.86c2.51-2.5,5.98-4.06,9.82-4.07v-0.02h0.02H79.04L79.04,21.69z M105.18,108.92V35.65v-0.02 h0.02c0-0.91-0.39-1.75-1.01-2.37c-0.61-0.61-1.46-1-2.37-1v0.02h-0.01h-61.7h-0.02v-0.02c-0.91,0-1.75,0.39-2.37,1.01 c-0.61,0.61-1,1.46-1,2.37h0.02v0.01v73.27v0.02h-0.02c0,0.91,0.39,1.75,1.01,2.37c0.61,0.61,1.46,1,2.37,1v-0.02h0.01h61.7h0.02 v0.02c0.91,0,1.75-0.39,2.37-1.01c0.61-0.61,1-1.46,1-2.37h-0.02V108.92L105.18,108.92z"/>
                                </svg>
                            </span>
                        </div>
                        <div style='font-size: 0.95rem; color: #666; margin-bottom: 5px;'>
                            Cảm ơn bạn đã mua hàng tại shop!
                        </div>
                    </div>
                    <div style='display: flex; gap: 10px; justify-content: center; margin-top: 25px;'>
                        <button class='btn-view-order' style='
                            padding: 10px 20px;
                            background: #007bff;
                            color: white;
                            border: none;
                            border-radius: 4px;
                            cursor: pointer;
                            font-size: 0.95rem;
                        '>Xem đơn hàng</button>
                        <button class='btn-go-home' style='
                            padding: 10px 20px;
                            background: #6c757d;
                            color: white;
                            border: none;
                            border-radius: 4px;
                            cursor: pointer;
                            font-size: 0.95rem;
                        '>Về trang chủ</button>
                    </div>
                </div>
            </div>
        `;

        // Thêm vào body
        document.getElementsByTagName("body")[0].appendChild(container);

        // Focus modal
        let modal = container.getElementsByClassName("my-modal-must-click-ok")[0];
        modal.focus();

        // Add copy event listener for COD order code
        container.getElementsByClassName("copy-icon-ordercode-cod")[0].addEventListener("click", function() {
            copyToClipboard(orderCode, this);
        });

        // Button "Xem đơn hàng"
        container.getElementsByClassName("btn-view-order")[0].addEventListener("click", function () {
            container.remove();
            location.replace("/Customer/Order");
            resolve("view-order");
        });

        // Button "Về trang chủ"
        container.getElementsByClassName("btn-go-home")[0].addEventListener("click", function () {
            container.remove();
            location.replace("/");
            resolve("home");
        });
    });
}

function CreateCheckoutSelectedModel(containerModel, sample) {
    let length = listCartObject.length;

    // Sinh bản sao
    for (let i = 0; i < length; i++) {
        let obj = listCartObject[i];

        let clone = sample.cloneNode(true);
        clone.setAttribute("data-model-id", obj.id.toString());

        // Cập nhật dữ liệu bản sao
        clone.getElementsByClassName("rTOisL")[0].src = Get320VersionOfImageSrc(GetSanPhamMediaUrl(obj.sanPhamBasicInfo.Id, obj.sanPhamBasicInfo.CoverImageFileName));
        clone.getElementsByClassName("rTOisL")[0].alt = obj.sanPhamBasicInfo.Name;
        clone.getElementsByClassName("item-name")[0].innerHTML = obj.sanPhamBasicInfo.Name;

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

        clone.getElementsByClassName("quantity-model")[0].innerHTML = obj.quantity;

        containerModel.appendChild(clone);
    }

    ShowCheckoutMoney();
}

function ShowErrorWhenLoadCart(error) {
    console.error('❌ Error in Checkout:', error);

    // Hiển thị lỗi cho user
    document.getElementsByClassName("cart-empty")[0].style.display = "flex";
    document.getElementsByClassName("main-container")[0].style.display = "none";

    CreateMustClickOkModal('Có lỗi khi tải giỏ hàng. Vui lòng thử lại sau.', null);
}

async function ShowCheckoutCartList() {
    // Làm mới nội dung
    document.getElementsByClassName("model-container")[0].innerHTML = "";

    if (listCartObject == null || listCartObject.length == 0) {
        document.getElementsByClassName("cart-empty")[0].style.display = "flex";
        document.getElementsByClassName("main-container")[0].style.display = "none";
        return;
    }
    // Có những sản phẩm số lượng trong kho nhỏ hơn số lượng khách đã chọn,
    // nhưng được tính lại phía server

    // Lấy mẫu
    let sample = document.getElementsByClassName("sample-model")[0].firstElementChild;
    let containerModel = document.getElementsByClassName("model-container")[0];
    CreateCheckoutSelectedModel(containerModel, sample);

    document.getElementsByClassName("cart-empty")[0].style.display = "none";
    document.getElementsByClassName("main-container")[0].style.display = "block";
}

async function LoadCheckoutCart() {
    try {
        let responseText = await CheckoutPageLoadCart();

        // Sau khi load cart với dữ liệu đầy đủ, set real = 0 với khách vãng lai và xóa cart với khách đăng nhập
        CartManager.setRealZeroOrClear();

        listCartObject = JSON.parse(responseText);

    } catch (error) {
        ShowErrorWhenLoadCart(error);
    }
}

// Hiển thị tiền
function ShowCheckoutMoney() {
    let length = listCartObject.length;
    let totalMoney = 0;
    for (let i = 0; i < length; i++) {
        totalMoney = totalMoney + listCartObject[i].quantity * listCartObject[i].sanPhamBasicInfo.SalePrice;
    }

    // Hiển thị tổng tiền hàng
    document.getElementsByClassName("model-money-sum")[0].innerHTML =
        ConvertMoneyToTextWithIcon(totalMoney);

    let shipFeeValue = GetShipFee();
    // Tính giảm giá phí ship
    let shipFeeDiscount = 0;       // Giảm phí ship (Type = 0)

    // Đã có địa chỉ
    // Hiển thị phí ship và giảm giá phí ship
    const shipFeeElement = document.getElementsByClassName("shipee-fee")[0];
    const freeShipFeeElement = document.getElementsByClassName("free-shipee-fee")[0];
    if (currentIndexInforObject != -1) {

        shipFeeElement.innerHTML = ConvertMoneyToTextWithIcon(shipFeeValue);

        shipFeeElement.closest('.checkout-money-flex-end').style.display = '';

        if (listOrderSimplePromotionsObject && listOrderSimplePromotionsObject.length > 0) {
            listOrderSimplePromotionsObject.find(promo => {
                if (promo.Type === EOrderSimplePromotionType.SHIP_DISCOUNT) {
                    // Type 0: Miễn phí ship
                    if (totalMoney >= promo.MinOrderValue) {
                        shipFeeDiscount = shipFeeValue * -1; // Giảm bằng phí ship
                    }
                }
            });
        }

        // Hiển thị/Ẩn giảm phí ship
        if (shipFeeDiscount < 0) {
            freeShipFeeElement.innerHTML = ConvertMoneyToTextWithIcon(shipFeeDiscount);
            freeShipFeeElement.closest('.checkout-money-flex-end').style.display = '';
        } else {
            freeShipFeeElement.closest('.checkout-money-flex-end').style.display = 'none';
        }
    }
    else {
        shipFeeElement.closest('.checkout-money-flex-end').style.display = 'none';
        freeShipFeeElement.closest('.checkout-money-flex-end').style.display = 'none';
    }

    // Tính giảm tổng tiền hàng
    let totalMoneyDiscount = 0;     // Giảm tổng tiền hàng (Type = 1)

    if (listOrderSimplePromotionsObject && listOrderSimplePromotionsObject.length > 0) {
        listOrderSimplePromotionsObject.find(promo => {
            if (promo.Type === EOrderSimplePromotionType.TOTAL_DISCOUNT) {
                // Type 1: Giảm theo bậc 100k
                if (totalMoney >= promo.MinOrderValue) {
                    let extraAmount = totalMoney - promo.MinOrderValue;
                    let multiplier = Math.floor(extraAmount / 100000) + 1;
                    totalMoneyDiscount = multiplier * promo.Discount * -1;
                }
            }
        });
    }

    // Hiển thị/Ẩn giảm tổng tiền hàng
    const discountSumElements = document.getElementsByClassName("discount-final-money");
    if (discountSumElements.length > 0) {
        if (totalMoneyDiscount < 0) {
            discountSumElements[0].innerHTML = ConvertMoneyToTextWithIcon(totalMoneyDiscount);
            discountSumElements[0].closest('.checkout-money-flex-end').style.display = '';
        } else {
            discountSumElements[0].closest('.checkout-money-flex-end').style.display = 'none';
        }
    }

    // Tổng giảm giá
    let totalDiscount = shipFeeDiscount + totalMoneyDiscount;

    // Tổng thanh toán = Tổng tiền hàng + Phí ship - Giảm giá
    let finalAmount = totalMoney + shipFeeValue + totalDiscount;


    // Hiển thị tổng thanh toán
    document.getElementsByClassName("final-money")[0].innerHTML =
        ConvertMoneyToTextWithIcon(finalAmount);

    // Debug log
    if (DEBUG && totalDiscount < 0) {
        console.log("🎁 Khuyến mãi:");
        if (shipFeeDiscount < 0) console.log(`  ✓ Miễn phí ship: ${shipFeeDiscount.toLocaleString()}đ`);
        if (totalMoneyDiscount < 0) console.log(`  ✓ Giảm tiền hàng: ${totalMoneyDiscount.toLocaleString()}đ`);
        console.log(`  💰 Tổng giảm: ${totalDiscount.toLocaleString()}đ`);
    }

    // Tạo list thanh toán: tổng tiền hàng, phí ship, tổng thanh toán
    /// 0: Tổng tiền hàng
    /// 1: Phí ship
    /// 2: Khuyến mại khác
    /// 10: Tổng thanh toán = Tổng tiền hàng + Phí ship - Khuyến mại khác
    listOrderPay = [];
    // Tổng tiền hàng
    listOrderPay.push(new objOrderPay(EOrderPayType.TOTAL, "Tổng tiền hàng", totalMoney, null));

    // Phí ship
    listOrderPay.push(new objOrderPay(EOrderPayType.SHIP, "Phí vận chuyển", shipFeeValue, null ));

    // Giảm giá phí ship
    listOrderPay.push(new objOrderPay(EOrderPayType.PROMOTION, "Giảm phí ship", shipFeeDiscount, listOrderSimplePromotionsObject.find(p => p.Type === 0)));

    // Giảm giá tổng tiền hàng theo bậc 100k
    listOrderPay.push(new objOrderPay(EOrderPayType.PROMOTION, "Mỗi 100k giảm thêm", totalMoneyDiscount, listOrderSimplePromotionsObject.find(p => p.Type === 1)));

    // Tổng thanh toán
    listOrderPay.push(new objOrderPay(EOrderPayType.FINAL, "Tổng thanh toán", finalAmount, null));
}

function GetShipFee() {
    let fee = 0;
    if (currentIndexInforObject != -1) {
        let obj = listCustomerInforObject[currentIndexInforObject];
        if (obj.province == HaNoiCity) {
            fee = standardShipFeeInHaNoi;
        }
        else {
            fee = standardShipFeeOutHaNoi;
        }
    }
    return fee;
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
        // Lấy từ localStorage
        listCustomerInforObject = GetListCustomerInforFromLocalStorage();
    }
    else {
        let res = await GetListAddress();
        if (JSON.parse(res.responseText) == null) {
            await CreateMustClickOkModal("Không lấy được dữ liệu. Vui lòng thử lại sau.", null);
            // Trả về định dạng giống truy vấn httpPost
            return GetEasyPromise();
        }
        else {
            listCustomerInforObject = JSON.parse(res.responseText);
        }
    }

    let isDefaultAdd = false;
    // Tìm địa chỉ mặc định
    for (let i = listCustomerInforObject.length - 1; i >= 0; i--) {
        let obj = listCustomerInforObject[i];
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

        if (listCustomerInforObject.length > 0) {
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
        obj.detail + ", " + obj.subdistrict + ", " + obj.province;
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

    let length = listCustomerInforObject.length;

    for (let i = 0; i < length; i++) {
        let obj = listCustomerInforObject[i];
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
            obj.subdistrict + ", " + obj.province;

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

    let obj = CreateAddressObjFromInput();
    if (currentIndexInforUpdateObject != -1) {// Cập nhật thông tin vào object
        obj.id = listCustomerInforObject[currentIndexInforUpdateObject].id;
        listCustomerInforObject[currentIndexInforUpdateObject] = obj;
        //Kiểm tra có đặt mặc định
        if (obj.defaultAdd) {
            // Bỏ mặc định cũ nếu có
            for (let i = listCustomerInforObject.length - 1; i >= 0; i--) {
                if (listCustomerInforObject[i].defaultAdd
                    && i != currentIndexInforUpdateObject) {
                    listCustomerInforObject[i].defaultAdd = 0;
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
            for (let i = listCustomerInforObject.length - 1; i >= 0; i--) {
                if (listCustomerInforObject[i].defaultAdd) {
                    listCustomerInforObject[i].defaultAdd = 0;
                }
            }
        }
        listCustomerInforObject.push(obj);
        // Chọn địa chỉ vừa thêm mới làm địa chỉ nhận hàng
        currentIndexInforObject = listCustomerInforObject.length - 1;
    }

    if (CheckAnonymousCustomer()) {// Khách vãng lai
        SaveListCustomerInforToLocalStorage(listCustomerInforObject);
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
            listCustomerInforObject[listCustomerInforObject.length - 1].id = resObj.myAnything;
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
    ShowCustomerInforFromObj(listCustomerInforObject[currentIndexInforObject]);

    // Tính lại tiền
    ShowCheckoutMoney();
}

// Xác nhận dùng thông tin nào nhận hàng
function ConfirmListCustomerInforModal() {
    let list = document.getElementsByClassName("list-customer-infor-container")[0].children;
    let length = list.length;
    for (let i = 0; i < length; i++) {
        let ele = list[i];

        if (ele.getElementsByClassName("address-radio")[0].checked) {
            currentIndexInforObject = parseInt(ele.getAttribute("data-index"));
            ShowCustomerInforFromObj(listCustomerInforObject[currentIndexInforObject]);
            break;
        }
    }
    DestroyListCustomerInforModal();

    // Tính lại tiền
    ShowCheckoutMoney();
}

function UpdateCustomerInfor(ele) {
    currentIndexInforUpdateObject = parseInt(ele.parentElement.parentElement.getAttribute("data-index"));
    ShowCustomerInforModal(false, true, listCustomerInforObject[currentIndexInforUpdateObject]);
}

function objOrderPay(type, strType, value, orderSimplePromotion) {
    this.type = type;
    this.strType = strType;
    this.value = value;
    this.orderSimplePromotion = orderSimplePromotion;
}

// Check lại thông tin đơn hàng bên phía server: mã sản phẩm, số lượng có đủ
// (vì trong thời gian khách chọn có thể số lượng tồn kho thay đổi), tổng tiền hàng, tiền ship.
// Đồng thời gửi thông tin địa chỉ, lời nhắn shop
async function CheckOrderOnSever() {
    const searchParams = new URLSearchParams();

    // Test thông báo
    {
        listCartObject.forEach(item => {
            //item.sanPhamId = item.sanPhamId + 10000;
            //item.quantity = item.quantity + 1000; // Reset real = 0 trước
            //item.sanPhamBasicInfo.SalePrice++;
        });

        listOrderPay.forEach(item => {
            //item.value++;
        });
    }
    searchParams.append("cart", JSON.stringify(listCartObject));
    searchParams.append("customerInfor", JSON.stringify(listCustomerInforObject[currentIndexInforObject]));

    searchParams.append("listOrderPay", JSON.stringify(listOrderPay));
    searchParams.append("noteToShop", document.getElementsByClassName("gQuJxM")[0].value);

    // Lấy payment method từ radio button
    const paymentMethodRadio = document.querySelector('input[name="payment-method"]:checked');
    const paymentMethod = paymentMethodRadio.value === 'bank' ? EPaymentMethod.BANK_TRANSFER : EPaymentMethod.CASH_ON_DELIVERY;
    searchParams.append("paymentMethod", paymentMethod);

    let query = "/Home/CheckOrderOnSever";

    return await RequestHttpPostPromise(searchParams, query);

}

async function CheckOutOrder() {
    if (currentIndexInforObject == -1) {
        await CreateMustClickOkModal("Vui lòng cung cấp thông tin người nhận hàng.", null)
        return;
    }

    ShowCircleLoader();
    let responseDB = await CheckOrderOnSever();
    RemoveCircleLoader();

    let result = JSON.parse(responseDB.responseText);
    if (result == null) {
        await CreateMustClickOkModal("Có lỗi. Vui lòng thử lại sau.");
        return;
    }
    if (result.State != 0) {
        await CreateMustClickOkModal(result.Message);
        return;
    }
    // Đặt hàng thành công
    // Với khách vãng lai xóa những sản phẩm vừa đặt mua
    if (CheckAnonymousCustomer()) {
        let guestCart = CartManager.getCart();

        // Lấy danh sách sanPhamId của những sản phẩm vừa mua
        const purchasedSanPhamIds = listCartObject.map(item => item.sanPhamId);

        // Lọc bỏ những sản phẩm có sanPhamId trong danh sách vừa mua
        const beforeCount = guestCart.length;
        guestCart = guestCart.filter(item => !purchasedSanPhamIds.includes(item.sanPhamId));

        CartManager.saveCart(guestCart);

        if (DEBUG) {
            console.log(`🛒 Đã xóa ${beforeCount - guestCart.length} sản phẩm khỏi cart`);
            console.log(`   Purchased IDs: ${purchasedSanPhamIds.join(', ')}`);
        }
    }

    // Lấy orderCode từ response
    let orderCode = result.Message;

    // Lưu mã đơn và thời gian đặt đơn vào localStorage cho khách vãng lai
    if (CheckAnonymousCustomer()) {
        SaveGuestOrderToLocalStorage(orderCode);
    }

    // Nếu thanh toán bằng chuyển khoản, hiển thị modal với QR code
    if (result.PaymentMethod === EPaymentMethod.BANK_TRANSFER && result.QRCodeUrl) {
        const paymentInfo = {
            orderCode: orderCode,
            qrCodeUrl: result.QRCodeUrl,
            bankAccount: result.BankAccount,
            totalAmount: result.TotalAmount
        };

        await CreateBankTransferPaymentModal(paymentInfo);
    } else {
        // COD - hiển thị modal thông thường
        await CreateOrderSuccessModal(orderCode);
    }
}

// Initial load
window.addEventListener('DOMContentLoaded', async function () {
    if (DEBUG) {
        console.log("🚀 DOMContentLoaded - Initial load");
    }

    // Load tất cả data song song (nhanh hơn 2x)
    await Promise.all([
        LoadCustomerInfor(),  // Load địa chỉ để tính phí ship
        LoadCheckoutCart(),           // Load giỏ hàng
        LoadPromotions()      // Load chương trình giảm giá
    ]);

    if (DEBUG) {
        console.log("✓ Loaded: Customer Info, Cart, Promotions");
    }

    // Hiển thị cart sau khi đã load xong tất cả data
    await ShowCheckoutCartList();

    // Toggle bank transfer info khi chọn payment method
    const paymentCod = document.getElementById('payment-cod');
    const paymentBank = document.getElementById('payment-bank');
    const bankContainer = document.querySelector('.bank-transfer-container');

    function updateBankContainerVisibility() {
        if (paymentBank.checked) {
            bankContainer.style.display = 'block';
        } else {
            bankContainer.style.display = 'none';
        }
    }

    paymentCod.addEventListener('change', updateBankContainerVisibility);
    paymentBank.addEventListener('change', updateBankContainerVisibility);
});
