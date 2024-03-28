// Constructor function for pair key/value in customer infor
// format: name=Hoàng Huệ#phone=0359127226#province=Hà Nội#district=Bắc Từ Liêm#subdistrict=Cổ Nhuế 2#detail=Số 24 , Ngõ Việt Hà 2, khu tập thể Việt Hà, tổ dân phố Phú Minh#defaultAdd=1
// defaultAdd:1 địa chỉ nhận hàng mặc định, ngược lại là 0
function objCustomerInforCookie(value) {
    if (DEBUG) {
        console.log("objCustomerInforCookie(value) CALL " + value);
    }
    let myArray = value.split("#");
    this.id = -1;
    this.name = myArray[0].split("=")[1];
    this.phone = myArray[1].split("=")[1];
    this.province = myArray[2].split("=")[1];
    this.district = myArray[3].split("=")[1];
    this.subdistrict = myArray[4].split("=")[1];
    this.detail = myArray[5].split("=")[1];
    this.defaultAdd = parseInt(myArray[6].split("=")[1]);
}

function objCustomerInforCookieFromInput(inName,
    inPhone, inProvince, inDistrict, inSubDistrict,
    inDetail, inDefaultAdd
) {
    this.id = -1; // -1 khi là địa chỉ của khách vãng lai
    this.name = inName;
    this.phone = inPhone;
    this.province = inProvince;
    this.district = inDistrict;
    this.subdistrict = inSubDistrict;
    this.detail = inDetail;
    this.defaultAdd = inDefaultAdd;
}

// Từ object lấy được string cookie
// cookie có dạng: cusinfor=name=Hoàng Huệ#phone=0359127226#province=Hà Nội#district=Bắc Từ Liêm#subdistrict=Cổ Nhuế 2#detail=Số 24 , Ngõ Việt Hà 2, khu tập thể Việt Hà, tổ dân phố Phú Minh#defaultAdd=1
// ...
// name=Hoàng Huệ#phone=0359127226#province=Hà Nội#district=Bắc Từ Liêm#subdistrict=Cổ Nhuế 2#detail=Số 24 , Ngõ Việt Hà 2, khu tập thể Việt Hà, tổ dân phố Phú Minh#defaultAdd=1
function GetListCustomerInforCookieFromCookie() {
    let cookie = GetCookie(customerInforKey);
    let listCustomerInforCookie = [];
    if (isEmptyOrSpaces(cookie))
        return listCustomerInforCookie;

    let myArray = cookie.split("$");
    for (let i = 0; i < myArray.length; i++) {
        listCustomerInforCookie.push(new objCustomerInforCookie(myArray[i]));
    }
    return listCustomerInforCookie;
}

// Từ list object lấy được string cookie
function GetCookieFromListCustomerInfortCookie(listCustomerInforCookie) {
    let cookie = "";
    for (let i = 0; i < listCustomerInforCookie.length; i++) {
        let obj = listCustomerInforCookie[i];
        if (i == 0) {
            cookie = "name=" + obj.name + "#phone=" + obj.phone
                + "#province=" + obj.province
                + "#district=" + obj.district
                + "#subdistrict=" + obj.subdistrict
                + "#detail=" + obj.detail
                + "#defaultAdd=" + obj.defaultAdd.toString();
        }
        else {
            cookie = cookie + "$name=" + obj.name + "#phone=" + obj.phone
                + "#province=" + obj.province
                + "#district=" + obj.district
                + "#subdistrict=" + obj.subdistrict
                + "#detail=" + obj.detail
                + "#defaultAdd=" + obj.defaultAdd.toString();
        }
    }
    return cookie;
}

// listCartCookieObject: danh sách đối tượng cookie server trả về
function SetCartCookieFromListCustomerInforCookieObject(listCustomerInforCookie) {
    let newCart = GetCookieFromListCustomerInfortCookie(listCustomerInforCookie);
    SetCookie(customerInforKey, newCart, 365);
}

// Nếu đối tượng tồn tại, cập nhật
function InsertAtTailToListCustomerInforCookieCheckExist(listCustomerInforCookie, customerInforCookieObj) {
    let exist = false;
    for (let i = 0; i < listCustomerInforCookie.length; i++) {
        if (listCustomerInforCookie[i].name == customerInforCookieObj.name) {
            exist = true;
            listCustomerInforCookie[i] = customerInforCookieObj;
            break;
        }
    }
    if (exist == false) {
        listCustomerInforCookie.push(customerInforCookieObj);
    }
}

function DeleteAllCustomerInforCookie() {
    DeleteCookie(customerInforKey);
}