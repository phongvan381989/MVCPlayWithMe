using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.ProductModel;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeCreateProduct;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeCreateProduct;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct;
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
    public class ShopeeProductSupporting
    {
        // Cập nhật quantity của model trên sàn Shopee
        // Nếu lỗi này xuất hiện:
        //"failure_list": [{
        //        "failed_reason": "model ID not exist in sku",
        //        "model_id": 232454047582
        //    }
        //]
        // Ta xóa bỏ mọi thứ liên quan đến model_id vĩnh viễn khỏi cơ sở dữ liệu => KHÔNG ỔN, ta chỉ disable
        public static async Task ShopeeProductUpdateStockAndImpactDbAsync(
            MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct.ShopeeUpdateStock st,
            MySqlResultState result)
        {
            try
            {
                ShopeeUpdateStockResponseHTTP rs =
                    await MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeProduct.ShopeeUpdateStock.ShopeeProductUpdateStockAsync(st);
                result.myJson = rs;
                if (rs == null)
                {
                    return;
                }

                foreach (var f in rs.response.failure_list)
                {
                    if (f.failed_reason == "model ID not exist in sku")
                    {
                        // Xóa bỏ mọi thứ liên quan đến model_id vĩnh viễn khỏi cơ sở dữ liệu => Disable
                        await ShopeeMySql.ShopeeDisableModelOnDBAsync(f.model_id);
                    }
                    else if (f.failed_reason.Contains(st.item_id.ToString() + " status is abnormal"))
                    {
                        //"response": {
                        //"failure_list": [{
                        //"failed_reason": "All the fields cannot be updated because the product  22046434841 status is abnormal"
                        //}
                        //],
                        //"success_list": []
                        //},

                        // Item trạng thái abnormal trên shopee. Xóa bỏ mọi thứ liên quan đến item vĩnh viễn khỏi csdl

                        await ShopeeMySql.ShopeeDeleteItemOnDBAsync(st.item_id);
                    }
                    else
                    {
                        result.State = EMySqlResultState.ERROR;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
        }


        // Cập nhật số lượng sản phẩm của tất cả model trong commonItem một lần
        public static async Task ShopeeUpdateQuantityOfOneItemAsync(CommonItem commonItem/*, MySqlConnection conn*/)
        {
            if (!commonItem.bActive)
            {
                commonItem.result = null;
                return;
            }

            MySqlResultState result = new MySqlResultState();

            long itemId = commonItem.itemId;
            long modelId = 0;
            int quantity = 0;
            MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct.ShopeeUpdateStock st =
                    new MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct.ShopeeUpdateStock();
            st.item_id = itemId;
            foreach (var model in commonItem.models)
            {
                modelId = model.modelId;
                //quantity = sqler.ShopeeGetQuantityOfOneItemModelConnectOut(itemId, modelId, conn);
                quantity = model.GetQuantityFromListMapping();

                if (modelId == -1)
                {
                    st.stock_list.Add(new ShopeeUpdateStockStock(0, quantity));
                }
                else
                {
                    st.stock_list.Add(new ShopeeUpdateStockStock(modelId, quantity));
                }
            }

            await ShopeeProductUpdateStockAndImpactDbAsync(st, result);
            commonItem.result = result;
        }

        public static async Task<List<CommonItem>> ShopeeGetListNeedUpdateQuantityAndUpdateAsync(MySqlConnection conn)
        {
            // Danh sách sản phẩm Shopee
            List<CommonItem> listCommonItem = await ShopeeMySql.ShopeeGetListNeedUpdateQuantityConnectOutAsync(conn);
            //ShopeeGetStatusImageSrcQuantitySellableAsync(listCommonItem);
            foreach (var commonItem in listCommonItem)
            {
                await ShopeeUpdateQuantityOfOneItemAsync(commonItem/*, conn*/);
            }

            return listCommonItem;
        }

        // Hàm này tạm thời chưa dùng vì trạng thái item/model ở db chưa được cập nhật realtime
        // Nếu db chưa lưu image src của item, model ta lấy và lưu
        public static async Task ShopeeUpdateImageSrcToDbIfNeedAsync(List<CommonItem> shopeeList, MySqlConnection conn)
        {
            foreach (var item in shopeeList)
            {
                Boolean isNeedDownloadImage = false;

                if (string.IsNullOrEmpty(item.imageSrc))
                {
                    isNeedDownloadImage = true;
                }
                foreach (var model in item.models)
                {
                    if (string.IsNullOrEmpty(model.imageSrc))
                    {
                        isNeedDownloadImage = true;
                        break;
                    }
                }
                if (isNeedDownloadImage)
                {
                    ShopeeGetItemBaseInfoItem pro = await ShopeeGetItemBaseInfo.ShopeeProductGetItemBaseInfoFromIdAsync(item.itemId);
                    if (pro != null)
                    {
                        // Lấy imageSrc cho item
                        if (string.IsNullOrEmpty(item.imageSrc))
                        {
                            item.imageSrc = pro.image.image_url_list[0];
                            await ProductMySql.UpdateImageSrcShopeeItemAsync(item.itemId, item.imageSrc, conn);
                        }

                        // Lấy imageSrc cho model nếu có
                        if (pro.has_model)
                        {
                            ShopeeGetModelListResponse obj = await ShopeeGetModelList.ShopeeProductGetModelListAsync(pro.item_id);
                            if (obj != null)
                            {
                                ShopeeGetModelList_TierVariation tierVar = obj.tier_variation[0];
                                int count = tierVar.option_list.Count;
                                for (int i = 0; i < count; i++)
                                {
                                    ShopeeGetModelList_Model model = CommonItem.GetModelFromModelListResponse(obj, i);
                                    ShopeeGetModelList_TierVariation_Option option = tierVar.option_list[i];
                                    if (option.image != null)
                                    {
                                        foreach (var m in item.models)
                                        {
                                            if (!string.IsNullOrEmpty(m.imageSrc))
                                                continue;

                                            if (m.modelId == model.model_id)
                                            {
                                                m.imageSrc = option.image.image_url;
                                                await ProductMySql.UpdateImageSrcShopeeModelAsync(m.modelId, m.imageSrc, conn);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Lấy trạng thái sản phẩm, image đại diện, số lượng trên sàn Shopee.
        // Cập nhật số lượng với sản phẩm NORMAL
        // NOTE:
        // Hàm này không dùng nữa do mất thời gian.
        // Image và status được lưu trong db. Sau này có chức nặng cập nhật Image và status
        public static async Task ShopeeGetStatusImageSrcQuantitySellableAsync(List<CommonItem> shopeeList)
        {
            foreach (var item in shopeeList)
            {
                ShopeeGetItemBaseInfoItem pro = await ShopeeGetItemBaseInfo.ShopeeProductGetItemBaseInfoFromIdAsync(item.itemId);
                if (pro != null)
                {
                    item.imageSrc = pro.image.image_url_list[0];
                    if (pro.item_status != "NORMAL")
                    {
                        item.bActive = false;
                        continue;
                    }

                    item.bActive = true;

                    // Lấy imageSrc cho model nếu có
                    if (pro.has_model)
                    {
                        ShopeeGetModelListResponse obj = await ShopeeGetModelList.ShopeeProductGetModelListAsync(pro.item_id);
                        if (obj != null)
                        {
                            ShopeeGetModelList_TierVariation tierVar = obj.tier_variation[0];
                            int count = tierVar.option_list.Count;
                            for (int i = 0; i < count; i++)
                            {
                                ShopeeGetModelList_Model model = CommonItem.GetModelFromModelListResponse(obj, i);
                                ShopeeGetModelList_TierVariation_Option option = tierVar.option_list[i];
                                // Lấy ảnh đại diện
                                if (option.image != null)
                                {
                                    foreach (var m in item.models)
                                    {
                                        if (m.modelId == model.model_id)
                                        {
                                            m.imageSrc = option.image.image_url;
                                            break;
                                        }
                                    }
                                }
                                // Lấy số lượng có thể bán trên sàn
                                foreach (var m in item.models)
                                {
                                    if (m.modelId == model.model_id)
                                    {
                                        m.quantity_sellable = model.stock_info_v2.seller_stock[0].stock; ;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        item.models[0].imageSrc = item.imageSrc;
                        item.models[0].quantity_sellable = pro.stock_info_v2.seller_stock[0].stock;
                    }
                }
            }
        }

        // Cập nhật số lượng sản phẩm của 1 model từ itemId, modelId
        public static async Task<MySqlResultState> ShopeeUpdateQuantityOfOneItemModelAsync(long itemId,
            long modelId,
            MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            int quantity = await ProductMySql.ShopeeGetQuantityOfOneItemModelConnectOutAsync(itemId, modelId, conn);

            MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct.ShopeeUpdateStock st =
                    new MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct.ShopeeUpdateStock();
            st.item_id = itemId;

            if (modelId == -1)
            {
                st.stock_list.Add(new ShopeeUpdateStockStock(0, quantity));
            }
            else
            {
                st.stock_list.Add(new ShopeeUpdateStockStock(modelId, quantity));
            }

            await ShopeeProductSupporting.ShopeeProductUpdateStockAndImpactDbAsync(st, result);
            return result;
        }

        // Sản phẩm kích thước vượt ngưỡng của kênh vận chuyển sẽ bị loại
        public static MySqlResultState ShopeeCheckLogisticInfo(
            ShopeeAddItem_RequestParameters requestParameters)
        {
            MySqlResultState result = new MySqlResultState();

            // NOTE: Hardcode chỉ với kênh Tủ Nhận Hàng
            // "logistics_channel_id": 50039,
            // "logistics_channel_name": "Tủ Nhận Hàng",
            //"item_max_dimension": {
            //    "dimension_sum": 0,
            //        "height": 8,
            //        "length": 32,
            //        "unit": "cm",
            //        "width": 26
            //    }
            //"volume_limit": {
            //"item_max_volume": 0,
            //        "item_min_volume": 0
            //    },
            //    "weight_limit": {
            //        "item_max_weight": 2,
            //        "item_min_weight": 0
            //    }
            if (requestParameters.dimension.package_length > 32 ||
                requestParameters.weight > 2)
            {
                // Loại bỏ khỏi danh sách vận chuyển
                for (int i = 0; i < requestParameters.logistic_info.Count; i++)
                {
                    if (requestParameters.logistic_info[i].logistic_id == 50039)
                    {
                        requestParameters.logistic_info.RemoveAt(i);
                        break;
                    }
                }
            }
            return result;
        }

        // Nếu up sản phẩm thành công, ta lấy sản phẩm và insert vào db, mapping luôn
        // Item chỉ có 1 model
        public static async Task ShopeeInsertNewItem_OneModelAndMapping(long ItemId, int productId,
            MySqlConnection conn)
        {
            ShopeeGetItemBaseInfoItem pro =
                await ShopeeGetItemBaseInfo.ShopeeProductGetItemBaseInfoFromIdAsync(ItemId);
            if (pro == null)
            {
                return;
            }

            CommonItem item = await CommonItem.CommonItemFromShopeeGetItemBaseInfoItemAsync(pro);
            long itemIdInserted = await ShopeeMySql.InserttbShopeeItemAsync(item, conn);

            // Vì chỉ có 1 model
            long modelIdInsert = await ShopeeMySql.InserttbShopeeModelAsync(itemIdInserted, item.models[0], conn);

            // Ta mapping
            await ShopeeMySql.ShopeeInsertNewMappingOneOfModelAsync(modelIdInsert, productId, 1, conn);
        }

        public static async Task<MySqlResultState> ShopeeCreateProductFromProductIdInWarehouse(
            int productId,
            string name,
            Boolean isNeedUploadImage,
            MySqlConnection conn)
        {
            MyLogger.GetInstance().Info("CreateShopeeProductFromProductIdInWarehouse Call productId: " + productId + ", name: " + name + ", isNeedUploadImage: " + isNeedUploadImage.ToString());
            // Lấy sản phẩm trong kho
            Product product = await ProductMySql.GetProductFromIdAsync(productId, conn);
            MySqlResultState result = CommonProductSupporting.CheckValidBookProductInfo(product);
            if (result.State != EMySqlResultState.OK)
            {
                return result;
            }

            if (!product.categoryName.StartsWith("sách", StringComparison.OrdinalIgnoreCase))
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "Sản phẩm không phải là sách, chưa được hỗ trợ đăng.";
                return result;
            }

            if (isNeedUploadImage)
            {
                // Cần kiểm tra xem đã up ảnh lên shopee chưa? 
                // Không mỗi lần đăng sản phẩm lại up ảnh lại không cần thiết

                int count = await ShopeeMySql.GetQuantityOfProductImageUploadedToShopeeAsync(0, productId, 0, conn);

                if (count == -1)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Không đếm được số ảnh sản phẩm đã up lên shopp.";
                    return result;
                }
                if (count == 0 || // chưa up ảnh lên shopee
                    product.imageSrc.Count != count)// SỐ lượng ảnh sản phẩm và ảnh đã up có sự thay đổi. Ta up lại lên shopee
                {

                    // Trước khi up xóa bỏ id ảnh cũ nếu có
                    if (count > 0)
                    {
                        result = await ShopeeMySql.DeleteProductImageUploadedToShopeeAsync(0, productId, 0, conn);
                        if (result.State != EMySqlResultState.OK)
                        {
                            return result;
                        }
                    }

                    result = await CommonProductSupporting.UploadImageOnECommerce_Core(productId, Common.eShopee, conn);
                    if (result.State != EMySqlResultState.OK)
                    {
                        return result;
                    }
                }
            }

            // Tạo đối tượng tham số của request
            ShopeeAddItem_RequestParameters requestParameters = new ShopeeAddItem_RequestParameters();
            requestParameters.original_price = product.bookCoverPrice;

            requestParameters.description = Common.CleanHtml_ByTwo(product.detail);

            requestParameters.weight = (float)product.productWeight / 1000 + (float)0.1;

            if (string.IsNullOrEmpty(name))
            {
                requestParameters.item_name = Product.GenerateName(product);
            }
            else
            {
                requestParameters.item_name = name;
            }

            requestParameters.item_status = "NORMAL";

            ShopeeDimension dimension = new ShopeeDimension();
            dimension.package_length = product.productLong / 10 + 1;
            dimension.package_width = product.productWide / 10 + 1;
            dimension.package_height = product.productHigh / 10 + 1;
            requestParameters.dimension = dimension;

            // Logistic channel setting
            requestParameters.logistic_info = await ShopeeLogistic.GetLogisticInfoAsync(false);

            if (requestParameters.logistic_info.Count == 0)
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "Không lấy được đơn vị vận vận chuyển.";
                return result;
            }

            // LOại bỏ kênh vận chuyển không phù hợp kích thước, cân nặng
            ShopeeCheckLogisticInfo(requestParameters);

            // attribute
            requestParameters.attribute_list = ShopeeAddItem.GetAttributeOfChildren_AudultBook(product);

            // category
            // Sách trẻ em
            if (product.maxAge > 0 && product.maxAge <= 12 * 18)
            {
                requestParameters.category_id = 101541;
            }
            else
            {
                // Sách người lớn
                requestParameters.category_id = 101543;
            }

            // Image
            List<string> image = await ShopeeMySql.GetUploadedImageOfProductOnShopeeAsync(0, product.id, 0, conn);
            requestParameters.image = new ShopeeImage(image);

            // Thương hiệu là tên tác giả, nhiều tác giả thì lựa chọn thương hiệu nhiều tác giả.
            ShopeeBrandRequestParameter brand = null;
            if (product.author.IndexOf(',') > 0)
            {
                // Hardcode luông
                brand = new ShopeeBrandRequestParameter(1139260, "Nhiều tác giả");
            }
            else
            {
                brand = await ShopeeMySql.GetBrandFromNameAsync(product.author, conn);
                if (brand == null)
                {
                    result.State = EMySqlResultState.INVALID;
                    result.Message = "Tác giả chưa được đăng ký thương hiệu.";
                    return result;
                }
            }
            requestParameters.brand = brand;

            // Tồn kho
            requestParameters.seller_stock = new List<ShopeeSellerStock>();
            requestParameters.seller_stock.Add(new ShopeeSellerStock(product.quantity));

            ShopeeAddItemResponseHTTP objResponse =
                await ShopeeAddItem.ShopeeProductAddItemAsync(requestParameters);
            if (objResponse == null)
            {
                result.State = EMySqlResultState.EMPTY;
                result.Message = "ShopeeProductAddItem response null";
                return result;
            }
            else if (!string.IsNullOrEmpty(objResponse.error))
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = JsonConvert.SerializeObject(objResponse.error);
                return result;
            }

            // Nếu up sản phẩm thành công, ta lấy sản phẩm và cập nhật mapping luôn
            // Lấy item id sản phẩm mới đăng thành công
            //objResponse.response.item_id;
            Thread.Sleep(5000);
            await ShopeeInsertNewItem_OneModelAndMapping(objResponse.response.item_id, productId, conn);

            return result;
        }

    }
}
