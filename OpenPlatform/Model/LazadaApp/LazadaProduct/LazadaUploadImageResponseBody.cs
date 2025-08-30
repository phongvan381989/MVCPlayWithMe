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
        public string hash_code { get; set; }
        public string url { get; set; }
    }
}