function AdminLogout() {
    const xhttp = new XMLHttpRequest();
    xhttp.onload = function () {
        if (this.readyState == 4 && this.status == 200) {
            window.location.href = "/Administrator/Login";
        }
    }
    const searchParams = new URLSearchParams();
    let query = "/Administrator/Logout";
    xhttp.open("POST", query);
    xhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    xhttp.send(searchParams.toString());
}

function ShowChangePasswordModal() {
    let ele = document.getElementById("modal-change-password");
    ele.style.display = "block";
    document.getElementById("old_password").value = "";
    document.getElementById("new_passWord").value = "";
    document.getElementById("re_new_passWord").value = "";

    let collection = document.getElementsByClassName("open_eye");
    for (let i = 0; i < collection.length; i++) {
        ShowCloseEye(collection[i]);
    }
}

function ReturnFromChangePasswordModal() {
    let ele = document.getElementById("modal-change-password");
    ele.style.display = "none";
}

async function FinishChangePassword() {
    if (document.getElementById("old_password").value.length == 0) {
        await CreateMustClickOkModal("Chưa nhập mật khẩu cũ.", null);
        document.getElementById("old_password").focus();
        return;
    }

    if (document.getElementById("new_passWord").value.length == 0) {
        await CreateMustClickOkModal("Chưa nhập mật khẩu mới.", null);
        document.getElementById("new_passWord").focus();
        return;
    }

    if (document.getElementById("re_new_passWord").value.length == 0) {
        await CreateMustClickOkModal("Chưa nhập lại mật khẩu mới.", null);
        document.getElementById("re_new_passWord").focus();
        return;
    }

    if (document.getElementById("new_passWord").value !== document.getElementById("re_new_passWord").value) {
        await CreateMustClickOkModal("Nhập lại mật khẩu chưa chính xác.", null);
        document.getElementById("re_new_passWord").focus();
        return;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("oldPassWord", document.getElementById("old_password").value);
    searchParams.append("newPassWord", document.getElementById("new_passWord").value);
    searchParams.append("renewPassWord", document.getElementById("re_new_passWord").value);

    let url = "/Administrator/ChangePassword";

    try {
        // Cập nhật vào db
        ShowCircleLoader();
        let responseDB = await RequestHttpPostPromise(searchParams, url);
        RemoveCircleLoader();
        let isOk = CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật thành công.", "Cập nhật thất bại.");
        if (isOk) {
            ReturnFromChangePasswordModal();
        }
    }
    catch (error) {
        CreateMustClickOkModal("Cập nhật lỗi.", null);
        return;
    }
}
