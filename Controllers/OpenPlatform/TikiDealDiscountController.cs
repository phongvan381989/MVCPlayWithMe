using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.ProductModel;
using MVCPlayWithMe.OpenPlatform;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.DealDiscount;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Product;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.TikiAPI.DealDiscount;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Deal;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.DealDiscount;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MVCPlayWithMe.Controllers.OpenPlatform
{
    public class TikiDealDiscountController : BasicController
    {
        public static TikiDealDiscountMySql sqler;

        public TikiDealDiscountController()
        {
            sqler = new TikiDealDiscountMySql();
        }

        // GET: TikiDealDiscount
        public async Task<ActionResult> Index()
        {
            return View();
        }

        private static async Task<List<DealCreatedResponseDetail>> SearchDealCoreOfOneSku(string sku, MySqlConnection conn)
        {
            List<DealCreatedResponseDetail> listDeal = await DealAction.SearchDealOfOneSku(sku);
            // Insert nếu chưa tồn tại
            if (listDeal != null && listDeal.Count > 0)
            {
                await sqler.InsertCheckExistTikiDealDiscountOfSkuListConnectOutAsync(listDeal, conn);
            }
            return listDeal;
        }

        private static async Task<List<DealCreatedResponseDetail>> SearchDealCoreOfSkuList(
            List<string> skuList,
            int is_active,
            MySqlConnection conn)
        {
            List<DealCreatedResponseDetail> listDeal = await DealAction.SearchDealOfSkuList(skuList, is_active);
            // Insert nếu chưa tồn tại
            if (listDeal != null && listDeal.Count > 0)
            {
                await sqler.InsertCheckExistTikiDealDiscountOfSkuListConnectOutAsync(listDeal, conn);
            }
            return listDeal;
        }

        [HttpPost]
        public async Task<string> GetDealDiscountOfSku(string sku)
        {
            List<DealCreatedResponseDetail> listDeal = new List<DealCreatedResponseDetail>();

            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(listDeal);
            }

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                listDeal = await SearchDealCoreOfOneSku(sku, conn);
            }

            return JsonConvert.SerializeObject(listDeal);
        }

        [HttpPost]
        public async Task<string> GetTikiIdBySku(string sku)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return "0";
            }

            int tikiId;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                tikiId = await sqler.GetTikiIdBySkuAsync(sku, conn);
            }

            return tikiId.ToString();
        }

        [HttpPost]
        public async Task<string> SaveDealDiscountOfAllSku()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    // Lấy danh sách sku đang bật bán và lấy chương trình giảm giá lần lượt
                    List<string> skuList = await sqler.GetListSkuOfActiveItemConnectOutAsync(conn);

                    List<DealCreatedResponseDetail> listDeal = await DealAction.SearchDealOfSkuList(skuList, -1);
                    // Insert nếu chưa tồn tại
                    if (listDeal != null && listDeal.Count > 0)
                    {
                        await sqler.InsertCheckExistTikiDealDiscountOfSkuListConnectOutAsync(listDeal, conn);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return JsonConvert.SerializeObject(result);
        }

        static public async Task<List<SimpleTikiProduct>> GetItemsNoDealDiscountRunning_CoreAsync(MySqlConnection conn,
            Boolean isUpdateStatusFromTiki)
        {
            // Lấy trạng thái từ Tiki deal đang chạy
            if (isUpdateStatusFromTiki)
            {
                UpdateDealStatusOfRunningDealOnDB_Core(conn);
            }

            // Lấy những sản phẩm chưa gắn với deal nào
            List<SimpleTikiProduct> simpleTikiProducts = new List<SimpleTikiProduct>();
            simpleTikiProducts = await sqler.GetItemsNoDealDiscountRunningAsync(conn);

            List<SimpleTikiProduct> listSimpleTikiProductTemp = new List<SimpleTikiProduct>();
            foreach (var simpleTiki in simpleTikiProducts)
            {
                // Tồn kho bằng 0 không set được chương trình giảm giá vì
                // tiki sẽ tắt chương trình giảm giá
                // Loại bỏ Item đã mapping nhưng tồn kho = 0.
                // Chưa mapping cũng không set deal
                if (simpleTiki.models[0].mapping.Count == 0 ||
                     (simpleTiki.models[0].mapping.Count > 0 &&
                     simpleTiki.models[0].GetQuatityFromListMapping() == 0))
                {
                    continue;
                }
                listSimpleTikiProductTemp.Add(simpleTiki);
            }
            simpleTikiProducts = listSimpleTikiProductTemp;

            return simpleTikiProducts;
        }

        [HttpPost]
        public async Task<string> GetItemsNoDealDiscountRunning()
        {
            List<SimpleTikiProduct> listSimpleTikiProduct = new List<SimpleTikiProduct>();
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(listSimpleTikiProduct);
            }
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    listSimpleTikiProduct = await GetItemsNoDealDiscountRunning_CoreAsync(conn, false);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                listSimpleTikiProduct = new List<SimpleTikiProduct>();
            }

            return JsonConvert.SerializeObject(listSimpleTikiProduct);
        }

        [HttpPost]
        public async Task<string> GetTaxAndFeeCore(string eEcommerceName)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }
            TaxAndFee taxAndFee = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    taxAndFee = await sqler.GetTaxAndFeeAsync(eEcommerceName, conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return JsonConvert.SerializeObject(taxAndFee);
        }

        [HttpPost]
        public async Task<string> CreateOneDeal(string sku,
            //string special_from_date,
            //string special_to_date,
            int special_price//,
            //int qty_max,
            //int qty_limit)
            )
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    List<DealCreatedResponseDetail> listDeal = await SearchDealCoreOfOneSku(sku, conn);

                    Boolean dontCreateDeal = false;
                    foreach (var deal in listDeal)
                    {
                        if (deal.is_active == 1 || deal.is_active == 2)
                        {
                            result.State = EMySqlResultState.INVALID;
                            result.Message = "Có chương trình giảm giá đang chạy hoặc sắp chạy nên không tạo chương trình giảm giá mới";
                            dontCreateDeal = true;
                            break;
                        }
                    }
                    if (!dontCreateDeal)
                    {
                        CreatingRequestBody creatingRequestBody = new CreatingRequestBody();

                        creatingRequestBody.ls.Add(new CreatingRequestBodyObject(sku, special_price));
                        DealCreatingResponse dealCreatingResponse = await DealAction.CreateDeal(creatingRequestBody);
                        if (dealCreatingResponse == null)
                        {
                            result.State = EMySqlResultState.ERROR;
                            result.Message = "DealAction.CreateDeal return null";
                        }
                        else
                        {
                            if (dealCreatingResponse.dealResponseStatus != null)
                            {
                                result.State = EMySqlResultState.ERROR;
                                result.Message = dealCreatingResponse.dealResponseStatus.error + ". " +
                                    dealCreatingResponse.dealResponseStatus.message.vi;
                            }
                            else
                            {
                                if (dealCreatingResponse.dealList != null && dealCreatingResponse.dealList.Count > 0)
                                {
                                    // Lưu chương trình tạo thành công
                                    // Insert nếu chưa tồn tại
                                    await sqler.InsertCheckExistTikiDealDiscountOfOneSkuConnectOutAsync(dealCreatingResponse.dealList, conn);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> OffDealFromId(int dealId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = new MySqlResultState();

            CreatingRequestBody creatingRequestBody = new CreatingRequestBody();
            List<int> ls = new List<int>();
            ls.Add(dealId);
            List<DealOffResponseObject> lsDealOff = await DealAction.OffDeal(ls);
            if (lsDealOff == null)
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = "DealAction.OffDealFromId return null";
            }
            else
            {
                // Cập nhật trạng thái chương trình giảm giá
                ls.Clear();
                foreach (var obj in lsDealOff)
                {
                    if (obj.success)
                    {
                        ls.Add(obj.id);
                    }
                }

                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    // Insert nếu chưa tồn tại
                    result = await sqler.UpdateIsActiveCloseFromLsDealIdAsync(ls, conn);
                }

            }
            return JsonConvert.SerializeObject(result);
        }

        // Hàm này mục đích cập nhật trạng thái mới nhất của những deal đang chạy
        // vì tiki có thể tắt deal mà mình không biết
        // Hàm này mất thời gian. Tính bằng 10 phút và gọi API TIKI liên tục. Hạn chế sử dụng.
        public static async Task UpdateDealStatusOfRunningDealOnDB_Core(MySqlConnection conn)
        {
            List<SimpleTikiProduct> simpleTikiProducts = await sqler.GetItemsHasDealDiscountRunningAsync(conn);
            List<string> skuList = new List<string>();
            foreach (var pro in simpleTikiProducts)
            {
                skuList.Add(pro.sku);
            }

            // Lấy deal từ TIki và thêm mới / cập nhật trạng thái của deal mới nhất
            List<DealCreatedResponseDetail> listDeal = await DealAction.SearchDealOfSkuList(skuList, 2);
            // Tìm trong skuList những sku không tồn tại trong listDeal, đó là những sku đã bị tắt
            List<string> strList1 = new List<string>();
            foreach(var deal in listDeal)
            {
                strList1.Add(deal.sku);
            }
            // khi chạy khoảng 700 sku, lấy is_active = 2,
            // nhưng kết quả có 1 trường hợp sku = '7053687777096' trả về cả
            // is_active = 2 và is_active = 5
            // Tìm phần tử có trong strList2 nhưng không có trong strList1
            List<string> skuListOff = skuList.Except(strList1).ToList();
            await sqler.UpdateIsActiveCloseFromSkuAsync(skuListOff, conn);
        }

        [HttpPost]
        public async Task<string> UpdateDealStatusOfRunningDealOnDB()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    UpdateDealStatusOfRunningDealOnDB_Core(conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> UpdatePriceToBookCoverPriceOfAll()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = new MySqlResultState();
            // Lấy tất cả sản phẩm trên sàn
            ProductECommerceController productECommerceController = new ProductECommerceController();
            List<CommonItem> lsCommonItem =
                await productECommerceController.TikiGetProductAll();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    int bookCoverPrice = 0;
                    foreach (var commonItem in lsCommonItem)
                    {
                        if(!commonItem.bActive)
                        {
                            continue;
                        }
                        bookCoverPrice = commonItem.models[0].GetBookCoverPrice();
                        // Tính lại giá bán (price) = giá bìa
                        if (bookCoverPrice == 0) // chưa có mapping hoặc sản phẩm đang để giá vô lý = 0
                        {
                            bookCoverPrice = commonItem.models[0].market_price;
                        }

                        if (bookCoverPrice == 0)
                        {
                            continue;
                        }

                        // Set giá
                        TikiUpdateStock.TikiProductUpdatePrice((int)commonItem.itemId, bookCoverPrice, result);
                        if(result.State != EMySqlResultState.OK)
                        {
                            MyLogger.GetInstance().Info(JsonConvert.SerializeObject(result.myJson));
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

        public static async Task<MySqlResultState> CreateDealForAllCoreAsync(List<SimpleTikiProduct> listSimpleTikiProduct,
            MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();

            // Lấy danh sách nhà phát hành, từ đó lấy được discount chung
            PublisherMySql publisherSqler = new PublisherMySql();
            List<Publisher> listPublisher = await publisherSqler.GetListPublisherConnectOutAsync(conn);

            // Lấy danh sách thuế phí
            TaxAndFee taxAndFee = await sqler.GetTaxAndFeeAsync(Common.eTiki, conn);

            // Tính giá bìa, chiết khấu hợp lý theo nhà phát hành hoặc sản phẩm, thuế, phí, lợi nhuận mong muốn.
            // Từ đó tính giá bán.
            int sale_price;
            List<CreatingRequestBody> listCreatingRequestBody = new List<CreatingRequestBody>();
            CreatingRequestBody creatingRequestBodyTemp = null;
            try
            {
                foreach (var simpleTiki in listSimpleTikiProduct)
                {
                    sale_price = CommonOpenPlatform.CaculateSpecialPriceCoreFromCommonModel(simpleTiki.models[0],
                        listPublisher,
                        taxAndFee);
                    if (!string.IsNullOrEmpty(simpleTiki.sku) && sale_price != 0)
                    {
                        creatingRequestBodyTemp = new CreatingRequestBody();
                        listCreatingRequestBody.Add(creatingRequestBodyTemp);

                        creatingRequestBodyTemp.ls.Add(new CreatingRequestBodyObject(simpleTiki.sku, sale_price));
                    }
                    else
                    {
                        MyLogger.GetInstance().Info(JsonConvert.SerializeObject(simpleTiki));
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Info(ex.ToString());
            }

            List<DealCreatingResponse> listFailDealCreatingResponse = new List<DealCreatingResponse>();
            foreach (var creatingRequestBody in listCreatingRequestBody)
            {
                DealCreatingResponse dealCreatingResponse = await DealAction.CreateDeal(creatingRequestBody);
                if (dealCreatingResponse == null)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "DealAction.CreateDeal return null.";
                }
                else
                {
                    if (dealCreatingResponse.dealResponseStatus != null)
                    {
                        listFailDealCreatingResponse.Add(dealCreatingResponse);
                    }
                    else
                    {
                        if (dealCreatingResponse.dealList != null && dealCreatingResponse.dealList.Count > 0)
                        {
                            // Lưu chương trình tạo thành công
                            // Insert nếu chưa tồn tại
                            await sqler.InsertCheckExistTikiDealDiscountOfOneSkuConnectOutAsync(dealCreatingResponse.dealList, conn);
                        }
                    }
                }
            }
            if(listFailDealCreatingResponse.Count > 0)
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = "Check log để xem chi tiết lỗi từng sku";
                MyLogger.GetInstance().Info(JsonConvert.SerializeObject(listFailDealCreatingResponse));
            }
            return result;
        }
        [HttpPost]
        public async Task<string> CreateDealForAllInTheTable(string listItem)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = null;
            List<SimpleTikiProduct> listSimpleTikiProduct = JsonConvert.DeserializeObject<List<SimpleTikiProduct>>(listItem);
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    result = await CreateDealForAllCoreAsync(listSimpleTikiProduct, conn);
                }
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Info(ex.ToString());
                result = new MySqlResultState(EMySqlResultState.ERROR, ex.ToString());
            }
            return JsonConvert.SerializeObject(result);
        }
    }
}
