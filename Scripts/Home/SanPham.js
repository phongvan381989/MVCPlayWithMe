// Global variables
let selectedIndex; // selected index media đang chọn hiện tại
let sanPhamObject; // object sản phẩm đang hiển thị
let variantsList; // Danh sách variants (sản phẩm cùng ComboId)
let metadataHasVideo; // true nếu sản phẩm có video, ngược false

let limitQuantity = "Số lượng bạn chọn đã đạt mức tối đa của sản phẩm này";
let dontSelectVariation = "Vui lòng chọn phân loại sản phẩm";

// Get the modal thông báo đã thêm vào giỏ hàng thành công
let modal = document.getElementById("myModal");
// Get the modal nhắc số lượng trong giỏ vượt quá tồn kho
let modalOverMax = document.getElementById("myModal-over-max");

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

// Helper: Preload image để lấy actual dimensions
function preloadImage(src) {
    return new Promise((resolve, reject) => {
        const img = new Image();
        img.onload = () => {
            resolve({
                width: img.naturalWidth,
                height: img.naturalHeight
            });
        };
        img.onerror = () => {
            // Nếu load fail, dùng fallback dimensions
            resolve({ width: 1200, height: 1200 });
        };
        img.src = src;
    });
}

// Cache dataSource (preloaded images) - rebuild khi đổi variant
let photoSwipeDataSource = null;

// Từ thứ tự ảnh trong metadata sinh alt
function GeneraAlt(media, i) {
    let alt = media.AltText;
    if (!media.AltText) {
        alt = sanPhamObject.Name + " - Trang " + (metadataHasVideo ? i : (i + 1)); // Có video thì video có thứ tự i = 0 nên ảnh sẽ từ 1,2,3 ngược lại ảnh sẽ từ 0,1,2
    }
    return alt;
}

// Build dataSource từ DB (không cần preload - đã có Width/Height)
function buildPhotoSwipeDataSource() {
    if (!sanPhamObject || !sanPhamObject.MediaList || sanPhamObject.MediaList.length === 0) {
        return [];
    }

    const dataSource = [];

    if (DEBUG) {
        console.log('Building PhotoSwipe dataSource from DB for', sanPhamObject.MediaList.length, 'media items...');
    }

    //for (const media of sanPhamObject.MediaList)
    for (let i = 0; i < sanPhamObject.MediaList.length; i++)
    {
        let media = sanPhamObject.MediaList[i];
        const mediaSrc = GetSanPhamMediaUrl(sanPhamObject.Id, media.FileName);

        // Dùng kích thước từ DB (fallback 1920x1080 cho data cũ chưa có Width/Height)
        const width = media.Width || 1920;
        const height = media.Height || 1080;

        if (media.MediaType === 'image') {
            // Image: dùng width/height từ DB (không cần preload!)
            if (DEBUG) {
                console.log("Image from DB:", media.FileName, `${width}x${height}`);
            }
            
            dataSource.push({
                src: mediaSrc,
                width: width,
                height: height,
                alt: GeneraAlt(media, i)
            });
        } else {
            // Video: dùng width/height từ DB (kích thước video thực tế)
            if (DEBUG) {
                console.log("Video from DB:", media.FileName, `${width}x${height}`);
            }
            dataSource.push({
                html: `<div style="display:flex;align-items:center;justify-content:center;width:100%;height:100%;">
                         <video controls style="max-width:100%;max-height:100%;object-fit:contain;">
                           <source src="${mediaSrc}" type="video/mp4">
                         </video>
                       </div>`,
                width: width,
                height: height
            });
        }
    }

    if (DEBUG) {
        console.log('✅ DataSource built from DB! PhotoSwipe ready with', dataSource.length, 'items');
    }

    return dataSource;
}

// Build dataSource khi page load / đổi variant
async function initPhotoSwipeLightbox() {
    // Check library có load chưa
    if (typeof PhotoSwipe === 'undefined') {
        console.error('PhotoSwipe library chưa load. Kiểm tra script tags trong SanPham.cshtml');
        return;
    }

    // Build và cache dataSource (từ DB - không cần await)
    photoSwipeDataSource = buildPhotoSwipeDataSource();
}

// Open PhotoSwipe lightbox tại index chỉ định
function openPhotoSwipe(startIndex) {
    if (typeof PhotoSwipe === 'undefined') {
        console.error('PhotoSwipe library chưa load');
        return;
    }

    if (!photoSwipeDataSource || photoSwipeDataSource.length === 0) {
        console.warn('PhotoSwipe dataSource chưa ready');
        return;
    }

    // Init PhotoSwipe trực tiếp (không dùng Lightbox wrapper)
    const pswp = new PhotoSwipe({
        dataSource: photoSwipeDataSource,
        index: startIndex || 0,

        // Zoom options
        maxZoomLevel: 4,           // Cho phép zoom 4x
        initialZoomLevel: 'fit',   // Ban đầu fit viewport
        secondaryZoomLevel: 2,     // Double-tap → zoom 2x

        // UI options
        bgOpacity: 0.9,
        spacing: 0.1,
        loop: true,
        pinchToClose: true,
        closeOnVerticalDrag: true,
        showHideAnimationType: 'zoom'
    });

    // Custom zoom buttons: hình tròn nền trắng + icon SVG kính lúp
    pswp.on('uiRegister', function() {

        const delta = 0.5;
        // Zoom In button (+)
        pswp.ui.registerElement({
            name: 'zoom-in-button',
            order: 10,
            isButton: true,
            html: `<button class="pswp__button pswp__button--zoom-in" title="Zoom in (+)">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="pswp__icn">
  <circle cx="11" cy="11" r="8"></circle>
  <line x1="21" y1="21" x2="16.65" y2="16.65"></line>
  <line x1="11" y1="8" x2="11" y2="14"></line>
  <line x1="8" y1="11" x2="14" y2="11"></line>
</svg>
            </button>`,
            onClick: (e, el, pswp) => {
                const currZoom = pswp.currSlide.currZoomLevel;
                const newZoom = Math.min(Math.ceil(currZoom) + delta, 4);
                pswp.zoomTo(newZoom, { x: pswp.viewportSize.x / 2, y: pswp.viewportSize.y / 2 }, 300);
            }
        });

        // Zoom Out button (-)
        pswp.ui.registerElement({
            name: 'zoom-out-button',
            order: 9,
            isButton: true,
            html: `<button class="pswp__button pswp__button--zoom-out" title="Zoom out (-)">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="pswp__icn">
  <circle cx="11" cy="11" r="8"></circle>
  <line x1="21" y1="21" x2="16.65" y2="16.65"></line>
  <line x1="8" y1="11" x2="14" y2="11"></line>
</svg>

            </button>`,
            onClick: (e, el, pswp) => {
                const currZoom = pswp.currSlide.currZoomLevel;
                const fitZoom = pswp.currSlide.zoomLevels.fit;

                // Chỉ zoom out nếu currZoom > fitZoom (đã zoom in rồi)
                if (currZoom > fitZoom) {
                    const newZoom = Math.max(currZoom -2* delta, fitZoom);
                    pswp.zoomTo(newZoom, { x: pswp.viewportSize.x / 2, y: pswp.viewportSize.y / 2 }, 300);
                }
            }
        });
    });

    pswp.init();

    // Control video playback: auto play khi xem, auto pause khi chuyển slide
    pswp.on('slideActivate', (e) => {
        const video = e.slide.container.querySelector('video');
        if (video) {
            video.play().catch(() => {}); // Ignore autoplay policy errors
        }
    });

    pswp.on('slideDeactivate', (e) => {
        const video = e.slide.container.querySelector('video');
        if (video) {
            video.pause();
        }
    });
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

    ShowRightLeftArrow();

    ShowSmallItem();

    // Chọn item đầu tiên
    if (sanPhamObject.MediaList.length > 0) {
        //ShowMediumMediaFromSelectedSmallItem(document.getElementById("item-small-media-container").children[0]);
        selectedIndex = 0;
        ShowMediumItemFromIndex(selectedIndex);
    }

    // Hiển thị phân loại (variants) nếu có
    ShowVariations();

    ShowItemSomething();

    // Init PhotoSwipeLightbox sau khi MediaList đã load (background preload - không block UI)
    initPhotoSwipeLightbox();  // KHÔNG await → chạy background
}

// Nếu sản phẩm có video thì hasVideo = true, nếu không thì hasVideo = false
function CreateContainerSmallItem(media, i, hasVideo) {
    const container = document.createElement("div");
    container.className = "small-media";
    container.setAttribute("data-index", i);

    // Vì nếu có video thì nó sẽ ở vị trí đầu tiên
    if (i == 0 && hasVideo) {
        // Dùng poster image thay vì video tag (performance tốt hơn)
        const picture = document.createElement("picture");
        picture.style.display = "flex";
        picture.style.justifyContent = "center";
        picture.style.alignItems = "center";
        picture.style.width = "100%";
        picture.style.height = "100%";

        // Poster image: video_poster.webp (320px) từ thư mục _320
        const posterSrc = media.PosterImage
            ? GetSanPhamMediaUrl(sanPhamObject.Id, media.PosterImage)
            : srcNoImageThumbnail;

        const sourceWebp = document.createElement("source");
        sourceWebp.srcset = Get320VersionOfImageSrc(posterSrc);
        sourceWebp.type = "image/webp";

        const img = document.createElement("img");
        img.src = srcNoImageThumbnail;  // Fallback
        img.alt = "Video " + sanPhamObject.Name;
        img.style.objectFit = "contain";
        img.style.maxWidth = "100%";
        img.style.maxHeight = "100%";

        picture.appendChild(sourceWebp);
        picture.appendChild(img);
        container.appendChild(picture);
        container.style.position = "relative";

        // Video icon overlay
        let videoIcon = document.createElement("div");
        videoIcon.style.position = "absolute";
        videoIcon.style.top = "30px";
        videoIcon.style.left = "30px";
        videoIcon.style.width = "40px";
        videoIcon.style.height = "40px";
        videoIcon.style.zIndex = 1;
        videoIcon.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" version="1.1" width="40" height="40" viewBox="0 0 256 256"><g style="stroke: none; stroke-width: 0; fill: none; opacity: 1;" transform="translate(1.4065934065934016 1.4065934065934016) scale(2.81 2.81)"><path d="M 45 0 C 20.147 0 0 20.147 0 45 c 0 24.853 20.147 45 45 45 s 45 -20.147 45 -45 C 90 20.147 69.853 0 45 0 z M 62.251 46.633 L 37.789 60.756 c -1.258 0.726 -2.829 -0.181 -2.829 -1.633 V 30.877 c 0 -1.452 1.572 -2.36 2.829 -1.634 l 24.461 14.123 C 63.508 44.092 63.508 45.907 62.251 46.633 z" style="stroke: none; fill: rgb(0,0,0); opacity: 1;" transform=" matrix(1 0 0 1 0 0) " /></g></svg>';

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
        img.alt = GeneraAlt(media, i);
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
    metadataHasVideo = sanPhamObject.MediaList[0].MediaType != "image"? true:false;
    for (let i = 0; i < sanPhamObject.MediaList.length; i++) {
        itemSmallMediaContainer.appendChild(CreateContainerSmallItem(sanPhamObject.MediaList[i], i, metadataHasVideo));
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
        mediumImage.alt = GeneraAlt(sanPhamObject.MediaList[i], i);

        if (mediumVideo.src != null) {
            mediumVideo.pause();
        }

        mediumPicture.style.display = "flex";
        mediumVideo.style.display = "none";
    }

    ChangeBorderColorOfSelectedSmallItem();
    ScrollToSelectedSmallItem();
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

    // Hiển thị giá, số lượng sản phẩm có thể đặt và reset số lượng đặt về 1
    ShowPriceAndReadyMaxQuantity();

    // Hiển thị thông tin chi tiết (specs table)
    ShowProductSpecifications();

    // Hiển thị mô tả chi tiết sản phẩm
    ShowProductDescription();
}

// Hiển thị giá bìa, giá bán và % chiết khấu
// Disable +/- số lượng, nút thêm vào giỏ hàng, mua hàng
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
    let maxQuantityElement = document.getElementById("max-quatity");
    if (sanPhamObject.Quantity > 0) {
        maxQuantityElement.textContent = sanPhamObject.Quantity + " sản phẩm có sẵn";
        maxQuantityElement.style.color = ""; // Reset về màu mặc định
    } else {
        maxQuantityElement.textContent = "HẾT HÀNG";
        maxQuantityElement.style.color = "#ee4d2d";
    }

    // Reset số lượng đặt về 1
    document.getElementById("quantity-input").value = "1";

    // Disable/Enable controls dựa vào tồn kho
    let btnAddToCart = document.querySelector(".btn-add-to-cart");
    let btnBuyNow = document.querySelector(".btn-buy-now");
    let quantityInput = document.getElementById("quantity-input");
    let quantityButtons = document.querySelectorAll(".shopee-input-quantity .xNxl-t");
    let quantityContainer = document.querySelector(".shopee-input-quantity");

    if (sanPhamObject.Quantity <= 0) {
        // Hết hàng → disable tất cả
        btnAddToCart.disabled = true;
        btnBuyNow.disabled = true;
        quantityInput.disabled = true;
        quantityButtons.forEach(btn => btn.disabled = true);
        quantityContainer.classList.add("disabled");
    } else {
        // Còn hàng → enable tất cả
        btnAddToCart.disabled = false;
        btnBuyNow.disabled = false;
        quantityInput.disabled = false;
        quantityButtons.forEach(btn => btn.disabled = false);
        quantityContainer.classList.remove("disabled");
    }
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
        if (DEBUG) {
            console.log("vãi đái thể nhỉ variant: " + JSON.stringify(variant));
        }
        // Tất cả variation đều click được (kể cả hết hàng)
        variantBtn.addEventListener("click", function () {
            VariantClick(variant.Id);
        });

        // Hết hàng → thêm class để styling khác biệt
        if (variant.Quantity <= 0) {
            variantBtn.classList.add("out-of-stock");
            if (DEBUG) {
                console.log(`Variant ${variant.Name} hết hàng (Quantity: ${variant.Quantity})`);
            }
        } else {
            // Còn hàng → có hover effect
            variantBtn.addEventListener("mouseenter", function () {
                let currentVariantId = parseInt(this.getAttribute("data-variant-id"));
                if (currentVariantId !== sanPhamObject.Id) {
                    this.style.borderColor = "rgba(255, 0, 0, 0.5)";
                }
            });

            variantBtn.addEventListener("mouseleave", function () {
                let currentVariantId = parseInt(this.getAttribute("data-variant-id"));
                if (currentVariantId !== sanPhamObject.Id) {
                    this.style.borderColor = "rgba(0, 0, 0, .09)";
                }
            });
        }

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

    ShowItemSomething();

    // Update highlight variant đang chọn (không re-render toàn bộ)
    HighlightSelectedVariant(variantId);

    if (sanPhamObject.MediaList == null || sanPhamObject.MediaList.length == 0) {
        // Load ảnh của variant mới
        ShowCircleLoader();
        sanPhamObject.MediaList = await LoadMediaList(variantId);
        RemoveCircleLoader();
    }

    ShowSmallItem();

    // Re-render media gallery
    if (sanPhamObject.MediaList.length > 0) {
        selectedIndex = 0;
        ShowMediumItemFromIndex(selectedIndex);
    }

    // Re-init PhotoSwipeLightbox với MediaList mới (background preload - không block UI)
    initPhotoSwipeLightbox();  // KHÔNG await → chạy background
}

// Tăng số lượng
function Increase() {
    let input = document.getElementById("quantity-input");
    let messageDiv = document.querySelector(".I-H1Co");
    let currentValue = parseInt(input.value) || 1;

    if (currentValue < sanPhamObject.Quantity) {
        input.value = currentValue + 1;
        messageDiv.style.display = "none";  // Ẩn thông báo
    } else {
        // Hiển thị thông báo inline
        messageDiv.style.display = "block";
        input.value = sanPhamObject.Quantity;
    }
}

// Giảm số lượng
function Decrease() {
    let input = document.getElementById("quantity-input");
    let messageDiv = document.querySelector(".I-H1Co");
    let currentValue = parseInt(input.value) || 1;

    if (currentValue > 1) {
        input.value = currentValue - 1;
    }
    messageDiv.style.display = "none";  // Ẩn thông báo khi giảm
}

// Validate input số lượng
function ValidateInput(event) {
    let input = event.target;
    let messageDiv = document.querySelector(".I-H1Co");
    let numValue = parseInt(input.value);

    // Kiểm tra input không hợp lệ hoặc < 1
    if (isNaN(numValue) || numValue < 1) {
        input.value = 1;
        messageDiv.style.display = "none";
    }
    // Kiểm tra vượt quá tồn kho
    else if (numValue > sanPhamObject.Quantity) {
        input.value = sanPhamObject.Quantity;
        messageDiv.style.display = "block";  // Hiển thị thông báo
    }
    // Input hợp lệ
    else {
        messageDiv.style.display = "none";   // Ẩn thông báo
    }
}

// Thêm vào giỏ hàng
async function AddToCart() {
    // Lấy số lượng  CheckAnonymousCustomer
    const quantityInput = document.getElementById('quantity-input');
    const quantity = parseInt(quantityInput?.value || 1);

    try {
        // Check user đã login chưa (cookie 'uid')
        const isLoggedIn = !CheckAnonymousCustomer();
        setTimeout(CloseModal, 500);
        if (isLoggedIn) {
            // Khách đã đăng nhập → Thêm vào DB
            await addToCartServer(sanPhamObject.Id, quantity, 0);
        } else {
            // Khách vãng lai → Thêm vào localStorage
            const success = CartManager.addToCart(sanPhamObject.Id, quantity, 0);
        }

    } catch (error) {
        console.error('❌ Error adding to cart:', error);
        CreateMustClickOkModal('Có lỗi khi thêm vào giỏ hàng. Vui lòng thử lại!', null);
    }

    ShowModal();
}

/**
 * Mua ngay - Thêm vào giỏ hàng với real = 1 và chuyển đến trang cart
 */
async function GoToCartNow() {
    // Lấy số lượng
    const quantityInput = document.getElementById('quantity-input');
    const quantity = parseInt(quantityInput?.value || 1);

    try {
        // Check user đã login chưa (cookie 'uid')
        if (!CheckAnonymousCustomer()) {
            await addToCartServer(sanPhamObject.Id, quantity, 0);
        } 

        // Với khách đăng nhập lưu tạm để biết sản phẩm nào được chọn mua tức real = 1
        // real = 1 -> sau gửi lên server check sản phẩm nào được chọn mua ngay cả với khách đăng nhập
        CartManager.addToCart(sanPhamObject.Id, quantity, 1);

        // Chuyển đến trang giỏ hàng
        window.location.href = '/Home/Cart';

    } catch (error) {
        console.error('❌ Error in BuyNow:', error);
        await CreateMustClickOkModal('Có lỗi xảy ra. Vui lòng thử lại!', null);
    }
}

function CloseModal() {
    modal.style.display = "none";
}

function ShowModal() {
    modal.style.display = "flex";
}

/**
 *  Nguyên tắc: real luôn luôn = 0 trong db, sản phẩm nào được chọn trên giao diện sẽ gửi riêng
 * Thêm vào cart trên server (logged-in users)
 * @param {Number} sanPhamId
 * @param {Number} quantity
 */
async function addToCartServer(id, q, r) {
    try {
        const resultText = await PostJSON('/Home/AddSanPhamToCart', {
            sanPhamId: id,
            quantity: q//,
            //real: r
        });
    }
    catch (ex) {
        console.error("add to cart error:", ex);
    }
}

// Load sản phẩm khi page load
window.addEventListener('DOMContentLoaded', async function () {
    InitializeTickOkModal();
    await HomePageShowSanPham();

    // Keyboard navigation cho ảnh sản phẩm
    document.addEventListener('keydown', function(event) {
        // Bỏ qua khi đang focus vào input/textarea
        if (document.activeElement.tagName === 'INPUT' ||
            document.activeElement.tagName === 'TEXTAREA') {
            return;
        }

        // Bỏ qua nếu chưa có sản phẩm hoặc chỉ có 1 ảnh
        if (!sanPhamObject || !sanPhamObject.MediaList ||
            sanPhamObject.MediaList.length <= 1) {
            return;
        }

        if (event.key === 'ArrowLeft') {
            event.preventDefault(); // Ngăn scroll page
            // Logic giống left arrow click
            if (selectedIndex == 0) {
                selectedIndex = sanPhamObject.MediaList.length - 1;
            } else {
                selectedIndex--;
            }
            ShowMediumItemFromIndex(selectedIndex);
        }
        else if (event.key === 'ArrowRight') {
            event.preventDefault(); // Ngăn scroll page
            // Logic giống right arrow click
            if (selectedIndex == sanPhamObject.MediaList.length - 1) {
                selectedIndex = 0;
            } else {
                selectedIndex++;
            }
            ShowMediumItemFromIndex(selectedIndex);
        }
    });

    // Touch/swipe navigation cho mobile
    let touchStartX = 0;
    let touchStartY = 0;
    const swipeThreshold = 50; // Minimum swipe distance in pixels

    const mediumImageContainer = document.getElementById('item-medium-media');
    if (mediumImageContainer) {
        mediumImageContainer.addEventListener('touchstart', function(event) {
            touchStartX = event.changedTouches[0].screenX;
            touchStartY = event.changedTouches[0].screenY;
        }, { passive: true });

        mediumImageContainer.addEventListener('touchend', function(event) {
            // Bỏ qua nếu chưa có sản phẩm hoặc chỉ có 1 ảnh
            if (!sanPhamObject || !sanPhamObject.MediaList ||
                sanPhamObject.MediaList.length <= 1) {
                return;
            }

            const touchEndX = event.changedTouches[0].screenX;
            const touchEndY = event.changedTouches[0].screenY;
            const deltaX = touchEndX - touchStartX;
            const deltaY = touchEndY - touchStartY;

            // Chỉ xử lý swipe ngang (horizontal), bỏ qua swipe dọc (vertical scroll)
            if (Math.abs(deltaX) > Math.abs(deltaY) && Math.abs(deltaX) > swipeThreshold) {
                if (deltaX > 0) {
                    // Swipe right → previous image (giống ArrowLeft)
                    if (selectedIndex == 0) {
                        selectedIndex = sanPhamObject.MediaList.length - 1;
                    } else {
                        selectedIndex--;
                    }
                    ShowMediumItemFromIndex(selectedIndex);
                } else {
                    // Swipe left → next image (giống ArrowRight)
                    if (selectedIndex == sanPhamObject.MediaList.length - 1) {
                        selectedIndex = 0;
                    } else {
                        selectedIndex++;
                    }
                    ShowMediumItemFromIndex(selectedIndex);
                }
            }
        }, { passive: true });
    }

    // PhotoSwipe: Attach click event vào medium image/video để mở lightbox
    const mediumImageEl = document.getElementById('medium_image');
    if (mediumImageEl) {
        mediumImageEl.addEventListener('click', function() {
            openPhotoSwipe(selectedIndex);
        });
    }

    const mediumVideoEl = document.getElementById('medium_video');
    if (mediumVideoEl) {
        mediumVideoEl.addEventListener('click', function() {
            openPhotoSwipe(selectedIndex);
        });
    }
});
