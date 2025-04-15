using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
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
        public TikiDealDiscountMySql sqler;

        public TikiDealDiscountController()
        {
            sqler = new TikiDealDiscountMySql();
        }

        // GET: TikiDealDiscount
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string GetDealDiscountOfSku(string sku)
        {
            List<DealCreatedResponseDetail> listDeal = new List<DealCreatedResponseDetail>();
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(listDeal);
            }

            DealAction.SearchDeal(sku, listDeal);

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            conn.Open();
            // Insert nếu chưa tồn tại
            sqler.InsertTikiDealDiscountConnectOut(listDeal, conn);
            conn.Close();

            return JsonConvert.SerializeObject(listDeal);
        }

        [HttpPost]
        public string SaveDealDiscountOfAllSku()
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();

                    // Lấy danh sách sku đang bật bán và lấy chương trình giảm giá lần lượt
                    List<string> skuList = sqler.GetListSkuOfActiveItemConnectOut(conn);

                    List<DealCreatedResponseDetail> listDeal = new List<DealCreatedResponseDetail>();
                    foreach (var sku in skuList)
                    {
                        listDeal.Clear();
                        DealAction.SearchDeal(sku, listDeal);

                        // Insert nếu chưa tồn tại
                        sqler.InsertTikiDealDiscountConnectOut(listDeal, conn);
                        Thread.Sleep(300);
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
        public string GetItemsNoDealDiscountRunning()
        {
            List<SimpleTikiProduct> simpleTikiProducts = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
                    simpleTikiProducts = sqler.GetItemsNoDealDiscountRunning(conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return JsonConvert.SerializeObject(simpleTikiProducts);
        }

        [HttpPost]
        public string GetTaxAndFeeCore(string eEcommerceName)
        {
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
            MySqlResultState result = new MySqlResultState();

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
                    if (dealCreatingResponse.dealList != null)
                    {
                        // Lưu chương trình tạo thành công
                        MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
                        conn.Open();
                        // Insert nếu chưa tồn tại
                        sqler.InsertTikiDealDiscountConnectOut(dealCreatingResponse.dealList, conn);
                        conn.Close();
                    }
                }
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string OffDealFromId(int dealId)
        {
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
                result = sqler.UpdateIsActiveOff(ls, conn);
                conn.Close();

            }
            return JsonConvert.SerializeObject(result);
        }

        // Xây dựng công thức tính giá bán
        // p: giá bìa, 
        // dI: chiết khấu nhập, VD: 0.4
        // dO: chiết khấu bán,
        // x: % lợi nhuận mong muốn so với giá thực tế nhập sách
        // t: phần trăm phí trả sản + thuế nộp nhà nước so với giá bán trên sàn
        // c: chi phí đóng gói cố đinh
        // m: lợi nhuận tuyệt đối. Nếu giá bìa lớn ta có thể chiết khấu sâu hơn khi bán để lợi nhuận tối thiểu bằng m
        // Giá bán  - giá nhập  - thuế phí   - phí đóng hàng = lợi nhuận còn lại
        // p(1 - dO) - p(1 -dI) - p(1 - dO)t - c             = p(1 - dI) x
        //
        // p(1 - dO)(1 - t) = p(1 -dI) + c + pdIx
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

            return salePrice;
        }

        public static int CaculateSalePriceCoreFromCommonItem(CommonItem commonItem,
            List<Publisher> listPublisher,
            TaxAndFee taxAndFee
            )
        {
            int p = commonItem.models[0].GetBookCoverPrice();
            int salePrice = 0;
            if (p == 0)
            {
                return salePrice;
            }

            // Lấy chiết khấu với 1 chữ số sau dấu phảy
            float dI = commonItem.models[0].GetDiscount(listPublisher) / 100;

            salePrice = CaculateSalePriceCore(p, dI,
                taxAndFee.expectedPercentProfit / 100,
                (taxAndFee.tax + taxAndFee.fee) / 100,
                taxAndFee.packingCost,
                taxAndFee.minProfit);
            return salePrice;
        }

        [HttpPost]
        public string UpdateDealStatusOfRunningDealOnDB()
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
                    List<SimpleTikiProduct> simpleTikiProducts = sqler.GetItemsHasDealDiscountRunning(conn);

                    // Lấy deal
                    List<DealCreatedResponseDetail> listDeal = new List<DealCreatedResponseDetail>();
                    foreach (var pro in simpleTikiProducts)
                    {
                        DealAction.SearchDeal(pro.sku, listDeal);
                    }

                    // Insert nếu chưa tồn tại
                    sqler.InsertTikiDealDiscountConnectOut(listDeal, conn);
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

        [HttpPost]
        public string CreateDealForAllInTheTable(string listItem)
        {
            MySqlResultState result = new MySqlResultState();
            List<SimpleTikiProduct> listSimpleTikiProduct = JsonConvert.DeserializeObject<List<SimpleTikiProduct>>(listItem);

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            conn.Open();
            // Lấy danh sách commonItem
            List<CommonItem> listCommonItem = new List<CommonItem>();
            TikiMySql tikiSqler = new TikiMySql();
            foreach(var simpleTiki in listSimpleTikiProduct)
            {
                CommonItem item = new CommonItem();
                item.models.Add(new CommonModel());
                item.sku = simpleTiki.sku;
                item.itemId = simpleTiki.id;
                tikiSqler.TikiGetItemFromIdConnectOut(simpleTiki.id, item, conn);
                if (item.models[0].mapping.Count > 0) // Đã mapping
                {
                    listCommonItem.Add(item);
                }
            }

            // Lấy danh sách nhà phát hành, từ đó lấy được discount chung
            PublisherMySql publisherSqler = new PublisherMySql();
            List<Publisher> listPublisher = publisherSqler.GetListPublisherConnectOut(conn);

            // Lấy danh sách thuế phí
            TaxAndFee taxAndFee = sqler.GetTaxAndFee(Common.eTiki, conn);

            // Tính giá bìa, chiết khấu hợp lý theo nhà phát hành hoặc sản phẩm, thuế, phí, lợi nhuận mong muốn.
            // Từ đó tính giá bán.
            int special_price;
            List<CreatingRequestBody> listCreatingRequestBody = new List<CreatingRequestBody>();
            CreatingRequestBody creatingRequestBodyTemp = null;
            try
            {
                ProductMySql productMySql = new ProductMySql();
                foreach (var commonItem in listCommonItem)
                {

                    // Nếu tồn kho lớn hơn 0 mới chạy chương trình giảm giá
                    if(productMySql.TikiGetQuantityOfOneItemModelConnectOut((int)commonItem.itemId, conn) == 0)
                    {
                        continue;
                    }

                    special_price = CaculateSalePriceCoreFromCommonItem(commonItem, listPublisher, taxAndFee);
                    if (!string.IsNullOrEmpty(commonItem.sku) && special_price != 0)
                    {
                        creatingRequestBodyTemp = new CreatingRequestBody();
                        listCreatingRequestBody.Add(creatingRequestBodyTemp);

                        creatingRequestBodyTemp.ls.Add(new CreatingRequestBodyObject(commonItem.sku, special_price));
                    }
                    else
                    {
                        MyLogger.GetInstance().Info(JsonConvert.SerializeObject(commonItem));
                    }
                }
            }
            catch(Exception ex)
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
                    //break;
                }
                else
                {
                    if (dealCreatingResponse.dealResponseStatus != null)
                    {
                        // Tiếp tục dù lỗi
                        //result.State = EMySqlResultState.ERROR;
                        //result.Message = dealCreatingResponse.dealResponseStatus.error + ". " +
                        //    dealCreatingResponse.dealResponseStatus.message.vi;
                        listFailDealCreatingResponse.Add(dealCreatingResponse);
                    }
                    else
                    {
                        if (dealCreatingResponse.dealList != null)
                        {
                            // Lưu chương trình tạo thành công
                            // Insert nếu chưa tồn tại
                            sqler.InsertTikiDealDiscountConnectOut(dealCreatingResponse.dealList, conn);
                        }
                    }
                }
            }

            conn.Close();
            if (result.State == EMySqlResultState.ERROR || listFailDealCreatingResponse.Count > 0)
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = "Check log để xem chi tiết lỗi từng sku";
                MyLogger.GetInstance().Info(JsonConvert.SerializeObject(listFailDealCreatingResponse));
            }
            return JsonConvert.SerializeObject(result);
        }
    }
}