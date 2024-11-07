using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.Dev;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Product;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product;
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

        [HttpPost]
        public string ShopeeSaveImageSourceOfItemAndModel()
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            ShopeeMySql shopeeSqler = new ShopeeMySql();
            List<CommonItem> lsCommonItem = shopeeSqler.GetForSaveImageSource();

            try
            {
                foreach (var item in lsCommonItem)
                {
                    ShopeeGetItemBaseInfoItem pro =
                        ShopeeGetItemBaseInfo.ShopeeProductGetItemBaseInfoFromId(item.itemId);
                    if (pro != null)
                    {
                        item.imageSrc = pro.image.image_url_list[0];

                        // Lấy imageSrc cho model nếu có
                        if (pro.has_model)
                        {
                            ShopeeGetModelListResponse obj =
                                ShopeeGetModelList.ShopeeProductGetModelList(pro.item_id);
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

                                }
                            }
                        }
                        else
                        {
                            item.models[0].imageSrc = item.imageSrc;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            shopeeSqler.UpdateSourceTotbShopeeItem_Model(lsCommonItem);
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
            TikiMySql tikiSqler = new TikiMySql();
            List<CommonItem> lsCommonItem = tikiSqler.GetForSaveImageSource();

            try
            {
                foreach (var item in lsCommonItem)
                {
                    TikiProduct pro = GetListProductTiki.GetProductFromOneShop((int)item.itemId);
                    if (pro == null)
                        continue;

                    item.imageSrc = pro.thumbnail;
                    if (string.IsNullOrEmpty(item.imageSrc))
                    {
                        if(pro.images != null && pro.images.Count() > 0)
                        {
                            item.imageSrc = pro.images[0].url;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            tikiSqler.UpdateSourceTotbTikiItem(lsCommonItem);
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