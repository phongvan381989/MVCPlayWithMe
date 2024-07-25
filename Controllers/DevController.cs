using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.Dev;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI;
using MVCPlayWithMe.OpenPlatform.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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