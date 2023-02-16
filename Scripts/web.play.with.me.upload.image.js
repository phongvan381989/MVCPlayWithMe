
///productIdForUpdate = -1: tạo mới sản phẩm, ngược lại là id sản phẩm cần update
function AddNewPro(productIdForUpdate) {
    let barcode = document.getElementById("barcode").value;
    if (isEmptyOrSpaces(barcode)) {
        document.getElementById("result-insert").innerHTML = "Mã vạch trống.";
        if (DEBUG) {
            console.log("Mã vạch trống.");
        }
        return;
    }

    let productName = document.getElementById("product-name").value;
    if (isEmptyOrSpaces(productName)) {
        document.getElementById("result-insert").innerHTML = "Tên sản phẩm trống.";
        if (DEBUG) {
            console.log("Tên sản phẩm trống.");
        }
        return;
    }

    let productID = 0;
    const xhttp = new XMLHttpRequest();
    xhttp.onload = function () {
        if (this.readyState == 4 && this.status == 200) {
            if (GetJsonResponse(this.responseText)) {
                // Bắt đầu upload ảnh/video sản phẩm lên server
                const obj = JSON.parse(responseText);
                productID = obj.anything;
                SendFiles(productID);
            }
        }
    }

    const searchParams = new URLSearchParams();
    searchParams.append("barcode", barcode);

    searchParams.append("productName", productName);

    searchParams.append("comboName", document.getElementById("combo-name").value);

    searchParams.append("category", document.getElementById("category").value);

    let bookCoverPrice = document.getElementById("book-cover-price").value;
    if (isEmptyOrSpaces(bookCoverPrice)) {
        bookCoverPrice = 0;
    }
    searchParams.append("bookCoverPrice", bookCoverPrice);

    searchParams.append("author", document.getElementById("author").value);
    searchParams.append("translator", document.getElementById("translator").value);

    let publisherId = document.getElementById("publisher-id").value;
    if (isEmptyOrSpaces(publisherId)) {
        publisherId = 0;
    }
    searchParams.append("publisherId", publisherId);

    searchParams.append("publishingCompany", document.getElementById("publishing-company").value);

    let publishingTime = document.getElementById("publishing-time").value;
    if (isEmptyOrSpaces(publishingTime)) {
        publishingTime = "2018-08-05"; // Giá trị mặc định là ngày sinh Sâu béo
    }
    searchParams.append("publishingTime", publishingTime);

    let productLong = document.getElementById("product-long").value;
    if (isEmptyOrSpaces(productLong)) {
        productLong = 0;
    }
    searchParams.append("productLong", productLong);

    let productWide = document.getElementById("product-wide").value;
    if (isEmptyOrSpaces(productWide)) {
        productWide = 0;
    }
    searchParams.append("productWide", productWide);

    let productHigh = document.getElementById("product-high").value;
    if (isEmptyOrSpaces(productHigh)) {
        productHigh = 0;
    }
    searchParams.append("productHigh", productHigh);


    let productWeight = document.getElementById("product-weight").value;
    if (isEmptyOrSpaces(productWeight)) {
        productWeight = 0;
    }
    searchParams.append("productWeight", productWeight);

    searchParams.append("positionInWarehouse", document.getElementById("position-in-warehouse").value);
    searchParams.append("detail", document.getElementById("detail").value);
    searchParams.append("productIdForUpdate", productIdForUpdate);
    let query = "/Administrator/AddNewPro?" + searchParams.toString();
    if (DEBUG) {
        console.log(query);
    }
    xhttp.open("GET", query);
    xhttp.send();
}

// Xử lý image
const fileSelect = document.getElementById("fileSelect"),
    fileUnSelect = document.getElementById("fileUnSelect"),
    fileElem = document.getElementById("fileElem"),
    fileList = document.getElementById("fileList");

fileSelect.addEventListener("click", (e) => {
    if (fileElem) {
        fileElem.click();
    }
    e.preventDefault(); // prevent navigation to "#"
}, false);

fileUnSelect.addEventListener("click", (e) => {
    fileList.innerHTML = "<p>Chưa ảnh nào được chọn!</p>";
    document.getElementById("fileElem").value = "";
    e.preventDefault(); // prevent navigation to "#"
}, false);

fileElem.addEventListener("change", HandleFiles, false);

// Xử lý video
const videofileSelect = document.getElementById("videofileSelect"),
    videofileUnSelect = document.getElementById("videofileUnSelect"),
    videofileElem = document.getElementById("videofileElem"),
    videofileList = document.getElementById("videofileList");

videofileSelect.addEventListener("click", (e) => {
    if (videofileElem) {
        videofileElem.click();
    }
    e.preventDefault(); // prevent navigation to "#"
}, false);

videofileUnSelect.addEventListener("click", (e) => {
    videofileList.innerHTML = "<p>Chưa video nào được chọn!</p>";
    document.getElementById("videofileElem").value = "";
    e.preventDefault(); // prevent navigation to "#"
}, false);

videofileElem.addEventListener("change", videoHandleFiles, false);

// Xóa ảnh đã chọn khi click
function RemoveImage(el) {
    el.remove();
};

// Xóa vide0 đã chọn khi click
function RemoveVideo(el) {
    el.remove();
    document.getElementById("videofileElem").value = "";
};

function HandleFiles() {
    if (!this.files.length) {
        let list = document.getElementById("ulImageList");
        if (list == null) {
            fileList.innerHTML = "<p>Chưa ảnh nào được chọn!</p>";
        }
    } else {
        let list = document.getElementById("ulImageList");
        if (DEBUG) {
            console.log(list);
        }

        if (list == null) {
            fileList.innerHTML = "";
            list = document.createElement("ul");
            list.id = "ulImageList";
            fileList.appendChild(list);
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
            const info = document.createElement("span");
            info.innerHTML = `${this.files[i].name}: ${this.files[i].size} bytes`;
            li.appendChild(info);
        }
    }
}

function videoHandleFiles() {
    if (DEBUG) {
        console.log("TH0");
    }
    if (!this.files.length) {
        if (DEBUG) {
            console.log("TH1");
        }
        let list = document.getElementById("ulVideoList");
        if (list == null) {
            videofileList.innerHTML = "<p>Chưa video nào được chọn!</p>";
        }
    }
    else {
        if (DEBUG) {
            console.log("TH2");
        }
        let list = document.getElementById("ulVideoList");
        if (DEBUG) {
            console.log(list);
        }

        if (list != null)
            list.remove();

        videofileList.innerHTML = "";
        list = document.createElement("ul");
        list.id = "ulVideoList";
        videofileList.appendChild(list);

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
            const info = document.createElement("span");
            info.innerHTML = `${this.files[i].name}: ${this.files[i].size} bytes`;
            li.appendChild(info);
        }
    }
}

function SendFiles(productId) {
    if (DEBUG) {
        console.log("Start up load image and video.");
    }
    const imgs = document.getElementsByClassName("objImage");
    if (DEBUG) {
        console.log("imgs.length: " + imgs.length);
    }
    const videos = document.getElementsByClassName("objVideo");
    if (DEBUG) {
        console.log("videos.length: " + videos.length);
    }
    if (imgs.length === 0 && videos.length === 0)
        return;


    // Gửi yêu cầu xóa hết ảnh/video cũ
    const xhttp = new XMLHttpRequest();
    xhttp.onload = function () {
        if (this.readyState == 4 && this.status == 200) {
            const obj = JSON.parse(responseText);
            if (DEBUG) {
                console.log(obj);
            }
            if (obj.State == 0) {
                try {
                    // Upload ảnh lên server
                    for (let i = 0; i < imgs.length; i++) {
                        if (DEBUG) {
                            console.log("image file name: " + imgs[i].fileName);
                        }
                        new FileUpload(productId, imgs[i], imgs[i].file, imgs[i].fileName, i);
                    }

                    // Upload video lên server
                    for (let i = 0; i < videos.length; i++) {
                        if (DEBUG) {
                            console.log("videos file name: " + videos[i].fileName);
                        }
                        new FileUpload(productId, videos[i], videos[i].file, videos[i].fileName, i);
                    }
                }
                catch (err) {
                    document.getElementById("result-insert").innerHTML = "Error: " + err + ".";
                    return;
                }
                document.getElementById("result-insert").innerHTML = "Update success."
            }
            else
                document.getElementById("result-insert").innerHTML = obj.Message;
        }
    }
    const searchParams = new URLSearchParams();
    searchParams.append("productId", productId);
    let query = "/Administrator/DeleteOldImage?" + searchParams.toString();
    if (DEBUG) {
        console.log(query);
    }
    xhttp.open("GET", query);
    xhttp.send();

}

function FileUpload(productId, img, file, fileName, fileOrder) {
    if (DEBUG) {
        console.log("productId: " + productId);
        console.log("Order: " + fileOrder);
    }
    //const reader = new FileReader();
    this.ctrl = CreateThrobber(img);
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
    xhr.open("POST", "/Administrator/UploadImage");
    //xhr.overrideMimeType('text/plain; charset=x-user-defined-binary');
    xhr.setRequestHeader("Content-Type", "multipart/form-data");
    //let newFileName = `${fileOrder}.${GetExtensionOfFileName(fileName)}`;
    xhr.setRequestHeader("imgFileName", `${fileOrder}.${GetExtensionOfFileName(fileName)}`);
    xhr.setRequestHeader("productId", productId);
    xhr.send(file);
    //reader.onload = (evt) => {
    //    xhr.send(evt.target.result);
    //};
    //reader.readAsBinaryString(file);
}

function CreateThrobber(img) {
    const throbberWidth = 64;
    const throbberHeight = 6;
    const throbber = document.createElement('canvas');
    throbber.classList.add('upload-progress');
    throbber.setAttribute('width', throbberWidth);
    throbber.setAttribute('height', throbberHeight);
    img.parentNode.appendChild(throbber);
    throbber.ctx = throbber.getContext('2d');
    throbber.ctx.fillStyle = 'orange';
    throbber.update = (percent) => {
        throbber.ctx.fillRect(0, 0, throbberWidth * percent / 100, throbberHeight);
        if (percent === 100) {
            throbber.ctx.fillStyle = 'green';
        }
    }
    throbber.update(0);
    return throbber;
}