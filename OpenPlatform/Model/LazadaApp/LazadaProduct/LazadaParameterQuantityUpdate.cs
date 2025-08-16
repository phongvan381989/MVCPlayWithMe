using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct
{
    public class LazadaParameterQuantity_PriceUpdate
    {
        public long itemId { get; set; }
        public long skuId { get; set; }
        public string sellerSku { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public decimal salePrice { get; set; }

        // nếu cập nhật lỗi, lưu code, message lỗi
        public string code { get; set; }
        public string message { get; set; }

        public LazadaParameterQuantity_PriceUpdate(long initemId, long inskuId, int inquantity)
        {
            itemId = initemId;
            skuId = inskuId;
            quantity = inquantity;
        }

        public LazadaParameterQuantity_PriceUpdate(long initemId, long inskuId,
            decimal inprice, decimal insalePrice)
        {
            itemId = initemId;
            skuId = inskuId;
            price = inprice;
            salePrice = insalePrice;
        }
    }
}