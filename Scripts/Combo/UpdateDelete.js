// Load dữ liệu sản phẩm khi trang load
window.onload = async function () {
    await GetCombo();
    await GetSomeData();

    // Setup character counter for detail textarea
    const detailTextarea = document.getElementById('common-detail');
    const charCount = document.getElementById('detail-char-count');
    const warning = document.getElementById('detail-warning');

    if (detailTextarea) {
        detailTextarea.addEventListener('input', function() {
            const length = this.value.length;
            charCount.textContent = length;

            if (length > 0 && length < 100) {
                warning.style.display = 'inline';
                charCount.style.color = '#f44336';
            } else if (length > 5000) {
                warning.style.display = 'inline';
                warning.textContent = '⚠️ Vượt quá 5000 ký tự';
                charCount.style.color = '#f44336';
            } else {
                warning.style.display = 'none';
                charCount.style.color = length >= 100 ? '#4caf50' : '#666';
            }
        });
    }

    // Setup character counter for combo description textarea
    const comboDescTextarea = document.getElementById('combo-description');
    const comboDescCharCount = document.getElementById('combo-desc-char-count');

    if (comboDescTextarea) {
        comboDescTextarea.addEventListener('input', function() {
            const length = this.value.length;
            comboDescCharCount.textContent = length;
        });
    }
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
        document.getElementById("combo-status").value = combo.status;
        document.getElementById("combo-description").value = combo.detail || '';

        // Trigger character count update
        const comboDescCharCount = document.getElementById('combo-desc-char-count');
        if (comboDescCharCount) {
            comboDescCharCount.textContent = (combo.detail || '').length;
        }

        ShowProductTable(combo.products);

        if (combo.products.length > 0) {
            SetProductCommonInfoWithCombo(combo.products[0]);
        }

        // Load ảnh combo
        await LoadComboImages(combo.id);
    }
}

async function UpdateCombo() {
    let name = document.getElementById("combo-name").value.trim();
    if (CheckIsEmptyOrSpacesAndShowResult(name, "Tên Combo không hợp lệ.")) {
        document.getElementById("combo-name").focus();
        return;
    }

    let code = document.getElementById("combo-code").value.trim();
    let status = document.getElementById("combo-status").value;
    let description = document.getElementById("combo-description").value.trim();

    const searchParams = new URLSearchParams();
    searchParams.append("id", GetValueFromUrlName("id"));
    searchParams.append("name", CapitalizeWords(name));
    searchParams.append("code", code);
    searchParams.append("status", status);
    searchParams.append("description", description);
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

async function UpdateCommonTranslator() {
    const searchParams = new URLSearchParams();
    searchParams.append("comboId", GetValueFromUrlName("id"));

    let translator = document.getElementById("translator-id").value.trim();
    searchParams.append("translator", translator);

    let query = "/Product/UpdateCommonTranslatorWithCombo";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Update thành công.", "Có lỗi xảy ra.");
}

async function UpdateCommonDiscount() {
    const searchParams = new URLSearchParams();
    searchParams.append("comboId", GetValueFromUrlName("id"));

    let discount = GetValueInputById("discount-when-import", 0);
    if (discount < 0 || discount > 100) {
        CreateMustClickOkModal("Chiết khấu phải từ 0-100%.");
        document.getElementById("discount-when-import").focus();
        return;
    }
    searchParams.append("discount", discount);

    let query = "/Product/UpdateCommonDiscountWithCombo";

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

async function UpdateCommonDetail() {
    const detail = document.getElementById("common-detail").value.trim();

    // Validation - chỉ check nếu có nhập
    if (detail.length > 0) {
        if (detail.length < 100) {
            CreateMustClickOkModal("⚠️ Mô tả quá ngắn. Tối thiểu 100 ký tự.\n\nHiện tại: " + detail.length + " ký tự.");
            document.getElementById("common-detail").focus();
            return;
        }

        if (detail.length > 5000) {
            CreateMustClickOkModal("⚠️ Mô tả quá dài. Tối đa 5000 ký tự.\n\nHiện tại: " + detail.length + " ký tự.");
            document.getElementById("common-detail").focus();
            return;
        }
    }

    // Confirm - message khác nhau cho trống vs có nội dung
    let confirmMessage = '';
    if (detail === '') {
        confirmMessage = `⚠️ Xóa mô tả của ${combo.products.length} sản phẩm trong combo?\n\n` +
                        `Tất cả sản phẩm sẽ có mô tả trống.`;
    } else {
        confirmMessage = `Xác nhận cập nhật mô tả chung cho ${combo.products.length} sản phẩm trong combo?\n\n` +
                        `Mô tả (${detail.length} ký tự):\n"${detail.substring(0, 100)}${detail.length > 100 ? '...' : ''}"`;
    }

    const confirmed = await new Promise(resolve => {
        CreateMustClickOkModal(confirmMessage, () => resolve(true));
    });

    if (!confirmed) return;

    const searchParams = new URLSearchParams();
    searchParams.append("comboId", GetValueFromUrlName("id"));
    searchParams.append("detail", detail);

    let query = "/Product/UpdateCommonDetailWithCombo";

    try {
        ShowCircleLoader();
        let responseDB = await RequestHttpPostPromise(searchParams, query);
        RemoveCircleLoader();
        CheckStatusResponseAndShowPrompt(responseDB.responseText, "✅ Cập nhật mô tả thành công!", "❌ Có lỗi xảy ra.");
    } catch (error) {
        RemoveCircleLoader();
        CreateMustClickOkModal("❌ Lỗi khi cập nhật: " + error.message);
    }
}

// ==================== VALIDATION CHECKER FUNCTIONS ====================

function ShowValidationResult(title, missingProducts, fieldName) {
    const resultDiv = document.getElementById('validation-result');
    const titleEl = document.getElementById('validation-title');
    const summaryEl = document.getElementById('validation-summary');
    const listEl = document.getElementById('validation-list');

    if (!combo || !combo.products || combo.products.length === 0) {
        CreateMustClickOkModal('⚠️ Chưa có sản phẩm trong combo.');
        return;
    }

    titleEl.textContent = title;

    if (missingProducts.length === 0) {
        summaryEl.innerHTML = `<span style="color: #4caf50; font-weight: 600;">✅ Tất cả ${combo.products.length} sản phẩm đều đã có ${fieldName}!</span>`;
        listEl.innerHTML = '';
    } else {
        summaryEl.innerHTML = `<span style="color: #ff6f00; font-weight: 600;">⚠️ Có ${missingProducts.length}/${combo.products.length} sản phẩm chưa có ${fieldName}:</span>`;

        let html = '<ul style="margin: 0; padding-left: 20px; list-style: none;">';
        missingProducts.forEach((product, index) => {
            html += `
                <li class="missing-product-item" onclick="window.open('/Product/UpdateDelete?id=${product.id}')">
                    <span style="font-weight: 600; color: #ff6f00;">${index + 1}.</span>
                    <span style="flex: 1;">${product.name}</span>
                    <span style="color: #999; font-size: 12px;">ID: ${product.id}</span>
                </li>
            `;
        });
        html += '</ul>';
        listEl.innerHTML = html;
    }

    resultDiv.style.display = 'block';
    resultDiv.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
}

function CheckMissingBookCoverPrice() {
    const missing = combo.products.filter(p => !p.bookCoverPrice || p.bookCoverPrice === 0);
    ShowValidationResult('🔍 Kiểm Tra Giá Bìa', missing, 'giá bìa');
}

function CheckMissingHardCover() {
    // hardCover có thể là 0 (bìa mềm) hoặc 1 (bìa cứng), coi như thiếu nếu undefined/null
    const missing = combo.products.filter(p => p.hardCover === undefined || p.hardCover === null);
    ShowValidationResult('🔍 Kiểm Tra Loại Bìa', missing, 'thông tin bìa');
}

function CheckMissingCategory() {
    const missing = combo.products.filter(p => !p.categoryId || p.categoryId === -1);
    ShowValidationResult('🔍 Kiểm Tra Thể Loại', missing, 'thể loại');
}

function CheckMissingAuthor() {
    const missing = combo.products.filter(p => !p.author || p.author.trim() === '');
    ShowValidationResult('🔍 Kiểm Tra Tác Giả', missing, 'tác giả');
}

function CheckMissingPublisher() {
    const missing = combo.products.filter(p => !p.publisherId || p.publisherId === -1);
    ShowValidationResult('🔍 Kiểm Tra Nhà Phát Hành', missing, 'nhà phát hành');
}

function CheckMissingPublishingCompany() {
    const missing = combo.products.filter(p => !p.publishingCompany || p.publishingCompany.trim() === '');
    ShowValidationResult('🔍 Kiểm Tra Nhà Xuất Bản', missing, 'nhà xuất bản');
}

function CheckMissingPublishingTime() {
    const missing = combo.products.filter(p => !p.publishingTime || p.publishingTime === 0);
    ShowValidationResult('🔍 Kiểm Tra Năm Xuất Bản', missing, 'năm xuất bản');
}

function CheckMissingLanguage() {
    const missing = combo.products.filter(p => !p.language || p.language.trim() === '');
    ShowValidationResult('🔍 Kiểm Tra Ngôn Ngữ', missing, 'ngôn ngữ');
}

function CheckMissingPageNumber() {
    const missing = combo.products.filter(p => !p.pageNumber || p.pageNumber === 0);
    ShowValidationResult('🔍 Kiểm Tra Số Trang', missing, 'số trang');
}

function CheckMissingDimension() {
    const missing = combo.products.filter(p =>
        !p.productLong || p.productLong === 0 ||
        !p.productWide || p.productWide === 0 ||
        !p.productHigh || p.productHigh === 0 ||
        !p.productWeight || p.productWeight === 0
    );
    ShowValidationResult('🔍 Kiểm Tra Kích Thước', missing, 'kích thước (dài/rộng/cao/nặng)');
}

function CheckMissingAge() {
    const missing = combo.products.filter(p =>
        !p.minAge ||
        !p.maxAge
    );
    ShowValidationResult('🔍 Kiểm Tra Độ Tuổi', missing, 'độ tuổi');
}

function CheckMissingDetail() {
    const missing = combo.products.filter(p =>
        !p.detail || p.detail.trim() === '' || p.detail.trim().length < 100
    );
    ShowValidationResult('🔍 Kiểm Tra Mô Tả', missing, 'mô tả (hoặc mô tả quá ngắn < 100 ký tự)');
}

function CheckMissingImages() {
    const missing = combo.products.filter(p => !p.imageSrc || p.imageSrc.length <= 3);
    ShowValidationResult('🔍 Kiểm Tra Hình Ảnh', missing, 'hình ảnh');
}

function CheckAll() {
    if (!combo || !combo.products || combo.products.length === 0) {
        CreateMustClickOkModal('⚠️ Chưa có sản phẩm trong combo.');
        return;
    }

    const checks = [
        { name: 'Giá bìa', count: combo.products.filter(p => !p.bookCoverPrice || p.bookCoverPrice === 0).length },
        { name: 'Loại bìa', count: combo.products.filter(p => p.hardCover === undefined || p.hardCover === null).length },
        { name: 'Thể loại', count: combo.products.filter(p => !p.categoryId || p.categoryId === -1).length },
        { name: 'Tác giả', count: combo.products.filter(p => !p.author || p.author.trim() === '').length },
        { name: 'Nhà phát hành', count: combo.products.filter(p => !p.publisherId || p.publisherId === -1).length },
        { name: 'Nhà xuất bản', count: combo.products.filter(p => !p.publishingCompany || p.publishingCompany.trim() === '').length },
        { name: 'Năm xuất bản', count: combo.products.filter(p => !p.publishingTime || p.publishingTime === 0).length },
        { name: 'Ngôn ngữ', count: combo.products.filter(p => !p.language || p.language.trim() === '').length },
        { name: 'Số trang', count: combo.products.filter(p => !p.pageNumber || p.pageNumber === 0).length },
        { name: 'Kích thước', count: combo.products.filter(p => !p.productLong || p.productLong === 0 || !p.productWide || p.productWide === 0 || !p.productHigh || p.productHigh === 0 || !p.productWeight || p.productWeight === 0).length },
        { name: 'Độ tuổi', count: combo.products.filter(p => !p.minAge || !p.maxAge).length },
        { name: 'Mô tả', count: combo.products.filter(p => !p.detail || p.detail.trim() === '' || p.detail.trim().length < 100).length },
        { name: 'Hình ảnh', count: combo.products.filter(p => !p.imageSrc || p.imageSrc.length === 0).length }
    ];

    const resultDiv = document.getElementById('validation-result');
    const titleEl = document.getElementById('validation-title');
    const summaryEl = document.getElementById('validation-summary');
    const listEl = document.getElementById('validation-list');

    titleEl.textContent = '⚡ Báo Cáo Tổng Hợp Kiểm Tra';

    const totalMissing = checks.reduce((sum, check) => sum + check.count, 0);

    if (totalMissing === 0) {
        summaryEl.innerHTML = `<span style="color: #4caf50; font-weight: 600; font-size: 16px;">✅ Hoàn hảo! Tất cả ${combo.products.length} sản phẩm đều đã có đầy đủ thông tin!</span>`;
        listEl.innerHTML = '';
    } else {
        summaryEl.innerHTML = `<span style="color: #ff6f00; font-weight: 600;">⚠️ Tổng số: ${combo.products.length} sản phẩm - Phát hiện thiếu thông tin:</span>`;

        let html = '<div style="display: grid; grid-template-columns: repeat(auto-fill, minmax(250px, 1fr)); gap: 12px; margin-top: 12px;">';
        checks.forEach(check => {
            const percentage = Math.round((check.count / combo.products.length) * 100);
            const color = check.count === 0 ? '#4caf50' : check.count < combo.products.length / 2 ? '#ff9800' : '#f44336';
            const icon = check.count === 0 ? '✅' : '⚠️';

            html += `
                <div style="padding: 12px; background: white; border-left: 4px solid ${color}; border-radius: 4px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);">
                    <div style="font-weight: 600; color: ${color}; margin-bottom: 4px;">
                        ${icon} ${check.name}
                    </div>
                    <div style="font-size: 13px; color: #666;">
                        Thiếu: <strong>${check.count}</strong>/${combo.products.length} (${percentage}%)
                    </div>
                </div>
            `;
        });
        html += '</div>';
        listEl.innerHTML = html;
    }

    resultDiv.style.display = 'block';
    resultDiv.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
}

// Upload ảnh combo
async function UploadComboImages() {
    const fileInput = document.getElementById('combo-image-upload');
    const files = fileInput.files;
    
    if (!files || files.length === 0) {
        CreateMustClickOkModal('⚠️ Chưa chọn ảnh nào!', null);
        return;
    }
    
    const comboId = GetValueFromUrlName("id");
    if (!comboId || comboId <= 0) {
        CreateMustClickOkModal('⚠️ Combo ID không hợp lệ!', null);
        return;
    }
    
    // Upload từng file
    let successCount = 0;
    let errorCount = 0;
    
    ShowCircleLoader();
    
    for (let i = 0; i < files.length; i++) {
        const formData = new FormData();
        formData.append('file', files[i]);
        formData.append('comboId', comboId);
        
        try {
            const response = await fetch('/Combo/UploadComboImage', {
                method: 'POST',
                body: formData
            });
            
            const result = await response.text();
            if (result.includes('thành công') || result.includes('Ok')) {
                successCount++;
            } else {
                errorCount++;
            }
        } catch (error) {
            errorCount++;
        }
    }
    
    RemoveCircleLoader();
    
    // Clear input
    fileInput.value = '';
    
    // Reload images
    await LoadComboImages(comboId);
    
    CreateMustClickOkModal(`✅ Upload thành công: ${successCount} ảnh\n❌ Lỗi: ${errorCount} ảnh`, null);
}

// Load và hiển thị ảnh combo
async function LoadComboImages(comboId) {
    try {
        const response = await fetch(`/Combo/GetComboImages?comboId=${comboId}`);
        const images = await response.json();
        
        const previewDiv = document.getElementById('combo-images-preview');
        previewDiv.innerHTML = '';
        
        if (!images || images.length === 0) {
            previewDiv.innerHTML = '<p style="color: #999;">Chưa có ảnh nào</p>';
            return;
        }
        
        images.forEach((imgSrc, index) => {
            const imgContainer = document.createElement('div');
            imgContainer.style.cssText = 'position: relative; display: inline-block;';
            
            const img = document.createElement('img');
            img.src = imgSrc;
            img.style.cssText = 'width: 100px; height: 100px; object-fit: cover; border: 2px solid #ddd; border-radius: 4px;';
            img.title = `Ảnh ${index + 1}`;
            
            imgContainer.appendChild(img);
            previewDiv.appendChild(imgContainer);
        });
    } catch (error) {
        console.error('Load combo images error:', error);
    }
}
