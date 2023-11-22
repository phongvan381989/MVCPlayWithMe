var cartKey = "cart";

// Constructor function for pair key/value in cart
// format: id=123#q=10#real=1
function objCartCookie(value) {
    if (DEBUG) {
        console.log("objCartCookie(value) CALL " + value);
    }
    let myArray = value.split("#");

    this.id = parseInt(myArray[0].split("=")[1]);
    this.q = parseInt(myArray[1].split("=")[1]);
    this.real = parseInt(myArray[2].split("=")[1])
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
            break;
        }
    }
    if (exist == false) {
        listCartCookie.unshift(cartCookieObj);
    }
    return overMax;
}