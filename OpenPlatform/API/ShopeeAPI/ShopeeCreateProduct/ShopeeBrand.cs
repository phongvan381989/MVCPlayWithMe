using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.API;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeCreateProduct;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeCreateProduct
{
    class ShopeeBrand
    {
        // Danh sách quá dài, nhiều nghìn thương hiệu nên ta insert db trong hàm luôn
        // category id: sách trẻ em 101541, sách người lớn 101543 có chung brand id
        public static async Task<MySqlResultState> ShopeeGetBrandList(
            long categoryId,
            ShopeeMySql shopeeSqler,
            MySqlConnection conn)
        {
            List<ShopeeBrandObject> brandList = new List<ShopeeBrandObject>();

            List<DevNameValuePair> ls = new List<DevNameValuePair>();
            ls.Add(new DevNameValuePair("offset", "0"));
            ls.Add(new DevNameValuePair("page_size", "100"));
            ls.Add(new DevNameValuePair("category_id", categoryId.ToString()));
            ls.Add(new DevNameValuePair("status", "1"));
            ls.Add(new DevNameValuePair("language", "VN"));

            string path = "/api/v2/product/get_brand_list";
            long offset = 0;
            ShopeeGetBrandListResponseHTTP objResponse = null;
            MySqlResultState result = new MySqlResultState();
            while (true)
            {
                ls[0].value = offset.ToString();
                IRestResponse response = CommonShopeeAPI.ShopeeGetMethod(path, ls);
                if (response == null)
                {
                    break;
                }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        objResponse = JsonConvert.DeserializeObject<ShopeeGetBrandListResponseHTTP>(response.Content, Common.jsonSerializersettings);

                        //brandList.AddRange(objResponse.response.brand_list);
                        result = shopeeSqler.InserttbShopeeBrand(categoryId, objResponse.response.brand_list, conn);
                        if(result.State != EMySqlResultState.OK)
                        {
                            break;
                        }

                        if (objResponse.response.has_next_page)
                        {
                            offset = objResponse.response.next_offset;
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Common.SetResultException(ex, result);
                        break;
                    }
                }
            }
            return result;
        }
    }
}
