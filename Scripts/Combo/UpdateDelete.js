// Load dữ liệu sản phẩm khi trang load
window.onload = async function () {
    await GetSomeData();
};

let combo = null;
document.getElementById("adjxn90snkx").remove();

// Chọn Tất cả
document.getElementById("all-e-ecommonerce-type").checked = true;

function ShowProductTable(list) {
    let length = list.length;
    if (length == 0)
        return;
    let table = document.getElementById("product-table");
    table.style.display = "initial";
    for (let i = 0; i < length; i++) {
        let pro = list[i];
        let row = table.insertRow(-1);
        // Insert new cells (<td> elements)
        let cell1 = row.insertCell(0);
        let cell2 = row.insertCell(1);
        let cell3 = row.insertCell(2);
        let cell4 = row.insertCell(3);
        let cell5 = row.insertCell(4);
        let cell6 = row.insertCell(5);

        // Id
        cell1.innerHTML = pro.id;
        cell1.style.display = "none";

        // STT
        cell2.innerHTML = i + 1;

        // Image
        let img = document.createElement("img");
        if (pro.imageSrc.length > 0) {
            img.setAttribute("src", Get320VersionOfImageSrc(pro.imageSrc[0]));
        } else {
            img.setAttribute("src", srcNoImageThumbnail);
        }
        img.height = thumbnailHeight;
        img.width = thumbnailWidth;
        img.className = "go-to-detail-item";
        img.title = "Xem sản phẩm";
        img.onclick = function () {
            window.open("/Product/UpdateDelete?id=" + pro.id);
        };
        cell3.append(img);

        // Tên
        let pName = document.createElement("p");
        //pName.className = "go-to-detail-product";
        pName.innerHTML = pro.name;

        UpdateProductNameStyle(pName, pro.status, pro.quantity);
        cell4.append(pName);

        // Giá bìa
        cell5.innerHTML = ConvertMoneyToText(pro.bookCoverPrice);

        // Số lượng tồn kho
        cell6.innerHTML = pro.quantity;
    }
}

async function GetCombo() {
    const searchParams = new URLSearchParams();
    searchParams.append("id", GetValueFromUrlName("id"));
    let query = "/Combo/GetCombo";

    let responseDB = null;
    ShowCircleLoader();
    try {
        responseDB = await RequestHttpPostPromise(searchParams, query);
    }
    catch (msgLoi) {
        RemoveCircleLoader();
        await CreateMustClickOkModal(msgLoi, null);
        return;
    }
    RemoveCircleLoader();

    if (responseDB.responseText == "null") {
        ShowDoesntFindId();
        return;
    }
    else {
        combo = JSON.parse(responseDB.responseText);
        document.getElementById("combo-name").value = combo.name;
        document.getElementById("combo-code").value = combo.code;

        ShowProductTable(combo.products);

        if (combo.products.length > 0) {
            SetProductCommonInfoWithCombo(combo.products[0]);
        }
    }
}

async function UpdateCombo() {
    let name = document.getElementById("combo-name").value.trim();
    if (CheckIsEmptyOrSpacesAndShowResult(name, "Tên Combo không hợp lệ.")) {
        document.getElementById("combo-name").focus();
        return;
    }

    let code = document.getElementById("combo-code").value.trim();

    const searchParams = new URLSearchParams();
    searchParams.append("id", GetValueFromUrlName("id"));
    searchParams.append("name", CapitalizeWords(name));
    searchParams.append("code", code);
    let query = "/Combo/UpdateCombo";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Update thành công.", "Có lỗi xảy ra.");
}

async function DeleteCombo() {
    if (combo.products.length > 0) {
        CreateMustClickOkModal("Bạn không thể xóa vì có sản phẩm thuộc combo này.", null);
        return;
    }

    let text = "Nếu còn sản phẩm thuộc combo này bạn sẽ không thể xóa dù thông báo đã xóa thành công. Bạn chắc chắn muốn XÓA?";
    if (confirm(text) == false)
        return;

    const searchParams = new URLSearchParams();
    searchParams.append("id", GetValueFromUrlName("id"));
    let query = "/Combo/DeleteCombo";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Xóa thành công.", "Có lỗi xảy ra.");
}

function MappingOfCombo() {
    window.open("/Combo/MappingOfCombo?id=" + GetValueFromUrlName("id"));
}

async function UpdateCommonInfor() {
    const searchParams = new URLSearchParams();
    searchParams.append("comboId", GetValueFromUrlName("id"));
    if (AddUpdateWithCommonParameters(searchParams) === false) {
        return false;
    }

    let query = "/Product/UpdateCommonInfoWithCombo";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Update thành công.", "Có lỗi xảy ra.");
}

async function UpdateCommonHardCover() {
    const searchParams = new URLSearchParams();
    searchParams.append("comboId", GetValueFromUrlName("id"));
    let hardCover = document.getElementById("hard-cover").value;
    searchParams.append("hardCover", hardCover);

    let query = "/Product/UpdateCommonHardCoverWithCombo";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Update thành công.", "Có lỗi xảy ra.");
}

async function UpdateCommonAge() {
    const searchParams = new URLSearchParams();
    searchParams.append("comboId", GetValueFromUrlName("id"));
    let minAge = GetValueInputById("min-age", -1);
    searchParams.append("minAge", minAge);

    let maxAge = GetValueInputById("max-age", -1);
    searchParams.append("maxAge", maxAge);

    let query = "/Product/UpdateCommonAgeWithCombo";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Update thành công.", "Có lỗi xảy ra.");
}

async function UpdateCommonLanguage() {
    const searchParams = new URLSearchParams();
    searchParams.append("comboId", GetValueFromUrlName("id"));
    searchParams.append("language", document.getElementById("book-language-id").value);

    let query = "/Product/UpdateCommonLanguageWithCombo";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Update thành công.", "Có lỗi xảy ra.");
}

async function UpdateCommonDimension() {
    const searchParams = new URLSearchParams();
    searchParams.append("comboId", GetValueFromUrlName("id"));

    let productLong = GetValueInputById("product-long", 0);
    searchParams.append("productLong", productLong);

    let productWide = GetValueInputById("product-wide", 0);
    searchParams.append("productWide", productWide);

    let productHigh = GetValueInputById("product-high", 0);
    searchParams.append("productHigh", productHigh);

    let productWeight = GetValueInputById("product-weight", 0);
    searchParams.append("productWeight", productWeight);

    let query = "/Product/UpdateCommonDimensionWithCombo";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Update thành công.", "Có lỗi xảy ra.");
}

async function UpdateCommonCategory() {
    const searchParams = new URLSearchParams();
    searchParams.append("comboId", GetValueFromUrlName("id"));

    let categoryName = document.getElementById("category-id").value;
    let categoryId = GetDataIdFromCategoryDatalist(categoryName);
    if (categoryId == null) {
        CreateMustClickOkModal("Thể loại chưa chính xác.");
        document.getElementById("category-id").focus();
        return;
    }
    else {
        searchParams.append("categoryId", categoryId);
    }

    let query = "/Product/UpdateCommonCategoryWithCombo";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Update thành công.", "Có lỗi xảy ra.");
}

async function UpdateCommonPageNumber() {
    const searchParams = new URLSearchParams();
    searchParams.append("comboId", GetValueFromUrlName("id"));

    let pageNumber = GetValueInputById("page-number", 0);
    searchParams.append("pageNumber", pageNumber);
    if (pageNumber == 0) {
        CreateMustClickOkModal("Số trang chưa chính xác.");
        document.getElementById("page-number").focus();
        return;
    }

    let query = "/Product/UpdateCommonPageNumberWithCombo";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Update thành công.", "Có lỗi xảy ra.");
}

async function UpdateCommonPublishingTime() {
    const searchParams = new URLSearchParams();
    searchParams.append("comboId", GetValueFromUrlName("id"));

    let publishingTime = GetValueInputById("publishing-time", 0);
    if (publishingTime == 0) {
        CreateMustClickOkModal("Năm xuất bản chưa chính xác.");
        document.getElementById("publishing-time").focus();
        return;
    }
    searchParams.append("publishingTime", publishingTime);

    let query = "/Product/UpdateCommonPublishingTimeWithCombo";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Update thành công.", "Có lỗi xảy ra.");
}

async function UpdateCommonBookCoverPrice() {
    const searchParams = new URLSearchParams();
    searchParams.append("comboId", GetValueFromUrlName("id"));

    let bookCoverPrice = GetValueInputById("book-cover-price", 0);
    if (bookCoverPrice == 0) {
        CreateMustClickOkModal("Giá bìa chưa chính xác.");
        document.getElementById("book-cover-price").focus();
        return;
    }
    searchParams.append("bookCoverPrice", bookCoverPrice);

    let query = "/Product/UpdateCommonBookCoverPriceWithCombo";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Update thành công.", "Có lỗi xảy ra.");
}

async function CreateProductOfComboOnECommerce() {
    // Lấy id
    let id = GetValueFromUrlName("id");
    const searchParams = new URLSearchParams();
    searchParams.append("comboId", id);
    searchParams.append("eType", GetECommerceType());

    let url = "/Product/CreateProductOfComboOnECommerce";

    try {
        // Cập nhật vào db
        ShowCircleLoader();
        let responseDB = await RequestHttpPostPromise(searchParams, url);
        RemoveCircleLoader();
        CheckStatusResponseAndShowPrompt(responseDB.responseText, "Thành công.", "Thất bại.");
    }
    catch (error) {
        CreateMustClickOkModal("Cập nhật lỗi.", null);
        return;
    }
}
