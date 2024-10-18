// Khách vãng lai hay đăng nhập để hiển thị hành động tương ứng trên
// menu Tài Khoản góc trên phải màn hình
// Hàm này phải gọi mỗi khi load page
function ShowAccoutAction() {
    let ele = document.getElementsByClassName("dropdown-content")[0];
    if (ele == null) { // Đăng nhập với vai trò admin
        return;
    }

    document.getElementsByTagName("BODY")[0].addEventListener("touchmove", function (event) {
        if (window.getComputedStyle(ele, null).display == "block") {
            ele.style.display = "none";
        }
    });

    ele.innerHTML = "";

    if (CheckAnonymousCustomer()) {
        // Đăng nhập
        CreateChildOfAccountElementV2(ele, "/Customer/Login", "Đăng nhập")

        // Đăng ký
        CreateChildOfAccountElementV2(ele, "/Customer/CreateCustomer", "Đăng ký")

        // Đơn hàng của tôi
        CreateChildOfAccountElementV2(ele, "/Customer/Order", "Đơn hàng vãng lai")
    }
    else {
        // Thông tin tài khoản
        CreateChildOfAccountElementV2(ele, "/Customer/AccountInfor", "Thông tin tài khoản")

        // Đơn hàng của tôi
        CreateChildOfAccountElementV2(ele, "/Customer/Order", "Đơn hàng của tôi")

        // Đăng xuất
        CreateChildOfAccountElement(ele, function () { Logout(); }, "Đăng xuất")
    }
}

// Cập nhật số sản phẩm trong giỏ hàng (menu top), ẩn hiện icon giỏ hàng nếu cần thiết
async function UpdateCartCount() {
    if (DEBUG) {
        console.log("UpdateCartCount CALL ");
    }
    let length = 0;
    if (CheckAnonymousCustomer()) {
        // Lấy giỏ hàng từ cookie
        let cartCookie = GetCookie(cartKey);
        let myArray = GetListCartCookieFromCartCookie(cartCookie);
        length = myArray.length;
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
    if (DEBUG) {
        console.log("CommonAction CALL");
    }
    if (document.getElementById("biggestContainer_top") == null) {
        return;
    }
    // Chỉ hiện tìm kiếm trên Home page
    {
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
    }
    if (true/*CheckIsCustomer()*/) { // Temporary comment

        //window.history.pushState({}, "");

        //window.addEventListener("popstate", (e) => {
        //    if (DEBUG) {
        //        console.log("popstate UpdateCartCount CALL ");
        //    }
        //    UpdateCartCount();
        //});

        await ShowAccoutAction();
        await UpdateCartCount();
    }
    else {
        // Xóa thông tin account
        document.getElementsByClassName("top-account-container")[0].remove();

        // Xóa thông tin giỏ hàng
        document.getElementsByClassName("cart-container")[0].remove();
    }
}

CommonAction();