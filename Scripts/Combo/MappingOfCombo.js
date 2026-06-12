// mảng sản phẩm cần cập nhật
let listCommonItem = [];

async function GetListCommonItemMappingFromComboId() {
    listCommonItem = [];
    let id = GetValueFromUrlName("id");
    const searchParams = new URLSearchParams();
    searchParams.append("id", id);
    let query = "/Combo/GetListMappingOfCombo";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    if (responseDB.responseText != "null") {
        listCommonItem = JSON.parse(responseDB.responseText);
    }
    else {
        listCommonItem = [];
    }
    let table = document.getElementById("myTable");
    ShowListCommonItem(listCommonItem, table, false);
}
