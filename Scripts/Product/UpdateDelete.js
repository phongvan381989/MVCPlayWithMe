let listOutput = [];
ShowUpdateButtonForOne();

ShowProductFromObject();
let product = null;
async function ShowProductFromObject() {
    let responseDB = await GetProductFromId(GetValueFromUrlName("id"));
    if (responseDB.responseText != "null") {
        GetSomeData();
        product = JSON.parse(responseDB.responseText);
    }
    else {
        ShowDoesntFindId();
        return;
    }
    SetProductInfomation(product);

    document.getElementById("product-name-when-creating").value = GenerateName(product);
}

function GenerateName(product) {
    // Tên đăng gồm: Sách Tên combo "-" tên sản phẩm.
    let name = product.name;
    if (IsValidString(product.comboName)) {
        name = product.comboName + " - " + product.name;
    }

    // Bỏ chữ "combo" ở đầu tên nếu có (không phân biệt hoa thường)
    if (name.trimStart().toLowerCase().startsWith("combo")) {
        // Bỏ từ "combo" ở đầu và loại bỏ khoảng trắng
        name = name.trimStart().substring(5).trim();
    } else {
        // Loại bỏ khoảng trắng nếu không có "combo"
        name = name.trim();
    }

    // Nếu category có chữ sách ở đầu => sản phẩm là sách
    if (product.categoryName.toLowerCase().startsWith("sách")) {
        // Nếu tên có chữ "sách" ở đầu rồi thì thôi, không thêm vào.
        if (!name.toLowerCase().startsWith("sách")) {
            name = "Sách " + name;
        }
    }

    return name;
}

function GenerateShopeeName(product) {
    // Tên đăng gồm: Sách + tên sản phẩm.
    let name = product.name.trim();

    // Nếu tên có chữ "sách" ở đầu rồi thì thôi, không thêm vào.
    if (!name.toLowerCase().startsWith("sách")) {

        // Nếu combo có chữ ehon thì thêm
        if (/ehon/i.test(product.comboName)) {
            name = "Sách Ehon " + name;
        }
        else {
            name = "Sách " + name;
        }
    }

    return name;
}

function MappingOfProduct() {
    // Lấy id
    let id = GetValueFromUrlName("id");
    window.open("MappingOfProduct?id=" + id);
}

GetOutputOfProduct();

function ECommerceTypeChange() {
    let index = GetIntECommerceType();
    if (DEBUG) {
        console.log("ECommerceTypeChange index: " + index);
    }

    if (index == intShopee) {
        document.getElementById("product-name-when-creating").value = GenerateShopeeName(product);
    }

    let listOutputTem = [];
    if (index == intAll) {
        listOutputTem = listOutput;
    }
    else {
        let length = listOutput.length;
        for (let i = 0; i < length; i++) {
            if (listOutput[i].eCommmerce === index) {
                listOutputTem.push(listOutput[i]);
            }
        }
    }
    // Show
    ShowListOutput(listOutputTem, document.getElementById("myTable"));
}

async function GetOutputOfProduct() {
    // Lấy id
    let id = GetValueFromUrlName("id");

    const searchParams = new URLSearchParams();
    searchParams.append("id", id);

    let query = "/Product/GetOutputOfProduct";

    let responseDB = await RequestHttpPostPromise(searchParams, query);

    if (responseDB.responseText != "null") {
        listOutput = JSON.parse(responseDB.responseText);
    }
    else {
        listOutput = [];
    }

    // Chọn Tất cả
    document.getElementById("all-e-ecommonerce-type").checked = true;

    // Show
    ShowListOutput(listOutput, document.getElementById("myTable"));
}

function ShowListOutput(list, table) {
    DeleteRowsExcludeHead(table);

    let length = list.length;
    // Số lượng sản phẩm thực tế bán.
    let realCount = 0;
    let normalOrderCount = 0;
    let cancelOrderCount = 0;

    if (length == 0) {
        document.getElementById("count-result").innerHTML =
            "Tổng " + length + " đơn. Đơn hoàn thành: " + normalOrderCount + ", đơn hủy: " + cancelOrderCount + " . Số lượng thực tế bán sau khi trừ hoàn hàng: " + realCount + " sản phẩm";
        return;
    }

    for (let i = 0; i < length; i++) {
        let obj = list[i];

        if (obj.isCancel)// đơn hoàn / hủy
        {
            cancelOrderCount++;
        }
        else {
            normalOrderCount++;
            realCount = realCount + obj.quantity;
        }

        let row = table.insertRow(-1);
        // Insert new cells (<td> elements)
        let cell1 = row.insertCell(0);
        let cell2 = row.insertCell(1);
        let cell3 = row.insertCell(2);
        let cell4 = row.insertCell(3);
        let cell5 = row.insertCell(4);

        // Id
        cell1.innerHTML = obj.id;
        cell1.style.display = "none";

        // Sàn
        cell2.innerHTML = GetEEcomnerceNameFromIntType(obj.eCommmerce);

        // Mã đơn hàng
        let p = document.createElement("p");
        p.innerHTML = obj.code;
        if (obj.isCancel)// đơn hoàn / hủy
        {
            p.style.color = "red";
            p.title = "Đơn hoàn / hủy";
        }
        //p.style.cursor = "pointer";
        //p.onclick = function () {
        //    let url = "/Product/UpdateDelete?id=" + obj.id.toString();
        //    window.open(url);
        //}
        cell3.append(p);

        // Số lượng
        cell4.innerHTML = obj.quantity;

        // Thời gian
        cell5.innerHTML = obj.time;
    }

    document.getElementById("count-result").innerHTML =
        "Tổng " + length + " đơn. Đơn hoàn thành: " + normalOrderCount + ", đơn hủy: " + cancelOrderCount + " . Số lượng thực tế bán sau khi trừ hoàn hàng: " + realCount + " sản phẩm";
}

async function CreateProductOnECommerce() {
    if (!IsValidString(document.getElementById("category-id").value)) {
        CreateMustClickOkModal("Chưa có thông tin thể loại");
        document.getElementById("category-id").focus();
        return;
    }

    if (!IsValidString(document.getElementById("publishing-company-id").value)) {
        CreateMustClickOkModal("Chưa có thông tin nhà xuất bản");
        document.getElementById("publishing-company-id").focus();
        return;
    }

    if (!IsValidString(document.getElementById("book-language-id").value)) {
        CreateMustClickOkModal("Chưa có thông tin ngôn ngữ");
        document.getElementById("book-language-id").focus();
        return;
    }

    // Sản phẩm phải có ảnh
    if (document.getElementsByClassName("objImage").length < 1) {
        CreateMustClickOkModal("Sản phẩm không có ảnh.");
        return;
    }

    // Sản phẩm phải có tối thiểu là 2 ảnh
    if (document.getElementsByClassName("objImage").length < 2) {
        let text = "Sản phẩm chỉ có 1 ảnh, bạn muốn ĐĂNG BÁN?";
        if (confirm(text) == false)
            return;
    }
    // Sản phẩm chưa set kích thước, hoặc khối lượng
    if (document.getElementById("product-long").value <= 0 ||
        document.getElementById("product-wide").value <= 0 ||
        document.getElementById("product-high").value <= 0 ||
        document.getElementById("product-weight").value <= 0) {
        CreateMustClickOkModal("Sản phẩm chưa set kích thước, hoặc khối lượng");
        return;
    }

    // Sản phẩm phải có mô tả dài ít nhất 100 ký tự
    if (document.getElementById("detail").value.length < 100) {
        CreateMustClickOkModal("Chưa có mô tả chi tiết sản phẩm hoặc ngắn hơn 100 ký tự");
        document.getElementById("detail").focus();
        return;
    }

    // Lấy id
    let id = GetValueFromUrlName("id");

    const searchParams = new URLSearchParams();
    searchParams.append("id", id);
    searchParams.append("eType", GetECommerceType());
    searchParams.append("name", document.getElementById("product-name-when-creating").value);

    let url = "/Product/CreateProductOnECommerce";

    try {
        // Cập nhật vào db
        ShowCircleLoader();
        let responseDB = await RequestHttpPostPromise(searchParams, url);
        RemoveCircleLoader();
        CheckStatusResponseAndShowPrompt(responseDB.responseText, "Thành công.", "Thất bại.");
    }
    catch (error) {
        CreateMustClickOkModal("Cập nhật lỗi.", null);
        return;
    }
}

async function UploadImageOnECommerce() {

    // Lấy id
    let id = GetValueFromUrlName("id");

    const searchParams = new URLSearchParams();
    searchParams.append("id", id);
    searchParams.append("eType", GetECommerceType());

    let url = "/Product/UploadImageOnECommerce";

    try {
        ShowCircleLoader();
        let responseDB = await RequestHttpGetPromise(searchParams, url);
        RemoveCircleLoader();
        CheckStatusResponseAndShowPrompt(responseDB.responseText, "Thành công.", "Thất bại.");
    }
    catch (error) {
        CreateMustClickOkModal("Cập nhật lỗi.", null);
        return;
    }
}