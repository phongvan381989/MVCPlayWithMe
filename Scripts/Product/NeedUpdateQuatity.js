// mảng sản phẩm cần cập nhật
let listCommonItem = [];
let listPro = [];

async function GetListProductInWarehoueChangedQuantity() {
    // Ẩn div chứa table kia
    document.getElementById("myTableTMDTDiv").style.display = "none";

    listPro = [];
    const searchParams = new URLSearchParams();
    let query = "/Product/GetListProductInWarehoueChangedQuantity";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    if (responseDB.responseText != "null") {
        listPro = JSON.parse(responseDB.responseText);
    }
    else {
        listPro = [];
        document.getElementById("myTableWarehoueseDiv").style.display = "none";
        CreateMustClickOkModal("Cập nhật có lỗi, vui lòng kiểm tra và thử lại.", null);
        return;
    }
    ShowListProductInWarehoueChangedQuatity(listPro);
}

function ShowListProductInWarehoueChangedQuatity(list) {

    let table = document.getElementById("myTableWarehouese");
    DeleteRowsExcludeHead(table);

    let length = list.length;
    if (length == 0) {
        alert("Không có sản phẩm thay đổi số lượng.")
        document.getElementById("myTableWarehoueseDiv").style.display = "none";
        return;
    }

    // Show
    document.getElementById("myTableWarehoueseDiv").style.display = "initial";

    for (let i = 0; i < length; i++) {
        let item = list[i];
        let row = table.insertRow(-1);

        // Insert new cells (<td> elements)
        let cell1 = row.insertCell(0);
        let cell2 = row.insertCell(1);
        let cell3 = row.insertCell(2);
        let cell4 = row.insertCell(3);


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
        img.title = "Xem sản phẩm liên kết trên sàn";
        img.onclick = function () {
            // Lấy id
            let id = Number(this.parentElement.parentElement.children[0].innerHTML);
            window.open("MappingOfProduct?id=" + id);
        };
        cell2.append(img);

        // Tên
        cell3.innerHTML = item.name;

        // Tồn kho
        cell4.innerHTML = item.quantity;
    }
}

async function GetListNeedUpdateQuantityAndUpdate() {
    // Ẩn div chứa table, table kia
    document.getElementById("myTableWarehoueseDiv").style.display = "none";

    const searchParams = new URLSearchParams();
    let query = "/Product/GetListNeedUpdateQuantityAndUpdate";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    if (responseDB.responseText != "null") {
        listCommonItem = JSON.parse(responseDB.responseText);
    }
    else {
        listCommonItem = [];
        document.getElementById("myTableTMDTDiv").style.display = "none";
        CreateMustClickOkModal("Cập nhật có lỗi, vui lòng kiểm tra và thử lại.", null);
        return;
    }

    if (listCommonItem.length == 0) {
        document.getElementById("myTableTMDTDiv").style.display = "none";
        alert("Không có sản phẩm trên sàn cần cập nhật số lượng.")
        return;
    }

    // Show table
    document.getElementById("myTableTMDTDiv").style.display = "initial";
    //ShowListCommonItem(listCommonItem, table, true);

    //let updateOk = ShowWhyUpdateFail(listCommonItem);
    let updateOk = ShowResultWithFilter();
    if (updateOk) {
        alert("Cập nhật thành công.")
    }
    else {
        CreateMustClickOkModal("Cập nhật có lỗi, vui lòng kiểm tra và thử lại.", null);
    }
}

function ShowResultWithFilter() {
    //if (DEBUG) {
    //    console.log("ShowResultWithFilter CALL");
    //}
    let table = document.getElementById("myTableTMDT");
    let obj = document.querySelector('input[name="status-radio"]:checked');
    if (obj.value == "On") {// Hiển thị tất cả item đã cập nhật số lượng trên sàn
        // Show table
        ShowListCommonItem(listCommonItem, table, true);

        let updateOk = ShowWhyUpdateFail(listCommonItem);
        return updateOk;
    }
    else { // Hiển thị tất cả item đã cập nhật LỖI số lượng trên sàn
        let listCommonItemTem = [];
        for (let i = 0; i < listCommonItem.length; i++) {
            let item = listCommonItem[i];
            if (item.result == null) // item không ở trạng thái NORMAL
            {
                continue;
            }

            if (item.eType == eTiki) {
                let responseHTTP = item.result.myJson;
                let errors = responseHTTP.errors;
                if (errors != null) {
                    //if (DEBUG) {
                    //    console.log("listCommonItemTem.push(item)");
                    //    console.log(JSON.stringify(item));
                    //}
                    listCommonItemTem.push(item);
                }

            }
            else if (item.eType == eShopee && item.result.myJson != null) {
                let responseHTTP = item.result.myJson;
                let failure_list = responseHTTP.response.failure_list;
                let leng_failure_list = failure_list.length;

                let modelLength = item.models.length;
                for (let j = 0; j < modelLength; j++) {
                    let model = item.models[j];
                    for (let k = 0; k < leng_failure_list; k++) {
                        if (model.modelId == failure_list[k].model_id ||
                            failure_list[k].model_id == 0)// item shopee không có model
                        {
                            console.log("listCommonItemTem.push(item)");
                            console.log(JSON.stringify(item));
                            listCommonItemTem.push(item);
                        }
                    }
                }
            }
            else if (item.eType == eShopee && item.result.myJson == null) {
                listCommonItemTem.push(item);
            }
        }

        // Show table
        ShowListCommonItem(listCommonItemTem, table, true);

        ShowWhyUpdateFail(listCommonItemTem);
    }

    return false;
}