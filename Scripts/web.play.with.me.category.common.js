// lấy danh sách
function LoadCategory() {
    const xhttp = new XMLHttpRequest();
    xhttp.onload = function () {
        if (this.readyState == 4 && this.status == 200) {
            document.getElementById("category-table").innerHTML = this.responseText;
        }
    }
    let query = "/Category/LoadCategory";
    xhttp.open("GET", query);
    xhttp.send();
}