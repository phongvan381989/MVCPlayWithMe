using MVCPlayWithMe.General;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.DealDiscount
{
    //// Tạo JsonConverter để format DateTime theo "yyyy-MM-dd HH:mm:ss"
    //public class CustomDateTimeConverter : IsoDateTimeConverter
    //{
    //    public CustomDateTimeConverter()
    //    {
    //        DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    //        Culture = CultureInfo.InvariantCulture;
    //    }
    //}

    public class CreatingRequestBodyObject
    {
        // Ví dụ của đối tượng.
        //{
        //    "sku": "12345678",
        //    "special_from_date": "2022-01-13 02:59:59",
        //    "special_to_date": "2022-01-13 03:59:59",
        //    "special_price": 100000,
        //    "qty_max": 50,
        //    "qty_limit": 30
        //  }

        // sku của sản phẩm tiki cài đặt giảm giá
        public string sku { get; set; }

        // Thời điểm chương trình giảm giá bắt đầu
        // Thời điểm này phải lớn hơn hiện tại và nhỏ hơn thời điểm chương trình kết thúc
        // Chương trình kéo dài không quá 180 ngày
        //[JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime special_from_date { get; set; }

        // Thời điểm chương trình giảm giá kết thúc
        //[JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime special_to_date { get; set; }

        // Giá khuyến mãi (thực tế bán). Giá khuyến mãi  không nhỏ hơn 50% giá bán (giá cấu hình sản phẩm = giá bìa)
        public int special_price { get; set; }

        // Số lượng khuyến mãi. Ví dụ: 100 sản phẩm, khi bán được 100 sản phẩm thì hết giá khuyến mại.
        // Không được chỉnh sửa sau khi kích hoạt khuyến mại.
        // Đây không phải số lượng tồn kho.
        public int qty_max { get; set; }

        // Số lượng tối đa có thể đặt trên mỗi đơn hàng.
        public int qty_limit { get; set; }

        public CreatingRequestBodyObject(string skuInput,
            DateTime specialFromDateInput,
            DateTime specialToDateInput,
            int specialPriceInput,
            int qtyMaxInput,
            int qtyLimitInput)
        {
            sku = skuInput;
            special_from_date = specialFromDateInput;
            special_to_date = specialToDateInput;
            special_price = specialPriceInput;
            qty_max = qtyMaxInput;
            qty_limit = qtyLimitInput;
        }

        // Khoảng 10 phút từ hiện tại chương trình sẽ chạy
        public CreatingRequestBodyObject(string skuInput,
            int specialPriceInput)
        {
            sku = skuInput;
            special_from_date = DateTime.Now.AddMinutes(Common.minutesAddNowTikiDealDiscount);
            special_to_date = special_from_date.AddDays(Common.intervalTikiDealDiscount);
            special_price = specialPriceInput;
            qty_max = Common.qtyMaxTikiDealDiscount;
            qty_limit = Common.qtyLimitTikiDealDiscount;
        }
    }
}