using Newtonsoft.Json;
using MVCPlayWithMe.General;
//using QuanLyKho.Model;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct;
//using QuanLyKho.Model.InOutWarehouse;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeOrder;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeProduct
{
    public class ShopeeUpdateStock
    {
        //{
        //"item_id": 1000,
        //"stock_list": [{
        //"model_id": 3456,
        //"normal_stock": 100
        //}]
        //}
        // Sản phẩm không có model
        // model_id : 0 
        public static ShopeeUpdateStockResponse ShopeeProductUpdateStock(MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct.ShopeeUpdateStock st)
        {
            string path = "/api/v2/product/update_stock";
            string body = JsonConvert.SerializeObject(st, Formatting.Indented);
            MyLogger.GetInstance().Info(body);

            IRestResponse response = CommonShopeeAPI.ShopeePostMethod(path, body);
            if (response == null)
                return null;
            ShopeeUpdateStockResponseHTTP objResponse = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    objResponse = JsonConvert.DeserializeObject<ShopeeUpdateStockResponseHTTP>(response.Content, settings);
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.Message);
                    return null;
                }
            }

            if (objResponse == null || objResponse.response == null)
                return null;

            return objResponse.response;
        }

        /// <summary>
        /// Kiểm tra tồn kho thực tế và trên sàn có bằng nhau không. Nếu bằng nhau thì không cần cập nhật số lượng
        /// </summary>
        /// <returns>True: Nếu cần cập nhật số lượng. NGược lại trả về false</returns>
        public static Boolean NeedUpdateQuantityOfProTMDT(MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct.ShopeeItemId itemId)
        {
            //int quantityInWarehouse = 0;
            //quantityInWarehouse = ModelThongTinChiTiet.GetQuantityForProTMDT(Common.commerceNameShopee, itemId.model_id.ToString());
            //if (itemId.quantity == quantityInWarehouse)// ||   // Số lượng trên sàn và trong kho đã đúng
            //    //(quantityInWarehouse == -1 && itemId.quantity == 0)) // Không lấy được số lượng trong kho và số lượng trên sàn đang là 0
            //    return false;

            return true;
        }

        /// <summary>
        /// Đối với sản phẩm có model,ta cập nhật tất cả model sản phẩm
        /// Kiểm tra tồn kho thực tế và trên sàn có bằng nhau không. Nếu bằng nhau thì không cần cập nhật số lượng
        /// </summary>
        /// <returns>True: Nếu có 1 sản phẩm trong danh sách cần cập nhật số lượng. NGược lại trả về false</returns>
        public static Boolean NeedUpdateQuantityOfProTMDT(long itemId)
        {
            //// Lấy list sản phẩm có cùng itemId từ db
            //List<MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct.ShopeeItemId> lsItemIdFromWarehouse = ModelMappingSanPhamTMDT_SanPhamKho.Shopee_GetListProFromItemId(itemId);
            //if (lsItemIdFromWarehouse.Count() == 0)
            //    return true;

            //// Lấy list sản phẩm có cùng itemId từ sàn
            //List<Model.Dev.ShopeeApp.ShopeeProduct.ShopeeItemId> lsItemIdOnTMDT = new List<ShopeeItemId>();
            //ShopeeGetModelListResponse objModel = ShopeeGetModelList.ShopeeProductGetModelList(itemId.ToString());
            //if (objModel != null)
            //{
            //    int count = objModel.model.Count();
            //    for (int i = 0; i < count; i++)
            //    {
            //        ShopeeGetModelList_Model model = objModel.model[i];
            //        int amount = model.stock_info_v2.seller_stock[0].stock;
            //        lsItemIdOnTMDT.Add(new ShopeeItemId(itemId, model.model_id, amount));
            //    }
            //}
            //Boolean isNeedUpdate = false;
            //foreach (var e in lsItemIdOnTMDT)
            //{
            //    // Check xem số lượng sản phẩm trên sàn có khác kho thực tế, nếu khác mới cần update
            //    Boolean isNeedUpdateTemp = true;
            //    foreach (var ee in lsItemIdFromWarehouse)
            //    {
            //        if (e.item_id == ee.item_id &&
            //            e.model_id == ee.model_id &&
            //            e.quantity == ee.quantity)
            //        {
            //            isNeedUpdateTemp = false;
            //            break;
            //        }
            //    }
            //    if (!isNeedUpdateTemp)
            //        continue;

            //    isNeedUpdate = true;
            //    break;
            //}
            //if (!isNeedUpdate)
            //    return false;

            return true;
        }

        /// <summary>
        /// Từ item_id, model_id sản phẩm cập nhật số lượng
        /// </summary>
        /// <param name="shopeeItemId"></param>
        /// <returns></returns>
        public static Boolean ShopeeProductUpdateStock(ShopeeItemId shopeeItemId)
        {

            OpenPlatform.Model.ShopeeApp.ShopeeProduct.ShopeeUpdateStock stock = 
                new OpenPlatform.Model.ShopeeApp.ShopeeProduct.ShopeeUpdateStock();

            stock.item_id = shopeeItemId.item_id;
            if (shopeeItemId.model_id == -1)// Không có model
                stock.stock_list.Add(new ShopeeUpdateStockStock(0, shopeeItemId.quantity));
            else
                stock.stock_list.Add(new ShopeeUpdateStockStock(shopeeItemId.model_id, shopeeItemId.quantity));

            ShopeeUpdateStockResponse rs = MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeProduct.ShopeeUpdateStock.ShopeeProductUpdateStock(stock);

            Boolean isOk = true;
            Common.CommonErrorMessage = string.Empty;
            foreach (var eFailed in rs.failure_list)
            {
                // Lưu vào db update mã sản phẩm lỗi
                isOk = false;
                Common.CommonErrorMessage = Common.CommonErrorMessage + eFailed.model_id.ToString() + ":" + eFailed.failed_reason + "; ";
                //break;
            }
            if (!isOk)
            {
                MyLogger.GetInstance().Info(Common.CommonErrorMessage);
                return false;
            }
            return true;
        }

        //private static Boolean UpdateToTonKhoSanTMDT(List<Model.Dev.ShopeeApp.ShopeeProduct.ShopeeItemId> lsItemId)
        //{
        //    // Cập nhật vào db ModelTonKhoSanTMDT
        //    List<string> lsCode = new List<string>();
        //    foreach (var sitem in lsItemId)
        //    {
        //        List<ModelMappingSanPhamTMDT_SanPhamKho> lsMapping = ModelMappingSanPhamTMDT_SanPhamKho.Shopee_GetListModelMappingSanPhamTMDT_SanPhamKhoFromIDProTMDT(sitem.model_id.ToString());

        //        foreach (var e in lsMapping)
        //        {
        //            if (!lsCode.Exists(x => x == e.code))
        //                lsCode.Add(e.code);
        //        }

        //    }
        //    ModelTonKhoSanTMDT.SynchronizeThongTinChiTietToTonKhoSanTMDT(lsCode, EnumCommerceType.SHOPEE);
        //    return true;
        //}

        /// <summary>
        /// Từ item_id cập nhật số lượng cho tất cả sản phẩm có chung item_id
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static Boolean ShopeeProductUpdateStock(long itemId)
        {
            // Lấy list sản phẩm có cùng itemId từ db
            List<ShopeeItemId> lsItemIdFromWarehouse = null;// ModelMappingSanPhamTMDT_SanPhamKho.Shopee_GetListProFromItemId(itemId);
            if (lsItemIdFromWarehouse.Count() == 0)
                return true;

            // List sản phẩm trên sàn cần cập nhật số lượng theo kho
            List<ShopeeItemId> lsItemIdNeedUpdateQuantity = new List<ShopeeItemId>();

            // Lấy list sản phẩm có cùng itemId từ sàn
            List<ShopeeItemId> lsItemIdOnTMDT = new List<ShopeeItemId>();
            ShopeeGetModelListResponse objModel = ShopeeGetModelList.ShopeeProductGetModelList(itemId);
            int amount = 0;
            if (objModel != null)
            {
                int count = objModel.model.Count();
                for (int i = 0; i < count; i++)
                {
                    ShopeeGetModelList_Model model = objModel.model[i];
                    amount = model.stock_info_v2.seller_stock[0].stock;
                    lsItemIdOnTMDT.Add(new ShopeeItemId(itemId, model.model_id, amount));
                }
            }
            long quantity = 0;
            MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct.ShopeeUpdateStock stock = new MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct.ShopeeUpdateStock();
            stock.item_id = itemId;
            foreach (var e in lsItemIdOnTMDT)
            {
                // Check xem số lượng sản phẩm trên sàn có khác kho thực tế, nếu khác mới cần update
                Boolean isNeedUpdate = true;
                quantity = 0;
                foreach(var ee in lsItemIdFromWarehouse)
                {
                    if (e.item_id == ee.item_id &&
                        e.model_id == ee.model_id)
                    {
                        quantity = ee.quantity;
                        if (e.quantity == quantity)
                        {
                            isNeedUpdate = false;
                            break;
                        }
                    }
                }
                if (!isNeedUpdate)
                    continue;
                e.quantity = quantity;
                lsItemIdNeedUpdateQuantity.Add(e);
                if (e.item_id == e.model_id)// Không có model
                    stock.stock_list.Add(new ShopeeUpdateStockStock(0, quantity));
                else
                    stock.stock_list.Add(new ShopeeUpdateStockStock(e.model_id, quantity));
            }

            // Không cần cập nhật số lượng
            if (stock.stock_list.Count() == 0)
                return true;

            ShopeeUpdateStockResponse rs = ShopeeProductUpdateStock(stock);
            if (rs == null)
                return false;

            Boolean isOk = true;
            foreach (var eFailed in rs.failure_list)
            {
                // Lưu vào db update mã sản phẩm lỗi
                //ModelUpdateStockFailure.Shopee_Update(stock.item_id.ToString(), eFailed.model_id.ToString());
                isOk = false;
                //break;
                Common.CommonErrorMessage = Common.CommonErrorMessage + eFailed.model_id.ToString() + ":" + eFailed.failed_reason + "; ";
            }
            if (!isOk)
                return false;

            ////// Cập nhật vào db ModelTonKhoSanTMDT
            ////UpdateToTonKhoSanTMDT(lsItemIdOnTMDT);
            //ModelMappingSanPhamTMDT_SanPhamKho.Shopee_UpdateQuantityOnTMDT(lsItemIdNeedUpdateQuantity);

            return true;
        }

        /// <summary>
        /// Từ đơn hàng mới trong khoảng thời gian, ta lấy được số lượng hàng cần xuất
        /// <param name="time_from"></param>
        /// <param name="time_to"></param>
        /// <returns></returns>
        static public Dictionary<string, int> ShopeeGetProductQuantityPairToTake(DateTime time_from, DateTime time_to)
        {
            List<ShopeeOrderDetail> lsOrderShopeeFullInfo;
            lsOrderShopeeFullInfo = ShopeeGetOrderDetail.ShopeeOrderGetOrderDetailAll(time_from, time_to, new ShopeeOrderStatus(ShopeeOrderStatus.EnumShopeeOrderStatus.READY_TO_SHIP));
            Dictionary<string, int> dic = ShopeeGetOrderDetail.ShopeeGetDictionaryOfProductQuantityFromListDetailOrder(lsOrderShopeeFullInfo);

            return dic;
        }

        ///// <summary>
        ///// Từ danh sách mã sản phẩm trong kho / số lượng tồn ta cập nhật số lượng tồn sản phẩm trên sàn TMDT
        ///// </summary>
        ///// <param name="dic">danh sách mã sản phẩm trong kho / số lượng tồn</param>
        ///// <returns></returns>
        //static public Boolean ShopeeUpdateTonKhoSanTMDT(Dictionary<string, int> dic)
        //{
        //    if (dic == null)
        //    {
        //        Common.CommonErrorMessage = "Danh sách sản phẩm / số lượng tồn kho null";
        //        MyLogger.GetInstance().Info(Common.CommonErrorMessage);
        //        return false;
        //    }
        //    List<ShopeeItemId> lsItem = new List<ShopeeItemId>();
        //    foreach (var obj in dic)
        //    {
        //        List<ShopeeItemId> lsItemTemp = ModelMappingSanPhamTMDT_SanPhamKho.Shopee_GetIdSPTMDMappingFromIdInWarehouse(obj.Key);
        //        foreach (var eItemTemp in lsItemTemp)
        //        {
        //            Boolean isExist = false;
        //            foreach(var eItem in lsItem)
        //            {
        //                if(eItem.item_id == eItemTemp.item_id && eItem.model_id == eItemTemp.model_id)
        //                {
        //                    isExist = true;
        //                    break;
        //                }
        //            }
        //            if (!isExist)
        //                lsItem.Add(eItemTemp);
        //        }
        //    }

        //    // Lấy list update lỗi để thực hiện lại
        //    //List<ShopeeItemId> lsItemFailed = ModelUpdateStockFailure.Shopee_GetListShopeeItemId();

        //    //// Gộp danh sách
        //    //foreach(var eItemFailed in lsItemFailed)
        //    //{
        //    //    Boolean isExist = false;
        //    //    foreach (var eItem in lsItem)
        //    //    {
        //    //        if (eItem.item_id == eItemFailed.item_id && eItem.model_id == eItemFailed.model_id)
        //    //        {
        //    //            isExist = true;
        //    //            break;
        //    //        }
        //    //    }
        //    //    if (!isExist)
        //    //        lsItem.Add(eItemFailed);
        //    //}

        //    // Tham số phục vụ update stock
        //    List<Model.Dev.ShopeeApp.ShopeeProduct.ShopeeUpdateStock> lsStock = new List<Model.Dev.ShopeeApp.ShopeeProduct.ShopeeUpdateStock>();
        //    foreach( var e in lsItem)
        //    {
        //        if(e.item_id == e.model_id) // Không có model, chỉ xuất hiện 1 lần
        //        {
        //            Model.Dev.ShopeeApp.ShopeeProduct.ShopeeUpdateStock newStock = new Model.Dev.ShopeeApp.ShopeeProduct.ShopeeUpdateStock();
        //            newStock.item_id = e.item_id;
        //            newStock.stock_list.Add(new ShopeeUpdateStockStock(0, 0));
        //            lsStock.Add(newStock);
        //            continue;
        //        }

        //        // Check đã tồn tại
        //        Boolean isExist = false;
        //        foreach (var eStock in lsStock)
        //        {
        //            if(eStock.item_id == e.item_id)
        //            {
        //                isExist = true;
        //                eStock.stock_list.Add(new ShopeeUpdateStockStock(e.model_id, 0));
        //                break;
        //            }
        //        }
        //        if(!isExist)
        //        {
        //            Model.Dev.ShopeeApp.ShopeeProduct.ShopeeUpdateStock newStock = new Model.Dev.ShopeeApp.ShopeeProduct.ShopeeUpdateStock();
        //            newStock.item_id = e.item_id;
        //            newStock.stock_list.Add(new ShopeeUpdateStockStock(e.model_id, 0));
        //            lsStock.Add(newStock);
        //        }
        //    }

        //    // Cập nhật tồn kho cho tham số
        //    int quantity = 0;
        //    foreach (var eStock in lsStock)
        //    {
        //        if(eStock.stock_list[0].model_id == 0)// Không có model
        //        {
        //            quantity = ModelTonKhoSanTMDT.GetQuantityOnTMDT(Common.commerceNameShopee, eStock.item_id.ToString());
        //            if (quantity != -1)
        //            {
        //                eStock.stock_list[0].normal_stock = quantity;
        //            }
        //            continue;
        //        }

        //        // Có model
        //        foreach (var e in eStock.stock_list)
        //        {
        //            quantity = ModelTonKhoSanTMDT.GetQuantityOnTMDT(Common.commerceNameShopee, e.model_id.ToString());
        //            if (quantity != -1)
        //            {
        //                e.normal_stock = quantity;
        //            }
        //        }
        //    }

        //    // Lưu item update lỗi
        //    //Dictionary<string, string> dicFailed = new Dictionary<string, string>();
        //    foreach (var eStock in lsStock)
        //    {
        //        ShopeeUpdateStockResponse rs = ShopeeProductUpdateStock(eStock);
        //        if ( rs == null)
        //            return false;
        //        foreach (var eFailed in rs.failure_list)
        //        {
        //            //dicFailed.Add(eStock.item_id.ToString(), eFailed.model_id.ToString());
        //            return false;
        //        }
        //    }

        //    //// Lưu vào db
        //    //if(dicFailed.Count() > 0)
        //    //    ModelUpdateStockFailure.Shopee_Update(dicFailed);

        //    return true;
        //}
    }
}
