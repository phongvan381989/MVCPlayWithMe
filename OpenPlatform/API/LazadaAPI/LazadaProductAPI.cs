using Lazop.Api;
using Lazop.Api.Util;
using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct;
using MySql.Data.MySqlClient;
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
        public static int limitOfGetProduct = 50;
        public static int limitOfUpdateProduct = 20;
        public static int maximumOfImages = 8;
        public static int maximumOfImageSize = 4; // Kích thước ảnh không quá 4MB

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
                ILazopClient client = CommonLazadaAPI.GetLazopClient();

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
                while (count <= 200)
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
                    if (objectRes.data.products.Count < limitOfGetProduct ||
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
                ILazopClient client = CommonLazadaAPI.GetLazopClient();
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

            ILazopClient client = CommonLazadaAPI.GetLazopClient();
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

            ILazopClient client = CommonLazadaAPI.GetLazopClient();
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
                // vẫn tiếp tục cập nhật dù có sku cập nhật lỗi
                if (response.IsError() && isError == false)
                {
                    isError = true;
                }
                if (response.IsError())
                {
                    continue;
                }

                LazadaUpdatePriceQuantityReponseBody objectRes =
                    JsonConvert.DeserializeObject<LazadaUpdatePriceQuantityReponseBody>(response.Body,
                    Common.jsonSerializersettings);

                // Nếu có sản phẩm cập nhật lỗi sẽ lưu thông báo lỗi
                UpdateErrorMessage(skusTemp, objectRes);

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

            ILazopClient client = CommonLazadaAPI.GetLazopClient();
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
            if (response.IsError())
            {
                return null;
            }
            return response.Body;
        }

        // Cập nhật tồn kho giá bìa và giá bán
        public static Boolean LazadaUpdatePrice_SalePrie(List<LazadaParameterQuantity_PriceUpdate> skus)
        {
            int count = skus.Count;

            if (!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
            {
                return false;
            }

            ILazopClient client = CommonLazadaAPI.GetLazopClient();
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
                // vẫn tiếp tục cập nhật dù có sku cập nhật lỗi
                if (response.IsError() && isError == false)
                {
                    isError = true;
                }
                if (response.IsError())
                {
                    continue;
                }

                LazadaUpdatePriceQuantityReponseBody objectRes =
                    JsonConvert.DeserializeObject<LazadaUpdatePriceQuantityReponseBody>(response.Body,
                    Common.jsonSerializersettings);

                // Nếu có sản phẩm cập nhật lỗi sẽ lưu thông báo lỗi
                UpdateErrorMessage(skusTemp, objectRes);

                Thread.Sleep(500);
            }

            return isError;
        }

        public static void LazadaGetCategoryTree()
        {
            if (!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
            {
                return;
            }

            ILazopClient client = CommonLazadaAPI.GetLazopClient();
            LazopRequest request = new LazopRequest();
            // Use this API to retrieve the list of all product categories in the system.
            request.SetApiName("/category/tree/get");
            request.SetHttpMethod("GET");
            request.AddApiParameter("language_code", "vi_VN");

            LazopResponse response = client.Execute(request,
                    LazadaAuthenAPI.lazadaAuthen.accessToken);
            MyLogger.LazadaRestLog(request, response);
        }

        public static void LazadaGetCategoryAttributes(int categoryId)
        {
            if (!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
            {
                return;
            }

            ILazopClient client = CommonLazadaAPI.GetLazopClient();
            LazopRequest request = new LazopRequest();
            // Use this API to get a list of attributes for a specified product category.
            request.SetApiName("/category/attributes/get");
            request.SetHttpMethod("GET");
            request.AddApiParameter("primary_category_id", categoryId.ToString());
            request.AddApiParameter("language_code", "vi_VN");

            LazopResponse response = client.Execute(request,
                    LazadaAuthenAPI.lazadaAuthen.accessToken);
            MyLogger.LazadaRestLog(request, response);
        }

        public static Boolean LazadaGetBrandByPages(MySqlConnection conn)
        {
            if (!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
            {
                return true;
            }

            ILazopClient client = CommonLazadaAPI.GetLazopClient();
            LazopRequest request = new LazopRequest();
            // Use this API to retrieve all product brands by page index in the system.
            request.SetApiName("/category/brands/query");
            request.SetHttpMethod("GET");

            // Number of brands to skip (i.e., an offset into the result set;
            // together with the "limit" parameter, simple result set paging is possible;
            // if you do page through results, note that the list of brands might
            // change during paging).
            request.AddApiParameter("startRow", "");

            // The maximum number of brands that can be returned. If you omit this parameter,
            // the default of 40 is used. The Maximum is 200.
            int theMaximum = 200;
            request.AddApiParameter("pageSize", theMaximum.ToString());
            int startRow = 0;
            Boolean isError = false;
            try
            {
                LazadaMySql lazadaMySql = new LazadaMySql();
                while (true)
                {
                    request.UpdateApiParameter("startRow", startRow.ToString());
                    LazopResponse response = client.Execute(request,
                            LazadaAuthenAPI.lazadaAuthen.accessToken);
                    MyLogger.LazadaRestLog(request, response);
                    if (response.IsError())
                    {
                        isError = true;
                        break;
                    }

                    LazadaGetBrandByPagesResponseBody objectRes =
                        JsonConvert.DeserializeObject<LazadaGetBrandByPagesResponseBody>(response.Body,
                        Common.jsonSerializersettings);

                    lazadaMySql.InserttbLazadaBrand(objectRes.data.module, conn);

                    startRow = theMaximum * objectRes.data.page_index;
                    if(startRow >= objectRes.data.total_record)
                    {
                        break;
                    }
                    Thread.Sleep(500);
                }
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Warn("LazadaGetBrandByPages calling with exception " +
                    ex.ToString());
                isError = true;
            }

            return isError;
        }

        public static LazadaUploadImage LazadaUploadImage(string filePath)
        {
            try
            {
                if (!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
                {
                    return null;
                }

                ILazopClient client = CommonLazadaAPI.GetLazopClient();
                LazopRequest request = new LazopRequest();
                // Use this API to upload a single image file to Lazada site. Allowed image formats are JPG and PNG.
                // The maximum size of an image file is 1MB.
                // NOTE: Test với ảnh 3.25 MB vẫn up ok, với ảnh 12.2 MB thì crash
                request.SetApiName("/image/upload");
                request.AddFileParameter("image", new FileItem(filePath));

                LazopResponse response = client.Execute(request,
                        LazadaAuthenAPI.lazadaAuthen.accessToken);
                MyLogger.LazadaRestLog(request, response);
                if (response.IsError())
                {
                    return null;
                }

                LazadaUploadImageResponseBody objectRes =
                            JsonConvert.DeserializeObject<LazadaUploadImageResponseBody>(response.Body,
                            Common.jsonSerializersettings);

                return objectRes.data.image;
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
            }
            return null;
        }

        public static LazadaCreateProductResponseBody LazadaCreateProduct(
            LazadaCreateProductRequest requestPara)
        {
            if (!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
            {
                return null;
            }

            ILazopClient client = CommonLazadaAPI.GetLazopClient();
            LazopRequest request = new LazopRequest();
            // Use this API to create a single new product.
            // Find more details below: https://open.lazada.com/apps/doc/doc?nodeId=30720&docId=120949
            request.SetApiName("/product/create");
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,                // bỏ qua null
                DefaultValueHandling = DefaultValueHandling.Ignore           // bỏ qua giá trị mặc định
            };
            string payload = JsonConvert.SerializeObject(requestPara, settings);
            request.AddApiParameter("payload", payload);

            LazadaCreateProductResponseBody objectRes = null;

            try
            {
                LazopResponse response = client.Execute(request,
                        LazadaAuthenAPI.lazadaAuthen.accessToken);
                MyLogger.LazadaRestLog(request, response);

                objectRes =
                            JsonConvert.DeserializeObject<LazadaCreateProductResponseBody>(response.Body,
                            Common.jsonSerializersettings);

            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                objectRes = null;
            }
            return objectRes;
        }

        public static void LazadaCreateProductTest()
        {
            if (!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
            {
                return;
            }

            ILazopClient client = CommonLazadaAPI.GetLazopClient();
            LazopRequest request = new LazopRequest();
            // Use this API to create a single new product.
            // Find more details below: https://open.lazada.com/apps/doc/doc?nodeId=30720&docId=120949
            request.SetApiName("/product/create");

            //string payload = "{    \"Request\": {        \"Product\": {            \"Attributes\": {                \"brand\": \"No Brand\",                \"description\": \"Detailed description of this product.\",                \"model\": \"Sample Model\",                \"name\": \"Sample Product Name\",                \"package_height\": \"10\",                \"package_length\": \"20\",                \"package_weight\": \"0.8\",                \"package_width\": \"15\",                \"short_description\": \"Brief product info\"            },            \"Images\": {                \"Image\": [\"https://sg-test-11.slatic.net/p/843bc272d21ccb367697733ead04bbb7.jpg\", \"https://sg-test-11.slatic.net/p/8bba87af6b7bf5b3af7158e7c6f28f3c.png\"]            },            \"PrimaryCategory\": \"8666\",            \"Skus\": {                \"Sku\": {                    \"Images\": {                        \"Image\": [\"https://sg-test-11.slatic.net/p/843bc272d21ccb367697733ead04bbb7.jpg\"]                    },                    \"SellerSku\": \"SampleSKU001\",                    \"package_content\": \"Sample package content\",                    \"price\": \"50000.00\",                    \"quantity\": \"50\",                    \"special_from_date\": \"2024-01-01 00:00:00\",                    \"special_price\": \"45000.00\",                    \"special_to_date\": \"2028-12-31 23:59:59\"                }            }        }    }}";
            //string payload = "{\"Request\":{\"Product\":{\"Attributes\":{\"brand\":\"NoBrand\",\"description\":\"Detaileddescriptionofthisproduct.\",\"model\":\"SampleModel\",\"name\":\"SampleProductName\",\"package_height\":\"10\",\"package_length\":\"20\",\"package_weight\":\"0.8\",\"package_width\":\"15\",\"short_description\":\"Briefproductinfo\"},\"Images\":{\"Image\":[\"https://sg-test-11.slatic.net/p/843bc272d21ccb367697733ead04bbb7.jpg\",\"https://sg-test-11.slatic.net/p/8bba87af6b7bf5b3af7158e7c6f28f3c.png\"]},\"PrimaryCategory\":\"8666\",\"Skus\":{\"Sku\":{\"Images\":{\"Image\":[\"https://sg-test-11.slatic.net/p/843bc272d21ccb367697733ead04bbb7.jpg\"]},\"SellerSku\":\"SampleSKU001\",\"package_content\":\"Samplepackagecontent\",\"price\":\"50000.00\",\"quantity\":\"50\",\"special_from_date\":\"2024-01-0100:00:00\",\"special_price\":\"45000.00\",\"special_to_date\":\"2028-12-3123:59:59\"}}}}}";
            string payload = "{    \"Request\": {        \"Product\": {            \"AssociatedSku\": \"Existing SkuId in seller center\",            \"Attributes\": {                \"Hazmat\": \"None\",                \"brand\": \"No Brand\",                \"brand_id\": \"30768\",                \"delivery_option_sof\": \"No\",                \"description\": \"TEST\",                \"disableAttributeAutoFill\": false,                \"gift_wrapping\": \"Yes\",                \"laptop_size\": \"11 - 12 inches\",                \"material\": \"Leather\",                \"model\": \"test\",                \"name\": \"test 2022 02\",                \"name_engravement\": \"Yes\",                \"preorder_days\": \"25\",                \"preorder_enable\": \"Yes\",                \"propCascade\": {                    \"26\": \"120013644:162,100006867:160387\"                },                \"short_description\": \"cm x 1efgtecm<br /><brfwefgtek\",                \"warranty\": \"1 Month\",                \"warranty_type\": \"Local seller warranty\",                \"waterproof\": \"Waterproof\"            },            \"Images\": {                \"Image\": [\"https://my-live-02.slatic.net/p/47b6cb07bd8f80aa3cc34b180b902f3e.jpg\"]            },            \"PrimaryCategory\": \"10002019\",            \"Skus\": {                \"Sku\": [{                        \"Images\": {                            \"Image\": [\"https://my-live-02.slatic.net/p/47b6cb07bd8f80aa3cc34b180b902f3e.jpg\"]                        },                        \"SellerSku\": \"test2022 02\",                        \"package_content\": \"laptop bag\",                        \"package_height\": \"10\",                        \"package_length\": \"10\",                        \"package_weight\": \"0.5\",                        \"package_width\": \"10\",                        \"price\": \"35\",                        \"quantity\": \"3\",                        \"saleProp\": {                            \"color_family\": \"Green\",                            \"size\": \"10\"                        },                        \"special_from_date\": \"2022-06-20 17:18:31\",                        \"special_price\": \"33\",                        \"special_to_date\": \"2025-03-15 17:18:31\"                    }                ]            }        }    }}";
            request.AddApiParameter("payload", payload);

            LazadaCreateProductResponseBody objectRes = null;

            try
            {
                LazopResponse response = client.Execute(request,
                        LazadaAuthenAPI.lazadaAuthen.accessToken);
                MyLogger.LazadaRestLog(request, response);

                objectRes =
                            JsonConvert.DeserializeObject<LazadaCreateProductResponseBody>(response.Body,
                            Common.jsonSerializersettings);

            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
            }
        }
    }
}
