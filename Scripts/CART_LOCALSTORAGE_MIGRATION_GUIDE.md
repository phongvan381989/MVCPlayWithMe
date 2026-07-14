# 📦 Cart Migration: Cookie → localStorage

## ✅ Đã hoàn thành:

### 1. Backend (HomeController.cs)
- ✅ Updated `CartPageLoadCart()` để nhận cart từ POST body
- ✅ Merge guest cart vào DB khi user login

### 2. JavaScript
- ✅ Tạo `cart-manager.js` - localStorage cart manager
- ✅ Auto-migration từ cookie sang localStorage

---

## 🔧 Cách tích hợp vào Cart.js hiện tại:

### **Step 1: Include cart-manager.js trong layout**

Thêm vào `Views/Shared/_Layout.cshtml` (hoặc layout bạn đang dùng):

```html
<!-- Đặt TRƯỚC các script khác -->
<script src="~/Scripts/cart-manager.js"></script>
```

### **Step 2: Update LoadCart() function trong Cart.js**

**TRƯỚC (dùng cookie):**
```javascript
function LoadCart() {
    $.ajax({
        url: '/Home/CartPageLoadCart',
        type: 'POST',
        success: function (data) {
            listCartCookieObject = JSON.parse(data);
            CreateSelectedModel();
        }
    });
}
```

**SAU (dùng localStorage):**
```javascript
async function LoadCart() {
    // Lấy cart từ localStorage
    const guestCart = CartManager.getCart();

    // Gửi lên server
    const response = await fetch('/Home/CartPageLoadCart', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(guestCart)
    });

    const data = await response.json();
    listCartCookieObject = data;
    CreateSelectedModel();
}
```

### **Step 3: Update Add to Cart actions**

**Trong SanPham.js hoặc nơi user click "Thêm vào giỏ":**

**TRƯỚC (cookie):**
```javascript
// Code cũ set cookie...
document.cookie = "cart=" + cookieValue;
```

**SAU (localStorage):**
```javascript
// Sử dụng CartManager
CartManager.addToCart(modelId, quantity, real);

// Nếu logged-in, cũng POST lên server
if (isLoggedIn) {
    await fetch('/Home/Item_AddModelToCart', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ id: modelId, q: quantity, real: real })
    });
}
```

---

## 📝 API Reference: CartManager

### Methods:

```javascript
// Init (auto-run on page load)
CartManager.init();

// Get cart
const cart = CartManager.getCart(); // Returns: [{ id, q, real }, ...]

// Add to cart
CartManager.addToCart(modelId, quantity, real = 1);

// Remove from cart
CartManager.removeFromCart(modelId);

// Update quantity
CartManager.updateQuantity(modelId, newQuantity);

// Clear cart
CartManager.clearCart();

// Get count
const count = CartManager.getItemCount();

// Update badge (header icon)
CartManager.updateCartBadge();
```

---

## 🧪 Testing:

### 1. Test migration:
```javascript
// Console (trước khi migrate)
document.cookie = 'cart=id=123#q=2#real=1$id=456#q=1#real=1';

// Refresh page
// Check console: "✅ Migrated 2 items from cookie to localStorage"

// Verify
CartManager.getCart();
// => [{ id: 123, q: 2, real: 1 }, { id: 456, q: 1, real: 1 }]
```

### 2. Test add to cart:
```javascript
CartManager.addToCart(789, 3);
// Check localStorage in DevTools > Application > Local Storage
```

### 3. Test logged-in merge:
```javascript
// 1. Add items as guest
CartManager.addToCart(111, 1);

// 2. Login

// 3. Go to cart page
// Check console: "Merged X items from localStorage to DB for customer Y"
```

---

## ⚠️ Breaking Changes:

### Cookie KHÔNG còn được dùng nữa!

**Tất cả code đọc/ghi cart cookie phải thay bằng CartManager:**

| Cũ (Cookie) | Mới (localStorage) |
|---|---|
| `Cookie.GetListCartCookie()` | `CartManager.getCart()` |
| `document.cookie = "cart=..."` | `CartManager.addToCart()` |
| Parse cookie string thủ công | Dùng CartManager methods |

---

## 🚀 Deployment Checklist:

- [ ] Backup database
- [ ] Test trên dev environment
- [ ] Clear browser cache để force migration
- [ ] Monitor logs: Check "Migrated X items" messages
- [ ] Test guest checkout
- [ ] Test logged-in checkout
- [ ] Test cart merge after login
- [ ] Monitor for "QuotaExceededError" (unlikely với cart)

---

## 📊 Benefits:

| | Cookie | localStorage |
|---|---|---|
| Request overhead | 1-4KB mỗi request | 0 KB |
| Bandwidth/day | ~10MB (1000 requests) | ~10KB (5 cart requests) |
| Max items | ~20-30 | ~500+ |
| Mobile 3G | Chậm | Nhanh hơn đáng kể |

---

## 🐛 Troubleshooting:

### Migration không chạy?
```javascript
// Force clear localStorage và retry
localStorage.removeItem('cart');
localStorage.removeItem('cart_metadata');
location.reload();
```

### Cart badge không update?
```javascript
// Đảm bảo HTML có element:
<span id="cart-badge"></span>

// Manually trigger:
CartManager.updateCartBadge();
```

### QuotaExceededError?
```javascript
// Giới hạn 100 items đã được implement
// Nếu vẫn lỗi, check localStorage size:
navigator.storage.estimate().then(console.log);
```

---

## 📞 Support:

Nếu gặp vấn đề, check:
1. Console errors (F12)
2. Network tab (check POST /Home/CartPageLoadCart)
3. Application tab > Local Storage > cart
4. Log file: `C:\Users\phong\TUNM\Works\WebPlayWithMe\log.txt`
