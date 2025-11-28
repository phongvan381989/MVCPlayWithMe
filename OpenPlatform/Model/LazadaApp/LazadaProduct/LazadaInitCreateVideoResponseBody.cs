using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct
{

    public class LazadaInitCreateVideoResponseBody : CommonLazadaResponseHTTP
    {
        // return upload_id for further operation
        public string upload_id { get; set; }

        // whether the operation succeeds
        public Boolean success { get; set; }

        // error code when the operation fails
        public string result_code { get; set; }

        // error message when the operation fails
        public string result_message { get; set; }
    }
}