var DEBUG = true;
var thumbnailWidth = 150;
var thumbnailHeight = 150;
var avatarVideoHeight = 120;
var srcNoImageThumbnail = "/Media/NoImageThumbnail.png";
var itemModelQuota = 5;
var standardShipFeeInHaNoi = 15000; // Phí ship tiêu chuẩn trong Hà Nội
var standardShipFeeOutHaNoi = 30000; // Phí ship tiêu chuẩn ngoài Hà Nội
var HaNoiCity = "Thành phố Hà Nội";
var cartKey = "cart";
var customerInforKey = "cusinfor";
var uidKey = "uid";
var visitorType = "visitorType"; // Chỉ có cookie này khi đăng nhập như người quản trị
var eShopee = "SHOPEE";
var eTiki = "TIKI";
var eLazada = "LAZADA";
var ePlayWithMe = "PLAYWITHME";
var packedOrderStatusInWarehouse = "Đã Đóng";
var returnedOrderStatusInWarehouse = "Đã Hoàn";
var promoteMustClickOkModal; /* resolve-function reference */

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

function SetCookie(name, value, days) {
    //document.cookie = name + '=' + value + '; Path=/;';
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
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

// Lấy response và hiển thị thông báo
function GetJsonResponse(responseText) {
    const obj = JSON.parse(responseText);
    if (obj == null)
        return false;

    if (obj.State == 0)
        return true;

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
        CreateMustClickOkModal(messageError + " " + obj.Message, null);
    }

    return isOk;
}

// Check từ kết quả trả về của câu mysql
function CheckStatusAndShowPromptFromResponseObject(responseText) {
    const obj = JSON.parse(responseText);

    let mess = "Thao tác lỗi.";
    if (obj != null) {
        mess = obj.Message;
    }

    alert(mess);
}

function ShowResult(str) {

    alert(str);
}

// str: text cần check
// id của <p> hiển thị kết quả nếu text empty or space
function CheckIsEmptyOrSpacesAndShowResult(st, strResult) {
    if (isEmptyOrSpaces(str)) {
        ShowResult(strResult);
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
    if (DEBUG) {
        console.log("GetDataFromDatalist CALL value: " + str);
    }
    let option = document.getElementById(datalistId).options;
    if (option == null)
        return null;

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

// Xóa tất cả dữ liệu của bảng, để lại dòng đầu tiên
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
    if (src.includes("_320"))
        return src;
    // Nếu là NoImageThumbnail.png bỏ qua
    if (src.includes("NoImageThumbnail"))
        return src;

    let filename = src.replace(/^.*[\\/]/, '')
    let lastIndex = src.lastIndexOf(filename);
    let newSrc = src.substring(0, lastIndex - 1) + "_320/" + filename;
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
//            <button class='btn-modal-must-click-ok' type='button' onclick='CloseModalMustClickOk()'>OK</button>
//        </div>
//    </div>
//</div>
async function CreateMustClickOkModal(text, okFunction) {
    let container = document.createElement("div");
    container.className = "container-my-modal-must-click-ok";
    container.innerHTML = "<div class='my-modal-must-click-ok'><div class='modal-content-selected'><div class='alert-popup-message'></div><div><button class='btn-modal-must-click-ok' type='button'>OK</button></div></div></div>";
    container.getElementsByClassName("alert-popup-message")[0].innerHTML = text;
    container.getElementsByClassName("btn-modal-must-click-ok")[0].addEventListener("click", function () {
        document.getElementsByClassName("container-my-modal-must-click-ok")[0].remove();
        if (okFunction != null) {
            okFunction();
        }
        promoteMustClickOkModal("");
    });
    document.getElementsByTagName("body")[0].appendChild(container);
    document.getElementsByClassName("btn-modal-must-click-ok")[0].focus();
    promoteMustClickOkModal = null;
    let promise = new Promise((resolve) => { promoteMustClickOkModal = resolve });
    await promise.then((result) => {});
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
    window.location.href = "/Home/Index";

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

// Check khách vãng lai hay đăng nhập để hiển thị hành động tương ứng trên
// menu Tài Khoản góc trên phải màn hình
// Hàm này phải gọi mỗi khi load page
function ShowAccoutAction() {
    let ele = document.getElementsByClassName("dropdown-content")[0];
    if (ele == null) { // Đăng nhập với vai trò admin
        return;
    }

    ele.innerHTML = "";

    if (CheckAnonymousCustomer()) {
        // Đăng nhập
        CreateChildOfAccountElementV2(ele, "/Customer/Login", "Đăng nhập")

        // Đăng ký
        CreateChildOfAccountElementV2(ele, "/Customer/CreateCustomer", "Đăng ký")
    }
    else {

        // Thông tin tài khoản
        CreateChildOfAccountElementV2(ele, "/Customer/AccountInfor", "Thông tin tài khoản")

        // Đơn hàng của tôi
        CreateChildOfAccountElementV2(ele, "/Customer/Order", "Đơn hàng của tôi")

        // Đăng xuất
        CreateChildOfAccountElement(ele, function () { Logout(); }, "Đăng xuất")

    }
}

// Lấy được giá trị của tham số từ url và tên tham số
function GetValueFromUrlName(name) {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(name);
}

// Về trang chính
function GoHomePage() {
    if (!window.location.href.toUpperCase().includes("/Home/Index".toUpperCase())) {
        window.location.href = "/Home/Index";
    }
}

function GoMyCart() {
    window.location.href = "/Home/Cart";
}

async function AuthenFail() {

    await CreateMustClickOkModal("Xác thực người dùng thất bại.", null);

    // Quay về trang chủ
    window.location.href = "/Home/Index";
}

// Set min width khi hiển thị trên màn hình nhỏ
// ele: đối tượng html
function SetMinWidth(ele, minValue) {
    if (scrWidth <= minValue) {
        ele.style.minWidth = scrWidth + "px";
    }
}

// Cập nhật số sản phẩm trong giỏ hàng ( menu top), ẩn hiện icon giỏ hàng nếu cần thiết
async function UpdateCartCount() {
    let length = 0;
    if (CheckAnonymousCustomer()) {
        // Lấy giỏ hàng từ cookie
        let cartCookie = GetCookie(cartKey);
        let myArray = GetListCartCookieFromCartCookie(cartCookie);
        length = myArray.length;
    }
    else {
        const searchParams = new URLSearchParams();

        let query = "/Customer/GetCartCount";

        let responseDB = await RequestHttpPostPromise(searchParams, query);
        let result = JSON.parse(responseDB.responseText);

        // AUTHEN_FAIL
        if (result.State == 6) {
            AuthenFail();
            return;
        }
        length = result.myAnything;
    }

    document.getElementsByClassName("cart-count")[0].innerHTML = length;
}

// Với khách mọi page đều phải thực hiện sau khi load 
async function CommonAction() {
    if (DEBUG) {
        console.log("CommonAction CALL");
    }
    if (document.getElementById("biggestContainer_top") == null) {
        return;
    }
    // Chỉ hiện tìm kiếm trên Home page
    {
        let href = window.location.href.toUpperCase();
        if (href.endsWith("/HOME/INDEX") ||
            href.endsWith("/HOME/INDEX/") ||
            href.endsWith("/HOME") ||
            href.endsWith("/HOME/") ||
            href.endsWith("COM") ||
            href.endsWith("COM/") ||
            href.endsWith("56479") ||
            href.endsWith("56479/")) {
            document.getElementById("left_container").style.display = "flex";
        }
        else {
            document.getElementById("left_container").style.display = "none";
        }
    }
    if (CheckIsCustomer()) {
        await ShowAccoutAction();
        await UpdateCartCount();
    }
    else {
        // Ẩn thông tin account
        document.getElementsByClassName("top-account-container")[0].style.display = "none";
        document.getElementsByClassName("top-account-container")[0].style.display = "none";

        // Ẩn thông tin giỏ hàng
        document.getElementsByClassName("cart-container")[0].style.display = "none";
    }
}

CommonAction();

// ele là <datalist>
// list là danh sách dữ liệu có cấu trúc: id, name
function SetDataListOfIdName(ele, list) {
    if (ele != null) {
        ele.innerHTML = "";
    }
    if (ele == null || list == null) {
        return;
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
    const searchParams = new URLSearchParams();

    let query = "/Product/GetListProductName";

    let responseDB = await RequestHttpPostPromise(searchParams, query);
    let list = null;
    if (responseDB.responseText != "null") {
        list = JSON.parse(responseDB.responseText);
        let ele = document.getElementById("list-product-name");
        SetDataListOfIdName(ele, list);
    }
}

async function GetListCombo() {
    const searchParams = new URLSearchParams();

    let query = "/Combo/GetListCombo";

    let responseDB = await RequestHttpPostPromise(searchParams, query);
    let list = null;
    if (responseDB.responseText != "null") {
        list = JSON.parse(responseDB.responseText);
        let ele = document.getElementById("list-combo");
        SetDataListOfIdName(ele, list);
    }
}

async function GetListCategory() {
    const searchParams = new URLSearchParams();

    let query = "/Category/GetListCategory";

    let responseDB = await RequestHttpPostPromise(searchParams, query);
    let list = null;
    if (responseDB.responseText != "null") {
        list = JSON.parse(responseDB.responseText);
        let ele = document.getElementById("list-category");
        SetDataListOfIdName(ele, list);
    }
}

async function GetListAuthor() {
    const searchParams = new URLSearchParams();

    let query = "/Product/GetListAuthor";

    let responseDB = await RequestHttpPostPromise(searchParams, query);
    let list = null;
    if (responseDB.responseText != "null") {
        list = JSON.parse(responseDB.responseText);
        let ele = document.getElementById("list-author");
        SetDataListOfString(ele, list);
    }
}

async function GetListTranslator() {
    const searchParams = new URLSearchParams();

    let query = "/Product/GetListTranslator";

    let responseDB = await RequestHttpPostPromise(searchParams, query);
    let list = null;
    if (responseDB.responseText != "null") {
        list = JSON.parse(responseDB.responseText);
        let ele = document.getElementById("list-translator");
        SetDataListOfString(ele, list);
    }
}

async function GetListPublisher() {
    const searchParams = new URLSearchParams();

    let query = "/Publisher/GetListPublisher";

    let responseDB = await RequestHttpPostPromise(searchParams, query);
    let list = null;
    if (responseDB.responseText != "null") {
        list = JSON.parse(responseDB.responseText);
        let ele = document.getElementById("list-Publisher");
        SetDataListOfIdName(ele, list);
    }
}

async function GetListPublishingCompany() {
    const searchParams = new URLSearchParams();

    let query = "/Product/GetListPublishingCompany";

    let responseDB = await RequestHttpPostPromise(searchParams, query);
    let list = null;
    if (responseDB.responseText != "null") {
        list = JSON.parse(responseDB.responseText);
        let ele = document.getElementById("list-publishing-company");
        SetDataListOfString(ele, list);
    }
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
function GetTikiItemUrl(id) {
    return "https://tiki.vn/p" + id + ".html";
}

function GetProductUrl(id) {
    return "/Product/UpdateDelete?Id=" + id;
}
