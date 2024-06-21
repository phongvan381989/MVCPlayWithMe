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

function ShowListCommonItemFromProductId(list, table) {
    // Show
    DeleteRowsExcludeHead(table);

    let length = list.length;
    if (length == 0) {
        return;
    }

    // enum {
    //    PLAY_WITH_ME  0,
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

            // item Id
            cell1.innerHTML = item.itemId;
            cell1.style.display = "none";

            // Model Id
            cell2.innerHTML = model.modelId;
            cell2.style.display = "none";

            // Image
            let img = document.createElement("img");
            if (model.imageSrc.length > 0) {
                img.setAttribute("src", model.imageSrc);
            } else {
                img.setAttribute("src", srcNoImageThumbnail);
            }
            img.height = thumbnailHeight;
            img.width = thumbnailWidth;
            img.className = "go-to-detail-item";
            img.title = "Xem sản phẩm trên sàn thương mại";
            img.onclick = function () {
                // Lấy id
                let id = Number(this.parentElement.parentElement.children[0].innerHTML);
                let url;
                // Item là Tiki
                if (item.eType == 1) {
                    url = GetTikiItemUrl(id);
                }
                // Item là shopee
                else if (item.eType == 2) {
                    url = GetShopeeItemUrl(id);
                }
                window.open(url);
            };
            cell3.append(img);

            // Tên sàn
            // Item là Tiki
            if (item.eType == 1) {
                cell4.innerHTML = eTiki;
            }
            // Item là shopee
            else if (item.eType == 2) {
                cell4.innerHTML = eShopee;
            }

            // Số lượng model trên sàn
            cell5.innerHTML = model.quantity_sellable;

            // Tên
            if (model.modelId == -1) {
                cell6.innerHTML = item.name;
            }
            else {
                cell6.innerHTML = item.name + "--" + model.name;
            }

            // Liên kết
            GenerateDivMapping(cell7, model.mapping);

            // Nút cập nhật
            let btn = document.createElement("button");
            btn.innerHTML = "Cập nhật";
            btn.onclick = function () {
                // Lấy id
                let id = Number(this.parentElement.parentElement.children[0].innerHTML);
            }
            cell7.append(btn);

            // Kết quả
            if (model.whyUpdateFail != null)
            cell8.innerHTML = model.whyUpdateFail;
        }
    }
}