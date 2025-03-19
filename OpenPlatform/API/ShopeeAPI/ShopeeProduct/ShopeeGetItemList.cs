using Newtonsoft.Json;
using MVCPlayWithMe.General;
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
    public class ShopeeGetItemList
    {
        /// <summary>
        /// Lấy item theo các tham số
        /// </summary>
        /// <param name="update_time_from">  <= 0 để bỏ qua</param>
        /// <param name="update_time_to"> <= 0 để bỏ qua</param>
        /// <param name="offset"></param>
        /// <param name="page_size"></param>
        /// <param name="lsShopeeItemStatus"></param>
        /// <returns>null nếu không lấy thành công</returns>
        public static ShopeeGetItemListResponseHTTP ShopeeProductGetItemListOld(long update_time_from, long update_time_to,
            int offset, int page_size,
            List<ShopeeItemStatus> lsShopeeItemStatus)
        {
            string path = "/api/v2/product/get_item_list";

            List<DevNameValuePair> ls = new List<DevNameValuePair>();
            ls.Add(new DevNameValuePair("offset", offset.ToString()));
            ls.Add(new DevNameValuePair("page_size", page_size.ToString()));
            if (update_time_from > 0 && update_time_to > 0 && update_time_to > update_time_from)
            {
                ls.Add(new DevNameValuePair("update_time_from", update_time_from.ToString()));
                ls.Add(new DevNameValuePair("update_time_to", update_time_to.ToString()));
            }
            foreach (ShopeeItemStatus item in lsShopeeItemStatus)
            {
                ls.Add(new DevNameValuePair("item_status", item.GetString()));
            }

            IRestResponse response = CommonShopeeAPI.ShopeeGetMethod(path, ls);
            if (response == null)
                return null;

            ShopeeGetItemListResponseHTTP objResponse = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    objResponse = JsonConvert.DeserializeObject<ShopeeGetItemListResponseHTTP>(response.Content, settings);
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.Message);
                    return null;
                }
            }
            return objResponse;
        }

        public static ShopeeGetItemListResponseHTTP ShopeeProductGetItemList(List<DevNameValuePair> ls)
        {
            string path = "/api/v2/product/get_item_list";

            IRestResponse response = CommonShopeeAPI.ShopeeGetMethod(path, ls);
            if (response == null)
                return null;

            ShopeeGetItemListResponseHTTP objResponse = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    objResponse = JsonConvert.DeserializeObject<ShopeeGetItemListResponseHTTP>(response.Content, settings);
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
        /// Lấy tất cả item. Item này chứa dữ liệu vô cùng base
        /// </summary>
        /// <returns>Không phần tử nếu không lấy thành công</returns>
        public static List<ShopeeItem> ShopeeProductGetItemListAll()
        {
            List<ShopeeItem> rs = new List<ShopeeItem>();
            int offset = 0;
            int page_size = 50;
            List<ShopeeItemStatus> lsShopeeItemStatus = new List<ShopeeItemStatus>();
            lsShopeeItemStatus.Add(new ShopeeItemStatus(ShopeeItemStatus.EnumShopeeItemStatus.NORMAL));
            lsShopeeItemStatus.Add(new ShopeeItemStatus(ShopeeItemStatus.EnumShopeeItemStatus.UNLIST));
            lsShopeeItemStatus.Add(new ShopeeItemStatus(ShopeeItemStatus.EnumShopeeItemStatus.BANNED));
            lsShopeeItemStatus.Add(new ShopeeItemStatus(ShopeeItemStatus.EnumShopeeItemStatus.REVIEWING));
            lsShopeeItemStatus.Add(new ShopeeItemStatus(ShopeeItemStatus.EnumShopeeItemStatus.SHOPEE_DELETE));
            lsShopeeItemStatus.Add(new ShopeeItemStatus(ShopeeItemStatus.EnumShopeeItemStatus.SELLER_DELETE));

            List<DevNameValuePair> ls = new List<DevNameValuePair>();
            ls.Add(new DevNameValuePair("page_size", page_size.ToString()));

            foreach (ShopeeItemStatus item in lsShopeeItemStatus)
            {
                ls.Add(new DevNameValuePair("item_status", item.GetString()));
            }
            ls.Add(new DevNameValuePair("offset", offset.ToString())); // Add cuối cùng để cập nhật

            while (true)
            {
                ls.RemoveAt(ls.Count() - 1);
                ls.Add(new DevNameValuePair("offset", offset.ToString())); // Add cuối cùng để cập nhật

                ShopeeGetItemListResponseHTTP objResponse = ShopeeProductGetItemList(ls);
                if (objResponse == null)
                {
                    break;
                }
                if (objResponse.response.item != null)
                {
                    rs.AddRange(objResponse.response.item);
                    offset = offset + objResponse.response.item.Count();
                }
                if (!objResponse.response.has_next_page)
                {
                    break;
                }
            }
            return rs;
        }

        // Lấy danh sách sản phẩm NORMAL, trong khoảng thời gian nhất định
        public static List<ShopeeItem> ShopeeProductGetNormal_ItemList(
            long update_time_from, long update_time_to)
        {
            List<ShopeeItem> rs = new List<ShopeeItem>();
            int offset = 0;
            int page_size = 50;

            List<DevNameValuePair> ls = new List<DevNameValuePair>();
            ls.Add(new DevNameValuePair("page_size", page_size.ToString()));
            ls.Add(new DevNameValuePair("update_time_from", update_time_from.ToString()));
            ls.Add(new DevNameValuePair("update_time_to", update_time_to.ToString()));
            ls.Add(new DevNameValuePair("item_status", "NORMAL"));

            ls.Add(new DevNameValuePair("offset", offset.ToString())); // Add cuối cùng để cập nhật

            while (true)
            {
                ls.RemoveAt(ls.Count() - 1);
                ls.Add(new DevNameValuePair("offset", offset.ToString())); // Add cuối cùng để cập nhật

                ShopeeGetItemListResponseHTTP objResponse = ShopeeProductGetItemList(ls);
                if (objResponse == null)
                {
                    break;
                }
                if (objResponse.response.item != null)
                {
                    rs.AddRange(objResponse.response.item);
                    offset = offset + objResponse.response.item.Count();
                }
                if (!objResponse.response.has_next_page)
                    break;
            }
            return rs;
        }
    }
}
