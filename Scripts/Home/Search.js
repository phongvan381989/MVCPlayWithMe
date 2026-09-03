// ============================================
// Load More Pattern - Keyset Pagination
// ============================================

// Constants
const ITEMS_PER_PAGE = 30;  // Load 30 items mỗi lần (initial và load more)

// State
let lastId = 0;           // Cursor: Id của item cuối cùng đã load
let loadedCount = 0;      // Số sản phẩm đã load
let isLoading = false;    // Prevent double-click
let hasMore = false;      // Còn items để load không

// Search parameters (từ input fields)
let currentSearchParams = {
    keyword: '',
    author: '',
    translator: '',
    category: '',              // Slug: "sach-thieu-nhi"
    publishingCompany: '',
    publisher: ''              // Slug: "kim-dong"
};

// DOM elements
let inputSearch = document.getElementById("search-input-text-id");
let btnLoadMore = document.getElementById("btnLoadMore");
let btnText = document.getElementById("btn-text");
let endMessage = document.getElementById("end-message");
let loadMoreSection = document.getElementById("load-more-section");

// ============================================
// Initial Load - 30 items (or load to specific page from URL)
// ============================================
async function Search() {
    // Reset state
    lastId = 0;
    loadedCount = 0;
    hasMore = false;

    // Get search parameters từ URL
    SetSearchParametersFromUrl();

    // Update H1 dynamically
    const keyword = currentSearchParams.keyword || "";
    const h1 = document.getElementById("page-title");
    if (h1) {
        if (keyword) {
            h1.textContent = `Kết quả tìm kiếm: "${keyword}"`;
        } else {
            h1.textContent = "Tiệm sách voi bé nhỏ";
        }
    }

    // Get target page from URL
    const targetPage = parseInt(GetValueFromUrlName("page")) || 1;

    // Clear grid
    document.getElementById("biggestContainer_body_wraper_item").innerHTML = "";

    // Hide/show UI
    document.getElementById("empty-result").style.display = "none";
    document.getElementById("search-result").style.display = "none";
    loadMoreSection.style.display = "none";

    // Show loading
    ShowCircleLoader();

    try {
        // Gọi API: Load tất cả items từ page 1 đến target page (1 lần gọi duy nhất)
        const searchParams = new URLSearchParams();
        SetSearchParametersToUrlParams(searchParams);

        if (targetPage > 1) {
            // Load all items up to target page
            searchParams.append("page", targetPage.toString());
        } else {
            // Page 1: normal initial load
            searchParams.append("limit", ITEMS_PER_PAGE.toString());
        }

        let response = await fetch("/Home/HomeSearch?" + searchParams.toString());
        let responseText = await response.text();
        let result = JSON.parse(responseText);

        RemoveCircleLoader();

        if (result.State !== 0) {
            CreateMustClickOkModal("Có lỗi xảy ra, vui lòng thử lại sau.");
            EmptySomething();
            return;
        }

        let items = result.myJson.lsSearch || [];

        if (!items || items.length === 0) {
            EmptySomething();
            return;
        }

        // Update state
        loadedCount = result.myJson.loadedCount || items.length;
        hasMore = result.myJson.hasMore || false;

        if (items.length > 0) {
            lastId = items[items.length - 1].Id;
        }

        // Show results
        document.getElementById("search-result").style.display = "block";
        ShowSearchingResult(items);

        // Update progress UI
        UpdateProgressUI();

        // Auto scroll to target page position
        if (targetPage > 1) {
            ScrollToPagePosition(targetPage);
        }

    } catch (error) {
        console.error("Search error:", error);
        RemoveCircleLoader();
        CreateMustClickOkModal("Có lỗi xảy ra, vui lòng thử lại sau.");
        EmptySomething();
    }
}

// ============================================
// Load More - 20 items
// ============================================
async function LoadMore() {
    if (isLoading || !hasMore) {
        return;
    }

    // Check if keyword changed in input box
    const currentKeywordInInput = inputSearch ? (inputSearch.value || "") : "";
    if (currentKeywordInInput !== currentSearchParams.keyword) {
        // Keyword changed - trigger new search instead of load more
        await HomeSearch();
        return;
    }

    isLoading = true;
    btnLoadMore.disabled = true;

    ShowCircleLoader();

    try {
        // Gọi API: load thêm items với lastId
        const searchParams = new URLSearchParams();
        SetSearchParametersToUrlParams(searchParams);
        searchParams.append("lastId", lastId.toString());
        searchParams.append("limit", ITEMS_PER_PAGE.toString());

        let response = await fetch("/Home/HomeSearch?" + searchParams.toString());
        let responseText = await response.text();
        let result = JSON.parse(responseText);

        if (result.State !== 0) {
            console.error("Load more failed:", result.Message);
            CreateMustClickOkModal("Có lỗi xảy ra, vui lòng thử lại sau.");
            return;
        }

        let items = result.myJson.lsSearch || [];

        if (items.length > 0) {
            // Append items vào grid
            AppendItems(items);

            // Update state
            loadedCount += items.length;
            hasMore = result.myJson.hasMore || false;
            lastId = items[items.length - 1].Id;

            // Update URL with current page
            const currentPage = Math.ceil(loadedCount / ITEMS_PER_PAGE);
            UpdateURLWithPage(currentPage);

            // Update progress UI
            UpdateProgressUI();
        }

    } catch (error) {
        console.error("Load more error:", error);
        CreateMustClickOkModal("Có lỗi xảy ra, vui lòng thử lại sau.");
    } finally {
        RemoveCircleLoader();
        isLoading = false;
        btnLoadMore.disabled = false;
    }
}

// ============================================
// Helper Functions - URL & Scroll
// ============================================
function UpdateURLWithPage(page) {
    const searchParams = new URLSearchParams();
    SetSearchParametersToUrlParams(searchParams);
    searchParams.append("page", page.toString());

    window.history.replaceState(
        { keyword: currentSearchParams.keyword, page: page },
        "",
        "/Home/Search?" + searchParams.toString()
    );
}

function ScrollToPagePosition(page) {
    // Page 1: items 0-29
    // Page 2: items 30-59 → scroll to item #30
    // Page 3: items 60-89 → scroll to item #60
    // Page 4: items 90-119 → scroll to item #90

    // First item index of page n = (n - 1) × ITEMS_PER_PAGE
    const firstItemIndex = (page - 1) * ITEMS_PER_PAGE;

    const grid = document.getElementById("biggestContainer_body_wraper_item");
    const items = grid.children;

    if (items[firstItemIndex]) {
        setTimeout(() => {
            items[firstItemIndex].scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
            console.log(`Scrolled to page ${page}, item index ${firstItemIndex}`);
        }, 300);  // Delay để DOM render xong
    }
}

// ============================================
// UI Updates
// ============================================
function UpdateProgressUI() {
    // Show load more section
    loadMoreSection.style.display = "block";

    if (hasMore) {
        // Còn items → show button
        btnLoadMore.style.display = "inline-block";
        endMessage.style.display = "none";
    } else {
        // Hết items → show end message
        btnLoadMore.style.display = "none";
        endMessage.style.display = "block";
    }
}

function ShowSearchingResult(listItem) {
    let table = document.getElementById("biggestContainer_body_wraper_item");
    let sample = document.getElementsByClassName("product-for-selector-sample")[0];

    for (let i = 0; i < listItem.length; i++) {
        let item = listItem[i];
        let itemElement = CreateProductCard(item, sample);
        table.appendChild(itemElement);
    }
}

function AppendItems(listItem) {
    let table = document.getElementById("biggestContainer_body_wraper_item");
    let sample = document.getElementsByClassName("product-for-selector-sample")[0];

    for (let i = 0; i < listItem.length; i++) {
        let item = listItem[i];
        let itemElement = CreateProductCard(item, sample);
        table.appendChild(itemElement);
    }

    // Scroll to first new item (smooth UX)
    if (listItem.length > 0) {
        let firstNewItem = table.lastElementChild;
        for (let i = 0; i < listItem.length - 1; i++) {
            firstNewItem = firstNewItem.previousElementSibling;
        }
        firstNewItem.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
    }
}

function CreateProductCard(item, sample) {
    let itemElement = sample.cloneNode(true);

    // Set link chi tiết sản phẩm
    itemElement.getElementsByClassName("product-item")[0].href = "/san-pham/" + GenerateSlugId(item.Name, item.Id);

    // Hiển thị vì sample đang ẩn
    itemElement.style.display = "block";

    // Set ảnh
    let imgElement = itemElement.getElementsByClassName("card-img-top")[0];
    if (item.CoverImageFileName) {
        imgElement.src = Get320VersionOfImageSrc(GetSanPhamMediaUrl(item.Id, item.CoverImageFileName));
        // Alt text: ưu tiên AltText, fallback sang "Bìa sách [tên]"
        imgElement.alt = item.CoverImageAltText || ("Bìa sách " + item.Name);
        // Title: tooltip khi hover
        imgElement.title = item.CoverImageTitle || item.Name;
        // Lazy loading: browser tự động load ảnh trong viewport ngay, defer ảnh ngoài viewport
        imgElement.loading = "lazy";
    } else {
        imgElement.src = srcNoImageThumbnail;
        imgElement.alt = "Ảnh sách " + item.Name + " đang cập nhật";
        imgElement.title = item.Name;
    }

    // Set tên
    itemElement.getElementsByClassName("product-name-h3")[0].innerHTML = item.Name;

    // Set giá
    itemElement.getElementsByClassName("price-sell-detail")[0].innerHTML =
        ConvertMoneyToTextWithIcon(item.SalePrice);
    if (item.BookCoverPrice > item.SalePrice) {
        itemElement.getElementsByClassName("price-original-detail")[0].innerHTML =
            ConvertMoneyToTextWithIcon(item.BookCoverPrice);
        itemElement.getElementsByClassName("price-discount-percent-detail")[0].innerHTML =
            "-" + CalculateDiscountPercent(item.BookCoverPrice, item.SalePrice) + "%";
    }

    return itemElement;
}

function EmptySomething() {
    document.getElementById("empty-result").style.display = "flex";  // Changed to flex for centering
    document.getElementById("search-result").style.display = "none";
    loadMoreSection.style.display = "none";
}

// ============================================
// Search Parameters
// ============================================
function SetSearchParametersFromUrl() {
    // Optimize: chỉ parse URL 1 lần thay vì 6 lần
    const urlParams = new URLSearchParams(window.location.search);
    if (DEBUG) {
        console.log("SetSearchParametersFromUrl CALL");
        console.log("urlParams: " + urlParams);
    }

    currentSearchParams.keyword = urlParams.get("keyword") || "";
    currentSearchParams.author = urlParams.get("author") || "";
    currentSearchParams.translator = urlParams.get("translator") || "";
    currentSearchParams.publishingCompany = urlParams.get("publishingCompany") || "";
    currentSearchParams.category = urlParams.get("category") || null;
    currentSearchParams.publisher = urlParams.get("publisher") || null;
    if (DEBUG) {
        console.log("currentSearchParams: " + JSON.stringify(currentSearchParams));
    }
    // Set input field value
    if (inputSearch) {
        inputSearch.value = currentSearchParams.keyword;
    }
}

function SetSearchParametersToUrlParams(searchParams) {
    if (currentSearchParams.keyword) {
        searchParams.append("keyword", currentSearchParams.keyword);
    }
    if (currentSearchParams.author) {
        searchParams.append("author", currentSearchParams.author);
    }
    if (currentSearchParams.translator) {
        searchParams.append("translator", currentSearchParams.translator);
    }
    if (currentSearchParams.category) {
        searchParams.append("category", currentSearchParams.category);
    }
    if (currentSearchParams.publishingCompany) {
        searchParams.append("publishingCompany", currentSearchParams.publishingCompany);
    }
    if (currentSearchParams.publisher) {
        searchParams.append("publisher", currentSearchParams.publisher);
    }
}

// ============================================
// Search Button Click
// ============================================
async function HomeSearch() {
    // Update search parameters from input
    currentSearchParams.keyword = inputSearch.value || "";

    // Update URL (không reload page, không có page parameter = page 1)
    const searchParams = new URLSearchParams();
    SetSearchParametersToUrlParams(searchParams);

    window.history.pushState(
        { keyword: currentSearchParams.keyword },
        "",
        "/Home/Search?" + searchParams.toString()
    );

    // Trigger new search (will load page 1)
    await Search();
}

// ============================================
// Event Listeners
// ============================================

// Enter key trong search box
if (inputSearch) {
    inputSearch.addEventListener("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            HomeSearch();
        }
    });
}

// Load More button
if (btnLoadMore) {
    btnLoadMore.addEventListener("click", LoadMore);
}

// Browser back/forward
window.addEventListener("popstate", (e) => {
    if (e.state) {
        Search();
    }
});

// Initial load khi page load
window.addEventListener('DOMContentLoaded', async function () {
    await Search();
});
