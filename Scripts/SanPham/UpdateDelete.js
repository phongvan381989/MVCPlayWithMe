// Mapping column name → element ID
const fieldMapping = {
    'Code': 'sp-code',
    'Barcode': 'sp-barcode',
    'Name': 'sp-name',
    'ShortName': 'sp-short-name',
    'ComboId': 'combo-id',
    'CategoryId': 'category-id',
    'BookCoverPrice': 'sp-book-cover-price',
    'Author': 'author-id',
    'Translator': 'translator-id',
    'PublisherId': 'publisher-id',
    'PublishingCompany': 'publishing-company-id',
    'PublishingTime': 'sp-publishing-time',
    'ProductLong': 'sp-product-long',
    'ProductWide': 'sp-product-wide',
    'ProductHigh': 'sp-product-high',
    'ProductWeight': 'sp-product-weight',
    'PositionInWarehouse': 'sp-position-warehouse',
    'HardCover': 'sp-hard-cover',
    'MinAge': 'sp-min-age',
    'MaxAge': 'sp-max-age',
    'ParentId': 'sp-parent-id',
    'Republish': 'sp-republish',
    'Detail': 'sp-detail',
    'Status': 'sp-status',
    'Quantity': 'sp-quantity',
    'PageNumber': 'sp-page-number',
    'Discount': 'sp-discount',
    'SalePrice': 'sp-sale-price',
    'Language': 'sp-language',
    'Date': 'sp-date',
    'SoldQuantity': 'sp-sold-quantity',
    'URL': 'sp-url',
    'SEOKeyword': 'sp-seo-keyword'
};

// Load dữ liệu sản phẩm khi trang load
window.onload = async function () {
    // Thêm nút "Cập nhật" vào tất cả các input chưa có
    AddUpdateButtonsToAllFields();

    await GetListCombo();
    await GetListCategory();
    await GetListPublisher();
    await GetListPublishingCompany();
    await GetListProductName();
    await GetListAuthor();
    await GetListTranslator();
    await LoadSanPhamData();
    await LoadMediaList();
    document.getElementById("list-combo-2").innerHTML = document.getElementById("list-combo").innerHTML;
};

// Tự động thêm nút "Cập nhật" vào tất cả input/select/textarea
function AddUpdateButtonsToAllFields() {
    const section = document.querySelector('.section');
    const allInputs = section.querySelectorAll('input:not([type="hidden"]), select, textarea');

    allInputs.forEach(element => {
        // Bỏ qua nếu đã có wrapper rồi
        if (element.parentElement.classList.contains('field-wrapper')) {
            return;
        }

        const elementId = element.id;
        if (!elementId) return;

        // Tìm fieldName từ mapping
        let fieldName = null;
        for (const [key, value] of Object.entries(fieldMapping)) {
            if (value === elementId) {
                fieldName = key;
                break;
            }
        }

        if (!fieldName) return;

        // Tạo wrapper
        const wrapper = document.createElement('div');
        wrapper.className = 'field-wrapper';

        // Tạo nút
        const button = document.createElement('button');
        button.type = 'button';
        button.className = 'update-btn';
        button.textContent = 'Cập nhật';
        button.onclick = () => UpdateField(fieldName, elementId);

        // Thay thế: input -> wrapper > input + button
        element.parentNode.insertBefore(wrapper, element);
        wrapper.appendChild(element);
        wrapper.appendChild(button);
    });
}

// Hàm cập nhật 1 field riêng lẻ
async function UpdateField(fieldName, elementId) {
    const element = document.getElementById(elementId);
    let fieldValue = element.value;

    // Xử lý đặc biệt cho datalist (lấy ID thay vì name)
    if (fieldName === 'ComboId') {
        fieldValue = GetDataIdFromComboDatalist(fieldValue) || '-1';
    } else if (fieldName === 'CategoryId') {
        fieldValue = GetDataIdFromCategoryDatalist(fieldValue) || '-1';
    } else if (fieldName === 'PublisherId') {
        fieldValue = GetDataIdFromPublisherDatalist(fieldValue) || '-1';
    }

    // Viết hoa chữ cái đầu
    if (fieldName === 'Name' || fieldName === 'ShortName' || fieldName === 'Author') {
        fieldValue = CapitalizeWords(fieldValue);
    }

    try {
        ShowCircleLoader();
        const resultText = await PostJSON('/SanPham/UpdateSanPhamField', {
            sanPhamId: sanPhamId,
            fieldName: fieldName,
            fieldValue: fieldValue
        });
        RemoveCircleLoader();

        CheckStatusResponseAndShowPrompt(resultText, "Thành công", "Thất bại", false)
    } catch (error) {
        RemoveCircleLoader();
        CreateMustClickOkModal('❌ Lỗi kết nối: ' + error.message);
    }
}

// Helper: Lấy name từ ID trong datalist
function GetNameFromDatalistById(datalistId, id) {
    const datalist = document.getElementById(datalistId);
    if (!datalist || !id || id === -1) return '';

    const options = datalist.querySelectorAll('option');
    for (let option of options) {
        if (parseInt(option.getAttribute('data-id')) === id) {
            return option.value;
        }
    }
    return '';
}

async function LoadSanPhamData() {
    try {
        const resultText = await PostJSON('/SanPham/GetSanPhamFromId', { id: sanPhamId });
        const sanPham = JSON.parse(resultText);

        if (sanPham) {
            document.getElementById('sp-code').value = sanPham.Code || '';
            document.getElementById('sp-barcode').value = sanPham.Barcode || '';
            document.getElementById('sp-name').value = sanPham.Name || '';
            document.getElementById('sp-short-name').value = sanPham.ShortName || '';

            // Set datalist values từ ID
            document.getElementById('combo-id').value = GetNameFromDatalistById('list-combo', sanPham.ComboId);
            document.getElementById('category-id').value = GetNameFromDatalistById('list-category', sanPham.CategoryId);

            document.getElementById('sp-book-cover-price').value = sanPham.BookCoverPrice || 0;
            document.getElementById('author-id').value = sanPham.Author || '';
            document.getElementById('translator-id').value = sanPham.Translator || '';

            // Set publisher datalist value từ ID
            document.getElementById('publisher-id').value = GetNameFromDatalistById('list-Publisher', sanPham.PublisherId);
            document.getElementById('publishing-company-id').value = sanPham.PublishingCompany || '';
            document.getElementById('sp-publishing-time').value = (sanPham.PublishingTime && sanPham.PublishingTime !== -1) ? sanPham.PublishingTime : '';
            document.getElementById('sp-product-long').value = sanPham.ProductLong || '';
            document.getElementById('sp-product-wide').value = sanPham.ProductWide || '';
            document.getElementById('sp-product-high').value = sanPham.ProductHigh || '';
            document.getElementById('sp-product-weight').value = sanPham.ProductWeight || '';
            document.getElementById('sp-position-warehouse').value = sanPham.PositionInWarehouse || '';
            document.getElementById('sp-hard-cover').value = (sanPham.HardCover !== null && sanPham.HardCover !== -1) ? sanPham.HardCover : '';
            document.getElementById('sp-min-age').value = (sanPham.MinAge !== null && sanPham.MinAge !== -1) ? sanPham.MinAge : '';
            document.getElementById('sp-max-age').value = (sanPham.MaxAge !== null && sanPham.MaxAge !== -1) ? sanPham.MaxAge : '';
            document.getElementById('sp-parent-id').value = (sanPham.ParentId && sanPham.ParentId !== -1) ? sanPham.ParentId : '';
            document.getElementById('sp-republish').value = (sanPham.Republish && sanPham.Republish !== -1) ? sanPham.Republish : '';
            document.getElementById('sp-detail').value = sanPham.Detail || '';
            document.getElementById('sp-status').value = sanPham.Status ?? 0;
            document.getElementById('sp-quantity').value = sanPham.Quantity ?? 0;
            document.getElementById('sp-page-number').value = (sanPham.PageNumber && sanPham.PageNumber !== -1) ? sanPham.PageNumber : '';
            document.getElementById('sp-discount').value = sanPham.Discount || '';
            document.getElementById('sp-sale-price').value = sanPham.SalePrice || 0;
            document.getElementById('sp-language').value = sanPham.Language || '';
            if (sanPham.Date) {
                const date = new Date(sanPham.Date);
                document.getElementById('sp-date').value = date.toISOString().split('T')[0];
            }
            document.getElementById('sp-sold-quantity').value = (sanPham.SoldQuantity && sanPham.SoldQuantity !== -1) ? sanPham.SoldQuantity : '';
            document.getElementById('sp-url').value = sanPham.URL || '';
            document.getElementById('sp-seo-keyword').value = sanPham.SEOKeyword || '';
        }
    } catch (error) {
        alert('Lỗi load dữ liệu: ' + error.message);
    }
}

async function UpdateSanPham() {
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
        Id: sanPhamId,
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
        const resultText = await PostJSON('/SanPham/UpdateSanPham', data);
        RemoveCircleLoader();
        CheckStatusResponseAndShowPrompt(resultText, "Thành công", "Thất bại", false);
    } catch (error) {
        CreateMustClickOkModal('Lỗi kết nối: ' + error.message);
    }
}

async function DeleteSanPham() {
    if (!confirm('Bạn có chắc chắn muốn xóa sản phẩm này? Thao tác này không thể hoàn tác!')) {
        return;
    }

    try {
        const resultText = await PostJSON('/SanPham/DeleteSanPham', { id: sanPhamId });
        const result = JSON.parse(resultText);

        if (result.State === 0) {
            //alert(result.Message);
            window.location.href = '/SanPham/Create';
        } else {
            alert('Lỗi: ' + result.Message);
        }
    } catch (error) {
        alert('Lỗi kết nối: ' + error.message);
    }
}

async function LoadMediaList() {
    try {
        // Load metadata từ database (single source of truth)
        const metadataResultText = await PostJSON('/SanPham/GetAllMediaMetadata', { sanPhamId: sanPhamId });
        const metadataList = JSON.parse(metadataResultText);

        const container = document.getElementById('media-container');
        container.innerHTML = '';

        if (metadataList && metadataList.length > 0) {
            metadataList.forEach((metadata, index) => {
                const isImage = metadata.MediaType === 'image';

                // Tự sinh URL phiên bản 320 từ FileName
                const filePath = Get320VersionOfImageSrc(GetSanPhamMediaUrl(sanPhamId, metadata.FileName));

                const mediaItem = document.createElement('div');
                mediaItem.className = 'media-item';
                mediaItem.dataset.fileName = metadata.FileName;
                mediaItem.dataset.mediaId = metadata.Id;

                // Media element (ảnh hoặc video)
                let mediaElement;
                if (isImage) {
                    // WebP path (ảnh đã được convert sang WebP)
                    mediaElement = `
                            <picture>
                                <source srcset="${filePath}" type="image/webp">
                                <img src="${filePath}" alt="${metadata.AltText || metadata.FileName}" />
                            </picture>
                        `;
                } else {
                    mediaElement = `<video src="${GetSanPhamMediaUrl(sanPhamId, metadata.FileName)}" controls></video>`;
                }

                mediaItem.innerHTML = `
                        ${mediaElement}
                        <input type="text" value="${metadata.FileName}" readonly placeholder="Tên file"
                                onclick="RenameMediaFile('${metadata.FileName}')"
                                style="background: #eee; cursor: pointer;"
                                title="Click để đổi tên file" />
                        <div style="font-size: 12px; color: #666; margin: 5px 0; padding: 5px; background: #f5f5f5; border-radius: 3px;">
                            📐 Kích thước: <strong>${metadata.Width || 0} × ${metadata.Height || 0} px</strong>
                            ${(metadata.Width === 0 || metadata.Height === 0) ? ' <span style="color: #ff9800;">(chưa có - upload lại để lấy kích thước)</span>' : ''}
                        </div>
                        ${isImage ? `
                            <textarea spellcheck="false" class="media-alt" placeholder="Alt text (SEO - bắt buộc cho ảnh)" rows="2"
                                    title="Alt text (SEO - bắt buộc cho ảnh):nội dung ngắn gọn, có giá trị, có keyword + tác giả nếu tác giả nổi tiếng, giới hạn 125 ký tự VD: Sách eho moi moi trang bìa">${metadata.AltText || ''}</textarea>
                            <textarea spellcheck="false" class="media-title" placeholder="Caption (tùy chọn)" rows="2"
                                    title="Text hiển thị dưới ảnh (nếu cần)">${metadata.Title || ''}</textarea>
                        ` : `
                            <textarea spellcheck="false" class="media-title" placeholder="Title video (bắt buộc)" rows="2"
                                    title="Tiêu đề video. VD: Hướng dẫn sử dụng sách Toán">${metadata.Title || ''}</textarea>
                            <textarea spellcheck="false" class="media-description" placeholder="Mô tả video (tùy chọn)" rows="2"
                                        title="Mô tả chi tiết nội dung video">${metadata.Description || ''}</textarea>
                        `}
                        <input type="number" class="media-order" placeholder="Thứ tự" title="Số thứ tự ảnh"
                                value="${metadata.DisplayOrder ?? index}"
                                style="width: 80px;" min="0" />
                        <div style="margin-top: 5px;">
                            <button onclick="SaveMediaMetadata('${metadata.FileName}')"
                                    style="background-color: #4CAF50;">💾 Lưu metadata</button>
                            <button onclick="RenameMediaFile('${metadata.FileName}')"
                                    style="background-color: #2196F3;">Đổi tên</button>
                            ${isImage ? `
                                <button onclick="PinImageToDetail('${metadata.FileName}')"
                                        style="background-color: #FF9800;"
                                        title="Chèn {{image:${metadata.FileName}}} vào vị trí con trỏ">
                                    Ghim ảnh
                                </button>
                            ` : ''}
                            <button onclick="ViewBigImage('${GetSanPhamMediaUrl(sanPhamId, metadata.FileName)}')"
                                    style="background-color: #f136f4;">Xem ảnh lớn</button>
                            <button onclick="DeleteMediaFile('${metadata.FileName}')"
                                    style="background-color: #f44336;">Xóa</button>
                        </div>
                    `;

                container.appendChild(mediaItem);
            });
        } else {
            container.innerHTML = '<p>Chưa có media nào.</p>';
        }
    } catch (error) {
        console.error('Lỗi load media:', error);
        alert('Lỗi load media: ' + error.message);
    }
}

// Ghim ảnh vào chi tiết (insert {{ image: filename }} tại vị trí con trỏ)
function PinImageToDetail(fileName) {
    const detailTextarea = document.getElementById('sp-detail');
    const imageTag = `{{ image: ${fileName}}}`;

    // Lấy vị trí con trỏ hiện tại
    const cursorPos = detailTextarea.selectionStart;
    const textBefore = detailTextarea.value.substring(0, cursorPos);
    const textAfter = detailTextarea.value.substring(cursorPos);

    // Thêm newlines nếu cần để imageTag ở dòng riêng
    let prefix = '';
    let suffix = '';

    // Nếu có text trước và không kết thúc bằng newline → thêm newline
    if (textBefore.length > 0 && !textBefore.endsWith('\n')) {
        prefix = '\n';
    }

    // Nếu có text sau và không bắt đầu bằng newline → thêm newline
    if (textAfter.length > 0 && !textAfter.startsWith('\n')) {
        suffix = '\n';
    }

    // Insert imageTag với newlines
    const insertText = prefix + imageTag + suffix;
    detailTextarea.value = textBefore + insertText + textAfter;

    // Đặt con trỏ ngay sau imageTag (trước suffix newline nếu có)
    const newCursorPos = cursorPos + prefix.length + imageTag.length;
    detailTextarea.setSelectionRange(newCursorPos, newCursorPos);

    // Focus lại textarea
    detailTextarea.focus();

    // Flash effect để user biết đã thêm
    detailTextarea.style.backgroundColor = '#ffffcc';
    setTimeout(() => {
        detailTextarea.style.backgroundColor = '';
    }, 300);
}

// Lưu metadata (Alt text, Title, Description, DisplayOrder) vào database
async function SaveMediaMetadata(fileName) {
    try {
        // Tìm media item có fileName này
        const mediaItem = document.querySelector(`.media-item[data-file-name="${fileName}"]`);
        if (!mediaItem) {
            alert('Không tìm thấy media item!');
            return;
        }

        // Lấy giá trị từ input fields
        const altText = mediaItem.querySelector('.media-alt')?.value.trim() || null;
        const title = mediaItem.querySelector('.media-title')?.value.trim() || null;
        const description = mediaItem.querySelector('.media-description')?.value.trim() || null;
        const displayOrder = parseInt(mediaItem.querySelector('.media-order')?.value) || 0;

        // Xác định MediaType
        const isImage = mediaItem.querySelector('img') !== null;
        const mediaType = isImage ? 'image' : 'video';

        // Kiểm tra required fields
        if (isImage && !altText) {
            alert('Alt text là bắt buộc cho ảnh!');
            return;
        }
        if (!isImage && !title) {
            alert('Title là bắt buộc cho video!');
            return;
        }

        // Tạo data object
        const mediaId = mediaItem.dataset.mediaId;
        const data = {
            Id: mediaId ? parseInt(mediaId) : 0,
            SanPhamId: sanPhamId,
            MediaType: mediaType,
            FileName: fileName,
            Title: title,
            AltText: altText,
            Description: description,
            PosterImage: null, // TODO: implement nếu cần
            DisplayOrder: displayOrder
        };

        // Gọi API Insert hoặc Update
        const endpoint = mediaId ? '/SanPham/UpdateMediaMetadata' : '/SanPham/AddMediaMetadata';
        ShowCircleLoader();
        const resultText = await PostJSON(endpoint, data);
        RemoveCircleLoader();

        CheckStatusResponseAndShowPrompt(resultText, "Thành công", "Thất bại", false);
    } catch (error) {
        RemoveCircleLoader();
        CreateMustClickOkModal('Lỗi kết nối: ' + error.message, null);
    }
}

// Helper: Check xem file đã tồn tại trong danh sách media chưa
function checkFileExists(fileName) {
    const mediaItems = document.querySelectorAll('.media-item');
    for (let item of mediaItems) {
        if (item.dataset.mediaName === fileName) {
            return true;
        }
    }
    return false;
}

async function UploadFiles() {
    const fileInput = document.getElementById('file-upload');
    const files = fileInput.files;

    if (files.length === 0) {
        alert('Vui lòng chọn file để upload!');
        return;
    }

    // Lấy tên sản phẩm để tạo slug
    const sanPhamName = document.getElementById('sp-name').value.trim() || 'san-pham';

    // Bỏ check duplicate vì giờ server tự động tạo tên mới (slug-0, slug-1...)
    // Không còn lo trùng tên nữa

    ShowCircleLoader();
    for (let i = 0; i < files.length; i++) {
        const file = files[i];
        const fileName = file.name;

        try {
            const arrayBuffer = await file.arrayBuffer();
            const byteArray = new Uint8Array(arrayBuffer);

            await fetch('/SanPham/UploadMedia?sanPhamId=' + sanPhamId, {
                method: 'POST',
                headers: {
                    'fileName': encodeURIComponent(fileName),
                    'productId': sanPhamId,
                    'sanPhamName': encodeURIComponent(sanPhamName)  // Encode để hỗ trợ tiếng Việt
                },
                body: byteArray
            });

        } catch (error) {
            RemoveCircleLoader();
            CreateMustClickOkModal('Lỗi upload file ' + fileName + ': ' + error.message, null);
            return;
        }
    }
    RemoveCircleLoader();
    alert('Upload thành công!');
    fileInput.value = '';
    await LoadMediaList();
}

async function RenameMediaFile(oldFileName) {
    // Lấy tên file không có extension để làm giá trị mặc định
    const oldNameWithoutExt = oldFileName.substring(0, oldFileName.lastIndexOf('.')) || oldFileName;

    // Prompt nhập tên mới (chỉ tên, không cần extension)
    const newName = prompt('Nhập tên mới cho file (không cần đuôi file):', oldNameWithoutExt);

    if (!newName || newName.trim() === '') {
        return; // User cancel hoặc để trống
    }

    if (newName.trim() === oldNameWithoutExt) {
        alert('Tên mới giống tên cũ!');
        return;
    }

    try {
        ShowCircleLoader();
        const resultText = await PostJSON('/SanPham/RenameMedia', {
            sanPhamId: sanPhamId,
            oldFileName: oldFileName,
            newFileNameWithoutExt: newName.trim()
        });
        RemoveCircleLoader();

        CheckStatusResponseAndShowPrompt(resultText, "Thành công", "Thất bại", false);
        await LoadMediaList();
    } catch (error) {
        RemoveCircleLoader();
        CreateMustClickOkModal('Lỗi kết nối: ' + error.message);
    }
}

async function DeleteMediaFile(fileName) {
    if (!confirm('Bạn có chắc chắn muốn xóa file này?')) {
        return;
    }

    try {
        ShowCircleLoader();
        const resultText = await PostJSON('/SanPham/DeleteMedia', { sanPhamId: sanPhamId, fileName: fileName });
        RemoveCircleLoader();

        CheckStatusResponseAndShowPrompt(resultText, "Thành công", "Thất bại", false);
        await LoadMediaList();
    } catch (error) {
        RemoveCircleLoader();
        CreateMustClickOkModal('Lỗi kết nối: ' + error.message);
    }
}

function ViewBigImage(url) {
    // mở ảnh trong tab mới
    window.open(url, '_blank');
}

// ============================================
// Mapping Sản Phẩm Kho Functions
// ============================================

let currentTbProducts = []; // Kết quả search hiện tại
let currentMappingList = [];

// Load mapping list và datalist options khi page load
window.addEventListener('DOMContentLoaded', async function () {
    await Promise.all([
        LoadKhoMappingList(),
        LoadProductNameList(),
        LoadComboList()
    ]);

    // Clear input kia khi focus vào một input
    const nameInput = document.getElementById('product-name-id');
    const comboInput = document.getElementById('combo-id-2');

    nameInput?.addEventListener('focus', function () {
        comboInput.value = '';
    });

    comboInput?.addEventListener('focus', function () {
        nameInput.value = '';
    });
});

// Load danh sách tên sản phẩm cho datalist
async function LoadProductNameList() {
    try {
        const resultText = await PostJSON('/Product/GetListProductName', {});
        const products = JSON.parse(resultText);
        const datalist = document.getElementById('list-product-name');
        datalist.innerHTML = products.map(p => `<option value="${p.Name}">`).join('');
    } catch (error) {
        console.error('Error loading product names:', error);
    }
}

// Load danh sách combo cho datalist
async function LoadComboList() {
    try {
        const resultText = await PostJSON('/Combo/GetListCombo', {});
        const combos = JSON.parse(resultText);
        const datalist = document.getElementById('list-combo-2');
        datalist.innerHTML = combos.map(c => `<option value="${c.name}">`).join('');
    } catch (error) {
        console.error('Error loading combos:', error);
    }
}

// Search sản phẩm kho với filters (gọi khi user click button Tìm Kiếm)
async function SearchTbProductsForMapping() {
    const nameInput = document.getElementById('product-name-id').value.trim();
    const comboInput = document.getElementById('combo-id-2').value.trim();

    // Nếu cả 2 filter đều trống → không search
    if (!nameInput && !comboInput) {
        CreateMustClickOkModal('Vui lòng nhập tên hoặc combo để tìm kiếm');
        return;
    }

    try {
        ShowCircleLoader();
        // Gọi GET endpoint SearchProductForMapping
        const url = `/Product/SearchProductForMapping?codeOrBarcode=&name=${encodeURIComponent(nameInput)}&combo=${encodeURIComponent(comboInput)}`;
        const response = await fetch(url);
        const resultText = await response.text();
        currentTbProducts = JSON.parse(resultText);

        RenderTbProductsTable();
        RemoveCircleLoader();

        if (currentTbProducts.length === 0) {
            CreateMustClickOkModal('Không tìm thấy sản phẩm nào');
        }
    } catch (error) {
        RemoveCircleLoader();
        console.error('Error searching products:', error);
        CreateMustClickOkModal('Lỗi tìm kiếm: ' + error.message);
    }
}

// Render table sản phẩm kho với checkboxes
function RenderTbProductsTable() {
    const tbody = document.getElementById('tbproducts-tbody');

    if (!currentTbProducts || currentTbProducts.length === 0) {
        tbody.innerHTML = '<tr><td colspan="6" style="padding: 20px; text-align: center; color: #999;">Nhập tên hoặc combo để tìm kiếm sản phẩm</td></tr>';
        document.getElementById('tbproducts-total').textContent = '0';
        return;
    }

    // Get mapped IDs để disable checkbox
    const mappedIds = new Set(currentMappingList.map(m => m.SanPhamKhoId));

    // Check nếu chỉ có 1 kết quả và chưa bị map → auto-check
    const autoCheckFirstItem = currentTbProducts.length === 1 && !mappedIds.has(currentTbProducts[0].id);

    // Render table rows
    tbody.innerHTML = currentTbProducts.map((p, index) => {
        const isAlreadyMapped = mappedIds.has(p.id);
        const shouldAutoCheck = autoCheckFirstItem && index === 0;
        const thumbnailUrl = GetKhoThumbnailUrl_JPGFormat(p.id);
        const imgHtml = `<a href="/Product/UpdateDelete?id=${p.id}" target="_blank" title="Mở sản phẩm ${p.name || ''} trong tab mới">
        <img src="${thumbnailUrl}"
            alt="${p.name || ''}"
            onerror="this.src='${GetKhoThumbnailUrl_PNGFormat(p.id)}'; this.onerror=null;"
            style="width: 50px; height: 50px; object-fit: cover; border-radius: 4px; border: 1px solid #ddd; cursor: pointer;" />
    </a>`;

        return `
    <tr style="${isAlreadyMapped ? 'background: #f0f0f0; opacity: 0.6;' : ''}">
        <td style="padding: 6px; border: 1px solid #ddd; text-align: center;">
            <input type="checkbox"
                class="kho-checkbox"
                value="${p.id}"
                ${isAlreadyMapped ? 'disabled' : ''}
                ${shouldAutoCheck ? 'checked' : ''}
                onchange="UpdateSelectedCount()" />
        </td>
        <td style="padding: 6px; border: 1px solid #ddd; text-align: center;">${index + 1}</td>
        <td style="padding: 6px; border: 1px solid #ddd; text-align: center;">${imgHtml}</td>
        <td style="padding: 6px; border: 1px solid #ddd;">${p.code || 'N/A'}</td>
        <td style="padding: 6px; border: 1px solid #ddd;">${p.name || 'N/A'}${isAlreadyMapped ? ' <span style="color: #d9534f; font-size: 11px;">(Đã map)</span>' : ''}</td>
        <td style="padding: 6px; border: 1px solid #ddd; text-align: center;">1</td>
    </tr>
    `;
    }).join('');

    document.getElementById('tbproducts-total').textContent = currentTbProducts.length;

    // Reset select all checkbox
    const selectAllCheckbox = document.getElementById('select-all-checkbox');
    if (selectAllCheckbox) {
        selectAllCheckbox.checked = false;
    }
    UpdateSelectedCount();
}

// Toggle select all checkboxes
function ToggleSelectAll() {
    const selectAll = document.getElementById('select-all-checkbox').checked;
    const checkboxes = document.querySelectorAll('.kho-checkbox:not([disabled])');
    checkboxes.forEach(cb => cb.checked = selectAll);
    UpdateSelectedCount();
}

// Update selected count
function UpdateSelectedCount() {
    const checkedCount = document.querySelectorAll('.kho-checkbox:checked').length;
    document.getElementById('selected-count').textContent = checkedCount;
}

// Thêm multiple mappings (bulk add)
async function AddSelectedKhoMapping() {
    const checkedCheckboxes = Array.from(document.querySelectorAll('.kho-checkbox:checked'));

    if (checkedCheckboxes.length === 0) {
        CreateMustClickOkModal('Vui lòng chọn ít nhất 1 sản phẩm kho');
        return;
    }

    const selectedIds = checkedCheckboxes.map(cb => parseInt(cb.value));

    try {
        ShowCircleLoader();

        // Add all mappings in parallel
        const addPromises = selectedIds.map(khoId =>
            PostJSON('/SanPham/AddKhoMapping', {
                SanPhamBanId: parseInt(sanPhamId),
                SanPhamKhoId: khoId,
                Quantity: 1
            })
        );

        const results = await Promise.all(addPromises);

        // Parse results
        const parsedResults = results.map(r => JSON.parse(r));
        const successCount = parsedResults.filter(r => r.State === 0).length;
        const failCount = parsedResults.length - successCount;

        RemoveCircleLoader();

        // Tính lại giá bán, giá bìa, chiết khấu
        await CalculateSalePriceAuto();

        // Reload mapping list
        await LoadKhoMappingList();

        // Show result
        if (failCount === 0) {
            //CreateMustClickOkModal(`✓ Đã thêm ${successCount} mapping thành công`);
        } else {
            CreateMustClickOkModal(`Thêm ${successCount} thành công, ${failCount} thất bại`);
        }
    } catch (error) {
        RemoveCircleLoader();
        CreateMustClickOkModal('Lỗi kết nối: ' + error.message);
    }
}

// Load danh sách mapping
async function LoadKhoMappingList() {
    try {
        const resultText = await PostJSON('/SanPham/GetKhoMappingList', { sanPhamBanId: sanPhamId });
        currentMappingList = JSON.parse(resultText);

        RenderKhoMappingTable();

        // Re-render tbproducts table để update disabled checkboxes
        if (currentTbProducts.length > 0) {
            RenderTbProductsTable();
        }
    } catch (error) {
        console.error('Error loading mapping list:', error);
    }
}

// Render bảng mapping
function RenderKhoMappingTable() {
    const tbody = document.getElementById('mapping-tbody');
    document.getElementById('mapping-count').textContent = currentMappingList.length;

    // Enable/disable "Chép Dữ Liệu" button - chỉ enable khi có đúng 1 mapping
    const copyBtn = document.getElementById('copy-data-btn');
    if (copyBtn) {
        copyBtn.disabled = currentMappingList.length !== 1;
        // Update tooltip
        copyBtn.title = currentMappingList.length === 1
            ? 'Chép dữ liệu từ sản phẩm kho sang sản phẩm bán không bao gồm ảnh'
            : 'Chỉ có thể chép khi có đúng 1 mapping';
    }

    // Enable/disable "Chép Ảnh" button - chỉ enable khi có đúng 1 mapping
    const copyImagesBtn = document.getElementById('copy-images-btn');
    if (copyImagesBtn) {
        copyImagesBtn.disabled = currentMappingList.length !== 1;
        // Update tooltip
        copyImagesBtn.title = currentMappingList.length === 1
            ? 'Chép toàn bộ ảnh từ sản phẩm kho sang sản phẩm bán (xóa ảnh cũ và sinh phiên bản 320)'
            : 'Chỉ có thể chép khi có đúng 1 mapping';
    }

    if (currentMappingList.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="9" style="padding: 20px; text-align: center; color: #999;">
                    Chưa có mapping nào. Thêm sản phẩm kho ở phía trên.
                </td>
            </tr>
        `;
        return;
    }

    tbody.innerHTML = currentMappingList.map((mapping, index) => {
        const thumbnailUrl = GetKhoThumbnailUrl_JPGFormat(mapping.SanPhamKhoId);
        const imgHtml = `<a href="/Product/UpdateDelete?id=${mapping.SanPhamKhoId}" target="_blank" title="Mở sản phẩm ${mapping.SanPhamKhoName || ''} trong tab mới">
        <img src="${thumbnailUrl}"
            alt="${mapping.SanPhamKhoName || ''}"
            onerror="this.src='${GetKhoThumbnailUrl_PNGFormat(mapping.SanPhamKhoId)}'; this.onerror=null;"
            style="width: 60px; height: 60px; object-fit: cover; border-radius: 4px; border: 1px solid #ddd; cursor: pointer;" />
    </a>`;

        // Format giá bìa và chiết khấu
        const giaBia = (mapping.SanPhamKhoBookCoverPrice || 0).toLocaleString('vi-VN');
        const chietKhau = mapping.SanPhamKhoDiscount || 0;

        return `
    <tr>
        <td style="padding: 8px; border: 1px solid #ddd; text-align: center;">${index + 1}</td>
        <td style="padding: 8px; border: 1px solid #ddd; text-align: center;">${imgHtml}</td>
        <td style="padding: 8px; border: 1px solid #ddd;">${mapping.SanPhamKhoCode || 'N/A'}</td>
        <td style="padding: 8px; border: 1px solid #ddd;">${mapping.SanPhamKhoName || 'N/A'}</td>
        <td style="padding: 8px; border: 1px solid #ddd; text-align: right; font-weight: 500;">${giaBia} đ</td>
        <td style="padding: 8px; border: 1px solid #ddd; text-align: center; color: #FF5722; font-weight: 500;">${chietKhau}%</td>
        <td style="padding: 8px; border: 1px solid #ddd; text-align: center;">${mapping.SanPhamKhoQuantity}</td>
        <td style="padding: 8px; border: 1px solid #ddd; text-align: center;">
            <input type="number" value="${mapping.Quantity}" min="1"
                title="tăng giảm số lượng sẽ được cập nhật"
                onchange="UpdateKhoMappingQuantity(${mapping.Id}, this.value)"
                style="width: 60px; padding: 4px; border: 1px solid #ddd; border-radius: 4px; text-align: center;" />
        </td>
        <td style="padding: 8px; border: 1px solid #ddd; text-align: center;">
            <button type="button" onclick="DeleteKhoMapping(${mapping.Id})"
                style="padding: 4px 12px; background: #d9534f; color: white; border: none; border-radius: 4px; cursor: pointer;">
                🗑 Xóa
            </button>
        </td>
    </tr>
    `;
    }).join('');
}

// Update số lượng mapping
async function UpdateKhoMappingQuantity(mappingId, newQuantity) {
    const quantity = parseInt(newQuantity);
    if (isNaN(quantity) || quantity < 1) {
        CreateMustClickOkModal('Số lượng phải >= 1');
        await LoadKhoMappingList();
        return;
    }

    try {
        ShowCircleLoader();
        const resultText = await PostJSON('/SanPham/UpdateKhoMappingQuantity', {
            mappingId: mappingId,
            quantity: quantity
        });

        RemoveCircleLoader();

        const result = JSON.parse(resultText);
        if (result.State === 0) {
            // Success - silent update, no modal
            //console.log('Quantity updated successfully');
        } else {
            CreateMustClickOkModal('Lỗi: ' + result.Message);
            await LoadKhoMappingList();
        }
    } catch (error) {
        CreateMustClickOkModal('Lỗi kết nối: ' + error.message);
        await LoadKhoMappingList();
    }
}

// Xóa mapping
async function DeleteKhoMapping(mappingId) {
    if (!confirm('Xóa mapping này?')) {
        return;
    }

    try {
        ShowCircleLoader();
        const resultText = await PostJSON('/SanPham/DeleteKhoMapping', { mappingId: mappingId });
        RemoveCircleLoader();

        const result = JSON.parse(resultText);
        if (result.State === 0) {
            // Reload list FIRST (user sees updated table immediately)
            await LoadKhoMappingList();

            // Tính lại giá bán, giá bìa, chiết khấu
            await CalculateSalePriceAuto();

            // Then show success
            //CreateMustClickOkModal('✓ Xóa mapping thành công');
        } else {
            CreateMustClickOkModal('Lỗi: ' + result.Message);
        }
    } catch (error) {
        RemoveCircleLoader();
        CreateMustClickOkModal('Lỗi kết nối: ' + error.message);
    }
}

// Chép dữ liệu từ sản phẩm kho sang sản phẩm bán (khi chỉ có 1 mapping)
async function CopyDataFromKhoProduct() {
    if (currentMappingList.length !== 1) {
        CreateMustClickOkModal('Chỉ có thể chép dữ liệu khi có đúng 1 mapping');
        return;
    }

    const mapping = currentMappingList[0];
    const sanPhamKhoName = mapping.SanPhamKhoName || 'sản phẩm kho này';

    if (!confirm(`Chép tất cả dữ liệu từ "${sanPhamKhoName}" sang sản phẩm hiện tại?\n\nLưu ý: Dữ liệu hiện tại sẽ bị ghi đè (trừ Số lượng đã bán, URL, SEO).`)) {
        return;
    }

    try {
        ShowCircleLoader();
        const resultText = await PostJSON('/SanPham/CopyDataFromKhoProduct', {
            sanPhamBanId: parseInt(sanPhamId),
            sanPhamKhoId: mapping.SanPhamKhoId
        });
        RemoveCircleLoader();

        const result = JSON.parse(resultText);
        if (result.State === 0) {
            CreateMustClickOkModal('✓ Chép dữ liệu thành công! Đang tải lại trang...');

            // Reload page để hiển thị dữ liệu mới
            setTimeout(() => {
                location.reload();
            }, 1000);
        } else {
            CreateMustClickOkModal('Lỗi: ' + result.Message);
        }
    } catch (error) {
        RemoveCircleLoader();
        CreateMustClickOkModal('Lỗi kết nối: ' + error.message);
    }
}

// Chép ảnh từ sản phẩm kho sang sản phẩm bán (khi chỉ có 1 mapping)
async function CopyImagesFromKhoProduct() {
    if (currentMappingList.length !== 1) {
        CreateMustClickOkModal('Chỉ có thể chép ảnh khi có đúng 1 mapping');
        return;
    }

    const mapping = currentMappingList[0];
    const sanPhamKhoName = mapping.SanPhamKhoName || 'sản phẩm kho này';

    if (!confirm(`Chép toàn bộ ảnh từ "${sanPhamKhoName}" sang sản phẩm hiện tại?\n\nCảnh báo:\n• Ảnh/Video cũ sẽ bị XÓA hoàn toàn\n• Ảnh mới sẽ được convert và sinh phiên bản 320\n• Quá trình có thể mất vài giây`)) {
        return;
    }

    try {
        // Lấy tên sản phẩm để tạo slug
        const sanPhamName = document.getElementById('sp-name').value.trim() || 'san-pham';
        ShowCircleLoader();
        const resultText = await PostJSON('/SanPham/CopyImagesFromKhoProduct', {
            sanPhamName: sanPhamName,
            sanPhamBanId: parseInt(sanPhamId),
            sanPhamKhoId: mapping.SanPhamKhoId
        });
        RemoveCircleLoader();

        const result = JSON.parse(resultText);
        if (result.State === 0) {
            //CreateMustClickOkModal('✓ Chép ảnh thành công! Đang tải lại trang...');

            // Reload page để hiển thị ảnh mới
            setTimeout(() => {
                location.reload();
            }, 1000);
        } else {
            CreateMustClickOkModal('Lỗi: ' + result.Message);
        }
    } catch (error) {
        RemoveCircleLoader();
        CreateMustClickOkModal('Lỗi kết nối: ' + error.message);
    }
}

// ========================================
// Tạo Sản Phẩm Mới (Copy thông tin)
// ========================================

/**
 * Tạo sản phẩm mới từ sản phẩm hiện tại
 * Copy thông tin NHƯNG KHÔNG copy: giá tiền, media, mappings
 */
async function CopyToNewProduct() {
    const sanPhamName = document.getElementById('sp-name').value.trim() || 'Sản phẩm';

    const confirmMsg = `Tạo sản phẩm mới từ "${sanPhamName}"?\n\n` +
        `✅ Copy:\n` +
        `• Thông tin sản phẩm (tên, tác giả, NXB, kích thước...)\n` +
        `• Mô tả chi tiết\n\n` +
        `❌ KHÔNG copy:\n` +
        `• Giá bìa, giá bán, chiết khấu\n` +
        `• Media (ảnh/video)\n` +
        `• Mapping sản phẩm kho\n` +
        `• URL, SEO keyword\n` +
        `• Code, Barcode\n\n` +
        `Sản phẩm mới sẽ ở trạng thái "Ngừng bán" và cần mapping + tính giá.`;

    if (!confirm(confirmMsg)) {
        return;
    }

    try {
        ShowCircleLoader();
        const resultText = await PostJSON('/SanPham/CopyToNewProduct', {
            sanPhamId: parseInt(sanPhamId)
        });
        RemoveCircleLoader();

        const result = JSON.parse(resultText);

        if (result.State === 0) {
            const newProductId = result.myJson;
            CreateMustClickOkModal(`✓ ${result.Message}\n\nĐang chuyển sang sản phẩm mới...`, null);

            // Redirect to new product page
            setTimeout(() => {
                window.location.href = `/SanPham/UpdateDelete?id=${newProductId}`;
            }, 1500);
        } else {
            CreateMustClickOkModal('❌ ' + result.Message, null);
        }
    } catch (error) {
        RemoveCircleLoader();
        CreateMustClickOkModal('Lỗi kết nối: ' + error.message, null);
    }
}

// ========================================
// 🧮 Tính Giá Bán Tự Động
// ========================================

async function CalculateSalePriceAuto() {
    const sanPhamId = parseInt(document.getElementById('sp-id').value);
    const platform = document.getElementById('platform-select').value;

    if (!sanPhamId) {
        CreateMustClickOkModal('Không xác định được ID sản phẩm');
        return;
    }

    try {
        ShowCircleLoader();

        const resultText = await PostJSON('/SanPham/CalculateAndUpdateSalePrice', {
            sanPhamId: sanPhamId,
            platform: platform
        });

        RemoveCircleLoader();

        const result = JSON.parse(resultText);

        if (result.State === 0) {
            // Success - update SalePrice, BookCoverPrice, Discount
            document.getElementById('sp-sale-price').value = result.SalePrice;

            if (result.BookCoverPrice !== undefined) {
                document.getElementById('sp-book-cover-price').value = result.BookCoverPrice;
            }

            if (result.Discount !== undefined) {
                document.getElementById('sp-discount').value = result.Discount.toFixed(1);
            }

            // Show breakdown
            const breakdown = result.Breakdown;
            const breakdownDiv = document.getElementById('price-breakdown');

            if (breakdown) {
                breakdownDiv.innerHTML = `
                    <div style="font-weight: 600; margin-bottom: 8px; color: #1976D2;">
                        ✓ ${result.Message}
                    </div>
                    <div style="display: grid; grid-template-columns: auto 1fr; gap: 5px 15px;">
                        <span>Tổng giá nhập:</span>
                        <span style="text-align: right; font-weight: 500;">${Math.round(breakdown.TongGiaNhap).toLocaleString('vi-VN')} đ</span>

                        <span>Chi phí đóng gói:</span>
                        <span style="text-align: right; font-weight: 500;">${Math.round(breakdown.ChiPhiDongGoi).toLocaleString('vi-VN')} đ</span>

                        <span style="border-top: 1px solid #ddd; padding-top: 5px;">Tổng chi phí:</span>
                        <span style="text-align: right; font-weight: 600; border-top: 1px solid #ddd; padding-top: 5px;">${Math.round(breakdown.TongChiPhi).toLocaleString('vi-VN')} đ</span>

                        <span>Lợi nhuận mong muốn:</span>
                        <span style="text-align: right; font-weight: 500; color: #4CAF50;">+${Math.round(breakdown.LoiNhuanMongMuon).toLocaleString('vi-VN')} đ</span>

                        <span>Giá trước thuế phí:</span>
                        <span style="text-align: right; font-weight: 500;">${Math.round(breakdown.GiaTruocThuePhi).toLocaleString('vi-VN')} đ</span>

                        <span>Thuế:</span>
                        <span style="text-align: right; font-weight: 500; color: #FF5722;">+${Math.round(breakdown.Thue).toLocaleString('vi-VN')} đ</span>

                        <span>Phí sàn:</span>
                        <span style="text-align: right; font-weight: 500; color: #FF5722;">+${Math.round(breakdown.PhiSan).toLocaleString('vi-VN')} đ</span>

                        <span style="border-top: 2px solid #1976D2; padding-top: 5px; font-weight: 600;">💰 Giá bán thực tế:</span>
                        <span style="text-align: right; font-weight: 700; color: #1976D2; border-top: 2px solid #1976D2; padding-top: 5px; font-size: 16px;">${breakdown.GiaBanThucTe.toLocaleString('vi-VN')} đ</span>
                    </div>
                `;
                breakdownDiv.style.display = 'block';
            } else {
                breakdownDiv.style.display = 'none';
            }

            CreateMustClickOkModal(result.Message);
        } else {
            document.getElementById('price-breakdown').style.display = 'none';
            CreateMustClickOkModal('❌ ' + result.Message);
        }
    } catch (error) {
        RemoveCircleLoader();
        CreateMustClickOkModal('Lỗi kết nối: ' + error.Message);
    }
}
