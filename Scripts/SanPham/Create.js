        // Load datalists khi trang load
    window.addEventListener('DOMContentLoaded', async function () {
        await GetListCombo();
    await GetListCategory();
    await GetListPublisher();
    await GetListPublishingCompany();
    await GetListAuthor();
    await GetListTranslator();
        });

    async function CreateSanPham() {
            const name = document.getElementById('sp-name').value.trim();
    if (!name) {
        alert('Tên sản phẩm không được để trống!');
    return;
            }

    // Lấy ID từ datalist, chuyển -1 → null
    const comboId = GetDataIdFromComboDatalist(document.getElementById('combo-id').value) || -1;
    const categoryId = GetDataIdFromCategoryDatalist(document.getElementById('category-id').value) || -1;
    const publisherId = GetDataIdFromPublisherDatalist(document.getElementById('publisher-id').value) || -1;

    const data = {
        Code: document.getElementById('sp-code').value || null,
    Barcode: document.getElementById('sp-barcode').value || null,
    Name: CapitalizeWords(name),
    ShortName: CapitalizeWords(document.getElementById('sp-short-name').value || null),
    ComboId: comboId !== -1 ? comboId : null,
    CategoryId: categoryId !== -1 ? categoryId : null,
    BookCoverPrice: parseInt(document.getElementById('sp-book-cover-price').value) || 0,
    Author: CapitalizeWords(document.getElementById('author-id').value || null),
    Translator: document.getElementById('translator-id').value || null,
    PublisherId: publisherId !== -1 ? publisherId : null,
    PublishingCompany: document.getElementById('publishing-company-id').value || null,
    PublishingTime: parseIntOrNull(document.getElementById('sp-publishing-time').value),
    ProductLong: parseIntOrDefault(document.getElementById('sp-product-long').value, 0),
    ProductWide: parseIntOrDefault(document.getElementById('sp-product-wide').value, 0),
    ProductHigh: parseIntOrDefault(document.getElementById('sp-product-high').value, 0),
    ProductWeight: parseIntOrDefault(document.getElementById('sp-product-weight').value, 0),
    PositionInWarehouse: document.getElementById('sp-position-warehouse').value || null,
    HardCover: parseIntOrNull(document.getElementById('sp-hard-cover').value),
    MinAge: parseIntOrNull(document.getElementById('sp-min-age').value),
    MaxAge: parseIntOrNull(document.getElementById('sp-max-age').value),
    ParentId: parseIntOrNull(document.getElementById('sp-parent-id').value),
    Republish: parseIntOrNull(document.getElementById('sp-republish').value),
    Detail: document.getElementById('sp-detail').value || null,
    Status: parseInt(document.getElementById('sp-status').value) || 0,
    Quantity: parseInt(document.getElementById('sp-quantity').value) || 0,
    PageNumber: parseIntOrNull(document.getElementById('sp-page-number').value),
    Discount: parseFloatOrDefault(document.getElementById('sp-discount').value),
    SalePrice: parseInt(document.getElementById('sp-sale-price').value) || 0,
    Language: document.getElementById('sp-language').value || null,
    Date: document.getElementById('sp-date').value || null,
    SoldQuantity: parseIntOrNull(document.getElementById('sp-sold-quantity').value),
    URL: document.getElementById('sp-url').value || null,
    SEOKeyword: document.getElementById('sp-seo-keyword').value || null
            };

    try {
        ShowCircleLoader();
    const resultText = await PostJSON('/SanPham/AddNewSanPham', data);
    RemoveCircleLoader();
    CheckStatusResponseAndShowPrompt(resultText, "Thành công", "Thất bại", false);
            } catch (error) {
        CreateMustClickOkModal('Lỗi kết nối: ' + error.message);
            }
        }
