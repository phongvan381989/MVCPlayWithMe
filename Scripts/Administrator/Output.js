// Mapping ECommerce ID → Label + Badge Class
const ecommerceMap = {
    0: { name: 'PLAY WITH ME', badge: 'badge-pwm' },
    1: { name: 'TIKI', badge: 'badge-tiki' },
    2: { name: 'SHOPEE', badge: 'badge-shopee' },
    3: { name: 'LAZADA', badge: 'badge-lazada' }
};

async function SearchOutput() {
    const code = document.getElementById('input-code').value.trim();
    const ecommerce = parseInt(document.getElementById('select-ecommerce').value);

    if (!code) {
        alert('Vui lòng nhập mã đơn hàng!');
        return;
    }

    // Show loading
    document.getElementById('loading').style.display = 'block';
    document.getElementById('results-section').style.display = 'none';
    document.getElementById('no-results').style.display = 'none';

    try {
        // Gọi API bằng PostJSON từ web.play.with.me.common.js
        const responseText = await PostJSON('/Administrator/SearchOutput', {
            code: code,
            ecommerce: ecommerce
        });

        document.getElementById('loading').style.display = 'none';

        const result = JSON.parse(responseText);

        if (result.State === 0) { // OK
            const outputs = result.myJson || [];

            if (outputs.length > 0) {
                RenderResults(outputs);
            } else {
                document.getElementById('no-results').style.display = 'block';
            }
        } else {
            alert('Lỗi: ' + result.Message);
        }
    } catch (error) {
        document.getElementById('loading').style.display = 'none';
        alert('Lỗi kết nối đến server!');
    }
}

function RenderResults(outputs) {
    let html = '';

    for (let i = 0; i < outputs.length; i++) {
        const item = outputs[i];
        const ecom = ecommerceMap[item.eCommmerce] || { name: 'Unknown', badge: '' };
        const quantityClass = item.quantity >= 0 ? 'quantity-positive' : 'quantity-negative';
        const timeStr = new Date(item.time).toLocaleString('vi-VN');
        const thumbnailUrl = GetKhoThumbnailUrl_JPGFormat(item.productId);

        html += '<tr>';
        html += '<td>' + item.id + '</td>';
        html += '<td><strong>' + item.code + '</strong></td>';
        html += '<td><span class="badge ' + ecom.badge + '">' + ecom.name + '</span></td>';
        html += '<td><a href="' + GetProductUrl(item.productId) + '" target="_blank">';
        html += '<img src="' + thumbnailUrl + '"';
        html += ' alt=""';
        html += ' onerror="this.src=\'' + GetKhoThumbnailUrl_PNGFormat(item.productId) + '\'; this.onerror=null;"';
        html += ' style="width: 50px; height: 50px; object-fit: cover; border-radius: 4px; border: 1px solid #ddd; cursor: pointer;" />';
        html += '</a></td>';
        html += '<td class="' + quantityClass + '">' + (item.quantity > 0 ? '+' : '') + item.quantity + '</td>';
        html += '<td>' + (item.bookingCode || '-') + '</td>';
        html += '<td>' + timeStr + '</td>';
        html += '</tr>';
    }

    document.getElementById('results-tbody').innerHTML = html;
    document.getElementById('results-count').textContent = outputs.length;
    document.getElementById('results-section').style.display = 'block';
}

function ResetSearch() {
    document.getElementById('input-code').value = '';
    document.getElementById('select-ecommerce').value = '0';
    document.getElementById('results-section').style.display = 'none';
    document.getElementById('no-results').style.display = 'none';
}

async function UpdateInventoryForOrder() {
    const code = document.getElementById('input-code').value.trim();
    const ecommerce = parseInt(document.getElementById('select-ecommerce').value);

    if (!code) {
        alert('Vui lòng nhập mã đơn hàng!');
        return;
    }

    if (!confirm(`Xác nhận cập nhật tồn kho cho đơn hàng "${code}" trên sàn ${ecommerceMap[ecommerce].name}?`)) {
        return;
    }

    try {
        // Gọi API bằng PostJSON
        const responseText = await PostJSON('/Product/UpdateInventoryForOrder', {
            code: code,
            ecommerce: ecommerce
        });

        const result = JSON.parse(responseText);

        if (result.State === 0) {
            alert('✅ ' + result.Message);
        } else {
            alert('❌ ' + result.Message);
        }
    } catch (error) {
        alert('❌ Lỗi kết nối đến server!');
        console.error(error);
    }
}

// Focus vào input khi load trang
window.addEventListener('DOMContentLoaded', function() {
    document.getElementById('input-code').focus();
});
