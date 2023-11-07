//Số item hiển thị trên 1 hàng
let itemOnRow;
// Hardcode số row trên 1 page
let rowOnPage = 5;
function orientationChange() {
    let scrWidth = screen.availWidth;
    let widthProItem = 200;
    itemOnRow = Math.floor(scrWidth / widthProItem);
    if (itemOnRow > 6) {
        scrWidth = 1200;
        const collection = document.getElementsByClassName("product-for-selector-container");
        for (let i = 0; i < collection.length; i++) {
            collection[i].style.width = widthProItem.toString() + "px";
        }
        itemOnRow = 6;
    }
    else if (itemOnRow < 2) {
        //scrWidth = 400;
        let tempWidth = Math.floor(scrWidth / 2) - 1;
        tempWidth = tempWidth.toString() + "px";
        const collection = document.getElementsByClassName("product-for-selector-container");
        for (let i = 0; i < collection.length; i++) {
            collection[i].style.width = tempWidth;
        }
        itemOnRow = 2;
    }
    else {
        scrWidth = 200 * itemOnRow;
        const collection = document.getElementsByClassName("product-for-selector-container");
        for (let i = 0; i < collection.length; i++) {
            collection[i].style.width = widthProItem.toString() + "px";
        }
    }
    //// Cap nhat biggestContainer_top thanh 2 hang tren thiet bi chieu rong nho
    if (scrWidth <= 600) {
        document.getElementById("biggestContainer_top_first").style.display = "block";
    }
    else {
        document.getElementById("biggestContainer_top_first").style.display = "flex";
    }

    // Cap nhat chieu rong theo man hinh thiet bi
    scrWidth = scrWidth.toString() + "px";
    document.getElementById("biggestContainer").style.width = scrWidth;

    return scrWidth;
}

orientationChange();


let portrait = window.matchMedia("(orientation: portrait)");
portrait.addEventListener("change", function (e) {
    //alert("Hello World1!");
    orientationChange();
});