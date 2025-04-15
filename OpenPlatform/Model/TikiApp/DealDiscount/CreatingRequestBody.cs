using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.DealDiscount
{
    // Lớp sinh ra request body của HTTP POST tạo deal - giảm giá sản phẩm
    public class CreatingRequestBody
    {
        // Maximum là 100 item.
        public List<CreatingRequestBodyObject> ls { get; set; }

        public CreatingRequestBody()
        {
            ls = new List<CreatingRequestBodyObject>();
        }

        public string GetJson()
        {
            var settings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss",
                Culture = CultureInfo.InvariantCulture
            };

            string json = JsonConvert.SerializeObject(ls, settings);
            return json;
        }
    }
}