
//let namePara;
// số sản phẩm trong kết quả tìm kiếm
let countItem;
// itemOnPage: Số lượng item mỗi trang
let itemOnPage = 20;
// maxPage: Số trang lớn nhất, 1 là trang đầu tiên, maxPage là trang cuối cùng
let maxPage;
// currentPage: Trang hiện tại
let currentPage;

// Nếu url không thay đổi, không thực hiện search
let loadedUrl;

// Làm mới nút di chuyển trang
function RefreshPaging(url) {
    if (DEBUG) {
        console.log("RefreshPaging");
        console.log("url: " + url);
    }
    let wraperPagination = document.getElementsByClassName("wraper-pagination")[0];
    wraperPagination.style.display = "flex";
    wraperPagination.innerHTML = "";
    // page active ở giữa nếu 2 bên còn nhiều page
    wraperPagination.appendChild(CreateItem(currentPage, true, ChangePage, currentPage, url));

    // add nút phía trước
    let i = currentPage - 1;
    let count = 2;
    do {
        if (i == 0) {
            // Thêm <
            let temp = currentPage - 1;
            if (temp == 0) {
                temp = 1;
            }

            wraperPagination.insertBefore(CreateItem("<", false, ChangePage, temp, url), wraperPagination.children[0]);
            break;
        }
        else {
            wraperPagination.insertBefore(CreateItem(i, false, ChangePage, i, url), wraperPagination.children[0]);
        }

        i = i - 1;
        count = count - 1;
        if (count == 0) {
            if (i > 1) {
                // Thêm ...
                wraperPagination.insertBefore(CreateItem("...", false, null, 0, url), wraperPagination.children[0]);
                wraperPagination.insertBefore(CreateItem(1, false, ChangePage, 1, url), wraperPagination.children[0]);
            }
            else if (i == 1) {
                wraperPagination.insertBefore(CreateItem(i, false, ChangePage, i, url), wraperPagination.children[0]);
            }
            // Thêm <
            wraperPagination.insertBefore(CreateItem("<", false, ChangePage, currentPage - 1, url), wraperPagination.children[0]);
            break;
        }
    }
    while (true);

    // add nút phía sau
    i = currentPage + 1;
    count = 2;
    do {
        if (i == maxPage + 1) {
            // Thêm >
            let temp = currentPage + 1;
            if (temp == maxPage + 1) {
                temp = maxPage;
            }
            wraperPagination.insertBefore(CreateItem(">", false, ChangePage, temp, url), null);
            break;
        }
        else {
            wraperPagination.insertBefore(CreateItem(i, false, ChangePage, i, url), null);
        }

        i = i + 1;
        count = count - 1;
        if (count == 0) {
            if (i < maxPage) {
                // Thêm ...
                wraperPagination.insertBefore(CreateItem("...", false, null, 0, url), null);
                wraperPagination.insertBefore(CreateItem(maxPage, false, ChangePage, maxPage, url), null);
            }
            else if (i == maxPage) {
                wraperPagination.insertBefore(CreateItem(maxPage, false, ChangePage, maxPage, url), null);
            }
            // Thêm >
            wraperPagination.insertBefore(CreateItem(">", false, ChangePage, currentPage + 1, url), null);
            break;
        }
    }
    while (true);
}

// page: là trang muốn đi tới, '<','>' hoặc '...'
function CreateItem(textPage, isCurrentPage, ChangePage, goTo, url) {
    if (DEBUG) {
        console.log("CreateItem CALL");
        console.log("textPage: " + textPage);
        console.log("isCurrentPage: " + isCurrentPage);
        //console.log("ChangePage function: " + ChangePage);
        console.log("goTo: " + goTo);
        console.log("url: " + url);
    }
    let divItem = document.createElement("div");
    if (isCurrentPage == true) {
        divItem.className = "pagination-item active";
    }
    else {
        divItem.className = "pagination-item";
    }
    if (ChangePage != null) {
        divItem.onclick = function () {
            ChangePage(goTo, url);
        };

    }
    let pPage = document.createElement("p");
    pPage.innerHTML = textPage;
    divItem.appendChild(pPage);
    return divItem;
}

async function ChangePage(page, url) {
    if (DEBUG) {
        console.log("ChangePage(" + page + ")");
        console.log("page: " + page);
        console.log("maxPage: " + maxPage);
        console.log("url: " + url);
    }
    const searchParams = new URLSearchParams();
    SetSearchParameter(searchParams);
    searchParams.append("start", (page - 1) * itemOnPage);
    searchParams.append("offset", itemOnPage);

    if (page == null || url == null || currentPage == NaN) {
        return;
    }

    if (loadedUrl == searchParams.toString()) {
        if (DEBUG) {
            console.log("loadedUrl: " + loadedUrl);
            console.log("Dont ChangePage" );
        }
        return;
    }
    loadedUrl = searchParams.toString();
    if (DEBUG) {
        console.log("new value of loadedUrl: " + loadedUrl);
    }
    // Làm mới hiển thị phân trang
    currentPage = parseInt(page);
    if (DEBUG) {
        console.log("currentPage: " + currentPage);
    }
    RefreshPaging(url);

    let resObj = await RequestHttpGetPromise(searchParams, url);

    ShowSearchingResult(resObj);
}

// Ẩn kết quả, paging button, làm trống loadedUrl
function EmptySomethingV1() {
    document.getElementById("empty-result").style.display = "block";
    document.getElementById("search-result").style.display = "none";

    document.getElementsByClassName("wraper-pagination")[0].innerHTML = "";

    loadedUrl = "";
}

function EmptySomethingV2() {
    document.getElementsByClassName("wraper-pagination")[0].innerHTML = "";
    document.getElementsByClassName("wraper-pagination")[0].style.display = "none";

    loadedUrl = "";
}
