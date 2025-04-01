using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Deal
{
    // Tạo JsonConverter để format DateTime theo "yyyy-MM-dd HH:mm:ss"
    public class CustomDateTimeConverter : IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            Culture = CultureInfo.InvariantCulture;
        }
    }

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
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime specialFromDate { get; set; }

        // Thời điểm chương trình giảm giá kết thúc
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime specialToDate { get; set; }

        // Giá khuyến mãi (thực tế bán). Giá khuyến mãi  không nhỏ hơn 50% giá bán (giá cấu hình sản phẩm = giá bìa)
        public int specialPrice { get; set; }

        // Số lượng khuyến mãi. Ví dụ: 100 sản phẩm, khi bán được 100 sản phẩm thì hết giá khuyến mại.
        // Không được chỉnh sửa sau khi kích hoạt khuyến mại.
        // Đây không phải số lượng tồn kho.
        public int qtyMax { get; set; }

        // Số lượng tối đa có thể đặt trên mỗi đơn hàng.
        public int qtyLimit { get; set; }

        public CreatingRequestBodyObject(string skuInput,
            DateTime specialFromDateInput,
            DateTime specialToDateInput,
            int specialPriceInput,
            int qtyMaxInput,
            int qtyLimitInput)
        {
            sku = skuInput;
            specialFromDate = specialFromDateInput;
            specialToDate = specialToDateInput;
            specialPrice = specialPriceInput;
            qtyMax = qtyMaxInput;
            qtyLimit = qtyLimitInput;
        }
    }
}