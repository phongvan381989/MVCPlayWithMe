using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct
{
    public class LazadaCompleteCreateVideoResponseBody : CommonLazadaResponseHTTP
    {
        // whether the operation succeeds
        public Boolean success { get; set; }

        // error code when the operation fails
        public string result_code { get; set; }

        // return video_id for further call
        public string video_id { get; set; }

        // error message when the operation fails
        public string result_message { get; set; }
    }
}