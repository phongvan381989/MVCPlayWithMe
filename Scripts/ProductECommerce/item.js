//------ code lấy lại từ file đã xóa: Scripts\web.play.with.me.item.model.js START--------

let modelList = document.getElementById("model-list");
let classOfModelName = "class-model-name config-max-width ";
let classOfModelQuota = "class-model-quota";
let classOfModelImage = "class-model-image";
let classOfModelStatus = "class-model-status";
let classOfModelTable = "class-model-table";
let classOfModelPrice = "class-model-price";
let classOfModelBookCoverPrice = "class-model-book-cover-price";
let classOfModelQuantity = "class-model-quantity";
let classOfModelDiscount = "class-model-discount";
let countModel = 0;
let item = null; // Khi chọn item để xem, cập nhật thông tin
// Giá trị này chỉ tăng, không giảm
// Tăng khi add thêm model, mục đích cấu thành id của các tag, để id là khác nhau
let autoIncrease = 0;
//let isFinishUploadImageModel = 0;
let modelMapping = null; // model đang chọn mapping

// Thêm nút xóa phân loại
function AddDeleteButtonForModel(container) {
    // Thêm nút xóa ảnh bên cạnh
    let btn = document.createElement("BUTTON");
    let btnContent = document.createTextNode("Xóa phân loại");
    btn.onclick = function () {

        let model = this.parentElement.parentElement;
        if (model.modelId == -1) { // Tạo mới, model chưa có trên server nên không cần hỏi
            countModel = countModel - 1;
            this.parentElement.parentElement.nextSibling.remove(); // Xóa tag <br>
            this.parentElement.parentElement.remove();
        }
        else {
            // Hỏi trước khi xóa dữ liệu trên server
            let text = "Bạn chắc chắn muốn xóa phân loại?";
            if (confirm(text) == false) {
                return;
            }
            let rs = DeleteModel(model.modelId);
            if (rs) {// Xóa trên server thành công
                countModel = countModel - 1;
                this.parentElement.parentElement.nextSibling.remove(); // Xóa tag <br>
                this.parentElement.parentElement.remove();
            }
        }
    }
    btn.appendChild(btnContent);
    btn.className = "margin-vertical";
    //btn.style.cssFloat = "right";
    const div = document.createElement("div");
    div.appendChild(btn);
    container.appendChild(div);
}

// Thêm nút liên kết sản phẩm
function AddMappingButtonForModel(container, tmdtItemName, tmdtModelName) {
    let btn = document.createElement("BUTTON");
    let btnContent = document.createTextNode("Liên kết sản phẩm");
    btn.onclick = function () {
        modelMapping = this.parentElement.parentElement;
        // Get the modal
        let modal = document.getElementById("myModal");
        modal.style.display = "block";
        document.getElementById("product-name-id").focus();
        if (IsValidString(tmdtItemName) || IsValidString(tmdtModelName)) {
            SearchProductFromTMDTNameForMapping(tmdtItemName, tmdtModelName);
        }
    }
    btn.appendChild(btnContent);
    btn.className = "margin-vertical";
    //btn.style.cssFloat = "right";
    const div = document.createElement("div");
    div.style.display = "flex";
    div.style.flexDirection = "row-reverse";
    div.appendChild(btn);
    container.appendChild(div);
}

// Thêm nút cập nhật mapping
function AddMappingUpdateButtonForModel(container) {
    let btn = document.createElement("BUTTON");
    let btnContent = document.createTextNode("Cập nhật liên kết");
    btn.addEventListener("click", function (event) {
        ModelUpdateMapping(event.currentTarget);
    });
    btn.appendChild(btnContent);
    btn.className = "margin-vertical";
    const div = document.createElement("div");
    div.style.display = "flex";
    div.style.flexDirection = "row-reverse";
    div.appendChild(btn);
    container.appendChild(div);
}

function AddModelNameUpdateButtonForModel(container) {
    let btn = document.createElement("BUTTON");
    let btnContent = document.createTextNode("Cập nhật tên");
    btn.addEventListener("click", function (event) {
        ModelUpdateName(event.currentTarget);
    });
    btn.appendChild(btnContent);
    btn.className = "margin-vertical";
    const div = document.createElement("div");
    div.appendChild(btn);
    container.appendChild(div);
}

// Thêm table mapping vào model
function CreateModelTableMapping(container) {
    const div = document.createElement("div");
    div.style.marginLeft = "50px";
    let tab = document.createElement("table");

    tab.className = classOfModelTable;

    div.appendChild(tab);
    container.appendChild(div);
    return tab;
}

// Constructor function for obj gồm: id, img, name, quantity của sản phẩm trong kho,
// eType: tên sàn, eImageSrc: đường dẫn ảnh đại diện trên sàn phục vụ chép ảnh từ sàn nếu cần thiết.
function objRowTableMapping(id, imageSrc, name, quantity, eType, eImageSrc) {
    this.id = id;
    this.imageSrc = imageSrc;
    this.name = name;
    this.quantity = quantity;
    this.eType = eType;
    this.eImageSrc = eImageSrc;
}

function InsertObjToTable(table, obj) {
    //if (DEBUG) {
    //    console.log("InsertObjToTable call: " + JSON.stringify(obj));
    //}
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

    // Image
    let img = document.createElement("img");
    img.setAttribute("src", Get320VersionOfImageSrc(obj.imageSrc));
    img.height = thumbnailHeight / 2;
    img.width = thumbnailWidth / 2;
    img.style.cursor = "pointer";
    img.title = "Click để cập nhật thông tin sản phẩm trong kho như: Vị trí lưu kho, mã sản phẩm";
    img.onclick = function () {
        let url = "/Product/UpdateDelete?id=" + obj.id.toString();
        window.open(url);
    }

    cell2.append(img);

    // Nếu sản phẩm trong kho chưa có ảnh
    // Ta hiển thị nút cho phép sao chép ảnh của sản phẩm trên sàn

    if (obj.imageSrc.includes(noImageThumbnailName) && obj.eType != "" && obj.eImageSrc != "") {
        let btn = document.createElement("BUTTON");
        let btnContent = document.createTextNode("Chép ảnh");
        btn.appendChild(btnContent);
        btn.style.marginRight = "10px";
        btn.style.marginLeft = "10px";

        btn.obj = obj;
        btn.title = "Sản phẩm trong kho đang không có ảnh nào. Sao chép ảnh sản phẩm trên sàn cho sản phẩm trong kho?"
        btn.onclick = function (event) {
            CopyImageFromTMDTToWarehouseProduct(
                event.target.obj.eType,
                event.target.obj.eImageSrc,
                event.target.obj.id);
        }
        cell2.append(btn);
    }

    // Tên
    cell3.innerHTML = obj.name;

    // Số lượng
    let quan = document.createElement("INPUT");
    quan.setAttribute("type", "number");
    quan.value = obj.quantity;
    quan.style.width = "40px";
    quan.onchange = function () {
        if (this.value < 1) {
            this.value = 1;
        }
    }
    cell4.appendChild(quan);

    // Nút xóa
    let btn = document.createElement("button");
    btn.onclick = function () {
        this.parentElement.parentElement.remove();
        //UncheckboxWhenDeleteProductMapping(this.parentElement.parentElement.children[0].innerHTML);
        UpdateSTT(table, 2, true);
    };
    btn.innerHTML = "Xóa";
    cell5.appendChild(btn)
}

// obj gồm: id, img, name, quantity
// Chưa tồn tại thì thêm vào table, ngược lại không làm gì
function CheckObjExistAndInsert(modelTable, obj) {
    let length = modelTable.rows.length;
    let exist = false;
    for (let i = 0; i < length; i++) {
        if (modelTable.rows[i].cells[0].innerHTML == obj.id) {
            // Cập nhật số lượng mới
            modelTable.rows[i].cells[3].children[0].value = obj.quantity;
            exist = true;
        }
    }
    if (exist === true) {
        return;
    }

    // Thêm vào table
    InsertObjToTable(modelTable, obj);
}

// listObj: sản phẩm được chọn từ modal gồm: id, img, name, quantity
// Lưu mapping được chọn tới model tương ứng
function AddToTableMappingOfModel(container, listObj) {
    // Check container chứa table mapping chưa? Nếu không thì tạo mới
    let modelTable;
    if (container.getElementsByClassName(classOfModelTable).length == 0) {
        CreateModelTableMapping(container);
    }
    modelTable = container.getElementsByClassName(classOfModelTable)[0];

    // Insert listObj vào table, có check obj đã tồn tại trong table theo id
    let length = listObj.length;
    for (let i = 0; i < length; i++) {
        CheckObjExistAndInsert(modelTable, listObj[i]);
    }

    UpdateSTT(modelTable, 2, false);
}

function AddDistanceRows(modelContainer) {
    modelContainer.appendChild(document.createElement("br"));
}

function AddLabelInput(container, label, id, inputType, disabled) {
    // Thêm tên
    const div = document.createElement("div");
    let lab = document.createElement("label");
    lab.htmlFor = id + autoIncrease;
    lab.innerHTML = label;

    let inp = document.createElement("INPUT");
    inp.setAttribute("type", inputType);
    inp.id = id + autoIncrease;
    //inp.className = "config-max-width margin-vertical";
    inp.disabled = disabled;
    // Set giá trị quota mặc định
    if (id == "id-quota-" || id == "id-quota-item") {
        inp.value = itemModelQuota;
        inp.className = classOfModelQuota;
    }
    // Set class cho input tên
    if (id == "id-name-") {
        inp.className = classOfModelName;
    }
    // Set class cho input giá bán
    if (id == "id-price-") {
        inp.className = classOfModelPrice;
    }
    // Set class cho input giá bìa
    if (id == "id-book-cover-price-") {
        inp.className = classOfModelBookCoverPrice;
    }
    // Set class cho input tồn kho
    if (id == "id-quatity-") {
        inp.className = classOfModelQuantity;
    }

    // Set class cho input chiết khấu
    if (id == "id-discount-") {
        inp.className = classOfModelDiscount;
    }
    div.appendChild(lab);
    div.appendChild(inp);
    container.appendChild(div);
}

function AddDiscount(container) {
    // Thêm tên
    const div = document.createElement("div");
    let lab = document.createElement("label");
    lab.htmlFor = "id-discount-" + autoIncrease;
    lab.innerHTML = "Chiết khấu:";

    let inp = document.createElement("INPUT");
    inp.setAttribute("type", "number");
    inp.id = "id-discount-" + autoIncrease;
    inp.className = classOfModelDiscount;
    inp.addEventListener("input", function (event) {
        ValidatePositiveIntegerNumber(event.currentTarget);
    });

    let labPercent = document.createElement("label");
    labPercent.innerHTML = "%";

    let btnUpdateDiscount = document.createElement("button");
    var t = document.createTextNode("Cập nhật chiết khấu");
    btnUpdateDiscount.appendChild(t);
    btnUpdateDiscount.addEventListener("click", function (event) {
        ModelUpdateDiscount(event.currentTarget);
    });

    div.appendChild(lab);
    div.appendChild(inp);
    div.appendChild(labPercent);
    div.appendChild(btnUpdateDiscount);

    container.appendChild(div);
}

async function ModelUpdateDiscount(element) {
    // modelId, discount
    let modelContainer = element.parentElement.parentElement;
    let modelId = modelContainer.modelId;
    let discount = modelContainer.getElementsByClassName(classOfModelDiscount)[0].value;
    //if (DEBUG) {
    //    console.log("ModelUpdateDiscount CALL ");
    //    console.log("element.tagName " + element.tagName);
    //    console.log("modelContainer.tagName " + modelContainer.tagName);
    //    console.log("modelId: " + modelId);
    //    console.log("discount: " + discount);
    //}

    let url = "/ItemModel/UpdateDiscount";
    const searchParams = new URLSearchParams();
    searchParams.append("modelId", modelId);
    searchParams.append("discount", discount);
    ShowCircleLoader();
    await RequestHttpPostPromise(searchParams, url);
    RemoveCircleLoader();
}

async function ModelUpdateMapping(element) {
    let modelContainer = element.parentElement.parentElement;
    let modelId = modelContainer.modelId;
    //if (DEBUG) {
    //    console.log("ModelUpdateMapping CALL ");
    //    console.log("element.tagName " + element.tagName);
    //    console.log("modelContainer.tagName " + modelContainer.tagName);
    //    console.log("modelId: " + modelId);
    //}
    let listProIdMapping = JSON.stringify(GetListProIdMapping(modelContainer.getElementsByClassName(classOfModelTable)[0]));
    let listQuantityMapping = JSON.stringify(GetListQuantityMapping(modelContainer.getElementsByClassName(classOfModelTable)[0]));

    let url = "/ItemModel/UpdateMapping";
    const searchParams = new URLSearchParams();
    searchParams.append("modelId", modelId);
    searchParams.append("listProIdMapping", listProIdMapping);
    searchParams.append("listQuantityMapping", listQuantityMapping);
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, url);
    RemoveCircleLoader();
    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật liên kết thành công.", "Có lỗi xảy ra.");
}

async function ModelUpdateName(element) {
    let modelContainer = element.parentElement.parentElement;
    let modelId = modelContainer.modelId;
    //if (DEBUG) {
    //    console.log("ModelUpdateMapping CALL ");
    //    console.log("element.tagName " + element.tagName);
    //    console.log("modelContainer.tagName " + modelContainer.tagName);
    //    console.log("modelId: " + modelId);
    //}
    let name = modelContainer.getElementsByClassName(classOfModelName)[0].value;

    let url = "/ItemModel/UpdateModelName";
    const searchParams = new URLSearchParams();
    searchParams.append("modelId", modelId);
    searchParams.append("name", name);
    ShowCircleLoader();
    await RequestHttpPostPromise(searchParams, url);
    RemoveCircleLoader();
}

function AddLabelSelectOfStatus(container, label, id) {
    // Thêm tên
    const div = document.createElement("div");
    let lab = document.createElement("label");
    lab.htmlFor = id + autoIncrease;
    lab.innerHTML = label;

    let selectList = document.createElement("SELECT");
    selectList.id = id + autoIncrease;
    selectList.className = "margin-vertical class-model-status";

    let option = document.createElement("option");
    option.value = 0;
    option.text = "Đang kinh doanh";
    option.selected = "selected";
    selectList.appendChild(option);

    let option1 = document.createElement("option");
    option1.value = 1;
    option1.text = "Ngừng kinh doanh";
    selectList.appendChild(option1);

    let option2 = document.createElement("option");
    option2.value = 2;
    option2.text = "Hết hàng";
    selectList.appendChild(option2);

    div.appendChild(lab);
    div.appendChild(selectList);
    container.appendChild(div);
}

function AddModelToScreen() {
    countModel = countModel + 1;
    autoIncrease = autoIncrease + 1;

    const modelContainer = document.createElement("div");
    modelContainer.style.marginLeft = "10px";
    modelContainer.style.backgroundColor = "#e6e6e6";
    modelContainer.style.padding = "10px 10px 10px 10px";
    modelContainer.style.borderRadius = "10px";
    modelContainer.className = "model-container";
    //li.onclick = function () { RemoveImage(this) };
    modelList.appendChild(modelContainer);
    modelContainer.modelId = -1;
    // Tên:
    AddLabelInput(modelContainer, "Tên:", "id-name-", "text", false);
    if (window.location.href.toUpperCase().includes("/ItemModel/UpdateDelete".toUpperCase())) {
        AddModelNameUpdateButtonForModel(modelContainer);
    }
    AddDistanceRows(modelContainer);

    // Thêm ảnh
    const imgDiv = document.createElement("div");
    const img = document.createElement("img");
    //img.src = "https://bit.ly/3jFwe3d";//URL.createObjectURL(this.files[i]);
    img.alt = "Chọn ảnh";
    img.src = "";
    img.file = null;
    img.fileName = "";
    img.exist = "false";
    img.className = classOfModelImage;
    img.height = thumbnailHeight;
    img.width = thumbnailWidth;
    img.addEventListener("click", (e) => {
        inputImage.click();
    }, false);

    imgDiv.appendChild(img);
    modelContainer.appendChild(imgDiv);

    // Thêm input chọn ảnh
    let inputImage = document.createElement("INPUT");
    inputImage.setAttribute("type", "file");
    inputImage.setAttribute("accept", "image/*");
    inputImage.setAttribute("style", "display:none");
    inputImage.addEventListener("change", SetThumbnail, false);
    imgDiv.appendChild(inputImage);

    AddDistanceRows(modelContainer);

    // Thêm chiết khấu
    if (window.location.href.toUpperCase().includes("/ItemModel/UpdateDelete".toUpperCase())) {
        AddDiscount(modelContainer);
    }
    else {
        AddLabelInput(modelContainer, "Chiết khấu:", "id-discount-", "number", false);
    }
    AddDistanceRows(modelContainer);

    // Thêm giá bán, giá bán lấy từ các chương trình khyến mại, giảm giá
    AddLabelInput(modelContainer, "Giá bán:", "id-price-", "number", true);
    AddDistanceRows(modelContainer);

    // Thêm giá bìa, thuộc tính này hiển thị chứ không cần nhập
    AddLabelInput(modelContainer, "Giá bìa:", "id-book-cover-price-", "number", true);
    AddDistanceRows(modelContainer);

    // Thêm số lượng trong kho
    AddLabelInput(modelContainer, "Số lượng:", "id-quatity-", "number", true);
    AddDistanceRows(modelContainer);

    // Status
    AddLabelSelectOfStatus(modelContainer, "Trạng thái:", "id-status-");
    AddDistanceRows(modelContainer);

    // Quota
    AddLabelInput(modelContainer, "Quota:", "id-quota-", "number", false);
    AddDistanceRows(modelContainer);

    // Thêm nút xóa bên cạnh
    AddDeleteButtonForModel(modelContainer);

    // Thêm nút mapping
    AddMappingButtonForModel(modelContainer, null, null);

    //AddDistanceRows(modelContainer);
    AddDistanceRows(modelList);
    //EasyViewListModel();
    return modelContainer;
}

// Chọn 1 ảnh làm thumbnail từ local
function SetThumbnail() {
    if (this.files.length) {
        let img = this.previousSibling;

        for (let i = 0; i < this.files.length; i++) {
            img.src = URL.createObjectURL(this.files[i]);
            img.file = this.files[i];
            img.fileName = this.files[i].name;
            img.onload = () => {
                URL.revokeObjectURL(img.src);
            }
            img.exist = false;
            //const info = document.createElement("span");
            //info.innerHTML = `${this.files[i].name}: ${this.files[i].size} bytes`;
            //li.appendChild(info);
        }
    }
}

function CheckValidModelName() {
    const ls = document.getElementsByClassName(classOfModelName);
    let length = ls.length;

    let isValid = true;
    for (let i = 0; i < length; i++) {
        if (isEmptyOrSpaces(ls[i].value)) {
            isValid = false;
            break;
        }

        for (let j = i + 1; j < length; j++) {
            if (ls[i].value === ls[j].value) {
                isValid = false;
                break;
            }
        }
    }
    return isValid;
}

// Model bắt buộc có 1 ảnh thumbnail
function CheckModelHasImage() {
    const ls = document.getElementsByClassName(classOfModelImage);
    let length = ls.length;

    let isValid = true;
    for (let i = 0; i < length; i++) {
        if (ls[i].src == null) {
            isValid = false;
            break;
        }
    }
    return isValid;
}

// modelList chứa cả <br> ta lấy list chỉ <div> container
function GetListModelOnly() {
    const ls = modelList.children;
    let length = ls.length;

    let listModelOnly = [];
    for (let i = 0; i < length; i++) {
        if (ls[i].tagName == "DIV") {
            listModelOnly.push(ls[i]);
        }
    }
    return listModelOnly;
}

function GetListModelId() {
    let listModelOnly = GetListModelOnly();
    let length = listModelOnly.length;

    let listModelId = [];
    for (let i = 0; i < length; i++) {
        listModelId.push(listModelOnly[i].modelId);
    }

    return JSON.stringify(listModelId);
}

function GetListProIdMapping(table) {
    let listProIdMapping = [];
    if (table != null) {
        let rows = table.rows;
        let length = rows.length;
        // Danh sách đối tượng lưu về db
        for (let i = 0; i < length; i++) {
            listProIdMapping.push(Number(rows[i].cells[0].innerHTML));
        }
    }

    return listProIdMapping;
}

function GetListQuantityMapping(table) {
    let listQuantityMapping = [];
    if (table != null) {
        let rows = table.rows;
        let length = rows.length;

        // Danh sách đối tượng lưu về db
        for (let i = 0; i < length; i++) {
            listQuantityMapping.push(Number(rows[i].cells[3].children[0].value));
        }
    }

    return listQuantityMapping;
}

// Thay đổi màu nền giúp dễ nhìn các model
function EasyViewListModel() {
    const ls = document.getElementsByClassName("model-container");

    for (let i = 0; i < ls.length; i++) {
        if ((i % 2) == 0) {
            ls[i].style.backgroundColor = "#cccccc";
        }
    }
}

// Thêm đối số cho item
// string name, int status, int quota, string detail
function AddItemParameters(searchParams) {
    let name = document.getElementById("item-name-id").value;
    searchParams.append("name", name);

    let status = document.getElementById("item-status-id").value;
    searchParams.append("status", status);

    let quota = document.getElementById("item-quota-id").value;
    searchParams.append("quota", quota);

    let detail = document.getElementById("detail-id").value;
    searchParams.append("detail", detail);

    let category = GetDataIdFromCategoryDatalist(document.getElementById("category-id").value);
    if (category == null) {
        category = 0;// giá trị mặc định
    }
    searchParams.append("categoryId", category);
}

// Thông tin gồm ảnh, tên, quota,...
// Tạo mới modelId = -1
function ModelUpload(url, model, modelId, fileElement, file, modelName, quota, exist,
    itemId, discount, imageExtension,
    listProIdMapping, listQuantityMapping) {

    //isFinishUploadImageModel++;
    //const reader = new FileReader();
    let parrent = fileElement.parentElement;
    parrent.ctrl = CreateThrobber(fileElement);
    const xhr = new XMLHttpRequest();
    parrent.xhr = xhr;

    const self = parrent;
    self.xhr.upload.addEventListener("progress", (e) => {
        if (e.lengthComputable) {
            const percentage = Math.round((e.loaded * 100) / e.total);
            self.ctrl.update(percentage);
        }
    }, false);

    //xhr.upload.addEventListener("load", (e) => {
    //    self.ctrl.update(100);

    //    isFinishUploadImageModel--;
    //}, false);

    xhr.open("POST", url);
    xhr.setRequestHeader("Content-Type", "multipart/form-data");
    xhr.setRequestHeader("modelId", modelId);
    // model name chứa tiếng việt, cần encode
    xhr.setRequestHeader("encodeModelName", encodeURI(modelName));
    xhr.setRequestHeader("exist", exist);
    xhr.setRequestHeader("quota", quota);
    xhr.setRequestHeader("itemId", itemId);
    xhr.setRequestHeader("discount", discount);
    xhr.setRequestHeader("imageExtension", imageExtension);
    xhr.setRequestHeader("listProIdMapping", listProIdMapping);
    xhr.setRequestHeader("listQuantityMapping", listQuantityMapping);
    //xhr.send(file);
    let response = RequestHttpPostUpFilePromise(xhr, url, file);
    response.then(function (resolve) {
        const obj = JSON.parse(resolve);
        model.modelId = obj.myAnything;
        self.ctrl.update(100);

        //isFinishUploadImageModel--;
    }, null);
}

function ItemModelCheckValidInputProperty() {
    if (isEmptyOrSpaces(document.getElementById("item-name-id").value)) {
        CreateMustClickOkModal("Tên sản phẩm không hợp lệ.", null);
        document.getElementById("item-name-id").focus();
        return false;
    }

    //if (GetDataIdFromCategoryDatalist(document.getElementById("category-id").value) === null) {
    //    CreateMustClickOkModal("Thể loại không hợp lệ.", null);
    //    document.getElementById("category-id").focus();
    //    return false;
    //}

    return true;
}

//Lưu item, model vào db
async function AddItemModel() {
    if (ItemModelCheckValidInputProperty() === false) {
        return;
    }

    ShowCircleLoader();
    //if (!CheckValidModelName()) {
    //    alert("Tên phân loại không hợp lệ.");
    //    return;
    //}

    //if (!CheckModelHasImage()) {
    //    alert("Phân loại không có ảnh đại diện.");
    //    return;
    //}

    const searchParams = new URLSearchParams();
    AddItemParameters(searchParams);

    let urlAdd = "/ItemModel/AddItem";
    let urlUpItem = "/ItemModel/UploadFileItem";
    let urlDeleteAllFileWithType = "";
    let urlUpModel = "/ItemModel/UploadFileModel";
    let itemId = 0;
    try {
        // Cập nhật item vào db
        let responseDB = await RequestHttpGetPromise(searchParams, urlAdd);
        const obj = JSON.parse(responseDB.responseText);
        if (obj == null || obj.myAnything == -1) {
            alert("Tạo sản phẩm (item) lỗi.");
            return;
        }
        itemId = obj.myAnything;

        // Upload ảnh/video item lên server
        let respinseSendFile = await SendFilesPromise(urlUpItem, urlDeleteAllFileWithType, itemId);
        if (!respinseSendFile) {
            alert("Upload ảnh/video (item) lỗi.");
            RemoveCircleLoader();
            return;
        }

        // Upload thông tin,ảnh model lên server
        //isFinishUploadImageModel = 0;
        let listModelOnly = GetListModelOnly();
        let length = listModelOnly.length;

        for (let i = 0; i < length; i++) {
            let model = listModelOnly[i];
            let img = model.getElementsByClassName(classOfModelImage)[0];
            let modelName = model.getElementsByClassName(classOfModelName)[0].value;
            let modelQuota = ConvertToInt(model.getElementsByClassName(classOfModelQuota)[0].value);

            //let modelPrice = ConvertToInt(model.getElementsByClassName(classOfModelPrice)[0].value);
            //let modelQuantity = ConvertToInt(model.getElementsByClassName(classOfModelQuantity)[0].value);
            let modelDiscount = ConvertToInt(model.getElementsByClassName(classOfModelDiscount)[0].value);
            let exist = img.exist;

            let listProIdMapping = JSON.stringify(GetListProIdMapping(model.getElementsByClassName(classOfModelTable)[0]));
            let listQuantityMapping = JSON.stringify(GetListQuantityMapping(model.getElementsByClassName(classOfModelTable)[0]));
            //let modelStatus = model.getElementsByClassName(classOfModelStatus)[0].value;
            let imageExtension = GetExtensionOfFileName(img.fileName);

            ModelUpload(urlUpModel, model, model.modelId, img, img.file, modelName, modelQuota,
                exist, itemId, modelDiscount,
                imageExtension, listProIdMapping, listQuantityMapping);
        }
    }
    catch (err) {
        alert("Tạo sản phẩm lỗi. " + err.Message);
        RemoveCircleLoader();
        return;
    }

    //// Đợi upload xong ảnh/video của item
    //while (true) {
    //    await Sleep(1000);
    //    if (isFinishUploadImage == 0 && isFinishUploadVideo == 0) {
    //        break;
    //    }
    //}

    // Đợi upload xong ảnh của model
    //while (true) {
    //    await Sleep(1000);
    //    if (isFinishUploadImageModel == 0) {
    //        alert("Tạo sản phẩm thành công.");
    //        break;
    //    }
    //}

    // Refresh page
    RemoveCircleLoader();
    window.scrollTo(0, 0);
    await Sleep(1000)
    //window.location.reload();
}

async function UpdateItemModel() {
    let itemId = item.id;
    if (itemId == null) {
        CreateMustClickOkModal("Sản phẩm không chính xác.", null);
        return;
    }

    if (ItemModelCheckValidInputProperty() === false) {
        return;
    }

    ShowCircleLoader();
    const searchParams = new URLSearchParams();
    searchParams.append("id", itemId);
    AddItemParameters(searchParams);

    let url = "/ItemModel/UpdateItem";
    let urlUpItem = "/ItemModel/UploadFileItem";
    let urlDeleteAllFileWithType = "/ItemModel/DeleteAllFileWithType";
    let urlUpModel = "/ItemModel/UploadFileModel";

    try {
        // Cập nhật vào db
        let responseDB = await RequestHttpPostPromise(searchParams, url);

        // Upload ảnh/video item lên server
        let respinseSendFile = await SendFilesPromise(urlUpItem, urlDeleteAllFileWithType, itemId);

        // Upload thông tin,ảnh model lên server
        //isFinishUploadImageModel = 0;
        let listModelOnly = GetListModelOnly();
        let length = listModelOnly.length;

        for (let i = 0; i < length; i++) {
            let model = listModelOnly[i];
            let img = model.getElementsByClassName(classOfModelImage)[0];
            let modelName = model.getElementsByClassName(classOfModelName)[0].value;
            let modelQuota = ConvertToInt(model.getElementsByClassName(classOfModelQuota)[0].value);

            //let modelPrice = ConvertToInt(model.getElementsByClassName(classOfModelPrice)[0].value);
            //let modelQuantity = ConvertToInt(model.getElementsByClassName(classOfModelQuantity)[0].value);
            let modelDiscount = ConvertToInt(model.getElementsByClassName(classOfModelDiscount)[0].value);
            let exist = img.exist;

            let listProIdMapping = JSON.stringify(GetListProIdMapping(model.getElementsByClassName(classOfModelTable)[0]));
            let listQuantityMapping = JSON.stringify(GetListQuantityMapping(model.getElementsByClassName(classOfModelTable)[0]));
            //let modelStatus = model.getElementsByClassName(classOfModelStatus)[0].value;
            let imageExtension = GetExtensionOfFileName(img.fileName);

            ModelUpload(urlUpModel, model, model.modelId, img, img.file, modelName,
                modelQuota, exist, itemId, modelDiscount,
                imageExtension, listProIdMapping, listQuantityMapping);
        }
    }
    catch (err) {
        //alert("Cập nhật sản phẩm lỗi.");
        await CreateMustClickOkModal("Cập nhật sản phẩm lỗi. " + err.Message, null);
        RemoveCircleLoader();
        return;
    }

    //// Đợi upload xong ảnh/video của item
    //while (true) {
    //    await Sleep(1000);
    //    if (isFinishUploadImage == 0 && isFinishUploadVideo == 0) {
    //        break;
    //    }
    //}

    //// Đợi upload xong ảnh của model
    //while (true) {
    //    await Sleep(1000);
    //    if (isFinishUploadImageModel == 0) {
    //        alert("Cập nhật sản phẩm thành công.");
    //        break;
    //    }
    //}

    // Refresh page
    RemoveCircleLoader();
    window.scrollTo(0, 0);
    await Sleep(1000)
    //window.location.reload();
}

async function DeleteModel(modelId) {

    let text = "Xóa model sản phẩm, mapping sản phẩm trong kho tương ứng, mapping sản phẩm trên Shopee, Tiki, Lazada tương ứng. Bạn chắc chắn muốn XÓA?";
    if (confirm(text) == false)
        return;

    const searchParams = new URLSearchParams();
    let itemId = GetValueFromUrlName("id");
    searchParams.append("itemId", itemId);
    searchParams.append("modelId", modelId);
    let query = "/ItemModel/DeleteModel";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    let rs = CheckStatusResponseAndShowPrompt(responseDB.responseText, "Xóa thành công.", "Có lỗi xảy ra.");
    return rs;
}

async function DeleteItemModel(id) {
    let text = "Xóa item, model thuộc item, mapping sản phẩm trong kho tương ứng, mapping sản phẩm trên Shopee, Tiki, Lazada tương ứng. Bạn chắc chắn muốn XÓA?";
    if (confirm(text) == false)
        return;

    const searchParams = new URLSearchParams();
    let itemId = GetValueFromUrlName("id");
    searchParams.append("itemId", itemId);

    let query = "/ItemModel/DeleteItem";

    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    let isOk = CheckStatusResponseAndShowPrompt(responseDB.responseText, "Xóa thành công.", "Có lỗi xảy ra.");
    if (isOk) {
        window.location.href = "/Administrator/Index";
    }
}

function CloseModal(modal) {
    modal.style.display = "none";
    EmptyModal();
}

function InitializeModal() {
    // Get the modal
    let modal = document.getElementById("myModal");

    //// Get the button that opens the modal
    //let btn = document.getElementById("myBtn");

    // Get the <span> element that closes the modal
    let span = document.getElementsByClassName("close")[0];

    //// When the user clicks the button, open the modal
    //btn.onclick = function () {
    //    modal.style.display = "block";
    //}

    // When the user clicks on <span> (x), close the modal
    span.onclick = function () {
        //modal.style.display = "none";
        //EmptyModal();
        CloseModal(modal);
    }

    //// When the user clicks anywhere outside of the modal, close it
    //window.onclick = function (event) {
    //    if (event.target == modal) {
    //        //modal.style.display = "none";
    //        //EmptyModal();
    //        CloseModal(modal);
    //    }
    //}
}

function EmptyModal() {
    document.getElementById("code-or-isbn").value = "";
    document.getElementById("product-name-id").value = "";
    document.getElementById("combo-id").value = "";

    // Làm trống bảng
    DeleteRowsExcludeHead(document.getElementById("myTable"));
    DeleteRowsExcludeHead(document.getElementById("myTableMapping"));

}

// pro là sản phẩm được chọn mapping từ bảng kết quả tìm kiếm
// Lưu mapping vào table tạm trước khi lưu tới model tương ứng
function AddRowToTableMapping(pro, quantity) {
    if (pro == null)
        return;

    let table = document.getElementById("myTableMapping");
    let src;
    if (pro.imageSrc.length > 0) {
        src = pro.imageSrc[0];
    } else {
        src = srcNoImageThumbnail;
    }
    let obj = new objRowTableMapping(pro.id, src, pro.name, quantity, "", "");

    CheckObjExistAndInsert(table, obj);
    UpdateSTT(table, 2, true);
}

// pro là sản phẩm được chọn mapping từ bảng kết quả tìm kiếm
function DeleteRowFromTableMapping(pro) {
    if (pro == null)
        return;

    let table = document.getElementById("myTableMapping");
    let rows = table.rows;
    let length = rows.length;
    for (let i = 0; i < length; i++) {
        if (Number(rows[i].cells[0].innerHTML) == pro.id) {
            // Xóa row này
            table.deleteRow(i);
        }
    }

    UpdateSTT(table, 2, true);
}

function FindProductFromList(listProduct, id) {
    let length = listProduct.length;
    for (let i = 0; i < length; i++) {
        if (listProduct[i].id == id) {
            return listProduct[i];
        }
    }
}

// Khi xóa sản phẩm đã chọn mapping, bỏ checkbox tương ứng
function UncheckboxWhenDeleteProductMapping(id) {
    let table = document.getElementById("myTable");
    let rows = table.rows;
    if (rows == null)
        return;
    let length = rows.length;
    for (let i = length - 1; i > 0; i--) {
        if (table.rows[i].cells[0].innerHTML == id) {
            table.rows[i].cells[1].children[0].checked = false;
            break;
        }
    }
}

function ClickSelectAllResultSearch() {
    const table = document.getElementById("myTable");
    const selectAllCheckbox = document.getElementById("select-all-result-search");
    const isChecked = selectAllCheckbox.checked;
    ToggleCheckboxes(table, '.row-checkbox', isChecked);
}

function ToggleCheckboxes(table, checkboxSelector, isChecked) {
    // Lấy tất cả các checkbox trong bảng dựa trên selector
    const checkboxes = table.querySelectorAll(checkboxSelector);

    // Duyệt qua từng checkbox và cập nhật trạng thái
    checkboxes.forEach(checkbox => {
        //checkbox.checked = isChecked; // Cập nhật trạng thái chọn/bỏ chọn
        if (checkbox.checked != isChecked) {
            checkbox.click(); // Kích hoạt sự kiện click
        }
    });
}

function OnChangeMyTable() {
    if (event.target.classList.contains('row-checkbox')) {
        const table = document.getElementById("myTable");
        const checkboxes = table.querySelectorAll('.row-checkbox');
        const allChecked = Array.from(checkboxes).every(checkbox => checkbox.checked);
        const selectAllCheckbox = document.getElementById("select-all-result-search");
        selectAllCheckbox.checked = allChecked;
    }
}

function ShowResultSearchProductForMapping(listProduct, table) {
    // Làm trống bảng
    DeleteRowsExcludeHead(table);
    // Bỏ chọn tất cả
    document.getElementById("select-all-result-search").checked = false;
    if (listProduct == null) {
        return;
    }

    // Show
    let length = listProduct.length;
    for (let i = 0; i < length; i++) {
        let pro = listProduct[i];
        let row = table.insertRow(-1);

        // Insert new cells (<td> elements)
        let cell1 = row.insertCell(0);
        let cell2 = row.insertCell(1);
        let cell3 = row.insertCell(2);
        let cell4 = row.insertCell(3);

        // Id
        cell1.innerHTML = pro.id;
        cell1.style.display = "none";

        // Checkbox
        let checkbox = document.createElement("INPUT");
        checkbox.setAttribute("type", "checkbox");
        checkbox.className = "row-checkbox";
        checkbox.pro = pro;
        checkbox.onclick = function () {
            // Lấy id
            //let id = Number(this.parentElement.previousSibling.innerHTML);
            //let pro = FindProductFromList(listProduct, id);
            if (this.checked == true) {
                AddRowToTableMapping(this.pro, 1);
            }
            else {
                DeleteRowFromTableMapping(this.pro);
            }
        }
        cell2.appendChild(checkbox)

        // Image
        let img = document.createElement("img");
        if (pro.imageSrc.length > 0) {
            img.setAttribute("src", Get320VersionOfImageSrc(pro.imageSrc[0]));
        } else {
            img.setAttribute("src", srcNoImageThumbnail);
        }
        img.height = thumbnailHeight;
        img.width = thumbnailWidth;
        cell3.append(img);

        // Tên
        cell4.innerHTML = pro.name;
    }

    UpdateSTT(table, 3, true);
}

async function SearchProductForMapping() {
    let codeOrBarcode = document.getElementById("code-or-isbn").value;
    let name = document.getElementById("product-name-id").value;
    let combo = document.getElementById("combo-id").value;
    const searchParams = new URLSearchParams();
    searchParams.append("codeOrBarcode", codeOrBarcode);
    searchParams.append("name", name);
    searchParams.append("combo", combo);

    let url = "/Product/SearchProductForMapping";
    ShowCircleLoader();
    let resObj = await RequestHttpGetPromise(searchParams, url);
    RemoveCircleLoader();
    let listProduct = JSON.parse(resObj.responseText);
    let table = document.getElementById("myTable");

    ShowResultSearchProductForMapping(listProduct, table);
}

async function SearchProductFromTMDTNameForMapping(tmdtItemName, tmdtModelName) {
    const searchParams = new URLSearchParams();
    searchParams.append("tmdtItemName", tmdtItemName);
    searchParams.append("tmdtModelName", tmdtModelName);
    //if (DEBUG) {
    //    console.log("SearchProductFromTMDTNameForMapping Call tmdtName: " + tmdtName);
    //}

    let url = "/Product/SearchProductFromTMDTNameForMapping";
    ShowCircleLoader();
    let resObj = await RequestHttpGetPromise(searchParams, url);
    RemoveCircleLoader();
    let listProduct = JSON.parse(resObj.responseText);
    let table = document.getElementById("myTable");

    ShowResultSearchProductForMapping(listProduct, table);

    // Nếu listProduct chỉ có một sản phẩm, ta tự động thêm vào bảng liên kết
    if (listProduct != null && listProduct.length == 1) {
        document.getElementsByClassName("row-checkbox")[0].click();
    }
}

// 2 trường hợp dựa vào url:
// 1: save mapping tới sản phẩm trên web voibenho
// 2: save mapping tới sản phẩm trên sàn shopee, tiki, lazada
function SaveMappingToModel() {
    // Lấy danh sách sản phẩm đã chọn trên modal
    let rows = document.getElementById("myTableMapping").rows;
    let length = rows.length;
    if (length == 0) {
        return;
    }

    // Danh sách đối tượng lưu về db
    let listObj = [];
    for (let i = 1; i < length; i++) {
        let obj = new objRowTableMapping(
            Number(rows[i].cells[0].innerHTML),
            rows[i].cells[1].children[0].src,
            rows[i].cells[2].innerHTML,
            rows[i].cells[3].children[0].value,
            "",
            ""
        );
        listObj.push(obj);
    }

    // Mapping tới model
    if (modelMapping != null) {
        AddToTableMappingOfModel(modelMapping, listObj);
    }

    // Đóng modal
    let modal = document.getElementById("myModal");
    CloseModal(modal);
}

async function GetItemObjectFromId(id) {
    const searchParams = new URLSearchParams();
    searchParams.append("id", id);

    let query = "/ItemModel/GetItemObjectFromId";

    return RequestHttpPostPromise(searchParams, query);
}

function AddItemNameUpdateButtonForModel(container) {
    let btn = document.createElement("BUTTON");
    let btnContent = document.createTextNode("Cập nhật tên");
    btn.addEventListener("click", function (event) {
        ItemUpdateName(event.currentTarget);
    });
    btn.appendChild(btnContent);
    btn.className = "margin-vertical";
    const div = document.createElement("div");
    div.appendChild(btn);
    container.appendChild(div);
}

async function ItemUpdateName() {
    let itemId = GetValueFromUrlName("id");
    if (isEmptyOrSpaces(itemId)) {
        CreateMustClickOkModal("Định danh sản phẩm lỗi", null);
        return;
    }

    let name = document.getElementById("item-name-id").value;
    if (isEmptyOrSpaces(name)) {
        CreateMustClickOkModal("Tên sản phẩm lỗi", null);
        document.getElementById("item-name-id").focus();
        return;
    }

    let url = "/ItemModel/UpdateItemName";
    const searchParams = new URLSearchParams();
    searchParams.append("itemId", itemId);
    searchParams.append("name", name);
    ShowCircleLoader();
    await RequestHttpPostPromise(searchParams, url);
    RemoveCircleLoader();
}

async function ItemUpdateCategory() {
    let itemId = GetValueFromUrlName("id");
    if (isEmptyOrSpaces(itemId)) {
        CreateMustClickOkModal("Định danh sản phẩm lỗi", null);
        return;
    }

    let categoryId = GetDataIdFromCategoryDatalist(document.getElementById("category-id").value);
    if (categoryId === null) {
        CreateMustClickOkModal("Thể loại không hợp lệ.", null);
        document.getElementById("category-id").focus();
        return;
    }

    let url = "/ItemModel/UpdateItemCategory";
    const searchParams = new URLSearchParams();
    searchParams.append("itemId", itemId);
    searchParams.append("categoryId", categoryId);
    ShowCircleLoader();
    await RequestHttpPostPromise(searchParams, url);
    RemoveCircleLoader();
}

function ShowCategoryFromCategoryId(categoryId) {
    //if (DEBUG){
    //    console.log("ShowCategoryFromCategoryId CALL");
    //    console.log("categoryId: " + categoryId);
    //}
    if (categoryId == 0)// giá trị mặc định chưa chọn thể loại
        return;

    let option = document.getElementById("list-category").options;
    if (option == null)
        return null;

    let length = option.length;
    for (let i = 0; i < length; i++) {
        if (option.item(i).getAttribute("data-id") == categoryId) {
            document.getElementById("category-id").value = option.item(i).value;
            break;
        }
    }
}

// Từ item object hiển thị ra màn hình
async function ShowItemFromItemObject() {

    let responseDB = await GetItemObjectFromId(GetValueFromUrlName("id"));
    if (responseDB.responseText != "null") {
        GetListProductName();
        GetListCombo();
        item = JSON.parse(responseDB.responseText);
    }
    else {
        item = null;
        ShowDoesntFindId();
        return;
    }

    // Hiển thị dữ liệu, image, video của item
    document.getElementById("item-name-id").value = item.name;
    if (window.location.href.toUpperCase().includes("/ItemModel/UpdateDelete".toUpperCase())) {
        document.getElementById("afx902njnf").style.display = "initial";
        document.getElementById("gjdtc78dhjc").style.display = "initial";

        // Lấy danh sách thể loại
        const searchParams = new URLSearchParams();

        let query = "/Category/GetListCategory";

        let responseDB = await RequestHttpPostPromise(searchParams, query);
        let list = null;
        if (responseDB.responseText != "null") {
            list = JSON.parse(responseDB.responseText);
            let ele = document.getElementById("list-category");
            SetDataListOfIdName(ele, list);
        }
        ShowCategoryFromCategoryId(item.categoryId);
    }
    document.getElementById("item-status-id").value = item.status;
    document.getElementById("item-quota-id").value = item.quota;
    document.getElementById("detail-id").value = item.detail;

    InitializeImageList(item.imageSrc);
    // Vì item.videoSrc không phải array, cần chuyển sang array
    let lsVideo = [];
    if (!isEmptyOrSpaces(item.videoSrc)) {
        lsVideo.push(item.videoSrc);
    }
    InitializeVideoList(lsVideo);

    // Hiển thị các model
    let length = item.models.length;
    countModel = 0;
    for (let i = 0; i < length; i++) {
        // Hiển thị mapping sản phẩm trong kho nếu có
        let modelObj = item.models[i];

        let model = AddModelToScreen();
        model.modelId = item.models[i].id;
        // Hiển thị dữ liệu input
        model.getElementsByClassName(classOfModelName)[0].value = modelObj.name;
        model.getElementsByClassName(classOfModelQuota)[0].value = modelObj.quota;
        model.getElementsByClassName(classOfModelPrice)[0].value = modelObj.price;
        model.getElementsByClassName(classOfModelBookCoverPrice)[0].value = modelObj.bookCoverPrice;
        model.getElementsByClassName(classOfModelQuantity)[0].value = modelObj.quantity;
        model.getElementsByClassName(classOfModelDiscount)[0].value = modelObj.discount;
        model.getElementsByClassName(classOfModelStatus)[0].value = modelObj.status;

        // Hiển thị thumbnail image
        let img = model.getElementsByClassName(classOfModelImage)[0];
        if (modelObj.imageSrc != null) {
            img.src = modelObj.imageSrc;
        }
        else {
            img.src = srcNoImageThumbnail;
        }
        img.file = null;
        img.exist = true;

        // Hiển thị mapping
        let table = CreateModelTableMapping(model);

        let listObj = [];
        for (let j = 0; j < modelObj.mapping.length; j++) {
            let src;
            if (modelObj.mapping[j].product.imageSrc.length > 0) {
                src = modelObj.mapping[j].product.imageSrc[0];
            }
            else {
                src = srcNoImageThumbnail;
            }

            let obj = new objRowTableMapping(
                Number(modelObj.mapping[j].product.id),
                src,
                modelObj.mapping[j].product.name,
                Number(modelObj.mapping[j].quantity),
                "",
                ""
            );

            listObj.push(obj);
        }

        for (let j = 0; j < listObj.length; j++) {
            CheckObjExistAndInsert(table, listObj[j]);
        }

        UpdateSTT(table, 2, false);

        // Thêm nút cập nhật mapping
        if (window.location.href.toUpperCase().includes("/ItemModel/UpdateDelete".toUpperCase())) {
            AddMappingUpdateButtonForModel(model);
        }
    }
}

async function GetItemFromId(eType, id) {
    const searchParams = new URLSearchParams();
    searchParams.append("eType", eType);
    searchParams.append("id", id);

    let query = "/ProductECommerce/GetItemFromId";

    return RequestHttpPostPromise(searchParams, query);
}
// -----------------END ----------------------



InitializeModal();
ViewItemFromId();
let commonItem = null;
let vbnItemId = 0;

function GetEEcommerceTypeFromUrl() {
    let type = eShopee;

    if (window.location.href.includes("SHOPEE")) {
        type = eShopee;
    }
    else if (window.location.href.includes("TIKI")) {
        type = eTiki;
    }
    else if (window.location.href.includes("LAZADA")) {
        type = eLazada;
    }
    return type;
}

function GetQuantityFromListMapping(mapping) {
    if (DEBUG) {
        console.log("GetQuantityFromListMapping Call");
        console.log("mapping: " + JSON.stringify(mapping));
    }
    let qty = Number.MAX_SAFE_INTEGER;

    if (mapping.length === 0) {
        qty = 0;
    }

    for (const m of mapping) {
        const calculatedQty = Math.floor(m.product.quantity / m.quantity);
        if (qty > calculatedQty) {
            qty = calculatedQty;
        }
    }

    if (qty < 0) {
        qty = 0;
    }

    return qty;
}

async function ViewItemFromId() {
    ShowCircleLoader();

    let responseDB = await GetItemFromId(GetEEcommerceTypeFromUrl(), GetValueFromUrlName("id"));
    RemoveCircleLoader();
    if (responseDB.responseText != "null") {
        GetListProductName();
        GetListCombo();
        commonItem = JSON.parse(responseDB.responseText);
    }
    else {
        ShowDoesntFindId();
        return;
    }

    document.getElementById("anzka89zbhfj").href = GetTMDTItemUrlFromCommonItem(commonItem);

    let tikiIsVirtualParent = false;
    // Hiển thị sản phẩm của sàn
    if (commonItem.imageSrc) {
        document.getElementsByClassName("item-image-ACvv")[0].src = commonItem.imageSrc;
    }
    else {
        document.getElementsByClassName("item-image-ACvv")[0].src = srcNoImageThumbnail;
        if (commonItem.eType === eTiki) {
            tikiIsVirtualParent = true;
        }
    }
    document.getElementById("item-name-vsfc").innerHTML = commonItem.name;
    let itemStatus;
    if (commonItem.bActive) {
        itemStatus = "Đang Bật Bán Trên Sàn";
    }
    else {
        itemStatus = "Đang Tắt Bán Trên Sàn";
    }

    document.getElementById("item-status-azc").innerHTML = itemStatus;

    UpdateModelNameIfNeed(commonItem);

    // Nếu sản phẩm tiki và là cha chung ảo hiện thông báo và không hiển thị model, mapping
    if ((commonItem.eType === eTiki && commonItem.itemId == commonItem.tikiSuperId)
        || tikiIsVirtualParent) {
        itemStatus = itemStatus + ". --------->Đây Có Thể Là Sản Phẩm Cha Ảo, Không Được Insert Vào DB";
        document.getElementById("item-status-azc").innerHTML = itemStatus;
        document.getElementById("update-eEcommerce-mapping").style.display = "none";
        return;
    }

    // Hiển thị danh sách mapping
    ShowModelEEcommerce(commonItem);
}

function UpdateModelNameIfNeed(item) {
    // SHOPEE Nếu Item có nhiều model, tên model có định dạng VD: TÊN SÁCH--Xình xịch Xình xịch,
    // ta cập nhật tên mới: Xình xịch Xình xịch
    if (item.eType == eShopee) {
        for (let i = 0; i < item.models.length; i++) {
            if (item.models[i].name != null && item.models[i].name.includes("--")) {
                const myArray = item.models[i].name.split("--");
                item.models[i].name = myArray[myArray.length - 1];
            }
        }
    }
}

// Thêm dòng hiển thị số lượng trên sàn, số lượng thực tế trong kho
function AddQuantityInfor(modelObj, container) {
    let p = document.createElement("p");
    p.innerHTML = "Số lượng trên sàn: " + modelObj.quantity_sellable +
        " .Tồn trong kho: " + GetQuantityFromListMapping(modelObj.mapping);
    const div = document.createElement("div");
    div.appendChild(p);
    container.appendChild(div);
}

// Thêm nút sinh model trên web voibenho
function AddBornModelForVoiBeNhoButton(itemObj, modelObj, container) {
    // let btn = document.createElement("BUTTON");
    // btn.title = "Sinh model sản phẩm tương ứng trên web voibenho";
    // let btnContent = document.createTextNode("Sinh model");
    // btn.itemObj = itemObj;
    // btn.modelObj = modelObj;

    // btn.onclick = function () {
    //     if (this.modelObj.pWMMappingModelId != -1 && this.modelObj.pWMMappingModelId != 0) {
    //         if (confirm("Bạn CHẮC CHẮN muốn XÓA model sản phẩm đã sinh và sinh mới?") == false) {
    //             return;
    //         }
    //     }
    //     ShopeeBornModelForVoiBeNho(JSON.stringify(this.itemObj),
    //         this.modelObj.modelId,
    //         this.modelObj.pWMMappingModelId, this);
    // }
    // btn.appendChild(btnContent);
    // //btn.className = "margin-vertical";
    // //btn.style.cssFloat = "right";

    // const div = document.createElement("div");
    // //div.style.display = "flex";
    // //div.style.flexDirection = "row-reverse";

    // // Đã sinh ra model tương ứng trên web voibenho
    // if (modelObj.pWMMappingModelId != -1 && modelObj.pWMMappingModelId != 0) {
    //     const p = document.createElement("p");
    //     p.innerHTML = "Đã sinh trên voibenho";
    //     p.title = "Xem sản phẩm tương ứng trên voibenho";
    //     p.style.cursor = "pointer";
    //     p.onclick = async function () {
    //         if (vbnItemId == 0) {
    //             const searchParams = new URLSearchParams();
    //             searchParams.append("modelId", modelObj.pWMMappingModelId);
    //             let query = "/ItemModel/GetVBNItemIdFromModelId";

    //             let responseDB = await RequestHttpPostPromise(searchParams, query);

    //             if (responseDB.responseText != "null") {
    //                 let result = JSON.parse(responseDB.responseText);
    //                 //if (DEBUG) {
    //                 //    console.log(JSON.stringify(result));
    //                 //}
    //                 if (result.State == 0) {
    //                     vbnItemId = result.myAnything;
    //                 }
    //                 else {
    //                     CreateMustClickOkModal(result.Message, null);
    //                     return;
    //                 }
    //             }
    //         }
    //         if (vbnItemId != 0) {
    //             let url = "/ItemModel/UpdateDelete?id=" + vbnItemId.toString();
    //             window.open(url);
    //         }
    //         else {
    //             CreateMustClickOkModal("Không lấy được thông tin sản phẩm. Thử lại sau.", null);
    //         }
    //     }
    //     div.appendChild(p);
    // }
    // div.appendChild(btn);
    // container.appendChild(div);
}

// Thêm nút Cập nhật giá bìa 
function AddUpdateBookCoverPriceButton(itemObj, container) {
    let btn = document.createElement("BUTTON");
    btn.title = "Cập nhật giá bìa";
    let btnContent = document.createTextNode("Cập nhật giá bìa");
    btn.itemObj = itemObj;

    btn.onclick = async function () {
        await UpdateBookCoverPriceToEEcommerce(JSON.stringify(this.itemObj));
    }
    btn.appendChild(btnContent);
    const div = document.createElement("div");

    div.appendChild(btn);
    container.appendChild(div);
}

async function ModelUpdateMapping_ProductEEcomerce(element) {
    let listModelOnly = [];
    listModelOnly.push(element.parentElement.parentElement);
    await UpdateEEcommerceMapping_Core(listModelOnly);
}

// Thêm nút cập nhật mapping
function AddMappingUpdateButtonForModel_ProductEEcomerce(container) {
    let btn = document.createElement("BUTTON");
    let btnContent = document.createTextNode("Cập nhật liên kết");
    btn.addEventListener("click", async function (event) {
        let listModelOnly = [];
        listModelOnly.push(event.currentTarget.parentElement.parentElement);
        await UpdateEEcommerceMapping_Core(listModelOnly);
    });
    btn.appendChild(btnContent);
    btn.className = "margin-vertical";
    const div = document.createElement("div");
    div.style.display = "flex";
    div.style.flexDirection = "row-reverse";
    div.appendChild(btn);
    container.appendChild(div);
}

// modelIndex: Stt từ 1 của model trong item
function AddModelToScreenEEcommerce(itemObj, modelObj, modelIndex) {
    const modelContainer = document.createElement("div");
    modelContainer.style.marginLeft = "10px";
    modelContainer.style.backgroundColor = "#e6e6e6";
    modelContainer.style.padding = "10px 10px 10px 10px";
    modelContainer.style.borderRadius = "10px";
    modelContainer.className = "model-container";
    modelContainer.modelId = modelObj.modelId; // Tiki mặc định là -1 vì không có phân loại

    modelList.appendChild(modelContainer);

    // Thêm dòng hiển thị số lượng trên sàn, số lượng thực tế trong kho
    AddQuantityInfor(modelObj, modelContainer);

    // model không phải là item
    if (modelObj.modelId != -1) {
        const div = document.createElement("div");

        // Ảnh đại diện
        let imgDiv = document.createElement("img");
        imgDiv.height = thumbnailHeight;
        imgDiv.width = thumbnailWidth;
        if (modelObj.imageSrc) {
            imgDiv.src = modelObj.imageSrc;
        }
        else {
            imgDiv.src = srcNoImageThumbnail;
        }
        imgDiv.title = "STT: " + modelIndex;

        // Tên:
        let nameDiv = document.createElement("div");
        nameDiv.innerHTML = modelObj.name;
        nameDiv.style.marginLeft = "20px";

        div.appendChild(imgDiv);
        div.appendChild(nameDiv);
        div.style.display = "flex";
        div.style.alignItems = "center";
        modelContainer.appendChild(div);
    }
    // Tạm thời ẩn
    // Thêm nút sinh model trên web voibenho khi là sản phẩm Shopee
    // if (GetEEcommerceTypeFromUrl() == eShopee) {
    //     AddBornModelForVoiBeNhoButton(itemObj, modelObj, modelContainer);
    // }

    // Thêm nút cập nhật giá bìa
    //if (GetEEcommerceTypeFromUrl() == eTiki) {
    //    AddUpdateBookCoverPriceButton(itemObj, modelContainer);
    //}

    // Thêm nút mapping
    //let tmdtName = itemObj.name;
    //if (IsValidString(modelObj.name)) {
    //    tmdtName = tmdtName + " - " + modelObj.name;
    //}
    AddMappingButtonForModel(modelContainer, itemObj.name, modelObj.name);

    AddDistanceRows(modelContainer);
    AddDistanceRows(modelList);

    return modelContainer;
}

function ShowModelEEcommerce(itemObj) {
    // Hiển thị các model
    let length = itemObj.models.length;
    countModel = 0;
    for (let i = 0; i < length; i++) {
        // Hiển thị mapping sản phẩm trong kho nếu có
        let modelObj = itemObj.models[i];

        let model = AddModelToScreenEEcommerce(itemObj, modelObj, i + 1);

        // Hiển thị mapping
        let table = CreateModelTableMapping(model);
        let listObj = [];
        for (let j = 0; j < modelObj.mapping.length; j++) {
            let src = "";
            if (modelObj.mapping[j].product.imageSrc.length > 0) {
                src = Get320VersionOfImageSrc(modelObj.mapping[j].product.imageSrc[0]);
            }
            else {
                src = srcNoImageThumbnail;
            }

            let eImageSrc = "";
            if (modelObj.imageSrc) {
                eImageSrc = modelObj.imageSrc;
            }

            let obj = new objRowTableMapping(
                Number(modelObj.mapping[j].product.id),
                src,
                modelObj.mapping[j].product.name,
                Number(modelObj.mapping[j].quantity),
                itemObj.eType,
                eImageSrc
            );

            listObj.push(obj);
        }

        for (let j = 0; j < listObj.length; j++) {
            CheckObjExistAndInsert(table, listObj[j]);
        }

        UpdateSTT(table, 2, false);
        AddMappingUpdateButtonForModel_ProductEEcomerce(model);
    }
}

async function UpdateEEcommerceMapping_Core(listModelOnly) {
    const searchParams = new URLSearchParams();
    searchParams.append("eType", GetEEcommerceTypeFromUrl());

    let url = "/ProductECommerce/UpdateMapping";
    // tham số string gửi về server có dạng:
    // itemId,modelId,productId,productQuantity,...,itemId,modelId,productId,productQuantity
    let itemid = GetValueFromUrlName("id");
    let str = "";

    let length = listModelOnly.length;

    for (let i = 0; i < length; i++) {
        let model = listModelOnly[i];

        let listProIdMapping = GetListProIdMapping(model.getElementsByClassName(classOfModelTable)[0]);
        let listQuantityMapping = GetListQuantityMapping(model.getElementsByClassName(classOfModelTable)[0]);

        if (listProIdMapping.length == 0) {
            if (!isEmptyOrSpaces(str)) {
                str = str + ",";
            }
            str = str + itemid + "," + model.modelId + ",,"; // productId,productQuantity trống
        }
        else {
            for (let j = 0; j < listProIdMapping.length; j++) {
                if (!isEmptyOrSpaces(str)) {
                    str = str + ",";
                }
                str = str + itemid + "," + model.modelId + ","
                    + listProIdMapping[j] + "," + listQuantityMapping[j];
            }
        }
    }
    searchParams.append("str", str);

    try {
        ShowCircleLoader();
        let responseDB = await RequestHttpPostPromise(searchParams, url);
        RemoveCircleLoader();

        CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật thành công.", "Cập nhật lỗi.");
    }
    catch (err) {
        //alert("Cập nhật sản phẩm lỗi.");
        await CreateMustClickOkModal("Cập nhật sản phẩm lỗi. " + err.Message, null);
    }
    return GetEasyPromise();
}

async function UpdateEEcommerceMapping() {
    if (commonItem == null) {
        CreateMustClickOkModal("Sản phẩm không chính xác.", null);
        return;
    }
    let listModelOnly = GetListModelOnly();
    await UpdateEEcommerceMapping_Core(listModelOnly);
    window.scrollTo(0, 0);
}

async function UpdateQuantityPrice_SpecialPrice() {
    if (GetEEcommerceTypeFromUrl() != eLazada) {
        CreateMustClickOkModal("Không hỗ trợ ngoài sàn " + eLazada, null);
        return;
    }
    const searchParams = new URLSearchParams();
    searchParams.append("eType", GetEEcommerceTypeFromUrl());
    searchParams.append("id", GetValueFromUrlName("id"));

    let url = "/ProductECommerce/UpdateQuantityPrice_SpecialPrice";

    try {
        ShowCircleLoader();
        let responseDB = await RequestHttpGetPromise(searchParams, url);
        RemoveCircleLoader();

        CheckStatusResponseAndShowPrompt(responseDB.responseText, "Cập nhật thành công.", "Cập nhật lỗi.");
    }
    catch (err) {
        await CreateMustClickOkModal("Cập nhật lỗi. " + err.Message, null);
    }
    return GetEasyPromise();
}

// async function ShopeeBornModelForVoiBeNho(strCommonItem, shopeeModelId, pWMMappingModelId, btnElement) {
//     ShowCircleLoader();
//     const searchParams = new URLSearchParams();
//     searchParams.append("strCommonItem", strCommonItem);
//     searchParams.append("shopeeModelId", shopeeModelId);
//     searchParams.append("pWMMappingModelId", pWMMappingModelId);
//     let url = "/ProductECommerce/ShopeeBornModelForVoiBeNho";

//     try {
//         let responseDB = await RequestHttpPostPromise(searchParams, url);
//         let result = JSON.parse(responseDB.responseText);
//         if (result.State != 0) {
//             await CreateMustClickOkModal(result.Message, null);
//         }
//         else {
//             alert("Sinh model sản phẩm thành công.");
//             // Nếu chưa có dòng "Đã sinh trên voibenho" thì thêm, ngược lại xóa bỏ dòng cũ thêm
//             // "Đã sinh trên voibenho" mới. Dòng mới không click được để đến trang
//             // thông tin sản phẩm trên voibenho
//             let parrent = btnElement.parentElement;
//             // Xóa dòng cũ
//             if (parrent.children.length == 2) {
//                 parrent.children[0].remove();
//             }
//             // Thêm dòng mới
//             const p = document.createElement("p");
//             p.innerHTML = "Đã sinh trên voibenho";
//             p.title = "Load lại trang để xem sản phẩm tương ứng trên voibenho";
//             parrent.insertBefore(p, parrent.children[0]);
//         }
//     }
//     catch (err) {
//         await CreateMustClickOkModal("Sinh model sản phẩm lỗi. " + err.Message, null);
//         RemoveCircleLoader();
//         return;
//     }

//     RemoveCircleLoader();
// }

async function UpdateBookCoverPriceToEEcommerce(strCommonItem) {
    const searchParams = new URLSearchParams();
    searchParams.append("strCommonItem", strCommonItem);
    let url = "/ProductECommerce/UpdateBookCoverPriceToEEcommerce";
    ShowCircleLoader();
    let responseDB = await RequestHttpPostPromise(searchParams, url);
    RemoveCircleLoader();
    CheckStatusResponseAndShowPrompt(responseDB.responseText, "Thành công.", "Thất bại.");
}
