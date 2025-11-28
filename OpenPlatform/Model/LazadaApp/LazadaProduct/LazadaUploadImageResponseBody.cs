using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct
{
    public class LazadaUploadImageResponseBody : CommonLazadaResponseHTTP
    {
        public LazadaUploadImageData data { get; set; }
    }

    public class LazadaUploadImageData
    {
        public LazadaUploadImage image { get; set; }
    }

    public class LazadaUploadImage
    {
        // Chuối hash lazada trả về khác giá trị test hash ảnh với md5
        // Dự là lazda hash ảnh + thông tin nào đó trong request. Nên không thể hash ảnh rồi kiểm tra trong db
        // đã được up lên lazada chưa?
        public string hash_code { get; set; }
        public string url { get; set; }
    }
}