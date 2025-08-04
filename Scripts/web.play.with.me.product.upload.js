function AddUpdateWithCommonParameters(searchParams) {

    let publisherName = document.getElementById("publisher-id").value;
    if (isEmptyOrSpaces(publisherName)) {
        CreateMustClickOkModal("Tên nhà phát hành trống.", null);
        return false;
    }

    //let comboName = document.getElementById("combo-id").value;
    //searchParams.append("comboName", comboName);

    let categoryName = document.getElementById("category-id").value;
    let categoryId = GetDataIdFromCategoryDatalist(categoryName);
    if (categoryId == null) {
        searchParams.append("categoryId", -1);
    }
    else {
        searchParams.append("categoryId", categoryId);
    }

    let bookCoverPrice = GetValueInputById("book-cover-price", 0);
    searchParams.append("bookCoverPrice", bookCoverPrice);

    searchParams.append("discount", GetValueInputById("discount-when-import", 0));

    searchParams.append("author", document.getElementById("author-id").value);

    searchParams.append("translator", document.getElementById("translator-id").value);

    let publisherId = GetDataIdFromPublisherDatalist(publisherName);
    if (publisherId == null) {
        CreateMustClickOkModal("Tên nhà phát hành không chính xác. Vui lòng kiểm tra lại.", null);
        return false;
    }
    else {
        searchParams.append("publisherId", publisherId);
    }

    searchParams.append("publishingCompany", document.getElementById("publishing-company-id").value);

    let publishingTime = GetValueInputById("publishing-time", -1);
    searchParams.append("publishingTime", publishingTime);

    let pageNumber = GetValueInputById("page-number", 0);
    searchParams.append("pageNumber", pageNumber);

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

    let bookLanguge = document.getElementById("book-language-id").value;
    searchParams.append("bookLanguge", bookLanguge);

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
        CreateMustClickOkModal("Tên sản phẩm trống.", null);
        return false;
    }

    let code = document.getElementById("code").value;
    //if (!IsValidString(code)) {
    //    //if (code.length != 13 || code.substring(0, 2) != "89") {
    //        CreateMustClickOkModal("Mã sản phẩm không chính xác.", null);
    //        return false;
    //    //}
    //}

    let barcode = document.getElementById("barcode").value;
    //if (!IsValidString(barcode)) {
    //    //if (barcode.length != 13 || barcode.substring(0, 6) != "978604") {
    //        CreateMustClickOkModal("Mã ISBN không chính xác.", null);
    //        return false;
    //    //}
    //}

    searchParams.append("quantity", GetValueInputById("quantity", 0));

    searchParams.append("code", code);

    searchParams.append("barcode", barcode);

    searchParams.append("name", CapitalizeWords(productName));

    let comboId = GetDataIdFromComboDatalist(document.getElementById("combo-id").value);
    if (comboId == null) {
        comboId = -1;
    }
    searchParams.append("comboId", comboId);

    searchParams.append("parentId", -1);

    let encodeDetail = encodeURIComponent(document.getElementById("detail").value)
    searchParams.append("detail", encodeDetail);

    if (AddUpdateWithCommonParameters(searchParams) === false) {
        return false;
    }

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
//            if (CheckStatusResponse(this.responseText)) {
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
        ShowCircleLoader();
        // Cập nhật vào db
        let responseDB = await RequestHttpPostPromise(searchParams, urlAdd);

        const obj = JSON.parse(responseDB.responseText);
        if (obj == null) {
            CreateMustClickOkModal("Có lỗi xẩy ra.", null);
            RemoveCircleLoader();
            return;
        }
        if (obj.State != 0) {
            CreateMustClickOkModal("Có lỗi xẩy ra." + " " + obj.Message, null);
            RemoveCircleLoader();
            return;
        }

        productID = obj.myAnything;

        // Upload ảnh/video sản phẩm lên server
        let isOk = await SendFilesPromise(urlUp, urlDeleteAllFileWithType, productID);
        if (isOk) {
            alert("Tạo sản phẩm thành công.");
        }
    }
    catch (error) {
        RemoveCircleLoader();
        CreateMustClickOkModal("Tạo sản phẩm lỗi.", null);
        return;
    }

    // Đợi load ảnh xong
    //while (true) {
    //    await Sleep(1000);

    //    if (isFinishUploadImage == 0 && isFinishUploadVideo == 0) {
    //        alert("Tạo sản phẩm thành công.");
    //        break;
    //    }
    //}
    RemoveCircleLoader();
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

// Set thông tin chung của những sản phẩm thuộc combo
function SetProductCommonInfoWithCombo(product) {
    document.getElementById("category-id").value = product.categoryName;
    document.getElementById("book-cover-price").value = product.bookCoverPrice;
    document.getElementById("discount-when-import").value = product.discount;
    document.getElementById("author-id").value = product.author;
    document.getElementById("translator-id").value = product.translator;
    document.getElementById("publisher-id").value = product.publisherName;
    document.getElementById("publishing-company-id").value = product.publishingCompany;
    if (product.publishingTime != -1) {
        document.getElementById("publishing-time").value = product.publishingTime;
    }
    if (product.pageNumber != -1) {
        document.getElementById("page-number").value = product.pageNumber;
    }

    if (product.productLong != -1) {
        document.getElementById("product-long").value = product.productLong;
    }
    if (product.productWide != -1) {
        document.getElementById("product-wide").value = product.productWide;
    }
    if (product.productHigh != -1) {
        document.getElementById("product-high").value = product.productHigh;
    }
    if (product.productWeight != -1) {
        document.getElementById("product-weight").value = product.productWeight;
    }
    document.getElementById("position-in-warehouse").value = product.positionInWarehouse;
    document.getElementById("hard-cover").value = product.hardCover;
    document.getElementById("book-language-id").value = product.language;
    if (product.minAge != -1) {
        document.getElementById("min-age").value = product.minAge;
    }
    if (product.maxAge != -1) {
        document.getElementById("max-age").value = product.maxAge;
    }
    if (product.republish != -1) {
        document.getElementById("republish").value = product.republish;
    }
    document.getElementById("product-status").value = product.status;
}

// Set thông tin sản phẩm trừ tên
function SetProductInfomation(product) {
    document.getElementById("quantity").value = product.quantity;
    document.getElementById("code").value = product.code;
    document.getElementById("barcode").value = product.barcode;
    document.getElementById("product-name-id").value = product.name;
    document.getElementById("combo-id").value = product.comboName;
    document.getElementById("combo-id").comboIdValue = product.comboId;

    SetProductCommonInfoWithCombo(product);

    document.getElementById("parent-id").value = product.parentName;
    document.getElementById("detail").value = product.detail;

    InitializeImageList(product.imageSrc);
    InitializeVideoList(product.videoSrc);
}

async function UpdateProductPromise() {
    let productID = GetValueFromUrlName("id");
    const searchParams = new URLSearchParams();
    searchParams.append("productId", productID);
    if (!AddUpdateParameters(searchParams)) {
        return;
    }

    let url = "/Product/UpdateProduct";
    let urlUp = "/Product/UploadFile";
    let urlDeleteAllFileWithType = "/Product/DeleteAllFileWithType";

    try {
        ShowCircleLoader();
        // Cập nhật vào db
        let responseDB = await RequestHttpPostPromise(searchParams, url);
        const obj = JSON.parse(responseDB.responseText);
        if (obj == null) {
            CreateMustClickOkModal("Có lỗi xẩy ra.", null);
            RemoveCircleLoader();
            return;
        }
        if (obj.State != 0) {
            CreateMustClickOkModal("Có lỗi xẩy ra." + " " + obj.Message, null);
            RemoveCircleLoader();
            return;
        }

        // Upload ảnh/video sản phẩm lên server
        let respinseSendFile = await SendFilesPromise(urlUp, urlDeleteAllFileWithType, productID);
        if (respinseSendFile) {
            alert("Cập nhật sản phẩm thành công.");
        }
    }
    catch (error) {
        RemoveCircleLoader();
        //alert("Cập nhật sản phẩm lỗi.");
        await CreateMustClickOkModal("Cập nhật sản phẩm lỗi. " + error.message, null);
        return;
    }

    //// Đợi load ảnh xong
    //while (true) {
    //    await Sleep(1000);

    //    if (isFinishUploadImage == 0 && isFinishUploadVideo == 0) {
    //        alert("Cập nhật sản phẩm thành công.");
    //        break;
    //    }
    //}
    RemoveCircleLoader();
    // Refresh page
    //window.scrollTo(0, 0);
    //await Sleep(1000)
    //window.location.reload();
}

async function DeleteProduct(){
    let text = "Sản phẩm chỉ có thế xóa khi đang không liên kết với sản phẩm nào trên Shopee, Tiki, Lazada, web voibenho, trong đơn hàng đã được bán, thông tin nhập xuất thực tế,.... Bạn sẽ vẫn thấy sản phẩm dù thông báo xóa thành công.  Bạn chắc chắn muốn XÓA?";
    if (confirm(text) == false)
        return;

    let id = GetValueFromUrlName("id");
    if (id == null)
        return;

    const searchParams = new URLSearchParams();
    searchParams.append("id", id);
    let query = "/Product/DeleteProduct";
    //let OnloadFuntion = function () {
    //    if (this.readyState == 4 && this.status == 200) {
    //        if (CheckStatusResponse(this.responseText)) {
    //            window.location.reload();
    //        }
    //    }
    //}
    //RequestHttpPost(OnloadFuntion, searchParams, query);
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Xóa thành công.", "Có lỗi xảy ra.");
};


// HIển thị nút cập nhật cho riêng tên, isbn, mã sản phẩm,...
// Cho phép cập nhật tồn kho
function ShowUpdateButtonForOne() {
    let collection = document.getElementsByClassName("diplay-none-when-create");

    for (let i = 0; i < collection.length; i++)
    {
        collection[i].style.display = "block";
    }

    collection = document.getElementsByClassName("disable-true-when-create");

    for (let i = 0; i < collection.length; i++) {
        //if (DEBUG) {
        //    console.log("collection[i].disabled: " + collection[i].disabled);
        //}
        collection[i].disabled = false;
    }
}

async function UpdateImage_Video() {
    let productID = GetValueFromUrlName("id");

    let urlUp = "/Product/UploadFile";
    let urlDeleteAllFileWithType = "/Product/DeleteAllFileWithType";

    try {
        ShowCircleLoader();

        // Upload ảnh/video sản phẩm lên server
        let isOK = await SendFilesPromise(urlUp, urlDeleteAllFileWithType, productID);
        //if (DEBUG) {
        //    console.log("respinseSendFile: " + JSON.stringify(respinseSendFile));
        //}
        if (isOK) {
            alert("Cập nhật sản phẩm thành công.");
        }
    }
    catch (error) {
        RemoveCircleLoader();
        //alert("Cập nhật sản phẩm lỗi.");
        await CreateMustClickOkModal("Cập nhật ảnh video lỗi. " + error.message, null);
        return;
    }

    //// Đợi load ảnh xong
    //while (true) {
    //    await Sleep(1000);

    //    if (isFinishUploadImage == 0 && isFinishUploadVideo == 0) {
    //        alert("Cập nhật sản phẩm thành công.");
    //        break;
    //    }
    //}
    RemoveCircleLoader();
    // Refresh page
    //window.scrollTo(0, 0);
    //await Sleep(1000)
    //window.location.reload();
}

async function UpdateName() {
    let productName = document.getElementById("product-name-id").value;
    if (isEmptyOrSpaces(productName)) {
        CreateMustClickOkModal("Tên sản phẩm trống.", null);
        return false;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("id", GetValueFromUrlName("id"));
    searchParams.append("name", CapitalizeWords(productName));

    let url = "/Product/UpdateName";

    try {
        // Cập nhật vào db
        ShowCircleLoader();
        let responseDB = await RequestHttpGetPromise(searchParams, url);
        RemoveCircleLoader();
        CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật thành công.", "Cập nhật thất bại.");
    }
    catch (error) {
        CreateMustClickOkModal("Cập nhật tên lỗi.", null);
        return;
    }
}

async function UpdateQuantity() {
    let productQuantity = document.getElementById("quantity").value;

    const searchParams = new URLSearchParams();
    searchParams.append("id", GetValueFromUrlName("id"));
    searchParams.append("quantity", productQuantity);

    let url = "/Product/UpdateQuantity";

    try {
        // Cập nhật vào db
        ShowCircleLoader();
        let responseDB = await RequestHttpPostPromise(searchParams, url);
        RemoveCircleLoader();
        CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật thành công.", "Cập nhật thất bại.");
    }
    catch (error) {
        CreateMustClickOkModal("Cập nhật mã lỗi.", null);
        return;
    }
}

async function UpdateCode() {
    let productCode = document.getElementById("code").value;

    const searchParams = new URLSearchParams();
    searchParams.append("id", GetValueFromUrlName("id"));
    searchParams.append("code", productCode);

    let url = "/Product/UpdateCode";

    try {
        // Cập nhật vào db
        ShowCircleLoader();
        let responseDB = await RequestHttpGetPromise(searchParams, url);
        RemoveCircleLoader();
        CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật thành công.", "Cập nhật thất bại.");
    }
    catch (error) {
        CreateMustClickOkModal("Cập nhật mã lỗi.", null);
        return;
    }
}

async function UpdateISBN() {
    let productISBN = document.getElementById("barcode").value;

    const searchParams = new URLSearchParams();
    searchParams.append("id", GetValueFromUrlName("id"));
    searchParams.append("isbn", productISBN);

    let url = "/Product/UpdateISBN";

    try {
        // Cập nhật vào db
        ShowCircleLoader();
        let responseDB = await RequestHttpGetPromise(searchParams, url);
        RemoveCircleLoader();
        CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật thành công.", "Cập nhật thất bại.");
    }
    catch (error) {
        CreateMustClickOkModal("Cập nhật tên lỗi.", null);
        return;
    }
}
async function UpdateDetail() {
    const searchParams = new URLSearchParams();
    searchParams.append("id", GetValueFromUrlName("id"));
    let encodeDetail = encodeURIComponent(document.getElementById("detail").value)
    searchParams.append("detail", encodeDetail);
    let url = "/Product/UpdateDetail";

    try {
        // Cập nhật vào db
        ShowCircleLoader();
        let responseDB = await RequestHttpPostPromise(searchParams, url);
        RemoveCircleLoader();
        CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật thành công.", "Cập nhật thất bại.");
    }
    catch (error) {
        CreateMustClickOkModal("Cập nhật tên lỗi.", null);
        return;
    }
}

async function UpdateBookCoverPrice() {
    let bookCoverPrice = document.getElementById("book-cover-price").value;

    const searchParams = new URLSearchParams();
    searchParams.append("id", GetValueFromUrlName("id"));
    searchParams.append("bookCoverPrice", bookCoverPrice);

    let url = "/Product/UpdateBookCoverPrice";

    try {
        // Cập nhật vào db
        ShowCircleLoader();
        let responseDB = await RequestHttpGetPromise(searchParams, url);
        RemoveCircleLoader();
        CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật thành công.", "Cập nhật thất bại.");
    }
    catch (error) {
        CreateMustClickOkModal("Cập nhật giá bìa lỗi.", null);
        return;
    }
}

async function UpdateDiscountWhenImport() {
    const searchParams = new URLSearchParams();
    searchParams.append("id", GetValueFromUrlName("id"));
    searchParams.append("discount", GetValueInputById("discount-when-import", 0));

    let url = "/Product/UpdateDiscountWhenImport";

    try {
        // Cập nhật vào db
        ShowCircleLoader();
        let responseDB = await RequestHttpGetPromise(searchParams, url);
        RemoveCircleLoader();
        CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật thành công.", "Cập nhật thất bại.");
    }
    catch (error) {
        CreateMustClickOkModal("Cập nhật chiết khấu lỗi.", null);
        return;
    }
}

async function UpdatePositionInWarehouse() {
    let positionInWarehouse = document.getElementById("position-in-warehouse").value;

    const searchParams = new URLSearchParams();
    searchParams.append("id", GetValueFromUrlName("id"));
    searchParams.append("positionInWarehouse", positionInWarehouse);

    let url = "/Product/UpdatePositionInWarehouse";

    try {
        // Cập nhật vào db
        ShowCircleLoader();
        let responseDB = await RequestHttpGetPromise(searchParams, url);
        RemoveCircleLoader();
        CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật thành công.", "Cập nhật thất bại.");
    }
    catch (error) {
        CreateMustClickOkModal("Cập nhật vị trí lưu kho lỗi.", null);
        return;
    }
}

async function UpdateStatusOfProduct_ProductPage() {
    await UpdateStatusOfProduct(GetValueFromUrlName("id"), document.getElementById("product-status").value)
}

async function UpdateComboId() {
    let comboId = GetDataIdFromComboDatalist(document.getElementById("combo-id").value);
    if (comboId == null) {
        //CreateMustClickOkModal("Không lấy được thông tin combo cập nhật.", null);
        //return;
        comboId = -1;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("id", GetValueFromUrlName("id"));
    searchParams.append("comboId", comboId);

    let url = "/Product/UpdateComboId";

    try {
        // Cập nhật vào db
        ShowCircleLoader();
        let responseDB = await RequestHttpGetPromise(searchParams, url);
        RemoveCircleLoader();
        CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật thành công.", "Cập nhật thất bại.");
    }
    catch (error) {
        CreateMustClickOkModal("Cập nhật combo lỗi.", null);
        return;
    }
}

function ComboOfProductPage() {
    let comboId = document.getElementById("combo-id").comboIdValue;
    if (comboId == null || comboId == -1) {
        //if (DEBUG) {
        //    console.log("comboIdValue: " + comboId);
        //}
        CreateMustClickOkModal("Sản phẩm không thuộc combo nào hoặc không lấy được thông tin combo.", null);
        return;
    }

    window.open("/Combo/UpdateDelete?id=" + comboId);
}

async function UpdateCategoryId() {
    let categoryId = GetDataIdFromCategoryDatalist(document.getElementById("category-id").value);
    if (categoryId == null) {
        CreateMustClickOkModal("Không lấy được thông tin thể loại cập nhật.", null);
        return;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("id", GetValueFromUrlName("id"));
    searchParams.append("categoryId", categoryId);

    let url = "/Product/UpdateCategoryId";

    try {
        // Cập nhật vào db
        ShowCircleLoader();
        let responseDB = await RequestHttpGetPromise(searchParams, url);
        RemoveCircleLoader();
        CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật thành công.", "Cập nhật thất bại.");
    }
    catch (error) {
        CreateMustClickOkModal("Cập nhật thể loại lỗi.", null);
        return;
    }
}

async function UpdatePublisherId() {
    let publisherId = GetDataIdFromPublisherDatalist(document.getElementById("publisher-id").value);
    if (publisherId == null) {
        CreateMustClickOkModal("Không lấy được thông tin nhà phát hành cập nhật.", null);
        return;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("id", GetValueFromUrlName("id"));
    searchParams.append("publisherId", publisherId);

    let url = "/Product/UpdatePublisherId";

    try {
        // Cập nhật vào db
        ShowCircleLoader();
        let responseDB = await RequestHttpGetPromise(searchParams, url);
        RemoveCircleLoader();
        CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật thành công.", "Cập nhật thất bại.");
    }
    catch (error) {
        CreateMustClickOkModal("Cập nhật nhà phát hành lỗi.", null);
        return;
    }
}

async function UpdatePublishingCompany() {
    let publishingCompany = document.getElementById("publishing-company-id").value;
    if (!IsValidString(publishingCompany)) {
        CreateMustClickOkModal("Không lấy được thông tin nhà xuất bản cần cập nhật.", null);
        return;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("id", GetValueFromUrlName("id"));
    searchParams.append("publishingCompany", publishingCompany);

    let url = "/Product/UpdatePublishingCompany";

    try {
        // Cập nhật vào db
        ShowCircleLoader();
        let responseDB = await RequestHttpGetPromise(searchParams, url);
        RemoveCircleLoader();
        CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật thành công.", "Cập nhật thất bại.");
    }
    catch (error) {
        CreateMustClickOkModal("Cập nhật lỗi.", null);
        return;
    }
}

async function UpdateLanguage() {
    let language = document.getElementById("book-language-id").value;
    if (!IsValidString(language)) {
        CreateMustClickOkModal("Không lấy được thông tin ngôn ngữ cần cập nhật.", null);
        return;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("id", GetValueFromUrlName("id"));
    searchParams.append("language", language);

    let url = "/Product/UpdateLanguage";

    try {
        // Cập nhật vào db
        ShowCircleLoader();
        let responseDB = await RequestHttpGetPromise(searchParams, url);
        RemoveCircleLoader();
        CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật thành công.", "Cập nhật thất bại.");
    }
    catch (error) {
        CreateMustClickOkModal("Cập nhật lỗi.", null);
        return;
    }
}

async function GetProductFromId(id) {
    const searchParams = new URLSearchParams();
    searchParams.append("id", id);

    let query = "/Product/GetProductFromId";

    return RequestHttpPostPromise(searchParams, query);
}

function GetSomeData() {
    GetListCombo();
    GetListCategory();
    GetListPublisher();
    GetListPublishingCompany();
    SetListPublishingTime();
}
