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
            List<CommonItem> lsCommonItem = new List<CommonItem>();

            ShopeeMySql shopeeSqler = new ShopeeMySql();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
                    List<long> lsItem = shopeeSqler.GetForSaveImageSourceConnectOut(conn);
                    foreach (var itemId in lsItem)
                    {
                        ShopeeGetItemBaseInfoItem pro =
                            ShopeeGetItemBaseInfo.ShopeeProductGetItemBaseInfoFromId(itemId);
                        if (pro != null)
                        {
                            lsCommonItem.Add(new CommonItem(pro));
                        }
                    }

                    shopeeSqler.UpdateImageSourceTotbShopeeItem_ModelConnectOut(lsCommonItem, conn);
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
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            try
            {
                ShopeeMySql shopeeSqler = new ShopeeMySql();
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
                    //List<ShoppeBrandObject> brandList = 
                    result = await ShopeeBrand.ShopeeGetBrandList(categoryId, shopeeSqler, conn);
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return JsonConvert.SerializeObject(result);
        }

        //
        [HttpPost]
        public async Task<string> ShopeeGetChannelList()
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            try
            {
                List<ShopeeLogisticInfo> ls = ShopeeLogistic.GetLogisticInfo(true);
                if(ls.Count == 0)
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
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            try
            {
                LazadaMySql sqler = new LazadaMySql();
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
                    List<CommonItem> ls = sqler.LazadaGetItemOnDB(conn);
                    ProductController.LazadaUpdateQuantity_Core(ls);

                    // Lấy danh sách item cập nhật số lượng sai
                    List<CommonItem> failLs = new List<CommonItem>();
                    foreach (var item in ls)
                    {
                        foreach(var model in item.models)
                        {
                            if(!string.IsNullOrEmpty(model.whyUpdateFail))
                            {
                                failLs.Add(item);
                                break;
                            }
                        }
                        if(failLs.Count > 0 && failLs.Last().itemId == item.itemId)
                        {
                            break;
                        }
                    }

                    if(failLs.Count > 0)
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
        public async Task<string> LazadaUpdatePrice_SalePriceAll()
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            try
            {
                LazadaMySql sqler = new LazadaMySql();
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
                    List<CommonItem> ls = sqler.LazadaGetItemOnDB(conn);

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
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            try
            {
                LazadaProductAPI.LazadaGetCategoryTree();
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
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            try
            {
                LazadaProductAPI.LazadaGetCategoryAttributes(categoryId);
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
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            try
            {
                Boolean isError = false;
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
                    isError = LazadaProductAPI.LazadaGetBrandByPages(conn);
                }
                if(isError)
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
        public string TikiSaveImageSourceOfItemAndModel()
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            List<CommonItem> lsCommonItem = new List<CommonItem>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
                    TikiMySql tikiSqler = new TikiMySql();
                    List<int> lsItemId = tikiSqler.GetForSaveImageSourceConnectOut(conn);

                    foreach (var itemId in lsItemId)
                    {
                        TikiProduct pro = GetListProductTiki.GetProductFromOneShop(itemId);
                        if (pro == null)
                        {
                            continue;
                        }

                        lsCommonItem.Add(new CommonItem(pro));
                    }
                    tikiSqler.UpdateImageSourceTotbTikiItemConnectOut(lsCommonItem, conn);
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string TikiTestPullEvent()
        {
            if (AuthentAdministrator() == null)
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
            //tikiSqler.InsertTbItemOfEcommerceOder(commonOrder, EECommerceType.TIKI, conn);

            //tikiSqler.InsertTbItemOfEcommerceOder(commonOrder, EECommerceType.SHOPEE, conn);

            //tikiSqler.UpdateCancelledStatusTbItemOfEcommerceOder(commonOrder, EECommerceType.TIKI, conn);

            //tikiSqler.UpdateCancelledStatusTbItemOfEcommerceOder(commonOrder, EECommerceType.SHOPEE, conn);
            //conn.Close();
        }

        private void RecursionGetCategoryOfTiki(int id,
            string parrentName,
            List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category.TikiCategory> ls)
        {
            List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category.TikiCategory> lsTem = TikiCategoryAction.GetChildrenCategory(id);
            if(lsTem == null || lsTem.Count == 0)
            {
                return;
            }

            Thread.Sleep(1000);
            foreach ( var category in lsTem)
            {
                if(category.is_primary == false)
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
        private void GetCategoryOfTiki()
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
                    conn.Open();
                    TikiMySql tikiSqler = new TikiMySql();
                    tikiSqler.InsertTbTikiCategory(ls, conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        // NOTE: Gọi 1 lần DUY NHẤT TRONG ĐỜI, gọi lần sau dữ liệu sẽ bị duplicate
        private void InsertData_AttributeOfCategory()
        {
            // Lấy danh sách category id
            List<int> categoryIdList = new List<int>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
                    TikiMySql tikiSqler = new TikiMySql();
                    categoryIdList = tikiSqler.GetCatetoryIdList(conn);

                    List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category.TikiAttribute> attributeListGeneral =
    new List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category.TikiAttribute>();
                    foreach (var categoryId in categoryIdList)
                    {
                        // Lấy attribute mà category có thể có
                        List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category.TikiAttribute> attributeList =
                            TikiCategoryAction.GetAttributeOfCategory(categoryId);

                        Thread.Sleep(1000);
                        foreach (var attr in  attributeList)
                        {
                            // Lấy những attribute thỏa mãn "is_required": true
                            if( attr.is_required )
                            {
                                attr.category_id = categoryId;
                                attributeListGeneral.Add(attr);
                            }
                        }
                    }

                    // Lưu vào db
                    tikiSqler.InsertTikiAttributesOfCategory(attributeListGeneral, conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        // Từ category sản phẩm trên tiki đã mapping với sản phẩm trong kho,
        // ta cập nhật category Id sản phẩm trong kho
        private void UpdateCategoryIdOfProduct()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
                    List<int> listProId = new List<int>();
                    // Lấy id sản phẩm trong kho chưa có category Id (-1), trạng thái bình thường
                    {
                        MySqlCommand cmd = new MySqlCommand(
                        @"SELECT Id FROM webplaywithme.tbproducts WHERE CategoryId = -1 AND Status <> 2 ORDER BY Id;",
                    conn);
                        cmd.CommandType = CommandType.Text;
                        using (var reader = cmd.ExecuteReader())
                        {
                            int idOrdinal = reader.GetOrdinal("Id");
                            while (reader.Read())
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
                        using (var reader = cmd.ExecuteReader())
                        {
                            int tikiCategoryIdOrdinal = reader.GetOrdinal("TikiCategoryId");
                            while (reader.Read())
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
                            using (var reader = cmd.ExecuteReader())
                            {
                                int tikiIdOrdinal = reader.GetOrdinal("TikiId");
                                int productIdOrdinal = reader.GetOrdinal("ProductId");
                                while (reader.Read())
                                {
                                    if(tikiId == 0)
                                    {
                                        tikiId = reader.GetInt32(tikiIdOrdinal);
                                    }
                                    listProIdMapping.Add(reader.GetInt32(productIdOrdinal));
                                }
                            }
                        }

                        // Từ tikiId lấy được category id của sản phẩm trên sàn tiki trong chi tiết sản phẩm
                        TikiProduct pro = GetListProductTiki.GetProductFromOneShop(tikiId);
                        int tikiCategoryIdTemp = 0;
                        if(pro == null)
                        {
                            listProId.RemoveAt(0);
                        }
                        else
                        {
                            foreach (var cat in pro.categories)
                            {
                                if(listTikiCategoryId.Contains(cat.id))
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
                                cmd.ExecuteNonQuery();
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
        private string LazadaInsertSellerSku()
        {
            MySqlResultState result = new MySqlResultState();
            List<LazadaProduct> ls = LazadaProductAPI.GetProductAll();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(
                        @"UPDATE tb_lazada_model SET SellerSku = @inSellerSku WHERE TMDTLazadaModelId = @inTMDTLazadaModelId;",
                    conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inSellerSku", "");
                    cmd.Parameters.AddWithValue("@inTMDTLazadaModelId", 0L);
                    foreach (var pro in ls)
                    {
                        foreach(var sku in pro.skus)
                        {
                            cmd.Parameters[0].Value = sku.SellerSku;
                            cmd.Parameters[1].Value = sku.SkuId;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return JsonConvert.SerializeObject(result);
        }

        private void LazadaUpdatePrice_SalePrice()
        {
            LazadaMySql sqler = new LazadaMySql();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                conn.Open();
                List<CommonItem> ls = sqler.LazadaGetItemOnDB(conn);
                foreach(var item in ls)
                {
                    if(item.itemId == 3015755086)
                    {
                        List<CommonItem> lsTemp = new List<CommonItem>();
                        lsTemp.Add(item);
                        ProductController.LazadaUpdatePrice_SpecialPrice_Core(lsTemp, conn);
                    }
                }
            }
        }

        //private List<LazadaOrder> LazadaGetOrders(DateTime fromDate)
        //{

        //}
        public string GetListOrderLazada(int fromTo, int orderStatus)
        {
            List<CommonOrder> lsCommonOrder = new List<CommonOrder>();
            if (AuthentAdministrator() == null)
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
                lsOrderLazadaFullInfo = LazadaOrderAPI.LazadaGetOrdersDetailCanceled(time_from);
            }
            else if (eOrderStatus == CommonOrderStatus.READY_TO_SHIP_PROCESSED)
            {
                // Lazada
                lsOrderLazadaFullInfo = LazadaOrderAPI.LazadaGetOrdersDetailReadyToShip(time_from);
            }
            else
            {
                // Lazada
                lsOrderLazadaFullInfo = LazadaOrderAPI.LazadaGetOrdersDetailAll(time_from);
            }

            // Lazada
            foreach (var order in lsOrderLazadaFullInfo)
            {
                lsCommonOrder.Add(new CommonOrder(order));
            }
            return JsonConvert.SerializeObject(lsCommonOrder);
        }

        [HttpPost]
        public string TikiTestSomething()
        {
            if (AuthentAdministrator() == null)
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
            //LazadaMySql sqler = new LazadaMySql();
            //using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            //{
            //    conn.Open();
            //    sqler.InserttbLazadaMediaSpace(1234, 0, 0, images, conn);
            //}
            //LazadaProductAPI.LazadaCreateProductTest();

            //GetListOrderLazada(2, 0);

            //DateTime now = DateTime.Now;
            //ShopeeGetBookingDetail.ShopeeOrderGetBookingDetailAll(
            //    now.AddDays(-14),
            //    now,
            //    ShopeeOrderStatus.shopeeBookingStatusArray[(int)ShopeeOrderStatus.EnumShopeeBookingStatus.ALL],
            //    null);

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
            MySqlResultState result = new MySqlResultState();

            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            result = CommonShopeeAPI.ShopeeSaveCode(code);

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

        [HttpPost]
        public string LazadaGetAccessTokenFromCodeForFirst(string code)
        {
            MySqlResultState result = new MySqlResultState();

            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            Boolean isOk = LazadaAuthenAPI.LazadaAuthTokenCreateAndSaveDB(code);
            if (!isOk)
            {
                result.State = EMySqlResultState.ERROR;
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string LazadaRefreshAccessToken()
        {
            MySqlResultState result = new MySqlResultState();
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            Boolean isOk = LazadaAuthenAPI.LazadaAuthTokenRefresh();
            if (!isOk)
            { 
                result.State = EMySqlResultState.ERROR;
            }

            return JsonConvert.SerializeObject(result);
        }
    }
}