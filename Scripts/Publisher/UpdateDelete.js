// Không tìm thấy đối tượng, hiển thông báo
function ShowDoesntFindIdPublisher() {
    if (document.getElementById("publisher-id").value == "") {
        document.getElementById("result-find-id").remove();
        let ele = document.getElementById("doesnt-find-id");
        ele.style.display = "flex";
        ele.style.alignItems = "center";
        ele.style.justifyContent = "center";
    }
}
ShowDoesntFindIdPublisher();

async function UpdatePublisher() {
    let publisherName = document.getElementById("publisher-id").value;
    if (CheckIsEmptyOrSpacesAndShowResult(publisherName, "Tên nhà phát hành không hợp lệ.")) {
        document.getElementById("publisher-id").focus();
        return;
    }

    let publisherDetail = document.getElementById("publisher-detail").value;
    const searchParams = new URLSearchParams();
    searchParams.append("id", GetValueFromUrlName("id"));
    searchParams.append("name", CapitalizeWords(publisherName));
    searchParams.append("discount", GetValueInputById("publisher-discount", 20));
    searchParams.append("detail", publisherDetail);
    let query = "/Publisher/UpdatePublisher";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Update thành công.", "Có lỗi xảy ra.");
}

async function DeletePublisher() {
    let text = "Nếu còn sản phẩm thuộc nhà phát hành này bạn sẽ không thể xóa. Bạn chắc chắn muốn XÓA?";
    if (confirm(text) == false)
        return;

    let publisherId = GetValueFromUrlName("id");
    const searchParams = new URLSearchParams();
    searchParams.append("id", publisherId);
    let query = "/Publisher/DeletePublisher";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Xóa thành công.", "Có lỗi xảy ra.");
}
