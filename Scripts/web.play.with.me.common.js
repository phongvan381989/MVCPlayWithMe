﻿var DEBUG = true;
function isEmptyOrSpaces(str) {
    return str === null || str.match(/^[ |	]*$/) !== null;
}

function GetExtensionOfFileName(fileName) {
    var fileExt = fileName.split('.').pop();
    return fileExt;
}
function CheckValidateEmail(email) {
    return email.match(
        /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
    );
}
// Có thể dùng SDT, Email hoặc tên đăng nhập làm tên tài khoản
function CheckUserNameValid(userName) {
    if (isEmptyOrSpaces(userName)) {
        return '{"isValid":false, "message":"Tên đăng nhâp không chính xác."}';
    }
    let length = userName.length;
    {
        // Nếu là SDT thì độ dài = 10 với số di động, 11 với số cố định
        let pattern = /[^0-9]/g;
        let result = userName.match(pattern);
        if (result === null) {
            if (length < 10) {
                return '{"isValid":false, "message":"Số điện thoại quá ngắn."}'
            }
            else if (length > 12) {
                return '{"isValid":false, "message":"Số điện thoại quá dài."}'
            }
            else {
                return '{"isValid":true, "message":"SDT"}'
            }
        }
    }
    {
        // Nếu là Email thì độ dài < 200
        if (CheckValidateEmail(userName)) {
            if (length > 200) {
                return '{"isValid":false, "message":"Email dài hơn 200 ký tự."}'
            }
            else {
                return '{"isValid":true, "message":"Email"}'
            }
        }
    }

    {
        // Nếu là tên đăng nhập thì độ dài < 50
        let pattern = /[^0-9a-zA-Z]/g;
        let result = userName.match(pattern);
        if (result !== null) {
            return '{"isValid":false, "message":"Tên đăng nhập chỉ chứa 0-9, a-z, A-Z."}';
        }
        if (length > 200) {
            return '{"isValid":false, "message":"Tên đăng nhập dài hơn 50 ký tự."}'
        }
        else {
            return '{"isValid":true, "message":"userName"}'
        }
    }

    return '{"isValid":false, "message":"Tên đăng nhập không đúng."}'
}

// Check SDT hợp lệ
function CheckValidSDT(sdt) {
    // Nếu là SDT thì độ dài = 10 với số di động, 11 với số cố định
    let length = userName.length;
    let pattern = /[^0-9]/g;
    let result = sdt.match(pattern);
    if (result === null) {
        if (length < 10) {
            return '{"isValid":false, "message":"Số điện thoại quá ngắn."}'
        }
        else if (length > 12) {
            return '{"isValid":false, "message":"Số điện thoại quá dài."}'
        }
        else {
            return '{"isValid":true, "message":"SDT"}'
        }
    }
}

function CheckPassWordValid(passWord, repassWord) {
    if (passWord !== repassWord) {
        return '{"isValid":false, "message":"Nhập lại mật khẩu không chính xác."}';
    }
    if (isEmptyOrSpaces(passWord)) {
        return '{"isValid":false, "message":"Mật khẩu không để ký tự trắng."}';
    }
    let length = passWord.length;
    if (length < 8) {
        return '{"isValid":false, "message":"Mật khẩu ít hơn 8 ký tự."}';
    }

    //if (!passWord.match(/[a-zA-Z]/)) {
    //    return '{"isValid":false, "message":"Mật khẩu cần chứa ít nhất 1 ký tự trong a-zA-Z."}';
    //}
    //if (!passWord.match(/[0-9]/)) {
    //    return '{"isValid":false, "message":"Mật khẩu cần chứa ít nhất 1 ký tự số."}';
    //}
    //if (!passWord.match(/[!@#$%^&*]/)) {
    //    return '{"isValid":false, "message":"Mật khẩu cần chứa ít nhất 1 ký tự đặc biệt !@#$%^&*."}';
    //}
    return '{"isValid":true, "message":"Mật khẩu ok"}';
}

function SetCookie(name, value) {
    document.cookie = name + '=' + value + '; Path=/;';
}

function DeleteCookie(name) {
    document.cookie = name + '=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}

function GetCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

// Kiểm tra 1 string có trong datalist
function CheckExistInDatalist(str, datalistId) {
    if (DEBUG) {
        console.log("CheckExistInDatalist - str: " + str);
    }
    const x = document.getElementById(datalistId);
    const y = x.getElementsByTagName("option");
    for (let i = 0; i < y.length; i++) {
        if (y[i].value == str) {
            if (DEBUG) {
                console.log("y.length: " + y.length);
                console.log("i: " + i);
                console.log("y[i].value: " + y[i].value);
            }
            return true;
        }
    }

    return false;
}

// Kiểm tra 1 string có trong barcode datalist
// str có dạng: -12345-, 12345, -12345, 12345-, -123456-78910-
function CheckExistInBarcodeDatalist(str, datalistId) {
    // Bỏ - ở đầu và cuối str
    let newstr = str.replace(/-/g, " ").trim();
    const myArr = newstr.split(" ");
    if (DEBUG) {
        console.log("newstr: " + newstr);
    }
    const x = document.getElementById(datalistId);
    const y = x.getElementsByTagName("option");
    for (let i = 0; i < y.length; i++) {
        for (let j = 0; j < myArr.length; j++) {
            if (y[i].value.includes("-" + myArr[j] + "-"))
                return true;
        }
    }

    return false;
}

// Lấy response và hiển thị thông báo
function GetJsonResponse(responseText) {
    const obj = JSON.parse(responseText);
    if (DEBUG) {
        console.log(obj);
    }
    if (obj == null)
        return false;

    document.getElementById("result-insert").innerHTML = obj.Message;
    if (obj.State == 0)
        return true;

    return false;
}

function CheckStatusResponse(responseText) {
    const obj = JSON.parse(responseText);
    if (DEBUG) {
        console.log(obj);
    }
    if (obj == null)
        return false;

    if (obj.State == 0)
        return true;

    return false;
}

// haveAlert: true hiển thị thông báo qua Alert ngược lại hiển thị text ra màn hình
function CheckStatusResponseAndShowPrompt(responseText, haveAlert, messageOk, messageError) {
    const obj = JSON.parse(responseText);
    if (DEBUG) {
        console.log(obj);
    }
    let isOk = true;
    let mess = "";
    if (obj == null) {
        isOk = false;
        //alert("Thao tác thất bại.");
    }
    else {
        if (obj.State != 0) {
            //alert("Thao tác thành công.");
            isOk = false;
        }
        //alert("Thao tác có lỗi.");
    }
    if (isOk) {
        mess = messageOk;
    }
    else {
        mess = messageError;
    }
    if (haveAlert) {
        alert(mess);
    }
    else {
        document.getElementById("result-insert").innerHTML = mess;
    }
    return isOk;
}

// Show text vào thẻ <p id="result-insert">
function ShowResult(str) {
    document.getElementById("result-insert").innerHTML = str;
    if (DEBUG) {
        console.log(str);
    }
}

// str: text cần check
// id của <p> hiển thị kết quả nếu text empty or space
function CheckIsEmptyOrSpacesAndShowResult(str, id, strResult) {
    if (isEmptyOrSpaces(str)) {
        document.getElementById(id).innerHTML = strResult;
        console.log("string null, empty or space!");
        return true;
    }

    return false;
}

// Lấy dữ liệu attribute data-id từ giá trị text đầu vào
// datalistId: id của <datalist>
// dataIdAttributeName: attribute data-id
// str: giá trị text đầu vào
function GetDataFromDatalist(datalistId, dataIdAttributeName, str)
{
    let option = document.getElementById(datalistId).options;
    if (option == null)
        return null;

    let length = option.length;
    for (let i = 0; i < length; i++) {
        if (option.item(i).value === str) {
            return option.item(i).getAttribute(dataIdAttributeName);
        }
    }
    return null;
}

// Lấy dữ liệu attribute data-id của publisher list
// str: giá trị text đầu vào
function GetDataIdFromPublisherDatalist(str)
{
    return GetDataFromDatalist("list-Publisher", "data-id", str);
}

// Lấy dữ liệu attribute data-id của combo list
// str: giá trị text đầu vào
function GetDataIdFromComboDatalist(str) {
    return GetDataFromDatalist("list-combo", "data-id", str);
}

// Lấy dữ liệu attribute data-id của category list
// str: giá trị text đầu vào
function GetDataIdFromCategoryDatalist(str) {
    return GetDataFromDatalist("list-category", "data-id", str);
}

// Lấy dữ liệu attribute data-id của sản phẩm cha list
// str: giá trị text đầu vào
function GetDataIdFromProductNameDatalist(str) {
    return GetDataFromDatalist("list-product-name", "data-id", str);
}

// Lấy dữ liệu attribute data-id của sản phẩm list
// str: giá trị text đầu vào
function GetDataIdFromParentlist(str) {
    return GetDataFromDatalist("list-parent", "data-id", str);
}

function RequestHttpPost(onloadFunc, searchParams, query) {
    const xhttp = new XMLHttpRequest();
    xhttp.onload = onloadFunc;

    if (DEBUG) {
        console.log(query);
        console.log(searchParams.toString());
    }
    xhttp.open("POST", query);
    xhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    xhttp.send(searchParams.toString());
}

function RequestHttpGet(onloadFunc, searchParams, query) {
    const xhttp = new XMLHttpRequest();
    xhttp.onload = onloadFunc;

    let lastQuery = query + "?" + searchParams.toString();
    if (DEBUG) {
        console.log(lastQuery);
    }
    xhttp.open("GET", lastQuery);
    xhttp.send();
}

// Nếu input type number trống, ta lấy giá trị mặc định -1
function GetValueOfNumberInputById(id, defaultValue) {
    let value = document.getElementById(id).value;
    if (isEmptyOrSpaces(value)) {
        value = defaultValue;
    }

    return value;
}

function Sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

//// Nếu input type number trống, ta lấy giá trị mặc định -1
//function GetValueOfNumberInputByIdAndSetURLSearchParams(id, searchParams) {
//    let value = document.getElementById(id).value;
//    if (isEmptyOrSpaces(value)) {
//        value = -1;
//    }

//    return value;
//}

// Check xem element có đang focus
function CheckFocus(id) {
    let dummyEl = document.getElementById(id);
    // check for focus
    let isFocused = (document.activeElement === dummyEl);

    return isFocused;
}