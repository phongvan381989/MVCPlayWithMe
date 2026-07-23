/**
 * Cart Manager - localStorage-based cart system
 * Thay thế cookie cart để giảm bandwidth overhead
 * Version: 1.0
 */

const CartManager = {
    STORAGE_KEY: 'cart',
    METADATA_KEY: 'cart_metadata',

    /**
     * Khởi tạo: Migrate cookie → localStorage (chạy 1 lần)
     */
    init() {
        this.migrateCookieToLocalStorage();
        this.cleanupOldCart();
    },

    /**
     * Migration: Đọc cookie cart cũ → chuyển sang localStorage
     */
    migrateCookieToLocalStorage() {
        // Đã có localStorage cart → skip
        if (localStorage.getItem(this.STORAGE_KEY)) {
            return;
        }

        // Đọc cookie cart cũ (format: id=123#q=10#real=1$id=321#q=1#real=0$...)
        const cookieCart = this.getCookie('cart');
        if (!cookieCart) {
            return; // Không có cart cũ
        }

        try {
            // Parse cookie format
            const now = Date.now();
            const items = cookieCart.split('$')
                .filter(item => item.trim())
                .map(item => {
                    const parts = item.split('#');
                    return {
                        sanPhamId: parseInt(parts[0].split('=')[1]),  // Sản phẩm ID
                        quantity: parseInt(parts[1].split('=')[1]),   // Số lượng
                        real: parseInt(parts[2].split('=')[1]),       // 1=mua ngay, 0=mua sau
                        time: now                                      // Timestamp migration
                    };
                });

            // Lưu vào localStorage
            if (items.length > 0) {
                this.saveCart(items);
                console.log('✅ Migrated', items.length, 'items from cookie to localStorage');

                // Xóa cookie cũ
                document.cookie = 'cart=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
            }
        } catch (e) {
            console.error('❌ Migration failed:', e);
        }
    },

    /**
     * Xóa cart cũ > 90 ngày (abandoned cart cleanup)
     */
    cleanupOldCart() {
        const metadata = this.getMetadata();
        if (!metadata.lastUpdated) {
            return;
        }

        const daysSinceUpdate = (Date.now() - metadata.lastUpdated) / (1000 * 60 * 60 * 24);
        if (daysSinceUpdate > 90) {
            localStorage.removeItem(this.STORAGE_KEY);
            localStorage.removeItem(this.METADATA_KEY);
            console.log('🗑️ Cleaned up abandoned cart (90+ days old)');
        }
    },

    /**
     * Lấy giỏ hàng từ localStorage
     * Sắp xếp theo thời gian (mới nhất lên đầu)
     * @returns {Array} Danh sách cart items
     */
    getCart() {
        try {
            const cartJson = localStorage.getItem(this.STORAGE_KEY);
            const cart = cartJson ? JSON.parse(cartJson) : [];
            return cart;
            // Sắp xếp theo time DESC (mới nhất lên đầu)
            return cart.sort((a, b) => (b.time || 0) - (a.time || 0));
        } catch (e) {
            console.error('❌ Error reading cart:', e);
            return [];
        }
    },

    /**
     * Lưu giỏ hàng vào localStorage
     * @param {Array} cart - Danh sách cart items
     */
    saveCart(cart) {
        try {
            localStorage.setItem(this.STORAGE_KEY, JSON.stringify(cart));

            // Cập nhật metadata
            localStorage.setItem(this.METADATA_KEY, JSON.stringify({
                lastUpdated: Date.now(),
                version: '1.0'
            }));
        } catch (e) {
            if (e.name === 'QuotaExceededError') {
                alert('Giỏ hàng đầy! Vui lòng xóa bớt sản phẩm hoặc checkout.');
                console.error('❌ localStorage quota exceeded');
            } else {
                console.error('❌ Error saving cart:', e);
            }
        }
    },

    /**
     * Lấy metadata (timestamp, version...)
     * @returns {Object}
     */
    getMetadata() {
        try {
            const metadata = localStorage.getItem(this.METADATA_KEY);
            return metadata ? JSON.parse(metadata) : {};
        } catch (e) {
            return {};
        }
    },

    /**
     * Thêm sản phẩm vào giỏ hàng, tăng số lượng nếu tồn tại
     * @param {Number} sanPhamId - Sản Phẩm ID
     * @param {Number} quantity - Số lượng
     * @param {Number} real - 1=mua ngay, 0=mua sau (default: 1)
     * @returns {Boolean} Success
     */
    addToCart(sanPhamId, quantity, real) {
        const cart = this.getCart();

        // Giới hạn 1000 items
        if (cart.length >= 500) {
            alert('Giỏ hàng tối đa 500 sản phẩm!');
            return false;
        }

        // // Làm mới dữ liệu trước đó real = 0
        // cart.forEach(item => { item.real = 0; });

        // Tìm item đã tồn tại
        const existingItem = cart.find(item => item.sanPhamId === sanPhamId);

        if (existingItem) {
            // Tăng số lượng và cập nhật time
            existingItem.quantity += quantity;
            existingItem.real = real;
        } else {
            // Thêm mới với timestamp
            cart.push({
                sanPhamId: sanPhamId,
                quantity: quantity,
                real: real,
                time: Date.now()
            });
        }

        this.saveCart(cart);
        this.updateCartBadge();
        console.log('✅ Added to cart:', { sanPhamId, quantity, real, time: Date.now() });
        return true;
    },

    // Xóa cart nếu có và tạo mới với real = 1, phục vụ cho khách đã đăng nhập để biết chọn mua sản phẩm nào
    clearAndCreateCartFromList(listCartObject) {
        let cart = this.getCart();
        //xóa cũ
        cart = [];
        listCartObject.forEach(item => {
            cart.push({
                sanPhamId: item.sanPhamId,
                quantity: item.quantity,
                real: 1,
                time: Date.now()
            });
        });

        this.saveCart(cart);
        this.updateCartBadge();
    },

    /**
     * Xóa sản phẩm khỏi giỏ hàng
     * @param {Number} sanPhamId
     */
    removeFromCart(sanPhamId) {
        let cart = this.getCart();
        cart = cart.filter(item => item.sanPhamId !== sanPhamId);
        this.saveCart(cart);
        this.updateCartBadge();
        console.log('🗑️ Removed from cart:', sanPhamId);
    },

    // /**
    //  * Cập nhật số lượng sản phẩm
    //  * @param {Number} sanPhamId - ID
    //  * @param {Number} quantity - Số lượng mới
    //  */
    updateQuantity(sanPhamId, quantity) {
        const cart = this.getCart();
        const item = cart.find(item => item.sanPhamId === sanPhamId);

        if (item) {
            if (quantity <= 0) {
                this.removeFromCart(sanPhamId);
            } else {
                item.quantity = quantity;
            }
        }
        else {
            // Thêm mới với timestamp
            cart.push({
                sanPhamId: sanPhamId,
                quantity: quantity,
                real: 0,
                time: Date.now()
            });
        }
        this.saveCart(cart);
        this.updateCartBadge();
    },



    updateReal(sanPhamId, real) {
        const cart = this.getCart();
        const item = cart.find(item => item.sanPhamId === sanPhamId);

        if (item) {
            item.real = real;
        }
        else {
            // Thêm mới với timestamp
            cart.push({
                sanPhamId: sanPhamId,
                quantity: 0,
                real: real,
                time: Date.now()
            });
        }
        this.saveCart(cart);
        this.updateCartBadge();
    },

    setRealAll(real) {
        const cart = this.getCart();
        for (let c of cart) {
            c.real = real;
        }
        this.saveCart(cart);
        this.updateCartBadge();
    },

    /**
     * Xóa toàn bộ giỏ hàng
     */
    clearCart() {
        localStorage.removeItem(this.STORAGE_KEY);
        localStorage.removeItem(this.METADATA_KEY);
        this.updateCartBadge();
        console.log('🗑️ Cart cleared');
    },

    // Set tất cả real = 0 với khách vãng lai hoặc xóa cart với khách đăng nhập
    setRealZeroOrClear() {
        const cart = this.getCart();
        if (CheckAnonymousCustomer()) {
            for (let c of cart) {
                c.real = 0;
            }
            this.saveCart(cart);
        }
        else {
            localStorage.removeItem(this.STORAGE_KEY);
            localStorage.removeItem(this.METADATA_KEY);
        }
        this.updateCartBadge();
    }
    ,
    /**
     * Đếm tổng số items trong giỏ hàng
     * @returns {Number}
     */
    getItemCount() {
        const cart = this.getCart();
        return cart.reduce((sum, item) => sum + item.quantity, 0);
    },

    /**
     * Cập nhật badge số lượng giỏ hàng (header icon)
     */
    updateCartBadge() {
        // const count = this.getItemCount();
        // const badge = document.getElementById('cart-badge');

        // if (badge) {
        //     badge.textContent = count;
        //     badge.style.display = count > 0 ? 'inline-block' : 'none';
        // }
    },

    /**
     * Helper: Đọc cookie
     * @param {String} name - Cookie name
     * @returns {String|null}
     */
    getCookie(name) {
        const value = `; ${document.cookie}`;
        const parts = value.split(`; ${name}=`);
        if (parts.length === 2) {
            return parts.pop().split(';').shift();
        }
        return null;
    }
};

// // Auto-init khi page load
// document.addEventListener('DOMContentLoaded', () => {
//     CartManager.init();
//     CartManager.updateCartBadge();
// });
