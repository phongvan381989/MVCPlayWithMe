using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.ItemModel;
using MVCPlayWithMe.Models.Order;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeOrder;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Order;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Product;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using static MVCPlayWithMe.General.Common;
using static MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order.TikiOrderItemFilterByDate;

namespace MVCPlayWithMe.Controllers.OpenPlatform
{
    public class ProductECommerceController : BasicController
    {
        public ShopeeMySql shopeeSqler;
        public TikiMySql tikiSqler;
        public ItemModelMySql itemModelSqler;

        public ProductECommerceController ()
        {
            shopeeSqler = new ShopeeMySql();
            tikiSqler = new TikiMySql();
            itemModelSqler = new ItemModelMySql();

        }
        #region Xử lý item
        // GET: ProductECommerce
        public ActionResult Index()
        {
            if (AuthentAdministrator() == null)
                return View("~/Views/Administrator/Login.cshtml");

            return View();
        }

        [HttpGet]
        public ActionResult Item(string eType, string id)
        {
            if (AuthentAdministrator() == null)
                return View("~/Views/Administrator/Login.cshtml");

            //ViewDataGetListProductName();
            //ViewDataGetListCombo();

            return View();
        }

        [HttpPost]
        public string GetProductAll(string eType)
        {
            List<CommonItem> lsCommonItem = null;
            if (AuthentAdministrator() == null)
                return JsonConvert.SerializeObject(lsCommonItem);

            if (eType == Common.eShopee)
            {
                lsCommonItem = ShopeeGetProductAll();
            }
            else if(eType == Common.eTiki)
            {
                lsCommonItem = TikiGetProductAll();
            }
            return JsonConvert.SerializeObject(lsCommonItem);
        }

        [HttpPost]
        public string GetItemFromId(string eType, string id)
        {
            CommonItem commonItem = null;
            if (AuthentAdministrator() == null)
                return JsonConvert.SerializeObject(commonItem);

            if (eType == Common.eShopee)
            {
                long lid = Common.ConvertStringToInt64(id);
                if (lid != Int64.MinValue)
                {
                    commonItem = ShopeeGetItemFromId(lid);
                }
            }
            else if (eType == Common.eTiki)
            {
                int iid = Common.ConvertStringToInt32(id);
                if (iid != Int32.MinValue)
                {
                    commonItem = TikiGetProductFromId(iid);
                }
            }

            return JsonConvert.SerializeObject(commonItem);
        }

        List<CommonItem> ShopeeGetProductAll()
        {
            List<CommonItem> lsCommonItem = new List<CommonItem>();
            List<ShopeeGetItemBaseInfoItem> lsShopeeItem = new List<ShopeeGetItemBaseInfoItem>();

            // Lấy toàn bộ sản phẩm Shopee mất thời gian, giai đoạn test chỉ lấy 1 page ~ 50 sản phẩm
            lsShopeeItem = ShopeeGetItemBaseInfo.ShopeeProductGetItemBaseInfoAll();
            //lsShopeeItem = ShopeeGetItemBaseInfo.ShopeeProductGetItemBaseInfo_PageFisrst();

            shopeeSqler.ShopeeGetListCommonItemFromListShopeeItem(lsShopeeItem, lsCommonItem);

            return lsCommonItem;
        }

        List<CommonItem> TikiGetProductAll()
        {
            List<CommonItem> lsCommonItem = new List<CommonItem>();
            List<TikiProduct> lsTikiItem = new List<TikiProduct>();
            lsTikiItem = GetListProductTiki.GetListLatestProductsFromOneShop();

            tikiSqler.TikiGetListCommonItemFromListTikiProduct(lsTikiItem, lsCommonItem);
            return lsCommonItem;
        }

        private CommonItem ShopeeGetItemFromId(long id)
        {
            ShopeeGetItemBaseInfoItem pro = ShopeeGetItemBaseInfo.ShopeeProductGetItemBaseInfoFromId(id);
            if (pro == null)
                return null;

            CommonItem item = new CommonItem(pro);
            shopeeSqler.ShopeeInsertIfDontExist(item);
            shopeeSqler.ShopeeGetItemFromId(id, item);
            return item;
        }

        private MySqlResultState UpdateMapping(string eType, List<CommonForMapping> ls)
        {
            MySqlResultState result = null;
            if (eType == Common.eShopee)
            {
                result = shopeeSqler.ShopeeUpdateMapping(ls);
            }
            else if(eType == Common.eTiki)
            {
                result = tikiSqler.TikiUpdateMapping(ls);
            }

            return result;
        }

        private CommonItem TikiGetProductFromId(int id)
        {
            TikiProduct pro = null;
            pro = GetListProductTiki.GetProductFromOneShop(id);
            if (pro == null)
                return null;

            CommonItem item = new CommonItem(pro);
            tikiSqler.TikiInsertIfDontExist(item);
            tikiSqler.TikiGetItemFromId(id, item);
            return item;
        }

        /// <summary>
        /// str có dạng: itemId,modelId,productId,productQuantity,...,itemId,modelId,productId,productQuantity
        /// model chưa được mapping có dạng: itemId,modelId,,,
        /// với tiki: itemId là
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [HttpPost]
        public string UpdateMapping(string eType, string str)
        {
            MySqlResultState result;
            if (AuthentAdministrator() == null)
            {
                result = new MySqlResultState();
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = "Xác thực thất bại";
                return JsonConvert.SerializeObject(result);
            }
            List<CommonForMapping> ls = new List<CommonForMapping>();
            string[] values = str.Split(',');
            int length = values.Length;
            for(int i = 0; i < length; i = i + 4)
            {
                // Nếu model chưa được mapping productId, productQuantity là: System.Int32.MinValue;
                if (ls.Count > 0 && ls[ls.Count - 1].modelId == Common.ConvertStringToInt64(values[i + 1]))
                {
                    if (!string.IsNullOrEmpty(values[i + 2]) && !string.IsNullOrEmpty(values[i + 3]))
                    {
                        ls[ls.Count - 1].lsProductId.Add(Common.ConvertStringToInt32(values[i + 2]));
                        ls[ls.Count - 1].lsProductQuantity.Add(Common.ConvertStringToInt32(values[i + 3]));
                    }
                }
                else
                {
                    CommonForMapping commonForMapping = new CommonForMapping();
                    commonForMapping.itemId = Common.ConvertStringToInt64(values[i]);
                    commonForMapping.modelId = Common.ConvertStringToInt64(values[i + 1]);
                    if (!string.IsNullOrEmpty(values[i + 2]) && !string.IsNullOrEmpty(values[i + 3]))
                    {
                        commonForMapping.lsProductId.Add(Common.ConvertStringToInt32(values[i + 2]));
                        commonForMapping.lsProductQuantity.Add(Common.ConvertStringToInt32(values[i + 3]));
                    }

                    ls.Add(commonForMapping);
                }
            }
            result = UpdateMapping(eType, ls);
            return JsonConvert.SerializeObject(result);
        }

        [HttpGet]
        public ActionResult ItemOnDB()
        {
            if (AuthentAdministrator() == null)
                return View("~/Views/Administrator/Login.cshtml");

            return View();
        }

        [HttpPost]
        public string GetItemOnDB(string eType)
        {
            List<CommonItem> ls = null;
            if (eType == Common.eShopee)
            {
                ls = shopeeSqler.ShopeeGetItemOnDB();
            }
            else if (eType == Common.eTiki)
            {
                ls = tikiSqler.TikiGetItemOnDB();
            }
            return JsonConvert.SerializeObject(ls);
        }

        // Xóa item trên db
        [HttpPost]
        public string DeleteItemOnDB(string eType, string itemId)
        {
            MySqlResultState resultState = null;
            if (eType == Common.eShopee)
            {
                long id = Common.ConvertStringToInt64(itemId);
                resultState = shopeeSqler.ShopeeDeleteItemOnDB(id);
            }
            else if (eType == Common.eTiki)
            {
                int id = Common.ConvertStringToInt32(itemId);
                resultState = tikiSqler.TikiDeleteItemOnDB(id);
            }
            return JsonConvert.SerializeObject(resultState);
        }

        // Xóa model Shopee trên db
        [HttpPost]
        public string DeleteShopeeModelOnDB(string eType, string modelId)
        {
            MySqlResultState resultState = null;
            if (eType == Common.eShopee)
            {
                long id = Common.ConvertStringToInt64(modelId);
                resultState = shopeeSqler.ShopeeDeleteModelOnDB(id);
            }
            return JsonConvert.SerializeObject(resultState);
        }
        #endregion

        #region Xử lý đơn hàng
        [HttpGet]
        public ActionResult Order()
        {
            if (AuthentAdministrator() == null)
                return View("~/Views/Administrator/Login.cshtml");

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromTo"> 0: 1 ngày, 1: 7 ngày, 2: 30 ngày</param>
        /// <returns></returns>
        [HttpPost]
        public string GetListOrder(int fromTo)
        {
            List<CommonOrder> lsCommonOrder = new List<CommonOrder>();
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(lsCommonOrder);
            }

            // Lấy đơn hàng tiki
            List<TikiOrder> lsOrderTikiFullInfo;
            lsOrderTikiFullInfo = TikiGetListOrders.GetListOrderAShop((EnumOrderItemFilterByDate)fromTo);
            if(lsOrderTikiFullInfo != null)
            {
                foreach(var order in lsOrderTikiFullInfo)
                {
                    lsCommonOrder.Add(new CommonOrder(order));
                }
            }

            // Lấy đơn hàng của Shopee
            List<ShopeeOrderDetail> lsOrderShopeeFullInfo;
            DateTime time_from, time_to;
            time_from = DateTime.Now;
            time_to = DateTime.Now;
            if ((EnumOrderItemFilterByDate)fromTo == EnumOrderItemFilterByDate.today)
                time_from = time_to.AddDays(-1);
            else if ((EnumOrderItemFilterByDate)fromTo == EnumOrderItemFilterByDate.last7days)
                time_from = time_to.AddDays(-7);
            else if ((EnumOrderItemFilterByDate)fromTo == EnumOrderItemFilterByDate.last30days)
                time_from = time_to.AddDays(-30);
            lsOrderShopeeFullInfo = ShopeeGetOrderDetail.ShopeeOrderGetOrderDetailAll(time_from, time_to, new ShopeeOrderStatus());
            if (lsOrderShopeeFullInfo != null)
            {
                foreach (var order in lsOrderShopeeFullInfo)
                {
                    lsCommonOrder.Add(new CommonOrder(order));
                }
            }

            // Lấy đơn hàng của web Play With Me
            OrderMySql orderMySql = new OrderMySql();
            lsCommonOrder.AddRange(orderMySql.GetListCommonOrder(fromTo));

            // Cập nhật trạng thái đơn hàng: đã đóng / đã hoàn
            TikiMySql tikiMySql = new TikiMySql();
            tikiMySql.UpdateOrderStatusInWarehouseToCommonOrder(lsCommonOrder);

            return JsonConvert.SerializeObject(lsCommonOrder);
        }

        /// <summary>
        /// Load lại mapping, sản phẩm trong 1 order
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string ReloadOneOrder(string commonOrder)
        {
            CommonOrder order = JsonConvert.DeserializeObject<CommonOrder>(commonOrder);
            order.listMapping = new List<List<Models.Mapping>>(); // Reset để cập nhật lại

            if (order.ecommerceName == eTiki)
            {
                TikiMySql tikiMySql = new TikiMySql();
                tikiMySql.TikiUpdateMappingToCommonOrder(order);
            }
            else if (order.ecommerceName == eShopee)
            {
                shopeeSqler.ShopeeUpdateMappingToCommonOrder(order);
            }
            else if (order.ecommerceName == ePlayWithMe)
            {
                OrderMySql orderMySql = new OrderMySql();
                orderMySql.UpdateMappingToCommonOrder(order);
            }

            return JsonConvert.SerializeObject(order);
        }

        [HttpPost]
        public string EnoughProductInOrder(string eType, string commonOrder)
        {
            MySqlResultState resultState = null;
            CommonOrder order = null;
            try
            {
                order = JsonConvert.DeserializeObject<CommonOrder>(commonOrder);
            }
            catch(Exception ex)
            {
                resultState = new MySqlResultState();
                Common.SetResultException(ex, resultState);
                return JsonConvert.SerializeObject(resultState);
            }

            EECommerceType eECommerceType = EECommerceType.TIKI;
            if (eType == Common.eShopee)
            {
                eECommerceType = EECommerceType.SHOPEE;
            }
            else if (eType == Common.eTiki)
            {
                eECommerceType = EECommerceType.TIKI;
            }
            else if (eType == Common.ePlayWithMe)
            {
                eECommerceType = EECommerceType.PLAY_WITH_ME;
            }
            TikiMySql tikiMySql = new TikiMySql();
            resultState = tikiMySql.EnoughProductInOrder(order,  ECommerceOrderStatus.PACKED, eECommerceType);

            return JsonConvert.SerializeObject(resultState);
        }

        [HttpPost]
        public string ReturnedOrder(string eType, string commonOrder)
        {
            MySqlResultState resultState = null;

            CommonOrder order = null;
            try
            {
                order = JsonConvert.DeserializeObject<CommonOrder>(commonOrder);
            }
            catch (Exception ex)
            {
                resultState = new MySqlResultState();
                Common.SetResultException(ex, resultState);
                return JsonConvert.SerializeObject(resultState);
            }

            EECommerceType eECommerceType = EECommerceType.TIKI;
            if (eType == Common.eShopee)
            {
                eECommerceType = EECommerceType.SHOPEE;
            }
            else if (eType == Common.eTiki)
            {
                eECommerceType = EECommerceType.TIKI;
            }
            else if (eType == Common.ePlayWithMe)
            {
                eECommerceType = EECommerceType.PLAY_WITH_ME;
            }
            TikiMySql tikiMySql = new TikiMySql();
            resultState = tikiMySql.ReturnedOrder(order, ECommerceOrderStatus.RETURNED, eECommerceType);

            return JsonConvert.SerializeObject(resultState);
        }

        /// <summary>
        /// Lưu ảnh/video item shopee vào item voibenho.Xóa ảnh/video của item voibenho nếu đã tồn tại
        /// Không xóa ảnh của model
        /// </summary>
        /// <param name="commonItem"></param>
        /// <param name="vbnItemId"></param>
        private void SaveShopeeItemMediaToVoiBeNhoItem(CommonItem commonItem, int vbnItemId)
        {
            string path = Common.GetAbsoluteItemMediaFolderPath(vbnItemId);
            if(path == null)
            {
                path = Common.CreateAbsoluteItemMediaFolderPath(vbnItemId);
            }
            else // Xóa ảnh/video của item voibenho nếu đã tồn tại. Không xóa ảnh của model
            {
                Common.DeleteAllMediaFileInclude320(path);
            }

            // Tải ảnh
            int i = 0;
            foreach(var s in commonItem.imageSrcList)
            {
                Common.DownloadImageAddWaterMarkAndReduce(s, Path.Combine(path, i.ToString() + ".jfif"));
                i++;
            }

            // Tải video nếu có
            if(!string.IsNullOrEmpty(commonItem.videoSrc))
                Common.DownloadVideoAndSaveWithName(commonItem.videoSrc, Path.Combine(path, "0.mp4"));
        }

        /// <summary>
        /// Từ model sản phẩm trên sàn Shopee sinh model trên web voibenho
        /// Nếu trên web voibenho chưa có item tương ứng ta sinh item trước rồi sinh model sau
        /// </summary>
        /// <param name="strCommonItem"> đối tượng commmon item dạng string json</param>
        /// <param name="shopeeModelId"> shopee model id của sản phẩm cần sinh trên web voibenho</param>
        /// <param name="pWMMappingModelId"> model id của sản phẩm mappin gtrên web voibenho.
        /// 0: nếu chưa tồn tại</param>
        /// <returns></returns>
        [HttpPost]
        public string ShopeeBornModelForVoiBeNho(string strCommonItem, long shopeeModelId, int pWMMappingModelId)
        {
            MySqlResultState resultState = null;
            if (AuthentAdministrator() == null)
            {
                resultState = new MySqlResultState();
                resultState.State = EMySqlResultState.AUTHEN_FAIL;
                resultState.Message = "Xác thực thất bại";
                return JsonConvert.SerializeObject(resultState);
            }

            CommonItem commonItem = JsonConvert.DeserializeObject<CommonItem>(strCommonItem);

            // Check xem item đã được sinh ra trên voibenho
            int itemId = 0;
            itemId = itemModelSqler.GetItemIdFromName(commonItem.name);

            // Chưa sinh item tương ứng trên web voibenho.
            if (itemId <= 0)
            {
                // Sinh item trên web voibenho
                int status = 0;
                if (commonItem.item_status == "NORMAL")
                    status = 0;
                else
                    status = 1;
                itemId = itemModelSqler.AddItem(commonItem.name, status, commonItem.detail);

                // Lưu ảnh vào thư mục \Media\Item\ItemId\
                SaveShopeeItemMediaToVoiBeNhoItem(commonItem, itemId);
            }
            else
            {
                string path = Common.GetAbsoluteItemMediaFolderPath(itemId);
                if (path == null) // Chưa lưu image/video của item
                {
                    SaveShopeeItemMediaToVoiBeNhoItem(commonItem, itemId);
                }
            }

            // Lấy được đối tượng common model shopee
            CommonModel commonModel = null;
            foreach(var m in commonItem.models)
            {
                if(m.modelId == shopeeModelId)
                {
                    commonModel = m;
                    break;
                }
            }

            // Nếu model đã có trên voibenho, xóa dữ liệu ở tbMapping, tbpwmmappingother, tbModel
            // Từ giá bìa, giá bán tính toán chiết khấu làm tròn , giá bán theo chiết khấu
            int discount = 100 - commonModel.price * 100/ commonModel.market_price;
            int price = (100 - discount) * commonModel.market_price / 100;
            //price = price / 1000 * 1000; // Lấy đơn vị tròn 1000 vnđ
            resultState = itemModelSqler.BornModelFromShopeeModel(itemId, pWMMappingModelId,
                commonModel.name, 5, discount, price, commonModel.market_price, commonItem.itemId, commonModel.modelId);

            if(resultState.State != EMySqlResultState.OK)
            {
                return JsonConvert.SerializeObject(resultState);
            }
            // Xóa dữ liệu media cũ ở Media\Item\itemId\Model nếu có
            Common.DeleteImageModelInclude320(itemId, pWMMappingModelId);

            // Thêm dữ liệu media ở Media\Item\itemId\Model
            // Lấy model Id
            int newModelId = 0;
            newModelId = resultState.myAnything;
            // Thêm dữ liệu media  ở Media\Item\itemId\Model nếu có
            {
                string path = Common.GetAbsoluteModelMediaFolderPath(itemId);
                if(path == null)
                {
                    path = Common.CreateAbsoluteModelMediaFolderPath(itemId);
                }

                DownloadImageAddWaterMarkAndReduce(commonModel.imageSrc, Path.Combine(path,
                    newModelId.ToString() + ".jfif"));
            }

            // Insert dữ liệu cho tbMapping từ mapping của model shopee
            if (commonModel.mapping.Count > 0)
            {
                List<int> mappingOnlyProductId = new List<int>();
                List<int> mappingOnlyQuantity = new List<int>();

                foreach(var m in commonModel.mapping)
                {
                    mappingOnlyProductId.Add(m.product.id);
                    mappingOnlyQuantity.Add(m.quantity);
                }
                resultState = itemModelSqler.UpdateMapping(newModelId, mappingOnlyProductId, mappingOnlyQuantity);
            }

            return JsonConvert.SerializeObject(resultState);
        }
        #endregion
    }
}