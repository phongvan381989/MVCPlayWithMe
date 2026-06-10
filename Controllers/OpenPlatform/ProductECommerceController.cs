using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.ItemModel;
using MVCPlayWithMe.Models.Order;
using MVCPlayWithMe.Models.ProductModel;
using MVCPlayWithMe.OpenPlatform.API.LazadaAPI;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeOrder;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Event;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Order;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Product;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaOrder;
using MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using static MVCPlayWithMe.General.Common;
using static MVCPlayWithMe.OpenPlatform.CommonOpenPlatform;
using static MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order.TikiOrderItemFilterByDate;
using ItemForCreate = MVCPlayWithMe.OpenPlatform.Model.ItemForCreate;

namespace MVCPlayWithMe.Controllers.OpenPlatform
{
    public class ProductECommerceController : BasicController
    {
        public ShopeeMySql shopeeSqler;
        public TikiMySql tikiSqler;
        public LazadaMySql lazadaSqler;
        public ItemModelMySql itemModelSqler;
        public OrderMySql ordersqler;

        public ProductECommerceController ()
        {
            shopeeSqler = new ShopeeMySql();
            tikiSqler = new TikiMySql();
            lazadaSqler = new LazadaMySql();
            itemModelSqler = new ItemModelMySql();
            ordersqler = new OrderMySql();
        }
        #region Xử lý item
        // GET: ProductECommerce
        public async Task<ActionResult> Index()
        {
            if ((await AuthentAdministratorAsync()) == null)
                return View("~/Views/Administrator/Login.cshtml");

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Item(string eType, string id)
        {
            if ((await AuthentAdministratorAsync()) == null)
                return View("~/Views/Administrator/Login.cshtml");

            //ViewDataGetListProductName();
            //ViewDataGetListCombo();

            return View();
        }

        [HttpPost]
        public async Task<string> GetProductAll(string eType)
        {
            List<CommonItem> lsCommonItem = null;
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(lsCommonItem);
            }

            if (eType == Common.eShopee)
            {
                lsCommonItem = await ShopeeGetProductAllAsync();
            }
            else if(eType == Common.eTiki)
            {
                lsCommonItem = await TikiGetProductAll();
            }
            else if(eType == Common.eLazada)
            {
                lsCommonItem = await LazadaGetProductAll();
            }
            return JsonConvert.SerializeObject(lsCommonItem);
        }

        // Lấy sản phẩm NORMAL, tạo mới/cập nhật trong 1 tháng gần đây và chưa được lưu db
        [HttpPost]
        public async Task<string> GetNewItemOneMonth(string eType)
        {
            List<CommonItem> lsCommonItem = null;
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(lsCommonItem);
            }

            if (eType == Common.eShopee)
            {
                lsCommonItem = await ShopeeGetNewItemOneMonthAsync();
            }
            else if (eType == Common.eTiki)
            {
                lsCommonItem = await TikiGetNewItemOneMonth();
            }
            else if(eType == Common.eLazada)
            {
                lsCommonItem = await LazadaGetNewItemOneMonth();
            }

            return JsonConvert.SerializeObject(lsCommonItem);
        }

        [HttpPost]
        public async Task<string> GetItemFromId(string eType, string id)
        {
            CommonItem commonItem = null;
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(commonItem);
            }

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            await conn.OpenAsync();
            if (eType == Common.eShopee)
            {
                long lid = Common.ConvertStringToInt64(id);
                if (lid != Int64.MinValue)
                {
                    commonItem = await ShopeeGetItemFromIdConnectOutAsync(lid, conn);
                }
            }
            else if (eType == Common.eTiki)
            {
                int iid = Common.ConvertStringToInt32(id);
                if (iid != Int32.MinValue)
                {
                    commonItem = await TikiGetProductFromIdConnectOut(iid, conn);
                }
                else
                {
                    MyLogger.GetInstance().Info("GetItemFromId call eType: " + eType + ", id: " + id);
                }
            }
            else if (eType == Common.eLazada)
            {
                long lid = Common.ConvertStringToInt64(id);
                if (lid != Int64.MinValue)
                {
                    commonItem = await LazadaGetItemFromIdConnectOut(lid, conn);
                }
            }
            return JsonConvert.SerializeObject(commonItem);
        }

        async Task<List<CommonItem>> ShopeeGetProductAllAsync()
        {
            List<CommonItem> lsCommonItem = new List<CommonItem>();
            List<ShopeeGetItemBaseInfoItem> lsShopeeBaseInfoItem = new List<ShopeeGetItemBaseInfoItem>();

            // Lấy toàn bộ sản phẩm Shopee mất thời gian
            lsShopeeBaseInfoItem = await ShopeeGetItemBaseInfo.ShopeeProductGetItemBaseInfoAllAsync();

            // Test chỉ lấy 1 page ~ 50 sản phẩm
            //lsShopeeItem = ShopeeGetItemBaseInfo.ShopeeProductGetItemBaseInfo_PageFisrstAsync();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            await conn.OpenAsync();

            try
            {
                // Không tồn tại trong DB ta insert
                foreach (var pro in lsShopeeBaseInfoItem)
                {
                    CommonItem item = await CommonItem.CommonItemFromShopeeGetItemBaseInfoItemAsync(pro);
                    lsCommonItem.Add(item);
                    await shopeeSqler.ShopeeInsertIfDontExistConnectOutAsync(item, conn);
                }

                await shopeeSqler.ShopeeGetListCommonItemFromListShopeeItemConnectOutAsync(lsCommonItem, conn);

                //// Cập nhật trạng thái item vào DB
                //shopeeSqler.ShopeeUpdateStatusOfItemListToDbConnectOut(lsCommonItem, conn);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return lsCommonItem;
        }

        async Task<List<CommonItem>> LazadaGetProductAll()
        {
            List<CommonItem> lsCommonItem = new List<CommonItem>();

            try
            {
                // Lấy toàn bộ sản phẩm Lazada mất thời gian
                List<LazadaProduct>  lsLazadaProduct = await LazadaProductAPI.GetProductAll();

                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    // Không tồn tại trong DB ta insert
                    foreach (var pro in lsLazadaProduct)
                    {
                        CommonItem item = new CommonItem(pro);
                        lsCommonItem.Add(item);
                        await lazadaSqler.LazadaInsertIfDontExistConnectOutAsync(item, conn);
                    }

                    await lazadaSqler.LazadaGetListCommonItemFromListItemConnectOutAsync(lsCommonItem, conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return lsCommonItem;
        }

        async Task<List<CommonItem>> ShopeeGetNewItemOneMonthAsync()
        {
            List<CommonItem> lsCommonItem = new List<CommonItem>();

            // Lấy danh sách item trong khoảng 1 tháng, trạng thái NORMAL
            long update_time_to = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            List<ShopeeItem> ls = await ShopeeGetItemList.ShopeeProductGetNormal_ItemListAsync(
                update_time_to - 31 * 24 * 3600,
                update_time_to);

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    List<ShopeeGetItemBaseInfoItem> lsShopeeBaseInfoItem =
                        await ShopeeGetItemBaseInfo.ShopeeProductGetListItemBaseInforFromListShopeeItemAsync(ls);
                    foreach (var pro in lsShopeeBaseInfoItem)
                    {
                        CommonItem item = await CommonItem.CommonItemFromShopeeGetItemBaseInfoItemAsync(pro);
                        // Không tồn tại trong DB ta insert
                        if (!await shopeeSqler.ShopeeInsertIfDontExistConnectOutAsync(item, conn))
                        {
                            lsCommonItem.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return lsCommonItem;
        }

        async Task<List<CommonItem>> LazadaGetNewItemOneMonth()
        {
            List<CommonItem> lsCommonItem = new List<CommonItem>();

            try
            {
                List<LazadaProduct> lsLazadaProduct = await LazadaProductAPI.GetNewProductOneMonth();
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    foreach (var pro in lsLazadaProduct)
                    {
                        CommonItem item = new CommonItem(pro);
                        // Không tồn tại trong DB ta insert
                        if ( !await lazadaSqler.LazadaInsertIfDontExistConnectOutAsync(item, conn))
                        {
                            lsCommonItem.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return lsCommonItem;
        }

        public async Task<List<CommonItem>> TikiGetProductAll()
        {
            List<CommonItem> lsCommonItem = new List<CommonItem>();
            List<TikiProduct> lsTikiItem = new List<TikiProduct>();
            lsTikiItem = await GetListProductTiki.GetListLatestProductsFromOneShop();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            await conn.OpenAsync();

            await tikiSqler.TikiGetListCommonItemFromListTikiProductConnectOutAsync(lsTikiItem, lsCommonItem, conn);

            // Không tồn tại trong DB ta insert
            foreach (var item in lsCommonItem)
            {
                await tikiSqler.TikiInsertIfDontExistConnectOutAsync(item, conn);
            }
            //// Cập nhật trạng thái item vào DB
            //tikiSqler.TikiUpdateStatusOfItemListToDbConnectOut(lsCommonItem, conn);
            return lsCommonItem;
        }

        private async Task<List<CommonItem>> TikiGetNewItemOneMonth()
        {
            List<CommonItem> lsCommonItem = new List<CommonItem>();
            DateTime dtNow = DateTime.Now;
            List<TikiProduct> lsTikiItem = await GetListProductTiki.TikiProductGetNormal_ItemList(dtNow.AddDays(-31), dtNow);

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            await conn.OpenAsync();
            try
            {
                foreach (var pro in lsTikiItem)
                {
                    CommonItem item = new CommonItem(pro);
                    // Không tồn tại ta thêm vào danh sách
                    if (!await tikiSqler.TikiInsertIfDontExistConnectOutAsync(item, conn))
                    {
                        lsCommonItem.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return lsCommonItem;
        }

        private async Task<CommonItem> ShopeeGetItemFromIdConnectOutAsync(long id, MySqlConnection conn)
        {
            ShopeeGetItemBaseInfoItem pro = await ShopeeGetItemBaseInfo.ShopeeProductGetItemBaseInfoFromIdAsync(id);
            if (pro == null)
            {
                return null;
            }

            CommonItem item = await CommonItem.CommonItemFromShopeeGetItemBaseInfoItemAsync(pro);
            try
            {
                // Không tồn tại trong DB ta insert
                await shopeeSqler.ShopeeInsertIfDontExistConnectOutAsync(item, conn);

                await shopeeSqler.ShopeeGetItemFromIdConnectOutAsync(id, item, conn);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return item;
        }

        private async Task<CommonItem> LazadaGetItemFromIdConnectOut(long id, MySqlConnection conn)
        {
            LazadaProduct pro = await LazadaProductAPI.GetProductItem(id);
            if (pro == null)
            {
                return null;
            }

            CommonItem item = new CommonItem(pro);
            try
            {
                // Không tồn tại trong DB ta insert
                await lazadaSqler.LazadaInsertIfDontExistConnectOutAsync(item, conn);

                await lazadaSqler.LazadaGetItemFromIdConnectOutAsync(id, item, conn);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return item;
        }

        private async Task<MySqlResultState> UpdateMapping(string eType, List<CommonForMapping> ls)
        {
            MySqlResultState result = null;
            if (eType == Common.eShopee)
            {
                result = await shopeeSqler.ShopeeUpdateMappingAsync(ls);
            }
            else if(eType == Common.eTiki)
            {
                result = await tikiSqler.TikiUpdateMappingAsync(ls);
            }
            else if (eType == Common.eLazada)
            {
                result = await lazadaSqler.LazadaUpdateMappingAsync(ls);
            }

            return result;
        }

        private async Task<CommonItem> TikiGetProductFromIdConnectOut(int id, MySqlConnection conn)
        {
            TikiProduct pro = null;
            pro = await GetListProductTiki.GetProductFromOneShop(id);

            if (pro == null || pro.created_by != TikiConstValues.cstrCreatedBy)
            {
                return null;
            }

            CommonItem item = new CommonItem(pro);
            if(!string.IsNullOrEmpty(item.imageSrc)) // Không có ảnh đại diện, có thể đây là sản phẩm cha ảo
            {
                // Không tồn tại trong DB ta insert
                await tikiSqler.TikiInsertIfDontExistConnectOutAsync(item, conn);
                await tikiSqler.TikiGetItemFromIdConnectOutAsync(id, item, conn);
            }

            return item;
        }

        /// <summary>
        /// str có dạng: itemId,modelId,productId,productQuantity,...,itemId,modelId,productId,productQuantity
        /// model chưa được mapping có dạng: "itemId,modelId,,,"
        /// với tiki: chỉ có itemId
        /// NOTE: Tất cả thuộc 1 item, tức chỉ có 1 itemId dù xuất hiện nhiều lần
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> UpdateMapping(string eType, string str)
        {
            MySqlResultState result;
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            List<CommonForMapping> ls = new List<CommonForMapping>();
            string[] values = str.Split(',');
            int length = values.Length;
            for(int i = 0; i < length; i = i + 4)
            {
                // Nếu model chưa được mapping productId, productQuantity là: System.Int32.MinValue;
                if (ls.Count > 0 && ls[ls.Count - 1].modelId == Common.ConvertStringToInt64(values[i + 1]))
                {
                    if (!string.IsNullOrEmpty(values[i + 2]) && !string.IsNullOrEmpty(values[i + 3]))
                    {
                        ls[ls.Count - 1].lsProductId.Add(Common.ConvertStringToInt32(values[i + 2]));
                        ls[ls.Count - 1].lsProductQuantity.Add(Common.ConvertStringToInt32(values[i + 3]));
                    }
                }
                else
                {
                    CommonForMapping commonForMapping = new CommonForMapping();
                    commonForMapping.itemId = Common.ConvertStringToInt64(values[i]);
                    commonForMapping.modelId = Common.ConvertStringToInt64(values[i + 1]);
                    if (!string.IsNullOrEmpty(values[i + 2]) && !string.IsNullOrEmpty(values[i + 3]))
                    {
                        commonForMapping.lsProductId.Add(Common.ConvertStringToInt32(values[i + 2]));
                        commonForMapping.lsProductQuantity.Add(Common.ConvertStringToInt32(values[i + 3]));
                    }

                    ls.Add(commonForMapping);
                }
            }
            result = await UpdateMapping(eType, ls);
            return JsonConvert.SerializeObject(result);
        }

        [HttpGet]
        public async Task<ActionResult> ItemOnDB()
        {
            if ((await AuthentAdministratorAsync()) == null)
                return View("~/Views/Administrator/Login.cshtml");

            return View();
        }

        [HttpPost]
        public async Task<string> GetItemOnDB(string eType)
        {
            List<CommonItem> ls = new List<CommonItem>();
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(ls);
            }

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                if (eType == Common.eShopee)
                {
                    ls = await shopeeSqler.ShopeeGetItemOnDBAsync(conn);
                }
                else if (eType == Common.eTiki)
                {
                    ls = await tikiSqler.TikiGetItemOnDBAsync(conn);
                }
                else if (eType == Common.eLazada)
                {
                    ls = await lazadaSqler.LazadaGetItemOnDBAsync(conn);
                }
            }
            return JsonConvert.SerializeObject(ls);
        }

        // Nếu tên sản phẩm trên sàn bao gồm cả tên combo, tên sản phẩm trong kho thì mapping tương ứng
        [HttpPost]
        public async Task<string> AutoUpdateMapping(string eType)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState resultState = new MySqlResultState();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                await conn.OpenAsync();
                //// Lấy danh sách combo, sản phẩm thuộc combo trong kho
                //ComboMySql comboMysql = new ComboMySql();
                //List<Combo> lsCombo = comboMysql.GetListComboIncludeSimpleProducts(conn);
                // Lấy danh sách sản phẩm trong kho đơn giản
                ProductMySql productMySql = new ProductMySql();
                List<Product> lsProduct =await productMySql.GetSimpleComboCategoryAllConnectOutAsync(conn);
                string nameTemp = "";
                if (eType == Common.eTiki)
                {
                    // Lấy danh sách sản phẩm trên sàn chưa mapping
                    Dictionary<int, string> dic = await tikiSqler.TikiGetListItemDontMappingAsync(conn);
                    foreach(var item in dic)
                    {
                        foreach (var pro in lsProduct)
                        {
                            nameTemp = Product.GenerateName(pro);
                            if(nameTemp == item.Value)
                            {
                                await tikiSqler.TikiUpdateMappingSignleAsync(item.Key, pro.id, 1, conn);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    resultState.State = EMySqlResultState.DONT_EXIST;
                    resultState.Message = "Chưa hỗ trợ sàn " + eType;
                }
            }
            catch(Exception ex)
            {
                Common.SetResultException(ex, resultState);
            }
            return JsonConvert.SerializeObject(resultState);
        }

        // Xóa item trên db
        [HttpPost]
        public async Task<string> DeleteItemOnDB(string eType, string itemId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState resultState = null;
            if (eType == Common.eShopee)
            {
                long id = Common.ConvertStringToInt64(itemId);
                resultState = await shopeeSqler.ShopeeDeleteItemOnDBAsync(id);
            }
            else if (eType == Common.eTiki)
            {
                int id = Common.ConvertStringToInt32(itemId);
                resultState = await tikiSqler.TikiDeleteItemOnDBAsync(id);
            }
            return JsonConvert.SerializeObject(resultState);
        }

        // Xóa model Shopee trên db => Disable
        [HttpPost]
        public async Task<string> DeleteShopeeModelOnDB(string eType, string modelId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState resultState = null;
            if (eType == Common.eShopee)
            {
                long id = Common.ConvertStringToInt64(modelId);
                resultState = await ShopeeMySql.ShopeeDeleteModelOnDBAsync(id);
            }
            return JsonConvert.SerializeObject(resultState);
        }

        [HttpPost]
        public async Task<string> CopyImageFromTMDTToWarehouseProduct(string eType,
            string imageUrl, string productId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState resultState = new MySqlResultState();

            string path = Common.GetAbsoluteProductMediaFolderPath(productId);
            if (path == null)
            {
                path = Common.CreateAbsoluteProductMediaFolderPath(productId);
            }
            else// Nếu đã tồn tại ảnh trong thư mục thì return
            {
                string[] files = Directory.GetFiles(path);

                foreach (var f in files)
                {
                    if (ImageExtensions.Contains(Path.GetExtension(f).ToLower()))
                    {
                        resultState.State = EMySqlResultState.EXIST;
                        resultState.Message = "Sản phẩm đã có ảnh";
                        return JsonConvert.SerializeObject(resultState);
                    }
                }
            }

            //ShopeeGetItemBaseInfoItem pro =
            //            ShopeeGetItemBaseInfo.ShopeeProductGetItemBaseInfoFromId(
            //                Common.ConvertStringToInt64(itemId));
            //if (pro != null)
            //{
            //    string url = "";

            //    // Lấy imageSrc cho model nếu có
            //    if (pro.has_model)
            //    {
            //        ShopeeGetModelListResponse obj =
            //            ShopeeGetModelList.ShopeeProductGetModelList(pro.item_id);
            //        if (obj != null)
            //        {
            //            ShopeeGetModelList_TierVariation tierVar = obj.tier_variation[0];
            //            int count = tierVar.option_list.Count;
            //            for (int i = 0; i < count; i++)
            //            {
            //                ShopeeGetModelList_Model model = CommonItem.GetModelFromModelListResponse(obj, i);
            //                ShopeeGetModelList_TierVariation_Option option = tierVar.option_list[i];
            //                // Lấy ảnh đại diện
            //                if (option.image != null)
            //                {
            //                    if (Common.ConvertStringToInt64(modelId) == model.model_id)
            //                    {
            //                        url = option.image.image_url;
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        url = pro.image.image_url_list[0];
            //    }

            //    // Tải ảnh
            //    Common.DownloadImageAddWaterMarkAndReduce(url, Path.Combine(path, "0.jfif"));
            //}
            //else
            //{
            //    resultState.State = EMySqlResultState.INVALID;
            //    resultState.Message = "Không lấy được thông tin. Vui lòng thử lại sau";
            //}

            if (eType == eShopee)
            {
                Common.DownloadImageAddWaterMarkAndReduce(imageUrl, Path.Combine(path, "0.jfif"), false);
            }
            else if (eType == eTiki)
            {
                // Lấy phần mở rộng
                string fileExtension = Path.GetExtension(imageUrl);
                if (!string.IsNullOrEmpty(fileExtension))
                {
                    Common.DownloadImageAddWaterMarkAndReduce(imageUrl, Path.Combine(path, "0" + fileExtension), false);
                }
            }

            return JsonConvert.SerializeObject(resultState);
        }
        #endregion

        #region Xử lý đơn hàng
        [HttpGet]
        public async Task<ActionResult> Order()
        {
            if ((await AuthentAdministratorAsync()) == null)
                return View("~/Views/Administrator/Login.cshtml");

            return View();
        }

        /// <summary>
        /// Cần lấy danh sách order và booking
        /// </summary>
        /// <param name="fromTo"> 0: 1 ngày, 1: 7 ngày, 2: 30 ngày</param>
        /// <param name="orderStatus"> 0: tất cả, 1: Cần gửi hàng, 2: Hủy</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> GetListOrder(int fromTo, int orderStatus)
        {
            List<CommonOrder> lsCommonOrder = new List<CommonOrder>();
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(lsCommonOrder);
            }
            CommonOrderStatus eOrderStatus = (CommonOrderStatus)orderStatus;
            // Lấy đơn hàng tiki
            List<TikiOrder> lsOrderTikiFullInfo;
            lsOrderTikiFullInfo = await TikiGetListOrders.GetListOrderAShop((EnumOrderItemFilterByDate)fromTo, eOrderStatus);
            if(lsOrderTikiFullInfo != null)
            {
                foreach(var order in lsOrderTikiFullInfo)
                {
                    lsCommonOrder.Add(new CommonOrder(order));
                }
            }

            // Lấy đơn hàng của Shopee, Lazada
            List<ShopeeOrderDetail> lsOrderShopeeFullInfo = null ;
            List<ShopeeBookingDetail> lsBookingShopeeFullInfo = null;
            DateTime time_from, time_to;
            time_from = DateTime.Now;
            time_to = DateTime.Now;
            if ((EnumOrderItemFilterByDate)fromTo == EnumOrderItemFilterByDate.today)
                time_from = time_to.AddDays(-1);
            else if ((EnumOrderItemFilterByDate)fromTo == EnumOrderItemFilterByDate.last7days)
                time_from = time_to.AddDays(-7);
            else if ((EnumOrderItemFilterByDate)fromTo == EnumOrderItemFilterByDate.last30days)
                time_from = time_to.AddDays(-30);

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            await conn.OpenAsync();

            // Lấy đơn hàng của shopee, lazada
            List<LazadaOrder> lsOrderLazadaFullInfo = null;
            if (eOrderStatus == CommonOrderStatus.CANCELLED)
            {
                // Shopee
                lsOrderShopeeFullInfo = await ShopeeGetOrderDetail.ShopeeOrderGetOrderDetailAllAsync(
                time_from,
                time_to,
                ShopeeOrderStatus.shopeeOrderStatusArray[(int)ShopeeOrderStatus.EnumShopeeOrderStatus.CANCELLED],
                conn);

                lsBookingShopeeFullInfo = await ShopeeGetBookingDetail.ShopeeOrderGetBookingDetailAllAsync(
                    time_from,
                    time_to,
                    ShopeeOrderStatus.shopeeBookingStatusArray[(int)ShopeeOrderStatus.EnumShopeeBookingStatus.CANCELLED],
                    conn);

                // Lazada
                lsOrderLazadaFullInfo = await LazadaOrderAPI.LazadaGetOrdersDetailCanceledAsync(time_from);
            }
            else if (eOrderStatus == CommonOrderStatus.READY_TO_SHIP_PROCESSED)
            {
                // Shopee
                lsOrderShopeeFullInfo = await ShopeeGetOrderDetail.ShopeeOrderGetOrderDetailToPickUpAsync(
                time_from,
                time_to,
                conn);

                lsBookingShopeeFullInfo = await ShopeeGetBookingDetail.ShopeeOrderGetBookingDetailToPickUpAsync(
                    time_from,
                    time_to,
                    conn);

                // Lazada
                lsOrderLazadaFullInfo = await LazadaOrderAPI.LazadaGetOrdersDetailReadyToShipAsync(time_from);
            }
            else
            {
                // Shopee
                lsOrderShopeeFullInfo = await ShopeeGetOrderDetail.ShopeeOrderGetOrderDetailAllAsync(
                    time_from,
                    time_to,
                    ShopeeOrderStatus.shopeeOrderStatusArray[(int)ShopeeOrderStatus.EnumShopeeOrderStatus.ALL],
                    conn);

                lsBookingShopeeFullInfo = await ShopeeGetBookingDetail.ShopeeOrderGetBookingDetailAllAsync(
                    time_from,
                    time_to,
                    ShopeeOrderStatus.shopeeBookingStatusArray[(int)ShopeeOrderStatus.EnumShopeeBookingStatus.ALL],
                    conn);

                // Lazada
                lsOrderLazadaFullInfo = await LazadaOrderAPI.LazadaGetOrdersDetailAllAsync(time_from);
            }

            // Shopee
            foreach (var order in lsOrderShopeeFullInfo)
            {
                lsCommonOrder.Add(new CommonOrder(order));
            }
            foreach (var order in lsBookingShopeeFullInfo)
            {
                //if (string.IsNullOrEmpty(order.order_sn))
                //{
                //    // Đảm bảo chưa có đơn matched
                    lsCommonOrder.Add(new CommonOrder(order));
                //}
            }

            // Lazada
            foreach (var order in lsOrderLazadaFullInfo)
            {
                lsCommonOrder.Add(new CommonOrder(order));
            }

            // Lấy đơn hàng của web Play With Me
            lsCommonOrder.AddRange(await ordersqler.GetListCommonOrderAsync(fromTo));

            // Cập nhật trạng thái đơn hàng: giữ chỗ / hủy giữ chỗ / đã đóng / đã hoàn
            await ordersqler.GetOrderStatusInWarehouseToCommonOrderAsync(lsCommonOrder);
            return JsonConvert.SerializeObject(lsCommonOrder);
        }

        // Từ tên sàn, mã đơn hoặc mã vận đơn (với sàn SHOPEE, LAZADA) hoặc mã booking lấy được thông tin CommonOrder
        [HttpPost]
        public async Task<string> GetOrderFromOrderSN_TrackingNumber(string ecommerce,
            string sn_trackingNumber,
            int isBookingCode) // 0: tức là false, ngược lại là true
        {
            List<CommonOrder> lsCommonOrder = new List<CommonOrder>();
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(lsCommonOrder);
            }
            CommonOrder commonOrder = null;

            if (ecommerce == Common.eShopee)
            {
                // Lấy mã đơn hàng từ DB dựa vào tham số mã đơn hàng hoặc mã vận đơn 
                MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
                await conn.OpenAsync();
                if (isBookingCode == 0)
                {
                    var (sn, trackingNumber) = await tikiSqler.GetSN_TrackingNumberFromSN_TrackingNumberConnectOutAsync(
                        sn_trackingNumber, EECommerceType.SHOPEE, conn);

                    if (string.IsNullOrEmpty(sn)) // Vì push message xịt, nên chưa có thông tin mã đơn
                    {
                        // Ta cần lấy sn từ mã đơn / vận đơn
                        sn = sn_trackingNumber; // Nếu người dùng nhập mã vận đơn ở đây thì không tìm thấy dữ liệu chi tiết đơn hàng từ sàn
                    }
                    ShopeeOrderDetail shopeeOrderDetail =
                    await ShopeeGetOrderDetail.ShopeeOrderGetOrderDetailFromOrderSNAsync(sn);

                    if (shopeeOrderDetail != null)
                    {
                        shopeeOrderDetail.shipCode = trackingNumber;
                        commonOrder = new CommonOrder(shopeeOrderDetail);
                    }
                }
                else
                {
                    var (sn, trackingNumber) = await tikiSqler.GetBookingSN_TrackingNumberFromSN_TrackingNumberConnectOutAsync(
                        sn_trackingNumber, EECommerceType.SHOPEE, conn);

                    if (string.IsNullOrEmpty(sn)) // Vì push message xịt, nên chưa có thông tin mã booking
                    {
                        // Ta cần lấy sn từ mã đơn / vận đơn
                        sn = sn_trackingNumber; // Nếu người dùng nhập mã vận đơn ở đây thì không tìm thấy dữ liệu chi tiết đơn hàng từ sàn
                    }
                    // Lấy chi tiết booking
                    ShopeeBookingDetail detail =
                        await ShopeeGetBookingDetail.ShopeeOrderGetBookingDetailFromBookingSNAsync(sn);
                    if (detail != null)
                    {
                        ShopeeMySql shopeeMySql = new ShopeeMySql();
                        commonOrder = new CommonOrder(detail); ;
                    }
                }
            }
            else if (ecommerce == Common.eLazada)
            {
                // Lấy mã đơn hàng từ DB dựa vào tham số mã đơn hàng hoặc mã vận đơn 
                MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
                await conn.OpenAsync();
                var (sn, trackingNumber) = await tikiSqler.GetSN_TrackingNumberFromSN_TrackingNumberConnectOutAsync(
                    sn_trackingNumber, EECommerceType.LAZADA, conn);

                if (string.IsNullOrEmpty(sn)) // Vì push message xịt, nên chưa có thông tin mã đơn
                {
                    // Ta cần lấy sn từ mã đơn
                    sn = sn_trackingNumber; // Nếu người dùng nhập mã vận đơn ở đây thì không tìm thấy dữ liệu chi tiết đơn hàng từ sàn
                }
                long order_id = Common.ConvertStringToInt64(sn);
                if (order_id > 0)
                {
                    LazadaOrder orderDetail =
                    await LazadaOrderAPI.LazadaGetOrderDetailAsync(order_id);

                    if (orderDetail != null)
                    {
                        commonOrder = new CommonOrder(orderDetail);
                    }
                }
            }
            else if (ecommerce == Common.eTiki)
            {
                TikiOrder tikiOrder = await TikiGetListOrders.TikiGetOrderFromCode(sn_trackingNumber);
                if (tikiOrder != null)
                {
                    commonOrder = new CommonOrder(tikiOrder);
                }
            }

            if (commonOrder != null)
            {
                lsCommonOrder.Add(commonOrder);
            }

            // Cập nhật trạng thái đơn hàng: giữ chỗ / hủy giữ chỗ / đã đóng / đã hoàn
            await ordersqler.GetOrderStatusInWarehouseToCommonOrderAsync(lsCommonOrder);

            return JsonConvert.SerializeObject(lsCommonOrder);
        }

        /// <summary>
        /// Load lại mapping, sản phẩm trong 1 order
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> ReloadOneOrder(string commonOrder)
        {
            CommonOrder order = JsonConvert.DeserializeObject<CommonOrder>(commonOrder);
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(order);
            }

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            await conn.OpenAsync();
            try
            {
                // Nếu sản phẩm trên shopee, tiki,... chưa có trên tbShopeeItem, tbShopeeModel, tbTikiItem
                // khi vào thông tin chi tiết của sản phẩm trên sàn sẽ được insert vào db tương ứng.

                // Nếu sản phẩm trên shopee, tiki đã có trên tbShopeeItem, tbShopeeModel, tbTikiItem
                // nhưng trạng thái cũ là tắt (Status != 0) và giờ được bật (nên mới có đơn).
                // Ta cần cập nhật lại.
                await ordersqler.UpdateStatusNormalOfTMDTItemConnectOut(order, conn);

                order.listMapping = new List<List<Models.Mapping>>(); // Reset để cập nhật lại

                if (order.ecommerceName == eTiki)
                {
                    await tikiSqler.TikiGetMappingOfCommonOrderConnectOutAsync(order, conn);
                }
                else if (order.ecommerceName == eShopee)
                {
                    await shopeeSqler.ShopeeGetMappingOfCommonOrderConnectOutAsync(order, conn);
                }
                else if (order.ecommerceName == eLazada)
                {
                    await lazadaSqler.LazadaGetMappingOfCommonOrderConnectOutAsync(order, conn);
                }
                else if (order.ecommerceName == ePlayWithMe)
                {
                    await ordersqler.PlayWithMeGetMappingOfCommonOrderConnectOut(order, conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return JsonConvert.SerializeObject(order);
        }

        [HttpPost]
        public async Task<string> EnoughProductInOrder(string eType, string commonOrder)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            CommonOrder order = null;
            try
            {
                order = JsonConvert.DeserializeObject<CommonOrder>(commonOrder);

                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    EECommerceType eECommerceType = EECommerceType.TIKI;
                    if (eType == Common.eShopee)
                    {
                        eECommerceType = EECommerceType.SHOPEE;
                    }
                    else if (eType == Common.eTiki)
                    {
                        eECommerceType = EECommerceType.TIKI;
                        // Lấy event để trạng thái đơn hàng trên db là mới nhất
                        TikiPullEventService tikiPullEventService = new TikiPullEventService();
                        tikiPullEventService.DoWork(conn);
                    }
                    else if (eType == Common.eLazada)
                    {
                        eECommerceType = EECommerceType.LAZADA;
                    }
                    else if (eType == Common.ePlayWithMe)
                    {
                        eECommerceType = EECommerceType.PLAY_WITH_ME;
                    }
                    // Là đơn booking
                    if (order.isBooking)
                    {
                        TbEcommerceOrder tbEcommerceBookingLastest = await tikiSqler.GetLastestStatusOfECommerceBookingAsync(
                            order.bookingCode, eECommerceType, conn);
                        ECommerceOrderStatus oldStatus = (ECommerceOrderStatus)tbEcommerceBookingLastest.status;

                        ECommerceOrderStatus status = ECommerceOrderStatus.PACKED;

                        result = await tikiSqler.UpdateQuantityOfProductInWarehouseFromBookingConnectOutAsync(
                        order, status, 0, oldStatus,
                        EECommerceType.SHOPEE, conn);
                    }
                    else
                    {
                        // Nếu là đơn hỏa tốc, cập nhật trạng thái sang đã biết có đơn. Trạng thái này khả năng cao đã
                        // được cập nhật từ mini app viết mục đích nhắc có đơn hỏa tốc
                        if (order.isExpress)
                        {
                            await tikiSqler.UpdateStatusToKnownTbExpressOrderAsync(order.code, eECommerceType, conn);
                        }

                        TbEcommerceOrder tbEcommerceOrder = await tikiSqler.GetLastestStatusOfECommerceOrderAsync(
                            order.code, eECommerceType, conn);
                        ECommerceOrderStatus oldStatus = (ECommerceOrderStatus)tbEcommerceOrder.status;

                        ECommerceOrderStatus status = ECommerceOrderStatus.PACKED;
                        // Chuẩn bị đóng nhưng khách đã hủy đơn
                        if (status == ECommerceOrderStatus.PACKED && oldStatus == ECommerceOrderStatus.UNBOOKED)
                        {
                            result.State = EMySqlResultState.INVALID;
                            result.Message = "Đơn hàng đã bị hủy.";
                        }
                        else if (tikiSqler.IsNeedUpdateQuantityOfProductInWarehouseFromOrderStatus(status, oldStatus))
                        {
                            result = await tikiSqler.UpdateQuantityOfProductInWarehouseFromOrderConnectOutAsync(
                            order, status, 0, oldStatus, eECommerceType, conn);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
                return JsonConvert.SerializeObject(result);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> ReturnedOrder(string eType, string commonOrder)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();

            CommonOrder order = null;
            try
            {
                order = JsonConvert.DeserializeObject<CommonOrder>(commonOrder);

                EECommerceType eECommerceType = EECommerceType.TIKI;
                if (eType == Common.eShopee)
                {
                    eECommerceType = EECommerceType.SHOPEE;
                }
                else if (eType == Common.eTiki)
                {
                    eECommerceType = EECommerceType.TIKI;
                }
                else if (eType == Common.ePlayWithMe)
                {
                    eECommerceType = EECommerceType.PLAY_WITH_ME;
                }
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    // Là đơn booking
                    if (order.isBooking)
                    {
                        TbEcommerceOrder tbEcommerceBookingLastest = await tikiSqler.GetLastestStatusOfECommerceBookingAsync(
                            order.bookingCode, eECommerceType, conn);
                        ECommerceOrderStatus oldStatus = (ECommerceOrderStatus)tbEcommerceBookingLastest.status;

                        ECommerceOrderStatus status = ECommerceOrderStatus.RETURNED;

                        result = await tikiSqler.UpdateQuantityOfProductInWarehouseFromBookingConnectOutAsync(
                        order, status, 0, oldStatus,
                        EECommerceType.SHOPEE, conn);
                    }
                    else
                    {
                        TbEcommerceOrder tbEcommerceOrder = await tikiSqler.GetLastestStatusOfECommerceOrderAsync(
                            order.code, eECommerceType, conn);
                        ECommerceOrderStatus oldStatus = (ECommerceOrderStatus)tbEcommerceOrder.status;

                        ECommerceOrderStatus status = ECommerceOrderStatus.RETURNED;
                        if (tikiSqler.IsNeedUpdateQuantityOfProductInWarehouseFromOrderStatus(status, oldStatus))
                        {
                            result = await tikiSqler.UpdateQuantityOfProductInWarehouseFromOrderConnectOutAsync(
                            order, status, 0, oldStatus, eECommerceType, conn);

                            if (result != null && result.myAnything == 1)
                            {
                                // Cập nhật số lượng sản phẩm khác trên sàn SHOPEE, TIKI, LAZADA. Không quan tâm kết quả thành công hay không
                                await ProductController.GetListNeedUpdateQuantityAndUpdate_CoreAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
                return JsonConvert.SerializeObject(result);
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Lưu ảnh/video item shopee vào item voibenho. Xóa ảnh/video của item voibenho nếu đã tồn tại
        /// Không xóa ảnh của model
        /// </summary>
        /// <param name="commonItem"></param>
        /// <param name="vbnItemId"></param>
        private void SaveShopeeItemMediaToVoiBeNhoItem(CommonItem commonItem, int vbnItemId)
        {
            string path = Common.GetAbsoluteItemMediaFolderPath(vbnItemId);
            if(path == null)
            {
                path = Common.CreateAbsoluteItemMediaFolderPath(vbnItemId);
            }
            else // Xóa ảnh/video của item voibenho nếu đã tồn tại. Không xóa ảnh của model
            {
                Common.DeleteAllMediaFileInclude320(path);
            }

            // Tải ảnh
            int i = 0;
            foreach(var s in commonItem.imageSrcList)
            {
                Common.DownloadImageAddWaterMarkAndReduce(s, Path.Combine(path, i.ToString() + ".jfif"), true);
                i++;
            }

            // Tải video nếu có
            if (!string.IsNullOrEmpty(commonItem.videoSrc))
            {
                Common.DownloadVideo(commonItem.videoSrc, Path.Combine(path, "0.mp4"));
            }
        }

        /// <summary>
        /// Từ model sản phẩm trên sàn Shopee sinh model trên web voibenho
        /// Nếu trên web voibenho chưa có item tương ứng ta sinh item trước rồi sinh model sau
        /// </summary>
        /// <param name="strCommonItem"> đối tượng commmon item dạng string json</param>
        /// <param name="shopeeModelId"> shopee model id của sản phẩm cần sinh trên web voibenho</param>
        /// <param name="pWMMappingModelId"> model id của sản phẩm mappin gtrên web voibenho.
        /// 0: nếu chưa tồn tại</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> ShopeeBornModelForVoiBeNho(string strCommonItem, long shopeeModelId, int pWMMappingModelId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            CommonItem commonItem = JsonConvert.DeserializeObject<CommonItem>(strCommonItem);

            // Check xem item đã được sinh ra trên voibenho
            int itemId = 0;
            itemId = await itemModelSqler.GetItemIdFromNameAsync(commonItem.name);

            // Chưa sinh item tương ứng trên web voibenho.
            if (itemId <= 0)
            {
                // Sinh item trên web voibenho
                int status = 0;
                if (commonItem.item_status == "NORMAL")
                    status = 0;
                else
                    status = 1;
                itemId = await itemModelSqler.AddItemAsync(commonItem.name, status, commonItem.detail);

                // Lưu ảnh vào thư mục \Media\Item\ItemId\
                SaveShopeeItemMediaToVoiBeNhoItem(commonItem, itemId);
            }
            else
            {
                string path = Common.GetAbsoluteItemMediaFolderPath(itemId);
                if (path == null) // Chưa lưu image/video của item
                {
                    SaveShopeeItemMediaToVoiBeNhoItem(commonItem, itemId);
                }
            }

            // Lấy được đối tượng common model shopee
            CommonModel commonModel = null;
            foreach(var m in commonItem.models)
            {
                if(m.modelId == shopeeModelId)
                {
                    commonModel = m;
                    break;
                }
            }

            // Nếu model đã có trên voibenho, xóa dữ liệu ở tbMapping, tbpwmmappingother, tbModel
            // Từ giá bìa, giá bán tính toán chiết khấu làm tròn , giá bán theo chiết khấu
            float discount = 100 - commonModel.price * 100/ commonModel.market_price;
            int price = (int)((100 - discount) * commonModel.market_price / 100);
            //price = price / 1000 * 1000; // Lấy đơn vị tròn 1000 vnđ
            MySqlResultState resultState = await itemModelSqler.BornModelFromShopeeModelAsync(itemId, pWMMappingModelId,
                commonModel.name, 5, discount, price, commonModel.market_price, commonItem.itemId, commonModel.modelId);

            if(resultState.State != EMySqlResultState.OK)
            {
                return JsonConvert.SerializeObject(resultState);
            }
            // Xóa dữ liệu media cũ ở Media\Item\itemId\Model nếu có
            Common.DeleteImageModelInclude320(itemId, pWMMappingModelId);

            // Thêm dữ liệu media ở Media\Item\itemId\Model
            // Lấy model Id
            int newModelId = 0;
            newModelId = resultState.myAnything;
            // Thêm dữ liệu media  ở Media\Item\itemId\Model nếu có
            {
                string path = Common.GetAbsoluteModelMediaFolderPath(itemId);
                if(path == null)
                {
                    path = Common.CreateAbsoluteModelMediaFolderPath(itemId);
                }
                if (!string.IsNullOrEmpty(commonModel.imageSrc))
                {
                    DownloadImageAddWaterMarkAndReduce(commonModel.imageSrc, Path.Combine(path,
                        newModelId.ToString() + ".jfif"), false);
                }
            }

            // Insert dữ liệu cho tbMapping từ mapping của model shopee
            if (commonModel.mapping.Count > 0)
            {
                List<int> mappingOnlyProductId = new List<int>();
                List<int> mappingOnlyQuantity = new List<int>();

                foreach(var m in commonModel.mapping)
                {
                    mappingOnlyProductId.Add(m.product.id);
                    mappingOnlyQuantity.Add(m.quantity);
                }
                resultState = await itemModelSqler.UpdateMappingAsync(newModelId, mappingOnlyProductId, mappingOnlyQuantity);
            }

            return JsonConvert.SerializeObject(resultState);
        }
        #endregion

        [HttpPost]
        public async Task<string> UpdateBookCoverPriceToEEcommerce(string strCommonItem)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    if ((await AuthentAdministratorConnectOutAsync(conn)) == null)
                    {
                        return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
                    }

                    CommonItem commonItem = JsonConvert.DeserializeObject<CommonItem>(strCommonItem);

                    if (commonItem.eType != Common.eTiki)
                    {
                        result.State = EMySqlResultState.INVALID;
                        result.Message = "Hiện tại chức năng chỉ có với sàn TIKI.";
                    }

                    //// Lấy danh sách nhà phát hành, từ đó lấy được discount chung
                    //PublisherMySql publisherSqler = new PublisherMySql();
                    //List<Publisher> listPublisher = publisherSqler.GetListPublisherConnectOut(conn);

                    //// Lấy danh sách thuế phí
                    //TikiDealDiscountMySql tikiDealDiscountMySql = new TikiDealDiscountMySql();
                    //TaxAndFee taxAndFee = tikiDealDiscountMySql.GetTaxAndFee(Common.eTiki, conn);

                    //int salePrice = TikiDealDiscountController.CaculateSalePriceCoreFromCommonItem(
                    //    commonItem, listPublisher, taxAndFee);
                    int bookCoverPrice = commonItem.models[0].GetBookCoverPrice();

                    // Méo hiểu sao, update giá bìa luôn, response trả về ok nhưng check trên nhà bán thấy không được
                    // Nhưng khi cập nhật cao hơn giá bìa thì thành công
                    // Giải pháp: Cập nhật lần 1 cao hơn giá bìa, sau đó cập nhật về giá bìa
                    //TikiUpdateStock.TikiProductUpdatePrice((int)commonItem.itemId,
                    //    bookCoverPrice + 10000,
                    //    result);

                    //if (result.State == EMySqlResultState.OK)
                    //{
                    //    TikiUpdateStock.TikiProductUpdatePrice((int)commonItem.itemId,
                    //    bookCoverPrice + 10000,
                    //    result);
                    //}

                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error(ex.ToString());
            }

            return JsonConvert.SerializeObject(result);
        }

        public async Task<ActionResult> OrderStatistics()
        {
            if ((await AuthentAdministratorAsync()) == null)
                return View("~/Views/Administrator/Login.cshtml");

            return View();
        }

        // Lấy lịch sử đơn hàng
        [HttpPost]
        public async Task<string> GetOrderStatistics(int eType, int intervalDay)
        {
            List<TbEcommerceOrder> outputList = null;

            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<TbEcommerceOrder>());
            }

            try
            {
                MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
                await conn.OpenAsync();
                outputList = await tikiSqler.GetOrderStatisticsAsync(eType, intervalDay, conn);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                outputList = new List<TbEcommerceOrder>();
            }
            return JsonConvert.SerializeObject(outputList);
        }

        public async Task<string> UpdateStatusItemOpposite(string eType, long itemId, string currentStatus)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            try
            {
                if(eType == Common.eTiki)
                {
                    int id = (int)itemId;

                    // Lấy tồn kho, nếu bằng = 0 thì cho phép tắt bán sản phẩm trên sàn
                    MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
                    await conn.OpenAsync();
                    ProductMySql productMySql = new ProductMySql();
                    int quantity =await productMySql.TikiGetQuantityOfOneItemModelConnectOutAsync(id, conn);
                    if(quantity != 0 && currentStatus == Common.tikiActive)
                    {
                        result.State = EMySqlResultState.INVALID;
                        result.Message = "Tồn kho lớn hơn 0, không cho phép tắt sản phẩm từ đây. Bạn có thẻ vào Tiki Seller tắt.";
                    }
                    else
                    {
                        TikiUpdateStock.TikiProductUpdateStatus(id, currentStatus == Common.tikiActive ? 0 : 1, result);
                        if(result.State == EMySqlResultState.OK)
                        {
                            // cập nhật trạng thái item
                            await tikiSqler.TikiUpdateStatusOfItemToDbConnectOutAsync(id, currentStatus == Common.tikiActive? 1: 0, conn);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                Common.SetResultException(ex, result);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpGet]
        public async Task<string> UpdateQuantityPrice_SpecialPrice(string eType, long id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            if (eType != Common.eLazada)
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "Hiện tại chức năng chỉ có với sàn " + eType;
            }
            else
            {
                try
                {
                    LazadaMySql sqler = new LazadaMySql();
                    using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                    {
                        await conn.OpenAsync();
                        CommonItem item = await LazadaGetItemFromIdConnectOut(id, conn);
                        List<CommonItem> ls = new List<CommonItem>();

                        ls.Add(item);
                        ProductController.LazadaUpdateQuantityPrice_SpecialPrice_Core(ls, conn);

                    }
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                }
            }

            return JsonConvert.SerializeObject(result);
        }
    }
}
