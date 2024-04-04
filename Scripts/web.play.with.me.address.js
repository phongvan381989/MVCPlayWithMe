let administrativeAddressObject; // địa giới hành chính
//let addressObj; // Đối tượng địa chỉ hiển thị trên modal

// Chỉ gọi 1 lần, vì option không thay đổi
function AddProvince() {
    if (administrativeAddressObject == null)
        return;
    let ele = document.getElementById("province");
    let length = administrativeAddressObject.length;

    for (let i = 0; i < length; i++) {
        let option = document.createElement("option");
        option.value = administrativeAddressObject[i].province;
        option.text = administrativeAddressObject[i].province;
        ele.appendChild(option);
    }

}

async function GetAdministrativeAddress() {

    // Thông tin chung này lấy từ sản phẩm đầu tiên thuộc combo trong db
    const searchParams = new URLSearchParams();

    let query = "/Home/GetAdministrativeAddress";

    return await RequestHttpPostPromise(searchParams, query);
}

// Thay đổi tỉnh, add huyện tương ứng vào select tag
function AddDistrict() {
    let provinceEle = document.getElementById("province");
    if (provinceEle.selectedIndex == 0 || provinceEle.selectedIndex == -1) {
        return;
    }
    let districtEle = document.getElementById("district");

    // Trừ 1 vì option đầu là cấp độ: tỉnh, huyện, xã
    let Obj = administrativeAddressObject[provinceEle.selectedIndex - 1].districts;
    let length = Obj.length;
    for (let i = 0; i < length; i++) {
        let option = document.createElement("option");
        option.value = Obj[i];
        option.text = Obj[i];
        districtEle.appendChild(option);
    }
}

function DeleteDistrict() {
    let districtEle = document.getElementById("district");
    for (let i = districtEle.length - 1; i > 0; i--) {
        districtEle.remove(i);
    }
    districtEle.selectedIndex = 0;
}

// Thay đổi huyện, add xã tương ứng vào select tag
function AddSubDistrict() {
    let provinceEle = document.getElementById("province");
    if (provinceEle.selectedIndex == 0 || provinceEle.selectedIndex == -1) {
        return;
    }
    let districtEle = document.getElementById("district");
    if (districtEle.selectedIndex == 0 || districtEle.selectedIndex == -1) {
        return;
    }
    let subdistrictEle = document.getElementById("subdistrict");
    // Trừ 1 vì option đầu là cấp độ: tỉnh, huyện, xã
    let Obj = administrativeAddressObject[provinceEle.selectedIndex - 1].subdistricts[districtEle.selectedIndex - 1];

    let length = Obj.length;
    for (let i = 0; i < length; i++) {
        let option = document.createElement("option");
        option.value = Obj[i];
        option.text = Obj[i];
        subdistrictEle.appendChild(option);
    }
}

function DeleteSubDistrict() {
    let subdistrictEle = document.getElementById("subdistrict");
    for (let i = subdistrictEle.length - 1; i > 0; i--) {
        subdistrictEle.remove(i);
    }
    subdistrictEle.selectedIndex = 0;
}

function ChangeProvince() {
    DeleteDistrict();
    AddDistrict();

    DeleteSubDistrict();
}

function ChangeDistrict() {

    DeleteSubDistrict();
    AddSubDistrict();
}

function ChangeSubDistrict() {

}

function GetFocus(ele) {
    ele.style.border = "1px solid rgba(0,0,0,.14)";
}

// Ẩn modal và set giá trị input về ban đầu
function RefreshModalCustomerInfor() {
    // Ẩn modal
    document.getElementById("modal-customer-infor").style.display = "none";

    // Set giá trị input về ban đầu
    document.getElementById("customer-name").value = "";
    document.getElementById("phone-number").value = "";
    document.getElementById("province").selectedIndex = 0;

    document.getElementById("district").selectedIndex = 0;
    document.getElementById("subdistrict").selectedIndex = 0;
    document.getElementById("detail-subdistrict").value = "";
    document.getElementById("check-default").checked = false;

}

// isCreate: true là thêm mới thông tin địa chỉ
// false: Cập nhật thông tin cũ
// isModalUnder: true, có modal bên dưới, cần tăng zindex = 2 ngược lại không cần
async function ShowCustomerInforModal(isCreate, isModalUnder, addressObj) {

    if (administrativeAddressObject == null) {
        // Lấy dữ liệu tỉnh, huyện xã từ db
        let responseDB = await GetAdministrativeAddress();
        administrativeAddressObject = JSON.parse(responseDB.responseText);

        AddProvince();
    }
    let ele = document.getElementById("modal-customer-infor");
    if (isCreate) {
        ele.style.display = "block";
        if (isModalUnder) {
            ele.style.zIndex = 2;
        }
    }
    else {
        ele.style.display = "block";
        // mặc định có modal ở dưới
        ele.style.zIndex = 2;
        // Hiển thị thông tin địa chỉ
        //let obj = listCustomerInforCookieObject[currentIndexInforUpdateObject];
        document.getElementById("customer-name").value = addressObj.name;
        document.getElementById("phone-number").value = addressObj.phone;
        document.getElementById("province").value = addressObj.province;
        DeleteDistrict();
        AddDistrict();
        document.getElementById("district").value = addressObj.district;
        DeleteSubDistrict();
        AddSubDistrict();
        document.getElementById("subdistrict").value = addressObj.subdistrict;
        document.getElementById("detail-subdistrict").value = addressObj.detail;
        document.getElementById("check-default").checked = Boolean(addressObj.defaultAdd);
    }
}

// Kiểm tra thông tin tên, sdt, địa chỉ khách nhập đã chính xác
function ValidCustomerInforInput() {
    let isOk = true;
    // Check tên
    if (isEmptyOrSpaces(document.getElementById("customer-name").value)) {
        document.getElementById("customer-name").style.border = "1px solid red";
        isOk = false;
    }
    // Check số điện thoại di động
    let phoneNumber = document.getElementById("phone-number").value;
    if (!CheckValidSDT(phoneNumber)) {
        document.getElementById("phone-number").style.border = "1px solid red";
        isOk = false;
    }

    // Check tỉnh
    if (document.getElementById("province").selectedIndex < 1) {
        document.getElementById("province").style.border = "1px solid red";
        isOk = false;
    }

    // Check huyện
    if (document.getElementById("district").selectedIndex < 1) {
        document.getElementById("district").style.border = "1px solid red";
        isOk = false;
    }

    // Check xã
    if (document.getElementById("subdistrict").selectedIndex < 1) {
        document.getElementById("subdistrict").style.border = "1px solid red";
        isOk = false;
    }

    // Check chi tiết
    if (isEmptyOrSpaces(document.getElementById("detail-subdistrict").value)) {
        document.getElementById("detail-subdistrict").style.border = "1px solid red";
        isOk = false;
    }
    return isOk;
}

// Tạo cookie obj từ input
function CreateCookieValueFromInput() {
    let isDefault = 1;
    if (!document.getElementById("check-default").checked) {
        isDefault = 0;
    }
    let obj = new objCustomerInforCookieFromInput(
        document.getElementById("customer-name").value,
        document.getElementById("phone-number").value,
        document.getElementById("province").value,
        document.getElementById("district").value,
        document.getElementById("subdistrict").value,
        document.getElementById("detail-subdistrict").value,
        isDefault
    );

    return obj;
}

async function InsertAddress(obj) {
    const searchParams = new URLSearchParams();
    searchParams.append("address", JSON.stringify(obj));
    let query = "/Customer/InsertAddress";

    return await RequestHttpPostPromise(searchParams, query);
}

async function UpdateAddress(obj) {
    const searchParams = new URLSearchParams();
    searchParams.append("address", JSON.stringify(obj));
    let query = "/Customer/UpdateAddress";

    return await RequestHttpPostPromise(searchParams, query);
}

async function DeleteAddress(obj) {
    const searchParams = new URLSearchParams();
    searchParams.append("address", JSON.stringify(obj));
    let query = "/Customer/DeleteAddress";

    return await RequestHttpPostPromise(searchParams, query);
}
