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
let isFinishUploadImageModel = 0;
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
function AddMappingButtonForModel(container) {
    let btn = document.createElement("BUTTON");
    let btnContent = document.createTextNode("Liên kết sản phẩm");
    btn.onclick = function () {
        modelMapping = this.parentElement.parentElement;
        // Get the modal
        let modal = document.getElementById("myModal");
        modal.style.display = "block";
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
        UncheckboxWhenDeleteProductMapping(this.parentElement.parentElement.children[0].innerHTML);
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
function AddToTableMapping(container, listObj) {
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

    let selectList  = document.createElement("SELECT");
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
    AddMappingButtonForModel(modelContainer);

    //AddDistanceRows(modelContainer);
    AddDistanceRows(modelList);
    //EasyViewListModel();
    return modelContainer;
}

// Chọn 1 ảnh làm thumbnail từ local
function SetThumbnail() {
    if (this.files.length){
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
        if (isEmptyOrSpaces(ls[i].value)){
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
    if ( table != null) {
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

    isFinishUploadImageModel++;
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

        isFinishUploadImageModel--;
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

        // Upload thông tin,ảnh model lên server
        isFinishUploadImageModel = 0;
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
        alert("Tạo sản phẩm lỗi.");
        RemoveCircleLoader();
        return;
    }

    // Đợi upload xong ảnh/video của item
    while (true) {
        await Sleep(1000);
        if (isFinishUploadImage == 0 && isFinishUploadVideo == 0) {
            break;
        }
    }

    // Đợi upload xong ảnh của model
    while (true) {
        await Sleep(1000);
        if (isFinishUploadImageModel == 0) {
            alert("Tạo sản phẩm thành công.");
            break;
        }
    }

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
        isFinishUploadImageModel = 0;
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
        await CreateMustClickOkModal("Cập nhật sản phẩm lỗi.", null);
        RemoveCircleLoader();
        return;
    }

    // Đợi upload xong ảnh/video của item
    while (true) {
        await Sleep(1000);
        if (isFinishUploadImage == 0 && isFinishUploadVideo == 0) {
            break;
        }
    }

    // Đợi upload xong ảnh của model
    while (true) {
        await Sleep(1000);
        if (isFinishUploadImageModel == 0) {
            alert("Cập nhật sản phẩm thành công.");
            break;
        }
    }

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
}

// pro là sản phẩm được chọn mapping từ bảng kết quả tìm kiếm
function DeleteRowFromTableMapping(pro) {
    if (pro == null)
        return;

    let rows = document.getElementById("myTableMapping").rows;
    let length = rows.length;
    for (let i = 0; i < length; i++) {
        if (Number(rows[i].cells[0].innerHTML) == pro.id) {
            // Xóa row này
            document.getElementById("myTableMapping").deleteRow(i);
        }
    }
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

async function SearchProduct() {
    let codeOrBarcode = document.getElementById("code-or-isbn").value;
    let name = document.getElementById("product-name-id").value;
    let combo = document.getElementById("combo-id").value;
    const searchParams = new URLSearchParams();
    searchParams.append("codeOrBarcode", codeOrBarcode);
    searchParams.append("name", name);
    searchParams.append("combo", combo);

    let url = "/ItemModel/SearchProduct";
    ShowCircleLoader();
    let resObj = await RequestHttpGetPromise(searchParams, url);
    RemoveCircleLoader();
    let listProduct = JSON.parse(resObj.responseText);

    // Làm trống bảng
    DeleteRowsExcludeHead(document.getElementById("myTable"));
    if (listProduct == null) {
        return;
    }

    // Show
    let table = document.getElementById("myTable");
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
        checkbox.onclick = function () {
            // Lấy id
            let id = Number(this.parentElement.previousSibling.innerHTML);
            let pro = FindProductFromList(listProduct, id);
            if (this.checked == true) {
                AddRowToTableMapping(pro, 1);
            }
            else {
                DeleteRowFromTableMapping(pro);
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
        AddToTableMapping(modelMapping, listObj);
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
