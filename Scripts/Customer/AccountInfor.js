let customerObj = null; // Đối tượng khách hàng server trả về
let listCustomerAddressObj;
let currentIndexAddressUpdateObject = -1; // index cần cập nhật
LoadSomething();

async function GetCustomer() {
    const searchParams = new URLSearchParams();
    let query = "/Customer/GetCustomer";

    return await RequestHttpPostPromise(searchParams, query);
}

// Sau khi load page, cần thêm thông tin
async function LoadSomething() {

    await LoadCustomer();
}

// Hiển thị thông tin tài khoản
function ShowAccount() {
    let container = document.getElementsByClassName("account-profile-container")[0];

    container.getElementsByClassName("uxYEXm")[0].innerHTML = customerObj.userName;
    container.getElementsByClassName("full-name")[0].value = customerObj.fullName;
    container.getElementsByClassName("mail")[0].value = customerObj.email;
    container.getElementsByClassName("phone")[0].value = customerObj.sdt;
    // 1: Nam, 2: Nu, 3: Khac
    if (customerObj.sex == 1) {

        container.getElementsByClassName("radio-sex")[0].checked = true;
    }
    else if (customerObj.sex == 2) {

        container.getElementsByClassName("radio-sex")[1].checked = true;
    }
    else if (customerObj.sex == 3){
        container.getElementsByClassName("radio-sex")[2].checked = true;
    }
    // customerObj.birthday: 2020-01-01T00:00:00
    container.getElementsByClassName("birthday-time")[0].value = customerObj.birthday.split('T')[0];
}

// Hiển thị danh sách địa chỉ
function ShowAddress() {
    // Hiển thị danh sách địa chỉ
    // Lấy mẫu
    let sample = document.getElementsByClassName("sample-address-container")[0];
    let container = document.getElementsByClassName("list-address-container")[0];
    // Xóa nội dung cũ
    container.innerHTML = "";

    listCustomerAddressObj = customerObj.lsAddress;
    let length = listCustomerAddressObj.length;
    for (let i = 0; i < length; i++) {
        let obj = listCustomerAddressObj[i];
        let clone = sample.cloneNode(true);
        clone.style.display = "flex";
        clone.setAttribute("data-index", i.toString());

        clone.getElementsByClassName("name")[0].innerHTML = obj.name;
        clone.getElementsByClassName("phone")[0].innerHTML = obj.phone;

        clone.getElementsByClassName("detail")[0].innerHTML = obj.detail;
        clone.getElementsByClassName("province-district-subdistrict")[0].innerHTML =
            obj.subdistrict + ", " + obj.district + ", " + obj.province;

        if (!obj.defaultAdd) {
            clone.getElementsByClassName("default-address")[0].style.display = "none";

            // enable button thiết lập mặc định
            clone.getElementsByClassName("mgW0lg")[0].disabled = false;

        }
        else {
            // Ẩn nút xóa địa chỉ
            clone.getElementsByClassName("htlhtxbv")[0].children[1].style.display = "none";
        }
        container.appendChild(clone);
    }
}

function UpdateCustomerAddress(ele) {
    currentIndexAddressUpdateObject = parseInt(ele.parentElement.parentElement.parentElement.getAttribute("data-index"));
    ShowCustomerInforModal(false, false, listCustomerAddressObj[currentIndexAddressUpdateObject]);
}

async function DeleteCustomerAddress(ele) {
    let index = parseInt(ele.parentElement.parentElement.parentElement.getAttribute("data-index"));
    let obj = listCustomerAddressObj[index];

    let children = document.getElementsByClassName("list-address-container")[0].children;
    children[index].remove();

    listCustomerAddressObj.splice(index, 1);

    // Cập nhật data-index trên page
    children = document.getElementsByClassName("list-address-container")[0].children;
    let length = children.length;
    for (let i = 0; i < length; i++) {
        children[i].setAttribute("data-index", i.toString());
    }

    // Cập nhật db
    let res = await DeleteAddress(obj);
    let resObj = JSON.parse(res.responseText);
    if (resObj.State != 0) {
        await CreateMustClickOkModal("Có lỗi xảy ra. Vui lòng thử lại sau.", null);
        return;
    }
}

async function SetDefault(ele) {
    let index = parseInt(ele.parentElement.parentElement.getAttribute("data-index"));

    let children = document.getElementsByClassName("list-address-container")[0].children;
    let length = listCustomerAddressObj.length;

    for (let i = 0; i < length; i++) {
        if (i != index) {
            children[i].getElementsByClassName("default-address")[0].style.display = "none";
            // enable button thiết lập mặc định
            children[i].getElementsByClassName("mgW0lg")[0].disabled = false;
            // Hiện nút xóa địa chỉ
            children[i].getElementsByClassName("htlhtxbv")[0].children[1].style.display = "initial";

            listCustomerAddressObj[i].defaultAdd = 0;
        }
        else {
            children[i].getElementsByClassName("default-address")[0].style.display = "initial";
            // disable button thiết lập mặc định
            children[i].getElementsByClassName("mgW0lg")[0].disabled = true;
            // Ẩn nút xóa địa chỉ
            children[i].getElementsByClassName("htlhtxbv")[0].children[1].style.display = "none";

            listCustomerAddressObj[i].defaultAdd = 1;
        }
    }

    // Cập nhật db
    let obj = listCustomerAddressObj[index];
    let res = await UpdateAddress(obj);
    let resObj = JSON.parse(res.responseText);
    if (resObj.State != 0) {
        await CreateMustClickOkModal("Có lỗi xảy ra. Vui lòng thử lại sau.", null);
        return;
    }
}

function ReturnCustomerInforModal() {
    RefreshModalCustomerInfor();
    currentIndexAddressUpdateObject = -1;
}

async function FinishCustomerInforModal() {
    if (!ValidCustomerInforInput()) {
        return;
    }

    let obj = CreateAddressObjFromInput();
    if (currentIndexAddressUpdateObject != -1) {// Cập nhật thông tin vào object
        obj.id = listCustomerAddressObj[currentIndexAddressUpdateObject].id;
        listCustomerAddressObj[currentIndexAddressUpdateObject] = obj;
        //Kiểm tra có đặt mặc định
        if (obj.defaultAdd) {
            // Bỏ mặc định cũ nếu có
            for (let i = listCustomerAddressObj.length - 1; i >= 0; i--) {
                if (listCustomerAddressObj[i].defaultAdd
                    && i != currentIndexAddressUpdateObject) {
                    listCustomerAddressObj[i].defaultAdd = 0;
                }
            }
        }
    }
    else {
        // Thêm mới
        //Kiểm tra có đặt mặc đinh
        if (obj.defaultAdd) {
            // Bỏ mặc định cũ nếu có
            for (let i = listCustomerAddressObj.length - 1; i >= 0; i--) {
                if (listCustomerAddressObj[i].defaultAdd) {
                    listCustomerAddressObj[i].defaultAdd = 0;
                }
            }
        }

    }

    if (currentIndexAddressUpdateObject != -1) {// Cập nhật thông tin
        let res = await UpdateAddress(obj);
        let resObj = JSON.parse(res.responseText);
        if (resObj.State != 0) {
            await CreateMustClickOkModal("Có lỗi xảy ra. Vui lòng thử lại sau.", null);
            return;
        }
    }
    else {
        let res = await InsertAddress(obj);
        let resObj = JSON.parse(res.responseText);
        if (resObj.State != 0) {
            await CreateMustClickOkModal("Có lỗi xảy ra. Vui lòng thử lại sau.", null);
            return;
        }

        obj.id = resObj.myAnything;
        listCustomerAddressObj.push(obj);
    }

    currentIndexAddressUpdateObject = -1;

    RefreshModalCustomerInfor();

    // Hiển thị danh sách địa chỉ, ta cập nhật lại
    ShowAddress();
}

async function LoadCustomer() {
    let res = await GetCustomer();
    customerObj = JSON.parse(res.responseText);
    if (JSON.parse(res.responseText) == null) {
        await CreateMustClickOkModal("Không lấy được dữ liệu. Vui lòng thử lại sau.", null);
        // Trả về định dạng giống truy vấn httpPost
        return GetEasyPromise();
    }

    ShowAccount();
    ShowAddress();

    // Trả về định dạng giống truy vấn httpPost
    return GetEasyPromise();
}

async function SaveProfile() {
    let container = document.getElementsByClassName("account-profile-container")[0];

    const searchParams = new URLSearchParams();
    searchParams.append("email", container.getElementsByClassName("mail")[0].value);
    searchParams.append("sdt", container.getElementsByClassName("phone")[0].value);
    searchParams.append("fullName", container.getElementsByClassName("full-name")[0].value);

    let date = new Date(container.getElementsByClassName("birthday-time")[0].value);
    searchParams.append("day", date.getDate());
    searchParams.append("month", date.getMonth() + 1);
    searchParams.append("year", date.getFullYear());

    let sexEle = document.querySelector('input[name="sex"]:checked');
    let sex = 4;
    if (sexEle != null) {
        sex = sexEle.value;
    }
    searchParams.append("sex", sex);

    let url = "/Customer/UpdateInfor";

    try {
        // Cập nhật vào db
        ShowCircleLoader();
        let responseDB = await RequestHttpPostPromise(searchParams, url);
        RemoveCircleLoader();
        CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật thành công.", "Cập nhật thất bại.");
    }
    catch (error) {
        CreateMustClickOkModal("Cập nhật lỗi.", null);
        return;
    }
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

    let url = "/Customer/ChangePassword";

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
