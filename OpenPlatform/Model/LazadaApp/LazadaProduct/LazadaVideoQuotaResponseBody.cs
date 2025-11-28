using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct
{
    public class LazadaVideoQuotaResponseBody : CommonLazadaResponseHTTP
    {
        // the max space of all video files. Đơn vị Byte
        public long capacity_size { get; set; }

        // current space taken. Đơn vị Byte
        public long used_size { get; set; }

        // whether the operation succeeds
        public Boolean success { get; set; }

        // error code when the operation fails
        public string result_code { get; set; }

        // error message when the operation fails
        public string result_message { get; set; }
    }
}