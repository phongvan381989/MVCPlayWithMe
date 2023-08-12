function AddUpdateWithCommonParameters(searchParams) {

    let publisherName = document.getElementById("publisher-id").value;
    if (isEmptyOrSpaces(publisherName)) {
        ShowResult("Tên nhà phát hành trống.");
        return false;
    }
    let comboName = document.getElementById("combo-id").value;
    searchParams.append("comboName", comboName);

    let categoryName = document.getElementById("category-id").value;
    searchParams.append("categoryName", categoryName);

    let bookCoverPrice = GetValueOfNumberInputById("book-cover-price", 0);
    searchParams.append("bookCoverPrice", bookCoverPrice);

    searchParams.append("author", document.getElementById("author-id").value);

    searchParams.append("translator", document.getElementById("translator-id").value);

    searchParams.append("publisherName", publisherName)

    searchParams.append("publishingCompany", document.getElementById("publishing-company-id").value);

    let publishingTime = GetValueOfNumberInputById("publishing-time", -1);
    searchParams.append("publishingTime", publishingTime);

    let productLong = GetValueOfNumberInputById("product-long", 0);
    searchParams.append("productLong", productLong);

    let productWide = GetValueOfNumberInputById("product-wide", 0);
    searchParams.append("productWide", productWide);

    let productHigh = GetValueOfNumberInputById("product-high", 0);
    searchParams.append("productHigh", productHigh);

    let productWeight = GetValueOfNumberInputById("product-weight", 0);
    searchParams.append("productWeight", productWeight);

    searchParams.append("positionInWarehouse", document.getElementById("position-in-warehouse").value);

    let hardCover = document.getElementById("hard-cover").value;
    searchParams.append("hardCover", hardCover);

    let minAge = GetValueOfNumberInputById("min-age", -1);
    searchParams.append("minAge", minAge);

    let maxAge = GetValueOfNumberInputById("max-age", -1);
    searchParams.append("maxAge", maxAge);

    searchParams.append("republish", GetValueOfNumberInputById("republish", -1));

    let productStatus = document.getElementById("product-status").value;
    searchParams.append("status", productStatus);

    return true;
}

function AddUpdateParameters(searchParams) {
    let productName = document.getElementById("product-name-id").value;
    if (isEmptyOrSpaces(productName)) {
        ShowResult("Tên sản phẩm trống.");
        return false;
    }

    let code = document.getElementById("code").value;
    if (!isEmptyOrSpaces(code)){
        if (code.length != 13 || code.substring(0, 2) != "89") {
            ShowResult("Mã sản phẩm không chính xác.");
            return false;
            }
    }

    let barcode = document.getElementById("barcode").value;
    if (!isEmptyOrSpaces(barcode)) {
        if (barcode.length != 13 || barcode.substring(0, 6) != "978604") {
            ShowResult("Mã ISBN không chính xác.");
            return false;
        }
    }

    if (AddUpdateWithCommonParameters(searchParams) === false) {
        return false;
    }

    searchParams.append("code", code);

    searchParams.append("barcode", barcode);

    searchParams.append("name", productName);

    let parentName = document.getElementById("parent-id").value;
    searchParams.append("parentName", parentName);

    searchParams.append("detail", document.getElementById("detail").value);
    return true;
}

function AddNewPro() {
    const searchParams = new URLSearchParams();
    if (AddUpdateParameters(searchParams) === false)
        return;

    let query = "/Product/AddNewPro";
    let productID = 0;
    let OnloadFuntion = function () {
        if (this.readyState == 4 && this.status == 200) {
            if (GetJsonResponse(this.responseText)) {
                // Bắt đầu upload ảnh/video sản phẩm lên server
                const obj = JSON.parse(this.responseText);
                productID = obj.myAnything;
                SendFiles(productID);
            }
        }
    }
    RequestHttpGet(OnloadFuntion, searchParams, query);
}

// Set thông tin chung của những sản phẩm thuộc combo về mặc định trừ trường combo
function SetProductCommonInfoWithComboToDefault() {
    document.getElementById("category-id").value = "";
    document.getElementById("book-cover-price").value = "";
    document.getElementById("author-id").value = "";
    document.getElementById("translator-id").value = "";
    document.getElementById("publisher-id").value = "";
    document.getElementById("publishing-company-id").value = "";
    document.getElementById("publishing-time").value = "";
    document.getElementById("product-long").value = "";
    document.getElementById("product-wide").value = "";
    document.getElementById("product-high").value = "";
    document.getElementById("product-weight").value = "";
    document.getElementById("position-in-warehouse").value = "";
    document.getElementById("hard-cover").value = 0;
    document.getElementById("min-age").value = "";
    document.getElementById("max-age").value = "";
    document.getElementById("republish").value = "";
    document.getElementById("product-status").value = 0;
}

// Set thông tin sản phẩm về mặc định trừ tên
function SetProductInfomationToDefault() {
    document.getElementById("code").value = "";
    document.getElementById("barcode").value = "";
    document.getElementById("combo-id").value = "";

    SetProductCommonInfoWithComboToDefault();

    document.getElementById("parent-id").value = "";
    document.getElementById("detail").value = "";
}

// Set thông tin chung của những sản phẩm thuộc combo trừ combo
function SetProductCommonInfoWithCombo(product) {
    document.getElementById("category-id").value = product.categoryName;
    document.getElementById("book-cover-price").value = product.bookCoverPrice;
    document.getElementById("author-id").value = product.author;
    document.getElementById("translator-id").value = product.translator;
    document.getElementById("publisher-id").value = product.publisherName;
    document.getElementById("publishing-company-id").value = product.publishingCompany;
    document.getElementById("publishing-time").value = product.publishingTime;
    document.getElementById("product-long").value = product.productLong;
    document.getElementById("product-wide").value = product.productWide;
    document.getElementById("product-high").value = product.productHigh;
    document.getElementById("product-weight").value = product.productWeight;
    document.getElementById("position-in-warehouse").value = product.positionInWarehouse;
    document.getElementById("hard-cover").value = product.hardCover;
    document.getElementById("min-age").value = product.minAge;
    document.getElementById("max-age").value = product.maxAge;
    document.getElementById("republish").value = product.republish;
    document.getElementById("product-status").value = product.status;
}

// Set thông tin sản phẩm trừ tên
function SetProductInfomation(product) {
    document.getElementById("code").value = product.code;
    document.getElementById("barcode").value = product.barcode;
    document.getElementById("combo-id").value = product.comboName;

    SetProductCommonInfoWithCombo(product);

    document.getElementById("parent-id").value = product.parentName;
    document.getElementById("detail").value = product.detail;
}

// Thay đổi tên sản phẩm, hiển thị tất cả thông tin tương ứng
function ProductNameChange(str) {
    if (DEBUG) {
        console.log("ProductNameChange function");
    }
    let id = GetDataIdFromProductNameDatalist(str);
    if (id == null) {
        SetProductInfomationToDefault();
        return;
    }

    // Lấy tất cả thông tin về sản phẩm và hiển thị
    const searchParams = new URLSearchParams();
    searchParams.append("id", id);
    let query = "/Product/GetProduct";

    let OnloadFuntion = function () {
        if (this.readyState == 4 && this.status == 200) {
            const product = JSON.parse(this.responseText);
            if (DEBUG) {
                console.log(product);
            }
            if (product == null) {
                SetProductInfomationToDefault();
                return;
            }

            SetProductInfomation(product);

            const listMedia = JSON.parse(product.anything);
            const listImage = listMedia[0];
            const listVideo = listMedia[1];
            InitializeImageList(listImage);
            InitializeVideoList(listVideo);
        }
    }

    RequestHttpPost(OnloadFuntion, searchParams, query);
}

// Thay đổi combo, hiển thị tất cả thông tin chung các sản phẩm cùng combo
// Thông tin chung này lấy từ sản phẩm đầu tiên thuộc combo trong db
function ComboChange(str) {
    if (DEBUG) {
        console.log("ComboChange function");
    }
    let id = GetDataIdFromComboDatalist(str);
    if (id == null) {
        SetProductCommonInfoWithComboToDefault();
        return;
    }

    // Thông tin chung này lấy từ sản phẩm đầu tiên thuộc combo trong db
    const searchParams = new URLSearchParams();
    searchParams.append("id", id);
    let query = "/Product/GetProductCommonInfoWithComboFromFirst";

    let OnloadFuntion = function () {
        if (this.readyState == 4 && this.status == 200) {
            const product = JSON.parse(this.responseText);
            if (DEBUG) {
                console.log(product);
            }
            if (product == null) {
                SetProductCommonInfoWithComboToDefault();
                return;
            }
            SetProductCommonInfoWithCombo(product);
        }
    }

    RequestHttpPost(OnloadFuntion, searchParams, query);
}

function UpdateProduct() {
    let productID = GetDataIdFromProductNameDatalist(document.getElementById("product-name-id").value);
    if (productID == null) {
        ShowResult("Tên sản phẩm không chính xác.")
        return;
    }
    const searchParams = new URLSearchParams();
    AddUpdateParameters(searchParams);

    let query = "/Product/UpdateProduct";
    let OnloadFuntion = function () {
        if (this.readyState == 4 && this.status == 200) {
            if (GetJsonResponse(this.responseText)) {
                // Bắt đầu upload ảnh/video sản phẩm lên server
                const obj = JSON.parse(this.responseText);
                SendFiles(productID);
                if (CheckStatusResponseAndShowPrompt(this.responseText,true, "Cập nhật thành công", "Cập nhật thất bại")) {
                    ReloadAndScrollToTop();
                }
            }
        }
    }
    RequestHttpGet(OnloadFuntion, searchParams, query);
}

function DeleteProduct(){
    let text = "Bạn chắc chắn muốn XÓA?";
    if (confirm(text) == false)
        return;

    let str = document.getElementById("product-name-id").value;
    let id = GetDataIdFromProductNameDatalist(str);
    if (id == null)
        return;

    const searchParams = new URLSearchParams();
    searchParams.append("id", id);
    let query = "/Product/DeleteProduct";
    let OnloadFuntion = function () {
        if (this.readyState == 4 && this.status == 200) {
            GetJsonResponse(this.responseText);
            if (CheckStatusResponse(this.responseText)) {
                window.location.reload();
            }
        }
    }
    RequestHttpPost(OnloadFuntion, searchParams, query);
};

function UpdateCommonInfoWithCombo() {
    const searchParams = new URLSearchParams();
    if (AddUpdateWithCommonParameters(searchParams) === false) {
        return false;
    }

    let query = "/Product/UpdateCommonInfoWithCombo";
    let OnloadFuntion = function () {
        if (this.readyState == 4 && this.status == 200) {
            GetJsonResponse(this.responseText);
            if (CheckStatusResponseAndShowPrompt(this.responseText, true, "Cập nhật thành công", "Cập nhật thất bại")) {
                ReloadAndScrollToTop();
            }
        }
    }
    RequestHttpPost(OnloadFuntion, searchParams, query);
}

// Xử lý image
const imagefileSelect = document.getElementById("imagefileSelect"),
    //imagefileUnSelect = document.getElementById("imagefileUnSelect"),
    imagefileElem = document.getElementById("imagefileElem"),
    imagefileList = document.getElementById("imagefileList");

if (imagefileSelect != null) {
    imagefileSelect.addEventListener("click", (e) => {
        if (imagefileElem) {
            imagefileElem.click();
        }
        e.preventDefault(); // prevent navigation to "#"
    }, false);
}

//imagefileUnSelect.addEventListener("click", (e) => {
//    imagefileList.innerHTML = "<p>Chưa ảnh nào được chọn!</p>";
//    document.getElementById("imagefileElem").value = "";
//    e.preventDefault(); // prevent navigation to "#"
//}, false);

if (imagefileElem != null) {
    imagefileElem.addEventListener("change", ImageHandleFiles, false);
}

// Xử lý video
const videofileSelect = document.getElementById("videofileSelect"),
    //videofileUnSelect = document.getElementById("videofileUnSelect"),
    videofileElem = document.getElementById("videofileElem"),
    videofileList = document.getElementById("videofileList");

if (videofileSelect != null) {
    videofileSelect.addEventListener("click", (e) => {
        if (videofileElem) {
            videofileElem.click();
        }
        e.preventDefault(); // prevent navigation to "#"
    }, false);
}

//videofileUnSelect.addEventListener("click", (e) => {
//    videofileList.innerHTML = "<p>Chưa video nào được chọn!</p>";
//    document.getElementById("videofileElem").value = "";
//    e.preventDefault(); // prevent navigation to "#"
//}, false);
if (videofileElem != null) {
    videofileElem.addEventListener("change", VideoHandleFiles, false);
}

// Xóa ảnh đã chọn khi click
function RemoveImage(el) {
    el.remove();
};

// Xóa vide0 đã chọn khi click
function RemoveVideo(el) {
    el.remove();
    document.getElementById("videofileElem").value = "";
};

// Thêm nút xóa bên canh image/video
function AddDeleteButton(li) {
    // Thêm nút xóa ảnh bên cạnh
    var btn = document.createElement("BUTTON");
    var btnContent = document.createTextNode("Xóa");
    btn.onclick = function () {
        this.parentElement.remove();
    }
    btn.appendChild(btnContent);
    li.appendChild(btn);
}

// Hiển thị list ảnh đã có
function InitializeImageList(listImage) {
    if (DEBUG) {
        console.log(listImage);
    }
    if (listImage == null)
        return;

    let iLength = listImage.length;
    if (iLength == null || iLength == 0)
        return;

    // Tạo list ảnh
    imagefileList.innerHTML = "";
    list = document.createElement("ul");
    list.id = "ulImageList";
    imagefileList.appendChild(list);

    for (let i = 0; i < iLength; i++) {
        const li = document.createElement("li");
        list.appendChild(li);

        const img = document.createElement("img");
        img.src = listImage[i];
        img.file = null;
        img.fileName = listImage[i];
        img.className = "objImage";
        img.height = 60;

        li.appendChild(img);

        // Thêm nút xóa bên cạnh
        AddDeleteButton(li);
    }
}

function ImageHandleFiles() {
    if (!this.files.length) {
        let list = document.getElementById("ulImageList");
        if (list == null) {
            imagefileList.innerHTML = "<p>Chưa ảnh nào được chọn!</p>";
        }
    } else {
        let list = document.getElementById("ulImageList");
        if (DEBUG) {
            console.log(list);
        }

        if (list == null) {
            imagefileList.innerHTML = "";
            list = document.createElement("ul");
            list.id = "ulImageList";
            imagefileList.appendChild(list);
        }

        for (let i = 0; i < this.files.length; i++) {
            const li = document.createElement("li");
            //li.onclick = function () { RemoveImage(this) };
            list.appendChild(li);

            const img = document.createElement("img");
            img.src = URL.createObjectURL(this.files[i]);
            img.file = this.files[i];
            img.fileName = this.files[i].name;
            img.className = "objImage";
            img.height = 60;
            img.onload = () => {
                URL.revokeObjectURL(img.src);
            }
            li.appendChild(img);

            // Thêm nút xóa bên cạnh
            AddDeleteButton(li);

            const info = document.createElement("span");
            info.innerHTML = `${this.files[i].name}: ${this.files[i].size} bytes`;
            li.appendChild(info);
        }
    }
}

// Hiển thị list video đã có
function InitializeVideoList(listVideo) {
    if (DEBUG) {
        console.log(listVideo);
    }
    if (listVideo == null)
        return;

    let iLength = listVideo.length;
    if (iLength == null || iLength == 0)
        return;

    // Tạo list video
    videofileList.innerHTML = "";
    list = document.createElement("ul");
    list.id = "ulVideoList";
    videofileList.appendChild(list);

    for (let i = 0; i < iLength; i++) {
        const li = document.createElement("li");
        list.appendChild(li);

        const video = document.createElement("video");

        video.src = listVideo[i];;

        video.controls = true;
        video.file = null;
        video.fileName = listVideo[i];
        video.className = "objVideo";
        video.height = 120;
        //video.play();
        //video.onload = () => {
        //    URL.revokeObjectURL(video.src);
        //}
        li.appendChild(video);

        // Thêm nút xóa bên cạnh
        AddDeleteButton(li);
    }
}

function VideoHandleFiles() {
    if (!this.files.length) {
        let list = document.getElementById("ulVideoList");
        if (list == null) {
            videofileList.innerHTML = "<p>Chưa video nào được chọn!</p>";
        }
    }
    else {

        let list = document.getElementById("ulVideoList");
        if (DEBUG) {
            console.log(list);
        }

        if (list == null) {
        videofileList.innerHTML = "";
        list = document.createElement("ul");
        list.id = "ulVideoList";
            videofileList.appendChild(list);
        }

        for (let i = 0; i < this.files.length; i++) {
            const li = document.createElement("li");
            //li.onclick = function () { RemoveVideo(this) };
            list.appendChild(li);

            const video = document.createElement("video");
            //const source  = document.createElement("source");
            video.src = URL.createObjectURL(this.files[i]);
            //video.type = `video/${GetExtensionOfFileName(this.files[i].name)}`;
            //video.appendChild(source);
            video.controls = true;
            video.file = this.files[i];
            video.fileName = this.files[i].name;
            video.className = "objVideo";
            video.height = 120;
            //video.play();
            video.onload = () => {
                URL.revokeObjectURL(video.src);
            }
            li.appendChild(video);

            // Thêm nút xóa bên cạnh
            AddDeleteButton(li);

            const info = document.createElement("span");
            info.innerHTML = `${this.files[i].name}: ${this.files[i].size} bytes`;
            li.appendChild(info);
        }
    }
}

function DeleteAllFileWithType(productId, fileType) {
    const searchParams = new URLSearchParams();
    searchParams.append("id", productId);
    searchParams.append("fileType", fileType);
    let query = "/Product/DeleteAllFileWithType";
    let OnloadFuntion = function () {
        if (this.readyState == 4 && this.status == 200) {
            //GetJsonResponse(this.responseText);
            if (fileType === isImage) {
                isFinishUploadImage = 1;
            } else if (fileType === isVideo) {
                isFinishUploadVideo = 1;
            }
        }
    }
    RequestHttpPost(OnloadFuntion, searchParams, query);
}

const isImage = "isImage";
const isVideo = "isVideo";

// Check upload xong
async function WaitSendFiles() {
    let imax = 0;
    while (true) {
        if (DEBUG) {
            console.log("WaitSendFiles: imax = " + imax);
        }
        await Sleep(1000);
        if (isFinishUploadImage == 1 && isFinishUploadVideo == 1)
            break;
        imax = imax + 1;;
        if (imax == 60) // timeout
            break;
    }
}


function SendFiles(productId) {
    if (DEBUG) {
        console.log("Start up load image and video. productId = " + productId);
    }
    const imgs = document.getElementsByClassName("objImage");
    if (DEBUG) {
        console.log("imgs.length: " + imgs.length);
    }
    const videos = document.getElementsByClassName("objVideo");
    if (DEBUG) {
        console.log("videos.length: " + videos.length);
    }

    // Upload ảnh lên server
    for (let i = 0; i < imgs.length; i++) {
        if (DEBUG) {
            console.log("image file name: " + imgs[i].fileName);
        }
        let exist;
        if (imgs[i].file == null) {
            exist = "true";
        }
        else {
            exist = "false";
        }

        let finish;
        if (i == imgs.length - 1) {
            finish = "true";
        }
        else {
            finish = "false";
        }
        new FileUpload(productId, imgs[i], isImage, imgs[i].file, imgs[i].fileName, i, exist, finish);
    }
    // Không có ảnh nào gửi lệnh xóa ảnh trên server
    if (imgs.length == 0) {
        DeleteAllFileWithType(productId, isImage);
    }

    // Upload video lên server
    for (let i = 0; i < videos.length; i++) {
        if (DEBUG) {
            console.log("videos file name: " + videos[i].fileName);
        }
        let exist;
        if (videos[i].file == null) {
            exist = "true";
        }
        else {
            exist = "false";
        }

        let finish;
        if (i == videos.length - 1) {
            finish = "true";
        }
        else {
            finish = "false";
        }

        new FileUpload(productId, videos[i], isVideo, videos[i].file, videos[i].fileName, i, exist, finish);
    }
    // Không có ảnh nào gửi lệnh xóa ảnh trên server
    if (videos.length == 0) {
        DeleteAllFileWithType(productId, isVideo);
    }

    WaitSendFiles();
}

let isFinishUploadImage = 0;
let isFinishUploadVideo = 0;

// exist: true nếu file này đã tồn tại trên server, ngược lại false. Check tồn tại dựa vào tên
function FileUpload(productId, fileElement, fileType, file, fileName, fileOrder, exist, finish) {
    if (DEBUG) {
        console.log("productId: " + productId);
        console.log("Order: " + fileOrder);
    }
    //const reader = new FileReader();
    this.ctrl = CreateThrobber(fileElement, fileType, finish);
    const xhr = new XMLHttpRequest();
    this.xhr = xhr;

    const self = this;
    this.xhr.upload.addEventListener("progress", (e) => {
        if (e.lengthComputable) {
            const percentage = Math.round((e.loaded * 100) / e.total);
            self.ctrl.update(percentage);
        }
    }, false);

    xhr.upload.addEventListener("load", (e) => {
        self.ctrl.update(100);
        const canvas = self.ctrl.ctx.canvas;
        //canvas.parentNode.removeChild(canvas);
    }, false);
    xhr.open("POST", "/Product/UploadFile");
    //xhr.overrideMimeType('text/plain; charset=x-user-defined-binary');
    xhr.setRequestHeader("Content-Type", "multipart/form-data");
    //let newFileName = `${fileOrder}.${GetExtensionOfFileName(fileName)}`;
    xhr.setRequestHeader("fileName", `${fileOrder}.${GetExtensionOfFileName(fileName)}`);
    xhr.setRequestHeader("productId", productId);
    xhr.setRequestHeader("originalFileName", fileName);
    xhr.setRequestHeader("exist", exist);
    xhr.setRequestHeader("finish", finish);
    xhr.send(file);
    //reader.onload = (evt) => {
    //    xhr.send(evt.target.result);
    //};
    //reader.readAsBinaryString(file);
}

function CreateThrobber(fileElement, fileType, finish) {
    const throbberWidth = 64;
    const throbberHeight = 6;
    const throbber = document.createElement('canvas');
    throbber.classList.add('upload-progress');
    throbber.setAttribute('width', throbberWidth);
    throbber.setAttribute('height', throbberHeight);
    fileElement.parentNode.appendChild(throbber);
    throbber.ctx = throbber.getContext('2d');
    throbber.ctx.fillStyle = 'orange';
    throbber.update = (percent) => {
        throbber.ctx.fillRect(0, 0, throbberWidth * percent / 100, throbberHeight);
        if (percent === 100) {
            throbber.ctx.fillStyle = 'green';
            if (finish == "true") {
                if (fileType === isImage) {
                    isFinishUploadImage = 1;
                } else if (fileType === isVideo) {
                    isFinishUploadVideo = 1;
                }
            }

        }
    }
    throbber.update(0);
    return throbber;
}