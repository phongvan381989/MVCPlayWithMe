// Constructor function for pair key/value in cart
// format: id=123#q=10#real=1
function objCartCookie(value) {
    if (DEBUG) {
        console.log("objCartCookie(value) CALL " + value);
    }
    let myArray = value.split("#");

    this.id = parseInt(myArray[0].split("=")[1]);
    this.q = parseInt(myArray[1].split("=")[1]);
    this.real = parseInt(myArray[2].split("=")[1]);
}

function objCartCookieClone(obj) {
    if (DEBUG) {
        console.log("objCartCookieClone CALL");
    }

    this.id = obj.id;
    this.q = obj.q;
    this.real = obj.real;
}

// cookie có dạng: cart=id=123#q=10#real=1$id=321#q=1#real=0$....$id=321#q=2#real=0
// id: mã model, q: số lượng thêm vào giỏ hàng, real: 1-thực sự chọn mua, 0-có thể mua sau này
function GetListCartCookieFromCartCookie(cartCookie) {
    let listCartCookie = [];
    if (isEmptyOrSpaces(cartCookie))
        return listCartCookie;

    let myArray = cartCookie.split("$");
    for (let i = 0; i < myArray.length; i++) {
        listCartCookie.push(new objCartCookie(myArray[i]));
    }
    return listCartCookie;
}

// Từ list object lấy được string cookie
function GetCartCookieFromListCartCookie(listCartCookie) {
    let cartCookie = "";
    for (let i = 0; i < listCartCookie.length; i++) {
        if (i == 0) {
            cartCookie = "id=" + listCartCookie[i].id.toString() + "#q=" + listCartCookie[i].q.toString()
                + "#real=" + listCartCookie[i].real.toString();
        }
        else {
            cartCookie = cartCookie + "$id=" + listCartCookie[i].id.toString()
                + "#q=" + listCartCookie[i].q.toString() + "#real=" + listCartCookie[i].real.toString();
        }
    }
    return cartCookie;
}

// Nếu đối tượng tồn tại, tăng quantity tương ứng, nếu không insert ở đầu mảng
// Nếu đối tượng đã tồn tại, khi tăng quantity tương ứng có thể vượt quá tồn kho. Nếu vượt quá trả về true
function InsertAtBeginToListCartCookieCheckExist(listCartCookie, cartCookieObj, maxQuantity) {
    let exist = false;
    let overMax = false;
    for (let i = 0; i < listCartCookie.length; i++) {
        if (listCartCookie[i].id == cartCookieObj.id) {
            exist = true;
            listCartCookie[i].q = listCartCookie[i].q + cartCookieObj.q;
            if (listCartCookie[i].q > maxQuantity) {
                listCartCookie[i].q = maxQuantity; // Set số lượng về max tồn kho
                overMax = true;
            }
            listCartCookie[i].real = cartCookieObj.real;
            break;
        }
    }
    if (exist == false) {
        listCartCookie.unshift(cartCookieObj);
    }
    return overMax;
}

// listCartCookieObject: danh sách đối tượng cookie
function SetCartCookieFromListCartCookieObject(listCartCookie) {
    if (DEBUG) {
        console.log("SetCartCookieFromListCartCookieObject CALL");
        console.log("listCartCookie: " + JSON.stringify(listCartCookie));
    }
    let newCart = GetCartCookieFromListCartCookie(listCartCookie);
    SetCookie(cartKey, newCart, 365);
}

// Từ trang giỏ hàng, khi xóa sản phẩm, cập nhật vào cookie
function DeleteOneCartCookie(obj) {
    let listOld = GetOldListCartCookie();
    if (DEBUG) {
        console.log("DeleteOneCartCookie CALL listOld: " + JSON.stringify(listOld));
    }

    for (let i = 0; i < listOld.length; i++) {
        if (listOld[i].id == obj.id) {
            listOld.splice(i, 1);
        }
    }
    SetCartCookieFromListCartCookieObject(listOld);
}

// Từ trang giỏ hàng, khi thay đổi số lượng, cập nhật vào cookie
function UpdateQuantityOfCookie(obj) {
    let listOld = GetOldListCartCookie();
    if (DEBUG) {
        console.log("UpdateQuantityOfCookie CALL listOld: " + JSON.stringify(listOld));
    }

    for (let i = 0; i < listOld.length; i++) {
        if (listOld[i].id == obj.id) {
            listOld[i].q = obj.q;
        }
    }
    SetCartCookieFromListCartCookieObject(listOld);
}

// Lấy list cart cookie trước khi thêm mới sản phẩm vào giỏ
function GetOldListCartCookie() {
    // Lấy cookie cũ
    let oldCart = GetCookie(cartKey);

    let listCartCookie = GetListCartCookieFromCartCookie(oldCart);

    if (DEBUG) {
        console.log("listCartCookie of oldCart: " + JSON.stringify(listCartCookie));
    }
    return listCartCookie;
}

// Làm mới real=0 của cart cookie
function RefreshRealOfCartCookieAndGet() {
    if (DEBUG) {
        console.log("RefreshRealOfCartCookieAndGet CALL");
    }
    // Lấy cookie cũ
    let oldCart = GetCookie(cartKey);

    let listCartCookie = GetListCartCookieFromCartCookie(oldCart);

    // Set tất cả real trước đó về real=0
    for (let i = 0; i < listCartCookie.length; i++) {
        if (listCartCookie[i].real != 0) {
            listCartCookie[i].real = 0;
        }
    }

    if (DEBUG) {
        console.log("listCartCookie of oldCart: " + JSON.stringify(listCartCookie));
    }
    SetCartCookieFromListCartCookieObject(listCartCookie);
    return listCartCookie;
}

function DeleteAllCartCookie() {
    DeleteCookie(cartKey);
}