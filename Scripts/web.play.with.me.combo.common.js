// lấy danh sách
function LoadCombo() {
    const xhttp = new XMLHttpRequest();
    xhttp.onload = function () {
        if (this.readyState == 4 && this.status == 200) {
            document.getElementById("combo-table").innerHTML = this.responseText;
        }
    }
    let query = "/Combo/LoadCombo";
    xhttp.open("GET", query);
    xhttp.send();
}