using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    /// <summary>
    /// Customer tax information
    /// </summary>
    public class TikiTaxInfo
    {
        /// <summary>
        /// 370281****	Tax code
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// Số 30 Hoàng Hoa Thám...	Tax address
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// CONG TY TNHH ABC MEDIA	Tax company
        /// </summary>
        public string company_name { get; set; }
    }
}
