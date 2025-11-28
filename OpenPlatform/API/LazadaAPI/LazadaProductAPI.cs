using Lazop.Api;
using Lazop.Api.Util;
using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
        public static int maximumOfImageSize = 3; // Kích thước ảnh không quá 3MB

        // Giới hạn kích thước video file
        public const int maximumVideoSizeMB = 100;
        public const int maximumVideoSizeBytes = maximumVideoSizeMB * 1024 * 1024; // 100 MB
        public const int chunkSizeMB = 3;
        public const int chunkSizeBytes = chunkSizeMB * 1024 * 1024; // 3 MB

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

        public static string BuildSkuXmlForUpdateQuantityPrice_SalePrice(
            List<LazadaParameterQuantity_PriceUpdate> skus)
        {
            var sb = new StringBuilder();
            sb.Append("<Request><Product><Skus>");

            foreach (var sku in skus)
            {
                sb.Append("<Sku>")
                  .Append("<ItemId>").Append(sku.itemId).Append("</ItemId>")
                  .Append("<SkuId>").Append(sku.skuId).Append("</SkuId>");
                if(sku.quantity > -1)
                {
                    sb.Append("<Quantity>").Append(sku.quantity).Append("</Quantity>");
                }
                if(sku.price > -1)
                {
                    sb.Append("<Price>").Append(sku.price).Append("</Price>");
                }
                if(sku.salePrice > -1)
                {
                    sb.Append("<SalePrice>").Append(sku.salePrice).Append("</SalePrice>");
                }
                sb.Append("</Sku>");
            }

            sb.Append("</Skus></Product></Request>");
            return sb.ToString();
        }

        //public static string BuildSkuXmlForUpdatePrice_SalePrice(
        //    List<LazadaParameterQuantity_PriceUpdate> skus)
        //{
        //    var sb = new StringBuilder();
        //    sb.Append("<Request><Product><Skus>");

        //    foreach (var sku in skus)
        //    {
        //        sb.Append("<Sku>")
        //          .Append("<ItemId>").Append(sku.itemId).Append("</ItemId>")
        //          .Append("<SkuId>").Append(sku.skuId).Append("</SkuId>")
        //          .Append("<Price>").Append(sku.price).Append("</Price>")
        //          .Append("<SalePrice>").Append(sku.salePrice).Append("</SalePrice>")
        //          .Append("</Sku>");
        //    }

        //    sb.Append("</Skus></Product></Request>");
        //    return sb.ToString();
        //}

        private static void LazadaUpdateErrorMessage(
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

        // Cập nhật tồn kho, giá bìa, giá bán nếu trong tham số có tồn tại giá trị
        // tương ứng
        public static Boolean LazadaUpdateQuantityPrice_SalePrice_Core(
            List<LazadaParameterQuantity_PriceUpdate> skus)
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
                string payload = BuildSkuXmlForUpdateQuantityPrice_SalePrice(skusTemp);
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
                LazadaUpdateErrorMessage(skusTemp, objectRes);

                Thread.Sleep(500);
            }

            return isError;
        }

        // Cập nhật chỉ tồn kho
        public static Boolean LazadaUpdateQuantity(List<LazadaParameterQuantity_PriceUpdate> skus)
        {
            return LazadaUpdateQuantityPrice_SalePrice_Core(skus);
        }

        // Cập nhật tồn kho của 1 model.
        public static string LazadaUpdateQuantityOfOneItemModel(LazadaParameterQuantity_PriceUpdate sku)
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

            string payload = BuildSkuXmlForUpdateQuantityPrice_SalePrice(skusTemp);
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

        // Cập nhật chỉ giá bìa và giá bán
        public static Boolean LazadaUpdatePrice_SpecialPrice(List<LazadaParameterQuantity_PriceUpdate> skus)
        {
            return LazadaUpdateQuantityPrice_SalePrice_Core(skus);
        }

        // Cập nhật tồn kho, giá bìa và giá bán
        public static Boolean LazadaUpdateQuantityPrice_SpecialPrice(List<LazadaParameterQuantity_PriceUpdate> skus)
        {
            return LazadaUpdateQuantityPrice_SalePrice_Core(skus);
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
                // Please make sure your image size is less than 5000*5000px and file size is less than 3145728B = 3MB.
                // Size ảnh max tài liệu đang lôm côm
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

        // Nếu video chưa đăng thì đăng, ngược lại tìm trong db. 
        // Trả về url video
        public static async Task<MySqlResultState> LazadaUploadVideo(string videoTitle,
            string videoPath,
            MySqlConnection conn)
        {
            MyLogger.GetInstance().Info("LazadaUploadVideo Start");
            MySqlResultState result = new MySqlResultState();
            try
            {
                // Lấy ảnh đầu tiên của video
                string folder = Path.GetDirectoryName(videoPath);
                string strResult = Common.ExtractFirstFrame(videoPath, Path.Combine(folder, "videoThumbnail.jpg"));
                if(!string.IsNullOrEmpty(strResult))
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = strResult;
                    return result;
                }
                string coverUrl = Common.absoluteForCreateMediaFolderPath + "videoThumbnail.jpg";
                // Lấy giá trị hash để lưu db
                string hash = Common.CalculateSHA256FileHash(videoPath);
                if(string.IsNullOrEmpty(hash))
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Calculate file hash fail.";
                    return result;
                }

                string videoUrl = string.Empty;
                string videoId = string.Empty;
                // Kiểm tra giá trị hash đã lưu trong db, nếu có thì trả về video
                {
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM webplaywithme.tb_lazada_video_space WHERE Hash = @inHash;", conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inHash", hash);
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            videoId  = MyMySql.GetString(rdr, "VideoId");
                            videoUrl = MyMySql.GetString(rdr, "VideoUrl");
                        }
                    }
                    if (!string.IsNullOrEmpty(videoId))
                    {
                        // Lấy video Url cho nguy hiểm
                        if (string.IsNullOrEmpty(videoUrl))
                        {
                            // Vì lý do nào đó chưa có video src dù đã có video id. Ta lấy video src và cập nhật vào DB
                            // Lấy videoUrl của video
                            // 5 Lấy video source
                            LazadaGetVideoResponseBody getVideoResponse =
                                LazadaGetVideo(videoId);

                            if (getVideoResponse == null || !getVideoResponse.success)
                            {
                                string error = "LazadaGetVideoResponseBody fall when get video src for update: "
                                        + getVideoResponse == null ? "null" : getVideoResponse.result_message;
                                MyLogger.GetInstance().Info(error);
                                // Cho nguy hiểm nên không cần return
                            }

                            // Cập nhật video src vào DB
                            videoUrl = getVideoResponse.video_url;
                            MySqlCommand cmdTemp = new MySqlCommand("UPDATE webplaywithme.tb_lazada_video_space SET VideoUrl = @inVideoUrl WHERE VideoId = @inVideoId;", conn);
                            cmdTemp.CommandType = CommandType.Text;
                            cmdTemp.Parameters.AddWithValue("@inVideoUrl", videoUrl);
                            cmdTemp.Parameters.AddWithValue("@inVideoId", videoId);
                            cmdTemp.ExecuteNonQuery();
                        }

                        result.Message = videoId;
                        return result;
                    }
                }

                string fileName = Path.GetFileName(videoPath);
                string title = videoTitle;
                if (string.IsNullOrEmpty(title))
                {
                    title = fileName;
                }

                using (var fileStream = new FileStream(videoPath, FileMode.Open, FileAccess.Read))
                {
                    long fileSizeBytes = fileStream.Length;
                    if(fileSizeBytes >= maximumVideoSizeBytes)
                    {
                        result.State = EMySqlResultState.ERROR;
                        result.Message = "Greater " + maximumVideoSizeBytes.ToString() + " bytes";
                        return result;
                    }

                    // Chia nhỏ và upload từng phần video
                    int totalChunks = (int)Math.Ceiling((double)fileSizeBytes / chunkSizeBytes);
                    MyLogger.GetInstance().Info($"Bat dau tai len file {Path.GetFileName(videoPath)} ({fileSizeBytes / 1024 / 1024} MB) thanh {totalChunks} doan.");

                    // Dùng để lưu trữ ID của các đoạn đã tải lên
                    List<LazadaUploadVideoBlockeTag> eTags = new List<LazadaUploadVideoBlockeTag>();

                    // Khởi tạo ban đầu để upload video
                    LazadaInitCreateVideoResponseBody initCreateVideoResponse =
                        LazadaInitCreateVideo(fileName, fileSizeBytes);
                    if(initCreateVideoResponse == null || !initCreateVideoResponse.success)
                    {
                        string error = "LazadaInitCreateVideo fall: "
                            + (initCreateVideoResponse==null?"null":initCreateVideoResponse.result_message);
                        MyLogger.GetInstance().Info(error);
                        result.State = EMySqlResultState.ERROR;
                        result.Message = error;
                        return result;
                    }
                    string upload_id = initCreateVideoResponse.upload_id;

                    for (int partNumber = 0; partNumber < totalChunks; partNumber++)
                    {
                        // 1. Xác định kích thước đoạn hiện tại
                        long bytesToRead = Math.Min(chunkSizeBytes, fileSizeBytes - fileStream.Position);
                        byte[] buffer = new byte[bytesToRead];

                        // 2. Đọc đoạn dữ liệu từ FileStream
                        int bytesRead = await fileStream.ReadAsync(buffer, 0, (int)bytesToRead);

                        if (bytesRead == 0) continue;

                        // 3. Gửi đoạn dữ liệu lên Server
                        LazadaUploadVideoBlockResponseBody uploadVideoBlockResponse = 
                            LazadaUploadVideoBlock(upload_id, partNumber, totalChunks, buffer);
                        if (uploadVideoBlockResponse == null || !uploadVideoBlockResponse.success)
                        {
                            string error = "LazadaUploadVideoBlock fall: "
                                + (uploadVideoBlockResponse == null ? "null" : uploadVideoBlockResponse.result_message);
                            MyLogger.GetInstance().Info(error);
                            result.State = EMySqlResultState.ERROR;
                            result.Message = error;
                            return result;
                        }
                        eTags.Add(new LazadaUploadVideoBlockeTag(uploadVideoBlockResponse.e_tag, partNumber + 1));
                        Thread.Sleep(500);
                    }

                    Thread.Sleep(1000);
                    // 4. Hoàn tất quá trình tải lên
                    // Tải ảnh đại diện video lên
                    LazadaUploadImage objResponse =
                        LazadaProductAPI.LazadaUploadImage(coverUrl);
                    if(objResponse == null)
                    {
                        string error = "LazadaUploadImage fall. Tải ảnh đại diện video thất bại.";
                        MyLogger.GetInstance().Info(error);
                        result.State = EMySqlResultState.ERROR;
                        result.Message = error;
                        return result;
                    }

                    coverUrl = objResponse.url;
                    string parts = JsonConvert.SerializeObject(eTags,
                            Common.jsonSerializersettings);
                    LazadaCompleteCreateVideoResponseBody completeResponse = 
                        LazadaCompleteCreateVideo(upload_id, parts, title, coverUrl, null);
                    if (completeResponse == null || !completeResponse.success)
                    {
                        string error = "LazadaCompleteCreateVideo fall: "
                                + (completeResponse == null ? "null" : completeResponse.result_message);
                        MyLogger.GetInstance().Info(error);
                        result.State = EMySqlResultState.ERROR;
                        result.Message = error;
                        return result;
                    }

                    videoId = completeResponse.video_id;
                    result.Message = videoId;

                    // 5 Lấy video source cho nguy hiểm chứ không thực sự cần thiết
                    LazadaGetVideoResponseBody getVideoResponse =
                        LazadaGetVideo(completeResponse.video_id);

                    if (getVideoResponse == null || !getVideoResponse.success)
                    {
                        string error = "LazadaGetVideoResponseBody fall: "
                                + getVideoResponse == null ? "null" : getVideoResponse.result_message;
                        MyLogger.GetInstance().Info(error);
                        //result.State = EMySqlResultState.ERROR;
                        //result.Message = error;
                        // NOTE: Không return vì
                        // Lỗi vẫn lưu video id, video hash vào db
                    }
                    else
                    {
                        videoUrl = getVideoResponse.video_url;
                    }
                    // Lưu vào db
                    {
                        MySqlCommand cmd = new MySqlCommand("INSERT INTO webplaywithme.tb_lazada_video_space (VideoId, Hash, VideoUrl) VALUES ( @inVideoId, @inHash, @inVideoUrl);",
                            conn);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@inVideoId", videoId);
                        cmd.Parameters.AddWithValue("@inHash", hash);
                        cmd.Parameters.AddWithValue("@inVideoUrl", videoUrl);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                result.State = EMySqlResultState.ERROR;
                result.Message = ex.ToString();
            }
            return result;
        }

        // A seller starts to upload a video file
        public static LazadaInitCreateVideoResponseBody LazadaInitCreateVideo(string fileName, long fileBytes)
        {
            try
            {
                if (!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
                {
                    return null;
                }

                ILazopClient client = CommonLazadaAPI.GetLazopClient();
                LazopRequest request = new LazopRequest();

                request.SetApiName("/media/video/block/create");

                // local file name of vedio file
                request.AddApiParameter("fileName", fileName);
                // video file's bytes, should be less than 100M
                request.AddApiParameter("fileBytes", fileBytes.ToString());

                LazopResponse response = client.Execute(request,
                        LazadaAuthenAPI.lazadaAuthen.accessToken);
                MyLogger.LazadaRestLog(request, response);
                //Thread.Sleep(500);
                if (response.IsError())
                {
                    return null;
                }

                LazadaInitCreateVideoResponseBody objectRes =
                            JsonConvert.DeserializeObject<LazadaInitCreateVideoResponseBody>(response.Body,
                            Common.jsonSerializersettings);

                return objectRes;
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
            }
            return null;
        }

        // The API is used to upload one block of origin video file. 
        // The video file can split into multiple files. For example, a 8MB video file can be split
        // into three blocks. 3MB, 3MB and 2MB. These three blocks can be uploaded by calling
        // UploadVideoBlock three times.
        public static LazadaUploadVideoBlockResponseBody LazadaUploadVideoBlock(
            string uploadId,
            int blockNo,
            int blockCount,
            byte[] file)
        {
            try
            {
                if (!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
                {
                    return null;
                }

                ILazopClient client = CommonLazadaAPI.GetLazopClient();
                LazopRequest request = new LazopRequest();
                request.SetApiName("/media/video/block/upload");

                // return by calling InitCreateVideo
                request.AddApiParameter("uploadId", uploadId);
                // the current block number, from 0 to N-1
                request.AddApiParameter("blockNo", blockNo.ToString());
                // total block count of file
                request.AddApiParameter("blockCount", blockCount.ToString());
                //  binary content of the current block
                request.AddFileParameter("file", new FileItem(uploadId + "_" + blockNo.ToString(), file, "video/mp4"));

                LazopResponse response = client.Execute(request,
                        LazadaAuthenAPI.lazadaAuthen.accessToken);
                MyLogger.LazadaRestLog(request, response);
                //Thread.Sleep(500);
                if (response.IsError())
                {
                    return null;
                }

                LazadaUploadVideoBlockResponseBody objectRes =
                            JsonConvert.DeserializeObject<LazadaUploadVideoBlockResponseBody>(response.Body,
                            Common.jsonSerializersettings);

                return objectRes;
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
            }
            return null;
        }

        // After uploading all blocks of the video file, call CompleteCreateVideo to complete
        // the video uploading process.
        public static LazadaCompleteCreateVideoResponseBody LazadaCompleteCreateVideo(
            string uploadId,
            string parts, // "[{\"partNumber\":1,\"eTag\":\"AB693ADF0DF340F50637686D65CC062C\"},{\"partNumber\":2,\"eTag\":\"557C398778A948415C388B347509CE1C\"}]"
            string title,
            string coverUrl, // "https://sg-live-02.slatic.net/p/ae0f37dbf1c0ef8c560a0f0cfbaac3b6.png"
            string videoUsage) // Có thể không dùng
        {
            try
            {
                if (!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
                {
                    return null;
                }

                ILazopClient client = CommonLazadaAPI.GetLazopClient();
                LazopRequest request = new LazopRequest();
                request.SetApiName("/media/video/block/commit");

                // return by calling InitCreateVideo
                request.AddApiParameter("uploadId", uploadId);

                // a json string contains e_tag info of each block
                request.AddApiParameter("parts", parts);

                // the video title
                request.AddApiParameter("title", title);

                // the url of the video's cover image
                request.AddApiParameter("coverUrl", coverUrl);

                // Không bắt buộc nên bỏ qua
                //request.AddApiParameter("videoUsage", "pro_main_video");

                LazopResponse response = client.Execute(request,
                        LazadaAuthenAPI.lazadaAuthen.accessToken);
                MyLogger.LazadaRestLog(request, response);
                //Thread.Sleep(500);
                if (response.IsError())
                {
                    return null;
                }

                LazadaCompleteCreateVideoResponseBody objectRes =
                            JsonConvert.DeserializeObject<LazadaCompleteCreateVideoResponseBody>(response.Body,
                            Common.jsonSerializersettings);

                return objectRes;
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
            }
            return null;
        }

        // You call this action to get video info after uploading.
        public static LazadaGetVideoResponseBody LazadaGetVideo(string videoId)
        {
            try
            {
                if (!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
                {
                    return null;
                }

                ILazopClient client = CommonLazadaAPI.GetLazopClient();
                LazopRequest request = new LazopRequest();
                request.SetHttpMethod("GET");
                // A seller starts to upload a video file
                request.SetApiName("/media/video/get");

                // the previous return value by calling CompleteCreateVideo
                request.AddApiParameter("videoId", videoId);

                LazopResponse response = client.Execute(request,
                        LazadaAuthenAPI.lazadaAuthen.accessToken);
                MyLogger.LazadaRestLog(request, response);
                if (response.IsError())
                {
                    return null;
                }

                LazadaGetVideoResponseBody objectRes =
                            JsonConvert.DeserializeObject<LazadaGetVideoResponseBody>(response.Body,
                            Common.jsonSerializersettings);

                return objectRes;
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
            }
            return null;
        }

        // You call this api to get the capacity quota of seller.
        public static LazadaVideoQuotaResponseBody LazadaGetVideoQuota()
        {
            try
            {
                if (!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
                {
                    return null;
                }

                ILazopClient client = CommonLazadaAPI.GetLazopClient();
                LazopRequest request = new LazopRequest();
                request.SetApiName("/media/video/quota/get");
                request.SetHttpMethod("GET");

                LazopResponse response = client.Execute(request,
                        LazadaAuthenAPI.lazadaAuthen.accessToken);
                MyLogger.LazadaRestLog(request, response);
                if (response.IsError())
                {
                    return null;
                }

                LazadaVideoQuotaResponseBody objectRes =
                            JsonConvert.DeserializeObject<LazadaVideoQuotaResponseBody>(response.Body,
                            Common.jsonSerializersettings);

                return objectRes;
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
            }
            return null;
        }

        // Bí kíp đăng sản phẩm nhiều skus
        // https://open.lazada.com/apps/community/detail?spm=a1zq7z.27196124.0.0.10247c73Klt9Jq&id=121826
        // {"Request":{"Product":{"Images":{"Image":["https://sg-test-11.slatic.net/p/eebaf5c4c7308e8982309d22193ac144.jpg","https://sg-test-11.slatic.net/p/b64a84965f1f4f5e72862cf428a1cad1.jpg","https://sg-test-11.slatic.net/p/797421998482b57dc6142298d00a1023.jpg","https://sg-test-11.slatic.net/p/0b7cfde57b2f5ffab8dd8847f76ffe28.jpg","https://sg-test-11.slatic.net/p/53f36f5340d80336f0edb8d93612427a.jpg","https://sg-test-11.slatic.net/p/777ad4513739f2d553bbec2fab2b4b23.jpg","https://sg-test-11.slatic.net/p/3e277938104685c49b5b52693360192a.jpg","https://sg-test-11.slatic.net/p/4b7e5b08e9099aa3ff20952c9102d9c5.jpg"]},"Skus":{"Sku":[{"Images":{"Image":["https://sg-test-11.slatic.net/p/8d160004869e2adc275a847b389fe0fb.jpg"]},"SellerSku":"VBN20251008082817bWUyD","package_height":"10","package_length":"10","package_weight":"0.1","package_width":"10","price":"156000","quantity":"0","saleProp":{"TenSach":"COMBO 4 CUỐN"}},{"Images":{"Image":["https://sg-test-11.slatic.net/p/ebad379e4632a59a85e2642ee837174d.jpg"]},"SellerSku":"VBN20251008082818zMbWI","package_height":"10","package_length":"10","package_weight":"0.1","package_width":"10","price":"39000","quantity":"0","saleProp":{"TenSach":"Bữa tiệc sắc màu"}}]},"PrimaryCategory":"8666","Attributes":{"author":"Suzuki Mio","language":"Vietnamese","name":"Sách Ehon - Điều kì diệu của màu sắc ( Bộ 4 cuốn + lẻ tùy chọn) cho bé 0-6 tuổi","description":"<p>Sách Ehon - Điều kì diệu của màu sắc ( Bộ 4 cuốn + lẻ tùy chọn) cho bé 0-6 tuổi</p><p></p><p>Mô tả sản phẩm - Giới thiệu sách:</p><p>Theo các nhà nghiên cứu giáo dục, trẻ em luôn có những niềm hứng khởi bất tận với thiên nhiên, hình khối và màu sắc. Và nhiệm vụ đưa những “bài học” về hình khối, màu sắc vào giai đoạn đầu đời của bố mẹ là rất vô cùng cần thiết.</p><p></p><p>Bộ Ehon màu sắc gồm 4 cuốn: \"Bữa tiệc sắc màu của thú trắng\", \"Ơ! Tắc kè là nhà ảo thuật\", \"Một ngày của bạch tuộc\" và \"Có gì trong quả trứng?\". Cốt truyện của mỗi cuốn sách rất đơn giản, là sự lặp đi lặp lại của hành động, ví dụ như các bạn thú trắng ăn quả gì thì thân hình sẽ biến thành màu quả ấy, bạn tắc kè đi đến đâu thì màu da biến đổi theo môi trường, ... Nhờ đó, mạch câu chuyện sẽ dễ dàng thâm nhập sâu vào tiềm thức của trẻ, hình thành những khái niệm cơ bản nhất về màu sắc.</p><img src=\"https://sg-test-11.slatic.net/p/439e667e0be089e85b9ce94c210da3d3.jpg\"/><p>Sứ mệnh của bộ sách Ehon Màu sắc là phát triển chức năng thùy chẩm của trẻ - một bộ phận nằm phía sau của bộ não con người – là nơi tiếp nhận thông tin đến từ thị giác. Thùy chẩm thu nhận các hình ảnh, sắc màu, hình khối bằng mắt, giúp phát triển khả năng tổng hợp, phân tích về giác quan, cầm nắm nhận dạng vật thể, nhận diện không gian.</p><p></p><p>Cha mẹ có thể nuôi dưỡng năng khiếu nghệ thuật của con trẻ bằng phương pháp tự nhiên nhất là cho con tiếp xúc với ehon, đồng thời giúp bé có thể học tốt các môn học liên quan đến nghệ thuật như văn học, hội họa, âm nhạc...</p><img src=\"https://sg-test-11.slatic.net/p/7bfe31d91a300a9dcd21177d78073b93.jpg\"/><img src=\"https://sg-test-11.slatic.net/p/46cfb6b9a124b12821f9ab0d3697ad75.jpg\"/><img src=\"https://sg-test-11.slatic.net/p/eb9ed60f3ce198bb170edbe798678999.jpg\"/><p>#sachchobe #sachtreem #sáchchobé #sáchtrẻem #wabooks #ehon #ehonnhatban #sachnuoidaycon #sáchnuôidậycon #sáchthiếunhi #sachthieunhi</p>","brand":"Wabooks","brand_id":"184898","number_of_pages":"28","version":"Đầy đủ","isbn_issn":"9786046546900","video":"8000078826877"},"variation":{"Variation1":{"customize":true,"hasImage":true,"label":"TenSach","name":"TenSach","options":{"option":["COMBO 4 CUỐN","Bữa tiệc sắc màu"]}}}}}}
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
            // Nếu có variation thì cần thay thế tiếng anh bằng tiếng Việt.
            //Boolean hasVariation = false;
            if (requestPara.Request.Product.variation != null)
            {
                string variationName = requestPara.Request.Product.variation.Variation1.name;
                payload = payload.Replace("valueOfVariation", variationName);
                //hasVariation = true;
            }
            request.AddApiParameter("payload", payload);

            LazadaCreateProductResponseBody objectRes = null;

            try
            {
                LazopResponse response = null;
                //if (!hasVariation)
                //{
                    response = client.Execute(request,
                            LazadaAuthenAPI.lazadaAuthen.accessToken);
                //}
                //else
                //{
                //    response = ((LazopClient)client).DoExecuteJson(request,
                //            LazadaAuthenAPI.lazadaAuthen.accessToken);
                //}
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

        public static void LazadaCreateProductTest( string payload)
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
            //string payload = "{    \"Request\": {        \"Product\": {            \"AssociatedSku\": \"Existing SkuId in seller center\",            \"Attributes\": {                \"Hazmat\": \"None\",                \"brand\": \"No Brand\",                \"brand_id\": \"30768\",                \"delivery_option_sof\": \"No\",                \"description\": \"TEST\",                \"disableAttributeAutoFill\": false,                \"gift_wrapping\": \"Yes\",                \"laptop_size\": \"11 - 12 inches\",                \"material\": \"Leather\",                \"model\": \"test\",                \"name\": \"test 2022 02\",                \"name_engravement\": \"Yes\",                \"preorder_days\": \"25\",                \"preorder_enable\": \"Yes\",                \"propCascade\": {                    \"26\": \"120013644:162,100006867:160387\"                },                \"short_description\": \"cm x 1efgtecm<br /><brfwefgtek\",                \"warranty\": \"1 Month\",                \"warranty_type\": \"Local seller warranty\",                \"waterproof\": \"Waterproof\"            },            \"Images\": {                \"Image\": [\"https://my-live-02.slatic.net/p/47b6cb07bd8f80aa3cc34b180b902f3e.jpg\"]            },            \"PrimaryCategory\": \"10002019\",            \"Skus\": {                \"Sku\": [{                        \"Images\": {                            \"Image\": [\"https://my-live-02.slatic.net/p/47b6cb07bd8f80aa3cc34b180b902f3e.jpg\"]                        },                        \"SellerSku\": \"test2022 02\",                        \"package_content\": \"laptop bag\",                        \"package_height\": \"10\",                        \"package_length\": \"10\",                        \"package_weight\": \"0.5\",                        \"package_width\": \"10\",                        \"price\": \"35\",                        \"quantity\": \"3\",                        \"saleProp\": {                            \"color_family\": \"Green\",                            \"size\": \"10\"                        },                        \"special_from_date\": \"2022-06-20 17:18:31\",                        \"special_price\": \"33\",                        \"special_to_date\": \"2025-03-15 17:18:31\"                    }                ]            }        }    }}";
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

        // Từ Item (thường là sản phẩm chép trên web), ta upload ảnh, video lên server lazada
        public static async Task<MySqlResultState> LazadaUploadMediaFromItemForCreate(
            ItemForCreate item, MySqlConnection conn)
        {
            MyLogger.GetInstance().Info("LazadaUploadMediaFromItemForCreate Call");
            MySqlResultState result = new MySqlResultState();
            // Up ảnh của item
            foreach(var path in item.pathImages)
            {
                LazadaUploadImage up = LazadaUploadImage(path);
                if(up != null)
                {
                    item.srcImagesTo.Add(up.url);
                }
                else
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "LazadaUploadImage fail of " + path;
                    return result;
                }
            }

            // Up ảnh của models
            foreach(var model in item.models)
            {
                foreach (var path in model.pathImages)
                {
                    LazadaUploadImage up = LazadaUploadImage(path);
                    if (up != null)
                    {
                        model.srcImagesTo.Add(up.url);
                    }
                    else
                    {
                        result.State = EMySqlResultState.ERROR;
                        result.Message = "LazadaUploadImage fail of " + path;
                        return result;
                    }
                }
            }

            // Up ảnh trong description
            foreach(var des in item.descriptions)
            {
                if (!des.isText)
                {
                    LazadaUploadImage up = LazadaUploadImage(des.path);
                    if (up != null)
                    {
                        des.srcImageTo = up.url;
                    }
                    else
                    {
                        result.State = EMySqlResultState.ERROR;
                        result.Message = "LazadaUploadImage fail of " + des.path;
                        return result;
                    }
                }
            }

            // Up video nếu có
            if(!string.IsNullOrEmpty(item.pathVideo))
            {
                result = await LazadaUploadVideo(item.name, item.pathVideo, conn);
                if(result.State == EMySqlResultState.OK)
                {
                    item.idVideo = result.Message;
                }
                // Nếu upload video lỗi ta vẫn tạo sản phẩm, sau đó thêm video thủ công
                else
                {
                    result.State = EMySqlResultState.OK;
                    result.Message = "";
                }
            }
            return result;
        }
    }
}
