// Global variables
let selectedIndex; // selected index media đang chọn hiện tại
let sanPhamObject; // object sản phẩm đang hiển thị
let variantsList; // Danh sách variants (sản phẩm cùng ComboId)
let quantityInput; // Input số lượng

quantityInput = 1;

let limitQuantity = "Số lượng bạn chọn đã đạt mức tối đa của sản phẩm này";
let dontSelectVariation = "Vui lòng chọn phân loại sản phẩm";

// Get the modal thông báo đã thêm vào giỏ hàng thành công
let modal = document.getElementById("myModal");
// Get the modal nhắc số lượng trong giỏ vượt quá tồn kho
let modalOverMax = document.getElementById("myModal-over-max");
InitializeTickOkModal();

function InitializeTickOkModal() {
    // Get the <span> element that closes the modal
    let tickOk = document.getElementsByClassName("tick-ok")[0];

    tickOk.onclick = function () {
        CloseModal();
    }

    // When the user clicks anywhere outside of the modal, close it
    window.onclick = function (event) {
        if (event.target == modal) {
            CloseModal();
        }
    }
}

// Constructor function for media objects
function objMediaSrc(itemSrc, itemIsVideo) {
    this.src = itemSrc;
    this.isVideo = itemIsVideo;
}

async function GetSanPhamWithVariants(id) {
    const searchParams = new URLSearchParams();
    searchParams.append("id", id);

    let query = "/Home/GetSanPhamWithVariants";

    return RequestHttpPostPromise(searchParams, query);
}

// Lấy đường dẫn media folder của sản phẩm
function GetMediaFolderPath(sanPhamId) {
    return "/Media/Product/" + sanPhamId + "/";
}

// Lấy danh sách ảnh/video từ server folder
async function LoadMediaList(sanPhamId) {
    try {
        const metadataResultText = await PostJSON('/SanPham/GetAllMediaMetadata', { sanPhamId: sanPhamId });
        const metadataList = JSON.parse(metadataResultText);
        return metadataList;
    }
    catch (ex) {
        console.error("Error loading media list:", ex);
        return [];
    }
}

async function HomePageShowSanPham() {
    ShowCircleLoader();
    if (DEBUG) {
        console.log("current url: " + window.location.href);
        console.log("current id on url: " + GetIdFromCurrentSlugIdUrl());
    }

    let currentId = GetIdFromCurrentSlugIdUrl();
    if (DEBUG) {
        console.log("currentId: " + currentId);
    }
    let responseDB = await GetSanPhamWithVariants(currentId);
    RemoveCircleLoader();

    if (responseDB.responseText != "null") {
        // Parse danh sách variants (bao gồm cả sản phẩm chính)
        variantsList = JSON.parse(responseDB.responseText);
        if (DEBUG) {
            console.log("variantsList: " + JSON.stringify(variantsList));
        }

        // Tìm sản phẩm chính trong list
        sanPhamObject = variantsList.find(v => v.Id === parseInt(currentId));
        if(DEBUG)
        {
            console.log("sanPhamObject: " + JSON.stringify(sanPhamObject));
        }

        if (!sanPhamObject) {
            ShowDoesntFindId();
            return;
        }
    }
    else {
        ShowDoesntFindId();
        return;
    }

    // Tính chiều cao item-medium-media
    if (scrWidth >= 800) {
        document.getElementById("item-medium-media").style.height = "600px";
    }
    else {
        document.getElementById("item-medium-media").style.height = scrWidth + "px";
    }

    document.title = sanPhamObject.Name;

    ShowSmallItem();
    ShowRightLeftArrow();

    // Chọn item đầu tiên
    if (sanPhamObject.MediaList.length > 0) {
        ShowMediumMediaFromSelectedSmallItem(document.getElementById("item-small-media-container").children[0]);
    }

    // Hiển thị phân loại (variants) nếu có
    ShowVariations();

    ShowItemSomething();

    // Hiển thị thông tin chi tiết (specs table)
    ShowProductSpecifications();

    // Hiển thị mô tả chi tiết sản phẩm
    ShowProductDescription();
}

// Nếu sản phẩm có video thì hasVideo = true, nếu không thì hasVideo = false
function CreateContainerSmallItem(media, i, hasVideo) {
    const container = document.createElement("div");
    container.className = "small-media";
    container.setAttribute("data-index", i);

    // Vì nếu có video thì nó sẽ ở vị trí đầu tiên
    if (i == 0 && hasVideo) {
        const video = document.createElement("video");
        video.src = GetSanPhamMediaUrl(sanPhamObject.Id, media.FileName);
        video.controls = false;
        video.style.objectFit = "contain";
        video.style.width = "100%";
        video.style.height = "100%";
        container.style.position = "relative";

        // Thêm video icon
        let videoIcon = document.createElement("div");
        videoIcon.style.position = "absolute";
        videoIcon.style.top = "30px";
        videoIcon.style.left = "30px";
        videoIcon.style.width = "40px";
        videoIcon.style.height = "40px";
        videoIcon.style.zIndex = 1;
        videoIcon.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" version="1.1" width="40" height="40" viewBox="0 0 256 256"><g style="stroke: none; stroke-width: 0; fill: none; opacity: 1;" transform="translate(1.4065934065934016 1.4065934065934016) scale(2.81 2.81)"><path d="M 45 0 C 20.147 0 0 20.147 0 45 c 0 24.853 20.147 45 45 45 s 45 -20.147 45 -45 C 90 20.147 69.853 0 45 0 z M 62.251 46.633 L 37.789 60.756 c -1.258 0.726 -2.829 -0.181 -2.829 -1.633 V 30.877 c 0 -1.452 1.572 -2.36 2.829 -1.634 l 24.461 14.123 C 63.508 44.092 63.508 45.907 62.251 46.633 z" style="stroke: none; fill: rgb(0,0,0); opacity: 1;" transform=" matrix(1 0 0 1 0 0) " /></g></svg>';

        container.appendChild(video);
        container.appendChild(videoIcon);
    }
    else {
        // Tạo picture element để hỗ trợ WebP
        const picture = document.createElement("picture");
        picture.style.display = "flex";
        picture.style.justifyContent = "center";
        picture.style.alignItems = "center";
        picture.style.width = "100%";
        picture.style.height = "100%";

        // WebP source (ưu tiên WebP nếu browser hỗ trợ)
        const sourceWebp = document.createElement("source");
        const thumbnail320 = Get320VersionOfImageSrc(GetSanPhamMediaUrl(sanPhamObject.Id, media.FileName));
        sourceWebp.srcset = thumbnail320;
        sourceWebp.type = "image/webp";

        // Fallback img (srcNoImageThumbnail.png nếu browser không hỗ trợ WebP)
        const img = document.createElement("img");
        img.src = srcNoImageThumbnail;  // Fallback về NoImageThumbnail.png
        img.alt = (media.AltText || media.FileName) + " - Ảnh " + hasVideo? i: (i + 1);  // Thumbnail có số thứ tự
        img.style.objectFit = "contain";
        img.style.maxWidth = "100%";
        img.style.maxHeight = "100%";

        picture.appendChild(sourceWebp);
        picture.appendChild(img);
        container.appendChild(picture);
    }

    container.addEventListener("mouseenter", function (event) {
        ShowMediumMediaFromSelectedSmallItem(event.currentTarget);
    });
    container.addEventListener("mousedown", function (event) {
        ShowMediumMediaFromSelectedSmallItem(event.currentTarget);
    });

    return container;
}

function ShowSmallItem() {
    let itemSmallMediaContainer = document.getElementById("item-small-media-container");
    // Xóa item cũ nếu có
    itemSmallMediaContainer.innerHTML = "";
    if (sanPhamObject.MediaList.length == 0) {
        return;
    }

    // Kiểm tra xem có video hay không, nếu có thì video sẽ được đặt ở vị trí đầu tiên
    let hasVideo = sanPhamObject.MediaList[0].MediaType != "image"? true:false;
    for (let i = 0; i < sanPhamObject.MediaList.length; i++) {
        itemSmallMediaContainer.appendChild(CreateContainerSmallItem(sanPhamObject.MediaList[i], i, hasVideo));
    }
}

function ShowRightLeftArrow() {
    // if (sanPhamObject.MediaList.length <= 1) {
    //     document.getElementById("left_arrow").innerHTML = "";
    //     document.getElementById("right_arrow").innerHTML = "";
    //     return;
    // }

    // Add mũi tên di chuyển sang trái
    let leftArrow = document.getElementById("left_arrow");
    leftArrow.innerHTML = '<svg class="arrow-icon" xmlns="http://www.w3.org/2000/svg" version="1.1" height="100" width="40" fill-opacity="0.4"><polygon points="10,50 30,30 30,70" style="fill:lime;" /></svg>';

    // Attach click event vào SVG chứ không phải div
    let leftArrowSvg = leftArrow.querySelector("svg");
    leftArrowSvg.addEventListener("click", function (event) {
        event.preventDefault(); // Ngăn scroll jump
        if (sanPhamObject.MediaList.length == 0 || sanPhamObject.MediaList.length == 1) {
            return;
        }

        if (selectedIndex == 0) {
            selectedIndex = sanPhamObject.MediaList.length - 1;
        }
        else {
            selectedIndex--;
        }
        ShowMediumItemFromIndex(selectedIndex);
        ScrollToSelectedSmallItem();
    });

    // Add mũi tên di chuyển sang phải
    let rightArrow = document.getElementById("right_arrow");
    rightArrow.innerHTML = '<svg class="arrow-icon" xmlns="http://www.w3.org/2000/svg" version="1.1" height="100" width="40" fill-opacity="0.4"><polygon points="10,30 30,50 10,70" style="fill:lime;" /></svg>';

    // Attach click event vào SVG chứ không phải div
    let rightArrowSvg = rightArrow.querySelector("svg");
    rightArrowSvg.addEventListener("click", function (event) {
        event.preventDefault(); // Ngăn scroll jump
        if (sanPhamObject.MediaList.length == 0 || sanPhamObject.MediaList.length == 1) {
            return;
        }
        if (selectedIndex == sanPhamObject.MediaList.length - 1) {
            selectedIndex = 0;
        }
        else {
            selectedIndex++;
        }

        ShowMediumItemFromIndex(selectedIndex);
        ScrollToSelectedSmallItem();
    });
}

// Show medium item từ index
function ShowMediumItemFromIndex(i) {
    let mediumPicture = document.getElementById("medium_picture");
    let mediumVideo = document.getElementById("medium_video");

    if (sanPhamObject.MediaList[i].MediaType != "image") {
        if (isEmptyOrSpaces(mediumVideo.src)) {
            mediumVideo.src = GetSanPhamMediaUrl(sanPhamObject.Id, sanPhamObject.MediaList[i].FileName);
        }

        mediumVideo.play();
        mediumVideo.style.display = "block";

        mediumPicture.style.display = "none";
    }
    else {
        // Get image URL và WebP URL
        const imageUrl = GetSanPhamMediaUrl(sanPhamObject.Id, sanPhamObject.MediaList[i].FileName);

        // Set WebP source (browser hỗ trợ WebP sẽ dùng này)
        const mediumImageSource = document.getElementById("medium_image_source");
        mediumImageSource.srcset = imageUrl;

        // Set alt text cho medium image (SEO + Accessibility)
        const mediumImage = document.getElementById("medium_image");
        mediumImage.alt = sanPhamObject.MediaList[i].AltText || sanPhamObject.MediaList[i].FileName;

        if (mediumVideo.src != null) {
            mediumVideo.pause();
        }

        mediumPicture.style.display = "flex";
        mediumVideo.style.display = "none";
    }

    ChangeBorderColorOfSelectedSmallItem();
}

function ShowProductDescription() {
    let detailContainer = document.getElementsByClassName("f7AU53")[0];
    let productDetailSection = document.getElementsByClassName("product-detail")[0];

    if (sanPhamObject.Detail == null) {
        // Ẩn mô tả sản phẩm
        productDetailSection.style.display = "none";
        return;
    }

    // Hiển thị lại section nếu đã ẩn
    productDetailSection.style.display = "block";

    // Clear nội dung cũ
    detailContainer.innerHTML = "";

    // Parse Detail để replace {{image:filename}} bằng HTML
    let detailHtml = sanPhamObject.Detail;

    // Regex pattern: {{image:filename.ext}}
    detailHtml = detailHtml.replace(/\{\{image:([^}]+)\}\}/g, function(match, filename) {
        // Tìm metadata của image này trong MediaList
        let media = null;
        if (sanPhamObject.MediaList && sanPhamObject.MediaList.length > 0) {
            media = sanPhamObject.MediaList.find(m => m.FileName === filename);
        }

        // Build image URL
        let imgSrc = GetSanPhamMediaUrl(sanPhamObject.Id, filename);
        let alt = media ? (media.AltText || media.FileName) : filename;
        let caption = media ? (media.Title || "") : "";
        if (DEBUG) {
            console.log("ShowProductDescription media: " + JSON.stringify(media));
        }

        // Build HTML với figure + figcaption
        let html = '<figure class="product-detail-image">';
        html += `<img src="${imgSrc}" alt="${alt}" loading="lazy">`;
        if (caption) {
            html += `<figcaption>${caption}</figcaption>`;
        }
        html += '</figure>';

        return html;
    });

    // Thêm mô tả mới
    let p = document.createElement("p");
    p.className = "irIKAp";
    p.innerHTML = detailHtml;
    detailContainer.appendChild(p);
}

function ShowProductSpecifications() {
    let specificationsTable = document.querySelector(".specifications-table");
    let specificationsSection = document.querySelector(".product-specifications");

    if (!sanPhamObject) {
        specificationsSection.style.display = "none";
        return;
    }

    // Hiển thị section
    specificationsSection.style.display = "block";

    // Clear nội dung cũ
    specificationsTable.innerHTML = "";

    // Helper function để thêm spec row
    function AddSpecRow(label, value, url) {
        if (!value || value === "" || value === null || value === undefined) {
            return; // Skip nếu không có giá trị
        }

        let row = document.createElement("div");
        row.className = "spec-row";

        let labelDiv = document.createElement("div");
        labelDiv.className = "spec-label";
        labelDiv.textContent = label;

        let valueDiv = document.createElement("div");
        valueDiv.className = "spec-value";

        // Nếu có URL → tạo link, nếu không → text thường
        if (url) {
            let link = document.createElement("a");
            link.href = url;
            link.textContent = value;
            link.className = "spec-link";
            valueDiv.appendChild(link);
        } else {
            valueDiv.textContent = value;
        }

        row.appendChild(labelDiv);
        row.appendChild(valueDiv);
        specificationsTable.appendChild(row);
    }

    // Thêm các thông tin chi tiết (với links)
    if (sanPhamObject.Author) {
        AddSpecRow("Tác giả", sanPhamObject.Author, `/Search?author=${encodeURIComponent(sanPhamObject.Author)}`);
    }
    if (sanPhamObject.Translator) {
        AddSpecRow("Người dịch", sanPhamObject.Translator, `/Search?translator=${encodeURIComponent(sanPhamObject.Translator)}`);
    }
    if (sanPhamObject.CategoryName && sanPhamObject.CategoryId) {
        AddSpecRow("Danh mục", sanPhamObject.CategoryName, `/Search?categoryId=${sanPhamObject.CategoryId}`);
    }
    if (sanPhamObject.PublishingCompany) {
        AddSpecRow("Nhà xuất bản", sanPhamObject.PublishingCompany, `/Search?publishingCompany=${encodeURIComponent(sanPhamObject.PublishingCompany)}`);
    }
    if (sanPhamObject.PublisherName && sanPhamObject.PublisherId) {
        AddSpecRow("Nhà phát hành", sanPhamObject.PublisherName, `/Search?publisherId=${sanPhamObject.PublisherId}`);
    }

    if (sanPhamObject.PublishingTime) {
        AddSpecRow("Năm xuất bản", sanPhamObject.PublishingTime.toString());
    }

    AddSpecRow("Ngôn ngữ", sanPhamObject.Language);

    // Kích thước (chỉ hiển thị nếu có ít nhất 1 giá trị > 0)
    if (sanPhamObject.ProductLong > 0 || sanPhamObject.ProductWide > 0 || sanPhamObject.ProductHigh > 0) {
        let dimensions = `${sanPhamObject.ProductLong} × ${sanPhamObject.ProductWide} × ${sanPhamObject.ProductHigh} mm`;
        AddSpecRow("Kích thước", dimensions);
    }

    // Trọng lượng
    if (sanPhamObject.ProductWeight > 0) {
        AddSpecRow("Trọng lượng", sanPhamObject.ProductWeight + " gram");
    }

    // Số trang
    if (sanPhamObject.PageNumber) {
        AddSpecRow("Số trang", sanPhamObject.PageNumber.toString());
    }

    // Hình thức (Bìa cứng/mềm)
    if (sanPhamObject.HardCover !== null && sanPhamObject.HardCover !== undefined) {
        let coverType = sanPhamObject.HardCover === 1 ? "Bìa cứng" : "Bìa mềm";
        AddSpecRow("Hình thức", coverType);
    }

    // ISBN (từ Barcode)
    AddSpecRow("Mã", sanPhamObject.Code || sanPhamObject.Barcode);

    // Nếu không có thông tin nào → ẩn section
    if (specificationsTable.children.length === 0) {
        specificationsSection.style.display = "none";
    }
}

// Thay đổi medium media khi di chuyển bên trên small item
function ShowMediumMediaFromSelectedSmallItem(newSelectedItem) {
    if (selectedIndex == parseInt(newSelectedItem.getAttribute("data-index")))
        return;

    selectedIndex = parseInt(newSelectedItem.getAttribute("data-index"));

    // Hiển thị item ở kích thước to hơn
    ShowMediumItemFromIndex(selectedIndex);
}

// Thay đổi màu border
function ChangeBorderColorOfSelectedSmallItem() {
    if (selectedIndex != null) {
        let itemSmallMediaContainer = document.getElementById("item-small-media-container");
        for (let i = 0; i < itemSmallMediaContainer.children.length; i++) {
            if (parseInt(itemSmallMediaContainer.children[i].getAttribute("data-index")) == selectedIndex) {
                itemSmallMediaContainer.children[i].style.borderColor = "rgb(255, 0, 0)";
            }
            else {
                itemSmallMediaContainer.children[i].style.borderColor = "rgb(228, 228, 222)";
            }
        }
    }
}

// Scroll để hiển thị small item đã chọn
function ScrollToSelectedSmallItem() {
    let itemSmallMediaContainer = document.getElementById("item-small-media-container");

    for (let i = 0; i < itemSmallMediaContainer.children.length; i++) {
        if (parseInt(itemSmallMediaContainer.children[i].getAttribute("data-index")) == selectedIndex) {
            let offsetLeft = itemSmallMediaContainer.children[i].offsetLeft;

            // Mỗi small item chiếm 110px (100px width + 10px margin)
            if (offsetLeft + 110 > itemSmallMediaContainer.scrollLeft + itemSmallMediaContainer.clientWidth) {
                itemSmallMediaContainer.scrollLeft =
                    offsetLeft + 110 - itemSmallMediaContainer.clientWidth;
            }
            else if (offsetLeft < itemSmallMediaContainer.scrollLeft) {
                itemSmallMediaContainer.scrollLeft = offsetLeft;
            }
        }
    }
}

function ShowItemSomething() {
    // Hiển thị tên sản phẩm
    document.getElementById("item-name-h1").textContent = sanPhamObject.Name;

    // Hiển thị giá
    ShowPriceAndReadyMaxQuantity();
}

// Hiển thị giá bìa, giá bán và % chiết khấu
function ShowPriceAndReadyMaxQuantity() {
    // Giá bìa
    document.getElementById("book-cover-price").innerHTML =
        ConvertMoneyToTextWithIcon(sanPhamObject.BookCoverPrice);

    // Giá bán thực tế
    document.getElementById("price").innerHTML =
        ConvertMoneyToTextWithIcon(sanPhamObject.SalePrice);

    // % Chiết khấu
    let discountPercent = Math.round(100 * (sanPhamObject.BookCoverPrice - sanPhamObject.SalePrice) / sanPhamObject.BookCoverPrice);

    // Ẩn/hiện giá bìa và % giảm dựa vào discount
    if (discountPercent == 0) {
        // Không có giảm giá → ẩn giá bìa và % giảm
        document.getElementById("book-cover-price").style.display = "none";
        document.getElementById("discount").style.display = "none";
    }
    else {
        // Có giảm giá → hiển thị giá bìa và % giảm
        document.getElementById("book-cover-price").style.display = "";
        document.getElementById("discount").style.display = "";
        document.getElementById("discount").innerHTML = discountPercent + "% GIẢM";
    }

    // Hiển thị số lượng tồn kho
    document.getElementById("max-quatity").textContent =
        sanPhamObject.Quantity + " sản phẩm có sẵn";
}

// Apply highlight style và dấu tick cho variant button
function ApplyVariantHighlight(button) {
    button.style.borderColor = "rgb(255, 0, 0)";
    button.style.color = "rgb(255, 0, 0)";

    // Thêm dấu tick góc dưới phải
    let checkContainer = document.createElement("div");
    checkContainer.id = "check-container";

    let tickIcon = document.createElementNS("http://www.w3.org/2000/svg", "svg");
    tickIcon.setAttribute("viewBox", "0 0 12 12");
    tickIcon.setAttribute("class", "icon-tick-bold");

    let polyline = document.createElementNS("http://www.w3.org/2000/svg", "polyline");
    polyline.setAttribute("fill", "none");
    polyline.setAttribute("points", "1.5 6 4.5 9 10.5 3");
    polyline.setAttribute("stroke-width", "2");
    polyline.setAttribute("stroke", "currentColor");

    tickIcon.appendChild(polyline);
    checkContainer.appendChild(tickIcon);
    button.appendChild(checkContainer);
}

// Remove highlight style và dấu tick khỏi variant button
function RemoveVariantHighlight(button) {
    button.style.borderColor = "";
    button.style.color = "";

    // Xóa dấu tick nếu có
    let checkContainer = button.querySelector("#check-container");
    if (checkContainer) {
        checkContainer.remove();
    }
}

// Update highlight variant đang chọn (không re-render toàn bộ)
function HighlightSelectedVariant(variantId) {
    // Tìm tất cả variant buttons
    let allButtons = document.querySelectorAll(".variation-button");

    allButtons.forEach(button => {
        let buttonVariantId = parseInt(button.getAttribute("data-variant-id"));

        if (buttonVariantId === variantId) {
            // Highlight button được chọn
            ApplyVariantHighlight(button);
        } else {
            // Remove highlight các button khác
            RemoveVariantHighlight(button);
        }
    });
}

// Hiển thị phân loại (variants - các sản phẩm cùng ComboId)
function ShowVariations() {
    if (!variantsList || variantsList.length <= 1) {
        // Không có variants hoặc chỉ có 1 sản phẩm
        document.getElementById("item-variation-container").style.display = "none";
        return;
    }

    let variationContainer = document.getElementById("item-variation-container");
    variationContainer.innerHTML = "";

    // Tiêu đề phân loại
    let variationTitle = document.createElement("div");
    variationTitle.className = "variation-title";
    variationTitle.textContent = "Phân loại";
    variationContainer.appendChild(variationTitle);

    // Container chứa các button variant
    let variantButtonsContainer = document.createElement("div");
    variantButtonsContainer.className = "variation-buttons-container";

    variantsList.forEach(variant => {
        let variantBtn = document.createElement("button");
        variantBtn.className = "variation-button";
        variantBtn.setAttribute("data-variant-id", variant.Id);
        variantBtn.textContent = variant.ShortName || variant.Name;

        // Highlight variant đang chọn
        if (variant.Id === sanPhamObject.Id) {
            ApplyVariantHighlight(variantBtn);
        }

        variantBtn.addEventListener("click", function() {
            VariantClick(variant.Id);
        });

        variantBtn.addEventListener("mouseenter", function() {
            let currentVariantId = parseInt(this.getAttribute("data-variant-id"));
            if (currentVariantId !== sanPhamObject.Id) {
                this.style.borderColor = "rgba(255, 0, 0, 0.5)";
            }
        });

        variantBtn.addEventListener("mouseleave", function() {
            let currentVariantId = parseInt(this.getAttribute("data-variant-id"));
            if (currentVariantId !== sanPhamObject.Id) {
                this.style.borderColor = "rgba(0, 0, 0, .09)";
            }
        });

        variantButtonsContainer.appendChild(variantBtn);
    });

    variationContainer.appendChild(variantButtonsContainer);
}

// Xử lý khi click chọn variant
async function VariantClick(variantId) {
    if (sanPhamObject.Id === variantId) {
        return; // Đã chọn rồi, không làm gì
    }

    // Tìm variant object
    sanPhamObject = variantsList.find(v => v.Id === variantId);
    if (!sanPhamObject) return;

    // Cập nhật URL không reload page (dùng replaceState để không tạo history mới)
    let newSlug = GenerateSlug(sanPhamObject.Name) + "-" + variantId;
    let newUrl = "/San-Pham/" + newSlug;
    window.history.replaceState({ sanPhamId: variantId }, sanPhamObject.Name, newUrl);
    document.title = sanPhamObject.Name;

    // Cập nhật tên sản phẩm
    document.getElementById("item-name-h1").textContent = sanPhamObject.Name;

    // Cập nhật giá và tồn kho
    ShowPriceAndReadyMaxQuantity();

    // Cập nhật thông tin chi tiết (specs table)
    ShowProductSpecifications();

    // Cập nhật mô tả sản phẩm
    ShowProductDescription();

    // Update highlight variant đang chọn (không re-render toàn bộ)
    HighlightSelectedVariant(variantId);

    if (sanPhamObject.MediaList == null || sanPhamObject.MediaList.length == 0) {
        // Load ảnh của variant mới
        ShowCircleLoader();
        sanPhamObject.MediaList = await LoadMediaList(variantId);
        RemoveCircleLoader();
    }

    // Re-render media gallery
    if (sanPhamObject.MediaList.length > 0) {
        selectedIndex = 0;
        ShowMediumItemFromIndex(selectedIndex);
    }
    ShowSmallItem();
}

// Tăng số lượng
function Increase() {

    if (quantityInput < sanPhamObject.Quantity) {
        quantityInput++;
        document.getElementById("quantity-input").value = quantityInput;
    }
}

// Giảm số lượng
function Decrease() {
    if (quantityInput > 1) {
        quantityInput--;
        document.getElementById("quantity-input").value = quantityInput;
    }
}

// Validate input số lượng
function ValidateInput(event) {
    let value = event.target.value;
    let numValue = parseInt(value);

    if (isNaN(numValue) || numValue < 1) {
        quantityInput = 1;
        event.target.value = 1;
    }
    else {
        if (numValue > sanPhamObject.Quantity) {
            quantityInput = maxQuantity;
            event.target.value = maxQuantity;
        }
        else {
            quantityInput = numValue;
        }
    }
}

// Thêm vào giỏ hàng
function AddToCart() {
    // TODO: Implement add to cart logic
    alert("Thêm vào giỏ hàng: " + sanPhamObject.Name + " x " + quantityInput);
}

// Mua ngay
function BuyNow() {
    // TODO: Implement buy now logic
    alert("Mua ngay: " + sanPhamObject.Name + " x " + quantityInput);
}

// Load sản phẩm khi page load
window.addEventListener('DOMContentLoaded', function() {
    HomePageShowSanPham();
});
