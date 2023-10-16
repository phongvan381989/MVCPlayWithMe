
let namePara;
// số sản phẩm trong kết quả tìm kiếm
let countItem;
// itemOnPage: Số lượng item mỗi trang
let itemOnPage = 20;
// maxPage: Số trang lớn nhất, 1 là trang đầu tiên, maxPage là trang cuối cùng
let maxPage;
// currentPage: Trang hiện tại
let currentPage;

// Làm mới nút di chuyển trang
function RefreshPaging(url) {
    if (DEBUG) {
        console.log("RefreshPaging");
        console.log("url: " + url);
    }
    let wraperPagination = document.getElementsByClassName("wraper-pagination")[0];
    wraperPagination.innerHTML = "";
    // page active ở giữa nếu 2 bên còn nhiều page
    wraperPagination.appendChild(CreateItem(currentPage, true, ChangePage, currentPage, url));

    // add nút phía trước
    let i = currentPage - 1;
    let count = 2;
    do {
        if (i == 0) {
            // Thêm <
            wraperPagination.insertBefore(CreateItem("<", false, ChangePage, currentPage - 1, url), wraperPagination.children[0]);
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
                wraperPagination.insertBefore(CreateItem("...", false, ChangePage, 0, url), wraperPagination.children[0]);
                wraperPagination.insertBefore(CreateItem(1, false, ChangePage, 1, url), wraperPagination.children[0]);
            }
            if (i == 1) {
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
            wraperPagination.insertBefore(CreateItem(">", false, ChangePage, currentPage + 1, url), null);
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
                wraperPagination.insertBefore(CreateItem("...", false, ChangePage, 0, url), null);
                wraperPagination.insertBefore(CreateItem(maxPage, false, ChangePage, maxPage, url), null);
            }
            if (i == maxPage) {
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
    //if (DEBUG) {
    //    console.log("CreateItem");
    //    console.log("textPage: " + textPage);
    //    console.log("url: " + url);
    //}
    let divItem = document.createElement("div");
    if (isCurrentPage == true) {
        divItem.className = "pagination-item active";
    }
    else {
        divItem.className = "pagination-item";
    }
    divItem.onclick = function () {
        ChangePage(goTo, url);
    };

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
    if (page == null || url == null || currentPage == NaN) {
        return;
    }

    if (page == 0 || page == maxPage + 1 || page == currentPage) {
        if (DEBUG) {
            console.log("Dont ChangePage" );
        }
        return;
    }

    // Làm mới hiển thị phân trang
    currentPage = parseInt(page);
    if (DEBUG) {
        console.log("currentPage: " + currentPage);
    }
    RefreshPaging(url);

    //let url = "/ItemModel/ChangePage";
    const searchParams = new URLSearchParams();
    searchParams.append("namePara", namePara);
    searchParams.append("start", (page - 1) * itemOnPage);
    searchParams.append("offset", itemOnPage);
    let resObj = await RequestHttpGetPromise(searchParams, url);

    ShowResult(resObj);
}
