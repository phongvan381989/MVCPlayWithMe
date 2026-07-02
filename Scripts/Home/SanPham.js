// Global variables
let countOfSmallMedia; // Số lượng media gồm video + image
let selectedIndex; // selected index media đang chọn hiện tại from listItemData
let listItemData; // Mảng media, phần tử đầu tiên là video nếu có, sau là ảnh
let sanPhamObject; // object sản phẩm server trả về
let variantsList; // Danh sách variants (sản phẩm cùng ComboId)
let selectedSanPhamId; // Id sản phẩm đang chọn hiện tại
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

async function GetSanPhamFromId(id) {
    const searchParams = new URLSearchParams();
    searchParams.append("id", id);

    let query = "/Home/GetSanPhamFromId";

    return RequestHttpPostPromise(searchParams, query);
}

// Lấy đường dẫn media folder của sản phẩm
function GetMediaFolderPath(sanPhamId) {
    return "/Media/Product/" + sanPhamId + "/";
}

// Lấy danh sách ảnh/video từ server folder
async function LoadMediaList(sanPhamId) {
    const searchParams = new URLSearchParams();
    searchParams.append("sanPhamId", sanPhamId);

    let query = "/Home/GetSanPhamMediaList";

    try {
        let response = await RequestHttpPostPromise(searchParams, query);
        let mediaFiles = JSON.parse(response.responseText);

        let mediaList = [];
        for (let filePath of mediaFiles) {
            let extension = filePath.substring(filePath.lastIndexOf('.')).toLowerCase();
            let isVideo = (extension === '.mp4' || extension === '.avi' || extension === '.webm' || extension === '.mov');
            mediaList.push(new objMediaSrc(filePath, isVideo));
        }

        return mediaList;
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
    let responseDB = await GetSanPhamFromId(currentId);
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

        selectedSanPhamId = sanPhamObject.Id;
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

    // Load media list từ server
    listItemData = await LoadMediaList(sanPhamObject.Id);

    // Nếu không có ảnh nào, thêm placeholder
    if (listItemData.length === 0) {
        listItemData.push(new objMediaSrc("/Media/NoImageThumbnail.webp", false));
    }

    countOfSmallMedia = listItemData.length;

    document.title = sanPhamObject.Name;

    ShowSmallItem();
    ShowRightLeftArrow();

    // Chọn item đầu tiên
    if (listItemData.length > 0) {
        ShowMediumMediaFromSelectedSmallItem(document.getElementById("item-small-media-container").children[0]);
    }

    ShowItemSomething();

    // Hiển thị mô tả chi tiết sản phẩm
    ShowProductDescription();
}

function CreateContainerSmallItem(ItemData, i) {
    const container = document.createElement("div");
    container.className = "small-media";
    container.setAttribute("data-index", i);

    if (ItemData.isVideo) {
        const video = document.createElement("video");
        video.src = ItemData.src;
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
        const img = document.createElement("img");
        img.src = Get320VersionOfImageSrc(ItemData.src);
        img.style.verticalAlign = "baseline";
        img.onerror = function() {
            // Nếu không load được ảnh 320, dùng ảnh gốc
            this.src = ItemData.src;
        };
        container.appendChild(img);
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

    for (let i = 0; i < listItemData.length; i++) {
        itemSmallMediaContainer.appendChild(CreateContainerSmallItem(listItemData[i], i));
    }
}

function ShowRightLeftArrow() {
    // Add mũi tên di chuyển sang trái
    let leftArrow = document.getElementById("left_arrow");
    leftArrow.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" version="1.1" height="100" width="40" fill-opacity="0.4"><polygon points="10,50 30,30 30,70" style="fill:lime;" /></svg>';
    leftArrow.addEventListener("click", function () {
        if (selectedIndex == 0) {
            selectedIndex = listItemData.length - 1;
        }
        else {
            selectedIndex--;
        }
        ShowMediumItemFromIndex(selectedIndex);
        ScrollToSelectedSmallItem();
    });

    // Add mũi tên di chuyển sang phải
    let rightArrow = document.getElementById("right_arrow");
    rightArrow.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" version="1.1" height="100" width="40" fill-opacity="0.4"><polygon points="10,30 30,50 10,70" style="fill:lime;" /></svg>';
    rightArrow.addEventListener("click", function () {
        if (selectedIndex == listItemData.length - 1) {
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
    let mediumImage = document.getElementById("medium_image");
    let mediumVideo = document.getElementById("medium_video");

    if (listItemData[i].isVideo) {
        if (isEmptyOrSpaces(mediumVideo.src)) {
            mediumVideo.src = listItemData[i].src;
        }

        mediumVideo.play();
        mediumVideo.style.display = "block";
        mediumVideo.style.left = "0px";

        mediumImage.style.display = "none";
    }
    else {
        mediumImage.src = listItemData[i].src;
        if (mediumVideo.src != null) {
            mediumVideo.pause();
        }
        mediumImage.style.display = "block";
        mediumImage.style.left = "0px";

        mediumVideo.style.display = "none";
    }

    ChangeBorderColorOfSelectedSmallItem();
    document.getElementById("item-medium-media").isCanSwipe = true;
    ShowHideRightLeftArrow();
}

function ShowHideRightLeftArrow() {
    if (document.getElementById("item-medium-media").isCanSwipe) {
        document.getElementById("left_arrow").style.display = "flex";
        document.getElementById("right_arrow").style.display = "flex";
    }
    else {
        document.getElementById("left_arrow").style.display = "none";
        document.getElementById("right_arrow").style.display = "none";
    }
}

function ShowProductDescription() {
    if (sanPhamObject.Detail == null) {
        // Ẩn mô tả sản phẩm
        document.getElementsByClassName("product-detail")[0].style.display = "none";
        return;
    }

    let p = document.createElement("p");
    p.className = "irIKAp";
    p.innerHTML = sanPhamObject.Detail;
    document.getElementsByClassName("f7AU53")[0].appendChild(p);
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

    // Hiển thị phân loại (variants) nếu có
    ShowVariations();
}

// Hiển thị giá bìa, giá bán và % chiết khấu
function ShowPriceAndReadyMaxQuantity() {
    let selectedSanPham = sanPhamObject;

    // Nếu đã chọn variant khác, lấy variant đó
    if (selectedSanPhamId != sanPhamObject.Id && variantsList.length > 0) {
        selectedSanPham = variantsList.find(v => v.Id === selectedSanPhamId) || sanPhamObject;
    }

    // Giá bìa
    document.getElementById("book-cover-price").innerHTML =
        ConvertMoneyToTextWithIcon(selectedSanPham.BookCoverPrice);

    // Giá bán thực tế (giá bìa - discount)
    let sellingPrice = selectedSanPham.BookCoverPrice * (1 - selectedSanPham.Discount);
    document.getElementById("price").innerHTML =
        ConvertMoneyToTextWithIcon(Math.round(sellingPrice));

    // % Chiết khấu
    let discountPercent = Math.round(selectedSanPham.Discount * 100);
    document.getElementById("discount").innerHTML = discountPercent + "% GIẢM";

    // Hiển thị số lượng tồn kho
    document.getElementById("max-quatity").textContent =
        selectedSanPham.Quantity + " sản phẩm có sẵn";
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
    variantButtonsContainer.style.display = "flex";
    variantButtonsContainer.style.flexWrap = "wrap";
    variantButtonsContainer.style.gap = "10px";
    variantButtonsContainer.style.marginTop = "10px";

    variantsList.forEach(variant => {
        let variantBtn = document.createElement("button");
        variantBtn.className = "variant-button";
        variantBtn.setAttribute("data-variant-id", variant.Id);
        variantBtn.textContent = variant.ShortName || variant.Name;
        variantBtn.style.padding = "8px 16px";
        variantBtn.style.border = "1px solid rgba(0, 0, 0, .09)";
        variantBtn.style.borderRadius = "4px";
        variantBtn.style.cursor = "pointer";
        variantBtn.style.background = "white";
        variantBtn.style.transition = "all 0.3s";

        // Highlight variant đang chọn
        if (variant.Id === selectedSanPhamId) {
            variantBtn.style.borderColor = "rgb(255, 0, 0)";
            variantBtn.style.color = "rgb(255, 0, 0)";
        }

        variantBtn.addEventListener("click", function() {
            VariantClick(variant.Id);
        });

        variantBtn.addEventListener("mouseenter", function() {
            if (variant.Id !== selectedSanPhamId) {
                this.style.borderColor = "rgba(255, 0, 0, 0.5)";
            }
        });

        variantBtn.addEventListener("mouseleave", function() {
            if (variant.Id !== selectedSanPhamId) {
                this.style.borderColor = "rgba(0, 0, 0, .09)";
            }
        });

        variantButtonsContainer.appendChild(variantBtn);
    });

    variationContainer.appendChild(variantButtonsContainer);
}

// // Xử lý khi click chọn variant
// async function VariantClick(variantId) {
//     if (selectedSanPhamId === variantId) {
//         return; // Đã chọn rồi, không làm gì
//     }

//     selectedSanPhamId = variantId;

//     // Tìm variant object
//     let selectedVariant = variantsList.find(v => v.Id === variantId);
//     if (!selectedVariant) return;

//     // Cập nhật URL không reload page
//     let newSlug = GenerateSlug(selectedVariant.Name) + "-" + variantId;
//     let newUrl = "/Home/SanPham/" + newSlug;
//     window.history.pushState({sanPhamId: variantId}, selectedVariant.Name, newUrl);
//     document.title = selectedVariant.Name;

//     // Cập nhật tên sản phẩm
//     document.getElementById("item-name-h1").textContent = selectedVariant.Name;

//     // Cập nhật giá
//     ShowPriceAndReadyMaxQuantity();

//     // Re-render variants để update highlight
//     ShowVariations();

//     // Load ảnh của variant mới
//     ShowCircleLoader();
//     listItemData = await LoadMediaList(variantId);
//     if (listItemData.length === 0) {
//         listItemData.push(new objMediaSrc("/Media/NoImageThumbnail.webp", false));
//     }
//     RemoveCircleLoader();

//     // Re-render media gallery
//     selectedIndex = 0;
//     ShowSmallItem();
//     if (listItemData.length > 0) {
//         ShowMediumItemFromIndex(0);
//     }
// }

// Helper function tạo slug từ tên
function GenerateSlug(name) {
    if (!name) return "";

    // Convert Vietnamese to ASCII
    let slug = name.toLowerCase();
    slug = slug.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    slug = slug.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    slug = slug.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    slug = slug.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    slug = slug.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    slug = slug.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    slug = slug.replace(/đ/g, "d");

    // Remove special characters
    slug = slug.replace(/[^a-z0-9\s-]/g, "");

    // Replace spaces with hyphens
    slug = slug.replace(/\s+/g, "-");

    // Remove consecutive hyphens
    slug = slug.replace(/-+/g, "-");

    // Trim hyphens
    slug = slug.replace(/^-+|-+$/g, "");

    return slug;
}

// Tăng số lượng
function Increase() {
    let maxQuantity = sanPhamObject.Quantity;
    if (selectedSanPhamId !== sanPhamObject.Id) {
        let selectedVariant = variantsList.find(v => v.Id === selectedSanPhamId);
        if (selectedVariant) {
            maxQuantity = selectedVariant.Quantity;
        }
    }

    if (quantityInput < maxQuantity) {
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
        let maxQuantity = sanPhamObject.Quantity;
        if (selectedSanPhamId !== sanPhamObject.Id) {
            let selectedVariant = variantsList.find(v => v.Id === selectedSanPhamId);
            if (selectedVariant) {
                maxQuantity = selectedVariant.Quantity;
            }
        }

        if (numValue > maxQuantity) {
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

// Xử lý sự kiện popstate (khi user click back/forward)
window.addEventListener("popstate", function(event) {
    if (event.state && event.state.sanPhamId) {
        // User click back/forward, reload page
        location.reload();
    }
});

// Load sản phẩm khi page load
window.addEventListener('DOMContentLoaded', function() {
    HomePageShowSanPham();
});
