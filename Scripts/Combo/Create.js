let listCombo = [];

async function CreateCombo() {
    let name = document.getElementById("combo-name").value.trim();
    if (CheckIsEmptyOrSpacesAndShowResult(name, "Tên Combo không hợp lệ.")) {
        document.getElementById("combo-name").focus();
        return;
    }

    let code = document.getElementById("combo-code").value.trim();
    if (CheckIsEmptyOrSpacesAndShowResult(name, "Mã Combo không hợp lệ.")) {
        document.getElementById("combo-code").focus();
        return;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("name", CapitalizeWords(name));
    searchParams.append("code", code);
    let url = "/Combo/CreateCombo";

    ShowCircleLoader();
    let responseDB = await RequestHttpGetPromise(searchParams, url);
    RemoveCircleLoader();
    if (responseDB.responseText != "null") {
        CheckStatusResponseAndShowPrompt(responseDB.responseText, "Thêm mới thành công.", "Thêm mới thất bại.");
    }
    else {
        CreateMustClickOkModal("Thêm mới thất bại.", null);
    }
}

async function ShowListCombo() {
    // Lấy dữ liệu danh sách combo
    {
        const searchParams = new URLSearchParams();

        let query = "/Combo/GetListCombo";
        ShowCircleLoader();
        let responseDB = await RequestHttpPostPromise(searchParams, query);
        RemoveCircleLoader();
        if (responseDB.responseText != "null") {
            listCombo = JSON.parse(responseDB.responseText);
        }
        else {
            listCombo = [];
        }
    }

    let table = document.getElementById("combo-table");
    //if (DEBUG) {
    //    console.log(JSON.stringify(listCombo));
    //    console.log("length = " + listCombo.length);
    //}
    let length = listCombo.length;
    if (length == 0) {
        table.style.display = "none";
        return;
    }
    table.style.display = "initial";
    DeleteRowsExcludeHead(table);

    for (let i = 0; i < length; i++) {
        let combo = listCombo[i];

        let row = table.insertRow(-1);
        // Insert new cells (<td> elements)
        let cell1 = row.insertCell(0);
        let cell2 = row.insertCell(1);
        let cell3 = row.insertCell(2);
        let cell4 = row.insertCell(3);

        // Id
        cell1.innerHTML = combo.id;
        cell1.style.display = "none";

        // STT
        cell2.innerHTML = i + 1;

        // Mã
        cell3.innerHTML = combo.code;

        // Name
        let name = document.createElement("p");
        name.innerHTML = combo.name;
        name.className = "go-to-detail-item";
        name.title = "Xem chi tiết combo";
        name.onclick = function () {
            // Lấy id
            let id = Number(this.parentElement.parentElement.children[0].innerHTML);
            window.open("/Combo/UpdateDelete?id=" + id);
        };
        cell4.append(name);
    }
}
