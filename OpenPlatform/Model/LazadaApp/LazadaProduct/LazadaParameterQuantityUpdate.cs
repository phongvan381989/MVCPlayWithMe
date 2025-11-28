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
        public int quantity { get; set; } // -1 nếu không cần cập nhật
        public decimal price { get; set; } // -1 nếu không cần cập nhật
        public decimal salePrice { get; set; } // -1 nếu không cần cập nhật

        // nếu cập nhật lỗi, lưu code, message lỗi
        public string code { get; set; }
        public string message { get; set; }

        // Phục vụ set chỉ số lượng
        public LazadaParameterQuantity_PriceUpdate(long initemId, long inskuId, int inquantity)
        {
            itemId = initemId;
            skuId = inskuId;
            quantity = inquantity;
            price = -1;
            salePrice = -1;
        }

        // Phục vụ set giá bìa, giá bán
        public LazadaParameterQuantity_PriceUpdate(long initemId, long inskuId,
            decimal inprice, decimal insalePrice)
        {
            itemId = initemId;
            skuId = inskuId;
            quantity = -1;
            price = inprice;
            salePrice = insalePrice;
        }

        // Phục vụ set số lượng, giá bìa, giá bán
        public LazadaParameterQuantity_PriceUpdate(long initemId, long inskuId,
            int inquantity, decimal inprice, decimal insalePrice)
        {
            itemId = initemId;
            skuId = inskuId;
            quantity = inquantity;
            price = inprice;
            salePrice = insalePrice;
        }
    }
}