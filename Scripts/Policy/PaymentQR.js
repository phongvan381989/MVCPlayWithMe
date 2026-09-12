/**
 * PaymentQR.js - Xử lý sinh QR code thanh toán
 * Trang: /Policy/PaymentQR
 */

/**
 * Generate QR code từ form data
 * @param {Event} event - Form submit event
 */
async function generateQR(event) {
    event.preventDefault();

    let orderCode = document.getElementById('orderCode').value.trim();
    const amountText = document.getElementById('amount').value.trim();

    // Chuyển nội dung CK sang không dấu (QR code yêu cầu ASCII)
    if (orderCode) {
        orderCode = RemoveVietnameseDiacritics(orderCode);

        // Bỏ ký tự đặc biệt, chỉ giữ chữ cái, số, khoảng trắng, dấu gạch ngang
        orderCode = orderCode.replace(/[^a-zA-Z0-9\s\-]/g, '');

        // Giới hạn độ dài tối đa 100 ký tự
        if (orderCode.length > 100) {
            orderCode = orderCode.substring(0, 100);
        }
    }

    // Convert từ text có dấu phẩy (123,456) sang số (123456)
    let amount = amountText ? ConvertTextToMoney(amountText) : null;

    // Kiểm tra nếu số tiền < 0 thì gán = 0
    if (amount !== null && amount < 0) {
        amount = 0;
    }

    ShowCircleLoader();

    try {
        const responseText = await PostJSON('/Policy/GeneratePaymentQR', {
            orderCode: orderCode || null,
            amount: amount
        });

        const result = JSON.parse(responseText);

        RemoveCircleLoader();

        if (result.State !== 0) {
            CreateMustClickOkModal(result.Message || 'Có lỗi xảy ra');
            return;
        }

        // Hiển thị QR code
        document.getElementById('qrImage').src = result.QRCodeUrl;
        document.getElementById('displayAmount').textContent = result.Amount.toLocaleString() + 'đ';

        // Hiển thị order code nếu có
        if (result.OrderCode && result.OrderCode.trim() !== '') {
            document.getElementById('displayOrderCode').textContent = result.OrderCode;
            document.getElementById('orderCodeRow').style.display = 'flex';

            // Init copy icon cho order code
            initCopyOrderCodeQR();
        } else {
            document.getElementById('orderCodeRow').style.display = 'none';
        }

        // Hide form, show result
        document.getElementById('qrForm').style.display = 'none';
        document.getElementById('qrResult').classList.add('show');

    } catch (error) {
        RemoveCircleLoader();
        console.error('Error:', error);
        CreateMustClickOkModal('Có lỗi xảy ra. Vui lòng thử lại.');
    }
}

/**
 * Reset form về trạng thái ban đầu
 */
function resetForm() {
    document.getElementById('orderCode').value = '';
    document.getElementById('amount').value = '';
    document.getElementById('qrForm').style.display = 'block';
    document.getElementById('qrResult').classList.remove('show');
}


/**
 * Auto format số tiền khi nhập
 * Dùng ConvertMoneyToText từ web.play.with.me.common.js để format với dấu phẩy
 */
function initAmountFormatting() {
    const amountInput = document.getElementById('amount');
    if (amountInput) {
        amountInput.addEventListener('input', function (e) {
            // Lấy vị trí con trỏ trước khi format
            let cursorPos = e.target.selectionStart;
            let oldValue = e.target.value;

            // Chỉ giữ lại số
            let cleanValue = e.target.value.replace(/\D/g, '');

            if (cleanValue) {
                // Format với dấu phẩy bằng ConvertMoneyToText
                let formattedValue = ConvertMoneyToText(parseInt(cleanValue));
                e.target.value = formattedValue;

                // Điều chỉnh vị trí con trỏ sau khi format
                // Đếm số dấu phẩy được thêm vào
                let commasBefore = (oldValue.substring(0, cursorPos).match(/,/g) || []).length;
                let commasAfter = (formattedValue.substring(0, cursorPos).match(/,/g) || []).length;
                let newCursorPos = cursorPos + (commasAfter - commasBefore);

                // Set lại vị trí con trỏ
                e.target.setSelectionRange(newCursorPos, newCursorPos);
            } else {
                e.target.value = '';
            }
        });
    }
}

/**
 * Initialize copy icon cho số tài khoản
 */
function initCopyAccountNumber() {
    const iconContainer = document.getElementById('copy-icon-account');
    const accountDisplay = document.getElementById('account-number-display');

    if (iconContainer && accountDisplay) {
        // Tạo copy icon bằng hàm chung
        iconContainer.innerHTML = CreateCopyIcon({
            size: '16',
            color: 'white',
            title: 'Copy số tài khoản',
            className: 'copy-icon-account'
        });

        // Attach event listener
        const copyIcon = iconContainer.querySelector('.copy-icon-account');
        copyIcon.addEventListener('click', function() {
            const accountNumber = accountDisplay.textContent.trim();
            CopyWithVisualFeedback(accountNumber, this, {
                type: 'icon',
                successColor: '#28a745'
            });
        });
    }
}

/**
 * Initialize copy icon cho order code trong QR result
 */
function initCopyOrderCodeQR() {
    const iconContainer = document.getElementById('copy-icon-ordercode-qr');
    const orderCodeDisplay = document.getElementById('displayOrderCode');

    if (iconContainer && orderCodeDisplay) {
        // Tạo copy icon bằng hàm chung
        iconContainer.innerHTML = CreateCopyIcon({
            size: '16',
            color: '#007bff',
            title: 'Copy nội dung CK',
            className: 'copy-icon-ordercode-qr'
        });

        // Attach event listener
        const copyIcon = iconContainer.querySelector('.copy-icon-ordercode-qr');
        copyIcon.addEventListener('click', function() {
            const orderCode = orderCodeDisplay.textContent.trim();
            CopyWithVisualFeedback(orderCode, this, {
                type: 'icon',
                successColor: '#28a745'
            });
        });
    }
}

// Initialize khi page load
window.addEventListener('DOMContentLoaded', function () {
    initAmountFormatting();
    initCopyAccountNumber();
});
