function AddUpdateWithCommonParameters(searchParams) {

    let publisherName = document.getElementById("publisher-id").value;
    if (isEmptyOrSpaces(publisherName)) {
        ShowResult("Tên nhà phát hành trống.");
        return false;
    }
    let comboName = document.getElementById("combo-id").value;
    searchParams.append("comboName", comboName);

    let categoryName = document.getElementById("category-id").value;
    searchParams.append("categoryName", categoryName);

    let bookCoverPrice = GetValueInputById("book-cover-price", 0);
    searchParams.append("bookCoverPrice", bookCoverPrice);

    searchParams.append("author", document.getElementById("author-id").value);

    searchParams.append("translator", document.getElementById("translator-id").value);

    searchParams.append("publisherName", publisherName)

    searchParams.append("publishingCompany", document.getElementById("publishing-company-id").value);

    let publishingTime = GetValueInputById("publishing-time", -1);
    searchParams.append("publishingTime", publishingTime);

    let productLong = GetValueInputById("product-long", 0);
    searchParams.append("productLong", productLong);

    let productWide = GetValueInputById("product-wide", 0);
    searchParams.append("productWide", productWide);

    let productHigh = GetValueInputById("product-high", 0);
    searchParams.append("productHigh", productHigh);

    let productWeight = GetValueInputById("product-weight", 0);
    searchParams.append("productWeight", productWeight);

    searchParams.append("positionInWarehouse", document.getElementById("position-in-warehouse").value);

    let hardCover = document.getElementById("hard-cover").value;
    searchParams.append("hardCover", hardCover);

    let minAge = GetValueInputById("min-age", -1);
    searchParams.append("minAge", minAge);

    let maxAge = GetValueInputById("max-age", -1);
    searchParams.append("maxAge", maxAge);

    searchParams.append("republish", GetValueInputById("republish", -1));

    let productStatus = document.getElementById("product-status").value;
    searchParams.append("status", productStatus);

    return true;
}

function AddUpdateParameters(searchParams) {
    let productName = document.getElementById("product-name-id").value;
    if (isEmptyOrSpaces(productName)) {
        ShowResult("Tên sản phẩm trống.");
        return false;
    }

    let code = document.getElementById("code").value;
    if (!isEmptyOrSpaces(code)){
        if (code.length != 13 || code.substring(0, 2) != "89") {
            ShowResult("Mã sản phẩm không chính xác.");
            return false;
            }
    }

    let barcode = document.getElementById("barcode").value;
    if (!isEmptyOrSpaces(barcode)) {
        if (barcode.length != 13 || barcode.substring(0, 6) != "978604") {
            ShowResult("Mã ISBN không chính xác.");
            return false;
        }
    }

    if (AddUpdateWithCommonParameters(searchParams) === false) {
        return false;
    }

    searchParams.append("code", code);

    searchParams.append("barcode", barcode);

    searchParams.append("name", productName);

    let parentName = document.getElementById("parent-id").value;
    searchParams.append("parentName", parentName);

    searchParams.append("detail", document.getElementById("detail").value);
    return true;
}

//function AddNewPro() {
//    const searchParams = new URLSearchParams();
//    if (AddUpdateParameters(searchParams) === false)
//        return;

//    let query = "/Product/AddNewPro";
//    let productID = 0;
//    let OnloadFuntion = function () {
//        if (this.readyState == 4 && this.status == 200) {
//            if (GetJsonResponse(this.responseText)) {
//                // Bắt đầu upload ảnh/video sản phẩm lên server
//                const obj = JSON.parse(this.responseText);
//                productID = obj.myAnything;
//                SendFiles(productID);
//            }
//        }
//    }
//    RequestHttpGet(OnloadFuntion, searchParams, query);
//}

async function AddNewProPromise() {
    const searchParams = new URLSearchParams();
    if (AddUpdateParameters(searchParams) === false)
        return;

    let urlAdd = "/Product/AddNewPro";
    let urlUp = "/Product/UploadFile";
    let urlDeleteAllFileWithType = "";
    let productID = 0;

    try {
        // Cập nhật vào db
        let responseDB = await RequestHttpPostPromise(searchParams, urlAdd);

        const obj = JSON.parse(responseDB.responseText);
        productID = obj.myAnything;

        // Upload ảnh/video sản phẩm lên server
        let respinseSendFile = await SendFilesPromise(urlUp, urlDeleteAllFileWithType, productID);
    }
    catch (error) {

        alert("Tạo sản phẩm lỗi.");
        return;
    }

    // Đợi load ảnh xong
    while (true) {
        await Sleep(1000);

        if (isFinishUploadImage == 0 && isFinishUploadVideo == 0) {
            alert("Tạo sản phẩm thành công.");
            break;
        }
    }

    //// Refresh page
    //window.scrollTo(0, 0);
    //await Sleep(1000)
    //window.location.reload();
}

// Set thông tin chung của những sản phẩm thuộc combo về mặc định trừ trường combo
function SetProductCommonInfoWithComboToDefault() {
    document.getElementById("category-id").value = "";
    document.getElementById("book-cover-price").value = "";
    document.getElementById("author-id").value = "";
    document.getElementById("translator-id").value = "";
    document.getElementById("publisher-id").value = "";
    document.getElementById("publishing-company-id").value = "";
    document.getElementById("publishing-time").value = "";
    document.getElementById("product-long").value = "";
    document.getElementById("product-wide").value = "";
    document.getElementById("product-high").value = "";
    document.getElementById("product-weight").value = "";
    document.getElementById("position-in-warehouse").value = "";
    document.getElementById("hard-cover").value = 0;
    document.getElementById("min-age").value = "";
    document.getElementById("max-age").value = "";
    document.getElementById("republish").value = "";
    document.getElementById("product-status").value = 0;
}

// Set thông tin sản phẩm về mặc định trừ tên
function SetProductInfomationToDefault() {
    document.getElementById("code").value = "";
    document.getElementById("barcode").value = "";
    document.getElementById("combo-id").value = "";

    SetProductCommonInfoWithComboToDefault();

    document.getElementById("parent-id").value = "";
    document.getElementById("detail").value = "";
}

// Set thông tin chung của những sản phẩm thuộc combo trừ combo
function SetProductCommonInfoWithCombo(product) {
    document.getElementById("category-id").value = product.categoryName;
    document.getElementById("book-cover-price").value = product.bookCoverPrice;
    document.getElementById("author-id").value = product.author;
    document.getElementById("translator-id").value = product.translator;
    document.getElementById("publisher-id").value = product.publisherName;
    document.getElementById("publishing-company-id").value = product.publishingCompany;
    document.getElementById("publishing-time").value = product.publishingTime;
    document.getElementById("product-long").value = product.productLong;
    document.getElementById("product-wide").value = product.productWide;
    document.getElementById("product-high").value = product.productHigh;
    document.getElementById("product-weight").value = product.productWeight;
    document.getElementById("position-in-warehouse").value = product.positionInWarehouse;
    document.getElementById("hard-cover").value = product.hardCover;
    document.getElementById("min-age").value = product.minAge;
    document.getElementById("max-age").value = product.maxAge;
    document.getElementById("republish").value = product.republish;
    document.getElementById("product-status").value = product.status;
}

// Set thông tin sản phẩm trừ tên
function SetProductInfomation(product) {
    document.getElementById("quantity").value = product.quantity;
    document.getElementById("code").value = product.code;
    document.getElementById("barcode").value = product.barcode;
    document.getElementById("product-name-id").value = product.name;
    document.getElementById("combo-id").value = product.comboName;

    SetProductCommonInfoWithCombo(product);

    document.getElementById("parent-id").value = product.parentName;
    document.getElementById("detail").value = product.detail;

    InitializeImageList(product.imageSrc);
    InitializeVideoList(product.videoSrc);
}

// Thay đổi combo, hiển thị tất cả thông tin chung các sản phẩm cùng combo
// Thông tin chung này lấy từ sản phẩm đầu tiên thuộc combo trong db
function ComboChange(str) {
    let id = GetDataIdFromComboDatalist(str);
    if (id == null) {
        SetProductCommonInfoWithComboToDefault();
        return;
    }

    // Thông tin chung này lấy từ sản phẩm đầu tiên thuộc combo trong db
    const searchParams = new URLSearchParams();
    searchParams.append("id", id);
    let query = "/Product/GetProductCommonInfoWithComboFromFirst";

    let OnloadFuntion = function () {
        if (this.readyState == 4 && this.status == 200) {
            const product = JSON.parse(this.responseText);

            if (product == null) {
                SetProductCommonInfoWithComboToDefault();
                return;
            }
            SetProductCommonInfoWithCombo(product);
        }
    }

    RequestHttpPost(OnloadFuntion, searchParams, query);
}

async function UpdateProductPromise() {
    let productID = GetValueFromUrlName("id");
    if (productID == null) {
        ShowResult("Tên sản phẩm không chính xác.")
        return;
    }
    const searchParams = new URLSearchParams();
    AddUpdateParameters(searchParams);

    let url = "/Product/UpdateProduct";
    let urlUp = "/Product/UploadFile";
    let urlDeleteAllFileWithType = "/Product/DeleteAllFileWithType";

    try {
        // Cập nhật vào db
        let responseDB = await RequestHttpGetPromise(searchParams, url);

        // Upload ảnh/video sản phẩm lên server
        let respinseSendFile = await SendFilesPromise(urlUp, urlDeleteAllFileWithType, productID);
    }
    catch (error) {
        //alert("Cập nhật sản phẩm lỗi.");
        await CreateMustClickOkModal("Cập nhật sản phẩm lỗi.", null);
        return;
    }

    // Đợi load ảnh xong
    while (true) {
        await Sleep(1000);

        if (isFinishUploadImage == 0 && isFinishUploadVideo == 0) {
            alert("Cập nhật sản phẩm thành công.");
            break;
        }
    }

    // Refresh page
    //window.scrollTo(0, 0);
    //await Sleep(1000)
    //window.location.reload();
}

function DeleteProduct(){
    let text = "Bạn chắc chắn muốn XÓA?";
    if (confirm(text) == false)
        return;

    let id = GetValueFromUrlName("id");
    if (id == null)
        return;

    const searchParams = new URLSearchParams();
    searchParams.append("id", id);
    let query = "/Product/DeleteProduct";
    let OnloadFuntion = function () {
        if (this.readyState == 4 && this.status == 200) {
            GetJsonResponse(this.responseText);
            if (CheckStatusResponse(this.responseText)) {
                window.location.reload();
            }
        }
    }
    RequestHttpPost(OnloadFuntion, searchParams, query);
};

function UpdateCommonInfoWithCombo() {
    const searchParams = new URLSearchParams();
    if (AddUpdateWithCommonParameters(searchParams) === false) {
        return false;
    }

    let query = "/Product/UpdateCommonInfoWithCombo";
    let OnloadFuntion = function () {
        if (this.readyState == 4 && this.status == 200) {
            GetJsonResponse(this.responseText);
            if (CheckStatusResponseAndShowPrompt(this.responseText, "Cập nhật thành công", "Cập nhật thất bại")) {
                ReloadAndScrollToTop();
            }
        }
    }
    RequestHttpPost(OnloadFuntion, searchParams, query);
}

// HIển thị nút cập nhật cho riêng tên, isbn và mã sản phẩm
function ShowUpdateButtonForOne() {
    const collection = document.getElementsByClassName("diplay-none-when-create");

    for (let i = 0; i < collection.length; i++)
    {
        collection[i].style.display = "block";
    }
}

async function UpdateName() {
    const product = JSON.parse(document.getElementById("product-object").textContent);
    if (product == null) {
        ShowResult("Chưa chọn sản phẩm.");
        return false
    }

    let productName = document.getElementById("product-name-id").value;
    if (isEmptyOrSpaces(productName)) {
        ShowResult("Tên sản phẩm trống.");
        return false;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("id", product.id);
    searchParams.append("name", productName);

    let url = "/Product/UpdateName";

    try {
        // Cập nhật vào db
        let responseDB = await RequestHttpGetPromise(searchParams, url);
        CheckStatusAndShowPromptFromResponseObject(responseDB.responseText);
    }
    catch (error) {
        alert("Cập nhật tên lỗi.");
        return;
    }
}

async function UpdateCode() {
    const product = JSON.parse(document.getElementById("product-object").textContent);
    if (product == null) {
        ShowResult("Chưa chọn sản phẩm.");
        return false
    }

    let productCode = document.getElementById("code").value;

    const searchParams = new URLSearchParams();
    searchParams.append("id", product.id);
    searchParams.append("code", productCode);

    let url = "/Product/UpdateCode";

    try {
        // Cập nhật vào db
        let responseDB = await RequestHttpGetPromise(searchParams, url);
        CheckStatusAndShowPromptFromResponseObject(responseDB.responseText);
    }
    catch (error) {
        alert("Cập nhật mã lỗi.");
        return;
    }
}

async function UpdateISBN() {
    const product = JSON.parse(document.getElementById("product-object").textContent);
    if (product == null) {
        ShowResult("Chưa chọn sản phẩm.");
        return false
    }

    let productISBN = document.getElementById("barcode").value;

    const searchParams = new URLSearchParams();
    searchParams.append("id", product.id);
    searchParams.append("isbn", productISBN);

    let url = "/Product/UpdateISBN";

    try {
        // Cập nhật vào db
        let responseDB = await RequestHttpGetPromise(searchParams, url);
        CheckStatusAndShowPromptFromResponseObject(responseDB.responseText);
    }
    catch (error) {
        alert("Cập nhật tên lỗi.");
        return;
    }
}

async function GetItemObjectFromId(id) {
    const searchParams = new URLSearchParams();
    searchParams.append("id", id);

    let query = "/Product/GetItemObjectFromId";

    return RequestHttpPostPromise(searchParams, query);
}

async function ShowProductFromObject() {
    let responseDB = await GetItemObjectFromId(GetValueFromUrlName("id"));
    if (responseDB.responseText != null) {
        product = JSON.parse(responseDB.responseText);
    }
    else {
        product = null;
    }

    if (product == null) {
        SetProductInfomationToDefault();
        return;
    }

    SetProductInfomation(product);
}

function GetSomeData() {
    GetListCombo();
    GetListCategory();
    GetListAuthor();
    GetListTranslator();
    GetListPublisher();
    GetListPublishingCompany();
    SetListPublishingTime();
}
