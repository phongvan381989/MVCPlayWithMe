let listPublisher = [];

async function ShowListPublisher() {
    // Lấy dữ liệu danh sách nhà phát hành
    {
        const searchParams = new URLSearchParams();
        let query = "/Publisher/GetListPublisher";
        ShowCircleLoader();
        let responseDB = await RequestHttpPostPromise(searchParams, query);
        RemoveCircleLoader();
        if (responseDB.responseText != "null") {
            listPublisher = JSON.parse(responseDB.responseText);
        }
        else {
            listPublisher = [];
        }
    }

    let table = document.getElementById("publisher-table");
    //if (DEBUG) {
    //    console.log(JSON.stringify(listPublisher));
    //    console.log("length = " + listPublisher.length);
    //}
    let length = listPublisher.length;
    if (length == 0) {
        table.style.display = "none";
        return;
    }
    table.style.display = "initial";
    DeleteRowsExcludeHead(table);

    for (let i = 0; i < length; i++) {
        let publisher = listPublisher[i];

        let row = table.insertRow(-1);
        // Insert new cells (<td> elements)
        let cell1 = row.insertCell(0);
        let cell2 = row.insertCell(1);
        let cell3 = row.insertCell(2);
        let cell4 = row.insertCell(3);
        let cell5 = row.insertCell(4);

        // Id
        cell1.innerHTML = publisher.id;
        cell1.style.display = "none";

        // STT
        cell2.innerHTML = i + 1;

        // Name
        let name = document.createElement("p");
        name.innerHTML = publisher.name;
        name.className = "go-to-detail-item";
        name.title = "Xem chi tiết nhà phát hành";
        name.onclick = function () {
            // Lấy id
            let id = Number(this.parentElement.parentElement.children[0].innerHTML);
            window.open("/Publisher/UpdateDelete?id=" + id);
        };
        cell3.append(name);

        // Chiết khấu
        cell4.innerHTML = publisher.discount + "%";

        // Mô tả
        cell5.innerHTML = publisher.detail;
    }
}

async function CreatePublisher() {
    let publisherName = document.getElementById("publisher-id").value;
    if (CheckIsEmptyOrSpacesAndShowResult(publisherName, "Tên nhà phát hành không hợp lệ.")) {
        document.getElementById("publisher-id").focus();
        return;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("name", CapitalizeWords(publisherName));
    searchParams.append("discount", GetValueInputById("publisher-discount", 20));
    searchParams.append("detail", document.getElementById("publisher-detail").value);
    let query = "/Publisher/CreatePublisher";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Tạo thành công.", "Có lỗi xảy ra.");
}

