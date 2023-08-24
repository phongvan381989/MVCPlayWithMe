let modelList = document.getElementById("model-list");
let classOfModelName = "class-model-name";
let classOfModelQuota = "class-model-quota";
let classOfModelImage = "class-model-image";
let classOfModelStatus = "class-model-status";
let countModel = 0;
let isFinishUploadImageModel = 0;

// Thêm nút xóa phân loại
function AddDeleteButtonForModel(container) {
    // Thêm nút xóa ảnh bên cạnh
    var btn = document.createElement("BUTTON");
    var btnContent = document.createTextNode("Xóa phân loại");
    btn.onclick = function () {
        countModel = countModel - 1;
        this.parentElement.parentElement.remove();
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
    // Thêm nút xóa ảnh bên cạnh
    var btn = document.createElement("BUTTON");
    var btnContent = document.createTextNode("Liên kết sản phẩm");
    btn.onclick = function () {
        modal.style.display = "block";
    }
    btn.appendChild(btnContent);
    btn.className = "margin-vertical";
    btn.style.cssFloat = "right";
    const div = document.createElement("div");
    div.appendChild(btn);
    container.appendChild(div);
}

function AddDistanceRows(modelContainer) {
    modelContainer.appendChild(document.createElement("br"));
}

function AddLabelInput(container, label, id, inputType, disabled) {
    // Thêm tên
    const div = document.createElement("div");
    let lab = document.createElement("label");
    lab.htmlFor = id + countModel;
    lab.innerHTML = label;

    let inp = document.createElement("INPUT");
    inp.setAttribute("type", inputType);
    inp.id = id + countModel;
    inp.className = "config-max-width margin-vertical";
    inp.disabled = disabled;
    // Set giá trị quota mặc định
    if (id == "id-quota-") {
        inp.value = itemModelQuota;
        inp.className = classOfModelQuota;
    }
    // Set class cho input tên
    if (id == "id-name-") {
        inp.className = classOfModelName;
    }
    div.appendChild(lab);
    div.appendChild(inp);
    container.appendChild(div);
}

function AddLabelSelectOfStatus(container, label, id) {
    // Thêm tên
    const div = document.createElement("div");
    let lab = document.createElement("label");
    lab.htmlFor = id + countModel;
    lab.innerHTML = label;

    let selectList  = document.createElement("SELECT");
    selectList.id = id + countModel;
    selectList.className = "config-max-width margin-vertical class-model-status";

    var option = document.createElement("option");
    option.value = 0;
    option.text = "Đang kinh doanh";
    option.selected = "selected";
    selectList.appendChild(option);

    var option1 = document.createElement("option");
    option1.value = 1;
    option1.text = "Ngừng kinh doanh";
    selectList.appendChild(option1);

    var option2 = document.createElement("option");
    option2.value = 2;
    option2.text = "Hết hàng";
    selectList.appendChild(option2);

    div.appendChild(lab);
    div.appendChild(selectList);
    container.appendChild(div);
}

function AddModelToScreen() {
    countModel = countModel + 1;
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

    AddDistanceRows(modelContainer);

    // Thêm ảnh
    const imgDiv = document.createElement("div");
    const img = document.createElement("img");
    //img.src = "https://bit.ly/3jFwe3d";//URL.createObjectURL(this.files[i]);
    img.alt = "Chọn ảnh";
    img.src = "";
    img.file = null;
    img.fileName = "";
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

    //// Thêm giá bán, giá bán lấy từ các chương trình khyến mại, giảm giá
    //AddLabelInput(modelContainer, "Giá bán:", "id-price-", "number", true);
    //AddDistanceRows(modelContainer);

    //// Thêm giá bìa, thuộc tính này hiển thị chứ không cần nhập
    //AddLabelInput(modelContainer, "Giá bìa:", "id-book-cover-price-", "number", true);
    //AddDistanceRows(modelContainer);

    // Status
    AddLabelSelectOfStatus(modelContainer, "Trạng thái:", "id-status-");
    AddDistanceRows(modelContainer);

    // Quota
    AddLabelInput(modelContainer, "Quota:", "id-quota-", "number", false);
    AddDistanceRows(modelContainer);

    //// Số lượng sản phẩm trong kho, thuộc tính này hiển thị chứ không cần nhập
    //AddLabelInput(modelContainer, "Số lượng có thể bán:", "id-quantity-", "number", true);
    //AddDistanceRows(modelContainer);

    // Thêm nút xóa bên cạnh
    AddDeleteButtonForModel(modelContainer);

    // Thêm nút mapping
    AddMappingButtonForModel(modelContainer);

    AddDistanceRows(modelContainer);
    AddDistanceRows(modelList);
    //EasyViewListModel();
}

// Chọn 1 ảnh làm thumbnail từ local
function SetThumbnail() {
    if (this.files.length){
        if (DEBUG) {
            console.log("files.length: " + this.files.length);
            console.log("image selected: " + this.files[0].name);
        }

        let img = this.previousSibling;

        for (let i = 0; i < this.files.length; i++) {
            img.src = URL.createObjectURL(this.files[i]);
            img.file = this.files[i];
            img.fileName = this.files[i].name;
            img.onload = () => {
                URL.revokeObjectURL(img.src);
            }

            //const info = document.createElement("span");
            //info.innerHTML = `${this.files[i].name}: ${this.files[i].size} bytes`;
            //li.appendChild(info);
        }
    }
}

function CheckValidModelName() {
    const ls = document.getElementsByClassName(classOfModelName);
    let length = ls.length;
    if (DEBUG) {
        console.log("length of list model name: " + length);
        for (let i = 0; i < length; i++) {
            console.log(ls[i].value);
        }
    }
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
    if (DEBUG) {
        console.log("length of list model image: " + length);
    }

    let isValid = true;
    for (let i = 0; i < length; i++) {
        if (ls[i].src == null) {
            isValid = false;
            break;
        }
    }
    return isValid;
}

function GetListModelId() {
    const ls = modelList.children;
    let length = ls.length;
    if (DEBUG) {
        console.log("length of list model: " + length);
    }
    let listModelId = [];
    for (let i = 0; i < length; i++) {
        listModelId.push(ls[i].modelId);
    }
    if (DEBUG) {
        console.log(listModelId);
    }
    return JSON.stringify(listModelId);
}

// Thay đổi màu nền giúp dễ nhìn các model
function EasyViewListModel() {
    const ls = document.getElementsByClassName("model-container");
    if (ls) {
        if (DEBUG) {
            console.log("ls: " + ls.length);
        }
    }
    //let length = ls.length;
    //if (DEBUG) {
    //    for (let i = 0; i < length; i++) {
    //        console.log(ls[i].value);
    //    }
    //}
    for (let i = 0; i < ls.length; i++) {
        if ((i % 2) == 0) {
            ls[i].style.backgroundColor = "#cccccc";
        }
    }
}

// Thêm đối số cho item
// string name, int status, int quota, string detail
function AddItemParameters(searchParams){
    let name = document.getElementById("item-name-id").value;
    searchParams.append("name", name);

    let status = document.getElementById("item-status-id").value;
    searchParams.append("status", status);

    let quota = document.getElementById("item-quota-id").value;
    searchParams.append("quota", quota);

    let detail = document.getElementById("detail-id").value;
    searchParams.append("detail", detail);
}

// Thông tin gồm ảnh, tên, quota.
// Tạo mới modelId = -1
function ModelUpload(url, model, modelId, fileElement, file, modelName, quota, exist,
    listModelId, itemId, status, quantity, imageExtension) {
    if (DEBUG) {
        console.log(" Start upload image of modelId: " + modelId);
        console.log(" this of ModelUpload: " + this.nodeName);
    }
    isFinishUploadImageModel++;
    //const reader = new FileReader();
    let parrent = fileElement.parentElement;
    parrent.ctrl = CreateThrobber(fileElement);
    const xhr = new XMLHttpRequest();
    parrent.xhr = xhr;

    const self = parrent;
    parrent.xhr.upload.addEventListener("progress", (e) => {
        if (e.lengthComputable) {
            const percentage = Math.round((e.loaded * 100) / e.total);
            self.ctrl.update(percentage);
        }
    }, false);

    xhr.upload.addEventListener("load", (e) => {
        if (DEBUG) {
            console.log("Upload image model done.");
        }
        self.ctrl.update(100);

        isFinishUploadImageModel--;
    }, false);
    xhr.open("POST", url);
    xhr.setRequestHeader("Content-Type", "multipart/form-data");
    xhr.setRequestHeader("modelId", modelId);
    xhr.setRequestHeader("modelName", modelName);
    xhr.setRequestHeader("exist", exist);
    xhr.setRequestHeader("quota", quota);
    xhr.setRequestHeader("listModelId", listModelId); // Chứa list model id cũ cho trường hợp update
    xhr.setRequestHeader("itemId", itemId);
    xhr.setRequestHeader("status", status); 
    xhr.setRequestHeader("quantity", quantity);
    xhr.setRequestHeader("imageExtension", imageExtension);
    if (DEBUG) {
        console.log("modelId: " + modelId);
        console.log("modelName: " + modelName);
        console.log("exist: " + exist);
        console.log("quota: " + quota);
        console.log("listModelId: " + listModelId);
        console.log("itemId: " + itemId);
        console.log("status: " + status);
        console.log("quantity: " + quantity);
        console.log("imageExtension: " + imageExtension);
    }
    //xhr.send(file);
    let response = RequestHttpPostUpFilePromise(xhr, url, file);
    response.then(function (resolve) {
        const obj = JSON.parse(resolve);
        model.modelId = obj.myAnything;
        if (DEBUG) {
            console.log("resolve: " + resolve);
            console.log("new modelId: " + model.modelId);
        }
    }, null);
}

//Lưu item, model vào db
async function AddItemModel() {
    if (isEmptyOrSpaces(document.getElementById("item-name-id").value)) {
        alert("Tên sản phẩm không hợp lệ.");
        document.getElementById("item-name-id").focus();
        return;
    }

    if (!CheckValidModelName()) {
        alert("Tên phân loại không hợp lệ.");
        return;
    }
    
    //if (!CheckModelHasImage()) {
    //    alert("Phân loại không có ảnh đại diện.");
    //    return;
    //}

    const searchParams = new URLSearchParams();
    AddItemParameters(searchParams);

    let urlAdd = "/ItemModel/AddItem";
    let urlUpItem = "/ItemModel/UploadFileItem";
    let urlDeleItem = "";
    let urlUpModel = "/ItemModel/UploadFileModel";
    let itemId = 0;
    try {
        // Cập nhật vào db
        let responseDB = await RequestHttpGetPromise(searchParams, urlAdd);
        if (DEBUG) {
            console.log("responseDB.then: " + responseDB.responseText);
        }
        const obj = JSON.parse(responseDB.responseText);
        if (obj == null || obj.myAnything == -1) {
            alert("Tạo sản phẩm (item) lỗi.");
            return;
        }
        itemId = obj.myAnything;

        // Upload ảnh/video sản phẩm lên server
        let respinseSendFile = await SendFilesPromise(urlUpItem, urlDeleItem, itemId);

        // Upload thông tin,ảnh model lên server
        isFinishUploadImageModel = 0;
        let length = modelList.children.length;
        for (let i = 0; i < length; i++) {
            let model = modelList.children[i];
            let img = model.getElementsByClassName(classOfModelImage)[0];
            let modelName = model.getElementsByClassName(classOfModelName)[0].value;
            let modelQuota = model.getElementsByClassName(classOfModelQuota)[0].value;
            let exist;
            if (img.file == null) {
                exist = "true";
            }
            else {
                exist = "false";
            }

            let listModelId = GetListModelId();
            let status = model.getElementsByClassName(classOfModelStatus)[0].value;
            let imageExtension = GetExtensionOfFileName(img.fileName);
            ModelUpload(urlUpModel, model, model.modelId, img, img.file, modelName,
                modelQuota, exist, listModelId, itemId, status, 0, imageExtension);
        }
        
    }
    catch (error) {
        if (DEBUG) {
            console.log(error);
        }
        alert("Tạo sản phẩm lỗi.");
        return;
    }

    // Đợi upload xong ảnh/video của item
    while (true) {
        await Sleep(1000);
        if (DEBUG) {
            console.log("isFinishUploadImage = " + isFinishUploadImage);
            console.log("isFinishUploadVideo = " + isFinishUploadVideo);
        }
        if (isFinishUploadImage == 0 && isFinishUploadVideo == 0) {
            break;
        }
    }

    // Đợi upload xong ảnh của model
    while (true) {
        await Sleep(1000);
        if (DEBUG) {
            console.log("isFinishUploadImageModel = " + isFinishUploadImageModel);
        }
        if (isFinishUploadImageModel == 0) {
            alert("Tạo sản phẩm thành công.");
            break;
        }
    }

    // Refresh page
    window.scrollTo(0, 0);
    await Sleep(1000)
    //window.location.reload();
}
