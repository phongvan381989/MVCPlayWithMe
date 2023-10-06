
let namePara;
// số sản phẩm trong kết quả tìm kiếm
let countItem;
// itemOnPage: Số lượng item mỗi trang
let itemOnPage = 20;
// maxPage: Số trang lớn nhất, 1 là trang đầu tiên, maxPage là trang cuối cùng
let maxPage;
// currentPage: Trang hiện tại
let currentPage;

// Mục đích lấy số lượng trong kết quả trả về.
async function Search() {
    namePara = document.getElementById("item-model-name").value;

    let url = "/ItemModel/SearchItemCount";
    const searchParams = new URLSearchParams();
    searchParams.append("namePara", namePara);
    let resObj = await RequestHttpGetPromise(searchParams, url);
    if (DEBUG) {
        console.log("responseText: " + resObj.responseText);
    }
    // Lấy số lượng sản phẩm trong kết quả trả về
    countModel = parseInt(resObj.responseText);
    if (countModel == 0) {
        document.getElementById("empty-result").style.display = "block";
        document.getElementById("search-result").style.display = "none";
        return;
    }

    document.getElementById("empty-result").style.display = "none";
    document.getElementById("search-result").style.display = "block";

    maxPage = Math.floor(countModel / itemOnPage)
    if (countModel % itemOnPage != 0)
        maxPage = maxPage + 1;

    // Lấy dữ liệu cho page 1
    ChangePage(1, itemOnPage);

}

// Làm mới nút di chuyển trang
function RefreshPaging(){
    let wraperPagination = document.getElementsByClassName("wraper-pagination")[0];
    wraperPagination.innerHTML = "";
    // page active ở giữa nếu 2 bên còn nhiều page
    wraperPagination.appendChild(CreateItem(currentPage, true, ChangePage));

    // add nút phía trước
    let i = currentPage - 1;
    let count = 2;
    do {
        if (i == 0) {
            // Thêm <
            wraperPagination.insertBefore(CreateItem("<", false, ChangePage, currentPage - 1), wraperPagination.children[0]);
            break;
        }
        else {
            wraperPagination.insertBefore(CreateItem(i, false, ChangePage, i), wraperPagination.children[0]);
        }

        i = i - 1;
        count = count - 1;
        if (count == 0) {
            if (i > 1) {
                // Thêm ...
                wraperPagination.insertBefore(CreateItem("...", false, ChangePage, 0), wraperPagination.children[0]);
                wraperPagination.insertBefore(CreateItem(1, false, ChangePage, 1), wraperPagination.children[0]);
            }
            if (i == 1) {
                wraperPagination.insertBefore(CreateItem(i, false, ChangePage, i), wraperPagination.children[0]);
            }
            // Thêm <
            wraperPagination.insertBefore(CreateItem("<", false, ChangePage, currentPage - 1), wraperPagination.children[0]);
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
            wraperPagination.insertBefore(CreateItem(">", false, ChangePage, currentPage + 1), null);
            break;
        }
        else {
            wraperPagination.insertBefore(CreateItem(i, false, ChangePage, i), null);
        }

        i = i + 1;
        count = count - 1;
        if (count == 0) {
            if (i < maxPage) {
                // Thêm ...
                wraperPagination.insertBefore(CreateItem("...", false, ChangePage, 0), null);
                wraperPagination.insertBefore(CreateItem(maxPage, false, ChangePage, maxPage), null);
            }
            if (i == maxPage) {
                wraperPagination.insertBefore(CreateItem(maxPage, false, ChangePage, maxPage), null);
            }
            // Thêm >
            wraperPagination.insertBefore(CreateItem(">", false, ChangePage, currentPage + 1), null);
            break;
        }
    }
    while (true);
}

// page: là trang muốn đi tới, '<','>' hoặc '...'
function CreateItem(textPage, isCurrentPage, ChangePage, goTo) {
    let divItem = document.createElement("div");
    if (isCurrentPage == true) {
        divItem.className = "pagination-item active";
    }
    else {
        divItem.className = "pagination-item";
    }
    divItem.onclick = function () {
        ChangePage(goTo);
    };

    let pPage = document.createElement("p");
    pPage.innerHTML = textPage;
    divItem.appendChild(pPage);
    return divItem;
}

async function ChangePage(page) {
    if (DEBUG) {
        console.log("ChangePage(" + page + ")");
        console.log("maxPage: " + maxPage);
    }
    if (page == null) {
        return;
    }

    if (page == 0 || page == maxPage + 1) {
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
    RefreshPaging();

    let url = "/ItemModel/ChangePage";
    const searchParams = new URLSearchParams();
    searchParams.append("namePara", namePara);
    searchParams.append("start", (page - 1) * itemOnPage);
    searchParams.append("offset", itemOnPage);
    let resObj = await RequestHttpGetPromise(searchParams, url);

    // Danh sách item
    //int id
    //string name
    //int status
    //string detail
    //int quota
    //List < Model > models
    //List < string > imageSrc
    //string videoSrc
    let listItem = JSON.parse(resObj.responseText);

    // Làm trống bảng
    DeleteRowsExcludeHead(document.getElementById("myTable"));

    // Show
    let table = document.getElementById("myTable");
    let length = listItem.length;
    for (let i = 0; i < length; i++) {
        let item = listItem[i];
        let row = table.insertRow(-1);

        // Insert new cells (<td> elements)
        let cell1 = row.insertCell(0);
        let cell2 = row.insertCell(1);
        let cell3 = row.insertCell(2);

        // Id
        cell1.innerHTML = item.id;
        cell1.style.display = "none";

        // Image
        let img = document.createElement("img");
        if (item.imageSrc.length > 0) {
            img.setAttribute("src", item.imageSrc[0]);
        } else {
            img.setAttribute("src", srcNoImageThumbnail);
        }
        img.height = thumbnailHeight;
        img.width = thumbnailWidth;
        img.className = "go-to-detail-item";
        img.onclick = function () {
            // Lấy id
            let id = Number(this.parentElement.parentElement.children[0].innerHTML);
            GoToDetailItem(id);
        };
        cell2.append(img);

        // Tên
        let pName = document.createElement("p");
        pName.innerHTML = item.name;
        pName.className = "go-to-detail-item";
        pName.onclick = function () {
            // Lấy id
            let id = Number(this.parentElement.parentElement.children[0].innerHTML);
            GoToDetailItem(id);
        };
        //cell3.innerHTML = item.name;
        cell3.append(pName);
    }
}

function GoToDetailItem(id) {
    if (DEBUG) {
        console.log("GoToDetailItem Start");
        console.log("item id: " + id);
    }
    if (isNaN(id))
        return;

    window.open("UpdateDelete?id=" + id);
}