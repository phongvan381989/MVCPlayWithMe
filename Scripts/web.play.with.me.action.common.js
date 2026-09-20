// Khách vãng lai hay đăng nhập để hiển thị hành động tương ứng trên
// menu Tài Khoản góc trên phải màn hình
// Hàm này phải gọi mỗi khi load page
function ShowAccountAction() {
    let ele = document.getElementsByClassName("dropdown-content")[0];
    if (ele == null) { // Đăng nhập với vai trò admin
        return;
    }

    // ✅ Remove listener cũ trước khi add listener mới (tránh duplicate)
    if (window.dropdownClickOutsideListener) {
        document.removeEventListener("click", window.dropdownClickOutsideListener);
    }

    // ✅ Click outside to close: Đóng dropdown khi click/tap bên ngoài
    window.dropdownClickOutsideListener = function (event) {
        const accountContainer = document.querySelector('.top-account-container');

        // Nếu click KHÔNG vào account container (bao gồm cả dropdown)
        if (accountContainer && !accountContainer.contains(event.target)) {
            // Và dropdown đang hiển thị
            if (ele.offsetParent !== null) {
                ele.style.display = "none";
            }
        }
    };

    document.addEventListener("click", window.dropdownClickOutsideListener);

    ele.innerHTML = "";

    if (CheckAnonymousCustomer()) {
        // Đăng nhập
        CreateChildOfAccountElementV2(ele, "/Customer/Login", "Đăng Nhập")

        // Đăng ký
        CreateChildOfAccountElementV2(ele, "/Customer/CreateCustomer", "Đăng Ký")

        // Đơn hàng của tôi
        CreateChildOfAccountElementV2(ele, "/Customer/Order", "Đơn Hàng Vãng Lai")
    }
    else {
        // Thông tin tài khoản
        CreateChildOfAccountElementV2(ele, "/Customer/AccountInfor", "Tài Khoản Của Tôi")

        // Đơn hàng của tôi
        CreateChildOfAccountElementV2(ele, "/Customer/Order", "Đơn Mua")

        // Đăng xuất
        CreateChildOfAccountElement(ele, function () { Logout(); }, "Đăng Xuất")
    }
}

// Cập nhật số sản phẩm trong giỏ hàng (menu top), ẩn hiện icon giỏ hàng nếu cần thiết
async function UpdateCartCount() {
    if (DEBUG) {
        console.log("UpdateCartCount CALL ");
    }
    let length = 0;
    if (CheckAnonymousCustomer()) {
        const cart = CartManager.getCart();
        length = cart.length;
    }
    else {
        const searchParams = new URLSearchParams();

        let query = "/Customer/GetCartCount";

        let responseDB = await RequestHttpPostPromise(searchParams, query);
        let result = JSON.parse(responseDB.responseText);

        // AUTHEN_FAIL
        if (result.State == 6) {
            AuthenFail();
            return;
        }
        length = result.myAnything;
    }

    document.getElementsByClassName("cart-count")[0].innerHTML = length;
}

// Với khách mọi page đều phải thực hiện sau khi load
async function CommonAction() {
    let biggest = document.getElementById("biggestContainer_top");
    if (biggest == null) {
        return;
    }
    else {
        biggest.style.display = "block";
    }
    // Chỉ hiện tìm kiếm trên Home page

    let href = window.location.href.toUpperCase();
    if (/*href.includes("/HOME/INDEX") ||*/
        href.includes("/HOME/SEARCH") ||
        href.endsWith("/HOME") ||
        href.endsWith("/HOME/") ||
        href.endsWith("COM") ||
        href.endsWith("COM/") ||
        href.endsWith("56479") ||
        href.endsWith("56479/")) {
        document.getElementById("left_container").style.display = "flex";
    }
    else {
        document.getElementById("left_container").style.display = "none";
    }

    ShowAccountAction();
    if (href.includes("/HOME/CHECKOUT") ||
        href.includes("/HOME/CART")) {
        // remove cart icon
        document.getElementsByClassName("cart-container")[0].remove();
    }
    else {
        //cart-container
        await UpdateCartCount();
    }
}

// ✅ Guard flag: Chỉ chạy CommonAction() 1 lần duy nhất
// Tránh duplicate execution khi back/forward (bfcache) hoặc script load nhiều lần
if (typeof window.commonActionExecuted === 'undefined') {
    window.commonActionExecuted = true;
    CommonAction();
}
