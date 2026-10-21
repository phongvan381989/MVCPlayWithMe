using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.ProductModel;
using MVCPlayWithMe.OpenPlatform;
using MVCPlayWithMe.OpenPlatform.API.LazadaAPI;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MVCPlayWithMe.Controllers.ProductSupporting
{
    public class LazadaProductSupporting
    {
        // Từ những sku không cập nhật thành công,
        // ta truy ngược lại những sản phẩm thuộc item không cập nhật thành công
        static public void LazadaUpdateQuantityPrice_SpecialPriceError(List<CommonItem> listCommonItem,
            List<LazadaParameterQuantity_PriceUpdate> skus
            )
        {
            foreach (var sku in skus)
            {
                if (!string.IsNullOrEmpty(sku.message))
                {
                    foreach (var item in listCommonItem)
                    {
                        if (item.itemId == sku.itemId)
                        {
                            foreach (var model in item.models)
                            {
                                if (model.modelId == sku.skuId)
                                {
                                    model.whyUpdateFail = sku.message;
                                }
                            }
                        }
                    }
                }
            }
        }

        static public async Task LazadaUpdateQuantity_CoreAsync(List<CommonItem> listCommonItem)
        {
            List<LazadaParameterQuantity_PriceUpdate> skus = new List<LazadaParameterQuantity_PriceUpdate>();
            foreach (var commonItem in listCommonItem)
            {
                foreach (var commonModel in commonItem.models)
                {
                    // Chưa mapping set quantity = 0
                    skus.Add(new LazadaParameterQuantity_PriceUpdate(commonItem.itemId,
                        commonModel.modelId,
                        commonModel.GetQuantityFromListMapping()));
                }
            }
            if (skus.Count == 0)
            {
                return;
            }

            Boolean isOk = await LazadaProductAPI.LazadaUpdateQuantityAsync(skus);
            if (!isOk)
            {
                LazadaUpdateQuantityPrice_SpecialPriceError(listCommonItem, skus);
            }
        }

        // Trả về danh sách id sản phẩm mapping với sku cập nhật lỗi
        public static async Task<List<CommonItem>> LazadaGetListNeedUpdateQuantityAndUpdateAsync(MySqlConnection conn)
        {
            List<CommonItem> listCommonItem = await LazadaMySql.LazadaGetListNeedUpdateQuantityConnectOutAsync(conn);

            await LazadaUpdateQuantity_CoreAsync(listCommonItem);
            return listCommonItem;
        }

        static public async Task LazadaUpdatePrice_SpecialPrice_Core(List<CommonItem> listCommonItem,
    MySqlConnection conn)
        {
            List<LazadaParameterQuantity_PriceUpdate> skus = new List<LazadaParameterQuantity_PriceUpdate>();

            // Lấy danh sách thuế phí
            TaxAndFee taxAndFee = await TikiDealDiscountMySql.GetTaxAndFeeAsync(Common.eLazada, conn);

            // Lấy danh sách nhà phát hành, từ đó lấy được discount chung
            List<Publisher> listPublisher = await PublisherMySql.GetListPublisherConnectOutAsync(conn);

            // Tính giá bìa, chiết khấu hợp lý theo nhà phát hành hoặc sản phẩm, thuế, phí, lợi nhuận mong muốn.
            // Từ đó tính giá bán.
            int special_price;

            foreach (var commonItem in listCommonItem)
            {
                foreach (var commonModel in commonItem.models)
                {
                    if (commonModel.mapping.Count == 0)
                    {
                        continue;
                    }

                    special_price = CommonOpenPlatform.CaculateSpecialPriceCoreFromCommonModel(commonModel,
                        listPublisher,
                        taxAndFee);

                    skus.Add(new LazadaParameterQuantity_PriceUpdate(commonItem.itemId,
                        commonModel.modelId,
                        commonModel.GetBookCoverPrice(),
                        special_price));
                }
            }
            if (skus.Count == 0)
            {
                return;
            }

            Boolean isOk = await LazadaProductAPI.LazadaUpdatePrice_SpecialPriceAsync(skus);
            if (!isOk)
            {
                LazadaUpdateQuantityPrice_SpecialPriceError(listCommonItem, skus);
            }
        }

        static public async Task LazadaUpdateQuantityPrice_SpecialPrice_Core(List<CommonItem> listCommonItem,
            MySqlConnection conn)
        {
            List<LazadaParameterQuantity_PriceUpdate> skus = new List<LazadaParameterQuantity_PriceUpdate>();

            // Lấy danh sách thuế phí
            TaxAndFee taxAndFee = await TikiDealDiscountMySql.GetTaxAndFeeAsync(Common.eLazada, conn);

            // Lấy danh sách nhà phát hành, từ đó lấy được discount chung
            List<Publisher> listPublisher = await PublisherMySql.GetListPublisherConnectOutAsync(conn);

            // Tính giá bìa, chiết khấu hợp lý theo nhà phát hành hoặc sản phẩm, thuế, phí, lợi nhuận mong muốn.
            // Từ đó tính giá bán.
            int sale_price;

            foreach (var commonItem in listCommonItem)
            {
                foreach (var commonModel in commonItem.models)
                {
                    if (commonModel.mapping.Count == 0)
                    {
                        continue;
                    }

                    sale_price = CommonOpenPlatform.CaculateSpecialPriceCoreFromCommonModel(commonModel,
                        listPublisher,
                        taxAndFee);

                    skus.Add(new LazadaParameterQuantity_PriceUpdate(commonItem.itemId,
                        commonModel.modelId,
                        commonModel.GetQuantityFromListMapping(),
                        commonModel.GetBookCoverPrice(),
                        sale_price));
                }
            }
            if (skus.Count == 0)
            {
                return;
            }

            Boolean isOk = await LazadaProductAPI.LazadaUpdateQuantityPrice_SpecialPriceAsync(skus);
            if (!isOk)
            {
                LazadaUpdateQuantityPrice_SpecialPriceError(listCommonItem, skus);
            }
        }

        // Cập nhật số lượng sản phẩm của 1 model từ itemId, modelId
        static public async Task<MySqlResultState> LazadaUpdateQuantityOfOneItemModelAsync(long itemId,
            long modelId,
            MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            int quantity = await ProductMySql.LazadaGetQuantityOfOneItemModelConnectOutAsync(itemId, modelId, conn);

            result.myJson = await LazadaProductAPI.LazadaUpdateQuantityOfOneItemModelAsync(
                new LazadaParameterQuantity_PriceUpdate(itemId, modelId, quantity));

            return result;
        }

        public static MySqlResultState LazadaCheckValidProductInfo(Product product)
        {
            MySqlResultState result = new MySqlResultState();

            if (product.status == 2)
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "Sản phẩm đã ngừng kinh doanh.";
                return result;
            }

            if (product.imageSrc.Count == 0)
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "Sản phẩm chưa có ảnh. Cần tối thiểu 1 ảnh.";
                return result;
            }

            if (product.categoryId <= 0)
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "Chưa có thông tin thể loại.";
                return result;
            }

            if (product.bookCoverPrice <= 0)
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "Giá không hợp lý.";
                return result;
            }

            if (product.productLong <= 0 ||
                product.productHigh <= 0 ||
                product.productWide <= 0)
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "Kích thước sản phẩm quá nhỏ.";
                return result;
            }

            if (product.productWeight < 1)
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "Khối lượng sản phẩm quá nhẹ.";
                return result;
            }

            string realDetail = Common.CleanHtml_ByTwo(product.detail);

            if (realDetail.Length < Product.minLengthDetail)
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "Mô tả sản phẩm ngắn hơn " + Product.minLengthDetail.ToString() + " ký tự.";
                return result;
            }

            if (realDetail.Length > Product.maxLengthDetail)
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "Mô tả sản phẩm dài hơn " + Product.maxLengthDetail.ToString() + " ký tự.";
                return result;
            }

            return result;
        }

        private static async Task LazadaUploadImage(Boolean isNeedUploadImage,
            Product product,
            MySqlConnection conn,
            MySqlResultState result)
        {
            if (isNeedUploadImage)
            {
                // Cần kiểm tra xem đã up ảnh lên lazada chưa? 
                // Không mỗi lần đăng sản phẩm lại up ảnh lại không cần thiết
                int count = await LazadaMySql.GetQuantityOfProductImageUploadedToLazadaAsync(0, product.id, 0, conn);

                if (count == -1)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Không đếm được số ảnh sản phẩm đã up lên shopp.";
                    return;
                }
                if (count == 0 || // chưa up ảnh lên sàn
                    (count != LazadaProductAPI.maximumOfImages
                    && product.imageSrc.Count != count))// SỐ lượng ảnh sản phẩm và ảnh đã up có sự thay đổi. Ta up lại.
                {

                    // Trước khi up xóa bỏ id ảnh cũ nếu có
                    if (count > 0)
                    {
                        result = await LazadaMySql.DeleteProductImageUploadedToLazadaAsync(0, product.id, 0, conn);
                        if (result.State != EMySqlResultState.OK)
                        {
                            return;
                        }
                    }

                    result = await CommonProductSupporting.UploadImageOnECommerce_Core(product.id, Common.eLazada, conn);
                    if (result.State != EMySqlResultState.OK)
                    {
                        return;
                    }
                }
            }
        }

        public static string LazadaGetLanguage(string lang)
        {
            string language = "Vietnamese";
            if (lang == "Song ngữ")
            {
                language = "Vietnamese,English";
            }
            else if (lang == "Tiếng Anh")
            {
                language = "English";
            }
            return language;
        }

        public static void LazadaGetBrandForAttributesRequest(AttributesRequest attribute,
            Product product)
        {
            long brandId = 65074; // tương ứng với "No Brand"
            string brand = "No Brand";
            if (product.publisherId == 18) // Wabook
            {
                brandId = 184898;
                brand = "Wabooks";
            }
            else if (product.publisherId == 19) // Muki
            {
                // NOTE: Muki không có thương hiệu nên ta đăng nhờ thương hiệu FAHASA
                brandId = 2101;
                brand = "FAHASA";
            }
            else if (product.publisherId == 20) // Đinh Tị Books
            {
                brandId = 124098239;
                brand = "Đinh Tị Books";
            }
            else if (product.publisherId == 31) // Alpha Books
            {
                brandId = 206133;
                brand = "Alpha Books";
            }
            else if (product.publisherId == 33) // Kennen
            {
                brandId = 127283688;
                brand = "KENNEN";
            }
            else if (product.publisherId == 35) // Lalala Baby
            {
                brandId = 123312437;
                brand = "lalala baby";
            }
            else if (product.publisherId == 44) // Quảng Văn
            {
                brandId = 2113;
                brand = "Quang Van";
            }

            attribute.brand_id = brandId.ToString();
            attribute.brand = brand;
        }

        public static void LazadaGetBrandForAttributesRequest_FromItem(AttributesRequest attribute,
            ItemForCreate item)
        {
            long brandId = 65074; // tương ứng với "No Brand"
            string brand = "No Brand";
            if (item.publisher.ToLower().Contains("wabooks")) // Wabook
            {
                brandId = 184898;
                brand = "Wabooks";
            }
            else if (item.publisher.ToLower().Contains("muki")) // Muki
            {
                // NOTE: Muki không có thương hiệu nên ta đăng nhờ thương hiệu FAHASA
                brandId = 2101;
                brand = "FAHASA";
            }
            else if (item.publisher.ToLower().Contains("đinh tị")) // Đinh Tị Books
            {
                brandId = 124098239;
                brand = "Đinh Tị Books";
            }
            else if (item.publisher.ToLower().Contains("alpha books")) // Alpha Books
            {
                brandId = 206133;
                brand = "Alpha Books";
            }
            else if (item.publisher.ToLower().Contains("kennen")) // Kennen
            {
                brandId = 127283688;
                brand = "KENNEN";
            }
            else if (item.publisher.ToLower().Contains("quang van")) // Quảng Văn
            {
                brandId = 2113;
                brand = "Quang Van";
            }
            else if (item.publisher.ToLower().Contains("lalala")) // Lalala Baby
            {
                brandId = 123312437;
                brand = "lalala baby";
            }
            else if (string.IsNullOrEmpty(item.publisher.Trim()))
            {
                if (item.brand.ToLower().Contains("lalala")) // Lalala Baby
                {
                    brandId = 123312437;
                    brand = "lalala baby";
                }
            }

            attribute.brand_id = brandId.ToString();
            attribute.brand = brand;
        }

        // Nếu up sản phẩm thành công, ta lấy sản phẩm và insert vào db, mapping luôn
        // Item chỉ có 1 model
        public static async Task LazadaInsertNewItem_OneModelAndMapping(long ItemId, int productId,
            MySqlConnection conn)
        {
            LazadaProduct pro = await LazadaProductAPI.GetProductItem(ItemId);
            if (pro == null)
            {
                return;
            }

            CommonItem item = new CommonItem(pro);
            long itemIdInserted = await LazadaMySql.InserttbLazadaItemAsync(item, conn);

            // Vì chỉ có 1 model
            long modelIdInsert = await LazadaMySql.InserttbLazadaModelAsync(itemIdInserted, item.models[0], conn);

            // Ta mapping
            await LazadaMySql.LazadaInsertNewMappingOneOfModelAsync(modelIdInsert, productId, 1, conn);
        }

        // Tạo sản phẩm là sách, thực tế đều yêu cầu trường thông tin giống nhau
        public static async Task<MySqlResultState> LazadaCreateProduct_Book_Core(
            Product product,
            string name,
            Boolean isNeedUploadImage,
            long categoryId,
            MySqlConnection conn)
        {
            MySqlResultState result = CommonProductSupporting.CheckValidBookProductInfo(product);
            if (result.State != EMySqlResultState.OK)
            {
                return result;
            }

            await LazadaUploadImage(isNeedUploadImage, product, conn, result);
            if (result.State != EMySqlResultState.OK)
            {
                return result;
            }

            // Tạo đối tượng tham số của request
            LazadaCreateProductRequest request = new LazadaCreateProductRequest();
            request.Request = new LazadaCreateProductRequest_Core();
            request.Request.Product = new LazadaProductRequest();
            LazadaProductRequest lazadaPro = request.Request.Product;
            lazadaPro.PrimaryCategory = categoryId.ToString();

            //--------- "is_mandatory": 1------------
            lazadaPro.Skus = new LazadaSkusRequest();
            lazadaPro.Skus.Sku = new List<LazadaSkuRequest>();
            LazadaSkuRequest lazadaSku = new LazadaSkuRequest();
            lazadaPro.Skus.Sku.Add(lazadaSku);
            // isbn => chuyển sang attributes
            //lazadaSku.isbn_issn = !string.IsNullOrEmpty(product.barcode) ? product.barcode : product.code;

            // giá bìa và giá bán
            lazadaSku.price = product.bookCoverPrice.ToString();
            int special_price = await CommonProductSupporting.CaculateSpecial_Price(product, Common.eLazada, conn);
            lazadaSku.special_price = special_price.ToString();
            // SellerSku
            lazadaSku.SellerSku = TikiConstValues.GenerateRandomSKUString();
            // package_weight - kg
            // Khối lượng kiện hàng nên nằm trong khoảng 0.001 và 300.0
            lazadaSku.package_weight = ((product.productWeight + 100) / 1000.0).ToString("F1");
            // package_width - cm
            lazadaSku.package_width = (product.productWide / 10 + 1).ToString();
            // package_length - cm
            lazadaSku.package_length = (product.productLong / 10 + 1).ToString();
            // package_height - cm
            lazadaSku.package_height = (product.productHigh / 10 + 1).ToString();

            // quantity
            lazadaSku.quantity = product.quantity.ToString();

            //--------- "is_key_prop": 1------------
            lazadaPro.Attributes =
                new AttributesRequest();
            // "language": "Vietnamese,English",
            lazadaPro.Attributes.language = LazadaGetLanguage(product.language);
            // "author": "Marta Altés",
            lazadaPro.Attributes.author = product.author;

            // brand
            LazadaGetBrandForAttributesRequest(lazadaPro.Attributes, product);

            // description
            lazadaPro.Attributes.description = Common.CleanHtml_ByOne(product.detail);

            // number_of_pages
            lazadaPro.Attributes.number_of_pages = product.pageNumber.ToString();

            // version
            lazadaPro.Attributes.version = "Đầy đủ";

            // name
            if (string.IsNullOrEmpty(name))
            {
                lazadaPro.Attributes.name = Product.GenerateName(product);
            }
            else
            {
                lazadaPro.Attributes.name = name;
            }

            // isbn
            lazadaPro.Attributes.isbn_issn = !string.IsNullOrEmpty(product.barcode) ? product.barcode : product.code;

            // Ảnh của sản phẩm, không cần ảnh của sku vì chỉ có 1 sku
            List<string> images = await LazadaMySql.GetUploadedImageOfProductOnLazadaAsync(0, product.id, 0, conn);
            LazadaProductImageRequest lazadaProductImageRequest = new LazadaProductImageRequest();
            lazadaProductImageRequest.Image = images;
            lazadaPro.Images = lazadaProductImageRequest;

            LazadaCreateProductResponseBody response =
                await LazadaProductAPI.LazadaCreateProductAsync(request);

            if (response == null ||
                response.code != "0")
            {
                result.State = EMySqlResultState.ERROR;
                if (response != null)
                {
                    result.Message = response.message;
                    if (response.detail != null && response.detail.Count > 0)
                    {
                        foreach (var de in response.detail)
                        {
                            result.Message = result.Message + ", " + de.message;
                        }
                    }
                }
            }
            else
            {
                // Nếu up sản phẩm thành công, ta lấy sản phẩm và cập nhật mapping luôn
                // Lấy item id sản phẩm mới đăng thành công
                //objResponse.response.item_id;
                Thread.Sleep(5000);
                await LazadaInsertNewItem_OneModelAndMapping(response.data.item_id, product.id, conn);
            }

            return result;
        }

        // Tạo sản phẩm là đồ chơi học tập và giáo dục: 10333
        // Learning and educational toys
        public static async Task<MySqlResultState> LazadaCreateProduct_LearningAndEducationalToys(
            Product product,
            string name,
            Boolean isNeedUploadImage,
            long categoryId,
            MySqlConnection conn)
        {
            MySqlResultState result = LazadaCheckValidProductInfo(product);
            if (result.State != EMySqlResultState.OK)
            {
                return result;
            }

            await LazadaUploadImage(isNeedUploadImage, product, conn, result);
            if (result.State != EMySqlResultState.OK)
            {
                return result;
            }

            // Tạo đối tượng tham số của request
            LazadaCreateProductRequest request = new LazadaCreateProductRequest();
            request.Request = new LazadaCreateProductRequest_Core();
            request.Request.Product = new LazadaProductRequest();
            LazadaProductRequest lazadaPro = request.Request.Product;
            lazadaPro.PrimaryCategory = categoryId.ToString();

            lazadaPro.Attributes =
                new AttributesRequest();
            // name
            if (string.IsNullOrEmpty(name))
            {
                lazadaPro.Attributes.name = Product.GenerateName(product);
            }
            else
            {
                lazadaPro.Attributes.name = name;
            }

            // description
            lazadaPro.Attributes.description = Common.CleanHtml_ByOne(product.detail);

            //--------- "is_mandatory": 1------------
            lazadaPro.Skus = new LazadaSkusRequest();
            lazadaPro.Skus.Sku = new List<LazadaSkuRequest>();
            LazadaSkuRequest lazadaSku = new LazadaSkuRequest();
            lazadaPro.Skus.Sku.Add(lazadaSku);

            // giá bìa và giá bán
            lazadaSku.price = product.bookCoverPrice.ToString();
            int special_price = await CommonProductSupporting.CaculateSpecial_Price(product, Common.eLazada, conn);
            lazadaSku.special_price = special_price.ToString();

            // SellerSku
            lazadaSku.SellerSku = TikiConstValues.GenerateRandomSKUString();

            // package_weight - kg
            // Khối lượng kiện hàng nên nằm trong khoảng 0.001 và 300.0
            lazadaSku.package_weight = ((product.productWeight + 100) / 1000.0).ToString("F1");

            // package_width - cm
            lazadaSku.package_width = (product.productWide / 10 + 1).ToString();

            // package_length - cm
            lazadaSku.package_length = (product.productLong / 10 + 1).ToString();

            // package_height - cm
            lazadaSku.package_height = (product.productHigh / 10 + 1).ToString();

            // quantity
            lazadaSku.quantity = product.quantity.ToString();

            // brand
            LazadaGetBrandForAttributesRequest(lazadaPro.Attributes, product);

            //--------- "is_key_prop": 1------------
            //lazadaPro.Attributes.recommended_age = "Trẻ 0 – 4 tuổi";

            //// Cần pin hay không
            //lazadaPro.Attributes.battery_required = "KHÔNG";

            // Ảnh của sản phẩm, không cần ảnh của sku vì chỉ có 1 sku
            List<string> images = await LazadaMySql.GetUploadedImageOfProductOnLazadaAsync(0, product.id, 0, conn);
            LazadaProductImageRequest lazadaProductImageRequest = new LazadaProductImageRequest();
            lazadaProductImageRequest.Image = images;
            lazadaPro.Images = lazadaProductImageRequest;

            LazadaCreateProductResponseBody response =
                await LazadaProductAPI.LazadaCreateProductAsync(request);

            if (response == null ||
                response.code != "0")
            {
                result.State = EMySqlResultState.ERROR;
                if (response != null)
                {
                    result.Message = response.message;
                }
            }
            else
            {
                // Nếu up sản phẩm thành công, ta lấy sản phẩm và cập nhật mapping luôn
                // Lấy item id sản phẩm mới đăng thành công
                //objResponse.response.item_id;
                Thread.Sleep(5000);
                await LazadaInsertNewItem_OneModelAndMapping(response.data.item_id, product.id, conn);
            }

            return result;
        }

        //public async Task<MySqlResultState> CreateLazadaProduct_8666_SachTruyenNganChoBe(
        //    Product product,
        //    string name,
        //    Boolean isNeedUploadImage,
        //    MySqlConnection conn)
        //{
        //    MySqlResultState result = await LazadaCreateProduct_Book_Core(product, name, isNeedUploadImage, 8666, conn);
        //    return result;
        //}

        //public async Task<MySqlResultState> CreateLazadaProduct_8464_SachThuongThucDoiSong(
        //    Product product,
        //    string name,
        //    Boolean isNeedUploadImage,
        //    MySqlConnection conn)
        //{
        //    MySqlResultState result = await LazadaCreateProduct_Book_Core(product, name, isNeedUploadImage, 8464, conn);
        //    return result;
        //}

        public static async Task<MySqlResultState> LazadaCreateProductFromProductIdInWarehouse(
            int productId,
            string name,
            Boolean isNeedUploadImage,
            MySqlConnection conn)
        {
            MyLogger.GetInstance().Info("LazadaCreateProductFromProductIdInWarehouse Call productId: " + productId + ", name: " + name + ", isNeedUploadImage: " + isNeedUploadImage.ToString());
            // Lấy sản phẩm trong kho
            Product product = await ProductMySql.GetProductFromIdAsync(productId, conn);

            // Lấy category
            Category cate = await CategoryMySql.GetCategoryAsync(product.categoryId, conn);
            MySqlResultState result = new MySqlResultState();
            if (cate != null)
            {
                if (cate.lazadaCategoryId < 0)
                {
                    result.State = EMySqlResultState.INVALID;
                    result.Message = cate.id.ToString() + ": " + cate.name + " chưa có lazada category tương ứng";
                }
                else if (
                    // -- Sách và truyện hư cấu "category_id": 8654--
                    cate.lazadaCategoryId == 8655 ||// Văn học
                    cate.lazadaCategoryId == 8657 ||// Sách cho thanh thiếu niên
                    cate.lazadaCategoryId == 8661 ||// Truyện tranh
                    cate.lazadaCategoryId == 8666 ||// Tập truyện ngắn cho bé
                    cate.lazadaCategoryId == 8670 ||// Truyện tranh
                    cate.lazadaCategoryId == 8694 ||// Tiểu thuyết
                    cate.lazadaCategoryId == 8698 ||// Khoa học viễn tưởng

                    // -- Sách và truyện phi hư cấu "category_id": 8462
                    cate.lazadaCategoryId == 8464 ||// Sách thường thức đời sống
                    cate.lazadaCategoryId == 8466 ||// Tự truyện - Hồi ký
                    cate.lazadaCategoryId == 8467 ||// Sách kinh doanh & tài chính
                    cate.lazadaCategoryId == 8472 ||// Sách nuôi dạy con & xây dựng mối quan hệ
                    cate.lazadaCategoryId == 8476 ||// Sách tôn giáo
                    cate.lazadaCategoryId == 8491 ||// Nghệ thuật ẩm thực
                    cate.lazadaCategoryId == 8499 ||// Sách thể dục & thể thao
                    cate.lazadaCategoryId == 8500 ||// Sách lịch sử
                    cate.lazadaCategoryId == 8508 ||// Sách nghệ thuật - Giải trí
                    cate.lazadaCategoryId == 8513 ||// Sách nghệ thuật & thủ công
                    cate.lazadaCategoryId == 8519 ||// Xã hội học
                    cate.lazadaCategoryId == 8521 ||// Sách chuyên ngành
                    cate.lazadaCategoryId == 8531 ||// Sách khoa học
                    cate.lazadaCategoryId == 8534 ||// Sách phát triển bản thân
                    cate.lazadaCategoryId == 8536 // Sách du lịch
                    )
                {
                    result = await LazadaCreateProduct_Book_Core(product, name, isNeedUploadImage, cate.lazadaCategoryId, conn);
                }
                else if (cate.lazadaCategoryId == 10333)// Đồ Chơi Học Tập Và Giáo Dục)
                {
                    result = await LazadaCreateProduct_LearningAndEducationalToys(product, name, isNeedUploadImage, cate.lazadaCategoryId, conn);
                }
                else
                {
                    result.State = EMySqlResultState.INVALID;
                    result.Message = "Chưa hỗ trợ đăng thể loại Lazada này.";
                }
            }
            else
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "Chưa có thông tin thể loại.";
            }
            return result;
        }

        // Từ tên sàn gốc, sàn đích và tên category gốc, ta tìm ra category id của sản phẩm ở sàn mới
        public static long LazadaConvertCategory(string from, string to, string originalCategory)
        {
            long categoryId = 0;
            if (from == Common.eShopee && to == Common.eLazada)
            {
                if (originalCategory.Contains("Sách Thiếu Nhi"))
                {
                    categoryId = 8666; // Tập truyện ngắn cho bé
                }
                else if (originalCategory.Contains("Sách Vải") ||
                    originalCategory.Contains("Đồ chơi thủ công mỹ thuật") ||
                    originalCategory.Contains("Đồ chơi nhập vai") ||
                    originalCategory.Contains("Đồ chơi lắp ráp"))
                {
                    categoryId = 10333;// Đồ Chơi Học Tập Và Giáo Dục
                }
            }
            return categoryId;
        }

        public static async Task<MySqlResultState> LazadaCreateItemFromOther(ItemForCreate item)
        {
            MySqlResultState result = new MySqlResultState();

            // Tải media xuống local
            Boolean isOk = await ProductController.DownloadMediaOfItemForCreate(item);
            MyLogger.GetInstance().Info("Item with media after download:");
            MyLogger.GetInstance().Info(JsonConvert.SerializeObject(item, Common.jsonSerializersettings));
            if (!isOk)
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = "Tải media xuống local lỗi";
                return result;
            }
            Thread.Sleep(1000);


            // Tải media lên server lazada từ local
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                result = await LazadaProductAPI.LazadaUploadMediaFromItemForCreate(item, conn);
                MyLogger.GetInstance().Info("Item Lazada upload media:");
                MyLogger.GetInstance().Info(JsonConvert.SerializeObject(item, Common.jsonSerializersettings));
                if (result.State != EMySqlResultState.OK)
                {
                    return result;
                }
            }
            Thread.Sleep(1000);

            // Tạo sản phẩm trên sàn
            item.categoryId = LazadaConvertCategory(item.siteNameCopyFrom, item.siteName, item.category);
            if (item.categoryId == 8666) // Tập truyện ngắn cho bé
            {
                result = await LazadaCreateProductFromOther_Book(item);
            }
            else if (item.categoryId == 10333)// Đồ Chơi Học Tập Và Giáo Dục
            {
                result = await LazadaCreateProductFromOther_LearningAndEducationalToys(item);
            }
            if (result.State != EMySqlResultState.OK)
            {
                MyLogger.GetInstance().Info(JsonConvert.SerializeObject(result, Common.jsonSerializersettings));
            }

            return result;
        }

        public static async Task<string> LazadaGenerateDescription_FromItemForCreate(ItemForCreate item)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var des in item.descriptions)
            {
                if (des.isText)
                {
                    if (!string.IsNullOrEmpty(des.content))
                    {
                        builder.Append($"<p>{des.content}</p>");
                    }
                }
                else // là ảnh
                {
                    builder.Append($"<img src=\"{des.srcImageTo}\"/>");
                }
            }
            return builder.ToString();
        }


        // Tạo sản phẩm trên sàn từ đối tượng lấy từ web
        public static async Task<MySqlResultState> LazadaCreateProductFromOther_Book(
            ItemForCreate item)
        {
            MySqlResultState result = new MySqlResultState();

            // Tạo đối tượng tham số của request
            LazadaCreateProductRequest request = new LazadaCreateProductRequest();
            request.Request = new LazadaCreateProductRequest_Core();
            request.Request.Product = new LazadaProductRequest();
            LazadaProductRequest lazadaPro = request.Request.Product;

            //long categoryId = ConvertCategory(item.siteNameCopyFrom, item.siteName, item.category);
            if (item.categoryId == 0)
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = "Không lấy được thể loại sản phẩm";
                return result;
            }

            lazadaPro.PrimaryCategory = item.categoryId.ToString();

            // Thêm thông tin variation nếu phân loại có nhiều hơn 1
            if (item.models.Count > 1)
            {
                LazadaProductSkuVariationRequest variation = new LazadaProductSkuVariationRequest();
                variation.Variation1 = new LazadaProductSkuVariationCoreRequest();
                variation.Variation1.customize = true;
                variation.Variation1.hasImage = true;
                variation.Variation1.name = item.variationName;
                variation.Variation1.label = item.variationName;
                variation.Variation1.options = new LazadaProductSkuVariationOptionRequest();
                variation.Variation1.options.option = new List<string>();
                foreach (var model in item.models)
                {
                    variation.Variation1.options.option.Add(model.name);
                }

                lazadaPro.variation = variation;
            }

            //--------- "is_mandatory": 1------------
            lazadaPro.Skus = new LazadaSkusRequest();
            lazadaPro.Skus.Sku = new List<LazadaSkuRequest>();
            foreach (var model in item.models)
            {
                LazadaSkuRequest lazadaSku = new LazadaSkuRequest();
                lazadaPro.Skus.Sku.Add(lazadaSku);

                // giá bìa và giá bán
                lazadaSku.price = model.bookCoverPrice.ToString();
                //lazadaSku.special_price được set khi mapping

                // SellerSku
                lazadaSku.SellerSku = TikiConstValues.GenerateRandomSKUString();
                // package_weight - kg
                // Khối lượng kiện hàng nên nằm trong khoảng 0.001 và 300.0
                lazadaSku.package_weight = (item.weight / 1000.0).ToString("F1");
                // package_width - cm
                lazadaSku.package_width = (item.sizeWidth / 10).ToString();
                // package_length - cm
                lazadaSku.package_length = (item.sizeLength / 10).ToString();
                // package_height - cm
                lazadaSku.package_height = (item.sizeHeight / 10).ToString();

                // quantity
                lazadaSku.quantity = "0";

                // image
                if (model.srcImagesTo.Count() > 0)
                {
                    LazadaProductImageRequest modelImages = new LazadaProductImageRequest();
                    modelImages.Image = model.srcImagesTo;

                    if (modelImages.Image.Count > LazadaProductAPI.maximumOfImages)
                    {
                        modelImages.Image.RemoveRange(LazadaProductAPI.maximumOfImages,
                            modelImages.Image.Count - LazadaProductAPI.maximumOfImages);
                    }
                    lazadaSku.Images = modelImages;
                }
                else
                {
                    LazadaProductImageRequest modelImages = new LazadaProductImageRequest();
                    modelImages.Image = new List<string>();
                    // Nếu model không có ảnh, ta thêm 1 ảnh mặc định và sửa lại thủ công sau
                    modelImages.Image.Add("https://sg-test-11.slatic.net/p/fa68ed4f6eca23a9134aa2cceb926820.png");
                    lazadaSku.Images = modelImages;
                }

                if (lazadaPro.variation != null)
                {
                    //lazadaSku.valueOfVariation = model.name;
                    lazadaSku.saleProp = new LazadaProductSkuSalePro();
                    lazadaSku.saleProp.valueOfVariation = model.name;
                }
            }

            //--------- "is_key_prop": 1------------
            lazadaPro.Attributes =
                new AttributesRequest();
            // "language": "Vietnamese,English",
            lazadaPro.Attributes.language = LazadaGetLanguage(item.language);
            // "author": "Marta Altés",
            lazadaPro.Attributes.author = item.brand;

            // brand
            LazadaGetBrandForAttributesRequest_FromItem(
                lazadaPro.Attributes, item);

            // description
            lazadaPro.Attributes.description = await LazadaGenerateDescription_FromItemForCreate(item);

            // number_of_pages
            lazadaPro.Attributes.number_of_pages = item.pageNumber.ToString();

            // version
            lazadaPro.Attributes.version = "Đầy đủ";

            // name
            lazadaPro.Attributes.name = item.name;

            // isbn
            lazadaPro.Attributes.isbn_issn = item.isbn;

            // Video của sản phẩm nếu có
            if (!string.IsNullOrEmpty(item.pathVideo))
            {
                lazadaPro.Attributes.video = item.idVideo;
            }

            // Ảnh của sản phẩm
            List<string> images = new List<string>();
            foreach (var src in item.srcImagesTo)
            {
                if (images.Count == LazadaProductAPI.maximumOfImages)
                {
                    break;
                }
                images.Add(src);
            }
            LazadaProductImageRequest lazadaProductImageRequest = new LazadaProductImageRequest();
            lazadaProductImageRequest.Image = images;
            lazadaPro.Images = lazadaProductImageRequest;

            LazadaCreateProductResponseBody response =
                await LazadaProductAPI.LazadaCreateProductAsync(request);

            if (response == null ||
                response.code != "0")
            {
                result.State = EMySqlResultState.ERROR;
                if (response != null)
                {
                    // Nếu lỗi do video id không hợp lệ, ta thử lại 1 lần nữa. Video sẽ được up lại thủ công
                    if (response.detail != null && response.detail.Count > 0 &&
                        response.detail[0].message.Contains("BIZ_CHECK_NEW_VIDEO_ID_INVALID"))
                    {
                        Thread.Sleep(5000);
                        // Bỏ thuộc tính video của sản phẩm
                        lazadaPro.Attributes.video = string.Empty;

                        response =
                        await LazadaProductAPI.LazadaCreateProductAsync(request);
                    }

                    if (response != null && response.code != "0")
                    {
                        result.Message = response.message;
                        if (response.detail != null && response.detail.Count > 0)
                        {
                            foreach (var de in response.detail)
                            {
                                result.Message = result.Message + ", " + de.message;
                            }
                        }
                    }
                }
            }
            return result;
        }

        // LazadaCreateProduct_LearningAndEducationalToys
        public static async Task<MySqlResultState> LazadaCreateProductFromOther_LearningAndEducationalToys(
           ItemForCreate item)
        {
            MySqlResultState result = new MySqlResultState();

            // Tạo đối tượng tham số của request
            LazadaCreateProductRequest request = new LazadaCreateProductRequest();
            request.Request = new LazadaCreateProductRequest_Core();
            request.Request.Product = new LazadaProductRequest();
            LazadaProductRequest lazadaPro = request.Request.Product;

            if (item.categoryId == 0)
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = "Không lấy được thể loại sản phẩm";
                return result;
            }

            lazadaPro.PrimaryCategory = item.categoryId.ToString();

            // Thêm thông tin variation nếu phân loại có nhiều hơn 1
            if (item.models.Count > 1)
            {
                LazadaProductSkuVariationRequest variation = new LazadaProductSkuVariationRequest();
                variation.Variation1 = new LazadaProductSkuVariationCoreRequest();
                variation.Variation1.customize = true;
                variation.Variation1.hasImage = true;
                variation.Variation1.name = item.variationName;
                variation.Variation1.label = item.variationName;
                variation.Variation1.options = new LazadaProductSkuVariationOptionRequest();
                variation.Variation1.options.option = new List<string>();
                foreach (var model in item.models)
                {
                    variation.Variation1.options.option.Add(model.name);
                }

                lazadaPro.variation = variation;
            }

            //--------- "is_mandatory": 1------------
            lazadaPro.Skus = new LazadaSkusRequest();
            lazadaPro.Skus.Sku = new List<LazadaSkuRequest>();
            foreach (var model in item.models)
            {
                LazadaSkuRequest lazadaSku = new LazadaSkuRequest();
                lazadaPro.Skus.Sku.Add(lazadaSku);

                // giá bìa và giá bán
                lazadaSku.price = model.bookCoverPrice.ToString();
                //lazadaSku.special_price được set khi mapping

                // SellerSku
                lazadaSku.SellerSku = TikiConstValues.GenerateRandomSKUString();
                // package_weight - kg
                // Khối lượng kiện hàng nên nằm trong khoảng 0.001 và 300.0
                lazadaSku.package_weight = (item.weight / 1000.0).ToString("F1");
                // package_width - cm
                lazadaSku.package_width = (item.sizeWidth / 10).ToString();
                // package_length - cm
                lazadaSku.package_length = (item.sizeLength / 10).ToString();
                // package_height - cm
                lazadaSku.package_height = (item.sizeHeight / 10).ToString();

                // quantity
                lazadaSku.quantity = "0";

                // image
                if (model.srcImagesTo.Count() > 0)
                {
                    LazadaProductImageRequest modelImages = new LazadaProductImageRequest();
                    modelImages.Image = model.srcImagesTo;

                    if (modelImages.Image.Count > LazadaProductAPI.maximumOfImages)
                    {
                        modelImages.Image.RemoveRange(LazadaProductAPI.maximumOfImages,
                            modelImages.Image.Count - LazadaProductAPI.maximumOfImages);
                    }
                    lazadaSku.Images = modelImages;
                }
                else
                {
                    LazadaProductImageRequest modelImages = new LazadaProductImageRequest();
                    modelImages.Image = new List<string>();
                    // Nếu model không có ảnh, ta thêm 1 ảnh mặc định và sửa lại thủ công sau
                    modelImages.Image.Add("https://sg-test-11.slatic.net/p/fa68ed4f6eca23a9134aa2cceb926820.png");
                    lazadaSku.Images = modelImages;
                }

                if (lazadaPro.variation != null)
                {
                    //lazadaSku.valueOfVariation = model.name;
                    lazadaSku.saleProp = new LazadaProductSkuSalePro();
                    lazadaSku.saleProp.valueOfVariation = model.name;
                }
            }

            //--------- "is_key_prop": 1------------
            lazadaPro.Attributes =
                new AttributesRequest();

            // brand
            LazadaGetBrandForAttributesRequest_FromItem(
                lazadaPro.Attributes, item);

            // description
            lazadaPro.Attributes.description = await LazadaGenerateDescription_FromItemForCreate(item);

            // name
            lazadaPro.Attributes.name = item.name;

            // Video của sản phẩm nếu có
            if (!string.IsNullOrEmpty(item.pathVideo))
            {
                lazadaPro.Attributes.video = item.idVideo;
            }

            // Ảnh của sản phẩm
            List<string> images = new List<string>();
            foreach (var src in item.srcImagesTo)
            {
                if (images.Count == LazadaProductAPI.maximumOfImages)
                {
                    break;
                }
                images.Add(src);
            }
            LazadaProductImageRequest lazadaProductImageRequest = new LazadaProductImageRequest();
            lazadaProductImageRequest.Image = images;
            lazadaPro.Images = lazadaProductImageRequest;

            LazadaCreateProductResponseBody response =
                await LazadaProductAPI.LazadaCreateProductAsync(request);

            if (response == null ||
                response.code != "0")
            {
                result.State = EMySqlResultState.ERROR;
                if (response != null)
                {
                    // Nếu lỗi do video id không hợp lệ, ta thử lại 1 lần nữa. Video sẽ được up lại thủ công
                    if (response.detail != null && response.detail.Count > 0 &&
                        response.detail[0].message.Contains("BIZ_CHECK_NEW_VIDEO_ID_INVALID"))
                    {
                        Thread.Sleep(5000);
                        // Bỏ thuộc tính video của sản phẩm
                        lazadaPro.Attributes.video = string.Empty;

                        response =
                        await LazadaProductAPI.LazadaCreateProductAsync(request);
                    }

                    if (response != null && response.code != "0")
                    {
                        result.Message = response.message;
                        if (response.detail != null && response.detail.Count > 0)
                        {
                            foreach (var de in response.detail)
                            {
                                result.Message = result.Message + ", " + de.message;
                            }
                        }
                    }
                }
            }
            return result;
        }


        public static async Task<List<Product>> LazadaSearchDontSellFullComboAndSigleOnECommerce(
            List<Combo> lsCombo,
            MySqlConnection conn)
        {
            List<Combo> lsComboDontSellFull = new List<Combo>();
            Boolean isComboDontSellFull = false;
            foreach (var combo in lsCombo)
            {
                isComboDontSellFull = false;
                List<CommonItem> lsCommonItem = await TikiMySql.TikiGetListMappingOfComboAsync(combo.id, conn);
                foreach (var commonItem in lsCommonItem)
                {
                    List<Mapping> mapping = commonItem.models[0].mapping;
                    if (mapping.Count == combo.products.Count) // Là sản phẩm combo
                    {
                        Boolean isMapped = false;
                        for (int i = 0; i < mapping.Count; i++)
                        {
                            isMapped = false;
                            for (int j = 0; j < combo.products.Count; j++)
                            {
                                if (mapping[i].product.id == combo.products[j].id)
                                {
                                    isMapped = true;
                                    break;
                                }
                            }
                            if (!isMapped)
                            {
                                break;
                            }
                        }

                        if (isMapped)// Là sản phẩm combo
                        {
                            // Ta lấy danh sách commonItem có cùng cha, và bán sản phẩm lẻ
                            List<CommonItem> lsCI = new List<CommonItem>();
                            foreach (var cI in lsCommonItem)
                            {
                                if (cI.tikiSuperId == commonItem.tikiSuperId
                                    && cI.models[0].mapping.Count == 1)
                                {
                                    lsCI.Add(cI);
                                }
                            }

                            // Kiểm tra lsCI mỗi phần tử đã mapping với một sản phẩm trong combo
                            //List<int> lsProductId = new List<int>();
                            Boolean isExist = false;
                            foreach (var pro in combo.products)
                            {
                                isExist = false;
                                foreach (var cI in lsCI)
                                {
                                    if (pro.id == cI.models[0].mapping[0].product.id)
                                    {
                                        isExist = true;
                                        break;
                                    }
                                }
                                if (!isExist)
                                {
                                    break;
                                }
                            }

                            if (isExist)
                            {
                                isComboDontSellFull = true;
                            }
                        }
                    }

                    if (isComboDontSellFull)
                    {
                        break;
                    }
                }
                if (!isComboDontSellFull)
                {
                    lsComboDontSellFull.Add(combo);
                }
            }

            List<Product> lsProduct = new List<Product>();
            foreach (var combo in lsComboDontSellFull)
            {
                lsProduct.AddRange(combo.products);
            }

            return lsProduct;
        }
    }
}
