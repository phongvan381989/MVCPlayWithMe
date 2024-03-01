using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    public class TikiPaymentInfo
    {
        /// <summary>
        /// cod	Payment method
        /// Values: cod, checkmo, momo, pay123, cybersource
        /// </summary>
        public string method { get; set; }

        /// <summary>
        /// false	The order is prepaid
        /// </summary>
        public bool is_prepaid { get; set; }

        /// <summary>
        /// success	Status of the payment
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// Thanh toán tiền mặt khi nhận hàng	Description of the payment method
        /// </summary>
        public string description { get; set; }
    }
}
