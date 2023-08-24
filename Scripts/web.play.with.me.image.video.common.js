// Xử lý image/video khi upload nhiều image/video cho 1 sản phẩm trong kho hoặc Item

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

// Hiển thị list ảnh đã có
function InitializeImageList(listImage) {
    // Làm trống
    imagefileList.innerHTML = "";

    if (DEBUG) {
        console.log(listImage);
    }
    if (listImage == null)
        return;

    let iLength = listImage.length;
    if (iLength == null || iLength == 0)
        return;

    // Tạo list ảnh
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
        img.height = thumbnailHeight;
        img.width = thumbnailWidth;

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

        if (list == null) {
            imagefileList.innerHTML = "";
            list = document.createElement("ul");
            list.id = "ulImageList";
            imagefileList.appendChild(list);
        }

        if (DEBUG) {
            console.log(list);
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
            img.height = thumbnailHeight;
            img.width = thumbnailWidth;
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
    // Làm trống
    videofileList.innerHTML = "";

    if (DEBUG) {
        console.log(listVideo);
    }
    if (listVideo == null)
        return;

    let iLength = listVideo.length;
    if (iLength == null || iLength == 0)
        return;

    // Tạo list video
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

        if (list == null) {
        videofileList.innerHTML = "";
        list = document.createElement("ul");
        list.id = "ulVideoList";
            videofileList.appendChild(list);
        }

        if (DEBUG) {
            console.log(list);
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

const isImage = "isImage";
const isVideo = "isVideo";

function DeleteAllFileWithType(url, productId, fileType) {
    if (DEBUG) {
        console.log("start DeleteAllFileWithType");
    }
    const searchParams = new URLSearchParams();
    searchParams.append("id", productId);
    searchParams.append("fileType", fileType);
    let OnloadFuntion = function () {
        if (this.readyState == 4 && this.status == 200) {
            //GetJsonResponse(this.responseText);
        }
    }
    RequestHttpPost(OnloadFuntion, searchParams, url);
}

//
function SendFilesPromise(urlCreate, urlDelete, productId) {
    return new Promise(function (resolve, reject) {
        if (DEBUG) {
            console.log("Start up load image and video. productId = " + productId);
        }

        const imgs = document.getElementsByClassName("objImage");
        isFinishUploadImage = imgs.length;
        if (DEBUG) {
            console.log("imgs.length: " + imgs.length);
        }
        const videos = document.getElementsByClassName("objVideo");
        isFinishUploadVideo = videos.length;
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
                isFinishUploadImage = isFinishUploadImage - 1;
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
            new FileUpload(urlCreate, productId, imgs[i], isImage, imgs[i].file, imgs[i].fileName, i, exist, finish);
        }
        if (!isEmptyOrSpaces(urlDelete)) {// Trường hợp cập nhật mới gửi lệnh xóa ảnh cũ
            // Không có ảnh nào gửi lệnh xóa ảnh trên server
            if (imgs.length == 0) {
                DeleteAllFileWithType(urlDelete, productId, isImage);
            }
        }
        // Upload video lên server
        for (let i = 0; i < videos.length; i++) {
            if (DEBUG) {
                console.log("videos file name: " + videos[i].fileName);
            }
            let exist;
            if (videos[i].file == null) {
                exist = "true";
                isFinishUploadVideo = isFinishUploadVideo - 1;
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

            new FileUpload(urlCreate, productId, videos[i], isVideo, videos[i].file, videos[i].fileName, i, exist, finish);
        }
        if (!isEmptyOrSpaces(urlDelete)) {// Trường hợp cập nhật mới gửi lệnh xóa video cũ
            // Không có ảnh nào gửi lệnh xóa ảnh trên server
            if (videos.length == 0) {
                DeleteAllFileWithType(urlDelete, productId, isVideo);
            }
        }
        resolve("done");
        if (DEBUG) {
            console.log("end SendFilesPromise");
        }
    });
}

let isFinishUploadImage = 0;
let isFinishUploadVideo = 0;

// exist: true nếu file này đã tồn tại trên server, ngược lại false. Check tồn tại dựa vào tên
function FileUpload(url, productId, fileElement, fileType, file, fileName, fileOrder, exist, finish) {
    if (DEBUG) {
        console.log("productId: " + productId);
        console.log("Order: " + fileOrder);
    }
    //const reader = new FileReader();
    this.ctrl = CreateThrobber(fileElement);
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
        if (DEBUG) {
            console.log("Upload done.");
        }
        self.ctrl.update(100);
        //const canvas = self.ctrl.ctx.canvas;
        //canvas.parentNode.removeChild(canvas);
        if (fileType === isImage) {
            isFinishUploadImage = isFinishUploadImage - 1;
            if (DEBUG) {
                console.log("isFinishUploadImage = " + isFinishUploadImage);
            }
        } else if (fileType === isVideo) {
            isFinishUploadVideo = isFinishUploadVideo - 1;
            if (DEBUG) {
                console.log("isFinishUploadVideo = " + isFinishUploadVideo);
            }
        }
    }, false);
    xhr.open("POST", url);
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

function CreateThrobber(fileElement) {
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
    }
    throbber.update(0);
    return throbber;
}