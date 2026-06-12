        //let maxSmallMediaCanShow; // Số lượng iten có thể hiển thị
        let countOfSmallMedia; // Số lượng media gồm video + image
        let selectedIndex; // selected index media đang chọn hiện tại from listItemData
        let listItemData;// Mảng item, phần tử đầu tiên là video nếu có, sau là ảnh
        let itemObject; // object item đại diện đối tượng item server trả về
        let varition; // Models trong item
        let selectedIdModel;// Id của model được chọn để mua
        let quantityInput; // Input trước oninput

        quantityInput = 1;

        let limitQuantity = "Số lượng bạn chọn đã đạt mức tối đa của sản phẩm này";
        let dontSelectVariation = "Vui lòng chọn Phân loại hàng";
        // Get the modal thông báo đã thêm vào giỏ hàng thành công
        let modal = document.getElementById("myModal");
        // Get the modal nhắc số lượng trong giỏ vượt quá tồn kho
        let modalOverMax = document.getElementById("myModal-over-max");
        InitializeTickOkModal();

        // Constructor function for item objects
        function objMediaSrc(itemSrc, itemIsVideo) {
            this.src = itemSrc;
            this.isVideo = itemIsVideo;
        }

        async function GetItemFromId(id) {
            const searchParams = new URLSearchParams();
            searchParams.append("id", id);

            let query = "/Home/GetItemFromId";

            return RequestHttpPostPromise(searchParams, query);
        }

        // Từ danh sách model tính được tổng số lượng model đã bán, model rẻ nhất, model đắt nhất
        function SetPriceAndQuantityItemPage(item) {
            let cheapestModel = null;
            let mostExpensiveModel = null;
            for (let i = 0; i < item.models.length; i++) {

                // Model giá bìa rẻ nhất
                if (cheapestModel == null) {
                    cheapestModel = item.models[i];
                }
                else {
                    if (cheapestModel.bookCoverPrice > item.models[i].bookCoverPrice) {
                        cheapestModel = item.models[i];
                    }
                }
                // Model giá bìa đắt nhất
                if (mostExpensiveModel == null) {
                    mostExpensiveModel = item.models[i];
                }
                else {
                    if (mostExpensiveModel.bookCoverPrice < item.models[i].bookCoverPrice) {
                        mostExpensiveModel = item.models[i];
                    }
                }
            }
            item.cheapestModel = cheapestModel;
            item.mostExpensiveModel = mostExpensiveModel;
        }

        // Từ danh sách mapping của model tính được tồn kho của model
        function SetMaxQuantityOfModel(item) {
            let maxQuan = 0;
            let maxQuanTem = 0;
            for (let i = 0; i < item.models.length; i++) {
                let model = item.models[i];
                maxQuan = 4444;
                for (let j = 0; j < model.mapping.length; j++) {
                    maxQuanTem = Math.floor(model.mapping[j].product.quantity / model.mapping[j].quantity);
                    if (maxQuan > maxQuanTem) {
                        maxQuan = maxQuanTem;
                    }
                }

                model.quantity = maxQuan;
            }
        }

        async function HomePageShowItem() {
            ShowCircleLoader();
            let responseDB = await GetItemFromId(GetValueFromUrlName("id"));
            RemoveCircleLoader();
            if (responseDB.responseText != "null") {
                itemObject = JSON.parse(responseDB.responseText);
                SetPriceAndQuantityItemPage(itemObject);
                SetMaxQuantityOfModel(itemObject);
            }
            else {
                ShowDoesntFindId();
                return;
            }

            // Tính chiều cao item-medium-media
            if (scrWidth >= 800) {
                document.getElementById("item-medium-media").style.height = "600px";
            }
            else {
                document.getElementById("item-medium-media").style.height = scrWidth + "px";
            }

            //// Mỗi video, image item sẽ chiếm 100(width) + 5(margin lef) + 5(margin lef) + 2(border) = 112 px
            //// Tính số item có thể hiển thị
            //maxSmallMediaCanShow = Math.floor(scrWidth / 112);
            if (isEmptyOrSpaces(itemObject.videoSrc)) {
                countOfSmallMedia = 0;
            }
            else {
                countOfSmallMedia = 1;
            }
            countOfSmallMedia = countOfSmallMedia + itemObject.imageSrc.length;

            // Cập nhật dữ liệu cho listItemData
            listItemData = [];
            if (!isEmptyOrSpaces(itemObject.videoSrc)) {
                listItemData.push(new objMediaSrc(itemObject.videoSrc, true));
            }
            for (let i = 0; i < itemObject.imageSrc.length; i++) {
                listItemData.push(new objMediaSrc(itemObject.imageSrc[i], false));
            }

            document.title = itemObject.name;

            ShowSmallItem();

            ShowRightLeftArrow();

            // Chọn item đầu tiên
            ShowMediumMediaFromSelectedSmallItem(document.getElementById("item-small-media-container").children[0]);

            ShowItemSomthing();

            // Hien thi mo ta chi tiet san pham
            ShowProductDescription();
        }

        function CreateContainerSmallItem(ItemData, i) {
            const container = document.createElement("div");
            container.className = "small-media";
            container.setAttribute("data-index", i);
            if (ItemData.isVideo) {
                const video = document.createElement("video");
                video.src = ItemData.src;
                video.controls = false;
                video.style.objectFit = "contain";
                video.style.width = "100%";
                video.style.height = "100%";
                container.style.position = "relative";

                // Thêm video icon
                let videoIcon123 = document.createElement("div");
                videoIcon123.style.position = "absolute"
                videoIcon123.style.top = "30px";
                videoIcon123.style.left = "30px"
                videoIcon123.style.width = "40px";
                videoIcon123.style.height = "40px";
                videoIcon123.style.zIndex = 1;
                videoIcon123.innerHTML = '<svg xmlns = "http://www.w3.org/2000/svg" xmlns: xlink = "http://www.w3.org/1999/xlink" version = "1.1" width = "40" height = "40" viewBox = "0 0 256 256" xml: space = "preserve" ><g style="stroke: none; stroke-width: 0; stroke-dasharray: none; stroke-linecap: butt; stroke-linejoin: miter; stroke-miterlimit: 10; fill: none; fill-rule: nonzero; opacity: 1;" transform="translate(1.4065934065934016 1.4065934065934016) scale(2.81 2.81)"> <path d="M 45 0 C 20.147 0 0 20.147 0 45 c 0 24.853 20.147 45 45 45 s 45 -20.147 45 -45 C 90 20.147 69.853 0 45 0 z M 62.251 46.633 L 37.789 60.756 c -1.258 0.726 -2.829 -0.181 -2.829 -1.633 V 30.877 c 0 -1.452 1.572 -2.36 2.829 -1.634 l 24.461 14.123 C 63.508 44.092 63.508 45.907 62.251 46.633 z" style="stroke: none; stroke-width: 1; stroke-dasharray: none; stroke-linecap: butt; stroke-linejoin: miter; stroke-miterlimit: 10; fill: rgb(0,0,0); fill-rule: nonzero; opacity: 1;" transform=" matrix(1 0 0 1 0 0) " stroke-linecap="round" /> </g> </svg >';

                container.appendChild(video);
                container.appendChild(videoIcon123);
            }
            else {
                const img = document.createElement("img");
                img.src = Get320VersionOfImageSrc(ItemData.src);
                img.style.verticalAlign="baseline"
                container.appendChild(img);
            }
            container.addEventListener("mouseenter", function (event) {
                ShowMediumMediaFromSelectedSmallItem(event.currentTarget);
            });
            container.addEventListener("mousedown", function (event) {
                ShowMediumMediaFromSelectedSmallItem(event.currentTarget);
            });

            return container;
        }

        function ShowSmallItem(
        ) {
            let itemSmallMediaContainer = document.getElementById("item-small-media-container");
            // Xóa item cũ nếu có
            itemSmallMediaContainer.innerHTML = "";

            for (let i = 0; i < listItemData.length; i++) {
                itemSmallMediaContainer.appendChild(CreateContainerSmallItem(listItemData[i], i));
            }
        }

        function ShowRightLeftArrow() {
            // Ẩn mũi tên trái phải khi đang chọn variation
            // Add mũi tên di chuyển sang trái
            let leftArrow = document.getElementById("left_arrow");
            leftArrow.innerHTML = '<svg  xmlns="http://www.w3.org/2000/svg" version="1.1" height="100" width="40" fill-opacity="0.4"> <polygon points="10,50 30,30 30,70" style="fill:lime;" /> </svg>';
            leftArrow.addEventListener("click", function () {
                if (selectedIndex == 0) {
                    selectedIndex = listItemData.length - 1;
                }
                else {
                    selectedIndex--;
                }
                ShowMediumItemFromIndex(selectedIndex);
                ScrollToSelectedSmallItem();
            });

            // Add mũi tên di chuyển sang phải
            let rightArrow = document.getElementById("right_arrow");
            rightArrow.innerHTML = '<svg  xmlns="http://www.w3.org/2000/svg" version="1.1" height="100" width="40" fill-opacity="0.4"> <polygon points="10,30 30,50 10,70" style="fill:lime;" /> </svg>';
            rightArrow.addEventListener("click", function () {
                if (selectedIndex == listItemData.length - 1) {
                    selectedIndex = 0;
                }
                else {
                    selectedIndex++;
                }

                ShowMediumItemFromIndex(selectedIndex);
                ScrollToSelectedSmallItem();
            });
        }

        // Show medium item từ index
        function ShowMediumItemFromIndex(i) {
            // Show medium media
            let mediumImage = document.getElementById("medium_image");
            let mediumVideo = document.getElementById("medium_video");

            if (listItemData[i].isVideo) {

                if (isEmptyOrSpaces(mediumVideo.src)) {
                    mediumVideo.src = listItemData[i].src;
                }

                mediumVideo.play();
                mediumVideo.style.display = "block";
                mediumVideo.style.left = "0px";

                mediumImage.style.display = "none";

            }
            else {
                mediumImage.src = listItemData[i].src;
                if (mediumVideo.src != null) {
                    mediumVideo.pause();
                }
                mediumImage.style.display = "block";
                mediumImage.style.left = "0px";

                mediumVideo.style.display = "none";
            }

            ChangeBorderColorOfSelectdSmallItem();
            document.getElementById("item-medium-media").isCanSwipe = true;
            ShowHideRightLeftArrow()
        }

        // Show medium item từ variation đã chọn, đã đặt chuột
        // Hiển thị ảnh đại diện của variation
        function ShowMediumItemFromSelectedVariation(src) {
            let mediumImage = document.getElementById("medium_image");

            let mediumVideo = document.getElementById("medium_video");
            mediumImage.src = src;

            if (mediumVideo.style.display != "none") {
                mediumVideo.style.display = "none";
                mediumVideo.pause();
                mediumImage.style.display = "block";
            }
            document.getElementById("item-medium-media").isCanSwipe = false;
            ShowHideRightLeftArrow();
            mediumImage.style.cursor = "initial";
        }

        function ShowHideRightLeftArrow() {
            if (document.getElementById("item-medium-media").isCanSwipe) {
                document.getElementById("left_arrow").style.display = "flex";
                document.getElementById("right_arrow").style.display = "flex";
            }
            else {
                document.getElementById("left_arrow").style.display = "none";
                document.getElementById("right_arrow").style.display = "none";
            }
        }

        function ShowProductDescription() {
            if (itemObject.detail == null) {
                // An mo ta san pham
                document.getElementsByClassName("product-detail")[0].style.display = "none";
                return;
            }

            let p = document.createElement("p");
            p.className = "irIKAp";
            p.innerHTML = itemObject.detail;
            document.getElementsByClassName("f7AU53")[0].appendChild(p);
        }

        function GetModelObjectFromModelId(modelId) {
            for (let i = 0; i < itemObject.models.length; i++) {
                if (modelId == itemObject.models[i].id) {
                    return itemObject.models[i];
                }
            }
        }

        // Thay đổi medium media theo variation đã chọn, nếu chưa chọn variation thì check
        // theo small item đã chọn
        function ShowMediumMediaFromSelectedVariationOrSmallItem() {
            // Variation đã chọn và có từ 2 biến thể/ model trở lên
            if (selectedIdModel != null && itemObject.models.length > 1) {
                ShowMediumItemFromSelectedVariation(GetModelObjectFromModelId(selectedIdModel).imageSrc);
            }
            else {
                // Hiển thị small item/media đã chọn
                ShowMediumItemFromIndex(selectedIndex);
            }
        }

        // Thay đổi medium media khi di chuyển bên trên small item
        function ShowMediumMediaFromSelectedSmallItem(newSelectedItem) {
            if (selectedIndex == parseInt(newSelectedItem.getAttribute("data-index")))// là 1 element
                return;

            selectedIndex = parseInt(newSelectedItem.getAttribute("data-index"));

            // Hiển thị item ở kích thước to hơn
            ShowMediumItemFromIndex(selectedIndex);
        }

        // Thay đổi màu border
        function ChangeBorderColorOfSelectdSmallItem() {
            // Set border color về màu không được chọn
            if (selectedIndex != null) {
                let itemSmallMediaContainer = document.getElementById("item-small-media-container");
                for (let i = 0; i < itemSmallMediaContainer.children.length; i++) {
                    if (parseInt(itemSmallMediaContainer.children[i].getAttribute("data-index")) == selectedIndex) {
                        itemSmallMediaContainer.children[i].style.borderColor = "rgb(255, 0, 0)";
                        //if (DEBUG) {
                        //    console.log("selectedIndex: " + selectedIndex);
                        //    console.log("offsetLeft: " + itemSmallMediaContainer.children[i].offsetLeft);
                        //    console.log("scrollLeft: " + itemSmallMediaContainer.scrollLeft);
                        //    console.log("offsetParent.id: " + itemSmallMediaContainer.children[i].offsetParent.id);
                        //}
                    }
                    else {
                        itemSmallMediaContainer.children[i].style.borderColor = "rgb(228, 228, 222)";
                    }
                }
            }
        }

        // Thay đổi medium media bằng vuốt, phím mũi tên, chuột hiển thị nổi bật small item tương ứng
        function ScrollToSelectedSmallItem() {
            let itemSmallMediaContainer = document.getElementById("item-small-media-container");

            for (let i = 0; i < itemSmallMediaContainer.children.length; i++) {
                if (parseInt(itemSmallMediaContainer.children[i].getAttribute("data-index")) == selectedIndex) {
                    let offsetLeft = itemSmallMediaContainer.children[i].offsetLeft;
                    //let offsetWidth = itemSmallMediaContainer.children[i].offsetWidth;
                    //let scrollWidth = itemSmallMediaContainer.scrollWidth;
                    //if (DEBUG) {
                    //    console.log(
                    //        "offsetLeft: " + itemSmallMediaContainer.children[i].offsetLeft +
                    //        "  offsetWidth: " + itemSmallMediaContainer.children[i].offsetWidth
                    //        + "   scrollWidth:" + itemSmallMediaContainer.scrollWidth +
                    //        "  scrollLeft : " + itemSmallMediaContainer.scrollLeft);
                    //}
                    // Mỗi video, image small item sẽ chiếm 100(width) + 5(margin lef) + 5(margin lef) = 110 px
                    // Không nhìn thấy toàn bộ small item
                    if (offsetLeft + 110 > itemSmallMediaContainer.scrollLeft + itemSmallMediaContainer.clientWidth) {
                        itemSmallMediaContainer.scrollLeft =
                            offsetLeft + 110 - itemSmallMediaContainer.clientWidth;
                    }
                    else if (offsetLeft < itemSmallMediaContainer.scrollLeft) {
                        itemSmallMediaContainer.scrollLeft = offsetLeft;
                    }
                }
            }
        }


        // Lấy số lượng max của 1 variation/model.
        // Nếu chưa chọn variation / model số lượng max = tổng số lượng các variation / model
        // trong item
        function GetMaxQuantityInput() {
            let maxQuantity = 0;
            if (selectedIdModel == null) { // tổng số lượng các variation / model trong item
                for (let i = 0; i < itemObject.models.length; i++) {
                    maxQuantity = maxQuantity + itemObject.models[i].quantity;
                }
            }
            else {
                maxQuantity = GetModelObjectFromModelId(selectedIdModel).quantity;
            }
            return maxQuantity;
        }

        // Thay đổi medium media khi di chuyển chuột trên variation button
        function VariationMouseEnter(newSelectVariation) {
            newSelectVariation.style.borderColor = "rgba(255, 0, 0)";
            ShowMediumItemFromSelectedVariation(newSelectVariation.getElementsByClassName("variation-image")[0].src);
        }

        // Thay đổi medium media khi di chuyển chuột ra khỏi variation button
        function VariationMouseLeave(newSelectVariation) {
            ShowMediumMediaFromSelectedVariationOrSmallItem();
            if (parseInt(newSelectVariation.getAttribute("data-model-id")) != selectedIdModel) {
                newSelectVariation.style.borderColor = "rgba(0, 0, 0, .09)";
            }
        }

        function VariationClick(newSelectVariation) {
            // Lấy id variation/ model
            let id = parseInt(newSelectVariation.getAttribute("data-model-id"));

            // Bỏ dòng chữ chưa chọn phân loại hàng nếu đang hiển thị
            {
                if (document.getElementsByClassName("I-H1Co")[0].style.display != "none") {
                    document.getElementsByClassName("I-H1Co")[0].style.display = "none";
                }
                // Thay màu nền div chứa phân loại về mặc định
                document.getElementsByClassName("_7VDqtl")[0].style.backgroundColor = "initial";
            }
            // Bỏ chọn variontion cũ nếu có
            if (selectedIdModel != null) {
                if (selectedIdModel != id) {//  Bỏ chọn border màu đỏ button cũ
                    document.getElementById("check-container").parentElement.style.borderColor = "rgba(0, 0, 0, .09)";
                }
                // Bỏ icon V
                document.getElementById("check-container").remove();
                if (selectedIdModel == id) {
                    selectedIdModel = null;
                    ShowPriceAndReadyMaxQuantity();
                    return;
                }
            }

            selectedIdModel = id;
            // Hiển thị đã chọn V đỏ góc dưới phải nếu chưa có, có rồi thì bỏ chọn
            let checkContainer = document.createElement("div");
            checkContainer.id = "check-container";

            checkContainer.innerHTML = '<svg xmlns = "http://www.w3.org/2000/svg" xmlns: xlink = "http://www.w3.org/1999/xlink" version = "1.1" viewBox="0 0 12 12" class="shopee-svg-icon icon-tick-bold"><g><path d="m5.2 10.9c-.2 0-.5-.1-.7-.2l-4.2-3.7c-.4-.4-.5-1-.1-1.4s1-.5 1.4-.1l3.4 3 5.1-7c .3-.4 1-.5 1.4-.2s.5 1 .2 1.4l-5.7 7.9c-.2.2-.4.4-.7.4 0-.1 0-.1-.1-.1z"></path></g></svg>';
            newSelectVariation.appendChild(checkContainer);
            // Luôn hiển thị border màu đỏ với variation đã chọn
            newSelectVariation.style.borderColor = "rgb(255, 0, 0)";
            ShowPriceAndReadyMaxQuantity();
        }

        function GetModelObjectFromModelId(modelId) {
            let modelObject = null;
            for (let i = 0; i < itemObject.models.length; i++) {
                if (modelId == itemObject.models[i].id) {
                    modelObject = itemObject.models[i];
                    break;
                }
            }
            return modelObject;
        }

        // Show giá bìa, giá bán và % triết khấu
        function ShowPrice() {
            // Chưa chọn model nên ta hiển thị biến thể giá nhỏ nhất - đắt nhất, chiết khấu lớn nhất,
            // thường các biến thể cùng item sẽ chiết khấu như nhau
            if (selectedIdModel == null) {
                // Giá bìa
                document.getElementById("book-cover-price").innerHTML =
                    ConvertMoneyToTextWithIcon(itemObject.cheapestModel.bookCoverPrice) + " - " +
                    ConvertMoneyToTextWithIcon(itemObject.mostExpensiveModel.bookCoverPrice);

                // Giá bán thực tế
                document.getElementById("price").innerHTML =
                    ConvertMoneyToTextWithIcon(itemObject.cheapestModel.price) + " - " +
                    ConvertMoneyToTextWithIcon(itemObject.mostExpensiveModel.price);

                // % Chiết khấu
                let discount = itemObject.cheapestModel.discount;

                document.getElementById("discount").innerHTML = discount + "% GIẢM";
            }
            else {
                let modelObject = GetModelObjectFromModelId(selectedIdModel);

                // Giá bìa
                document.getElementById("book-cover-price").innerHTML =
                    ConvertMoneyToTextWithIcon(modelObject.bookCoverPrice);

                // Giá bán thực tế
                document.getElementById("price").innerHTML =
                    ConvertMoneyToTextWithIcon(modelObject.price);

                // % Chiết khấu
                let discount = modelObject.discount;
                document.getElementById("discount").innerHTML = discount + "% GIẢM";
            }
        }

        function ShowReadyMaxQuantity() {
            document.getElementById("max-quatity").innerHTML = GetMaxQuantityInput().toString() + " sản phẩm có sẵn";
        }

        function ShowPriceAndReadyMaxQuantity() {
            ShowPrice();
            ShowReadyMaxQuantity();

            // Refresh input quantity ve 1
            quantityInput = 1;
            if (GetMaxQuantityInput() == 0)
                quantityInput = 0;
            document.getElementById("quantity-input").value = quantityInput.toString();
            if (document.getElementsByClassName("I-H1Co")[0].style.display != "none") {
                document.getElementsByClassName("I-H1Co")[0].style.display = "none";
            }
        }

        // Gồm: Tên, giá bìa, giá chiết khấu, lựa chọn
        function ShowItemSomthing() {
            if (itemObject == null)
                return;

            // Show tên
            document.getElementById("item-name-h1").innerHTML = itemObject.name;

            if (itemObject.models.length > 1) {
                // Thêm các biến thể
                let variationContainer = document.getElementById("item-variation-container");
                for (let i = 0; i < itemObject.models.length; i++) {
                    let variationButton = document.createElement("button");
                    variationButton.type = "button";
                    variationButton.innerHTML = itemObject.models[i].name;
                    variationButton.className = "variation-button";
                    variationButton.setAttribute("data-model-id", itemObject.models[i].id);
                    variationButton.addEventListener("click", function (event) {
                        VariationClick(event.currentTarget);
                    });

                    variationButton.addEventListener("mouseenter", function (event) {
                        VariationMouseEnter(event.currentTarget);
                    });

                    variationButton.addEventListener("mouseleave", function (event) {
                        VariationMouseLeave(event.currentTarget);
                    });

                    let variationImg = document.createElement("img");
                    variationImg.className = "variation-image";
                    variationImg.src = Get320VersionOfImageSrc(itemObject.models[i].imageSrc);
                    variationButton.appendChild(variationImg);

                    variationContainer.appendChild(variationButton);
                }
            }
            else {
                // Mặc định chọn biến thể là model duy nhất
                selectedIdModel = itemObject.models[0].id;
            }
            // Show giá bìa, giá bán và % triết khấu, so luong max
            ShowPriceAndReadyMaxQuantity();

            // Show nút Shopee để di chuyển qua sàn Shopee
            if (itemObject.shopeeItemId != 0) {
                // Đường dẫn đến sản phẩm trên Shopee
                document.getElementsByClassName("fcak09va86")[0].href = GetShopeeItemUrl(itemObject.shopeeItemId);
            }
            else {
                // Đường dẫn đến cửa hàng trên Shopee
                document.getElementsByClassName("fcak09va86")[0].href = "https://shopee.vn/voibenhobooks";
            }
        }

        function Decrease() {
            if (quantityInput > 1) {
                quantityInput = quantityInput - 1;
                document.getElementById("quantity-input").value = quantityInput.toString();
            }
            if (document.getElementsByClassName("I-H1Co")[0].style.display != "none") {
                document.getElementsByClassName("I-H1Co")[0].style.display = "none";
            }
        }

        function Increase() {
            let maxQuatity = GetMaxQuantityInput();
            if (quantityInput < maxQuatity) {
                quantityInput = quantityInput + 1;
                document.getElementById("quantity-input").value = quantityInput.toString();
            }
            else {
                document.getElementsByClassName("I-H1Co")[0].innerHTML = limitQuantity;
                document.getElementsByClassName("I-H1Co")[0].style.display = "block";
            }
        }

        function ValidateInput(event) {
            let newInput = document.getElementById("quantity-input").value;
            if (IsNumeric(newInput)) {
                let iInput = ConvertToInt(newInput);
                let maxQuantity = GetMaxQuantityInput();
                if (iInput === 0) {
                    quantityInput = 1;
                    if (maxQuantity == 0) {
                        quantityInput = 0;
                    }
                }

                if (iInput > maxQuantity) {
                    quantityInput = maxQuantity;
                    document.getElementsByClassName("I-H1Co")[0].style.display = "block";
                }
                else {
                    quantityInput = iInput;
                    if (document.getElementsByClassName("I-H1Co")[0].style.display != "none") {
                        document.getElementsByClassName("I-H1Co")[0].style.display = "none";
                    }
                }
            }
            document.getElementById("quantity-input").value = quantityInput.toString();
        }

        function DontSelectModel() {
            // Chưa chọn phân loại hiện thị thông báo dòng chữ đỏ và thay màu nền div chứa phân loại
            document.getElementsByClassName("I-H1Co")[0].innerHTML = dontSelectVariation;
            document.getElementsByClassName("I-H1Co")[0].style.display = "block";
            // Thay màu nền div chứa phân loại
            document.getElementsByClassName("_7VDqtl")[0].style.backgroundColor = "#dee2e6";
        }

        // Thêm sản phẩm muốn mua vào giỏ hàng
        // Sản phẩm thêm vào giỏ hàng sẽ có real=0
        // cookie có dạng: cart=id=123#q=10#real=1$id=321#q=1#real=0$....$id=321#q=2#real=0
        async function AddToCart() {

            // Chưa chọn phân loại hiện thị thông báo dòng chữ đỏ và thay màu nền div chứa phân loại
            if (selectedIdModel == null) {
                //document.getElementsByClassName("I-H1Co")[0].innerHTML = dontSelectVariation;
                //document.getElementsByClassName("I-H1Co")[0].style.display = "block";
                //// Thay màu nền div chứa phân loại
                //document.getElementsByClassName("_7VDqtl")[0].style.backgroundColor = "#dee2e6";
                DontSelectModel();
                return;
            }

            let maxQuantity = GetMaxQuantityInput();
            let q = parseInt(document.getElementById("quantity-input").value);
            let overMax = false; // true: Nếu số lượng khách chọn + số lượng đã chọn lưu trong cookie/ db vượt quá số lượng max
            let obj = new objCartCookie("id=" + selectedIdModel.toString() + "#q=" + q.toString()
                + "#real=0");
            // Là khách vãng lai
            if (CheckAnonymousCustomer()) {
                let listCartCookie = RefreshRealOfCartCookieAndGet();

                let overMax = InsertAtBeginToListCartCookieCheckExist(listCartCookie, obj, maxQuantity);
                if (overMax) { // Hiện thị modal thông báo vượt tồn kho
                    document.getElementsByClassName("shopee-alert-popup__message")[0].innerHTML =
                        "Bạn đã có " + maxQuantity.toString() + " sản phẩm trong giỏ hàng. Không thể thêm số lượng đã chọn vào giỏ hàng vì sẽ vượt quá tồn kho của cửa hàng"
                    modalOverMax.style.display = "flex";
                }
                SetCartCookieFromListCartCookieObject(listCartCookie);
            }
            else { // Khách đăng nhập
                //
                let responseDB = await Item_AddModelToCart(obj, maxQuantity);
                let resultObj = JSON.parse(responseDB.responseText);
                if (resultObj.State == 4) { // Hiện thị modal thông báo vượt tồn kho
                    overMax = true;
                    document.getElementsByClassName("shopee-alert-popup__message")[0].innerHTML =
                        "Bạn đã có " + maxQuantity.toString() + " sản phẩm trong giỏ hàng. Không thể thêm số lượng đã chọn vào giỏ hàng vì sẽ vượt quá tồn kho của cửa hàng"
                    modalOverMax.style.display = "flex";
                }

            }
            await UpdateCartCount();

            if (overMax) {
                return;
            }
            ShowModal();
            setTimeout(CloseModal, 1000);
        }

        async function Item_AddModelToCart(obj, maxQuantity) {
            //
            const searchParams = new URLSearchParams();
            searchParams.append("uidCookie", GetCookie(uidKey));
            searchParams.append("cartObj", JSON.stringify(obj));
            searchParams.append("maxQuantity", maxQuantity);


            let query = "/Home/Item_AddModelToCart";

            return await RequestHttpPostPromise(searchParams, query);
        }

        // Chỉ khi đang chọn model mới chuyển sang trang giỏ hàng, chưa chọn hiện modal thông báo
        // Model đang chọn sẽ có real=1
        async function BuyNow() {
            // Chưa chọn phân loại hiện thị thông báo dòng chữ đỏ và thay màu nền div chứa phân loại
            if (selectedIdModel == null) {
                DontSelectModel();
                return;
            }

            let maxQuantity = GetMaxQuantityInput();
            let q = parseInt(document.getElementById("quantity-input").value);
            let obj = new objCartCookie("id=" + selectedIdModel.toString() + "#q=" + q.toString()
                + "#real=1");


            //let overMax = false; // true: Nếu số lượng khách chọn + số lượng đã chọn lưu trong cookie/ db vượt quá số lượng max
            // Là khách vãng lai
            if (CheckAnonymousCustomer()) {
                let listCartCookie = RefreshRealOfCartCookieAndGet();

                InsertAtBeginToListCartCookieCheckExist(listCartCookie, obj, maxQuantity);

                SetCartCookieFromListCartCookieObject(listCartCookie);

                location.href = "/Home/Cart";

            }
            else { // Khách đăng nhập
                let responseDB = await Item_AddModelToCart(obj, maxQuantity);
                //let resultObj = JSON.parse(responseDB.responseText);
                location.href = "/Home/Cart";
            }
        }

        function CloseModal() {
            modal.style.display = "none";
        }

        function CloseOverMaxModal() {
            modalOverMax.style.display = "none";
        }

        function ShowModal() {
            modal.style.display = "flex";
        }

        function InitializeTickOkModal() {
            // Get the <span> element that closes the modal
            let tickOk = document.getElementsByClassName("tick-ok")[0];

            tickOk.onclick = function () {
                CloseModal();
            }

            // When the user clicks anywhere outside of the modal, close it
            window.onclick = function (event) {
                if (event.target == modal) {
                    CloseModal();
                }
            }
        }
        let isHoldDownStateMouse = false;
        let clientXDown;
        let clientXOld;

        let clientXTouchStart;
        let clientYTouchStart;
        let clientXTouchMoveOld;
        let clientYTouchMoveOld;
        let statusNextPreviousNone; // 0: Next, 1: Previous, 2: None

        function ShowImageFromSwipeEventCore() {
            if (statusNextPreviousNone == 0) {// Next
                if (selectedIndex == listItemData.length - 1) {
                    selectedIndex = 0;
                }
                else {
                    selectedIndex++;
                }
            }
            else if (statusNextPreviousNone == 1) { // Previous
                if (selectedIndex == 0) {
                    selectedIndex = listItemData.length - 1;
                }
                else {
                    selectedIndex--;
                }
            }
            ShowMediumItemFromIndex(selectedIndex);
            ScrollToSelectedSmallItem();
        }

        function MouseUpMediumMedia(event) {
            if (!event.target.parentElement.isCanSwipe) {
                return;
            }

            event.target.style.cursor = "grab";

            if (isHoldDownStateMouse) {
                ShowImageFromSwipeEventCore();
            }

            isHoldDownStateMouse = false;
            statusNextPreviousNone = 2;
        }

        function MouseDownMediumMedia(event) {
            if (!event.target.parentElement.isCanSwipe) {
                return;
            }

            event.preventDefault()
            isHoldDownStateMouse = true;
            clientXOld = event.clientX;
            clientXDown = event.clientX;
            event.target.style.cursor = "grabbing";
            statusNextPreviousNone = 2;
        }

        function MouseEnterMediumMedia(event) {
            if (!event.target.parentElement.isCanSwipe) {
                return;
            }

            isHoldDownStateMouse = false;
            statusNextPreviousNone = 2;

            event.target.style.cursor = "grab";
        }

        function MouseMoveMediumMedia(event) {
            if (!event.target.parentElement.isCanSwipe) {
                return;
            }

            if (!isHoldDownStateMouse) {
                return;
            }

            let currentX = event.clientX;
            let diffX = currentX - clientXOld;
            clientXOld = currentX;

            //if (DEBUG) {
            //    console.log("currentX: " + currentX);
            //    console.log("clientXOld: " + clientXOld);
            //}

            if (diffX > 0) { // Previous
                statusNextPreviousNone = 1;
            }
            else if (diffX < 0) { // Next
                statusNextPreviousNone = 0;
            }
            //else { // None
            //    statusNextPreviousNone = 2;
            //}
            if (isHoldDownStateMouse) {
                event.target.style.left = currentX - clientXDown + "px";
            }
        }

        function MouseOutMediumMedia(event) {
            if (!event.target.parentElement.isCanSwipe) {
                return;
            }

            if (isHoldDownStateMouse) {
                ShowImageFromSwipeEventCore();
            }

            isHoldDownStateMouse = false;
            statusNextPreviousNone = 2;
        }

        function CheckTouchVerticalMoving(currentX, currentY) {
            if (Math.abs(currentX - clientXTouchStart) > Math.abs(currentY - clientYTouchStart)) {
                return false;
            }
            return true;
        }

        function TouchMoveMediumMedia(event) {
            if (!event.target.parentElement.isCanSwipe) {
                return;
            }

            let currentX = Math.floor(event.touches[0].clientX);
            let currentY = Math.floor(event.touches[0].clientY);
            clientYTouchMoveOld = currentY;
            if (CheckTouchVerticalMoving(currentX, currentY)) {
                clientXTouchMoveOld = currentX;
                return;
            }

            event.preventDefault();
            if (clientXTouchMoveOld === null) {
                clientXTouchMoveOld = currentX;
                statusNextPreviousNone = 2;
                return;
            }

            let diffX = currentX - clientXTouchMoveOld;
            clientXTouchMoveOld = currentX;
            if (diffX > 0) { // Previous
                statusNextPreviousNone = 1;
            }
            else if (diffX < 0) { // Next
                statusNextPreviousNone = 0;
            }
            //else { // None
            //    statusNextPreviousNone = 2;
            //}

            event.target.style.left = clientXTouchMoveOld - clientXTouchStart + "px";
        }

        function TouchEndMediumMedia(event) {
            if (!event.target.parentElement.isCanSwipe) {
                return;
            }

            if (CheckTouchVerticalMoving(clientXTouchMoveOld, clientYTouchMoveOld)) {
                return;
            }
            event.preventDefault();
            ShowImageFromSwipeEventCore();

        }

        function TouchStartMediumMedia(event) {
            if (!event.target.parentElement.isCanSwipe) {
                return;
            }

            clientXTouchStart = Math.floor(event.touches[0].clientX);
            clientYTouchStart = Math.floor(event.touches[0].clientY);
            statusNextPreviousNone = 2;
        }
