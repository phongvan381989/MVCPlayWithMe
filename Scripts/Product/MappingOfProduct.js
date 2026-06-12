// mảng sản phẩm cần cập nhật
let listCommonItem = [];

async function GetListCommonItemMappingFromProductId() {
    listCommonItem = [];
    let id = GetValueFromUrlName("id");
    const searchParams = new URLSearchParams();
    searchParams.append("id", id);
    let query = "/Product/GetListMappingOfProduct";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    if (responseDB.responseText != "null") {
        listCommonItem = JSON.parse(responseDB.responseText);
    }
    else {
        listCommonItem = [];
    }
    if (listCommonItem.length == 0) {
        ShowDoesntFindId();
        return;
    }

    ShowResultWithFilter();
}

function ShowResultWithFilter() {
    let table = document.getElementById("myTable");
    let obj = document.querySelector('input[name="status-radio"]:checked');
    let listCommonItemTem = [];
    if (obj.value == "All") {
        listCommonItemTem = listCommonItem;
    }
    else if (obj.value == "On") { // Hiển thị những item đang bán
        for (let i = 0; i < listCommonItem.length; i++) {
            if (listCommonItem[i].bActive) {
                listCommonItemTem.push(listCommonItem[i]);
            }
        }
    }
    else { // Item dừng bán
        for (let i = 0; i < listCommonItem.length; i++) {
            if (!listCommonItem[i].bActive) {
                listCommonItemTem.push(listCommonItem[i]);
            }
        }
    }

    // Show table
    ShowListCommonItem(listCommonItemTem, table, false);
}
