// ========================================
// GLOBAL VARIABLES
// ========================================

let allOrders = [];         // Tất cả orders từ DB
let filteredOrders = [];    // Orders sau khi filter
let currentPage = 1;
let pageSize = 20;
let sortColumn = 'time';
let sortDirection = 'desc';
let currentEditingOrderId = null;

// ========================================
// INITIALIZATION
// ========================================

// Vanilla JavaScript - chạy khi DOM đã load xong
document.addEventListener('DOMContentLoaded', function () {
    InitializeDateFilters();
    LoadAllOrders();  // Load với default 30 ngày
});

// Set default date range (last 30 days)
function InitializeDateFilters() {
    SetDateRange(30);  // Mặc định 30 ngày
}

// Set date range dựa trên số ngày
function SetDateRange(days) {
    let today = new Date();
    let fromDate = new Date();

    if (days === 'all') {
        // Tất cả - set từ rất xa (1 năm trước)
        fromDate.setFullYear(today.getFullYear() - 1);
    } else {
        fromDate.setDate(today.getDate() - parseInt(days));
    }

    document.getElementById('date-to').valueAsDate = today;
    document.getElementById('date-from').valueAsDate = fromDate;
}

// Handle khi click radio button
function OnDateRangeChange(radio) {
    let value = radio.value;
    SetDateRange(value);
    // KHÔNG auto load - user phải click "Tải dữ liệu"
}

// Handle khi user tự nhập date
function OnManualDateChange() {
    // Bỏ check radio nếu user tự nhập
    let radios = document.querySelectorAll('input[name="date-range"]');
    radios.forEach(r => r.checked = false);
    // KHÔNG auto load - user phải click "Tải dữ liệu"
}

// ========================================
// DATA LOADING
// ========================================

async function LoadAllOrders() {
    try {
        ShowLoading();

        // Lấy date range từ inputs
        let dateFrom = document.getElementById('date-from').value;
        let dateTo = document.getElementById('date-to').value;

        // Build request data
        let data = {};
        if (dateFrom) data.dateFrom = dateFrom;
        if (dateTo) data.dateTo = dateTo;

        // Dùng PostJSON từ common.js
        let responseText = await PostJSON('/Administrator/GetAllOrdersAdmin', data);
        let result = JSON.parse(responseText);

        if (result.State === 0) {  // OK
            allOrders = result.myJson || [];
            filteredOrders = [...allOrders];

            UpdateStats();
            ApplyFilters();
        } else {
            CreateMustClickOkModal(result.Message || 'Lỗi tải dữ liệu', null);
        }
    } catch (error) {
        console.error('LoadAllOrders error:', error);
        CreateMustClickOkModal('Lỗi kết nối server', null);
    } finally {
        HideLoading();
    }
}

// ========================================
// FILTERING
// ========================================

function ApplyFilters() {
    let dateFrom = document.getElementById('date-from').value;
    let dateTo = document.getElementById('date-to').value;
    let searchValue = document.getElementById('search-input').value.trim().toLowerCase();
    let statusFilter = document.querySelector('.status-filter-btn.active')?.dataset.status;

    filteredOrders = allOrders.filter(order => {
        // Date filter
        if (dateFrom) {
            let orderDate = new Date(order.time);
            let fromDate = new Date(dateFrom);
            if (orderDate < fromDate) return false;
        }

        if (dateTo) {
            let orderDate = new Date(order.time);
            let toDate = new Date(dateTo);
            toDate.setHours(23, 59, 59, 999);  // End of day
            if (orderDate > toDate) return false;
        }

        // Status filter
        if (statusFilter && statusFilter !== 'all') {
            // Filter by payment status (0=PENDING)
            if (statusFilter === '0' && order.OrderPayStatus !== 0) return false;
            // Filter by order status
            if (statusFilter !== '0' && order.OrderStatus !== parseInt(statusFilter) - 1) return false;
        }

        // Search filter
        if (searchValue) {
            let fullAddress = [order.province, order.subdistrict, order.detail].filter(Boolean).join(' ');
            let searchableText = [
                order.code,
                order.name,
                order.phone,
                fullAddress
            ].join(' ').toLowerCase();

            if (!searchableText.includes(searchValue)) return false;
        }

        return true;
    });

    // Reset to page 1 when filtering
    currentPage = 1;

    UpdateStats();
    RenderTable();
}

function FilterByStatus(button, status) {
    // Update button UI
    document.querySelectorAll('.status-filter-btn').forEach(btn => {
        btn.classList.remove('active');
    });
    button.classList.add('active');

    ApplyFilters();
}

function ResetFilters() {
    // Reset date
    InitializeDateFilters();

    // Reset search
    document.getElementById('search-input').value = '';

    // Reset status filter to "Tất cả"
    document.querySelectorAll('.status-filter-btn').forEach(btn => {
        btn.classList.remove('active');
    });
    document.querySelector('.status-filter-btn[data-status="all"]').classList.add('active');

    ApplyFilters();
}

// ========================================
// SORTING
// ========================================

function SortTable(column) {
    if (sortColumn === column) {
        sortDirection = sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
        sortColumn = column;
        sortDirection = 'desc';
    }

    filteredOrders.sort((a, b) => {
        let valueA = a[column];
        let valueB = b[column];

        // Handle null/undefined
        if (valueA == null) return 1;
        if (valueB == null) return -1;

        // String comparison
        if (typeof valueA === 'string') {
            valueA = valueA.toLowerCase();
            valueB = valueB.toLowerCase();
        }

        if (sortDirection === 'asc') {
            return valueA > valueB ? 1 : -1;
        } else {
            return valueA < valueB ? 1 : -1;
        }
    });

    UpdateSortIcons();
    RenderTable();
}

function UpdateSortIcons() {
    document.querySelectorAll('.sort-icon').forEach(icon => {
        icon.textContent = '↕️';
    });

    let activeHeader = document.querySelector(`th[onclick*="${sortColumn}"]`);
    if (activeHeader) {
        let icon = activeHeader.querySelector('.sort-icon');
        icon.textContent = sortDirection === 'asc' ? '↑' : '↓';
    }
}

// ========================================
// TABLE RENDERING
// ========================================

function RenderTable() {
    let tbody = document.getElementById('order-table-body');
    let emptyState = document.getElementById('empty-state');
    let tableContainer = document.querySelector('.table-container');

    if (filteredOrders.length === 0) {
        tbody.innerHTML = '';
        tableContainer.style.display = 'none';
        emptyState.style.display = 'flex';
        document.getElementById('pagination-container').innerHTML = '';
        return;
    }

    tableContainer.style.display = 'block';
    emptyState.style.display = 'none';

    // Pagination
    let start = (currentPage - 1) * pageSize;
    let end = start + pageSize;
    let pageOrders = filteredOrders.slice(start, end);

    // Render rows
    let html = '';
    pageOrders.forEach(order => {
        let finalMoney = GetFinalMoney(order);
        let orderCode = order.code || 'N/A';
        let customerName = order.name || 'N/A';
        let customerPhone = order.phone || 'N/A';

        html += `
            <tr>
                <td>${order.id}</td>
                <td>
                    <strong>${orderCode}</strong>
                    ${orderCode !== 'N/A' ? `<button class="btn-copy" onclick="CopyToClipboard('${orderCode}', this)" title="Copy mã đơn">📋</button>` : ''}
                </td>
                <td>
                    ${customerName}
                    ${customerName !== 'N/A' ? `<button class="btn-copy" onclick="CopyToClipboard('${customerName}', this)" title="Copy tên">📋</button>` : ''}
                </td>
                <td>
                    ${customerPhone}
                    ${customerPhone !== 'N/A' ? `<button class="btn-copy" onclick="CopyToClipboard('${customerPhone}', this)" title="Copy SĐT">📋</button>` : ''}
                </td>
                <td class="money-cell">${ConvertMoneyToTextWithIcon(finalMoney)}</td>
                <td>${GetPaymentStatusBadge(order.OrderPayStatus)}</td>
                <td>${GetOrderStatusBadge(order.OrderStatus)}</td>
                <td>${GetFormattedDate(new Date(order.time))}</td>
                <td>
                    <button class="btn-action btn-edit" onclick="OpenEditModal(${order.id})" title="Sửa">
                        ✏️
                    </button>
                    <button class="btn-action btn-view" onclick="ViewOrderDetail(${order.id})" title="Xem">
                        👁️
                    </button>
                </td>
            </tr>
        `;
    });

    tbody.innerHTML = html;

    RenderPagination();
}

// Helper: Get final money from lsOrderPay (type = EOrderPayType.FINAL = 10)
function GetFinalMoney(order) {
    if (!order.lsOrderPay || order.lsOrderPay.length === 0) return 0;

    let finalPayItem = order.lsOrderPay.find(pay => pay.type === 10);  // EOrderPayType.FINAL
    return finalPayItem ? finalPayItem.value : 0;
}

function GetPaymentStatusBadge(payStatus) {
    const statusMap = {
        0: { text: 'Chờ TT', class: 'badge-warning' },
        1: { text: 'Đã TT', class: 'badge-success' },
        2: { text: 'Hoàn tiền', class: 'badge-info' }
    };

    let status = statusMap[payStatus] || { text: 'N/A', class: 'badge-secondary' };
    return `<span class="badge ${status.class}">${status.text}</span>`;
}

function GetOrderStatusBadge(orderStatus) {
    const statusMap = {
        0: { text: 'Chuẩn bị', class: 'badge-warning' },
        1: { text: 'Đang giao', class: 'badge-info' },
        2: { text: 'Đã nhận', class: 'badge-success' },
        3: { text: 'Đã hủy', class: 'badge-danger' },
        4: { text: 'Hoàn về', class: 'badge-secondary' },
        5: { text: 'Hoàn thành', class: 'badge-success' }
    };

    let status = statusMap[orderStatus] || { text: 'N/A', class: 'badge-secondary' };
    return `<span class="badge ${status.class}">${status.text}</span>`;
}

// ========================================
// PAGINATION
// ========================================

function RenderPagination() {
    let totalPages = Math.ceil(filteredOrders.length / pageSize);
    let container = document.getElementById('pagination-container');

    if (totalPages <= 1) {
        container.innerHTML = '';
        return;
    }

    let html = '';

    // Previous button
    html += `
        <button class="page-btn" onclick="ChangePage(${currentPage - 1})" ${currentPage === 1 ? 'disabled' : ''}>
            &lt; Trước
        </button>
    `;

    // Page numbers (show max 7 pages)
    let startPage = Math.max(1, currentPage - 3);
    let endPage = Math.min(totalPages, currentPage + 3);

    if (startPage > 1) {
        html += `<button class="page-btn" onclick="ChangePage(1)">1</button>`;
        if (startPage > 2) html += `<span class="page-dots">...</span>`;
    }

    for (let i = startPage; i <= endPage; i++) {
        html += `
            <button class="page-btn ${i === currentPage ? 'active' : ''}" onclick="ChangePage(${i})">
                ${i}
            </button>
        `;
    }

    if (endPage < totalPages) {
        if (endPage < totalPages - 1) html += `<span class="page-dots">...</span>`;
        html += `<button class="page-btn" onclick="ChangePage(${totalPages})">${totalPages}</button>`;
    }

    // Next button
    html += `
        <button class="page-btn" onclick="ChangePage(${currentPage + 1})" ${currentPage === totalPages ? 'disabled' : ''}>
            Sau &gt;
        </button>
    `;

    container.innerHTML = html;
}

function ChangePage(newPage) {
    let totalPages = Math.ceil(filteredOrders.length / pageSize);
    if (newPage < 1 || newPage > totalPages) return;

    currentPage = newPage;
    RenderTable();
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

// ========================================
// STATISTICS
// ========================================

function UpdateStats() {
    document.getElementById('total-orders').textContent = allOrders.length;
    document.getElementById('filtered-orders').textContent = filteredOrders.length;
}

// ========================================
// EDIT MODAL
// ========================================

function OpenEditModal(orderId) {
    let order = allOrders.find(o => o.id === orderId);
    if (!order) return;

    currentEditingOrderId = orderId;

    document.getElementById('modal-order-code').value = order.code;
    document.getElementById('modal-order-status').value = order.OrderStatus;
    document.getElementById('modal-payment-status').value = order.OrderPayStatus;

    document.getElementById('edit-status-modal').style.display = 'flex';
}

function CloseEditModal() {
    document.getElementById('edit-status-modal').style.display = 'none';
    currentEditingOrderId = null;
}

async function SaveStatus() {
    if (!currentEditingOrderId) return;

    // Lấy order hiện tại để so sánh
    let order = allOrders.find(o => o.id === currentEditingOrderId);
    if (!order) {
        CreateMustClickOkModal('Không tìm thấy đơn hàng', null);
        return;
    }

    let newOrderStatus = parseInt(document.getElementById('modal-order-status').value);
    let newPaymentStatus = parseInt(document.getElementById('modal-payment-status').value);

    // Check xem có thay đổi không
    let orderStatusChanged = newOrderStatus !== order.OrderStatus;
    let paymentStatusChanged = newPaymentStatus !== order.OrderPayStatus;

    if (!orderStatusChanged && !paymentStatusChanged) {
        CreateMustClickOkModal('Không có thay đổi nào', null);
        return;
    }

    try {
        ShowLoading();

        let updateSuccess = true;
        let errorMessage = '';

        // Chỉ update order status nếu có thay đổi
        if (orderStatusChanged) {
            let orderData = {
                orderId: currentEditingOrderId,
                newStatus: newOrderStatus
            };
            let orderResponseText = await PostJSON('/Administrator/UpdateOrderStatus', orderData);
            let orderResult = JSON.parse(orderResponseText);

            if (orderResult.State !== 0) {
                updateSuccess = false;
                errorMessage = orderResult.Message || 'Lỗi cập nhật trạng thái đơn hàng';
            }
        }

        // Chỉ update payment status nếu có thay đổi
        if (paymentStatusChanged) {
            let paymentData = {
                orderId: currentEditingOrderId,
                newPaymentStatus: newPaymentStatus
            };
            let paymentResponseText = await PostJSON('/Administrator/UpdatePaymentStatus', paymentData);
            let paymentResult = JSON.parse(paymentResponseText);

            if (paymentResult.State !== 0) {
                updateSuccess = false;
                errorMessage = paymentResult.Message || 'Lỗi cập nhật trạng thái thanh toán';
            }
        }

        if (updateSuccess) {
            CloseEditModal();
            LoadAllOrders();  // Reload data
        } else {
            CreateMustClickOkModal(errorMessage, null);
        }
    } catch (error) {
        console.error('SaveStatus error:', error);
        CreateMustClickOkModal('Lỗi kết nối server', null);
    } finally {
        HideLoading();
    }
}

// ========================================
// VIEW ORDER DETAIL
// ========================================

function ViewOrderDetail(orderId) {
    // Navigate to admin order detail page
    window.open(`/Administrator/OrderDetail?id=${orderId}`, '_blank');
}

// ========================================
// LOADING INDICATOR
// ========================================

function ShowLoading() {
    // Reuse existing loading modal from common.js if available
    ShowCircleLoader();
}

function HideLoading() {
    RemoveCircleLoader()
}

// ========================================
// COPY TO CLIPBOARD
// ========================================

function CopyToClipboard(text, button) {
    // Dùng Clipboard API (modern browsers)
    if (navigator.clipboard && window.isSecureContext) {
        navigator.clipboard.writeText(text).then(() => {
            ShowCopyFeedback(button);
        }).catch(err => {
            console.error('Copy failed:', err);
            FallbackCopy(text, button);
        });
    } else {
        // Fallback cho browser cũ hoặc HTTP
        FallbackCopy(text, button);
    }
}

function FallbackCopy(text, button) {
    // Tạo textarea ẩn
    let textarea = document.createElement('textarea');
    textarea.value = text;
    textarea.style.position = 'fixed';
    textarea.style.opacity = '0';
    document.body.appendChild(textarea);
    textarea.select();

    try {
        document.execCommand('copy');
        ShowCopyFeedback(button);
    } catch (err) {
        console.error('Fallback copy failed:', err);
    }

    document.body.removeChild(textarea);
}

function ShowCopyFeedback(button) {
    // Đổi icon thành checkmark
    let originalText = button.innerHTML;
    button.innerHTML = '✅';
    button.style.color = '#28a745';

    // Reset sau 1.5s
    setTimeout(() => {
        button.innerHTML = originalText;
        button.style.color = '';
    }, 1500);
}
