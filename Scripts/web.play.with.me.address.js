let administrativeAddressObject; // địa giới hành chính
//let addressObj; // Đối tượng địa chỉ hiển thị trên modal

// Config cho localStorage cache
const ADDRESS_CACHE_CONFIG = {
    KEY: "vnAddressData",
    VERSION: "1.0"  // Tăng lên khi có thay đổi địa chỉ hành chính
};

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

// Lấy data từ localStorage (nếu version khớp)
function GetAddressFromCache() {
    try {
        const cached = localStorage.getItem(ADDRESS_CACHE_CONFIG.KEY);
        if (!cached) return null;

        const data = JSON.parse(cached);

        // Chỉ check version - không cần expiry vì data này rất ít thay đổi
        if (data.version === ADDRESS_CACHE_CONFIG.VERSION) {
            return data.addresses;
        }

        // Version không khớp → xóa cache cũ
        localStorage.removeItem(ADDRESS_CACHE_CONFIG.KEY);
        return null;
    } catch (e) {
        console.warn("Không đọc được localStorage:", e);
        return null;
    }
}

// Lưu data vào localStorage
function SaveAddressToCache(addresses) {
    try {
        const cacheData = {
            version: ADDRESS_CACHE_CONFIG.VERSION,
            addresses: addresses
        };
        localStorage.setItem(ADDRESS_CACHE_CONFIG.KEY, JSON.stringify(cacheData));
    } catch (e) {
        // localStorage full hoặc disabled → bỏ qua, vẫn hoạt động bình thường
        console.warn("Không lưu được localStorage:", e);
    }
}

async function GetAdministrativeAddress() {
    // Bước 1: Thử lấy từ cache trước
    let addresses = GetAddressFromCache();
    if (addresses) {
        if (DEBUG){
            console.log("✓ Load địa chỉ từ localStorage cache");
        }
        return addresses;
    }
    try {
        // Bước 2: Cache miss → load từ server
        if (DEBUG) {
            console.log("→ Load địa chỉ từ server...");
        }
        const searchParams = new URLSearchParams();
        let query = "/Home/GetAdministrativeAddress";

        const responseDB = await RequestHttpPostPromise(searchParams, query);

        // Bước 3: Lưu vào cache cho lần sau
        addresses = JSON.parse(responseDB.responseText);
        SaveAddressToCache(addresses);
        if (DEBUG) {
            console.log("✓ Đã cache địa chỉ vào localStorage");
        }
    } catch (e) {
        console.warn("Lỗi khi parse/cache địa chỉ:", e);
    }

    return addresses;
}

// Thay đổi tỉnh, add xã tương ứng vào select tag
function AddSubDistrict() {
    let provinceEle = document.getElementById("province");
    if (provinceEle.selectedIndex == 0 || provinceEle.selectedIndex == -1) {
        return;
    }

    let subdistrictEle = document.getElementById("subdistrict");
    // Trừ 1 vì option đầu là cấp độ: tỉnh, huyện, xã
    let subdistricts = administrativeAddressObject[provinceEle.selectedIndex - 1].subdistricts;

    let length = subdistricts.length;
    for (let i = 0; i < length; i++) {
        let option = document.createElement("option");
        option.value = subdistricts[i];
        option.text = subdistricts[i];
        subdistrictEle.appendChild(option);
    }
}

function DeleteSubDistrict() {
    let subdistrictEle = document.getElementById("subdistrict");
    for (let i = subdistrictEle.length - 1; i > 0; i--) {
        subdistrictEle.remove(i);
    }
    subdistrictEle.selectedIndex = 0;

    // Xóa detail-subdistrict
    document.getElementById("detail-subdistrict").value = "";
}

function ChangeProvince() {
    DeleteSubDistrict();
    AddSubDistrict();
}

function ChangeSubDistrict() {
    // Xóa detail-subdistrict
    document.getElementById("detail-subdistrict").value = "";
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

    document.getElementById("subdistrict").selectedIndex = 0;
    document.getElementById("detail-subdistrict").value = "";
    document.getElementById("check-default").checked = false;

}

// isCreate: true là thêm mới thông tin địa chỉ
// false: Cập nhật thông tin cũ
// isModalUnder: true, có modal bên dưới, cần tăng zindex = 2 ngược lại không cần
async function ShowCustomerInforModal(isCreate, isModalUnder, addressObj) {

    if (administrativeAddressObject == null) {
        // Lấy dữ liệu tỉnh, xã từ db
        administrativeAddressObject = await GetAdministrativeAddress();
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

// Tạo obj từ input
function CreateAddressObjFromInput() {
    let isDefault = 1;
    if (!document.getElementById("check-default").checked) {
        isDefault = 0;
    }
    let obj = new objCustomerAddressInforFromInput(
        document.getElementById("customer-name").value,
        document.getElementById("phone-number").value,
        document.getElementById("province").value,
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
