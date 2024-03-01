using Newtonsoft.Json;
using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.Config;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Order;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Config;

namespace MVCPlayWithMe.OpenPlatform.API.TikiAPI.Product
{
    public class TikiUpdateStock
    {
        static TikiConfigApp configApp = null;
        public static TikiConfigApp GetConfigApp()
        {
            if(configApp == null)
            {
                List<TikiConfigApp> l = ModelThongTinBaoMat.Tiki_InhouseAppGetListUsingApp();
                configApp = l[0];
            }
            return configApp;
        }
        //public static Boolean TikiProductUpdateQuantity(TikiUpdateQuantity st)
        //{
        //    //string http = TikiConstValues.cstrProductUpdate;
        //    //List<TikiConfigApp> l = ModelThongTinBaoMatTiki.Tiki_InhouseAppGetListUsingApp();
        //    //TikiConfigApp configApp = l[0];
        //    return TikiProductUpdateQuantity(GetConfigApp(), st);
        //}

        public static Boolean TikiProductUpdate(TikiConfigApp configApp, TikiUpdate st)
        {
            string http = TikiConstValues.cstrProductUpdate;
            IRestResponse response = CommonTikiAPI.PutExcuteRequest(configApp, http, st);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    TikiUpdateQuantityResponse updateResponse = JsonConvert.DeserializeObject<TikiUpdateQuantityResponse>(response.Content, settings);
                    Common.CommonErrorMessage = string.Empty;
                    foreach (var s in updateResponse.errors)
                    {
                        Common.CommonErrorMessage = Common.CommonErrorMessage + s + ". ";
                    }
                    MyLogger.GetInstance().Info(Common.CommonErrorMessage);
                    return false;
                }
                catch (Exception ex)
                {
                    Common.CommonErrorMessage = ex.Message;
                    MyLogger.GetInstance().Warn(ex.Message);
                    return false;
                }
            }
            return true;
        }
        public static Boolean TikiProductUpdateQuantity(TikiConfigApp configApp, TikiUpdateQuantity st)
        {
            return TikiProductUpdate(configApp, st);
        }

        public static Boolean TikiProductUpdatePrice(TikiConfigApp configApp, TikiUpdatePrice st)
        {
            return TikiProductUpdate(configApp, st);
        }

        public static Boolean TikiProductUpdateStatus(TikiConfigApp configApp, TikiUpdateStatus st)
        {
            return TikiProductUpdate(configApp, st);
        }

        public static int GetQuantityFromTikiProduct(TikiProduct product)
        {
            int quantity_sellable = 0;
            if(product == null)
                return quantity_sellable;
            if (product.inventory != null && product.inventory.warehouse_stocks != null)
            {
                foreach (var e in product.inventory.warehouse_stocks)
                {
                    if (e.warehouse_id == TikiConstValues.intIdKho28Ngo3TTDL)
                    {
                        quantity_sellable = e.quantity_sellable;
                        break;
                    }
                }
            }

            return quantity_sellable;
        }

        /// <summary>
        /// Set số lượng sản phẩm sau khi update số lượng lên sàn
        /// </summary>
        /// <param name="product"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static void SetQuantityToTikiProduct(TikiProduct product, int quantity)
        {
            if (product == null)
                return ;
            if (product.inventory != null && product.inventory.warehouse_stocks != null)
            {
                foreach (var e in product.inventory.warehouse_stocks)
                {
                    if (e.warehouse_id == TikiConstValues.intIdKho28Ngo3TTDL)
                    {
                        e.quantity_sellable = quantity;
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Kiểm tra tồn kho thực tế và trên sàn có bằng nhau không.
        /// Nếu bằng nhau thì không cần cập nhật số lượng
        /// </summary>
        /// <param name="proId"></param>
        /// <param name="quantity">Số lượng trên sàn TMDT</param>
        /// <returns>True: Nếu cần cập nhật số lượng. NGược lại trả về false</returns>
        public static Boolean NeedUpdateQuantityOfProTMDT(string proId, long quantity)
        {
            int quantityInWarehouse = 0;
            //quantityInWarehouse = ModelThongTinChiTiet.GetQuantityForProTMDT(Common.commerceNameTiki, proId);
            if (quantity == quantityInWarehouse)// ||  // Số lượng trên sàn và trong kho đã đúng
                //(quantityInWarehouse == -1 && quantity == 0)) // Không lấy được số lượng trong kho và số lượng trên sàn đang là 0
                return false;

            return true;
        }

        /// <summary>
        /// Từ id sản phẩm trên sàn cập nhật số lượng sản phẩm trên sàn theo số lượng trong kho
        /// </summary>
        /// <param name="proId"></param>
        /// <param name="quantity">Số lượng thực tế trên sàn. Nếu = -1 Không check số lượng trên sàn TMDT và số lượng trong kho có khác nhau.</param>
        /// <returns></returns>
        public static Boolean TikiProductUpdateQuantity(int proId, long quantity)
        {
            // Check số lượng trên sàn TMDT và số lượng trong kho có khác nhau
            if(quantity != -1)
            {
                if (!NeedUpdateQuantityOfProTMDT(proId.ToString(), quantity))
                {
                    //// Cập nhật vào db ModelTonKhoSanTMDT
                    //ModelTonKhoSanTMDT.SynchronizeThongTinChiTietToTonKhoSanTMDT(proId.ToString(), EnumCommerceType.TIKI);
                    //ModelMappingSanPhamTMDT_SanPhamKho.Tiki_UpdateQuantityOnTMDT(proId.ToString(), quantity);
                    return true;
                }
            }

            // Cập nhật số lượng trên sàn Tiki
            //List<TikiConfigApp> l = ModelThongTinBaoMatTiki.Tiki_InhouseAppGetListUsingApp();
            //TikiConfigApp configApp = l[0];

            TikiUpdateQuantity st = new TikiUpdateQuantity(proId, TikiConstValues.intIdKho28Ngo3TTDL);
            // Cập nhật tồn kho cho tham số
            int qty = 0;
            //qty = ModelThongTinChiTiet.GetQuantityForProTMDT(Common.commerceNameTiki, proId.ToString());
            //if (qty == -1)
            //{
            //    MyLogger.GetInstance().Info("Số lượng tồn kho của mã sản phẩm " + proId.ToString() + " là -1");

            //    // Ta set số lượng sản phẩm trên sàn về 0
            //    qty = 0;
            //}

            st.UpdateQuantity(qty);
            Boolean isOk = TikiProductUpdateQuantity(GetConfigApp(), st);
            if (!isOk)
            {
                //// Lưu vào db update mã sản phẩm lỗi
                //ModelUpdateStockFailure.Tiki_Update(proId.ToString());
                MyLogger.GetInstance().Info("Tiki sản phẩm cập nhật lỗi: " + proId.ToString());
                return false;
            }

            //// Cập nhật vào db ModelTonKhoSanTMDT
            //ModelTonKhoSanTMDT.SynchronizeThongTinChiTietToTonKhoSanTMDT(proId.ToString(), EnumCommerceType.TIKI);
            //ModelMappingSanPhamTMDT_SanPhamKho.Tiki_UpdateQuantityOnTMDT(proId.ToString(), qty);
            Thread.Sleep(200);
            return true;
        }

        /// <summary>
        /// Set giá bán thực tế cho id sản phẩm
        /// </summary>
        /// <param name="proId"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public static Boolean TikiProductUpdatePrice(int proId, int price)
        {

            TikiUpdatePrice st = new TikiUpdatePrice(proId);
            // Cập nhật tồn kho cho tham số

            st.UpdatePrice(price);
            Boolean isOk = TikiProductUpdatePrice(GetConfigApp(), st);
            if (!isOk)
            {
                MyLogger.GetInstance().Info("Tiki sản phẩm cập nhật giá bán lỗi: " + proId.ToString());
                return false;
            }

            Thread.Sleep(200);
            return true;
        }

        /// <summary>
        /// Set giá bán thực tế cho id sản phẩm
        /// </summary>
        /// <param name="proId"></param>
        /// <param name="status">enum {1: enabled, 2: disabled, 3: hided}</param>
        /// <returns></returns>
        public static Boolean TikiProductUpdateStatus(int proId, int status)
        {

            TikiUpdateStatus st = new TikiUpdateStatus(proId);
            // Cập nhật tồn kho cho tham số

            st.UpdateStatus(status);
            Boolean isOk = TikiProductUpdateStatus(GetConfigApp(), st);
            if (!isOk)
            {
                MyLogger.GetInstance().Info("Tiki sản phẩm cập nhật trạng thái lỗi: " + proId.ToString());
                return false;
            }

            Thread.Sleep(200);
            return true;
        }

        ///// <summary>
        ///// Từ danh sách mã sản phẩm trong kho / số lượng tồn ta cập nhật số lượng tồn sản phẩm trên sàn TMDT
        ///// </summary>
        ///// <param name="dic">danh sách mã sản phẩm trong kho / số lượng tồn</param>
        ///// <returns></returns>
        //static public Boolean TikiUpdateTonKhoSanTMDT(Dictionary<string, int> dic)
        //{
        //    if (dic == null)
        //    {
        //        Common.CommonErrorMessage = "Danh sách sản phẩm / số lượng tồn kho null";
        //        MyLogger.GetInstance().Info(Common.CommonErrorMessage);
        //        return false;
        //    }
        //    List<String> lsItem = new List<String>();
        //    foreach (var obj in dic)
        //    {
        //        List<String> lsItemTemp = ModelMappingSanPhamTMDT_SanPhamKho.Tiki_GetIdSPTMDMappingFromIdInWarehouse(obj.Key);
        //        foreach (var eItemTemp in lsItemTemp)
        //        {
        //            Boolean isExist = false;
        //            foreach (var eItem in lsItem)
        //            {
        //                if (eItem == eItemTemp)
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
        //    List<string> lsItemFailed = ModelUpdateStockFailure.Tiki_GetListId();

        //    // Gộp danh sách
        //    foreach (var eItemFailed in lsItemFailed)
        //    {
        //        Boolean isExist = false;
        //        foreach (var eItem in lsItem)
        //        {
        //            if (eItem == eItemFailed)
        //            {
        //                isExist = true;
        //                break;
        //            }
        //        }
        //        if (!isExist)
        //            lsItem.Add(eItemFailed);
        //    }

        //    // Tham số phục vụ update stock
        //    List<TikiUpdateQuantity> lsStock = new List<TikiUpdateQuantity>();
        //    foreach (var e in lsItem)
        //    {

        //        // Check đã tồn tại
        //        Boolean isExist = false;
        //        foreach (var eStock in lsStock)
        //        {
        //            if (eStock.product_id.ToString() == e)
        //            {
        //                isExist = true;
        //                break;
        //            }
        //        }
        //        if (!isExist)
        //        {
        //            lsStock.Add(new TikiUpdateQuantity(Common.ConvertStringToInt32(e), TikiConstValues.intIdKho28Ngo3TTDL));
        //        }
        //    }

        //    // Cập nhật tồn kho cho tham số
        //    int quantity = 0;
        //    foreach (var eStock in lsStock)
        //    {
        //        quantity = ModelTonKhoSanTMDT.GetQuantityOnTMDT(Common.commerceNameTiki, eStock.product_id.ToString());
        //        if (quantity != -1)
        //        {
        //            eStock.UpdateQuantity( quantity);
        //        }
        //        continue;
        //    }

        //    //List<TikiConfigApp> l = ModelThongTinBaoMatTiki.Tiki_InhouseAppGetListUsingApp();
        //    //TikiConfigApp configApp = l[0];

        //    // Lưu item update lỗi
        //    List<string> lsUpdateFailed = new List<string>();
        //    foreach (var eStock in lsStock)
        //    {
        //        Boolean isOk = TikiProductUpdateQuantity(GetConfigApp(), eStock);
        //        if (!isOk)
        //        {
        //            lsUpdateFailed.Add(eStock.product_id.ToString());
        //        }
        //    }

        //    // Lưu vào db
        //    if (lsUpdateFailed.Count() > 0)
        //        ModelUpdateStockFailure.Tiki_Update(lsUpdateFailed);

        //    return true;
        //}

        /// <summary>
        /// Từ đơn hàng mới trong khoảng thời gian, ta lấy được số lượng hàng cần xuất
        /// </summary>
        /// <param name="time_from"></param>
        /// <param name="time_to"></param>
        /// <returns></returns>
        static public Dictionary<string, int> TikiGetProductQuantityPairToTake(DateTime time_from, DateTime time_to)
        {
            //List<TikiConfigApp> l = ModelThongTinBaoMatTiki.Tiki_InhouseAppGetListUsingApp();
            //TikiConfigApp configApp = l[0];

            List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order.TikiOrder> lsOrderTikiFullInfo;
            lsOrderTikiFullInfo = MVCPlayWithMe.OpenPlatform.API.TikiAPI.Order.TikiGetListOrders.GetListOrderAShop(GetConfigApp(),
                MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order.TikiOrderItemFilterByDate.EnumOrderItemFilterByDate.last7days);
            List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order.TikiOrder> lsTem = new List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order.TikiOrder>();
            foreach(var e in lsOrderTikiFullInfo)
            {
                if(e.created_at >= time_from && e.created_at <= time_to)
                {
                    lsTem.Add(e);
                }
            }
            lsOrderTikiFullInfo = lsTem;
            Dictionary<string, int> dic = TikiGetListOrders.TikiGetDictionaryOfProductQuantityFromListDetailOrder(lsOrderTikiFullInfo);

            return dic;
        }
    }
}
