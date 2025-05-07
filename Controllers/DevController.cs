using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.Dev;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Event;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Product;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static MVCPlayWithMe.General.Common;

namespace MVCPlayWithMe.Controllers
{
    public class DevController : BasicController
    {
        public DevMySql sqler { get; set; }

        public DevController() : base()
        {
            sqler = new DevMySql();
        }

        // GET: Dev
        public ActionResult Index()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }

            return View();
        }
        [HttpPost]
        public string CopyShopeeProductImageToProduct()
        {
            ShopeeMySql shopeeSqler = new ShopeeMySql();
            return string.Empty;
        }

        [HttpPost]
        public string ShopeeSaveImageSourceOfItemAndModel()
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            List<CommonItem> lsCommonItem = new List<CommonItem>();

            ShopeeMySql shopeeSqler = new ShopeeMySql();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
                    List<long> lsItem = shopeeSqler.GetForSaveImageSourceConnectOut(conn);
                    foreach (var itemId in lsItem)
                    {
                        ShopeeGetItemBaseInfoItem pro =
                            ShopeeGetItemBaseInfo.ShopeeProductGetItemBaseInfoFromId(itemId);
                        if (pro != null)
                        {
                            lsCommonItem.Add(new CommonItem(pro));
                        }
                    }

                    shopeeSqler.UpdateImageSourceTotbShopeeItem_ModelConnectOut(lsCommonItem, conn);
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string TikiSaveImageSourceOfItemAndModel()
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            List<CommonItem> lsCommonItem = new List<CommonItem>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
                    TikiMySql tikiSqler = new TikiMySql();
                    List<int> lsItemId = tikiSqler.GetForSaveImageSourceConnectOut(conn);

                    foreach (var itemId in lsItemId)
                    {
                        TikiProduct pro = GetListProductTiki.GetProductFromOneShop(itemId);
                        if (pro == null)
                        {
                            continue;
                        }

                        lsCommonItem.Add(new CommonItem(pro));
                    }
                    tikiSqler.UpdateImageSourceTotbTikiItemConnectOut(lsCommonItem, conn);
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string TikiTestPullEvent()
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();

            //TikiPullEventService tikiPullEventService = new TikiPullEventService();
            //tikiPullEventService.DoWork();

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string TikiTestSomething()
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            conn.Open();
            CommonOrder commonOrder = new CommonOrder();
            commonOrder.code = "ABCDEDFTest";
            commonOrder.listItemId.Add(1000);
            commonOrder.listItemId.Add(2000);

            commonOrder.listModelId.Add(1);
            commonOrder.listModelId.Add(2);

            commonOrder.listQuantity.Add(1);
            commonOrder.listQuantity.Add(1);

            TikiMySql tikiSqler = new TikiMySql();
            tikiSqler.InsertTbItemOfEcommerceOder(commonOrder, EECommerceType.TIKI, conn);

            tikiSqler.InsertTbItemOfEcommerceOder(commonOrder, EECommerceType.SHOPEE, conn);

            tikiSqler.UpdateCancelledStatusTbItemOfEcommerceOder(commonOrder, EECommerceType.TIKI, conn);

            tikiSqler.UpdateCancelledStatusTbItemOfEcommerceOder(commonOrder, EECommerceType.SHOPEE, conn);
            conn.Close();

            return JsonConvert.SerializeObject(result);
        }

        ///// <summary>
        //// Hàm chỉ gọi 1 lần trong đời do thư mục ảnh chưa được add logo, thêm phiên bản thu nhỏ 320
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public string AddWaterMarkAllExistImage()
        //{
        //    //MyLogger.GetInstance().Debug("AddWaterMarkAllExistImage CALL");
        //    MySqlResultState rss = new MySqlResultState();
        //    if (AuthentAdministrator() == null)
        //    {
        //        rss.State = EMySqlResultState.AUTHEN_FAIL;
        //        rss.Message = "Xác thực thất bại.";
        //        return JsonConvert.SerializeObject(rss);
        //    }

        //    // Thư mục Media\Item
        //    Common.AddWatermark_DeleteOriginalImageFunc_ReduceSize_Folder(
        //        System.Web.HttpContext.Current.Server.MapPath(Common.ItemMediaFolderPath));

        //    // Thư mục Media\Product
        //    Common.AddWatermark_DeleteOriginalImageFunc_ReduceSize_Folder(
        //        System.Web.HttpContext.Current.Server.MapPath(Common.ProductMediaFolderPath));
        //    return JsonConvert.SerializeObject(rss);
        //}

        [HttpPost]
        public string DeleteDuplicateDataOftbShopeeModel()
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(sqler.DeleteDuplicateDataOftbShopeeModel());
        }

        [HttpPost]
        public string ShopeeGetAuthorizationURL()
        {
            MySqlResultState result = new MySqlResultState();
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            result.Message = CommonShopeeAPI.ShopeeGenerateAuthPartnerUrl();

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string ShopeeSaveLivePartnerKey(string key)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = sqler.ShopeeSaveLivePartnerKey(key);

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string ShopeeSaveCode(string code)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = sqler.ShopeeSaveCode(code);

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string ShopeeGetTokenShopLevelAfterAuthorization()
        {
            MySqlResultState result = new MySqlResultState();
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            if(CommonShopeeAPI.ShopeeGetTokenShopLevel() == null)
            {
                result.State = EMySqlResultState.ERROR;
            }

            return JsonConvert.SerializeObject(result);
        }
    }
}