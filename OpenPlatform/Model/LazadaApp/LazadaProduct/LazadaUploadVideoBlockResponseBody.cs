using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct
{
    public class LazadaUploadVideoBlockResponseBody : CommonLazadaResponseHTTP
    {
        // whether the operation succeeds
        public Boolean success { get; set; }

        // error code when the operation fails
        public string result_code { get; set; }

        // return e_tag for using in commit operation
        public string e_tag { get; set; }

        // error message when the operation fails
        public string result_message { get; set; }
    }

    public class LazadaUploadVideoBlockeTag
    {
        public int partNumber { get; set; }
        public string eTag { get; set; }

        public LazadaUploadVideoBlockeTag(string eTagIn, int partNumberIn)
        {
            eTag = eTagIn;
            partNumber = partNumberIn;
        }
    }
}