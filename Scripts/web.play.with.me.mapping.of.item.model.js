// Sinh dev chứa mapping, append vào cell
function GenerateDivMapping(cell, listMapping) {
    let sample = document.getElementsByClassName("mapping_sample")[0];
    let clone = sample.cloneNode(true);
    clone.style.display = "initial";
    clone.className = "fdfxz0s9ff";
    let length = listMapping.length;
    let table = clone.firstElementChild;

    for (let i = 0; i < length; i++) {
        let map = listMapping[i];

        let row = table.insertRow(-1);
        // Insert new cells (<td> elements)
        let cell1 = row.insertCell(0);
        let cell2 = row.insertCell(1);
        let cell3 = row.insertCell(2);
        let cell4 = row.insertCell(3);
        let cell5 = row.insertCell(4);

        // Id
        cell1.innerHTML = map.product.id;
        cell1.style.display = "none";

        // STT
        cell2.innerHTML = i + 1;

        // Tên
        let pName = document.createElement("P");
        pName.innerHTML = map.product.name;
        pName.className = "go-to-detail-item";
        pName.title = "Xem sản phẩm trong kho";
        pName.onclick = function () {
            // Lấy id
            let id = Number(this.parentElement.parentElement.children[0].innerHTML);
            window.open(GetProductUrl(id));
        };
        cell3.append(pName);

        // Số lượng mapping
        cell4.innerHTML = map.quantity;

        // Số lượng kho
        cell5.innerHTML = map.product.quantity;
    }
    cell.append(clone);
}

// Từ list common item server trả về ta hiển thị kết quả cập nhật mà các sàn thương mại điện tử trả về
function ShowWhyUpdateFail(listCommonItemTemp) {
    // Hiển thị trạng thái cập nhật
    let updateOk = true;
    let length = listCommonItemTemp.length;
    for (let i = 0; i < length; i++) {
        let item = listCommonItemTemp[i];
        if (item.result == null) // item không ở trạng thái NORMAL
        {
            continue;
        }

        if (item.eType == eTiki) {
            let eleWhyUpdateFail = document.getElementById(item.eType + "_" + item.itemId + "_" + item.models[0].modelId + "_whyUpdateFail");
            let responseHTTP = item.result.myJson;
            let errors = responseHTTP.errors;
            if (errors != null) {
                eleWhyUpdateFail.innerHTML = errors[0];
                updateOk = false;
            }
            else {
                eleWhyUpdateFail.innerHTML = "Xong";
            }
        }
        else if (item.eType == eShopee && item.result.myJson != null) {
            let responseHTTP = item.result.myJson;
            let failure_list = responseHTTP.response.failure_list;
            let leng_failure_list = failure_list.length;

            //let success_list = responseHTTP.response.success_list;
            //let leng_success_list = success_list.length;

            let modelLength = item.models.length;
            for (let j = 0; j < modelLength; j++) {
                let model = item.models[j];
                let mes = "Xong";
                let eleWhyUpdateFail = document.getElementById(item.eType + "_" + item.itemId + "_" + model.modelId + "_whyUpdateFail");
                for (let k = 0; k < leng_failure_list; k++) {
                    if (model.modelId == failure_list[k].model_id ||
                        failure_list[k].model_id == 0)// item shopee không có model
                    {
                        mes = failure_list[k].failed_reason;
                        updateOk = false;
                        break;
                    }
                }
                eleWhyUpdateFail.innerHTML = mes;
                //if (UpdateOk) {// Hiển thị số lượng sản phẩm trên sàn
                //    let eleQuantity = document.getElementById(item.eType + "_" + item.itemId + "_" + model.modelId + "_quantity");
                //    for (let k = 0; k < leng_success_list; k++) {
                //        if (model.modelId == success_list[k].model_id ||
                //            success_list[k].model_id == 0)// item shopee không có model
                //        {
                //            eleQuantity.innerHTML = success_list[k].stock;
                //            break;
                //        }
                //    }
                //}
            }
        }
    }

    return updateOk;
}

// Từ list common item server trả về ta kiểm tra tất cả các sản phẩm trên sàn cập nhật thành công hay không
function CheckUpdateQuantitySuccessAll(listCommonItemTemp) {
    // Hiển thị trạng thái cập nhật
    let updateOk = true;
    let length = listCommonItemTemp.length;
    for (let i = 0; i < length; i++) {
        let item = listCommonItemTemp[i];
        if (item.result == null) // item không ở trạng thái NORMAL
        {
            continue;
        }

        if (item.eType == eTiki) {
            let responseHTTP = item.result.myJson;
            let errors = responseHTTP.errors;
            if (errors != null) {
                updateOk = false;
                break;
            }

        }
        else if (item.eType == eShopee) {
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
                        updateOk = false;
                        break;
                    }
                }
            }
            if (!updateOk) {
                break;
            }
        }
    }

    return updateOk;
}

// Cập nhật số lượng 1 model lên sàn
// ele show kết quả cập nhật
async function UpdateQuantityOfOneItemModel(eType, itemId, modelId, ele) {

    const searchParams = new URLSearchParams();
    searchParams.append("eType", eType);
    searchParams.append("itemId", itemId);
    searchParams.append("modelId", modelId);
    let query = "/Product/UpdateQuantityOfOneItemModel";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    let result = JSON.parse(responseDB.responseText);
    if (eType == eTiki) {
        if (result.myJson == null) {
            ele.innerHTML = "Có lỗi không xác định. Vui lòng thử lại sau.";
        }
        else if (result.myJson.errors != null && result.myJson.errors.length > 0) {
            ele.innerHTML = result.myJson.errors[0];
        }
        else {
            ele.innerHTML = "Xong";
        }
    }
    else if (eType == eShopee) {
        if (result.myJson == null) {
            ele.innerHTML = "Có lỗi không xác định. Vui lòng thử lại sau.";
        }
        else if (result.myJson.response.failure_list.length > 0) {
            ele.innerHTML = result.myJson.failure_list[0].failed_reason;
        }
        else {
            ele.innerHTML = "Xong";
        }
    }
    else if (eType == eLazada) {
        if (result.myJson == null) {
            ele.innerHTML = "LazadaRefreshAccessTokenIfNeed thất bại.";
        }
        else if (result.myJson.code != null && result.myJson.code != "0") {
            ele.innerHTML = result.myJson.message;
        }
        else {
            ele.innerHTML = "Xong";
        }
    }
    return new Promise(function (resolve, reject) {
    });
}

function ShowListCommonItem(list, table, disableUpdateButton) {
    //if (DEBUG) {
    //    console.log("ShowListCommonItem CALL");
    //    console.log("list.length: " + list.length);
    //}
    // Show
    DeleteRowsExcludeHead(table);

    let length = list.length;
    if (length == 0) {
        return;
    }

    // enum {
    //    PLAYWITHME  0,
    //    TIKI  1,
    //    SHOPEE  2,
    //    LAZADA  3
    //}
    for (let i = 0; i < length; i++) {
        let item = list[i];

        let modelLength = item.models.length;
        for (let j = 0; j < modelLength; j++) {

            let row = table.insertRow(-1);
            let model = item.models[j];
            // Insert new cells (<td> elements)
            let cell1 = row.insertCell(0);
            let cell2 = row.insertCell(1);
            let cell3 = row.insertCell(2);
            let cell4 = row.insertCell(3);
            let cell5 = row.insertCell(4);
            let cell6 = row.insertCell(5);
            let cell7 = row.insertCell(6);
            let cell8 = row.insertCell(7);
            let cell9 = row.insertCell(8);

            // item Id
            cell1.innerHTML = item.itemId;
            cell1.style.display = "none";

            // Model Id
            cell2.innerHTML = model.modelId;
            cell2.style.display = "none";

            // Image
            let img = document.createElement("img");
            if (model.imageSrc !=null && model.imageSrc.length > 0) {
                img.setAttribute("src", model.imageSrc);
            } else {
                img.setAttribute("src", srcNoImageThumbnail);
            }
            img.height = thumbnailHeight;
            img.width = thumbnailWidth;
            img.className = "go-to-detail-item";
            img.title = "Xem sản phẩm trên sàn thương mại";
            img.itemUrl = GetTMDTItemUrlFromCommonItem(item);
            img.onclick = function () {
                // Lấy id
                let id = Number(this.parentElement.parentElement.children[0].innerHTML);
                window.open(this.itemUrl);
            };
            cell3.append(img);

            // Tên sàn
            cell4.innerHTML = item.eType;

            // Số lượng model trên sàn
            cell5.innerHTML = "";//model.quantity_sellable;
            //cell5.id = item.eType + "_" + item.itemId + "_" + model.modelId + "_quantity";

            // Tên
            if (model.modelId == -1) {
                cell6.innerHTML = item.name;
            }
            else {
                cell6.innerHTML = item.name + "--" + model.name;
            }
            cell6.title = "Click để tới trang thay đổi mapping.";
            cell6.style.cursor = "pointer";
            cell6.onclick = function () {
                let url = "";
                if (item.eType == eTiki) {
                    url = "/ProductECommerce/Item?eType=TIKI&id=" + item.itemId;
                }
                else if (item.eType == eShopee) {
                    url = "/ProductECommerce/Item?eType=SHOPEE&id=" + item.itemId;
                }
                else if (item.eType == eLazada) {
                    url = "/ProductECommerce/Item?eType=LAZADA&id=" + item.itemId;
                }
                if (url !== "") {
                    window.open(url);
                }
            };

            // Liên kết
            GenerateDivMapping(cell7, model.mapping);

            // Nút cập nhật
            let btn = document.createElement("button");
            btn.innerHTML = "Cập nhật.";
            btn.title = "Nếu Item không hoạt động bình thường, nút này được vô hiệu.";
            if (disableUpdateButton == false && item.bActive) {
                btn.onclick = async function () {
                    let grandFather = this.parentElement.parentElement;
                    // Lấy id
                    let itemId = Number(grandFather.children[0].innerHTML);
                    let modelId = Number(grandFather.children[1].innerHTML);
                    let eType = grandFather.children[3].innerHTML;
                    let ele = grandFather.children[8];
                    await UpdateQuantityOfOneItemModel(eType, itemId, modelId, ele);
                }
            }
            else {
                btn.disabled = true;
            }
            cell8.append(btn);

            // Kết quả
            cell9.id = item.eType + "_" + item.itemId + "_" + model.modelId + "_whyUpdateFail";
            cell9.title = "Item không hoạt động bình thường sẽ không cập nhật, dữ liệu ô này sẽ trống";
        }
    }
}

async function UpdateQuantityToTMDTFromListCommonItem(isComboId, listCommonItem) {
    if (listCommonItem == null || listCommonItem.length == 0) {
        return;
    }
    const searchParams = new URLSearchParams();
    searchParams.append("isCombo", isComboId);
    searchParams.append("productOrComboId", GetValueFromUrlName("id"));
    searchParams.append("listCommonItem", JSON.stringify(listCommonItem));
    let query = "/Product/UpdateQuantityToTMDTFromListCommonItem";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();
    if (responseDB.responseText != "null") {
        let listCommonItemTemp = JSON.parse(responseDB.responseText);
        // listCommonItem và listCommonItemTemp chỉ khác nhau trường result
        let updateOk = ShowWhyUpdateFail(listCommonItemTemp);
        if (updateOk) {
            alert("Cập nhật thành công.")
        }
        else {
            CreateMustClickOkModal("Cập nhật có lỗi, vui lòng kiểm tra và thử lại.", null);
        }
    }
}