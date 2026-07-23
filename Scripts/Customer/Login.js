async function Login_Login() {
    let userName = document.getElementById("userName").value;
    if (isEmptyOrSpaces(userName)) {
        await CreateMustClickOkModal("Nhập tài khoản.", null);
        userName.focus();
        return;
    }

    let passWord = document.getElementById("passWord").value;
    if (isEmptyOrSpaces(passWord)) {
        await CreateMustClickOkModal("Nhập mật khẩu.", null);
        passWord.focus();
        return;
    }

    const searchParams = new URLSearchParams();
    searchParams.append("userName", userName);
    searchParams.append("passWord", passWord);

    let query = "/Customer/Login_Login";

    ShowCircleLoader();
    let res = await RequestHttpPostPromise(searchParams, query);
    RemoveCircleLoader();

    let resObj = JSON.parse(res.responseText);

    if (resObj.State != 0) {
        await CreateMustClickOkModal(resObj.Message, null);
        return;
    }

    // ✅ Sync cart + addresses từ localStorage lên DB sau khi login thành công
    await SyncGuestDataToServer();

    window.location.href = "/Home";
}

// ✅ AUTO-SYNC: Gửi cart + addresses từ localStorage lên server sau khi login
// Backend sẽ merge cả 2 trong 1 transaction
async function SyncGuestDataToServer() {
    try {
        // 1. Collect guest cart từ localStorage
        let guestCart = null;
        if (typeof CartManager !== 'undefined') {
            guestCart = CartManager.getCart();
        }

        // 2. Collect guest addresses từ localStorage
        let guestAddresses = GetListCustomerInforFromLocalStorage();

        // 3. Nếu không có gì → skip
        if ((!guestCart || guestCart.length === 0) &&
            (!guestAddresses || guestAddresses.length === 0)) {
            return; // Không có gì để sync
        }

        // 4. Gửi 1 API duy nhất (backend merge cả cart + addresses)
        const searchParams = new URLSearchParams();

        if (guestCart && guestCart.length > 0) {
            searchParams.append("guestCartJson", JSON.stringify(guestCart));
        }

        if (guestAddresses && guestAddresses.length > 0) {
            searchParams.append("guestAddressesJson", JSON.stringify(guestAddresses));
        }

        let query = "/Customer/SyncGuestData";

        let res = await RequestHttpPostPromise(searchParams, query);

        let resObj = JSON.parse(res.responseText);

        if (resObj.State === 0) {
            // 5. Clear localStorage sau khi sync thành công
            if (typeof CartManager !== 'undefined') {
                CartManager.clearCart();
            }
            DeleteAllCustomerInfor();

            console.log("✅ Synced guest cart + addresses to server");
        } else {
            console.warn("⚠️ Sync guest data warning:", resObj.Message);
        }
    } catch (e) {
        console.error("❌ Error syncing guest data:", e);
        // Không throw error để không block login flow
    }
}

function GoToCreateCustormerPage() {
    window.location.href = "/Customer/CreateCustomer";
}
