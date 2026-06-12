using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.Dev;
using MVCPlayWithMe.Models.ProductModel;
using MVCPlayWithMe.OpenPlatform;
using MVCPlayWithMe.OpenPlatform.API.LazadaAPI;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeCreateProduct;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeOrder;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Category;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Event;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Product;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaOrder;
using MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeCreateProduct;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static MVCPlayWithMe.General.Common;
using static MVCPlayWithMe.OpenPlatform.CommonOpenPlatform;
using static MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order.TikiOrderItemFilterByDate;

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
        public async Task<ActionResult> Index()
        {
            if (await AuthentAdministratorAsync() == null)
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
        public async Task<string> ShopeeSaveImageSourceOfItemAndModel()
        {
            if (await AuthentAdministratorAsync() == null)
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
                    await conn.OpenAsync();
                    List<long> lsItem = await shopeeSqler.GetForSaveImageSourceConnectOutAsync(conn);
                    foreach (var itemId in lsItem)
                    {
                        ShopeeGetItemBaseInfoItem pro =
                            await ShopeeGetItemBaseInfo.ShopeeProductGetItemBaseInfoFromIdAsync(itemId);
                        if (pro != null)
                        {
                            lsCommonItem.Add(await CommonItem.CommonItemFromShopeeGetItemBaseInfoItemAsync(pro));
                        }
                    }

                    await shopeeSqler.UpdateImageSourceTotbShopeeItem_ModelConnectOutAsync(lsCommonItem, conn);
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> ShopeeGetBrandList(long categoryId)
        {
            if (await AuthentAdministratorAsync() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            try
            {
                ShopeeMySql shopeeSqler = new ShopeeMySql();
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    result = await ShopeeBrand.ShopeeGetBrandList(categoryId, shopeeSqler, conn);
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> ShopeeGetChannelList()
        {
            if (await AuthentAdministratorAsync() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            try
            {
                List<ShopeeLogisticInfo> ls = await ShopeeLogistic.GetLogisticInfoAsync(true);
                if (ls.Count == 0)
                {
                    result.State = EMySqlResultState.INVALID;
                    result.Message = "Không lấy được kênh vận chuyển.";
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> LazadaUpdateQuantityAll()
        {
            if (await AuthentAdministratorAsync() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            try
            {
                LazadaMySql lazadaSQLer = new LazadaMySql();
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    List<CommonItem> ls = await lazadaSQLer.LazadaGetItemOnDBAsync(conn);
                    await ProductController.LazadaUpdateQuantity_CoreAsync(ls);

                    // Lấy danh sách item cập nhật số lượng sai
                    List<CommonItem> failLs = new List<CommonItem>();
                    foreach (var item in ls)
                    {
                        foreach (var model in item.models)
                        {
                            if (!string.IsNullOrEmpty(model.whyUpdateFail))
                            {
                                failLs.Add(item);
                                break;
                            }
                        }
                        if (failLs.Count > 0 && failLs.Last().itemId == item.itemId)
                        {
                            break;
                        }
                    }

                    if (failLs.Count > 0)
                    {
                        result.State = EMySqlResultState.ERROR;
                        result.Message = "Có item cập nhật lỗi";
                        result.myJson = JsonConvert.SerializeObject(failLs);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return JsonConvert.SerializeObject(result);
        }

        // Cập nhật giá bìa price, và giá bán spacial_price tất cả sản phẩm
        [HttpPost]
        public async Task<string> LazadaUpdatePrice_SpecialPriceAll()
        {
            if (await AuthentAdministratorAsync() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            try
            {
                LazadaMySql lazadaSQLer = new LazadaMySql();
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    List<CommonItem> ls = await lazadaSQLer.LazadaGetItemOnDBAsync(conn);

                    ProductController.LazadaUpdatePrice_SpecialPrice_Core(ls, conn);

                    // Lấy danh sách item cập nhật số lượng sai
                    List<CommonItem> failLs = new List<CommonItem>();
                    foreach (var item in ls)
                    {
                        foreach (var model in item.models)
                        {
                            if (!string.IsNullOrEmpty(model.whyUpdateFail))
                            {
                                failLs.Add(item);
                                break;
                            }
                        }
                        if (failLs.Count > 0 && failLs.Last().itemId == item.itemId)
                        {
                            break;
                        }
                    }

                    if (failLs.Count > 0)
                    {
                        result.State = EMySqlResultState.ERROR;
                        result.Message = "Có item cập nhật lỗi";
                        result.myJson = JsonConvert.SerializeObject(failLs);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> LazadaGetCategoryTree()
        {
            if (await AuthentAdministratorAsync() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            try
            {
                await LazadaProductAPI.LazadaGetCategoryTreeAsync();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> LazadaGetCategoryAttributes(int categoryId)
        {
            if (await AuthentAdministratorAsync() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            try
            {
                await LazadaProductAPI.LazadaGetCategoryAttributesAsync(categoryId);
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> LazadaGetBrandByPages()
        {
            if (await AuthentAdministratorAsync() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            try
            {
                Boolean isError = false;
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    isError = await LazadaProductAPI.LazadaGetBrandByPagesAsync(conn);
                }
                if (isError)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Có lỗi";
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> TikiSaveImageSourceOfItemAndModel()
        {
            if (await AuthentAdministratorAsync() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            List<CommonItem> lsCommonItem = new List<CommonItem>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    TikiMySql tikiSqler = new TikiMySql();
                    List<int> lsItemId = await tikiSqler.GetForSaveImageSourceConnectOutAsync(conn);

                    foreach (var itemId in lsItemId)
                    {
                        TikiProduct pro = await GetListProductTiki.GetProductFromOneShop(itemId);
                        if (pro == null)
                        {
                            continue;
                        }

                        lsCommonItem.Add(new CommonItem(pro));
                    }
                    await tikiSqler.UpdateImageSourceTotbTikiItemConnectOutAsync(lsCommonItem, conn);
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> TikiTestPullEvent()
        {
            if (await AuthentAdministratorAsync() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();

            //TikiPullEventService tikiPullEventService = new TikiPullEventService();
            //tikiPullEventService.DoWork();

            return JsonConvert.SerializeObject(result);
        }

        private void TikiTestSomething1()
        {
            //MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            //conn.Open();
            //CommonOrder commonOrder = new CommonOrder();
            //commonOrder.code = "ABCDEDFTest";
            //commonOrder.listItemId.Add(1000);
            //commonOrder.listItemId.Add(2000);

            //commonOrder.listModelId.Add(1);
            //commonOrder.listModelId.Add(2);

            //commonOrder.listQuantity.Add(1);
            //commonOrder.listQuantity.Add(1);

            //TikiMySql tikiSqler = new TikiMySql();
            //tikiSqler.InsertTbItemOfEcommerceOderAsync(commonOrder, EECommerceType.TIKI, conn);

            //tikiSqler.InsertTbItemOfEcommerceOderAsync(commonOrder, EECommerceType.SHOPEE, conn);

            //tikiSqler.UpdateCancelledStatusTbItemOfEcommerceOderAsync(commonOrder, EECommerceType.TIKI, conn);

            //tikiSqler.UpdateCancelledStatusTbItemOfEcommerceOderAsync(commonOrder, EECommerceType.SHOPEE, conn);
            //conn.Close();
        }

        private async Task RecursionGetCategoryOfTiki(int id,
            string parrentName,
            List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category.TikiCategory> ls)
        {
            List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category.TikiCategory> lsTem = await TikiCategoryAction.GetChildrenCategory(id);
            if (lsTem == null || lsTem.Count == 0)
            {
                return;
            }

            Thread.Sleep(1000);
            foreach (var category in lsTem)
            {
                if (category.is_primary == false)
                {
                    if (!string.IsNullOrWhiteSpace(parrentName))
                    {
                        category.name = parrentName + ">" + category.name;
                    }
                    RecursionGetCategoryOfTiki(category.id, category.name, ls);
                }
                else
                {
                    category.name = parrentName + ">" + category.name;
                    ls.Add(category);
                }
            }
        }

        // Lấy category của Tiki phục vụ đăng sản phẩm tự động cho sàn Tiki
        // NOTE: Gọi 1 lần DUY NHẤT TRONG ĐỜI, gọi lần sau dữ liệu sẽ bị duplicate
        private async Task GetCategoryOfTiki()
        {
            // id: 8322 - Tương ứng Category cụ tổ : Nhà Sách Tiki
            // Ta đi lấy đến đời thấp nhất: is_primary = true;
            //int greatFather = 8322;// "Nhà Sách Tiki"
            //int greatFather = 393;// "Sách thiếu nhi "
            int greatFather = 316; // "Sách tiếng Việt"
            List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category.TikiCategory> ls =
                new List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category.TikiCategory>();

            RecursionGetCategoryOfTiki(greatFather, "Sách tiếng Việt", ls);
            MyLogger.GetInstance().Info(JsonConvert.SerializeObject(ls));
            // Lưu db
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    TikiMySql tikiSqler = new TikiMySql();
                    await tikiSqler.InsertTbTikiCategoryAsync(ls, conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        // NOTE: Gọi 1 lần DUY NHẤT TRONG ĐỜI, gọi lần sau dữ liệu sẽ bị duplicate
        private async Task InsertData_AttributeOfCategory()
        {
            // Lấy danh sách category id
            List<int> categoryIdList = new List<int>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    TikiMySql tikiSqler = new TikiMySql();
                    categoryIdList = await tikiSqler.GetCatetoryIdListAsync(conn);

                    List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category.TikiAttribute> attributeListGeneral =
    new List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category.TikiAttribute>();
                    foreach (var categoryId in categoryIdList)
                    {
                        // Lấy attribute mà category có thể có
                        List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category.TikiAttribute> attributeList =
                            await TikiCategoryAction.GetAttributeOfCategory(categoryId);

                        Thread.Sleep(1000);
                        foreach (var attr in attributeList)
                        {
                            // Lấy những attribute thỏa mãn "is_required": true
                            if (attr.is_required)
                            {
                                attr.category_id = categoryId;
                                attributeListGeneral.Add(attr);
                            }
                        }
                    }

                    // Lưu vào db
                    await tikiSqler.InsertTikiAttributesOfCategoryAsync(attributeListGeneral, conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        // Từ category sản phẩm trên tiki đã mapping với sản phẩm trong kho,
        // ta cập nhật category Id sản phẩm trong kho
        private async Task UpdateCategoryIdOfProductAsync()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    List<int> listProId = new List<int>();
                    // Lấy id sản phẩm trong kho chưa có category Id (-1), trạng thái bình thường
                    {
                        MySqlCommand cmd = new MySqlCommand(
                        @"SELECT Id FROM webplaywithme.tbproducts WHERE CategoryId = -1 AND Status <> 2 ORDER BY Id;",
                    conn);
                        cmd.CommandType = CommandType.Text;
                        using (MySqlDataReader reader = (MySqlDataReader) await cmd.ExecuteReaderAsync())
                        {
                            int idOrdinal = reader.GetOrdinal("Id");
                            while (await reader.ReadAsync())
                            {
                                listProId.Add(reader.GetInt32(idOrdinal));
                            }
                        }
                    }

                    List<int> listTikiCategoryId = new List<int>();
                    // Lấy tiki category Id
                    {
                        MySqlCommand cmd = new MySqlCommand(
                        @"SELECT TikiCategoryId FROM webplaywithme.tbcategory;",
                    conn);
                        cmd.CommandType = CommandType.Text;
                        using (MySqlDataReader reader =(MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            int tikiCategoryIdOrdinal = reader.GetOrdinal("TikiCategoryId");
                            while (await reader.ReadAsync())
                            {
                                listTikiCategoryId.Add(reader.GetInt32(tikiCategoryIdOrdinal));
                            }
                        }
                    }

                    int count = 0;
                    while (listProId.Count > 0 && count <= 1000)
                    {
                        count++;
                        // Lấy id sản phẩm trên tiki trạng thái bình thường / đã tắt, đã mapping và danh sách id sản phẩm trong kho
                        // được mapping
                        int tikiId = 0;
                        List<int> listProIdMapping = new List<int>();
                        {
                            MySqlCommand cmd = new MySqlCommand(
                            @"SELECT tbtikiitem.TikiId, tbtikimapping.ProductId
                            FROM  tbtikiitem LEFT JOIN tbtikimapping ON tbtikiitem.Id = tbtikimapping.TikiItemId
                            WHERE tbtikiitem.Id =
                            (
	                            SELECT tbtikiitem.Id
	                            FROM tbtikimapping LEFT JOIN tbtikiitem ON tbtikimapping.TikiItemId = tbtikiitem.Id
	                            WHERE tbtikimapping.ProductId = @inProductId AND tbtikiitem.Status = 1 LIMIT 1
                            ) ORDER BY tbtikimapping.ProductId;",
                        conn);
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@inProductId", listProId[0]);
                            using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                            {
                                int tikiIdOrdinal = reader.GetOrdinal("TikiId");
                                int productIdOrdinal = reader.GetOrdinal("ProductId");
                                while (await reader.ReadAsync())
                                {
                                    if (tikiId == 0)
                                    {
                                        tikiId = reader.GetInt32(tikiIdOrdinal);
                                    }
                                    listProIdMapping.Add(reader.GetInt32(productIdOrdinal));
                                }
                            }
                        }

                        // Từ tikiId lấy được category id của sản phẩm trên sàn tiki trong chi tiết sản phẩm
                        TikiProduct pro = await GetListProductTiki.GetProductFromOneShop(tikiId);
                        int tikiCategoryIdTemp = 0;
                        if (pro == null)
                        {
                            listProId.RemoveAt(0);
                        }
                        else
                        {
                            foreach (var cat in pro.categories)
                            {
                                if (listTikiCategoryId.Contains(cat.id))
                                {
                                    tikiCategoryIdTemp = cat.id;
                                    break;
                                }
                            }
                        }

                        // Cập nhật categoryId của tbProducts từ id sản phẩm và TikiCategoryId trong tbCategory
                        {
                            MySqlCommand cmd = new MySqlCommand(
                            @"st_tbProducts_Update_CategoryId_From_TikiCategoryId", conn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@in_productId", 0);
                            cmd.Parameters.AddWithValue("@in_tikicategoryId", tikiCategoryIdTemp);
                            foreach (var proId in listProIdMapping)
                            {
                                cmd.Parameters["@in_productId"].Value = proId;
                                await cmd.ExecuteNonQueryAsync();
                                // Loại bỏ sản phẩm ra khỏi list chờ cập nhật
                                listProId.Remove(proId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        // Thêm mới SellerSku sản phẩm của Lazada
        private async Task<string> LazadaInsertSellerSkuAsync()
        {
            MySqlResultState result = new MySqlResultState();
            List<LazadaProduct> ls = await LazadaProductAPI.GetProductAll();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand(
                        @"UPDATE tb_lazada_model SET SellerSku = @inSellerSku WHERE TMDTLazadaModelId = @inTMDTLazadaModelId;",
                    conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inSellerSku", "");
                    cmd.Parameters.AddWithValue("@inTMDTLazadaModelId", 0L);
                    foreach (var pro in ls)
                    {
                        foreach (var sku in pro.skus)
                        {
                            cmd.Parameters[0].Value = sku.SellerSku;
                            cmd.Parameters[1].Value = sku.SkuId;
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return JsonConvert.SerializeObject(result);
        }

        private async Task LazadaUpdatePrice_SalePrice()
        {
            LazadaMySql lazadaSQLer = new LazadaMySql();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                List<CommonItem> ls = await lazadaSQLer.LazadaGetItemOnDBAsync(conn);
                foreach (var item in ls)
                {
                    if (item.itemId == 3015755086)
                    {
                        List<CommonItem> lsTemp = new List<CommonItem>();
                        lsTemp.Add(item);
                        ProductController.LazadaUpdatePrice_SpecialPrice_Core(lsTemp, conn);
                    }
                }
            }
        }

        public async Task<string> GetListOrderLazada(int fromTo, int orderStatus)
        {
            List<CommonOrder> lsCommonOrder = new List<CommonOrder>();
            if (await AuthentAdministratorAsync() == null)
            {
                return JsonConvert.SerializeObject(lsCommonOrder);
            }
            CommonOrderStatus eOrderStatus = (CommonOrderStatus)orderStatus;

            // Lấy đơn hàng của Lazada
            DateTime time_from, time_to;
            time_from = DateTime.Now;
            time_to = DateTime.Now;
            if ((EnumOrderItemFilterByDate)fromTo == EnumOrderItemFilterByDate.today)
                time_from = time_to.AddDays(-1);
            else if ((EnumOrderItemFilterByDate)fromTo == EnumOrderItemFilterByDate.last7days)
                time_from = time_to.AddDays(-7);
            else if ((EnumOrderItemFilterByDate)fromTo == EnumOrderItemFilterByDate.last30days)
                time_from = time_to.AddDays(-30);

            // Lấy đơn hàng của lazada
            List<LazadaOrder> lsOrderLazadaFullInfo = null;
            if (eOrderStatus == CommonOrderStatus.CANCELLED)
            {
                // Lazada
                lsOrderLazadaFullInfo = await LazadaOrderAPI.LazadaGetOrdersDetailCanceledAsync(time_from);
            }
            else if (eOrderStatus == CommonOrderStatus.READY_TO_SHIP_PROCESSED)
            {
                // Lazada
                lsOrderLazadaFullInfo = await LazadaOrderAPI.LazadaGetOrdersDetailReadyToShipAsync(time_from);
            }
            else
            {
                // Lazada
                lsOrderLazadaFullInfo = await LazadaOrderAPI.LazadaGetOrdersDetailAllAsync(time_from);
            }

            // Lazada
            foreach (var order in lsOrderLazadaFullInfo)
            {
                lsCommonOrder.Add(new CommonOrder(order));
            }
            return JsonConvert.SerializeObject(lsCommonOrder);
        }

        [HttpPost]
        public async Task<string> TikiTestSomething()
        {
            if (await AuthentAdministratorAsync() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();

            //ShopeeGetAttributeTreeResponseHTTP response =
            //    ShopeeCategory.ShopeeGetAttributeTreeOfCategory(101541);

            //Boolean isOK = LazadaProductAPI.UpdateQuantity();
            //List<LazadaOrder> orders = LazadaOrderAPI.LazadaGetOrders(DateTime.Now.AddDays(-100), null);
            //List<LazadaOrderItem> orderItems = LazadaOrderAPI.GetOrderItems(513278818272637);


            //List<LazadaUploadImage> images = new List<LazadaUploadImage>();
            //LazadaUploadImage image = LazadaProductAPI.LazadaUploadImage(@"C:\Users\phong\OneDrive\Desktop\ProductImageTemp\0.png");
            //if(image != null)
            //{
            //    images.Add(image);
            //}
            //LazadaMySql lazadaSQLer = new LazadaMySql();
            //using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            //{
            //    conn.Open();
            //    lazadaSQLer.InserttbLazadaMediaSpace(1234, 0, 0, images, conn);
            //}
            //LazadaProductAPI.LazadaCreateProductTest();

            //GetListOrderLazada(2, 0);

            //DateTime now = DateTime.Now;
            //ShopeeGetBookingDetail.ShopeeOrderGetBookingDetailAll(
            //    now.AddDays(-14),
            //    now,
            //    ShopeeOrderStatus.shopeeBookingStatusArray[(int)ShopeeOrderStatus.EnumShopeeBookingStatus.ALL],
            //    null);

            //Common.ExtractFirstFrame(Common.absoluteForCreateMediaFolderPath + "test.mp4",
            //    Common.absoluteForCreateMediaFolderPath + "1.jpg");

            //Common.DownloadVideoAndSaveWithNameNotExtention("https://down-bs-sg.vod.susercontent.com/api/v4/11110105/mms/vn-11110105-6khw6-m29dapkxu0tu36.16000081730883931.mp4",
            //    Common.absoluteForCreateMediaFolderPath + "0");

            //LazadaProductAPI.LazadaGetVideoQuota();

            //using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            //{
            //    conn.Open();
            //    await LazadaProductAPI.LazadaUploadVideo("Sách vải lalala baby", Common.absoluteForCreateMediaFolderPath + "0.mp4", conn);
            //}

            //string md5Hash = Common.CalculateMd5FileHash(Common.absoluteForCreateMediaFolderPath + "videoThumbnail.jpg");

            //await Common.DownloadVideo(
            //    "https://down-zl-sg.vod.susercontent.com/api/v4/11110105/mms/vn-11110105-6khw6-m29dapkxu0tu36.16000081730883931.mp4",
            //    Common.absoluteForCreateMediaFolderPath + "/0.mp4");

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                result = await LazadaProductAPI.LazadaUploadVideoAsync(
                    "Ehon Buồn ngủ", @"C:\Users\phong\TUNM\Works\WebPlayWithMe\MVCPlayWithMe\MVCPlayWithMe\Media\Temporary\ForCreate\0.mp4",
                    conn);
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<string> TikiTestSomethingWithParameter(string str)
        {
            if (await AuthentAdministratorAsync() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    result.State = EMySqlResultState.EMPTY;
                    result.Message = "Tham số rỗng";
                    return JsonConvert.SerializeObject(result);
                }

                ItemForCreate item = JsonConvert.DeserializeObject<ItemForCreate>(str, Common.jsonSerializersettings);

                ProductController proController = new ProductController();
                result = await proController.LazadaCreateItemFromOther(item);
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return JsonConvert.SerializeObject(result);
        }

        // Khi mở địa chỉ kho mới, tắt kho cũ, tồn kho sản phẩm trên sàn về 0. Ta cần cập nhật số lượng.
        [HttpPost]
        [ValidateInput(false)]
        public async Task<string> TikiChangeQuantityWhenSetupOtherWarehouse()
        {
            if (await AuthentAdministratorAsync() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    List<CommonItem> listCommonItem = await TikiMySql.TikiGetListAllUpdateQuantityConnectOutAsync(conn);
                    foreach (var commonItem in listCommonItem)
                    {
                        ProductController.TikiUpdateQuantityOfOneItem(commonItem);
                        await Task.Delay(500);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
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
        public async Task<string> DeleteDuplicateDataOftbShopeeModel()
        {
            if (await AuthentAdministratorAsync() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(sqler.DeleteDuplicateDataOftbShopeeModel());
        }

        [HttpPost]
        public async Task<string> ShopeeGetAuthorizationURL()
        {
            MySqlResultState result = new MySqlResultState();
            if (await AuthentAdministratorAsync() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            result.Message = CommonShopeeAPI.ShopeeGenerateAuthPartnerUrl();

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> ShopeeSaveLivePartnerKey(string key)
        {
            if (await AuthentAdministratorAsync() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = await CommonShopeeAPI.ShopeeSaveLivePartnerKeyAsync(key);

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> ShopeeSaveCode(string code)
        {
            MySqlResultState result = new MySqlResultState();

            if (await AuthentAdministratorAsync() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            result = await CommonShopeeAPI.ShopeeSaveCode(code);

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> ShopeeGetTokenShopLevelAfterAuthorization()
        {
            MySqlResultState result = new MySqlResultState();
            if (await AuthentAdministratorAsync() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            if (await CommonShopeeAPI.ShopeeGetTokenShopLevelAsync() == null)
            {
                result.State = EMySqlResultState.ERROR;
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> LazadaGetAccessTokenFromCodeForFirst(string code)
        {
            MySqlResultState result = new MySqlResultState();

            if (await AuthentAdministratorAsync() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            Boolean isOk =await LazadaAuthenAPI.LazadaAuthTokenCreateAndSaveDBAsync(code);
            if (!isOk)
            {
                result.State = EMySqlResultState.ERROR;
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> LazadaRefreshAccessToken()
        {
            MySqlResultState result = new MySqlResultState();
            if (await AuthentAdministratorAsync() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            Boolean isOk = await LazadaAuthenAPI.LazadaAuthTokenRefreshAsync();
            if (!isOk)
            {
                result.State = EMySqlResultState.ERROR;
            }

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Sinh file sitemap.xml tĩnh từ danh sách sản phẩm trong DB
        /// </summary>
        [HttpPost]
        public async Task<string> GenerateSitemap()
        {
            MySqlResultState result = new MySqlResultState();
            if (await AuthentAdministratorAsync() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    // Lấy danh sách item active
                    MVCPlayWithMe.Models.ItemModel.ItemModelMySql itemModelsqler = new MVCPlayWithMe.Models.ItemModel.ItemModelMySql();
                    List<MVCPlayWithMe.Models.Item> items = new List<Models.Item>();// await itemModelsqler.GetListItemActiveAsync(); // temporary comment

                    // Tạo XML sitemap
                    string baseUrl = "https://voibenho.com";
                    StringBuilder xml = new StringBuilder();
                    xml.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    xml.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

                    // Trang chủ
                    xml.AppendLine("  <url>");
                    xml.AppendLine("    <loc>" + baseUrl + "/</loc>");
                    xml.AppendLine("    <lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>");
                    xml.AppendLine("    <changefreq>daily</changefreq>");
                    xml.AppendLine("    <priority>1.0</priority>");
                    xml.AppendLine("  </url>");

                    // Trang chính sách
                    xml.AppendLine("  <url>");
                    xml.AppendLine("    <loc>" + baseUrl + "/Policy/Index</loc>");
                    xml.AppendLine("    <lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>");
                    xml.AppendLine("    <changefreq>monthly</changefreq>");
                    xml.AppendLine("    <priority>0.6</priority>");
                    xml.AppendLine("  </url>");

                    // Chi tiết từng sản phẩm
                    foreach (var item in items)
                    {
                        if (item != null && item.id > 0 && !string.IsNullOrWhiteSpace(item.name))
                        {
                            string slug = Common.GenerateSlug(item.name);
                            string slugId = slug + "-" + item.id;

                            xml.AppendLine("  <url>");
                            xml.AppendLine("    <loc>" + baseUrl + "/item/" + System.Web.HttpUtility.UrlPathEncode(slugId) + "</loc>");
                            xml.AppendLine("    <lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>");
                            xml.AppendLine("    <changefreq>weekly</changefreq>");
                            xml.AppendLine("    <priority>0.8</priority>");
                            xml.AppendLine("  </url>");
                        }
                    }

                    xml.AppendLine("</urlset>");

                    // Lưu file vào root folder
                    string sitemapPath = Server.MapPath("~/sitemap.xml");
                    System.IO.File.WriteAllText(sitemapPath, xml.ToString(), Encoding.UTF8);

                    result.Message = "Đã sinh sitemap.xml với " + items.Count + " sản phẩm";
                    MyLogger.GetInstance().Info("Generated sitemap.xml with " + items.Count + " items");
                }
            }
            catch (Exception ex)
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = "Lỗi: " + ex.Message;
                MyLogger.GetInstance().Error("Error generating sitemap: " + ex.ToString());
            }

            return JsonConvert.SerializeObject(result);
        }
    }
}
