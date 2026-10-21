using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.ProductModel;
using MVCPlayWithMe.Models.SanPhamModel;
using MVCPlayWithMe.OpenPlatform;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Category;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Product;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MVCPlayWithMe.General.Common;

namespace MVCPlayWithMe.Controllers.ProductSupporting
{
    public class TikiProductSupporting
    {
        public static async Task<List<CommonItem>> TikiGetListNeedUpdateQuantityAndUpdateAsync(MySqlConnection conn)
        {
            // Danh sách sản phẩm Tiki
            List<CommonItem> listCommonItem = await TikiMySql.TikiGetListNeedUpdateQuantityConnectOutAsync(conn);
            //TikiGetStatusImageSrcQuantitySellable(listCommonItem);
            foreach (var commonItem in listCommonItem)
            {
                await TikiUpdateQuantityOfOneItemAsync(commonItem/*, conn*/);
            }
            return listCommonItem;
        }

        // Cập nhật theo lô
        public static async Task<List<CommonItem>> TikiGetListNeedUpdateQuantityAndUpdate_BatchAsync(MySqlConnection conn)
        {
            // Danh sách sản phẩm Tiki
            List<CommonItem> listCommonItem = await TikiMySql.TikiGetListNeedUpdateQuantityConnectOutAsync(conn);
            await TikiUpdateQuantity_BatchAsync(listCommonItem);
            return listCommonItem;
        }

        // Với Tiki TikiUpdateQuantityOfOneItemModel, và TikiUpdateQuantityOfOneItem giống nhau, khác tham số truyền vào
        // TikiUpdateQuantityOfOneItem có thể check item active
        public static async Task TikiUpdateQuantityOfOneItemAsync(CommonItem commonItem/*, MySqlConnection conn*/)
        {
            if (!commonItem.bActive)
            {
                commonItem.result = null;
                return;
            }

            MySqlResultState result = new MySqlResultState();
            //int quantity = sqler.TikiGetQuantityOfOneItemModelConnectOut((int)commonItem.itemId, conn);

            int quantity = commonItem.models[0].GetQuantityFromListMapping();

            await TikiUpdateStock.TikiProductUpdateQuantityAsync((int)commonItem.itemId, quantity, result);

            // Comment vì mỗi ngày check lại toàn bộ deal đang chạy
            //if (quantity <= Common.minQuantityOfDealTiki)
            //{
            //    TikiDealDiscountMySql.UpdateIsActiveCloseFromItemId((int)commonItem.itemId, conn);
            //}

            commonItem.result = result;
        }

        public static async Task TikiUpdateQuantity_BatchAsync(List<CommonItem> commonItems/*, MySqlConnection conn*/)
        {
            List<int> ids = new List<int>();
            List<int> quantities = new List<int>();
            foreach(var it in commonItems)
            {
                ids.Add((int)it.itemId);
                quantities.Add(it.models[0].GetQuantityFromListMapping());
            }

            List<TikiUpdatePriceQuantityActiveResultItem> responses = await TikiUpdateStock.TikiProductUpdateQuantity_BatchAsync(ids, quantities);

            foreach(var it in commonItems)
            {
                MySqlResultState result = null;
                foreach( var re in responses)
                {
                    if ( re!= null && (int)it.itemId == re.product_id)
                    {
                        result = new MySqlResultState();
                        if (!re.is_success)
                        {
                            result.State = EMySqlResultState.ERROR;
                            result.Message = "code: " + re.code + ", message: " + re.message;
                        }
                        break;
                    }
                }
                it.result = result;
            }
        }

        // Cập nhật theo lô
        public static async Task<List<CommonItem>> TikiGetListNeedUpdatePriceAndUpdate_BatchAsync(MySqlConnection conn)
        {
            // Danh sách sản phẩm Tiki
            List<CommonItem> listCommonItem = await TikiMySql.TikiGetListNeedUpdatePriceConnectOutAsync(conn);
            await TikiUpdatePrice_BatchAsync(listCommonItem, conn);
            return listCommonItem;
        }

        public static async Task<List<CommonItem>> TikiUpdatePriceAll_BatchAsync(MySqlConnection conn)
        {
            // Danh sách sản phẩm Tiki
            List<CommonItem> listCommonItem = await TikiMySql.TikiGetListUpdatePriceAllConnectOutAsync(conn);
            await TikiUpdatePrice_BatchAsync(listCommonItem, conn);
            return listCommonItem;
        }

        public static async Task TikiUpdatePrice_BatchAsync(List<CommonItem> commonItems, MySqlConnection conn)
        {
            TaxAndFee taxAndFee = await TikiDealDiscountMySql.GetTaxAndFeeAsync(Common.eTiki, conn);

            // Lấy danh sách nhà phát hành, từ đó lấy được discount chung
            List<Publisher> listPublisher = await PublisherMySql.GetListPublisherConnectOutAsync(conn);

            List<int> ids = new List<int>();
            List<int> prices = new List<int>();
            int price = 0;
            foreach (var it in commonItems)
            {
                ids.Add((int)it.itemId);

                price = CommonOpenPlatform.CaculateSpecialPriceCoreFromCommonModel(it.models[0],
                        listPublisher,
                        taxAndFee);

                if(price <= Common.minPrice)
                {
                    price = it.models[0].GetBookCoverPrice();
                }    
                prices.Add(price);
            }

            List<TikiUpdatePriceQuantityActiveResultItem> responses = await TikiUpdateStock.TikiProductUpdatePrice_BatchAsync(ids, prices);

            foreach (var it in commonItems)
            {
                MySqlResultState result = null;
                foreach (var re in responses)
                {
                    if (re != null && (int)it.itemId == re.product_id)
                    {
                        result = new MySqlResultState();
                        if (!re.is_success)
                        {
                            result.State = EMySqlResultState.ERROR;
                            result.Message = "code: " + re.code + ", message: " + re.message;
                        }
                        break;
                    }
                }
                it.result = result;
            }
        }

        // Lấy trạng thái sản phẩm, image đại diện, số lượng trên sàn Tiki.
        // Cập nhật số lượng với sản phẩm active
        public async Task TikiGetStatusImageSrcQuantitySellable(List<CommonItem> tikiList)
        {
            foreach (var item in tikiList)
            {
                TikiProduct pro = null;
                pro = await GetListProductTiki.GetProductFromOneShop((int)item.itemId);
                if (pro == null)
                    continue;

                item.imageSrc = pro.thumbnail;
                item.models[0].imageSrc = pro.thumbnail;

                if (pro.active == 1)
                    item.bActive = true;
                else
                    item.bActive = false;

                item.models[0].quantity_sellable = TikiUpdateStock.GetQuantityFromTikiProduct(pro);
                item.has_model = false;

                //// Lấy tên của super item nếu có
                //if(item.tikiSuperId != 0)
                //{
                //    pro = GetListProductTiki.GetProductFromOneShop(item.tikiSuperId);
                //    item.tikiSuperName = pro.name;
                //}
            }
        }

        public static async Task<MySqlResultState> TikiUpdateQuantityOfOneItemModelAsync(int itemId, MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            int quantity = await ProductMySql.TikiGetQuantityOfOneItemModelConnectOutAsync(itemId, conn);

            await TikiUpdateStock.TikiProductUpdateQuantityAsync(itemId, quantity, result);

            // Comment vì mỗi ngày check lại toàn bộ deal đang chạy
            //if (quantity <= Common.minQuantityOfDealTiki)
            //{
            //    TikiDealDiscountMySql.UpdateIsActiveCloseFromItemId(itemId, conn);
            //}

            return result;
        }

        public static async Task<List<Product>> TikiSearchDontSellFullComboAndSigleOnECommerce(
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

        // Vì tiki hiển thị detail liền tù tì, không có xuống dòng nên cần thêm thẻ <p> nếu chưa có
        public static string TikiGetDescriptsFromDetail(string detail)
        {
            if (string.IsNullOrWhiteSpace(detail))
            {
                return string.Empty;
            }

            // Nếu đã có <p>
            if (detail.Contains("<p>"))
            {
                return Common.ReplaceMoreNewLineCharacterByTwo(detail);
            }

            // Phân tách theo dòng và bọc mỗi dòng không rỗng bằng thẻ <p>
            var lines = detail.Split('\n');
            var result = new List<string>();

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                {
                    result.Add($"<p>{trimmed}</p>");
                }
            }

            return string.Join("\n", result);
        }

        public static void TikiGetCertificateFiles(
            TikiCreatingProduct tikiCreatingProduct,
            Publisher publisher
            )
        {
            CertificateFile certificateFile = new CertificateFile();
            certificateFile.type = "category";
            certificateFile.document_id = 18;

            certificateFile.url = Common.GetSrcCertificateFolderPath(Common.eTiki) + publisher.tikiCertificate;

            tikiCreatingProduct.certificate_files.Add(certificateFile);
        }

        // Từ sản phẩm trong kho, tạo sản phẩm trên sàn Tiki
        public static async Task TikiCreateProductFromProductIdInWarehouse(int id,
            string name,
            MySqlResultState result,
            MySqlConnection conn)
        {
            TikiCreateProductTrackingResponse trackObj = null;
            try
            {
                TikiCreatingProduct tikiCreatingProduct = new TikiCreatingProduct();

                // Lấy sản phẩm trong kho
                Product product = await ProductMySql.GetProductFromIdAsync(id, conn);

                // Từ category id sản phẩm trong kho, lấy category tương ứng trên Tiki
                tikiCreatingProduct.category_id =
                    await ProductMySql.GetTikiCategoryIdFromProductCategoryIdAsync(product.categoryId, conn);

                if (string.IsNullOrEmpty(name))
                {
                    tikiCreatingProduct.name = Product.GenerateName(product);
                }
                else
                {
                    tikiCreatingProduct.name = name;
                }

                tikiCreatingProduct.description = TikiGetDescriptsFromDetail(product.detail);
                // Tính giá bán
                tikiCreatingProduct.market_price = product.bookCoverPrice;

                Publisher publisher = await PublisherMySql.GetPublisherAsync(product.publisherId);

                // Attribute. Ta chỉ cập nhật những thuộc tính bắt buộc phải có và 1 vài thuộc tính khác
                List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category.TikiAttribute> attributes =
                    await TikiMySql.GetTikiAttributesOfCategoryAsync(tikiCreatingProduct.category_id, conn);
                var tikiAttributesGroups = new Dictionary<string, object>();
                foreach (var attr in attributes)
                {
                    if (attr.code == "age_group")
                    {
                        tikiCreatingProduct.attributes.age_group = TikiCreateProduct.TikiGetAgeGroups(product.minAge, product.maxAge);
                    }
                    else if (attr.code == "book_cover")
                    {
                        tikiCreatingProduct.attributes.book_cover = TikiCreateProduct.TikiGetStringCover(product.hardCover);
                    }
                    else if (attr.code == "language_book")
                    {
                        tikiCreatingProduct.attributes.language_book = product.language;
                    }
                    else if (attr.code == "manufacturer")
                    {
                        tikiCreatingProduct.attributes.manufacturer = product.publishingCompany;
                    }
                    else if (attr.code == "product_height")
                    {
                        tikiCreatingProduct.attributes.product_height =
                            TikiCreateProduct.TikiGetStringOneDimension(product.productHigh);
                    }
                    else if (attr.code == "product_length")
                    {
                        tikiCreatingProduct.attributes.product_length =
                            TikiCreateProduct.TikiGetStringOneDimension(product.productLong);
                    }
                    else if (attr.code == "product_width")
                    {
                        tikiCreatingProduct.attributes.product_width =
                            TikiCreateProduct.TikiGetStringOneDimension(product.productWide);
                    }
                    else if (attr.code == "product_weight_kg")
                    {
                        tikiCreatingProduct.attributes.product_weight_kg =
                            TikiCreateProduct.TikiGetStringWeight(product.productWeight);
                    }
                    else if (attr.code == "publisher_vn")
                    {
                        tikiCreatingProduct.attributes.publisher_vn = publisher.tikiAttributeValue;
                    }
                    else if (attr.code == "origin")
                    {
                        tikiCreatingProduct.attributes.origin = publisher.origin;
                    }
                    else if (attr.code == "Organization_address")
                    {
                        tikiCreatingProduct.attributes.Organization_address = publisher.organization_address;
                    }
                    else if (attr.code == "Organization_name")
                    {
                        tikiCreatingProduct.attributes.Organization_name = publisher.organization_name;
                    }
                }

                // number_of_page
                if (product.pageNumber > 0)
                {
                    tikiCreatingProduct.attributes.number_of_page = product.pageNumber.ToString();
                }
                // dimensions
                tikiCreatingProduct.attributes.dimensions =
                TikiCreateProduct.TikiGetStringDimensions(product.productLong, product.productWide, product.productHigh);

                // dịch giả
                if (!string.IsNullOrEmpty(product.translator))
                {
                    tikiCreatingProduct.attributes.dich_gia = product.translator;
                }

                // Tác giả. Thuộc tính này là "input_type": "multiselect"
                if (!string.IsNullOrEmpty(product.author))
                {
                    tikiCreatingProduct.attributes.author = product.author.Split(',')
                                .Select(s => s.Trim()) // Loại bỏ khoảng trắng ở đầu và cuối
                                .ToList();

                    foreach (string aut in tikiCreatingProduct.attributes.author)
                    {
                        MySqlResultState mySqlResult = await TikiCategoryAction.CheckGetAttributeValues(498, aut);
                        if (mySqlResult.State != EMySqlResultState.OK)
                        {
                            result.GetValueFrom(mySqlResult);
                            result.Message = result.Message;
                            result.myJson = trackObj;
                            return;
                        }
                    }
                }
                else
                {
                    result.State = EMySqlResultState.EMPTY;
                    result.Message = "Tác giả đang trống.";
                }

                product.SetSrcImageVideo();
                if (product.imageSrc.Count > 0)
                {
                    tikiCreatingProduct.image = Common.httpsVoiBeNho + product.imageSrc[0];
                    for (int i = 1; i < product.imageSrc.Count; i++)
                    {
                        tikiCreatingProduct.images.Add(Common.httpsVoiBeNho + product.imageSrc[i]);
                    }
                }

                Variant variant = new Variant();
                variant.price = await CommonProductSupporting.CaculateSpecial_Price(product, Common.eTiki, conn); ;
                variant.inventory_type = TikiConstValues.inventory_type;
                variant.seller_warehouse = TikiConstValues.intIdKho28Ngo3TTDL.ToString();
                variant.sku = TikiConstValues.GenerateRandomSKUString();
                List<TrackOriginalSku> trackOriginalSkus = new List<TrackOriginalSku>();
                trackOriginalSkus.Add(new TrackOriginalSku
                {
                    ECommmerce = EECommerceType.TIKI,
                    OriginalSku = variant.sku,
                    ProductId = product.id,
                    Quantity = 1,
                    IsVariant = 0
                });

                variant.min_code = Common.GenerateRandomMincodeLong();

                WarehouseStock warehouseStock = new WarehouseStock();
                warehouseStock.warehouseId = TikiConstValues.intIdKho28Ngo3TTDL;
                warehouseStock.qtyAvailable = product.quantity;

                variant.warehouse_stocks.Add(warehouseStock);
                tikiCreatingProduct.variants.Add(variant);

                // certificate_files => Bỏ vì chỉ cần cập nhật lần đầu thủ công
                TikiGetCertificateFiles(tikiCreatingProduct, publisher);

                // meta_data
                MetaData metaData = new MetaData();
                metaData.is_auto_turn_on = true;
                tikiCreatingProduct.meta_data = metaData;

                // Tạo sản phẩm
                trackObj = await TikiCreateProduct.CreateProduct(tikiCreatingProduct, result);

                if (result.State == EMySqlResultState.OK)
                {
                    await TikiWaitingCreatingProduct(trackObj, result, trackOriginalSkus, conn);
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
                trackObj = null;
            }

            result.myJson = trackObj;
        }

        // Từ combo, tạo sản phẩm với nhiều variant trên sàn Tiki.
        //NOTE: Tạo nhiều biến thể, ảnh, kích thước, cân nặng sẽ được cập nhật sau từ bảng tb_track_original_sku
        // Trung lặp code khá nhiều với hàm CreateTikiProductFromProductIdInWarehouse
        public static async Task TikiCreateVariantsFromComboId(int comboId,
            MySqlResultState result,
            MySqlConnection conn)
        {
            TikiCreateProductTrackingResponse trackObj = null;
            try
            {
                TikiCreatingProduct tikiCreatingProduct = new TikiCreatingProduct();
                Combo combo = await ComboMySql.GetComboAsync(comboId);
                if (string.IsNullOrEmpty(combo.detail))
                {
                    result.State = EMySqlResultState.INVALID;
                    result.Message = "Chưa có mô tả chung của combo";
                    return;
                }
                combo.SetSrcImageVideo();

                // Lấy sản phẩm trong kho
                Product product = combo.products.FirstOrDefault();

                // Từ category id sản phẩm trong kho, lấy category tương ứng trên Tiki
                tikiCreatingProduct.category_id =
                    await ProductMySql.GetTikiCategoryIdFromProductCategoryIdAsync(product.categoryId, conn);

                tikiCreatingProduct.name = Product.GenerateVariantName(combo.name, product.categoryName);

                tikiCreatingProduct.description = TikiGetDescriptsFromDetail(combo.detail);


                int totalPrice = combo.products.Sum(p => p.bookCoverPrice);
                tikiCreatingProduct.market_price = totalPrice;

                Publisher publisher = await PublisherMySql.GetPublisherAsync(product.publisherId);

                // Attribute. Ta chỉ cập nhật những thuộc tính bắt buộc phải có và 1 vài thuộc tính khác
                List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category.TikiAttribute> attributes =
                    await TikiMySql.GetTikiAttributesOfCategoryAsync(tikiCreatingProduct.category_id, conn);
                var tikiAttributesGroups = new Dictionary<string, object>();
                foreach (var attr in attributes)
                {
                    if (attr.code == "age_group")
                    {
                        // Lấy minAge
                        int minAge = combo.products.Min(p => p.minAge);
                        // Lấy maxAge
                        int maxAge = combo.products.Max(p => p.maxAge);
                        if (combo.products.Min(p => p.maxAge) == 0)
                        {
                            maxAge = 0;
                        }
                        tikiCreatingProduct.attributes.age_group = TikiCreateProduct.TikiGetAgeGroups(minAge, maxAge);
                    }
                    else if (attr.code == "book_cover")
                    {
                        tikiCreatingProduct.attributes.book_cover = TikiCreateProduct.TikiGetStringCover(product.hardCover);
                    }
                    else if (attr.code == "language_book")
                    {
                        tikiCreatingProduct.attributes.language_book = product.language;
                    }
                    else if (attr.code == "manufacturer")
                    {
                        tikiCreatingProduct.attributes.manufacturer = product.publishingCompany;
                    }
                    else if (attr.code == "product_height") // Kích thước, cân nặng được cập nhật sau với từng sản phẩm
                    {
                        tikiCreatingProduct.attributes.product_height =
                            TikiCreateProduct.TikiGetStringOneDimension(product.productHigh);
                    }
                    else if (attr.code == "product_length")
                    {
                        tikiCreatingProduct.attributes.product_length =
                            TikiCreateProduct.TikiGetStringOneDimension(product.productLong);
                    }
                    else if (attr.code == "product_width")
                    {
                        tikiCreatingProduct.attributes.product_width =
                            TikiCreateProduct.TikiGetStringOneDimension(product.productWide);
                    }
                    else if (attr.code == "product_weight_kg")
                    {
                        tikiCreatingProduct.attributes.product_weight_kg =
                            TikiCreateProduct.TikiGetStringWeight(product.productWeight);
                    }
                    else if (attr.code == "publisher_vn")
                    {
                        tikiCreatingProduct.attributes.publisher_vn = publisher.tikiAttributeValue;
                    }
                    else if (attr.code == "origin")
                    {
                        tikiCreatingProduct.attributes.origin = publisher.origin;
                    }
                    else if (attr.code == "Organization_address")
                    {
                        tikiCreatingProduct.attributes.Organization_address = publisher.organization_address;
                    }
                    else if (attr.code == "Organization_name")
                    {
                        tikiCreatingProduct.attributes.Organization_name = publisher.organization_name;
                    }
                }

                //// pageNumber, dimensions: Đây chỉ là thông tin tương đối của sản phẩm trong combo
                //// number_of_page
                //if (product.pageNumber > 0)
                //{
                //    tikiCreatingProduct.attributes.number_of_page = product.pageNumber.ToString();
                //}
                // dimensions
                tikiCreatingProduct.attributes.dimensions =
                TikiCreateProduct.TikiGetStringDimensions(product.productLong, product.productWide, product.productHigh);

                // dịch giả
                if (!string.IsNullOrEmpty(product.translator))
                {
                    tikiCreatingProduct.attributes.dich_gia = product.translator;
                }

                // Lấy tất cả tên tác giả của từng cuốn lẻ, nếu số lượng tác giả lớn > 3 thì thay bằng "nhiều tác giả"
                List<string> authorList = new List<string>();
                StringBuilder authorsStrBuilder = new StringBuilder();
                foreach (var pro in combo.products)
                {
                    authorsStrBuilder.Append(pro.author).Append(',');
                }

                List<string> authorListTemp = authorsStrBuilder.ToString().Split(',')
                                .Select(s => s.Trim()) // Loại bỏ khoảng trắng ở đầu và cuối
                                .ToList();
                foreach (var au in authorListTemp)
                {
                    if (!authorList.Contains(au))
                    {
                        authorList.Add(au);
                    }
                }
                if (authorList.Count() > 3)
                {
                    tikiCreatingProduct.attributes.author.Add("Nhiều tác giả");
                }
                else
                {
                    // Tác giả. Thuộc tính này là "input_type": "multiselect"
                    if (!string.IsNullOrEmpty(product.author))
                    {
                        tikiCreatingProduct.attributes.author = product.author.Split(',')
                                    .Select(s => s.Trim()) // Loại bỏ khoảng trắng ở đầu và cuối
                                    .ToList();

                        foreach (string aut in tikiCreatingProduct.attributes.author)
                        {
                            MySqlResultState mySqlResult = await TikiCategoryAction.CheckGetAttributeValues(498, aut);
                            if (mySqlResult.State != EMySqlResultState.OK)
                            {
                                result.GetValueFrom(mySqlResult);
                                result.Message = "Đây là sản phẩm đầu tiên của combo. " + result.Message;
                                result.myJson = trackObj;
                                return;
                            }
                        }
                    }
                    else
                    {
                        result.State = EMySqlResultState.EMPTY;
                        result.Message = "Sản phẩm đầu tiên của combo tác giả đang trống.";
                    }
                }

                product.SetSrcImageVideo();
                if (product.imageSrc.Count > 0)
                {
                    tikiCreatingProduct.image = Common.httpsVoiBeNho + product.imageSrc[0];
                    //for (int i = 1; i < product.imageSrc.Count; i++)
                    //{
                    //    tikiCreatingProduct.images.Add(Common.httpsVoiBeNho + product.imageSrc[i]);
                    //}
                }
                tikiCreatingProduct.option_attributes.Add("Chủ Đề");

                List<TrackOriginalSku> trackOriginalSkus = new List<TrackOriginalSku>();

                // Tạo variant cho từng sản phẩm trong combo
                // Sản phẩm phải ở trạng thái khác ngừng kinh doanh
                // Lấy danh sách thuế phí
                TaxAndFee taxAndFee = await TikiDealDiscountMySql.GetTaxAndFeeAsync(Common.eTiki, conn);

                float discountOfPublisher = publisher.discount;

                Boolean hasNgungKinhDoanh = false;
                foreach (var p in combo.products)
                {
                    if (p.status == (int)ESanPhamStatus.NGUNG_KINH_DOANH)
                    {
                        hasNgungKinhDoanh = true;
                        continue;
                    }
                    Variant variant = new Variant();

                    variant.price = CommonOpenPlatform.CaculateSalePriceCoreFromBookCoverPrice(
                        product.bookCoverPrice, product.discount, discountOfPublisher, taxAndFee);
                    variant.inventory_type = TikiConstValues.inventory_type;
                    variant.seller_warehouse = TikiConstValues.intIdKho28Ngo3TTDL.ToString();
                    variant.sku = TikiConstValues.GenerateRandomSKUString();
                    trackOriginalSkus.Add(new TrackOriginalSku
                    {
                        OriginalSku = variant.sku,
                        IsVariant = 1,
                        ProductId = p.id,
                        ECommmerce = EECommerceType.TIKI,
                        Quantity = 1
                    });

                    variant.min_code = Common.GenerateRandomMincodeLong();
                    variant.option1 = !string.IsNullOrEmpty(p.shortName) ? p.shortName : p.name;

                    p.SetSrcImageVideo();
                    if (p.imageSrc.Count > 0)
                    {
                        //variant.image = Common.httpsVoiBeNho + p.imageSrc[0]; // TEMPORARY
                        variant.image = Common.httpsVoiBeNho + product.imageSrc[0];
                        // Cập nhật ảnh sau
                        //for (int i = 1; i < p.imageSrc.Count; i++)
                        //{
                        //    variant.images.Add(Common.httpsVoiBeNho + p.imageSrc[i]);
                        //}
                    }

                    WarehouseStock warehouseStock = new WarehouseStock();
                    warehouseStock.warehouseId = TikiConstValues.intIdKho28Ngo3TTDL;
                    warehouseStock.qtyAvailable = p.quantity;

                    variant.warehouse_stocks.Add(warehouseStock);
                    tikiCreatingProduct.variants.Add(variant);
                }

                // Tạo variant là combo nếu combo có ảnh riêng, có mô tả, và không có sản phẩm thuộc combo ngừng kinh doanh
                if (!string.IsNullOrEmpty(combo.detail) && combo.imageSrc.Count > 0 && !hasNgungKinhDoanh)
                {
                    Variant variant = new Variant();
                    variant.price = CommonOpenPlatform.CaculateSalePriceCoreFromBookCoverPrice(
                        totalPrice, product.discount, discountOfPublisher, taxAndFee);

                    variant.inventory_type = TikiConstValues.inventory_type;
                    variant.seller_warehouse = TikiConstValues.intIdKho28Ngo3TTDL.ToString();
                    variant.sku = TikiConstValues.GenerateRandomSKUString();
                    foreach (var pp in combo.products)
                    {
                        trackOriginalSkus.Add(new TrackOriginalSku
                        {
                            OriginalSku = variant.sku,
                            IsVariant = 1,
                            ProductId = pp.id,
                            ECommmerce = EECommerceType.TIKI,
                            Quantity = 1
                        });
                    }
                    variant.min_code = Common.GenerateRandomMincodeLong();
                    variant.option1 = "Combo " + combo.products.Count + " Cuốn";

                    if (combo.imageSrc.Count > 0)
                    {
                        //variant.image = Common.httpsVoiBeNho + combo.imageSrc[0]; //TEMPORARY
                        variant.image = Common.httpsVoiBeNho + product.imageSrc[0];

                        // Ảnh sẽ cập nhật sau này
                        //for (int i = 1; i < combo.imageSrc.Count; i++)
                        //{
                        //    variant.images.Add(Common.httpsVoiBeNho + combo.imageSrc[i]);
                        //}
                    }

                    WarehouseStock warehouseStock = new WarehouseStock();
                    warehouseStock.warehouseId = TikiConstValues.intIdKho28Ngo3TTDL;
                    warehouseStock.qtyAvailable = combo.products.Min(p => p.quantity);

                    variant.warehouse_stocks.Add(warehouseStock);
                    tikiCreatingProduct.variants.Add(variant);
                }

                // certificate_files
                TikiGetCertificateFiles(tikiCreatingProduct, publisher);

                // meta_data
                MetaData metaData = new MetaData();
                metaData.is_auto_turn_on = true;
                tikiCreatingProduct.meta_data = metaData;

                // Tạo sản phẩm
                trackObj = await TikiCreateProduct.CreateProduct(tikiCreatingProduct, result);

                if (result.State == EMySqlResultState.OK)
                {
                    await TikiWaitingCreatingProduct(trackObj, result, trackOriginalSkus, conn);
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
                trackObj = null;
            }
            result.myJson = trackObj;
        }


        public static async Task TikiWaitingCreatingProduct(TikiCreateProductTrackingResponse trackObj,
            MySqlResultState result,
             List<TrackOriginalSku> trackOriginalSkus,
            MySqlConnection conn)
        {
            if (trackObj != null)
            {
                foreach (var tra in trackOriginalSkus)
                {
                    tra.TrackId = trackObj.track_id;
                }

                await TikiMySql.TikiInsert_tbTikiTrackCreateProductAsync(trackObj.track_id,
                    trackObj.state,
                    trackObj.reason,
                    trackObj.request_id,
                    string.Empty,
                    conn);
            }

            if (trackObj.state == "queuing")
            {
                // Không có lỗi nghiêm trọng
                // Ta lưu original sku vào db
                await TrackOriginalSkuMySql.InsertTrackOriginalSkuBatchAsync(trackOriginalSkus, conn);

                //// Ta đợi 10 giây, lấy lại trạng thái để xem đã được approved
                //// Nếu được approved ta thêm sản phẩm vào tikiItem, mapping với sản phẩm riêng lẻ
                //// Với sản phẩm nhiều biến thể, ta cập nhật ảnh, kích thước, cân nặng, trang,... đã
                //Thread.Sleep(10000);
                //trackObj = await TikiCreateProduct.TrackingRequestCreateProduct(trackObj.track_id);
                //if (trackObj != null)
                //{
                //    await TikiMySql.TikiInsert_tbTikiTrackCreateProductAsync(trackObj.track_id,
                //        trackObj.state,
                //        trackObj.reason,
                //        trackObj.request_id,
                //        string.Empty,
                //        conn);
                //}

                //if (trackObj.state != "approved")
                //{
                //    result.State = EMySqlResultState.PENDING;
                //    result.Message = "Đăng sản phẩm đang ở trạng thái: " + trackObj.state + ". " + trackObj.reason;
                //}
            }

            result.State = EMySqlResultState.PENDING;
            result.Message = "Đăng sản phẩm đang ở trạng thái: " + trackObj.state + ". " + trackObj.reason;
        }

        /// <summary>
        /// Tìm product id cập nhật số lượng thất bại qua thông tin sàn trả về
        /// </summary>
        /// <param name="listCommonItem"></param>
        /// <param name="listProductIdChanged"></param>
        /// <returns></returns>
        public static List<int> TikiGetListProductIdUpdateFail_Batch(List<CommonItem> listCommonItem,
            List<int> listProductIdChanged)
        {
            List<int> listProductIdUpdateFail = new List<int>();
            foreach (var commonItem in listCommonItem)
            {
                // Sàn Tiki trả về cập nhật lỗi
                if (commonItem.result == null ||
                   commonItem.result.State != EMySqlResultState.OK)
                {
                    foreach (var mapping in commonItem.models[0].mapping)
                    {
                        if (listProductIdChanged.Contains(mapping.product.id) &&
                            !listProductIdUpdateFail.Contains(mapping.product.id))
                        {
                            listProductIdUpdateFail.Add(mapping.product.id);
                        }
                    }
                }
            }
            return listProductIdUpdateFail;
        }
    }
}
