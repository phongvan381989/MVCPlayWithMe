using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.ProductModel;
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
        public ActionResult Index()
        {
            return View();
        }

        private static List<DealCreatedResponseDetail> SearchDealCoreOfOneSku(string sku, MySqlConnection conn)
        {
            List<DealCreatedResponseDetail> listDeal = DealAction.SearchDealOfOneSku(sku);
            // Insert nếu chưa tồn tại
            if (listDeal != null && listDeal.Count > 0)
            {
                sqler.InsertCheckExistTikiDealDiscountOfSkuListConnectOut(listDeal, conn);
            }
            return listDeal;
        }

        private static List<DealCreatedResponseDetail> SearchDealCoreOfSkuList(
            List<string> skuList,
            int is_active,
            MySqlConnection conn)
        {
            List<DealCreatedResponseDetail> listDeal = DealAction.SearchDealOfSkuList(skuList, is_active);
            // Insert nếu chưa tồn tại
            if (listDeal != null && listDeal.Count > 0)
            {
                sqler.InsertCheckExistTikiDealDiscountOfSkuListConnectOut(listDeal, conn);
            }
            return listDeal;
        }

        [HttpPost]
        public string GetDealDiscountOfSku(string sku)
        {
            List<DealCreatedResponseDetail> listDeal = new List<DealCreatedResponseDetail>();

            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(listDeal);
            }

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            conn.Open();
            listDeal = SearchDealCoreOfOneSku(sku, conn);
            conn.Close();

            return JsonConvert.SerializeObject(listDeal);
        }

        [HttpPost]
        public string GetTikiIdBySku(string sku)
        {
            if (AuthentAdministrator() == null)
            {
                return "0";
            }

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            conn.Open();
            int tikiId = sqler.GetTikiIdBySku(sku, conn);
            conn.Close();

            return tikiId.ToString();
        }

        [HttpPost]
        public string SaveDealDiscountOfAllSku()
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();

                    // Lấy danh sách sku đang bật bán và lấy chương trình giảm giá lần lượt
                    List<string> skuList = sqler.GetListSkuOfActiveItemConnectOut(conn);

                    List<DealCreatedResponseDetail> listDeal = DealAction.SearchDealOfSkuList(skuList, -1);
                    // Insert nếu chưa tồn tại
                    if (listDeal != null && listDeal.Count > 0)
                    {
                        sqler.InsertCheckExistTikiDealDiscountOfSkuListConnectOut(listDeal, conn);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return JsonConvert.SerializeObject(result);
        }

        static public List<SimpleTikiProduct> GetItemsNoDealDiscountRunning_Core(MySqlConnection conn,
            Boolean isUpdateStatusFromTiki)
        {
            // Lấy trạng thái từ Tiki deal đang chạy
            if (isUpdateStatusFromTiki)
            {
                UpdateDealStatusOfRunningDealOnDB_Core(conn);
            }

            // Lấy những sản phẩm chưa gắn với deal nào
            List<SimpleTikiProduct> simpleTikiProducts = new List<SimpleTikiProduct>();
            simpleTikiProducts = sqler.GetItemsNoDealDiscountRunning(conn);

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
        public string GetItemsNoDealDiscountRunning()
        {
            List<SimpleTikiProduct> listSimpleTikiProduct = new List<SimpleTikiProduct>();
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(listSimpleTikiProduct);
            }
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
                    listSimpleTikiProduct = GetItemsNoDealDiscountRunning_Core(conn, false);
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
        public string GetTaxAndFeeCore(string eEcommerceName)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(null);
            }
            TaxAndFee taxAndFee = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
                    taxAndFee = sqler.GetTaxAndFee(eEcommerceName, conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return JsonConvert.SerializeObject(taxAndFee);
        }

        [HttpPost]
        public string CreateOneDeal(string sku,
            //string special_from_date,
            //string special_to_date,
            int special_price//,
            //int qty_max,
            //int qty_limit)
            )
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
                    List<DealCreatedResponseDetail> listDeal = SearchDealCoreOfOneSku(sku, conn);

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
                        DealCreatingResponse dealCreatingResponse = DealAction.CreateDeal(creatingRequestBody);
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
                                    sqler.InsertCheckExistTikiDealDiscountOfOneSkuConnectOut(dealCreatingResponse.dealList, conn);
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
        public string OffDealFromId(int dealId)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = new MySqlResultState();

            CreatingRequestBody creatingRequestBody = new CreatingRequestBody();
            List<int> ls = new List<int>();
            ls.Add(dealId);
            List<DealOffResponseObject> lsDealOff = DealAction.OffDeal(ls);
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

                MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
                conn.Open();
                // Insert nếu chưa tồn tại
                result = sqler.UpdateIsActiveCloseFromLsDealId(ls, conn);
                conn.Close();

            }
            return JsonConvert.SerializeObject(result);
        }

        // Xây dựng công thức tính giá bán
        // p: giá bìa, 
        // dI: chiết khấu nhập, VD: 0.4
        // dO: chiết khấu bán,
        // x: % lợi nhuận mong muốn so với GIÁ NHẬP SÁCH
        // t: phần trăm phí trả sản + thuế nộp nhà nước so với giá bán trên sàn
        // c: chi phí đóng gói cố đinh
        // m: lợi nhuận tuyệt đối. Nếu giá bìa lớn ta có thể chiết khấu sâu hơn khi bán để lợi nhuận tối thiểu bằng m
        // Giá bán  - giá nhập  - thuế phí   - phí đóng hàng = lợi nhuận còn lại
        // p(1 - dO) - p(1 -dI) - p(1 - dO)t - c             = p(1 - dI) x
        //
        // p(1 - dO)(1 - t) = p(1 -dI) + c + p(1 - dI) x
        // =>p(1 - dO) = (p(1 -dI) + c + p(1 - dI)x) / (1 - t)
        // Làm tròn xuống thành số nguyên
        private static int CaculateSalePriceCore(int p, float dI, float x, float t, int c, int m)
        {
            if (p * (1 - dI) * x > m)
            {
                x = m / (p * (1 - dI));
            }

            int salePrice = (int)Math.Floor((p * (1 - dI) + c + p * (1 - dI) * x) / (1 - t));

            // Làm tròn salePrice là bội của 100 VND
            if (salePrice % 100 != 0)
            {
                salePrice = salePrice - salePrice % 100;
            }

            // Vì sản phẩm giá bìa thấp, để đạt % như mong muốn giá bán cần cao hơn cả giá bìa
            // Ta tính lại giá bán, bán dưới điểm hòa vốn
            if (salePrice >= p)
            {
                salePrice = p * (100 - TikiConstValues.constDiscount) / 100;
                // Làm tròn salePrice là bội của 100 VND
                if (salePrice % 100 != 0)
                {
                    salePrice = salePrice - salePrice % 100;
                }
            }

            return salePrice;
        }

        // Xây dựng công thức tính giá bán
        // p: giá bìa, 
        // dI: chiết khấu nhập, VD: 0.4
        // dO: chiết khấu bán,
        // x: % lợi nhuận mong muốn so với GIÁ BÁN
        // t: phần trăm phí trả sản + thuế nộp nhà nước so với giá bán trên sàn
        // c: chi phí đóng gói cố đinh
        // NOTE: Bỏ qua lợi nhuận tuyệt đối m
        // m: lợi nhuận tuyệt đối. Nếu giá bìa lớn ta có thể chiết khấu sâu hơn khi bán để lợi nhuận tối thiểu bằng m
        // Giá bán  - giá nhập  - thuế phí   - phí đóng hàng = lợi nhuận còn lại
        // p(1 - dO) - p(1 -dI) - p(1 - dO)t - c             = p(1 - dO) x
        //
        // p(1 - dO)(1 - t -x) = p(1 -dI) + c
        // =>p(1 - dO) = (p(1 -dI) + c) / (1 - t - x)
        // Làm tròn xuống thành số nguyên
        private static int CaculateSalePriceCore_Ver2(int p, float dI, float x, float t, int c, int m)
        {
            //if (p * (1 - dO) * x > m)
            //{
            //    x = m / (p * (1 - dI));
            //}

            int salePrice = 0;
            if(p == 0)
            {
                return salePrice;
            }

            salePrice = (int)Math.Floor((p * (1 - dI) + c) / (1 - t - x));

            // Vì sản phẩm giá bìa thấp, để đạt % như mong muốn giá bán cần cao hơn cả giá bìa
            // Ta tính lại giá bán, bán dưới điểm hòa vốn => Chấp nhận lỗ
            if (salePrice >= p)
            {
                salePrice = p * (100 - TikiConstValues.constDiscount) / 100;
                // Làm tròn salePrice là bội của 100 VND
                if (salePrice % 100 != 0)
                {
                    salePrice = salePrice - salePrice % 100;
                }
            }

            // Làm tròn salePrice là bội của 100 VND
            if (salePrice % 100 != 0)
            {
                salePrice = salePrice - salePrice % 100;
            }

            return salePrice;
        }

        public static int CaculateSalePriceCoreFromCommonModel(CommonModel commonModel,
            List<Publisher> listPublisher,
            TaxAndFee taxAndFee
            )
        {
            int p = commonModel.GetBookCoverPrice();
            int salePrice = 0;
            if (p == 0)
            {
                return salePrice;
            }

            // Lấy chiết khấu với 1 chữ số sau dấu phảy
            float dI = commonModel.GetDiscount(listPublisher) / 100;

            salePrice = CaculateSalePriceCore_Ver2(p, dI,
                taxAndFee.expectedPercentProfit / 100,
                (taxAndFee.tax + taxAndFee.fee) / 100,
                taxAndFee.packingCost,
                taxAndFee.minProfit);

            return salePrice;
        }


        // Hàm này mục đích cập nhật trạng thái mới nhất của những deal đang chạy
        // vì tiki có thể tắt deal mà mình không biết
        // Hàm này mất thời gian. Tính bằng 10 phút và gọi API TIKI liên tục. Hạn chế sử dụng.
        public static void UpdateDealStatusOfRunningDealOnDB_Core(MySqlConnection conn)
        {
            List<SimpleTikiProduct> simpleTikiProducts = sqler.GetItemsHasDealDiscountRunning(conn);
            List<string> skuList = new List<string>();
            foreach (var pro in simpleTikiProducts)
            {
                skuList.Add(pro.sku);
            }

            // Lấy deal từ TIki và thêm mới / cập nhật trạng thái của deal mới nhất
            List<DealCreatedResponseDetail> listDeal = DealAction.SearchDealOfSkuList(skuList, 2);
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
            sqler.UpdateIsActiveCloseFromSku(skuListOff, conn);
        }

        [HttpPost]
        public string UpdateDealStatusOfRunningDealOnDB()
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
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
        public string UpdatePriceToBookCoverPriceOfAll()
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = new MySqlResultState();
            // Lấy tất cả sản phẩm trên sàn
            ProductECommerceController productECommerceController = new ProductECommerceController();
            List<CommonItem> lsCommonItem =
                productECommerceController.TikiGetProductAll();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
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

        public static MySqlResultState CreateDealForAllCore(List<SimpleTikiProduct> listSimpleTikiProduct,
            MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            // Lấy danh sách nhà phát hành, từ đó lấy được discount chung
            PublisherMySql publisherSqler = new PublisherMySql();
            List<Publisher> listPublisher = publisherSqler.GetListPublisherConnectOut(conn);

            // Lấy danh sách thuế phí
            TaxAndFee taxAndFee = sqler.GetTaxAndFee(Common.eTiki, conn);

            // Tính giá bìa, chiết khấu hợp lý theo nhà phát hành hoặc sản phẩm, thuế, phí, lợi nhuận mong muốn.
            // Từ đó tính giá bán.
            int sale_price;
            List<CreatingRequestBody> listCreatingRequestBody = new List<CreatingRequestBody>();
            CreatingRequestBody creatingRequestBodyTemp = null;
            try
            {
                ProductMySql productMySql = new ProductMySql();
                foreach (var simpleTiki in listSimpleTikiProduct)
                {
                    sale_price = CaculateSalePriceCoreFromCommonModel(simpleTiki.models[0],
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
                DealCreatingResponse dealCreatingResponse = DealAction.CreateDeal(creatingRequestBody);
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
                            sqler.InsertCheckExistTikiDealDiscountOfOneSkuConnectOut(dealCreatingResponse.dealList, conn);
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
        public string CreateDealForAllInTheTable(string listItem)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = null;
            List<SimpleTikiProduct> listSimpleTikiProduct = JsonConvert.DeserializeObject<List<SimpleTikiProduct>>(listItem);
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
                    result = CreateDealForAllCore(listSimpleTikiProduct, conn);
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