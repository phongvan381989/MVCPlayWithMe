// Constructor function for customer info object
// defaultAdd: 1 = địa chỉ nhận hàng mặc định, 0 = địa chỉ phụ
function objCustomerInfor(data) {
    if (typeof data === 'object') {
        // Từ object (JSON parsed hoặc input trực tiếp)
        this.id = data.id || -1;
        this.name = data.name || '';
        this.phone = data.phone || '';
        this.province = data.province || '';
        this.provinceId = data.provinceId || -1;
        this.subdistrict = data.subdistrict || '';
        this.subdistrictId = data.subdistrictId || -1;
        this.detail = data.detail || '';
        this.defaultAdd = data.defaultAdd || 0;
    } else {
        // Empty object
        this.id = -1;
        this.name = '';
        this.phone = '';
        this.province = '';
        this.provinceId = -1;
        this.subdistrict = '';
        this.subdistrictId = -1;
        this.detail = '';
        this.defaultAdd = 0;
    }
}

function objCustomerAddressInforFromInput(inName,
    inPhone, inProvince, inSubDistrict,
    inDetail, inDefaultAdd
) {
    this.id = -1; // -1 khi là địa chỉ của khách vãng lai
    this.name = inName;
    this.phone = inPhone;
    this.province = inProvince;
    this.subdistrict = inSubDistrict;
    this.detail = inDetail;
    this.defaultAdd = inDefaultAdd;
}

// ✅ Đọc danh sách địa chỉ từ localStorage
// Return: Array of plain objects (không cần constructor vì JSON.parse đã trả về object)
function GetListCustomerInforFromLocalStorage() {
    try {
        let data = localStorage.getItem(customerInforKey);
        if (!data || isEmptyOrSpaces(data)) {
            return [];
        }

        let jsonArray = JSON.parse(data);
        if (!Array.isArray(jsonArray)) {
            return [];
        }

        return jsonArray;
    } catch (e) {
        console.error("Error reading customer info from localStorage:", e);
        return [];
    }
}

// ✅ Lưu danh sách địa chỉ vào localStorage
function SaveListCustomerInforToLocalStorage(listCustomerInfor) {
    try {
        let jsonString = JSON.stringify(listCustomerInfor);
        localStorage.setItem(customerInforKey, jsonString);
        return true;
    } catch (e) {
        console.error("Error saving customer info to localStorage:", e);
        return false;
    }
}

// ✅ Insert hoặc Update địa chỉ (kiểm tra trùng theo name+phone)
// Nếu đã tồn tại → cập nhật, chưa tồn tại → thêm vào cuối
function InsertOrUpdateCustomerInfor(listCustomerInfor, customerInforObj) {
    let exist = false;

    // Check exist by name + phone (unique identifier cho khách vãng lai)
    for (let i = 0; i < listCustomerInfor.length; i++) {
        if (listCustomerInfor[i].name === customerInforObj.name &&
            listCustomerInfor[i].phone === customerInforObj.phone) {
            exist = true;
            listCustomerInfor[i] = customerInforObj;
            break;
        }
    }

    if (!exist) {
        listCustomerInfor.push(customerInforObj);
    }
}

// ✅ Xóa tất cả thông tin khách hàng
function DeleteAllCustomerInfor() {
    localStorage.removeItem(customerInforKey);
}

// ✅ Xóa 1 địa chỉ cụ thể (theo name + phone)
function DeleteCustomerInfor(name, phone) {
    let list = GetListCustomerInforFromLocalStorage();
    let filtered = list.filter(item => !(item.name === name && item.phone === phone));
    SaveListCustomerInforToLocalStorage(filtered);
    return filtered.length < list.length; // true nếu đã xóa
}

async function GetListAddress() {
    const searchParams = new URLSearchParams();
    let query = "/Customer/GetListAddress";
    return await RequestHttpPostPromise(searchParams, query);
}
