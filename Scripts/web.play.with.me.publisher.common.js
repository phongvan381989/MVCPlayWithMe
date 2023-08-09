// lấy danh sách nhà phát hành
function loadPublishers() {
    const xhttp = new XMLHttpRequest();
    xhttp.onload = function () {
        if (this.readyState == 4 && this.status == 200) {
            document.getElementById("publisher-table").innerHTML = this.responseText;
        }
    }
    let query = "/Publisher/LoadPublisher";
    xhttp.open("GET", query);
    xhttp.send();
}

function PublisherChange() {
    let publisherName = document.getElementById("publisher-id").value;
    let detail = GetDataFromDatalist("list-Publisher", "data-detail", publisherName);
    if (DEBUG) {
        console.log(detail);
    }

    if (detail == null) {
        document.getElementById("detail").value = "";
        return;
    }
    document.getElementById("detail").value = detail;
}