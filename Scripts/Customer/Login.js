async function Login_Login() {
    let userName = document.getElementById("userName").value;
    if (isEmptyOrSpaces(userName)) {
        await CreateMustClickOkModal("Nhập tài khoản.", null);
        userName.focus();
        return;
    }

    let passWord = document.getElementById("passWord").value;
    if (isEmptyOrSpaces(passWord)) {
        await CreateMustClickOkModal("Nhập mật khẩu.", null);
        passWord.focus();
        return;
    }

    // Lấy guest cart từ localStorage (nếu có)
    let guestCart = null;
    if (typeof CartManager !== 'undefined') {
        guestCart = CartManager.getCart();
    }

    const searchParams = new URLSearchParams();
    searchParams.append("userName", userName);
    searchParams.append("passWord", passWord);

    // Gửi cart data để merge vào DB
    if (guestCart && guestCart.length > 0) {
        searchParams.append("guestCartJson", JSON.stringify(guestCart));
    }

    let query = "/Customer/Login_Login";

    ShowCircleLoader();
    let res = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    let resObj = JSON.parse(res.responseText);

    if (resObj.State != 0) {
        await CreateMustClickOkModal(resObj.Message, null);
        return;
    }

    // Xóa guest cart sau khi login thành công (đã merge vào DB)
    if (typeof CartManager !== 'undefined') {
        CartManager.clearCart();
    }

    window.location.href = "/Home";
}

function GoToCreateCustormerPage() {
    window.location.href = "/Customer/CreateCustomer";
}
