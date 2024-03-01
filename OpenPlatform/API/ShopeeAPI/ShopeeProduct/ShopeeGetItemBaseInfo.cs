using Newtonsoft.Json;
using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeProduct
{
    public class ShopeeGetItemBaseInfo
    {
        /// <summary>
        /// Lấy base info
        /// </summary>
        /// <param name="ls"> Chứa id item cần lấy base info</param>
        /// <returns>null nếu không lấy thành công</returns>
        public static ShopeeGetItemBaseInfoResponseHTTP ShopeeProductGetItemBaseInfo(List<DevNameValuePair> ls)
        {
            string path = "/api/v2/product/get_item_base_info";

            IRestResponse response = CommonShopeeAPI.ShopeeGetMethod(path, ls);
            if (response == null)
                return null;
            ShopeeGetItemBaseInfoResponseHTTP objResponse = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    objResponse = JsonConvert.DeserializeObject<ShopeeGetItemBaseInfoResponseHTTP>(response.Content, settings);
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.Message);
                    return null;
                }
            }

            return objResponse;
        }

        /// <summary>
        /// Lấy base info của tất cả item
        /// </summary>
        /// <returns>null nếu không lấy thành công</returns>
        public static List<ShopeeGetItemBaseInfoItem> ShopeeProductGetItemBaseInfoAll()
        {
            // Lấy danh sách id của item
            List<ShopeeGetItemBaseInfoItem> rs = new List<ShopeeGetItemBaseInfoItem>();
            List<ShopeeItem> shopeeItems = ShopeeGetItemList.ShopeeProductGetItemListAll();
            if (shopeeItems == null || shopeeItems.Count() == 0)
                return rs;

            StringBuilder strListItemId = new StringBuilder();
            Boolean isOk = true;
            int countItemID = shopeeItems.Count();
            int indexItemID = 0;
            int maxSize = 50;
            int i;

            List<DevNameValuePair> ls = new List<DevNameValuePair>();
            // need_tax_info  If true, will return tax info in response.
            ls.Add(new DevNameValuePair("need_tax_info", "false"));

            // need_complaint_policy If true, will return complaint_policy in response.
            ls.Add(new DevNameValuePair("need_complaint_policy", "false"));

            // item_id_list Required item_id  limit [0,50]
            //ls.Add(new DevNameValuePair("item_id_list", strListItemId.ToString())); // Add cuối cùng để cập nhật
            while (indexItemID < countItemID)
            {
                strListItemId.Clear();
                for (i = indexItemID; (i < countItemID) && (i < indexItemID + maxSize); i++)
                {
                    strListItemId.Append(shopeeItems[i].item_id.ToString() + ",");
                }
                indexItemID = i;
                // xóa bỏ , cuối cùng
                strListItemId.Remove(strListItemId.Length - 1, 1);

                ls.RemoveAt(ls.Count() - 1);

                // item_id_list Required item_id  limit [0,50]
                ls.Add(new DevNameValuePair("item_id_list", strListItemId.ToString()));

                ShopeeGetItemBaseInfoResponseHTTP objResponse = ShopeeProductGetItemBaseInfo(ls);

                if (objResponse == null || objResponse.response == null || objResponse.response.item_list == null)
                {
                    isOk = false;
                    break;
                }

                rs.AddRange(objResponse.response.item_list);
            }
            if (!isOk)
                return null;

            return rs;
        }

        /// <summary>
        /// Lấy base info của page đầu, mục đích test
        /// </summary>
        /// <returns>null nếu không lấy thành công</returns>
        public static List<ShopeeGetItemBaseInfoItem> ShopeeProductGetItemBaseInfo_PageFisrst()
        {
            // Lấy danh sách id của item
            List<ShopeeGetItemBaseInfoItem> rs = new List<ShopeeGetItemBaseInfoItem>();
            List<ShopeeItem> shopeeItems = ShopeeGetItemList.ShopeeProductGetItemListAll();
            if (shopeeItems == null || shopeeItems.Count() == 0)
                return rs;

            StringBuilder strListItemId = new StringBuilder();
            Boolean isOk = true;
            int countItemID = shopeeItems.Count();
            int indexItemID = 0;
            int maxSize = 50;
            int i;

            List<DevNameValuePair> ls = new List<DevNameValuePair>();
            // need_tax_info  If true, will return tax info in response.
            ls.Add(new DevNameValuePair("need_tax_info", "false"));

            // need_complaint_policy If true, will return complaint_policy in response.
            ls.Add(new DevNameValuePair("need_complaint_policy", "false"));

            // item_id_list Required item_id  limit [0,50]
            ls.Add(new DevNameValuePair("item_id_list", strListItemId.ToString())); // Add cuối cùng để cập nhật
            while (indexItemID < countItemID)
            {
                strListItemId.Clear();
                for (i = indexItemID; (i < countItemID) && (i < indexItemID + maxSize); i++)
                {
                    strListItemId.Append(shopeeItems[i].item_id.ToString() + ",");
                }
                indexItemID = i;
                // xóa bỏ , cuối cùng
                strListItemId.Remove(strListItemId.Length - 1, 1);

                ls.RemoveAt(ls.Count() - 1);

                // item_id_list Required item_id  limit [0,50]
                ls.Add(new DevNameValuePair("item_id_list", strListItemId.ToString()));

                ShopeeGetItemBaseInfoResponseHTTP objResponse = ShopeeProductGetItemBaseInfo(ls);

                if (objResponse == null || objResponse.response == null || objResponse.response.item_list == null)
                {
                    isOk = false;
                    break;
                }

                rs.AddRange(objResponse.response.item_list);
                break;
            }
            if (!isOk)
                return null;

            return rs;
        }

        /// <summary>
        /// Lấy base info của 1 item
        /// </summary>
        /// <returns>null nếu không lấy thành công</returns>
        public static ShopeeGetItemBaseInfoItem ShopeeProductGetItemBaseInfoFromId(long id)
        {
            List<DevNameValuePair> ls = new List<DevNameValuePair>();
            // need_tax_info  If true, will return tax info in response.
            ls.Add(new DevNameValuePair("need_tax_info", "false"));

            // need_complaint_policy If true, will return complaint_policy in response.
            ls.Add(new DevNameValuePair("need_complaint_policy", "false"));

            // item_id_list Required item_id  limit [0,50]
            ls.Add(new DevNameValuePair("item_id_list", id.ToString()));


            ShopeeGetItemBaseInfoResponseHTTP objResponse = ShopeeProductGetItemBaseInfo(ls);
            if (objResponse == null || objResponse.response == null || objResponse.response.item_list == null)
            {
                if (objResponse != null)
                {
                    Common.CommonErrorMessage = CommonShopeeAPI.GetResponseErrorMessage((CommonResponseHTTP)objResponse);
                }
                return null;
            }

            return objResponse.response.item_list[0];
        }
    }
}
