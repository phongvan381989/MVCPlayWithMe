async function CreateCustomer_Add() {

    let userName = document.getElementById("userName").value;
    let passWord = document.getElementById("passWord").value;
    let repassWord = document.getElementById("repassWord").value;

    if (isEmptyOrSpaces(userName)
        || isEmptyOrSpaces(passWord)
        || isEmptyOrSpaces(repassWord)) {
        await CreateMustClickOkModal("Vui lòng không để trống thông tin tài khoản, mật khẩu.", null);
        return;
    }
    // Kiểm tra password và repassword hợp lệ
    {
        const obj = JSON.parse(CheckPassWordValid(passWord, repassWord));
        if (obj.isValid === false) {
            await CreateMustClickOkModal(obj.message, null);
            return;
        }
    }

    const searchParams = new URLSearchParams();
    searchParams.append("userName", userName);
    searchParams.append("passWord", passWord);

    let url = "/Customer/CreateCustomer_Add";

    try {
        // Cập nhật vào db
        ShowCircleLoader();
        let responseDB = await RequestHttpPostPromise(searchParams, url);
        RemoveCircleLoader();
        let isOk = CheckStatusResponseAndShowPrompt(responseDB.responseText, "Tạo tài khoản thành công.", "Tạo tài khoản thất bại.");
        if (isOk) {
            window.location.replace("/Customer/Login");
        }
    }
    catch (error) {
        CreateMustClickOkModal("Tạo tài khoản thất bại.", null);
        return;
    }
}
