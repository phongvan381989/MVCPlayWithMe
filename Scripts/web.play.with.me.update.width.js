function orientationChange() {
    let scrWidth = screen.availWidth;
    let widthProItem = 200;
    let quotient = Math.floor(scrWidth / widthProItem);
    if (quotient > 6) {
        scrWidth = 1200;
        const collection = document.getElementsByClassName("product-for-selector-container");
        for (let i = 0; i < collection.length; i++) {
            collection[i].style.width = widthProItem.toString() + "px";
        }
    }
    else if (quotient < 2) {
        //scrWidth = 400;
        let tempWidth = Math.floor(scrWidth / 2);
        tempWidth = tempWidth.toString() + "px";
        const collection = document.getElementsByClassName("product-for-selector-container");
        for (let i = 0; i < collection.length; i++) {
            collection[i].style.width = tempWidth;
        }
    }
    else {
        scrWidth = 200 * quotient;
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

    document.getElementById("result-insert").innerHTML = scrWidth + ": " + document.getElementById("biggestContainer_top_first").style.display;
    
    return scrWidth;
}

orientationChange();


let portrait = window.matchMedia("(orientation: portrait)");
portrait.addEventListener("change", function (e) {
    //alert("Hello World1!");
    orientationChange();
});