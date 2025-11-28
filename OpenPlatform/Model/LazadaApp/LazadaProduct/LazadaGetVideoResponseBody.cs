using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct
{
    public class LazadaGetVideoResponseBody : CommonLazadaResponseHTTP
    {
        // cover url of the video
        public string cover_url { get; set; }

        // url of the video
        public string video_url { get; set; }

        // whether the operation succeeds
        public Boolean success { get; set; }

        // error code when the operation fails
        public string result_code { get; set; }

        // possible values: READY_FOR_TRANSCODE, TRANSCODING, TRANSCODE_FAILED, READY_FOR_AUDIT,
        // AUDIT_FAILED, AUDIT_SUCCESS, DELETED
        public string state { get; set; }

        public string title { get; set; }

        // error message when the operation fails
        public string result_message { get; set; }
    }
}