using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Deal
{
    // Lớp sinh ra request body của HTTP POST tạo deal - giảm giá sản phẩm
    public class CreatingRequestBody
    {
        // Tài liệu không ghi tự đặt ra độ dài ls là 50.
        List<CreatingRequestBodyObject> ls { get; set; }

        public CreatingRequestBody()
        {
            ls = new List<CreatingRequestBodyObject>();
        }

        public string GetJson()
        {
            string json = JsonConvert.SerializeObject(ls, Formatting.Indented);
            return json;
        }
    }
}