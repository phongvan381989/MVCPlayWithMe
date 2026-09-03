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
    const resultDiv = document.getElementById('generate-sitemap-result');

    try {
        resultDiv.style.display = 'block';
        resultDiv.style.background = '#fff3cd';
        resultDiv.style.border = '1px solid #ffc107';
        resultDiv.style.color = '#856404';
        resultDiv.innerHTML = '⏳ Đang sinh sitemap.xml...';

        const responseText = await PostJSON('/Dev/GenerateSitemap', {});
        const result = JSON.parse(responseText);

        if (result.State === 0) {
            resultDiv.style.background = '#d4edda';
            resultDiv.style.border = '1px solid #28a745';
            resultDiv.style.color = '#155724';
            resultDiv.innerHTML = '✅ ' + result.Message + '\n\n📍 File: ~/sitemap.xml\n🔗 URL: https://voibenho.com/sitemap.xml';
            alert('✅ ' + result.Message);
        } else {
            resultDiv.style.background = '#f8d7da';
            resultDiv.style.border = '1px solid #dc3545';
            resultDiv.style.color = '#721c24';
            resultDiv.innerHTML = '❌ Lỗi: ' + result.Message;
            alert('❌ Lỗi: ' + result.Message);
        }
    } catch (error) {
        resultDiv.style.display = 'block';
        resultDiv.style.background = '#f8d7da';
        resultDiv.style.border = '1px solid #dc3545';
        resultDiv.style.color = '#721c24';
        resultDiv.innerHTML = '❌ Lỗi kết nối: ' + error.message;
        alert('❌ Lỗi kết nối đến server!');
        console.error(error);
    }
}

/**
 * Populate Slugs cho tbCategories và tbPublishers
 * Migration tool - chạy 1 lần duy nhất
 */
async function PopulateSlugs() {
    const resultDiv = document.getElementById('populate-slugs-result');

    // Confirm
    if (!confirm('⚠️ BẠN CHẮC CHẮN MUỐN POPULATE SLUGS?\n\nĐiều này sẽ UPDATE slug cho TẤT CẢ categories và publishers trong database!\n\nChỉ nên chạy 1 LẦN DUY NHẤT để migrate data.')) {
        return;
    }

    // Show loading
    resultDiv.style.display = 'block';
    resultDiv.style.background = '#d1ecf1';
    resultDiv.style.border = '1px solid #bee5eb';
    resultDiv.style.color = '#0c5460';
    resultDiv.textContent = '⏳ Processing... Please wait...';

    ShowCircleLoader();

    try {
        const response = await fetch('/Dev/PopulateSlugs', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const text = await response.text();
        const result = JSON.parse(text);

        RemoveCircleLoader();

        if (result.State === 0) {
            // Success
            resultDiv.style.background = '#d4edda';
            resultDiv.style.border = '1px solid #c3e6cb';
            resultDiv.style.color = '#155724';
            resultDiv.innerHTML = `<strong>✅ SUCCESS!</strong>

${result.Message}

=== DETAILED LOG ===
${result.myJson.log}

=== SUMMARY ===
Categories updated: ${result.myJson.categoryCount}
Publishers updated: ${result.myJson.publisherCount}

=== NEXT STEPS ===
1. Verify slugs trong DB:
   SELECT Id, Name, Slug FROM tbCategories ORDER BY Name LIMIT 20;
   SELECT Id, Name, Slug FROM tbPublishers ORDER BY Name LIMIT 20;

2. Check duplicates (nên trả về empty!):
   SELECT Slug, COUNT(*) FROM tbCategories GROUP BY Slug HAVING COUNT(*) > 1;
   SELECT Slug, COUNT(*) FROM tbPublishers GROUP BY Slug HAVING COUNT(*) > 1;

3. Create indexes (nếu chưa có):
   CREATE UNIQUE INDEX idx_category_slug ON tbCategories(Slug);
   CREATE UNIQUE INDEX idx_publisher_slug ON tbPublishers(Slug);

4. Test frontend:
   /Home/Search?category=sach-thieu-nhi&publisher=kim-dong
`;

            // Show success modal
            CreateMustClickOkModal('✅ Populate Slugs thành công!\n\nKiểm tra kết quả bên dưới và verify trong DB.', null);

        } else {
            // Error
            resultDiv.style.background = '#f8d7da';
            resultDiv.style.border = '1px solid #f5c6cb';
            resultDiv.style.color = '#721c24';
            resultDiv.textContent = `❌ ERROR!\n\n${result.Message}`;

            CreateMustClickOkModal('❌ Lỗi khi populate slugs!\n\n' + result.Message, null);
        }
    } catch (error) {
        RemoveCircleLoader();

        resultDiv.style.background = '#f8d7da';
        resultDiv.style.border = '1px solid #f5c6cb';
        resultDiv.style.color = '#721c24';
        resultDiv.textContent = `❌ EXCEPTION!\n\n${error.message}`;

        CreateMustClickOkModal('❌ Lỗi kết nối!\n\n' + error.message, null);
    }
}
