using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    public class TikiCancelInfo
    {
        /// <summary>
        /// 2034	Unique cancel reason code
        /// </summary>
        public string reason_code { get; set; }

        /// <summary>
        /// Thời gian giao hàng quá lâu/sớm	Cancel reason short description. This may include customer’s explanation.
        /// </summary>
        public string reason_text { get; set; }

        /// <summary>
        /// 	Cancel reason details description
        /// </summary>
        public string comment { get; set; }

        /// <summary>
        /// 2020-06-16 23:59:43	When the order is canceled
        /// </summary>
        public DateTime canceled_at { get; set; }
    }
}
