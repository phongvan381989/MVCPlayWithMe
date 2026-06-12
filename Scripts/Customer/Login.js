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
    const searchParams = new URLSearchParams();
    searchParams.append("userName", userName);
    searchParams.append("passWord", passWord);

    let query = "/Customer/Login_Login";

    ShowCircleLoader();
    let res = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    let resObj = JSON.parse(res.responseText);

    if (resObj.State != 0) {
        await CreateMustClickOkModal(resObj.Message, null);

        return;
    }
    //// Xóa cart cookie bên javascript
    //DeleteAllCartCookie();
    //// Xóa customer info cookie bên javascript
    //DeleteAllCustomerInforCookie();

    //if (window.history.length > 2) {
    //    // Quay về trang trước đó
    //    alert("pasueeee");
    //    window.history.back();
    //    // Refresh lại page với thông tin đã đăng nhập
    //    window.location.reload();
    //}
    //else {
    //    window.location.href = "/Home";
    //}

    window.location.href = "/Home";
}

function GoToCreateCustormerPage() {
    window.location.href = "/Customer/CreateCustomer";
}
