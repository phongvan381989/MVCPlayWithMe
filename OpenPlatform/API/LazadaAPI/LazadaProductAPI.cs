using Lazop.Api;
using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.API.LazadaAPI
{
    public class LazadaProductAPI
    {
        public static string serverUrl = "https://api.lazada.vn/rest";
        public static int limitOfGetProduct = 50;
        public static int limitOfUpdateProduct = 20;

        public static ILazopClient GetLazopClient()
        {
            ILazopClient client = new LazopClient(serverUrl,
                    LazadaAuthenAPI.lazadaAuthen.appKey,
                    LazadaAuthenAPI.lazadaAuthen.appSecret);
            return client;
        }

        // GET /products/get
        // Use this API to get detailed information of the specified products.

        // Returns the products with the status matching this parameter.
        //Possible values are all, live, inactive, deleted, pending, rejected, sold-out.
        public static List<LazadaProduct> GetProductAllFromStatus(
            string status,
            DateTime? date)
        {
            List<LazadaProduct> ls = new List<LazadaProduct>();
            if(!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
            {
                return ls;
            }
            try
            {
                ILazopClient client = GetLazopClient();

                LazopRequest request = new LazopRequest();
                request.SetApiName("/products/get");
                request.SetHttpMethod("GET");

                // Returns the products with the status matching this parameter.
                // Possible values are all, live, inactive, deleted, pending, rejected, sold-out. Mandatory.
                request.AddApiParameter("filter", status);
                //request.AddApiParameter("update_before", "2018-01-01T00:00:00+0800");
                //request.AddApiParameter("create_before", "2018-01-01T00:00:00+0800");

                // Deprecated(The number of Items you want to skip before you start counting),
                // It is recommended to use date for scrolling query.The maximum offset is 10000
                request.AddApiParameter("offset", "0");

                // Limits the returned products to those created after or on the specified date,
                // given in ISO 8601 date format. Optional
                //request.AddApiParameter("create_after", "2024-01-01T00:00:00+0700");
                //request.AddApiParameter("update_after", "2010-01-01T00:00:00+0800");
                if (date.HasValue)
                {
                    request.AddApiParameter("create_after",
                        CommonLazadaAPI.FormatDateTimeWithOffset(date.Value));
                }

                // The number of Items you would like to fetch from every response,
                // The maximum is 50.
                request.AddApiParameter("limit", limitOfGetProduct.ToString());
                //request.AddApiParameter("options", "1");
                //request.AddApiParameter("sku_seller_list", " [\"39817:01:01\", \"Apple 6S Black\"]");
                int count = 0;

                int offset = 0;
                //int total_products = -1;
                while (count <= 100)
                {
                    count++;
                    request.UpdateApiParameter("offset", offset.ToString());
                       LazopResponse response = client.Execute(request, LazadaAuthenAPI.lazadaAuthen.accessToken);
                    MyLogger.LazadaRestLog(request, response);

                    if(response.IsError())
                    {
                        break;
                    }

                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    LazadaGetProductsResponseBody objectRes =
                        JsonConvert.DeserializeObject<LazadaGetProductsResponseBody>(response.Body, settings);

                    if(objectRes.data == null || objectRes.data.products == null)
                    {
                        break;
                    }

                    ls.AddRange(objectRes.data.products);

                    offset = offset + objectRes.data.products.Count;
                    if (objectRes.data.products.Count < 50 ||
                        offset >= objectRes.data.total_products)
                    {
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                ls.Clear();
            }
            return ls;
        }

        public static List<LazadaProduct> GetProductAll()
        {
            return GetProductAllFromStatus("all", null);
        }

        public static List<LazadaProduct> GetProductLiveAll()
        {
            return GetProductAllFromStatus("live", null);
        }

        public static LazadaProduct GetProductItem(long item_id)
        {
            LazadaProduct pro = null;
            if (!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
            {
                return pro;
            }

            try
            {
                ILazopClient client = GetLazopClient();
                LazopRequest request = new LazopRequest();
                request.SetApiName("/product/item/get");
                request.SetHttpMethod("GET");
                request.AddApiParameter("item_id", item_id.ToString());
                LazopResponse response = client.Execute(request, LazadaAuthenAPI.lazadaAuthen.accessToken);
                MyLogger.LazadaRestLog(request, response);

                if (response.IsError())
                {
                    return pro;
                }
                // Lấy Variation1.name nếu có
                // Parse JSON
                string json = response.Body;
                var obj = Newtonsoft.Json.Linq.JObject.Parse(json);

                // Lấy tên thuộc tính đầu tiên trong "saleProp"
                string variationName = "";
                try
                {
                    if (obj["data"]["variation"] != null &&
                        obj["data"]["variation"]["Variation1"] != null)
                    {
                        variationName = (string)obj["data"]["variation"]["Variation1"]["name"];
                        json = json.Replace(variationName + "\":", "mySkuName\":");
                    }
                }
                catch(Exception ex)
                {
                    MyLogger.GetInstance().Warn("Get variationName crashed");
                    MyLogger.GetInstance().Warn(ex.Message);
                }

                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                LazadaGetProductItemResponseBody objectRes =
                    JsonConvert.DeserializeObject<LazadaGetProductItemResponseBody>(json, settings);

                pro = objectRes.data;
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                pro = null;
            }
            return pro;
        }

        // Lấy sản phẩm mới đăng trong 1 tháng gần đây
        public static List<LazadaProduct> GetNewProductOneMonth()
        {
            return GetProductAllFromStatus("live", DateTime.Now.AddDays(-30));
        }

        // Hàm này điều chỉnh tăng giảm tồn kho theo tham số truyền vào. 
        public static Boolean AdjustSellableQuantity()
        {
            if (!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
            {
                return false;
            }

            ILazopClient client = GetLazopClient();
            LazopRequest request = new LazopRequest();
            request.SetApiName("/product/stock/sellable/adjust");

            request.AddApiParameter("payload", "<Request><Product><Skus><Sku><ItemId>3015484776</ItemId><SkuId>14730059197</SkuId><SellerSku>3063228760-1743410643073-0</SellerSku><MultiWarehouseInventories><MultiWarehouseInventory><WarehouseCode>dropshipping</WarehouseCode><SellableQuantity>20</SellableQuantity> </MultiWarehouseInventory> </MultiWarehouseInventories> </Sku> </Skus> </Product> </Request>");
            LazopResponse response = client.Execute(request,
                LazadaAuthenAPI.lazadaAuthen.accessToken);
            MyLogger.LazadaRestLog(request, response);

            if (response.IsError())
            {
                return false;
            }

            return true;
        }

        public static string BuildSkuXmlForUpdateQuantity(
            List<LazadaParameterQuantity_PriceUpdate> skus)
        {
            var sb = new StringBuilder();
            sb.Append("<Request><Product><Skus>");

            foreach (var sku in skus)
            {
                sb.Append("<Sku>")
                  .Append("<ItemId>").Append(sku.itemId).Append("</ItemId>")
                  .Append("<SkuId>").Append(sku.skuId).Append("</SkuId>")
                  .Append("<Quantity>").Append(sku.quantity).Append("</Quantity>")
                  .Append("</Sku>");
            }

            sb.Append("</Skus></Product></Request>");
            return sb.ToString();
        }

        public static string BuildSkuXmlForUpdatePrice_SalePrice(
            List<LazadaParameterQuantity_PriceUpdate> skus)
        {
            var sb = new StringBuilder();
            sb.Append("<Request><Product><Skus>");

            foreach (var sku in skus)
            {
                sb.Append("<Sku>")
                  .Append("<ItemId>").Append(sku.itemId).Append("</ItemId>")
                  .Append("<SkuId>").Append(sku.skuId).Append("</SkuId>")
                  .Append("<Price>").Append(sku.price).Append("</Price>")
                  .Append("<SalePrice>").Append(sku.salePrice).Append("</SalePrice>")
                  .Append("</Sku>");
            }

            sb.Append("</Skus></Product></Request>");
            return sb.ToString();
        }

        private static void UpdateErrorMessage(
            List<LazadaParameterQuantity_PriceUpdate> skus,
            LazadaUpdatePriceQuantityReponseBody objectRes
            )
        {
            if(objectRes.detail != null)
            {
                foreach(var detailObj in objectRes.detail)
                {
                    foreach(var sku in skus)
                    {
                        if(detailObj.item_id == sku.itemId && detailObj.sku_id == sku.skuId)
                        {
                            sku.code = detailObj.code;
                            sku.message = detailObj.message; // Đây là thông báo lỗi lazada trả về
                        }
                    }
                }
            }
        }

        // Cập nhật tồn kho bằng tham số truyền vào.
        public static Boolean UpdateQuantity(List<LazadaParameterQuantity_PriceUpdate> skus)
        {
            int count = skus.Count;

            if (!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
            {
                return false;
            }

            ILazopClient client = GetLazopClient();
            LazopRequest request = new LazopRequest();
            request.SetApiName("/product/price_quantity/update");

            request.AddApiParameter("payload", "");
            int index = 0;
            Boolean isError = false;
            while (index < count)
            {
                int ite = 0;
                List<LazadaParameterQuantity_PriceUpdate> skusTemp =
                    new List<LazadaParameterQuantity_PriceUpdate>();
                for (int i = index; i < count; i++)
                {
                    skusTemp.Add(skus[i]);
                    index = i + 1;
                    ite++;
                    if(ite == limitOfUpdateProduct)
                    {
                        break;
                    }
                }
                string payload = BuildSkuXmlForUpdateQuantity(skusTemp);
                request.UpdateApiParameter("payload", payload);
                LazopResponse response = client.Execute(request,
                    LazadaAuthenAPI.lazadaAuthen.accessToken);
                MyLogger.LazadaRestLog(request, response);

                LazadaUpdatePriceQuantityReponseBody objectRes =
                    JsonConvert.DeserializeObject<LazadaUpdatePriceQuantityReponseBody>(response.Body,
                    Common.jsonSerializersettings);

                // Nếu có sản phẩm cập nhật lỗi sẽ lưu thông báo lỗi
                UpdateErrorMessage(skusTemp, objectRes);

                // vẫn tiếp tục cập nhật dù có sku cập nhật lỗi
                if (response.IsError() && isError == false)
                {
                    isError = true;
                }
                Thread.Sleep(500);
            }

            return isError;
        }

        // Cập nhật tồn kho bằng tham số truyền vào.
        public static string UpdateQuantityOfOneItemModel(LazadaParameterQuantity_PriceUpdate sku)
        {
            if (!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
            {
                return null;
            }

            ILazopClient client = GetLazopClient();
            LazopRequest request = new LazopRequest();
            request.SetApiName("/product/price_quantity/update");

            request.AddApiParameter("payload", "");
            List<LazadaParameterQuantity_PriceUpdate> skusTemp =
            new List<LazadaParameterQuantity_PriceUpdate>();
            skusTemp.Add(sku);

            string payload = BuildSkuXmlForUpdateQuantity(skusTemp);
            request.UpdateApiParameter("payload", payload);
            LazopResponse response = client.Execute(request,
                LazadaAuthenAPI.lazadaAuthen.accessToken);
            MyLogger.LazadaRestLog(request, response);
            return response.Body;
        }

        // Cập nhật tồn kho giá bìa và giá bán
        public static Boolean UpdatePrice_SalePrie(List<LazadaParameterQuantity_PriceUpdate> skus)
        {
            int count = skus.Count;

            if (!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
            {
                return false;
            }

            ILazopClient client = GetLazopClient();
            LazopRequest request = new LazopRequest();
            request.SetApiName("/product/price_quantity/update");

            request.AddApiParameter("payload", "");
            int index = 0;
            Boolean isError = false;
            while (index < count)
            {
                int ite = 0;
                List<LazadaParameterQuantity_PriceUpdate> skusTemp =
                    new List<LazadaParameterQuantity_PriceUpdate>();
                for (int i = index; i < count; i++)
                {
                    skusTemp.Add(skus[i]);
                    index = i + 1;
                    ite++;
                    if (ite == limitOfUpdateProduct)
                    {
                        break;
                    }
                }
                string payload = BuildSkuXmlForUpdatePrice_SalePrice(skusTemp);
                request.UpdateApiParameter("payload", payload);
                LazopResponse response = client.Execute(request,
                    LazadaAuthenAPI.lazadaAuthen.accessToken);
                MyLogger.LazadaRestLog(request, response);

                LazadaUpdatePriceQuantityReponseBody objectRes =
                    JsonConvert.DeserializeObject<LazadaUpdatePriceQuantityReponseBody>(response.Body,
                    Common.jsonSerializersettings);

                // Nếu có sản phẩm cập nhật lỗi sẽ lưu thông báo lỗi
                UpdateErrorMessage(skusTemp, objectRes);

                // vẫn tiếp tục cập nhật dù có sku cập nhật lỗi
                if (response.IsError() && isError == false)
                {
                    isError = true;
                }
                Thread.Sleep(500);
            }

            return isError;
        }
    }
}
