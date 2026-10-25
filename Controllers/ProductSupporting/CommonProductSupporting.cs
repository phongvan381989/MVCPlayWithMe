using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.ProductModel;
using MVCPlayWithMe.OpenPlatform;
using MVCPlayWithMe.OpenPlatform.API.LazadaAPI;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeCreateProduct;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeCreateProduct;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static MVCPlayWithMe.General.Common;

namespace MVCPlayWithMe.Controllers.ProductSupporting
{
    public class CommonProductSupporting
    {
        /// <summary>
        /// Tìm product id cập nhật số lượng thất bại qua thông tin sàn trả về
        /// </summary>
        /// <param name="listCommonItem"></param>
        /// <param name="listProductIdChanged"></param>
        /// <returns></returns>
        public static List<int> GetListProductIdUpdateFail(List<CommonItem> listCommonItem,
            List<int> listProductIdChanged)
        {
            List<int> listProductIdUpdateFail = new List<int>();
            foreach (var commonItem in listCommonItem)
            {
                if (!commonItem.bActive) // tương ứng với commonItem.result = null
                    continue;

                if (commonItem.eType == Common.eTiki)
                {
                    // Sàn Tiki trả về cập nhật lỗi
                    if (commonItem.result.myJson == null ||
                        ((TikiUpdatePriceQuantityActiveResponse)commonItem.result.myJson).errors != null)
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
                else if (commonItem.eType == Common.eShopee)
                {
                    // Sàn Shoppee trả về cập nhật lỗi
                    if (commonItem.result.myJson == null ||
                        ((ShopeeUpdateStockResponseHTTP)commonItem.result.myJson).response.failure_list != null)
                    {
                        foreach (var fai in ((ShopeeUpdateStockResponseHTTP)commonItem.result.myJson).response.failure_list)
                        {
                            // Méo biết khi item không có model thì modelIdFail = 0 hay -1,...?
                            long modelIdFail = fai.model_id;
                            foreach (var model in commonItem.models)
                            {
                                if (modelIdFail == model.modelId || modelIdFail == 0)
                                {
                                    foreach (var mapping in model.mapping)
                                    {
                                        if (listProductIdChanged.Contains(mapping.product.id) &&
                                            !listProductIdUpdateFail.Contains(mapping.product.id))
                                        {
                                            listProductIdUpdateFail.Add(mapping.product.id);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (commonItem.eType == Common.eLazada)
                {
                    foreach (var model in commonItem.models)
                    {
                        if (!string.IsNullOrEmpty(model.whyUpdateFail))
                        {
                            foreach (var mapping in model.mapping)
                            {
                                if (listProductIdChanged.Contains(mapping.product.id) &&
                                    !listProductIdUpdateFail.Contains(mapping.product.id))
                                {
                                    listProductIdUpdateFail.Add(mapping.product.id);
                                }
                            }
                            break;
                        }
                    }
                }
            }
            return listProductIdUpdateFail;
        }

        public static async Task UpdateZeroStatusOfNeedUpdateQuantityConnectOutAsync(
            EECommerceType eECommerceType,
            List<int> listProductIdChanged,
            List<CommonItem> listCommonItem,
            MySqlConnection conn)
        {
            // Danh sách sản phẩm chưa cập nhật số lượng thành công
            List<int> listProductIdUpdateFail = GetListProductIdUpdateFail(listCommonItem, listProductIdChanged);

            // Danh sách sản phẩm cập nhật thành công
            List<int> listProductIdUpdateSuccess = new List<int>();
            foreach (var id in listProductIdChanged)
            {
                if (!listProductIdUpdateFail.Contains(id))
                {
                    listProductIdUpdateSuccess.Add(id);
                }
            }

            await ProductMySql.UpdateZeroStatusOfNeedUpdateQuantityConnectOutAsync(listProductIdUpdateSuccess, eECommerceType, conn);
        }

        public static async Task UpdateZeroStatusOfNeedUpdateQuantity_BatchConnectOutAsync(
            EECommerceType eECommerceType,
            List<int> listProductIdChanged,
            List<CommonItem> listCommonItem,
            MySqlResultState result,
            MySqlConnection conn)
        {
            // Danh sách sản phẩm chưa cập nhật số lượng thành công
            List<int> listProductIdUpdateFail = null;
            if (eECommerceType == EECommerceType.TIKI)
            {
                listProductIdUpdateFail = TikiProductSupporting.TikiGetListProductIdUpdateFail_Batch(listCommonItem, listProductIdChanged);
            }

            if (listProductIdUpdateFail == null)
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "listProductIdUpdateFail lấy lỗi";
                return;
            }

            // Danh sách sản phẩm cập nhật thành công
            List<int> listProductIdUpdateSuccess = new List<int>();
            foreach (var id in listProductIdChanged)
            {
                if (!listProductIdUpdateFail.Contains(id))
                {
                    listProductIdUpdateSuccess.Add(id);
                }
            }
            if (listProductIdUpdateFail.Count > 0)
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = "listProductIdUpdateFail lỗi: " + JsonConvert.SerializeObject(listProductIdUpdateFail);
            }

            await ProductMySql.UpdateZeroStatusOfNeedUpdateQuantityConnectOutAsync(listProductIdUpdateSuccess, eECommerceType, conn);
        }

        public static async Task UpdateZeroStatusOfNeedUpdatePrice_BatchConnectOutAsync(
            EECommerceType eECommerceType,
            List<int> listProductIdChanged,
            List<CommonItem> listCommonItem,
            MySqlResultState result,
            MySqlConnection conn)
        {
            // Danh sách sản phẩm chưa cập nhật số lượng thành công
            List<int> listProductIdUpdateFail = null;
            if (eECommerceType == EECommerceType.TIKI)
            {
                listProductIdUpdateFail = TikiProductSupporting.TikiGetListProductIdUpdateFail_Batch(listCommonItem, listProductIdChanged);
            }

            if(listProductIdUpdateFail == null)
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "listProductIdUpdateFail lấy lỗi";
                return;
            }

            // Danh sách sản phẩm cập nhật thành công
            List<int> listProductIdUpdateSuccess = new List<int>();
            foreach (var id in listProductIdChanged)
            {
                if (!listProductIdUpdateFail.Contains(id))
                {
                    listProductIdUpdateSuccess.Add(id);
                }
            }
            if(listProductIdUpdateFail.Count > 0)
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = "listProductIdUpdateFail lỗi: " + JsonConvert.SerializeObject(listProductIdUpdateFail);
            }

            await ProductMySql.UpdateZeroStatusOfNeedUpdatePriceConnectOutAsync(listProductIdUpdateSuccess, eECommerceType, conn);
        }

        public static async Task<List<CommonItem>> GetListNeedUpdateQuantityAndUpdate_CoreAsync()
        {
            List<CommonItem> ls = new List<CommonItem>();
            // CommonItem chưa có ảnh đại diện cho item, model ta lấy và lưu vào db
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    // Danh sách sản phẩm trong kho có thay đổi số lượng cần cập nhật
                    List<int> tikiProductIdList =
                        await ProductMySql.GetListProductOfNeedUpdateQuantityConnectOutAsync(conn, EECommerceType.TIKI);
                    if (tikiProductIdList.Count > 0)
                    {
                        //List<CommonItem> tikiList = await TikiProductSuporting.TikiGetListNeedUpdateQuantityAndUpdateAsync(conn);
                        List<CommonItem> tikiList = await TikiProductSupporting.TikiGetListNeedUpdateQuantityAndUpdate_BatchAsync(conn);
                        //await CommonProductSupporting.UpdateZeroStatusOfNeedUpdateQuantityConnectOutAsync(EECommerceType.TIKI, tikiProductIdList, tikiList, conn);
                        MySqlResultState result = new MySqlResultState();
                        await CommonProductSupporting.UpdateZeroStatusOfNeedUpdateQuantity_BatchConnectOutAsync(EECommerceType.TIKI, tikiProductIdList, tikiList, result, conn);
                        ls.AddRange(tikiList);
                    }

                    List<int> shopeeProductIdList =
                        await ProductMySql.GetListProductOfNeedUpdateQuantityConnectOutAsync(conn, EECommerceType.SHOPEE);
                    if (shopeeProductIdList.Count > 0)
                    {
                        List<CommonItem> shopeeList = await ShopeeProductSupporting.ShopeeGetListNeedUpdateQuantityAndUpdateAsync(conn);
                        await CommonProductSupporting.UpdateZeroStatusOfNeedUpdateQuantityConnectOutAsync(EECommerceType.SHOPEE, shopeeProductIdList, shopeeList, conn);
                        ls.AddRange(shopeeList);
                    }

                    List<int> lazadaProductIdList =
                        await ProductMySql.GetListProductOfNeedUpdateQuantityConnectOutAsync(conn, EECommerceType.LAZADA);

                    if (lazadaProductIdList.Count > 0)
                    {
                        List<CommonItem> lazadaList = await LazadaProductSupporting.LazadaGetListNeedUpdateQuantityAndUpdateAsync(conn);
                        await CommonProductSupporting.UpdateZeroStatusOfNeedUpdateQuantityConnectOutAsync(EECommerceType.LAZADA, lazadaProductIdList, lazadaList, conn);
                        ls.AddRange(lazadaList);
                    }

                    // Cập nhật số lượng cho voibenho
                    await VBNProductSupporting.VBNGetListNeedUpdateQuantityAndUpdateAsync(conn);
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    ls.Clear();
                }
            }
            // Lấy danh sách sản phẩm
            return ls;
        }

        public static MySqlResultState CheckValidBookProductInfo(Product product)
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

            if (string.IsNullOrEmpty(product.code) && string.IsNullOrEmpty(product.barcode))
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "Sản phẩm chưa có mã sản phẩm và isbn. Cần ít nhất 1 mã";
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
            if (string.IsNullOrWhiteSpace(product.author))
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "Chưa có thông tin tác giả.";
                return result;
            }

            if (product.publisherId <= 0)
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "Chưa có thông tin nhà phát hành.";
                return result;
            }

            if (string.IsNullOrWhiteSpace(product.publishingCompany))
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "Chưa có thông tin nhà xuất bản.";
                return result;
            }

            if (product.publishingTime <= 2000)
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "Chưa có thông tin năm xuất bản.";
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

            if (product.categoryName.StartsWith("sách", StringComparison.OrdinalIgnoreCase) &&
                product.pageNumber <= 0)
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "Chưa có thuộc tính số trang.";
                return result;
            }

            if (string.IsNullOrEmpty(product.language))
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "Chưa có thuộc tính ngôn ngữ.";
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

        public static async Task<MySqlResultState> UploadImageOnECommerce_Core(
            int id,
            string eType,
            MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            List<string> src = Product.GetProductImageAbsolutePath(id.ToString());
            if (eType == Common.eShopee)
            {
                int length = src.Count > ShopeeMediaSpace.maxImageOnItem ? ShopeeMediaSpace.maxImageOnItem : src.Count;
                for (int i = 0; i < length; i++)
                {
                    ShopeeUploadImageResponseHTTP objResponse =
                        await ShopeeMediaSpace.ShopeeUpload_Image(src[i]);

                    if (!string.IsNullOrEmpty(objResponse.error))
                    {
                        result.State = EMySqlResultState.ERROR;
                        result.Message = objResponse.error;
                        break;
                    }

                    result = await ShopeeMySql.InserttbShopeeMediaSpaceAsync(0,
                        objResponse.response.image_info.image_id,
                        id, 0, conn);
                    if (result.State != EMySqlResultState.OK)
                    {
                        break;
                    }
                }
            }
            else if (eType == Common.eLazada)
            {
                int length = src.Count;
                List<LazadaUploadImage> images = new List<LazadaUploadImage>();
                for (int i = 0; i < length; i++)
                {
                    // Vì hiện tại chỉ hỗ trợ đăng max có giới hạn, up nhiều cũng không dùng làm gì
                    if (i == LazadaProductAPI.maximumOfImages)
                    {
                        break;
                    }
                    // Kiểm tra xem ảnh có nặng không, nếu nặng thì giảm kích thước
                    Common.ReduceImageSizeToMediumAndSave(src[i], LazadaProductAPI.maximumOfImageSize);
                    LazadaUploadImage objResponse =
                        await LazadaProductAPI.LazadaUploadImageAsync(src[i]);

                    if (objResponse == null)
                    {
                        result.State = EMySqlResultState.ERROR;
                        result.Message = "Có ảnh đăng không thành công";
                        break;
                    }
                    images.Add(objResponse);
                }
                if (result.State == EMySqlResultState.OK)
                {
                    await LazadaMySql.InserttbLazadaMediaSpaceAsync(id, 0, 0, images, conn);
                }
            }

            return result;
        }

        public static async Task<string> CreateProductOnECommerceFromList_Core(
            List<int> productIds,
            string eType)
        {
            MySqlResultState result = new MySqlResultState();
            if (eType != Common.eShopee && eType != Common.eLazada && eType != Common.eTiki)
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "Chưa hỗ trợ tạo nhiều sản phẩm trên " + eType;
                return JsonConvert.SerializeObject(result);
            }
            MySqlResultState resultUp = null;
            MySqlResultState resultTemp = null;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    // Lấy danh sách sản phẩm chưa đăng bán riêng lẻ
                    List<int> needList =
                        await ProductMySql.SearchDontSellSigleWithNoParrentOnECommerce_GetIdListOnly_ConnectOutAsync(
                        eType, conn
                        );

                    foreach (var productId in productIds)
                    {
                        if (!needList.Contains(productId))
                        {
                            continue;
                        }

                        if (eType == Common.eTiki)
                        {
                            resultTemp = new MySqlResultState();
                            await TikiProductSupporting.TikiCreateProductFromProductIdInWarehouse(productId,
                            null, resultTemp, conn);
                            resultTemp.State = EMySqlResultState.OK;// Vì hàm luôn trả về Pendding
                        }
                        else if (eType == Common.eShopee)
                        {
                            resultTemp = await ShopeeProductSupporting.ShopeeCreateProductFromProductIdInWarehouse(productId,
                            null, true, conn);
                        }
                        else if (eType == Common.eLazada)
                        {
                            resultTemp = await LazadaProductSupporting.LazadaCreateProductFromProductIdInWarehouse(productId,
                            null, true, conn);
                        }

                        if (resultTemp.State != EMySqlResultState.OK && resultUp == null)
                        {
                            resultUp = new MySqlResultState();
                            resultUp.State = EMySqlResultState.INVALID;
                            resultUp.Message = "Có sản phẩm đăng lỗi. Lấy lại danh sách sản phẩm, và đăng từng sản phẩm một để biết lỗi cụ thể.";
                        }
                        Thread.Sleep(5000);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            if (resultUp != null && result.State == EMySqlResultState.OK)
            {
                result = resultUp;
            }

            return JsonConvert.SerializeObject(result);
        }

        // Từ giá bìa, lấy chiết khấu của nhà phát hành, thuế phí của sàn và tính được giá bán
        public static async Task<int> CaculateSpecial_Price(Product product,
            string eEcommerceName,
            MySqlConnection conn)
        {
            // Lấy danh sách thuế phí
            TaxAndFee taxAndFee = await TikiDealDiscountMySql.GetTaxAndFeeAsync(eEcommerceName, conn);

            // Lấy danh sách nhà phát hành, từ đó lấy được discount chung
            List<Publisher> listPublisher = await PublisherMySql.GetListPublisherConnectOutAsync(conn);

            float discountOfPublisher = 0;
            foreach (var publisher in listPublisher)
            {
                if (publisher.id == product.publisherId)
                {
                    discountOfPublisher = publisher.discount;
                    break;
                }
            }

            int sale_price = CommonOpenPlatform.CaculateSalePriceCoreFromBookCoverPrice(
                product.bookCoverPrice,
                product.discount,
                discountOfPublisher,
                taxAndFee);

            return sale_price;
        }
    }
}
