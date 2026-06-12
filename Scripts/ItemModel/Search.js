let listItem; // Danh sách item kết quả tìm kiếm
let listItemTemp; // Danh sách item hiển thị ra màn hình
// Tham số tìm kiếm
GetListPublisher();
// Mục đích lấy số lượng trong kết quả trả về phục vụ phân trang
async function Search() {
    const searchParams = new URLSearchParams();
    if (!SetSearchParameter(searchParams)) {
        return;
    }
    let url = "/ItemModel/SearchItem";
    ShowCircleLoader();
    let resObj = await RequestHttpGetPromise(searchParams, url);
    RemoveCircleLoader();
    listItem = [];
    if (resObj.responseText != "null") {
        listItem = JSON.parse(resObj.responseText);
    }

    let length = listItem.length;
    if (length == 0) {
        document.getElementById("empty-result").style.display = "block";
        document.getElementById("search-result").style.display = "none";
        return;
    }

    document.getElementById("empty-result").style.display = "none";
    document.getElementById("search-result").style.display = "block";

    // Hiển thị tất cả kết quả sau tìm kiếm
    document.getElementById("sgdhgf567xbx").checked = true;
    FilterItemAll();
}


function SetSearchParameter(searchParams) {
    //let publisherId = GetDataIdFromPublisherDatalist(document.getElementById("publisher-id").value)
    //if (DEBUG) {
    //    console.log("SetSearchParameter CALL publisherId: " + publisherId);
    //}
    //if (publisherId == null) {
    //    CreateMustClickOkModal("Chưa chọn đúng nhà phát hành.", null);
    //    return false;
    //}
    searchParams.append("publisherId", 0);
    let namePara = document.getElementById("item-model-name").value;
    searchParams.append("namePara", namePara);
    return true;
}

function FilterItemAll() {
    //if (DEBUG) {
    //    console.log("FilterItemAll CALL");
    //}
    listItemTemp = listItem;
    ShowFilteringResult(listItemTemp);
}

// Trả lại những item có model giá bìa hoặc giá bán <= 0
function FilterItemNoPrice() {
    //if (DEBUG) {
    //    console.log("FilterItemNoPrice CALL");
    //}
    listItemTemp = [];
    let length = listItem.length;
    for (let i = 0; i < length; i++) {
        if (listItem[i].models.length > 0) {
            for (let j = 0; j < listItem[i].models.length; j++) {
                if (listItem[i].models[j].bookCoverPrice <= 0 ||
                    listItem[i].models[j].price <= 0) {
                    listItemTemp.push(listItem[i]);
                    break;
                }
            }
        }
    }

    ShowFilteringResult(listItemTemp);
}
//
// Trả lại những item có model chưa liên kết sản phẩm trong kho
function FilterItemNoMapping() {
    //if (DEBUG) {
    //    console.log("FilterItemNoMapping CALL");
    //}
    listItemTemp = [];
    let length = listItem.length;
    for (let i = 0; i < length; i++) {
        if (listItem[i].models.length > 0) {
            for (let j = 0; j < listItem[i].models.length; j++) {
                if (listItem[i].models[j].mapping[0].id == -1) {
                    listItemTemp.push(listItem[i]);
                    break;
                }
            }
        }
    }

    ShowFilteringResult(listItemTemp);
}

function FilterItemDiscountBigger() {
    //if (DEBUG) {
    //    console.log("FilterItemDiscountBigger CALL");
    //}
    let discount = document.getElementsByClassName("discout-bigger")[0].value;
    listItemTemp = [];
    let length = listItem.length;
    for (let i = 0; i < length; i++) {
        if (listItem[i].models.length > 0) {
            for (let j = 0; j < listItem[i].models.length; j++) {
                if (listItem[i].models[j].discount >= discount) {
                    listItemTemp.push(listItem[i]);
                    break;
                }
            }
        }
    }

    ShowFilteringResult(listItemTemp);
}

function FilterItemDiscountSmaller() {
    //if (DEBUG) {
    //    console.log("FilterItemDiscountSmaller CALL");
    //}
    let discount = document.getElementsByClassName("discout-smaller")[0].value;
    listItemTemp = [];
    let length = listItem.length;
    for (let i = 0; i < length; i++) {
        if (listItem[i].models.length > 0) {
            for (let j = 0; j < listItem[i].models.length; j++) {
                if (listItem[i].models[j].discount <= discount) {
                    listItemTemp.push(listItem[i]);
                    break;
                }
            }
        }
    }

    ShowFilteringResult(listItemTemp);
}

function FilterItemPriceBigger() {
    //if (DEBUG) {
    //    console.log("FilterItemPriceBigger CALL");
    //}
    let price = document.getElementsByClassName("price-bigger")[0].value;
    listItemTemp = [];
    let length = listItem.length;
    for (let i = 0; i < length; i++) {
        if (listItem[i].models.length > 0) {
            for (let j = 0; j < listItem[i].models.length; j++) {
                if (listItem[i].models[j].price >= price) {
                    listItemTemp.push(listItem[i]);
                    break;
                }
            }
        }
    }

    ShowFilteringResult(listItemTemp);
}

function FilterItemPriceSmaller() {
    //if (DEBUG) {
    //    console.log("FilterItemPriceSmaller CALL");
    //}
    let price = document.getElementsByClassName("price-smaller")[0].value;
    listItemTemp = [];
    let length = listItem.length;
    for (let i = 0; i < length; i++) {
        if (listItem[i].models.length > 0) {
            for (let j = 0; j < listItem[i].models.length; j++) {
                if (listItem[i].models[j].price <= price) {
                    listItemTemp.push(listItem[i]);
                    break;
                }
            }
        }
    }

    ShowFilteringResult(listItemTemp);
}

function UpdateDiscountForListItemBigger() {
    if (listItemTemp == null || listItemTemp.length == 0) {
        CreateMustClickOkModal("Danh sách sản phẩm trống.", null);
        return;
    }
    let discount = document.getElementsByClassName("discout-for-list-item-bigger")[0].value;
    let price = document.getElementsByClassName("price-for-list-item-bigger")[0].value;
    let listModelId = [];
    let length = listItemTemp.length;
    for (let i = 0; i < length; i++) {
        if (listItemTemp[i].models.length > 0) {
            for (let j = 0; j < listItemTemp[i].models.length; j++) {
                if (listItemTemp[i].models[j].price >= price) {
                    listModelId.push(listItemTemp[i].models[j].id);
                }
            }
        }
    }
    UpdateDiscountForListModelId(discount, listModelId);
}

function UpdateDiscountForListItemSmaller() {
    if (listItemTemp == null || listItemTemp.length == 0) {
        CreateMustClickOkModal("Danh sách sản phẩm trống.", null);
        return;
    }
    let discount = document.getElementsByClassName("discout-for-list-item-smaller")[0].value;
    let price = document.getElementsByClassName("price-for-list-item-smaller")[0].value;
    let listModelId = [];
    let length = listItemTemp.length;
    for (let i = 0; i < length; i++) {
        if (listItemTemp[i].models.length > 0) {
            for (let j = 0; j < listItemTemp[i].models.length; j++) {
                if (listItemTemp[i].models[j].price <= price) {
                    listModelId.push(listItemTemp[i].models[j].id);
                }
            }
        }
    }
    //if (DEBUG) {
    //    console.log("discount: " + discount);
    //    console.log("price: " + price);
    //    console.log("listModelId: " + listModelId.toString());
    //}
    UpdateDiscountForListModelId(discount, listModelId);
}

async function UpdateDiscountForListModelId(discount, listModelId) {
    if (listModelId == null || listModelId.length == 0)
        return;

    //if (DEBUG) {
    //    console.log("UpdateDiscountForListModelId CALL");
    //    console.log("discount: " + discount);
    //    console.log("listModelId: " + listModelId.toString());
    //}
    let url = "/ItemModel/UpdateDiscountForListModelId";
    const searchParams = new URLSearchParams();
    searchParams.append("discount", discount);
    searchParams.append("listModelId", listModelId.toString());
    ShowCircleLoader();
    await RequestHttpPostPromise(searchParams, url);
    RemoveCircleLoader();
}

function ShowFilteringResult(list) {
    // Danh sách item
    //int id
    //string name
    //int status
    //string detail
    //int quota
    //List < Model > models
    //List < string > imageSrc
    //string videoSrc
    let length = list.length;
    document.getElementsByClassName("avasf931")[0].innerHTML = length + " sản phẩm";

    // Show
    let table = document.getElementById("myTable");
    // Làm trống bảng
    DeleteRowsExcludeHead(table);

    for (let i = 0; i < length; i++) {
        let item = list[i];
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
            img.setAttribute("src", Get320VersionOfImageSrc(item.imageSrc[0]));
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
        //pName.className = "go-to-detail-item";
        //pName.onclick = function () {
        //    // Lấy id
        //    let id = Number(this.parentElement.parentElement.children[0].innerHTML);
        //    GoToDetailItem(id);
        //};
        //cell3.innerHTML = item.name;
        cell3.append(pName);
    }
}

function GoToDetailItem(id) {
    if (isNaN(id))
        return;

    window.open("/ItemModel/UpdateDelete?id=" + id);
}
