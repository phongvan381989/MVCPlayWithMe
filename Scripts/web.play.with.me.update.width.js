// ============================================
// CSS Grid Responsive Layout
// Browser tự tính số columns, JavaScript chỉ ĐỌC layout
// ============================================

// Số items hiển thị trên 1 hàng (đọc từ CSS Grid layout)
let itemOnRow = 6; // Default fallback

function SetCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

// ĐỌC số items/row từ CSS Grid layout thực tế
function calculateItemsOnRow() {
    const container = document.getElementById("biggestContainer_body_wraper_item");
    if (!container) return;

    const items = container.querySelectorAll(".product-for-selector-container");
    if (items.length < 2) return;

    // Tính số items/row bằng cách đếm items cùng offsetTop
    const firstItemTop = items[0].offsetTop;
    let count = 1;
    for (let i = 1; i < items.length; i++) {
        if (items[i].offsetTop === firstItemTop) {
            count++;
        } else {
            break; // Đã xuống row thứ 2
        }
    }

    itemOnRow = count;

    // Lưu cookie cho pagination (nếu backend cần)
    SetCookie("itemOnRow", itemOnRow, 1000);
}

// Toggle display mode cho biggestContainer_top_first (mobile ≤600px)
function updateTopContainerDisplay() {
    const top_first = document.getElementById("biggestContainer_top_first");
    if (top_first) {
        const isMobile = window.innerWidth <= 600;
        top_first.style.display = isMobile ? "block" : "flex";
    }
}

// Update layout khi resize (debounced)
let resizeTimeout;
function handleResize() {
    clearTimeout(resizeTimeout);
    resizeTimeout = setTimeout(() => {
        calculateItemsOnRow();
        updateTopContainerDisplay();
    }, 150);
}

// Init
window.addEventListener("DOMContentLoaded", () => {
    calculateItemsOnRow();
    updateTopContainerDisplay();
});

window.addEventListener("resize", handleResize);
