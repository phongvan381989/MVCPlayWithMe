function New_AddAdministrator() {
    //let responseId = 0;
    const xhttp = new XMLHttpRequest();
    xhttp.onload = function () {
        if (this.readyState == 4 && this.status == 200) {
        }
    }

    const searchParams = new URLSearchParams();
    let userNameType = 0;//1: email, 2: SDT, 3: user name
    let userName = document.getElementById("userName").value;
    {
        const obj = JSON.parse(CheckUserNameValid(userName));
        if (obj.isValid === false) {
            return;
        }
        if (obj.message === "Email") {
            userNameType = "1";
        }
        else if (obj.message === "SDT") {
            userNameType = "2";
        }
        else {
            userNameType = "3";
        }
    }
    searchParams.append("userName", userName);
    searchParams.append("userNameType", userNameType);

    let passWord = document.getElementById("passWord").value;
    let repassWord = document.getElementById("repassWord").value;
    {
        const obj = JSON.parse(CheckPassWordValid(passWord, repassWord));

        if (obj !== null) {
            if (obj.isValid === false) {
                return;
            }
        }
    }
    searchParams.append("passWord", passWord);

    let privilege = document.getElementById("privilege").value;
    searchParams.append("privilege", privilege);

    let query = "/Administrator/New_AddAdministrator";

    xhttp.open("POST", query);
    xhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    xhttp.send(searchParams.toString());
}
