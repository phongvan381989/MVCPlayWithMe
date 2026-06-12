function CopyShopeeProductImageToProduct() {

}

async function ShopeeSaveImageSourceOfItemAndModel() {
    const searchParams = new URLSearchParams();
    let query = "/Dev/ShopeeSaveImageSourceOfItemAndModel";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Lấy ảnh thành công", "Lấy ảnh có lỗi");
}

async function ShopeeGetBrandList() {
    let categoryId = 0;
    try {
        categoryId = BigInt(document.getElementById("shopee_something_input").value);
    } catch (e) {
        categoryId = 0;
    }
    if (categoryId == 0) {
        CreateMustClickOkModal("Id Thể loại không đúng", null);
        document.getElementById("shopee_something_input").focus();
        return;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("categoryId", categoryId);
    let query = "/Dev/ShopeeGetBrandList";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Thành công", "Có lỗi");
}

async function ShopeeGetChannelList() {
    const searchParams = new URLSearchParams();
    let query = "/Dev/ShopeeGetChannelList";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Thành công", "Có lỗi");
}

async function LazadaUpdateQuantityAll() {
    const searchParams = new URLSearchParams();
    let query = "/Dev/LazadaUpdateQuantityAll";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Thành công", "Có lỗi");
}

async function LazadaUpdatePrice_SpecialPriceAll() {
    const searchParams = new URLSearchParams();
    let query = "/Dev/LazadaUpdatePrice_SpecialPriceAll";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Thành công", "Có lỗi");
}

async function LazadaGetCategoryTree() {
    const searchParams = new URLSearchParams();
    let query = "/Dev/LazadaGetCategoryTree";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Thành công", "Có lỗi");
}


async function LazadaGetCategoryAttributes() {
    let categoryId = 0;
    try {
        categoryId = BigInt(document.getElementById("lazada_something_input").value);
    } catch (e) {
        categoryId = 0;
    }
    if (categoryId == 0) {
        CreateMustClickOkModal("Id Thể loại không đúng", null);
        document.getElementById("lazada_something_input").focus();
        return;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("categoryId", categoryId);
    let query = "/Dev/LazadaGetCategoryAttributes";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Thành công", "Có lỗi");
}

async function LazadaGetBrandByPages() {
    const searchParams = new URLSearchParams();
    let query = "/Dev/LazadaGetBrandByPages";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Thành công", "Có lỗi");
}

async function TikiSaveImageSourceOfItemAndModel() {
    const searchParams = new URLSearchParams();
    let query = "/Dev/TikiSaveImageSourceOfItemAndModel";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Lấy ảnh thành công", "Lấy ảnh có lỗi");
}

// TikiTestPullEvent
async function TikiTestPullEvent() {
    const searchParams = new URLSearchParams();
    let query = "/Dev/TikiTestPullEvent";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Thành công", "Có lỗi");
}
async function TikiTestSomething() {
    const searchParams = new URLSearchParams();
    let query = "/Dev/TikiTestSomething";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Thành công", "Có lỗi");
}

//
async function TikiTestSomethingWithParameter() {
    const searchParams = new URLSearchParams();
    let query = "/Dev/TikiTestSomethingWithParameter";
    searchParams.append("str", document.getElementById("tiki_something_input").value)
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Thành công", "Có lỗi");
}

//
async function TikiChangeQuantityWhenSetupOtherWarehouse() {
    const searchParams = new URLSearchParams();
    let query = "/Dev/TikiChangeQuantityWhenSetupOtherWarehouse";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Thành công", "Có lỗi");
}

async function AddWaterMark() {
    //const searchParams = new URLSearchParams();
    //let query = "/Dev/AddWaterMarkAllExistImage";
    //ShowCircleLoader();
    //let responseDB = await RequestHttpPostPromise(searchParams, query);
    //RemoveCircleLoader();
    //let result = JSON.parse(responseDB.responseText);

    //if (result.State != 0) {
    //    await CreateMustClickOkModal(result.Message)
    //    return;
    //}
    //alert("Thêm logo voi bé nhỏ thành công.");
}

async function DeleteDuplicateDataOftbShopeeModel() {
    //const searchParams = new URLSearchParams();
    //let query = "/Dev/DeleteDuplicateDataOftbShopeeModel";
    //ShowCircleLoader();
    //let responseDB = await RequestHttpPostPromise(searchParams, query);
    //RemoveCircleLoader();
    //let result = JSON.parse(responseDB.responseText);

    //if (result.State != 0) {
    //    await CreateMustClickOkModal(result.Message)
    //    return;
    //}
    //alert("Thêm logo voi bé nhỏ thành công.");
}

async function ShopeeGetAuthorizationURL() {
    const searchParams = new URLSearchParams();
    let query = "/Dev/ShopeeGetAuthorizationURL";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();
    let result = JSON.parse(responseDB.responseText);

    if (result.State != 0) {
        CreateMustClickOkModal(result.Message)
        return;
    }
    else {
        document.getElementById("authorization_url").value = result.Message;
    }
}

async function ShopeeSaveLivePartnerKey() {
    let livePartnerKey = document.getElementById("live_partner_key").value;
    if (CheckIsEmptyOrSpacesAndShowResult(livePartnerKey, "key không hợp lệ.")) {
        document.getElementById("live_partner_key").focus();
        return;
    }
    const searchParams = new URLSearchParams();
    searchParams.append("key", livePartnerKey);
    let query = "/Dev/ShopeeSaveLivePartnerKey";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật thành công.", "Có lỗi xảy ra.");
}

async function ShopeeSaveCode() {
    let livePartnerKey = document.getElementById("code").value;
    if (CheckIsEmptyOrSpacesAndShowResult(livePartnerKey, "code không hợp lệ.")) {
        document.getElementById("code").focus();
        return;
    }
    const searchParams = new URLSearchParams();
    searchParams.append("code", livePartnerKey);
    let query = "/Dev/ShopeeSaveCode";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật thành công.", "Có lỗi xảy ra.");
}

async function ShopeeGetTokenShopLevelAfterAuthorization() {
    let text = "Bạn vừa được chủ shop ủy quyền?";
    if (confirm(text) == false)
        return;

    const searchParams = new URLSearchParams();
    let query = "/Dev/ShopeeGetTokenShopLevelAfterAuthorization";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Thành công.", "Có lỗi xảy ra.");
}

function LazadaGenerateAuthorityUrl() {
    let urlAuth = "https://auth.lazada.com/oauth/authorize?response_type=code&force_auth=true&redirect_uri=https://vnexpress.net/&client_id=133247";
    document.getElementById("generate_authority_url_lazada").value = urlAuth;
}

async function LazadaGetAccessTokenFromCodeForFirst() {
    let text = "Bạn vừa được chủ shop ủy quyền?";
    if (confirm(text) == false)
        return;

    let code = document.getElementById("code_after_authority_lazada").value;
    if (CheckIsEmptyOrSpacesAndShowResult(code, "code không hợp lệ.")) {
        document.getElementById("code_after_authority_lazada").focus();
        return;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("code", code);
    let query = "/Dev/LazadaGetAccessTokenFromCodeForFirst";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Thành công.", "Có lỗi xảy ra.");
}

async function LazadaRefreshAccessToken() {

    const searchParams = new URLSearchParams();
    let query = "/Dev/LazadaRefreshAccessToken";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Thành công.", "Có lỗi xảy ra.");
}

async function GenerateSitemap() {
    const searchParams = new URLSearchParams();
    let query = "/Dev/GenerateSitemap";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Sinh sitemap.xml thành công!", "Có lỗi khi sinh sitemap.xml");
}
