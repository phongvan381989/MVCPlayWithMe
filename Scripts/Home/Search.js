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

// ============================================
// SEO: Dynamic Page Title
// ============================================
/**
 * Cập nhật page title dựa trên keyword search (tối ưu SEO)
 * - Trang chủ: "Tiệm sách Voi bé nhỏ"
 * - Search: "Tìm kiếm {keyword} | Tiệm sách Voi bé nhỏ"
 */
function UpdatePageTitle_H1() {
    const keyword = currentSearchParams.keyword || '';

    if (keyword.trim() !== '') {
        // Search results - Focus vào keyword cho SEO
        document.title = `Tìm kiếm "${keyword.trim()}" | ${titleVoiBeNho}`;
    } else {
        // Trang chủ
        document.title = titleVoiBeNho;
    }

    // Update H1 dynamically
    const h1 = document.getElementById("page-title");
    if (h1) {
        if (keyword) {
            h1.textContent = `Kết quả tìm kiếm: "${keyword}"`;
        } else {
            h1.textContent = titleVoiBeNho;
        }
    }
}

async function LoadAndRenderSearchingResultCore(searchParams) {
    ShowCircleLoader();
    let table = document.getElementById("biggestContainer_body_wraper_item");
    table.innerHTML = "";
    try {
        let response = await fetch("/Home/HomeSearch?" + searchParams.toString());
        let responseText = await response.text();
        let result = JSON.parse(responseText);

        if (result.State !== 0) {
            console.error("Load more failed:", result.Message);
            CreateMustClickOkModal("Có lỗi xảy ra, vui lòng thử lại sau.");
            return;
        }

        let items = result.myJson.lsSearch || [];
        console.log("LoadAndRenderSearchingResultCore call - items: " + JSON.stringify(items));

        if (items.length > 0) {
            // ✅ Lấy template element (HTML5 <template>)
            let template = document.getElementById("product-card-template");

            for (let i = 0; i < items.length; i++) {
                let item = items[i];
                let itemElement = CreateProductCard(item, template);
                table.appendChild(itemElement);
            }

            // Update state
            loadedCount += items.length;
            hasMore = result.myJson.hasMore || false;
            lastId = items[items.length - 1].Id;
        }

        if (loadedCount > 0) {
            DisplayEmptyResult(false);
            UpdateLoadMoreUI();
        }
        else {
            DisplayEmptyResult(true);
        }

    } catch (error) {
        console.error("search error:", error);
        CreateMustClickOkModal("Có lỗi xảy ra, vui lòng thử lại sau.");
        DisplayEmptyResult(true);
    } finally {
        RemoveCircleLoader();
    }
}

// ============================================
// Initial Load - 30 items (or load to specific page from URL)
// ============================================
async function Search(checkHasServerData) {
    // Reset state
    lastId = 0;
    loadedCount = 0;
    hasMore = false;

    // Get search parameters từ URL
    SetSearchParametersFromUrl();

    // Get target page from URL
    const targetPage = parseInt(GetValueFromUrlName("page")) || 1;

    // ✅ SSR: HTML đã render sẵn ở server, chỉ cần update state
    const hasServerData = window.serverData &&
                          typeof window.serverData.loadedCount === 'number';

    console.log("Search call - window.serverData: " + JSON.stringify(window.serverData));
    console.log("Search call - checkHasServerData: " + JSON.stringify(checkHasServerData));
    if (hasServerData && checkHasServerData) {
        // ✅ Server đã render HTML sẵn, chỉ cần update state từ metadata
        loadedCount = window.serverData.loadedCount;
        hasMore = window.serverData.hasMore;
        lastId = window.serverData.lastId;

        if (loadedCount === 0) {
            DisplayEmptyResult(true);
            return;
        }

        if (DEBUG) {
            console.log("✓ SSR HTML:", loadedCount, "products already rendered (page", window.serverData.currentPage, ")");
        }

        // ✅ Auto Load More nếu targetPage > loaded pages (e.g., page 8 nhưng chỉ load 5)
        const loadedPages = Math.ceil(loadedCount / ITEMS_PER_PAGE);
        if (targetPage > loadedPages && hasMore) {
            // Fetch phần còn thiếu (page 6, 7, 8...)
            await AutoLoadRemainingPages(targetPage, loadedPages);
        }

        // ✅ Auto scroll to target page position (sau khi load xong)
        if (targetPage > 1) {
            ScrollToPagePosition(targetPage);
        }

        UpdateLoadMoreUI();
    }
    else {
        console.log("Load data SPA");
        // Update page title, h1 cho SEO
        UpdatePageTitle_H1();

        // Gọi API: Load tất cả items từ page 1 đến target page (1 lần gọi duy nhất)
        const searchParams = new URLSearchParams();
        SetNewSearchParametersFromCurrent(searchParams);

        if (targetPage > 1) {
            // Load all items up to target page
            searchParams.append("page", targetPage.toString());
        } else {
            // Page 1: normal initial load
            searchParams.append("limit", ITEMS_PER_PAGE.toString());
        }

        await LoadAndRenderSearchingResultCore(searchParams);

        // ✅ Auto scroll to target page position (sau khi load xong)
        if (targetPage > 1 && loadedCount > 0) {
            ScrollToPagePosition(targetPage);
        }
    }
}

// ============================================
// Auto Load Remaining Pages (cho targetPage > SSR page)
// ============================================
/**
 * Auto fetch items còn thiếu khi targetPage > loaded pages
 * VD: URL /?page=8, SSR load 150 items (page 1-5)
 * → Fetch thêm 90 items (page 6-8) trong 1 lần
 */
async function AutoLoadRemainingPages(targetPage, loadedPages) {
    if (isLoading || !hasMore) {
        return;
    }

    const remainingPages = targetPage - loadedPages;
    const remainingItems = remainingPages * ITEMS_PER_PAGE;

    if (DEBUG) {
        console.log(`Auto loading ${remainingPages} more pages (${remainingItems} items)...`);
    }

    isLoading = true;
    ShowCircleLoader();

    try {
        // ✅ Fetch tất cả items còn thiếu trong 1 lần (thay vì gọi nhiều lần)
        const searchParams = new URLSearchParams();
        SetNewSearchParametersFromCurrent(searchParams);
        searchParams.append("lastId", lastId.toString());
        searchParams.append("limit", remainingItems.toString());  // VD: 90 items cho page 6-8

        let response = await fetch("/Home/HomeSearch?" + searchParams.toString());
        let responseText = await response.text();
        let result = JSON.parse(responseText);

        if (result.State !== 0) {
            console.error("Auto load remaining pages failed:", result.Message);
            // Không show error modal, vì user vẫn thấy 150 items đầu
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

            if (DEBUG) {
                console.log(`✓ Auto loaded ${items.length} items, total: ${loadedCount}`);
            }
        }

    } catch (error) {
        console.error("Auto load remaining pages error:", error);
        // Không show error modal, user vẫn có 150 items
    } finally {
        RemoveCircleLoader();
        isLoading = false;
    }
}

// ============================================
// Load More
// ============================================
async function LoadMore() {
    if (isLoading || !hasMore) {
        return;
    }

    isLoading = true;
    btnLoadMore.disabled = true;

    ShowCircleLoader();

    try {
        // Gọi API: load thêm items với lastId
        const searchParams = new URLSearchParams();
        SetNewSearchParametersFromCurrent(searchParams);
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

            UpdateLoadMoreUI();
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
    SetNewSearchParametersFromCurrent(searchParams);
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
function UpdateLoadMoreUI() {
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

function AppendItems(listItem) {
    let table = document.getElementById("biggestContainer_body_wraper_item");
    // ✅ Lấy template element (HTML5 <template>)
    let template = document.getElementById("product-card-template");

    for (let i = 0; i < listItem.length; i++) {
        let item = listItem[i];
        let itemElement = CreateProductCard(item, template);
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

function CreateProductCard(item, template) {
    // ✅ Clone từ template.content (DocumentFragment)
    let clone = template.content.cloneNode(true);

    // ⚠️ Vì clone là DocumentFragment, phải lấy <article> element để set attributes
    let itemElement = clone.querySelector("article");

    // Set link chi tiết sản phẩm
    clone.querySelector(".product-item").href = GenerateSanPhamUrlForCustomer(item.Name, item.Id);

    // Set ảnh
    let imgElement = clone.querySelector(".card-img-top");
    if (item.CoverImageFileName) {
        imgElement.src = Get320VersionOfImageSrc(GetSanPhamMediaUrl(item.Id, item.CoverImageFileName));
        // Alt text: ưu tiên AltText, fallback sang "Bìa sách [tên]"
        imgElement.alt = item.CoverImageAltText || ("Bìa sách " + item.Name);
        // Title: tooltip khi hover
        imgElement.title = item.CoverImageTitle || item.Name;

        // ✅ Set dimensions để prevent CLS (tính từ kích thước gốc)
        if (item.CoverImageWidth && item.CoverImageHeight) {
            const width = 320;
            const height = Math.round(item.CoverImageHeight * (320 / item.CoverImageWidth));
            imgElement.width = width;
            imgElement.height = height;

            console.log(`✓ Set dimensions: ${width}×${height} (from ${item.CoverImageWidth}×${item.CoverImageHeight})`);
        } else {
            // Fallback: aspect ratio 2:3
            imgElement.width = 320;
            imgElement.height = 480;
            if (DEBUG) {
                console.warn(`⚠ Missing dimensions for ${item.Name}, using fallback 320×480`);
            }
        }

        // Lazy loading: browser tự động load ảnh trong viewport ngay, defer ảnh ngoài viewport
        imgElement.loading = "lazy";
    } else {
        imgElement.src = srcNoImageThumbnail;
        imgElement.alt = "Ảnh sách " + item.Name + " đang cập nhật";
        imgElement.title = item.Name;
        imgElement.width = 320;
        imgElement.height = 480; // Fallback 2:3
    }

    // Set tên
    clone.querySelector(".product-name-h3").innerHTML = item.Name;

    // Set giá
    clone.querySelector(".price-sell-detail").innerHTML =
        ConvertMoneyToTextWithIcon(item.SalePrice);
    if (item.BookCoverPrice > item.SalePrice) {
        clone.querySelector(".price-original-detail").innerHTML =
            ConvertMoneyToTextWithIcon(item.BookCoverPrice);
        clone.querySelector(".price-discount-percent-detail").innerHTML =
            "-" + CalculateDiscountPercent(item.BookCoverPrice, item.SalePrice) + "%";
    }

    // ✅ Return DocumentFragment (appendChild sẽ chỉ thêm children vào DOM)
    return clone;
}

function DisplayEmptyResult(isEmpty) {
    if (isEmpty) {
        document.getElementById("empty-result").style.display = "flex";  // Changed to flex for centering
        document.getElementById("search-result").style.display = "none";
    }
    else {
        document.getElementById("empty-result").style.display = "none";  // Changed to flex for centering
        document.getElementById("search-result").style.display = "block";
    }
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

function SetNewSearchParametersFromCurrent(searchParams) {
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

function EmptyCurrentSearchParams() {
    if (currentSearchParams.keyword) {
        currentSearchParams.keyword = '';
    }
    if (currentSearchParams.author) {
        currentSearchParams.author = '';
    }
    if (currentSearchParams.translator) {
        currentSearchParams.translator = '';
    }
    if (currentSearchParams.category) {
        currentSearchParams.category = '';
    }
    if (currentSearchParams.publishingCompany) {
        currentSearchParams.publishingCompany = '';
    }
    if (currentSearchParams.publisher) {
        currentSearchParams.publisher = '';
    }
}

// ============================================
// Search Button Click
// ============================================
async function HomeSearch() {
    // Update search parameters from input
    EmptyCurrentSearchParams();
    currentSearchParams.keyword = inputSearch.value || "";

    // Update URL
    const searchParams = new URLSearchParams();
    SetNewSearchParametersFromCurrent(searchParams);
    searchParams.append("page", 1);

    window.history.pushState(
        { keyword: currentSearchParams.keyword },
        "",
        "/Home/Search?" + searchParams.toString()
    );

    UpdatePageTitle_H1();

    // Reset state
    lastId = 0;
    loadedCount = 0;
    hasMore = false;

    searchParams.append("lastId", lastId.toString());
    searchParams.append("limit", ITEMS_PER_PAGE.toString());
    await LoadAndRenderSearchingResultCore(searchParams);

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
window.addEventListener("popstate", async (e) => {
    console.log("popstate call ");
    console.log("event.persisted: " + event.persisted);
    await Search(false);

});

// Initial load khi page load
window.addEventListener('DOMContentLoaded', async function () {
    console.log("DOMContentLoaded call ");
    await Search(true);
});
