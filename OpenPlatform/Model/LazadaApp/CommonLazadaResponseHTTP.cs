using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.LazadaApp
{
    public class CommonLazadaResponseHTTP
    {
        /// <summary>
        /// error type for error response.
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// error code for error response, zero means successful response.
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// error message for error response.
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// request id for api request.
        /// </summary>
        public string request_id { get; set; }
    }
}
