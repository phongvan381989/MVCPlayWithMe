// mảng đối tượng product
let listBasicInfo = [];
GetListPublisher();

async function ChangePublisher() {
    let publisherId =
        GetDataIdFromPublisherDatalist(document.getElementById("publisher-id").value);
    if (publisherId === null) {
        listBasicInfoTemp = listBasicInfo;
    }
    else {
        const searchParams = new URLSearchParams();
        searchParams.append("publisherId", publisherId);
        searchParams.append("intervalDay", document.getElementById("interval-day").value);
        let query = "/Product/GetHintQuantityFromPublisher";
        ShowCircleLoader();
        let responseDB = await RequestHttpPostPromise(searchParams, query);
        RemoveCircleLoader();
        if (responseDB.responseText != "null") {
            listBasicInfo = JSON.parse(responseDB.responseText);
        }
        else {
            listBasicInfo = [];
        }
    }

    ShowListBasicInfo(listBasicInfo, document.getElementById("myTableStatistics"));
}

function ChangeIntervalDay(ele) {
    DeleteRowsExcludeHead(document.getElementById("myTableStatistics"));
    document.getElementById("count-result").innerHTML = "";
    document.getElementById("container-statistics").style.display = "none";
    document.getElementById("publisher-id").value = "";
    document.getElementById("xvxvxsg64sgs").innerHTML = "Lượng Bán Trong " + ele.options[ele.selectedIndex].text;
}

function ShowListBasicInfo(list, table) {
    // Show
    DeleteRowsExcludeHead(table);
    document.getElementById("count-result").innerHTML = "Tổng " + list.length + " sản phẩm";
    document.getElementById("container-statistics").style.display = "block";

    let length = list.length;
    if (length == 0) {
        return;
    }


    for (let i = 0; i < length; i++) {
        let obj = list[i];

        let row = table.insertRow(-1);
        // Insert new cells (<td> elements)
        let cell1 = row.insertCell(0);
        let cell2 = row.insertCell(1);
        let cell3 = row.insertCell(2);
        let cell4 = row.insertCell(3);
        let cell5 = row.insertCell(4);
        let cell6 = row.insertCell(5);
        let cell7 = row.insertCell(6);
        let cell8 = row.insertCell(7);

        // Id
        cell1.innerHTML = obj.id;
        cell1.style.display = "none";

        // Tên
        let p = document.createElement("p");
        p.innerHTML = obj.name;
        p.style.cursor = "pointer";
        p.onclick = function () {
            let url = "/Product/UpdateDelete?id=" + obj.id.toString();
            window.open(url);
        }
        cell2.append(p);


        // Số lượng sẽ đặt lấy bằng số lượng lấy mới nhất từ nhà phát hành
        cell3.innerHTML = obj.newImportedQuantity;

        // Sản phẩm/tháng
        cell4.innerHTML = Math.round(obj.soldQuantity / 3) + " sản phẩm/tháng";
        cell4.title = "Trong 3 tháng gần nhất đã bán được " + obj.soldQuantity + "sản phẩm"

        // Lượng bán trong khoảng thời gian đã chọn
        cell5.innerHTML = obj.soldQuantity;

        // Tồn kho
        cell6.innerHTML = obj.quantityInWarehouse;

        // Combo id
        cell7.innerHTML = obj.comboId;

        // Bao ngày chưa có đơn
        let pdaysDifference = document.createElement("p");
        pdaysDifference.innerHTML = obj.daysDifference;
        cell8.append(pdaysDifference);
        if (obj.daysDifference >
            ConvertToInt(document.getElementById("interval-day").value)) {
            pdaysDifference.title = "Trong khoảng thời gian đã chọn không phát sinh đơn nào cả";
        }
    }
}
