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
        searchInput.placeholder = "Gõ 3 ký tự: mã đơn, tên, SDT";
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
    let minLength = isAnonymous ? 5 : 3;

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
            btn.style.color = "#ee4d2d";
            btn.style.borderBottom = "2px solid #ee4d2d";
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

        // Mã đơn
        clone.getElementsByClassName("order-code")[0].innerHTML = "MÃ . " + orderObj.OrderCode;

        // Trạng thái đơn
        // lấy đầu tiên vì query đã sort
        clone.getElementsByClassName("fxASnxvd")[0].innerHTML =
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
        if (orderObj.PaymentMethod == 1) {
            clone.getElementsByClassName("payment-method-text")[0].innerHTML = "Chuyển khoản ngân hàng";
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
// Initial Load - Gọi GetOrder() sau khi page load xong
// =============================================
window.addEventListener('DOMContentLoaded', async function () {
    // Set placeholder phù hợp với loại khách
    InitializeSearchPlaceholder();

    // Load danh sách đơn hàng
    await GetOrder();
});
