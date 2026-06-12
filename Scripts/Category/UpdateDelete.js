// Không tìm thấy đối tượng, hiển thông báo
function ShowDoesntFindIdCategory() {
    if (document.getElementById("category-id").value == "" ) {
        document.getElementById("result-find-id").remove();
        let ele = document.getElementById("doesnt-find-id");
        ele.style.display = "flex";
        ele.style.alignItems = "center";
        ele.style.justifyContent = "center";
    }
}
ShowDoesntFindIdCategory();

async function UpdateCategory() {
    let categoryName = document.getElementById("category-id").value;
    if (CheckIsEmptyOrSpacesAndShowResult(categoryName, "Tên nhà thể loại không hợp lệ.")) {
        document.getElementById("category-id").focus();
        return;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("id", GetValueFromUrlName("id"));
    searchParams.append("name", categoryName);
    let query = "/Category/UpdateCategory";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Update thành công.", "Có lỗi xảy ra.");
}

async function DeleteCategory() {
    let text = "Nếu còn sản phẩm trong kho / sản phẩm trên voibenho thuộc thể loại này bạn sẽ không thể xóa. Bạn chắc chắn muốn XÓA?";
    if (confirm(text) == false)
        return;

    let id = GetValueFromUrlName("id");
    const searchParams = new URLSearchParams();
    searchParams.append("id", id);
    let query = "/Category/DeleteCategory";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Xóa thành công.", "Có lỗi xảy ra.");
}
