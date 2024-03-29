﻿using MVCPlayWithMe.General;
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
using System.Web.Mvc;
using static MVCPlayWithMe.General.Common;
using static MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order.TikiOrderItemFilterByDate;

namespace MVCPlayWithMe.Controllers.OpenPlatform
{
    public class ProductECommerceController : BasicController
    {
        public ShopeeMySql shopeeSqler;
        public TikiMySql tikiSqler;

        public ProductECommerceController ()
        {
            shopeeSqler = new ShopeeMySql();
            tikiSqler = new TikiMySql();

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

            ViewDataGetListProductName();
            ViewDataGetListCombo();

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
        public string GetProductFromId(string eType, string id)
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
            lsShopeeItem = ShopeeGetItemBaseInfo.ShopeeProductGetItemBaseInfo_PageFisrst();
            foreach(var pro in lsShopeeItem)
            {
                CommonItem item = new CommonItem(pro);
                shopeeSqler.ShopeeGetItemFromId(pro.item_id, item);
                lsCommonItem.Add(item);
            }
            return lsCommonItem;
        }

        List<CommonItem> TikiGetProductAll()
        {
            List<CommonItem> lsCommonItem = new List<CommonItem>();
            List<TikiProduct> lsTikiItem = new List<TikiProduct>();
            lsTikiItem = GetListProductTiki.GetListLatestProductsFromOneShop();
            foreach (var pro in lsTikiItem)
            {
                CommonItem item = new CommonItem(pro);
                tikiSqler.TikiGetItemFromId(pro.product_id, item);
                lsCommonItem.Add(item);
            }
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
                    ls[ls.Count - 1].lsProductId.Add(Common.ConvertStringToInt32(values[i + 2]));
                    ls[ls.Count - 1].lsProductQuantity.Add(Common.ConvertStringToInt32(values[i + 3]));
                }
                else
                {
                    CommonForMapping commonForMapping = new CommonForMapping();
                    commonForMapping.itemId = Common.ConvertStringToInt64(values[i]);
                    commonForMapping.modelId = Common.ConvertStringToInt64(values[i + 1]);
                    commonForMapping.lsProductId.Add(Common.ConvertStringToInt32(values[i + 2]));
                    commonForMapping.lsProductQuantity.Add(Common.ConvertStringToInt32(values[i + 3]));

                    ls.Add(commonForMapping);
                }
            }
            result = UpdateMapping(eType, ls);
            return JsonConvert.SerializeObject(result);
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
                ShopeeMySql shopeeMySql = new ShopeeMySql();
                shopeeMySql.ShopeeUpdateMappingToCommonOrder(order);
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
        #endregion
    }
}