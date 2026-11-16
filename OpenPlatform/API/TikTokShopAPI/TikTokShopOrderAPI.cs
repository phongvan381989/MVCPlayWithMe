using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.TikTokApp;
using MVCPlayWithMe.OpenPlatform.Model.TikTokApp.TikTokAuth;
using MVCPlayWithMe.OpenPlatform.Model.TikTokApp.TikTokOrder;
using MySqlConnector;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.API.TikTokShopAPI
{
    /// <summary>
    /// TikTok Shop Order API
    /// </summary>
    public class TikTokShopOrderAPI
    {
        private const string ORDER_LIST_PATH = "/order/202309/orders/search";

        /// <summary>
        /// Get Order List from TikTok Shop with comprehensive filters
        /// API: POST /order/202309/orders/search
        /// </summary>
        /// <param name="shopCipher">Shop cipher (optional, defaults to tikTokAuth.ShopCipher)</param>
        /// <param name="pageSize">Number of results per page (1-100, default 20)</param>
        /// <param name="pageToken">Token for next page (from previous response)</param>
        /// <param name="sortField">Sort field: create_time or update_time (default: create_time)</param>
        /// <param name="sortOrder">Sort order: ASC or DESC (default: DESC)</param>
        /// <param name="createTimeGe">Filter orders created on or after this time (Unix timestamp)</param>
        /// <param name="createTimeLt">Filter orders created before this time (Unix timestamp)</param>
        /// <param name="updateTimeGe">Filter orders updated on or after this time (Unix timestamp)</param>
        /// <param name="updateTimeLt">Filter orders updated before this time (Unix timestamp)</param>
        /// <param name="orderStatus">Filter by order status (use TikTokOrderStatus constants)</param>
        /// <param name="isBuyerRequestCancel">Filter by buyer cancellation request status</param>
        /// <param name="shippingType">Filter by shipping type: TIKTOK or SELLER</param>
        /// <param name="buyerUserId">Filter by buyer user ID</param>
        /// <param name="warehouseIds">Filter by warehouse IDs (max 100)</param>
        /// <returns>Order list response</returns>
        public static async Task<TikTokOrderListResponse> GetOrderListAsync_OnePage(
            string shopCipher = null,
            int pageSize = 20,
            string pageToken = null,
            string sortField = null,
            string sortOrder = null,
            long? createTimeGe = null,
            long? createTimeLt = null,
            long? updateTimeGe = null,
            long? updateTimeLt = null,
            string orderStatus = null,
            bool? isBuyerRequestCancel = null,
            string shippingType = null,
            string buyerUserId = null,
            string[] warehouseIds = null)
        {
            try
            {
                // Check and refresh token if needed
                bool isTokenValid = await TikTokAuthAPI.CheckAndRefreshTokenIfNeededAsync();
                if (!isTokenValid)
                {
                    MyLogger.GetInstance().Error("TikTok auth not found or refresh failed");
                    return null;
                }

                // Use provided shopCipher or fallback to global auth
                string effectiveShopCipher = shopCipher ?? TikTokAuthAPI.tikTokAuth.ShopCipher;
                if (string.IsNullOrEmpty(effectiveShopCipher))
                {
                    MyLogger.GetInstance().Error("Shop cipher is required");
                    return null;
                }

                // Build request body with filters
                var requestBody = new Dictionary<string, object>();

                // Time filters
                if (createTimeGe.HasValue)
                    requestBody["create_time_ge"] = createTimeGe.Value;

                if (createTimeLt.HasValue)
                    requestBody["create_time_lt"] = createTimeLt.Value;

                if (updateTimeGe.HasValue)
                    requestBody["update_time_ge"] = updateTimeGe.Value;

                if (updateTimeLt.HasValue)
                    requestBody["update_time_lt"] = updateTimeLt.Value;

                // Status filters
                if (!string.IsNullOrEmpty(orderStatus))
                    requestBody["order_status"] = orderStatus;

                if (isBuyerRequestCancel.HasValue)
                    requestBody["is_buyer_request_cancel"] = isBuyerRequestCancel.Value;

                // Type filters
                if (!string.IsNullOrEmpty(shippingType))
                    requestBody["shipping_type"] = shippingType;

                // User filter
                if (!string.IsNullOrEmpty(buyerUserId))
                    requestBody["buyer_user_id"] = buyerUserId;

                // Warehouse filter
                if (warehouseIds != null && warehouseIds.Length > 0)
                    requestBody["warehouse_ids"] = warehouseIds;

                string requestBodyJson = JsonConvert.SerializeObject(requestBody);

                // Query parameters
                var queryParams = new Dictionary<string, string>
                {
                    { "app_key", TikTokAuthAPI.tikTokAuth.AppKey },
                    { "timestamp", TikTokSigningHelper.GetTimestamp().ToString() },
                    { "shop_cipher", effectiveShopCipher },
                    { "page_size", pageSize.ToString() }
                };

                // Pagination
                if (!string.IsNullOrEmpty(pageToken))
                    queryParams["page_token"] = pageToken;

                // Sorting
                if (!string.IsNullOrEmpty(sortField))
                    queryParams["sort_field"] = sortField;

                if (!string.IsNullOrEmpty(sortOrder))
                    queryParams["sort_order"] = sortOrder;

                // Generate signature with request body
                string signature = TikTokSigningHelper.GenerateSignature(
                    TikTokAuthAPI.tikTokAuth.AppSecret,
                    ORDER_LIST_PATH,
                    queryParams,
                    requestBodyJson
                );

                // Build URL with signature
                var urlParams = new List<string>();
                foreach (var param in queryParams)
                {
                    urlParams.Add($"{param.Key}={Uri.EscapeDataString(param.Value)}");
                }
                urlParams.Add($"sign={signature}");

                string url = $"{TikTokSigningHelper.BASE_URL}{ORDER_LIST_PATH}?{string.Join("&", urlParams)}";

                // Execute POST request
                RestRequest request = new RestRequest(url, Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("x-tts-access-token", TikTokAuthAPI.tikTokAuth.AccessToken);
                request.AddParameter("application/json", requestBodyJson, ParameterType.RequestBody);

                IRestResponse response = Common.client.Execute(request);
                MyLogger.InfoRestLog(Common.client, request, response);

                if (!response.IsSuccessful)
                {
                    MyLogger.GetInstance().Error($"TikTok GetOrderList failed: {response.StatusCode} - {response.Content}");
                    return null;
                }

                // Parse response
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };

                TikTokOrderListResponse orderListResponse =
                    JsonConvert.DeserializeObject<TikTokOrderListResponse>(response.Content, settings);

                return orderListResponse;
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"TikTok GetOrderList exception: {ex}");
                return null;
            }
        }

        /// <summary>
        /// Get ALL orders (auto-pagination) by time range and status
        /// Lấy TẤT CẢ đơn hàng theo thời gian và trạng thái
        /// </summary>
        /// <param name="timeRange">Time range: 1 day, 7 days, or 30 days</param>
        /// <param name="orderStatus">
        /// Order status filter:
        /// - null or empty = Tất Cả (All)
        /// - TikTokOrderStatus.AWAITING_SHIPMENT = Cần Gửi Hàng
        /// - TikTokOrderStatus.CANCELLED = Hủy
        /// </param>
        /// <param name="maxPages">Maximum pages to fetch (default 100, safety limit)</param>
        /// <returns>List of all orders matching criteria</returns>
        public static async Task<List<TikTokOrder>> GetAllOrdersAsync(
            TikTokOrderTimeRange timeRange = TikTokOrderTimeRange.SevenDays,
            string orderStatus = null,
            int maxPages = 100)
        {
            try
            {
                MyLogger.GetInstance().Info($"TikTok GetAllOrders - TimeRange: {timeRange}, Status: {orderStatus ?? "ALL"}");

                // Calculate time range
                long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                long startTime = now - ((int)timeRange * 24 * 60 * 60); // Convert days to seconds

                MyLogger.GetInstance().Info($"Fetching orders from {DateTimeOffset.FromUnixTimeSeconds(startTime):yyyy-MM-dd HH:mm} to {DateTimeOffset.FromUnixTimeSeconds(now):yyyy-MM-dd HH:mm}");

                // Collect all orders from all pages
                var allOrders = new List<TikTokOrder>();
                string pageToken = null;
                int pageCount = 0;

                do
                {
                    pageCount++;
                    MyLogger.GetInstance().Info($"Fetching page {pageCount}, pageToken: {pageToken ?? "null"}");

                    var response = await GetOrderListAsync_OnePage(
                        pageSize: 100,
                        pageToken: pageToken,
                        sortField: TikTokOrderSortField.CREATE_TIME,
                        sortOrder: TikTokOrderSortOrder.DESC,
                        createTimeGe: startTime,
                        createTimeLt: now,
                        orderStatus: orderStatus
                    );

                    if (response == null || !response.IsSuccess())
                    {
                        MyLogger.GetInstance().Error($"Failed to get orders on page {pageCount}: {response?.message}");
                        break;
                    }

                    // Add orders from this page
                    if (response.data?.orders != null && response.data.orders.Count > 0)
                    {
                        allOrders.AddRange(response.data.orders);
                        MyLogger.GetInstance().Info($"Page {pageCount}: Got {response.data.orders.Count} orders, total so far: {allOrders.Count}");
                    }
                    else
                    {
                        MyLogger.GetInstance().Info($"Page {pageCount}: No orders returned");
                        break; // No more orders, exit loop
                    }

                    // Get next page token
                    pageToken = response.data?.next_page_token;

                    // Safety check to prevent infinite loop
                    if (pageCount >= maxPages)
                    {
                        MyLogger.GetInstance().Warn($"Reached max page limit ({maxPages}), stopping pagination");
                        break;
                    }

                } while (!string.IsNullOrEmpty(pageToken));

                MyLogger.GetInstance().Info($"Successfully fetched {allOrders.Count} orders in {pageCount} page(s)");
                return allOrders;
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"TikTok GetAllOrders exception: {ex}");
                return new List<TikTokOrder>();
            }
        }

        /// <summary>
        /// Get ALL orders (auto-pagination) by custom time range and status
        /// Lấy TẤT CẢ đơn hàng theo khoảng thời gian tùy chỉnh
        /// </summary>
        /// <param name="daysBack">Number of days to look back (e.g., 1, 7, 30)</param>
        /// <param name="orderStatus">Order status filter (null = all)</param>
        /// <param name="maxPages">Maximum pages to fetch (default 100)</param>
        /// <returns>List of all orders matching criteria</returns>
        public static async Task<List<TikTokOrder>> GetAllOrdersAsync(
            int daysBack,
            string orderStatus = null,
            int maxPages = 100)
        {
            // Convert days to enum for logging consistency
            TikTokOrderTimeRange timeRange;
            switch (daysBack)
            {
                case 1:
                    timeRange = TikTokOrderTimeRange.OneDay;
                    break;
                case 7:
                    timeRange = TikTokOrderTimeRange.SevenDays;
                    break;
                case 30:
                    timeRange = TikTokOrderTimeRange.ThirtyDays;
                    break;
                default:
                    timeRange = (TikTokOrderTimeRange)daysBack; // Custom value
                    break;
            }

            return await GetAllOrdersAsync(timeRange, orderStatus, maxPages);
        }

    }
}
