async function Login_Login() {

    const searchParams = new URLSearchParams();
    let userName = document.getElementById("userName").value;

    searchParams.append("userName", userName);

    let passWord = document.getElementById("passWord").value;

    searchParams.append("passWord", passWord);

    let query = "/Administrator/Login_Login";

    let res = await RequestHttpPostPromise(searchParams, query);
    let resObj = JSON.parse(res.responseText);

    if (resObj.State != 0) {
        await CreateMustClickOkModal(resObj.Message, null);
        return;
    }

    //// Quay về trang trước đó
    //window.history.back();
    //// Refresh lại page với thông tin đã đăng nhập
    //window.location.reload();
    window.location.href = "/Administrator/Index";
}
