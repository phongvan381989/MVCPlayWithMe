var DEBUG = true;
var thumbnailWidth = 150;
var thumbnailHeight = 150;
var avatarVideoHeight = 120;
var noImageThumbnailName = "NoImageThumbnail.png";
var srcNoImageThumbnail = "/Media/NoImageThumbnail.png";
var itemModelQuota = 5;
var standardShipFeeInHaNoi = 15000; // Phí ship tiêu chuẩn trong Hà Nội
var standardShipFeeOutHaNoi = 30000; // Phí ship tiêu chuẩn ngoài Hà Nội
var HaNoiCity = "Thành phố Hà Nội";
var cartKey = "cart";
var customerInforKey = "cusinfor";
var uidKey = "uid";
var visitorType = "visitorType"; // Chỉ có cookie này khi đăng nhập như người quản trị
var eAll = "ALL";
var ePlayWithMe = "PLAYWITHME";
var eTiki = "TIKI";
var eShopee = "SHOPEE";
var eLazada = "LAZADA";

var intAll = -1;
var intPlayWithMe = 0;
var intTiki = 1;
var intShopee = 2;
var intLazada = 3;
var packedOrderStatusInWarehouse = "Đã Đóng";
var returnedOrderStatusInWarehouse = "Đã Hoàn";
var bookedOrderStatusInWarehouse = "Giữ Chỗ";
var unbookedOrderStatusInWarehouse = "Hủy Giữ Chỗ";

var tikiConstDiscount = 10;

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

function ValidatePositiveIntegerNumber(ele) {
    if (ele.value < 0)
        ele.value = 0;
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

// Check SDT di động hợp lệ
// Đầu số Viettel: 086|096|097|098|039|038|037|036|035|034|033|032
// Đầu số Vinaphone: 091|094|088|083|084|085|081|082
// Đầu số MobiFone: 070|079|077|076|078|089|090|093
// Đầu số Vietnamobile: 092|052|056|058
// Đầu số Gmobile: 099|059
// Đầu số Itelecom: 087
function CheckValidSDT(sdt) {
    let pattern = /((086|096|097|098|039|038|037|036|035|034|033|032  |091|094|088|083|084|085|081|082  |070|079|077|076|078|089|090|093  |092|052|056|058  |099|059  |087)+([0-9]{7})\b)/g;
    if (!pattern.test(sdt)) {
        return false;
    }
    return true;
}

function IsValidString(str) {
    return typeof str === 'string' && str.trim() !== '';
}

// Check mã sản phẩm hợp lệ bắt đầu 89, dài 13
// 89..., VD: 8938519861794
function CheckValidProductCode(code) {
    let pattern = /((89)+([0-9]{11})\b)/g;
    if (!pattern.test(code)) {
        return false;
    }

    return true;
}

// Check isbn sản phẩm hợp lệ bắt đầu 978604, dài 13
// 978604..., VD: 9786046546948
function CheckValidProductISBN(code) {
    let pattern = /((978604)+([0-9]{7})\b)/g;
    if (!pattern.test(code)) {
        return false;
    }

    return true;

}
function CheckPassWordValid(passWord, repassWord) {
    if (passWord !== repassWord) {
        return '{"isValid":false, "message":"Nhập lại mật khẩu không chính xác."}';
    }
    if (isEmptyOrSpaces(passWord)) {
        return '{"isValid":false, "message":"Mật khẩu không để ký tự trắng."}';
    }
    //let length = passWord.length;
    //if (length < 8) {
    //    return '{"isValid":false, "message":"Mật khẩu ít hơn 8 ký tự."}';
    //}

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

function DeleteCookie(name) {
    document.cookie = name + '=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}

function GetCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    let cookie = "";
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            cookie = c.substring(name.length, c.length);
            break;
        }
    }

    return cookie;
}

// Kiểm tra 1 string có trong datalist
function CheckExistInDatalist(str, datalistId) {
    const x = document.getElementById(datalistId);
    const y = x.getElementsByTagName("option");
    for (let i = 0; i < y.length; i++) {
        if (y[i].value == str) {
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

function CheckStatusResponse(responseText) {
    const obj = JSON.parse(responseText);
    if (obj == null)
        return false;

    if (obj.State == 0)
        return true;

    return false;
}

// responseText là 1 đối tượng result
// Thành công thông báo alert, lỗi thông báo modal
function CheckStatusResponseAndShowPrompt(responseText, messageOk, messageError) {
    const obj = JSON.parse(responseText);

    let isOk = true;
    if (obj == null) {
        isOk = false;
    }
    else {
        if (obj.State != 0) {
            isOk = false;
        }
    }
    if (isOk) {
        alert(messageOk);
    }
    else {
        if (obj == null) {
            CreateMustClickOkModal(messageError + ". Server trả về null.", null);
        }
        else {
            CreateMustClickOkModal(messageError + " " + obj.Message, null);
        }
    }

    return isOk;
}

// str: text cần check
// id của <p> hiển thị kết quả nếu text empty or space
function CheckIsEmptyOrSpacesAndShowResult(str, strResult) {
    if (isEmptyOrSpaces(str)) {
        CreateMustClickOkModal(strResult, null);
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
    //if (DEBUG) {
    //    console.log("GetDataFromDatalist CALL value: " + str);
    //}
    let option = document.getElementById(datalistId).options;
    if (option === null) {
        return null;
    }

    let length = option.length;
    for (let i = 0; i < length; i++) {
        if (option.item(i).value === str) {
            if (DEBUG) {
                console.log(option.item(i).getAttribute(dataIdAttributeName));
            }
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

function RequestHttpPost(onloadFunc, searchParams, url) {
    const xhttp = new XMLHttpRequest();
    xhttp.onload = onloadFunc;

    xhttp.open("POST", url);
    xhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    xhttp.send(searchParams.toString());
}

function RequestHttpPostPromise(searchParams, url) {
    return new Promise(function (resolve, reject) {
        const xhttp = new XMLHttpRequest();
        xhttp.onload = function () {
            if (this.readyState == 4 && this.status == 200) {
                if (DEBUG) {
                    console.log(this.responseText);
                }
                resolve(this);
            }
        };
        xhttp.onerror = function () {
            reject(this.statusText);
        }

        if (DEBUG) {
            let lastQuery = url + "?" + searchParams.toString();
            console.log(lastQuery);
        }
        xhttp.open("POST", url);
        xhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
        xhttp.send(searchParams.toString());
    });
}

function RequestHttpPotstPromiseUploadFile(file, url) {
    return new Promise(function (resolve, reject) {
        const xhttp = new XMLHttpRequest();
        xhttp.onload = function () {
            if (this.readyState == 4 && this.status == 200) {
                if (DEBUG) {
                    console.log(this.responseText);
                }
                resolve(this);
            }
        };
        xhttp.onerror = function () {
            reject(this.statusText);
        }

        if (DEBUG) {
            console.log(url);
        }
        xhttp.open("POST", url);
        xhttp.setRequestHeader("Content-Type", "multipart/form-data");
        xhttp.send(file);
    });
}

function RequestHttpPostUpFilePromise(xhttp, url, file) {
    return new Promise(function (resolve, reject) {
        xhttp.onload = function () {
            if (this.readyState == 4 && this.status == 200) {
                //if (DEBUG) {
                //    console.log(this.responseText);
                //}
                resolve(this.responseText);
            }
        };
        xhttp.onerror = function () {
            reject(this.statusText)
        };

        xhttp.send(file);
    });
}

function RequestHttpGet(onloadFunc, searchParams, query) {
    const xhttp = new XMLHttpRequest();
    xhttp.onload = onloadFunc;

    let lastQuery = query + "?" + searchParams.toString();
    //if (DEBUG) {
    //    console.log(lastQuery);
    //}
    xhttp.open("GET", lastQuery);
    xhttp.send();
}

function RequestHttpGetPromise(searchParams, url) {
    return new Promise(function (resolve, reject) {
        const xhttp = new XMLHttpRequest();
        xhttp.onload = function () {
            if (this.readyState == 4 && this.status == 200) {
                if (DEBUG) {
                    console.log(this.responseText);
                }
                resolve(this);
            }
        };
        xhttp.onerror = function () {
            reject(this.statusText);
        }

        let lastQuery = url + "?" + searchParams.toString();
        if (DEBUG) {
            console.log(lastQuery);
        }
        xhttp.open("GET", lastQuery);
        xhttp.send();
    });
}

// Nếu input trống, ta lấy theo giá trị mặc định
function GetValueInputById(id, defaultValue) {
    let value = document.getElementById(id).value;
    if (isEmptyOrSpaces(value)) {
        value = defaultValue;
    }

    return value;
}

// Nếu input type number trống, ta lấy theo giá trị mặc định
function GetFloatValueFromInputTypeNumberById(id, defaultValue) {
    let inputElement = document.getElementById(id);
    let value = inputElement.value.trim(); // Lấy giá trị và loại bỏ khoảng trắng
    let number = value ? parseFloat(value) : defaultValue; // Nếu trống thì lấy 10

    return number;
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

// Xóa tất cả dữ liệu của bảng, để lại dòng đầu tiên chứa tên cột
function DeleteRowsExcludeHead(table) {
    let rows = table.rows;
    if (rows == null)
        return;
    let length = rows.length;
    for (let i = length - 1; i > 0; i--) {
        table.deleteRow(i);
    }
}

// reload and scroll to top
async function ReloadAndScrollToTop() {
    window.scrollTo(0, 0);
    await Sleep(1000);
    window.location.reload();
}

function TopFunction() {
    //if (DEBUG) {
    //    console.log("TopFunction CALL");
    //}
    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;
}

// Thêm nút xóa bên cạnh image/video trong <li>
function AddDeleteButton(li) {
    // Thêm nút xóa ảnh bên cạnh
    var btn = document.createElement("BUTTON");
    var btnContent = document.createTextNode("Xóa");
    btn.onclick = function () {
        this.parentElement.remove();
    }
    btn.appendChild(btnContent);
    li.appendChild(btn);
}

// Hiển thị vòng tròn đợi thao tác thực hiện xong
function ShowCircleLoader() {
    // add vào body tag
    let circleLoader = document.createElement("div");
    circleLoader.id = "circle-loader";
    document.getElementsByTagName("body")[0].appendChild(circleLoader);
}

function RemoveCircleLoader() {
    if (document.getElementById("circle-loader"))
        document.getElementById("circle-loader").remove();
}

// Kiểm tra value chỉ gồm chữ số
function IsNumeric(value) {
    let isnum = /^\d+$/.test(value);
    return isnum;
}

// Nếu input chưa nhập, hoặc nhập không phải số tự nhiên, số tự nhiên âm thì set giá trị là 0
function ConvertToInt(value) {
    if (isNaN(value)) {
        return 0;
    }

    let i = parseInt(value);
    if (i < 0) {
        i = 0;
    }
    return i;
}

// Convert số tiền sang text dạng: 123,456,700
function ConvertMoneyToText(money) {
    let text = money.toString();
    let textMoney = "";
    let length = text.length;
    for (let i = length - 1; i >= 0; i--) {
        textMoney = text.charAt(i) + textMoney;
        if ((i == length - 3 && length > 3) ||
            (i == length - 6 && length > 6) ||
            (i == length - 9 && length > 9)) {
            textMoney = "," + textMoney;
        }
    }

    return textMoney;
}

// Convert số tiền sang text dạng: đ123,456,700
function ConvertMoneyToTextWithIcon(money) {
    let text = ConvertMoneyToText(money);
    return "<sup>₫</sup>" + text;
}

// Convert text dạng: 123,456,700 sang số tiền
function ConvertTextToMoney(text) {
    let textMoney = "";
    let length = text.length;
    for (let i = length - 1; i >= 0; i--) {
        if (text.charAt(i).localeCompare(",") != 0) {
            textMoney = text.charAt(i) + textMoney;
        }
    }

    return ConvertToInt(textMoney);
}

function ConvertIntToPixel(value) {
    return value.toString() + "px";
}

// Từ src ảnh lấy được src phiên bản 320
// /Media/Item/578/2.jpg =>/Media/Item/578_320/2.jpg
function Get320VersionOfImageSrc(src) {
    // Nếu đã là phiên bản 320 bỏ qua
    if (src.includes("_320")) {
        return src;
    }
    // Nếu là NoImageThumbnail.png bỏ qua
    if (src.includes("NoImageThumbnail")) {
        return src;
    }

    let filename = src.replace(/^.*[\\/]/, '');


    let lastIndex = src.lastIndexOf(filename);
    // Nếu đuôi file khác .png, .jpg thì mặc định là .jpeg
    let newFileName = filename;
    const myArray = filename.split(".");
    if (myArray[1].toLowerCase() != "png" &&
        myArray[1].toLowerCase() != "jpg") {
        newFileName = myArray[0] + ".jpg";
    }
    let newSrc = src.substring(0, lastIndex - 1) + "_320/" + newFileName;
    return newSrc;
}

//  Set dữ liệu cho tag nếu có list-publishing-time
function SetListPublishingTime() {
    let ele = document.getElementById("list-publishing-time");
    const d = new Date();
    let year = d.getFullYear();
    let option = null;
    for (let i = 0; i < 10; i++) {
        option = document.createElement("option");
        option.text = year - i;
        ele.appendChild(option);
    }
}

// Tạo modal, bắt buộc phải click button ok để tắt modal
// text tham số hiển thị thông báo của modal
//<div class='my-modal-must-click-ok'>
//    <div class='modal-content-selected'>
//        <div class='alert-popup-message'>
//        </div>
//        <div>
//            <div class='div-modal-must-click-ok' onclick='CloseModalMustClickOk()'>OK</div>
//        </div>
//    </div>
//</div>
async function CreateMustClickOkModal(text, okFunction) {
    return new Promise((resolve, reject) => {
        let container = document.createElement("div");
        container.className = "container-my-modal-must-click-ok";
        container.innerHTML = "<div tabindex='0' class='my-modal-must-click-ok'><div class='modal-content-selected'><div class='alert-popup-message'></div><div><div class='div-modal-must-click-ok'>OK</div></div></div></div>";
        container.getElementsByClassName("alert-popup-message")[0].innerHTML = text;
        container.getElementsByClassName("div-modal-must-click-ok")[0].addEventListener("click", function () {
            document.getElementsByClassName("container-my-modal-must-click-ok")[0].remove();
            if (okFunction != null) {
                okFunction();
            }
            resolve("");
        });
        document.getElementsByTagName("body")[0].appendChild(container);
        document.getElementsByClassName("my-modal-must-click-ok")[0].focus();
    });
}

// Lấy được obj từ danh sách obj thỏa obj.id = id
function GetObjFromListAndId(id, list) {
    let obj = null;
    for (let i = 0; i < list.length; i++) {
        if (list[i].id == id) {
            obj = list[i];
            break;
        }
    }
    return obj;
}
// Trả về định dạng giống truy vấn httpPost
function GetEasyPromise() {
    return new Promise(function (resolve, reject) {
        let obj = { responseText: '{"State":0, "Message":""}' };
        resolve(obj);
    });
}

// Check khách vô danh
// Chưa đăng nhập trả về true, ngược lại false
function CheckAnonymousCustomer() {
    let cookie = GetCookie(uidKey);

    if (isEmptyOrSpaces(cookie)) {
        return true;
    }

    return false;
}

function CheckIsCustomer() {
    let visTypeCookie = GetCookie(visitorType);
    if (isEmptyOrSpaces(visTypeCookie)) {
        return true;
    }

    return false;
}

// Check khách vô danh
// Chưa đăng nhập trả về true, ngược lại false
async function CheckAnonymousCustomerFromServer() {
    const searchParams = new URLSearchParams();
    let query = "/Customer/CheckUidCookieValid";

    return await RequestHttpPostPromise(searchParams, query);
}

// Đăng xuất khỏi tài khoản
async function Logout() {
    const searchParams = new URLSearchParams();
    let query = "/Customer/Logout";

    await RequestHttpPostPromise(searchParams, query);

    // Uidkey được xóa từ phía server
    // Quay về trang chủ
    window.location.href = "/Home/Search";

}

// Tạo thẻ con của dropdown-content, hiển thị hành động tương ứng trên
// menu Tài Khoản góc trên phải màn hình
function CreateChildOfAccountElement(parrent, func, title) {
    let childDiv = document.createElement("div");
    childDiv.className = "remove-underline-container";

    let childA = document.createElement("a");
    childA.className = "remove-underline";
    childA.href = "javascript:void(0);";
    childA.title = title;
    childA.innerHTML = title;
    if (func != null) {
        childA.onclick = function () { func(); };
    }

    childDiv.appendChild(childA);
    parrent.appendChild(childDiv);
}

// Tạo thẻ con của dropdown-content, hiển thị hành động tương ứng trên
// menu Tài Khoản góc trên phải màn hình
function CreateChildOfAccountElementV2(parrent, href, title) {
    let childDiv = document.createElement("div");
    childDiv.className = "remove-underline-container";

    let childA = document.createElement("a");
    childA.className = "remove-underline";
    childA.href = href;
    childA.title = title;
    childA.innerHTML = title;
    //if (func != null) {
    //    childA.onclick = function () { func(); };
    //}

    childDiv.appendChild(childA);
    parrent.appendChild(childDiv);
}

// Tạo img tag
function CreateImageElement(/*width, height, */src, className) {
    let img = document.createElement("img");
    img.src = src;
    img.className = className;
    //img.height = height;
    //img.width = width;
    return img;
}

// Lấy được giá trị của tham số từ url và tên tham số
function GetValueFromUrlName(name) {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(name);
}

// Về trang chính
function GoHomePage() {
    //if (!window.location.href.toUpperCase().includes("/Home/Search".toUpperCase())) {
        window.location.href = "/";
    //}
}

function MouseEnterDropdownContentOfTopAccount() {
    let drop = document.getElementsByClassName("dropdown-content")[0];
    drop.style.display = "block";
}

function MouseLeaveDropdownContentOfTopAccount() {
    let drop = document.getElementsByClassName("dropdown-content")[0];
    drop.style.display = "none";
}

function ClickShowDropdownContentOfTopAccount() {
    let drop = document.getElementsByClassName("dropdown-content")[0];
    drop.style.display = "block";
}

function GoMyCart() {
    window.location.href = "/Home/Cart";
}

async function AuthenFail() {

    await CreateMustClickOkModal("Xác thực người dùng thất bại.", null);

    // Quay về trang chủ
    window.location.href = "/Home/Search";
}

// Set min width khi hiển thị trên màn hình nhỏ
// ele: đối tượng html
function SetMinWidth(ele, minValue) {
    if (scrWidth <= minValue) {
        ele.style.minWidth = scrWidth + "px";
    }
}

// ele là <datalist>
// list là danh sách dữ liệu có cấu trúc: id, name
function SetDataListOfIdName(ele, list) {

    if (ele == null || list == null) {
        return;
    }

    if (ele != null) {
        ele.innerHTML = "";
    }

    let length = list.length;
    let option = null;
    for (let i = 0; i < length; i++) {
        option = document.createElement("option");
        option.setAttribute("data-id", list[i].id);
        option.value = list[i].name;
        ele.appendChild(option);
    }
}

// ele là <datalist>
// List<string>
function SetDataListOfString(ele, list) {
    if (ele == null || list == null)
        return;

    let length = list.length;
    let option = null;
    for (let i = 0; i < length; i++) {
        option = document.createElement("option");
        option.text = list[i];
        ele.appendChild(option);
    }
}

async function GetListProductName() {
    if (document.getElementById("product-name-id") == null) {
        return;
    }
    document.getElementById("product-name-id").disabled = true;
    const searchParams = new URLSearchParams();

    let query = "/Product/GetListProductName";

    let responseDB = await RequestHttpPostPromise(searchParams, query);
    let list = null;
    if (responseDB.responseText != "null") {
        list = JSON.parse(responseDB.responseText);
        let ele = document.getElementById("list-product-name");
        SetDataListOfIdName(ele, list);
        document.getElementById("product-name-id").disabled = false;
    }
}

async function GetListCombo() {
    if (document.getElementById("combo-id") == null) {
        return;
    }
    document.getElementById("combo-id").disabled = true;
    const searchParams = new URLSearchParams();

    let query = "/Combo/GetListCombo";

    let responseDB = await RequestHttpPostPromise(searchParams, query);
    let list = [];
    if (responseDB.responseText != "null") {
        list = JSON.parse(responseDB.responseText);
        let ele = document.getElementById("list-combo");
        SetDataListOfIdName(ele, list);
        document.getElementById("combo-id").disabled = false;
    }
}

async function GetListCategory() {
    if (document.getElementById("category-id") == null) {
        return;
    }
    document.getElementById("category-id").disabled = true;
    const searchParams = new URLSearchParams();

    let query = "/Category/GetListCategory";

    let responseDB = await RequestHttpPostPromise(searchParams, query);
    let list = null;
    if (responseDB.responseText != "null") {
        list = JSON.parse(responseDB.responseText);
        let ele = document.getElementById("list-category");
        SetDataListOfIdName(ele, list);
        document.getElementById("category-id").disabled = false;
    }
}

async function GetListAuthor() {
    if (document.getElementById("author-id") == null) {
        return;
    }
    document.getElementById("author-id").disabled = true;
    const searchParams = new URLSearchParams();

    let query = "/Product/GetListAuthor";

    let responseDB = await RequestHttpPostPromise(searchParams, query);
    let list = null;
    if (responseDB.responseText != "null") {
        list = JSON.parse(responseDB.responseText);
        let ele = document.getElementById("list-author");
        SetDataListOfString(ele, list);
        document.getElementById("author-id").disabled = false;
    }
}

async function GetListTranslator() {
    if (document.getElementById("translator-id") == null) {
        return;
    }
    document.getElementById("translator-id").disabled = true;
    const searchParams = new URLSearchParams();

    let query = "/Product/GetListTranslator";

    let responseDB = await RequestHttpPostPromise(searchParams, query);
    let list = null;
    if (responseDB.responseText != "null") {
        list = JSON.parse(responseDB.responseText);
        let ele = document.getElementById("list-translator");
        SetDataListOfString(ele, list);
        document.getElementById("translator-id").disabled = false;
    }
}

async function GetListPubliserCore() {
    const searchParams = new URLSearchParams();
    let query = "/Publisher/GetListPublisher";

    let responseDB = await RequestHttpPostPromise(searchParams, query);
    let list = [];
    if (responseDB.responseText != "null") {
        list = JSON.parse(responseDB.responseText);
    }
    return list;
}
async function GetListPublisher() {
    if (document.getElementById("publisher-id") == null) {
        return;
    }
    document.getElementById("publisher-id").disabled = true;

    let list = await GetListPubliserCore();
    if (list.length > 0) {
        let ele = document.getElementById("list-Publisher");
        SetDataListOfIdName(ele, list);
        document.getElementById("publisher-id").disabled = false;
    }
}

async function GetListPublishingCompany() {
    if (document.getElementById("publishing-company-id") == null) {
        return;
    }
    document.getElementById("publishing-company-id").disabled = true;
    const searchParams = new URLSearchParams();

    let query = "/Product/GetListPublishingCompany";

    let responseDB = await RequestHttpPostPromise(searchParams, query);
    let list = null;
    if (responseDB.responseText != "null") {
        list = JSON.parse(responseDB.responseText);
        let ele = document.getElementById("list-publishing-company");
        SetDataListOfString(ele, list);
    }
    document.getElementById("publishing-company-id").disabled = false;
}

async function GetTaxAndFeeCore(eEcommerceName) {
    const searchParams = new URLSearchParams();
    searchParams.append("eEcommerceName", eEcommerceName);
    let query = "/TikiDealDiscount/GetTaxAndFeeCore";

    let responseDB = await RequestHttpPostPromise(searchParams, query);

    let taxAndFee = null;
    if (responseDB.responseText != "null") {
        taxAndFee = JSON.parse(responseDB.responseText);
    }
    return taxAndFee;
}

async function GetListItem() {
    const searchParams = new URLSearchParams();

    let query = "/ItemModel/GetListItem";

    let responseDB = await RequestHttpPostPromise(searchParams, query);
    let list = null;
    if (responseDB.responseText != "null") {
        list = JSON.parse(responseDB.responseText);
        let ele = document.getElementById("list-item-name");
        SetDataListOfIdName(ele, list);
    }
}

function GetShopeeItemUrl(itemid) {
    return "https://shopee.vn/product/137637267/" + itemid;
}

// https://tiki.vn/p76217978.html
// id: 76217978
function GetTikiItemUrl(itemid) {
    return "https://tiki.vn/p" + itemid + ".html";
}

function GetTMDTItemUrlFromCommonItem(commonItem) {
    let itemUrl = "";
    // Item là Tiki
    if (commonItem.eType == eTiki) {
        if (commonItem.tikiSuperId != 0) {// Có sản phẩm super Id
            itemUrl = GetTikiItemUrl(commonItem.tikiSuperId);
        }
        else {
            itemUrl = GetTikiItemUrl(commonItem.itemId);
        }
    }
    // Item là shopee
    else if (commonItem.eType == eShopee) {
        itemUrl = GetShopeeItemUrl(commonItem.itemId);
    }
    return itemUrl;
}

function GetProductUrl(id) {
    return "/Product/UpdateDelete?id=" + id;
}

// Không tìm thấy đối tượng, hiển thông báo
function ShowDoesntFindId() {
    document.getElementById("result-find-id").remove();
    let ele = document.getElementById("doesnt-find-id");
    ele.style.display = "flex";
    ele.style.alignItems = "center";
    ele.style.justifyContent = "center";
}

function ShowCloseEye(ele) {
    ele.nextElementSibling.style.display = "block";
    ele.previousElementSibling.type = "password";

    ele.style.display = "none";
}

function ShowOpenEye(ele) {
    ele.previousElementSibling.style.display = "block";
    ele.previousElementSibling.previousElementSibling.type = "text";

    ele.style.display = "none";
}

async function CopyImageFromTMDTToWarehouseProduct(eType, imageUrl, productId) {
    const searchParams = new URLSearchParams();
    searchParams.append("eType", eType);
    searchParams.append("imageUrl", imageUrl);
    searchParams.append("productId", productId);


    let query = "/ProductECommerce/CopyImageFromTMDTToWarehouseProduct";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();
    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Chép ảnh thành công.", "Chép ảnh thất bại.");
}

async function UpdateStatusOfProduct(id, productStatus) {
    const searchParams = new URLSearchParams();
    searchParams.append("id", id);
    searchParams.append("statusOfProduct", productStatus);

    let url = "/Product/UpdateStatusOfProduct";
    let isOK = true;
    try {
        // Cập nhật vào db
        ShowCircleLoader();
        let responseDB = await RequestHttpGetPromise(searchParams, url);
        RemoveCircleLoader();
        CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật thành công.", "Cập nhật thất bại.");
    }
    catch (error) {
        await CreateMustClickOkModal("Cập nhật trạng thái sản phẩm lỗi.", null);
        isOK = false;
    }

    return new Promise((resolve) => {
        resolve(isOK);
    });
}

// Trả về tên sàn
function GetECommerceType() {
    let ecommerce = eAll; // Tương ứng tất cả

    if (document.getElementById("tiki-e-ecommonerce-type").checked == true) {
        ecommerce = eTiki;
    }
    else if (document.getElementById("shopee-e-ecommonerce-type").checked == true) {
        ecommerce = eShopee;
    }
    else if (document.getElementById("lazada-e-ecommonerce-type").checked == true) {
        ecommerce = eLazada;
    }
    else if (document.getElementById("play-with-me-e-ecommonerce-type").checked == true) {
        ecommerce = ePlayWithMe;
    }

    return ecommerce;
}

// Trả về chỉ số sàn kiểu int
function GetIntECommerceType() {
    let ecommerce = intAll; // Tương ứng tất cả

    if (document.getElementById("tiki-e-ecommonerce-type").checked == true) {
        ecommerce = intTiki;
    }
    else if (document.getElementById("shopee-e-ecommonerce-type").checked == true) {
        ecommerce = intShopee;
    }
    else if (document.getElementById("lazada-e-ecommonerce-type").checked == true) {
        ecommerce = intLazada;
    }
    else if (document.getElementById("play-with-me-e-ecommonerce-type").checked == true) {
        ecommerce = intPlayWithMe;
    }

    return ecommerce;
}


// --------- Xử lý tiền ----------------
// Add event khi nhập số tiền
function AddEventFormatInputVND(inputId) {
    const input = document.getElementById(inputId);

    // Format khi người dùng nhập
    input.addEventListener('input', function () {
        // Bỏ dấu phẩy cũ, lấy số "sạch"
        let rawValue = input.value.replace(/,/g, '');

        // Nếu là số, format lại
        if (!isNaN(rawValue) && rawValue !== '') {
            input.value = Number(rawValue).toLocaleString('en-US');
        }
    });
}

function SetFormattedMoneyInput(inputId, rawValue) {
    const input = document.getElementById(inputId);
    if (input) {
        const formatted = Number(rawValue || "").toLocaleString('en-US');
        input.value = formatted;
    }
}

// Khi cần lấy giá trị tiền để xử lý (submit, tính toán, v.v...)
function GetVNDValue(inputId) {
    const input = document.getElementById(inputId);
    let value = input.value.replace(/,/g, '');
    return parseInt(value, 10) || 0;
}

// Sắp xếp cột của bảng tăng hoặc giảm dần khi click vào <th>. Cột chứa giá trị số nguyên hoặc string nhưng
// lấy được số nguyên đầu tiên VD: 1234 con vịt => 1234
function SortTable(tableId, thEle) {
    const table = document.getElementById(tableId);
    const rows = Array.from(table.rows).slice(1); // Exclude the first row (header)

    // Determine if sorting is ascending or descending
    let isAscending = table.getAttribute("data-sort-order") !== "asc";
    table.setAttribute("data-sort-order", isAscending ? "asc" : "desc");
    let columnIndex = thEle.cellIndex;
    // Sort rows
    rows.sort((a, b) => {
        let valA = ExtractNumber(a.cells[columnIndex].innerText.trim());
        let valB = ExtractNumber(b.cells[columnIndex].innerText.trim());

        if (valA < valB) return isAscending ? -1 : 1;
        if (valA > valB) return isAscending ? 1 : -1;
        return 0;
    });

    // Reorder rows, keeping the header row unchanged
    rows.forEach(row => table.appendChild(row));
}

function ExtractNumber(text) {
    const match = text.match(/\d+/); // Extract the first number in the string
    return match ? parseInt(match[0], 10) : 0; // Convert to integer, default to 0 if no number found
}

// Từ loại sàn kiểu int trả về tên sàn
//public enum EECommerceType {
//    PLAYWITHME,
//    TIKI,
//    SHOPEE,
//    LAZADA,
//    ALL
//}
function GetEEcomnerceNameFromIntType(type) {
    let name = ePlayWithMe;
    if (type === intTiki) {
        name = eTiki;
    }
    else if (type === intShopee) {
        name = eShopee;
    }
    else if (type === intLazada) {
        name = eLazada;
    }
    return name;
}

// Hàm cập nhật cột STT, đánh số từ 1, trừ dòng tiêu đề
function updateSTT(tableId, sttIndex) {
    // Lấy bảng theo ID
    const table = document.getElementById(tableId);

    // Lấy tất cả các hàng (tr) của bảng
    const rows = table.getElementsByTagName("tr");

    // Duyệt qua các hàng, bắt đầu từ hàng thứ 2 (bỏ qua hàng tiêu đề)
    for (let i = 1; i < rows.length; i++) {
        const sttCell = rows[i].cells[sttIndex]; // chỉ số cột STT
        sttCell.textContent = i; // Gán số thứ tự
    }
}

// Từ file csv lấy được thông tin nhập kho
//code, quantity
//8938532871558, 10
//8938532871565, 10
//8938532871572, 10
function ConvertCSVForImport(csvId) {
    let data = [];
    const csvText = document.getElementById(csvId).value.trim();
    if (DEBUG) {
        console.log("csvText: " + csvText);
    }
    if (!csvText) {
        return data;
    }

    const rows = csvText.split('\n');
    const headers = rows[0].split(',');

    data = rows.slice(1).map(row => {
        const values = row.split(',');
        return {
            [headers[0]]: values[0],
            [headers[1]]: values[1] ? parseInt(values[1], 10) : 0
        };
    });
    if (DEBUG) {
        console.log("return of convertToCSV: " + JSON.stringify(data));
    }
    return data;
}


// Hàm cập nhật STT, hiển thị ở title
// indexColumn: Cột cập nhật title nội dung STT
// isHeader: true nếu table có dòng đầu là header
function UpdateSTT(table, indexColumn, isHeader) {
    if (DEBUG) {
        console.log("UpdateSTT CALL");
    }
    // Lấy tất cả các hàng trong bảng
    const rows = table.querySelectorAll('tbody tr');

    // Xác định điểm bắt đầu (nếu có header thì bỏ qua dòng đầu)
    const startIndex = isHeader ? 1 : 0;

    // Duyệt qua các hàng để cập nhật STT
    rows.forEach((row, index) => {
        if (index >= startIndex) {
            const sttCell = row.cells[indexColumn]; // Lấy ô cần cập nhật STT
            const sttValue = index - startIndex + 1; // Tính số thứ tự

            // Cập nhật thuộc tính title và nội dung ô (nếu cần)
            sttCell.setAttribute('title', `STT: ${sttValue}`);
        }
    });
}

// Viết hoa chữ cái đầu của từ
function CapitalizeWords(input) {
    return input
        .toLowerCase() // Chuyển toàn bộ chuỗi về chữ thường
        .split(' ')    // Tách chuỗi thành mảng các từ
        .map(word => {
            let i = 0;
            // Bỏ qua các ký tự không phải chữ cái (hỗ trợ Unicode)
            while (i < word.length && !/\p{L}/u.test(word[i])) {
                i++;
            }
            // Nếu tìm thấy ký tự chữ cái, viết hoa ký tự đó
            return word.slice(0, i) +
                (i < word.length ? word[i].toUpperCase() : '') +
                word.slice(i + 1);
        })
        .join(' '); // Gộp các từ lại thành chuỗi
}

// Loại bỏ khoảng trắng ở đầu và cuối, viết hoa chữ cái đầu
function TrimCapitalizeWords(input) {
    let inputTemp = input.trim();
    return inputTemp
        .toLowerCase() // Chuyển toàn bộ chuỗi về chữ thường
        .split(' ')    // Tách chuỗi thành mảng các từ
        .map(word => word.charAt(0).toUpperCase() + word.slice(1)) // Viết hoa chữ cái đầu và giữ phần còn lại
        .join(' ');    // Gộp các từ lại thành chuỗi
}

// Cập nhật màu tên sản phẩm dựa theo trạng thái kinh doanh, ngừng kinh doanh, tồn kho
function UpdateProductNameStyle(p, status, quantity) {
    if (status == 2)// ngừng kinh doanh
    {
        p.style.color = "aqua";
        p.title = "Sản phẩm ngừng kinh doanh";
    }
    else if (quantity == 0) {
        p.style.color = "red";
        p.title = "Sản phẩm hết hàng";
    }
    else {
        p.style.color = "initial";
    }
}