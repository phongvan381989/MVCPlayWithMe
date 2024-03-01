using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    public class TikiDeliveryConfirmation
    {
        /// <summary>
        /// true	Delivery has been confirmed by Tiki, shipping partner or seller
        /// </summary>
        public bool delivery_confirmed { get; set; }

        /// <summary>
        /// 2020-08-10 15:37:32	When delivery has been confirmed
        /// </summary>
        public DateTime delivery_confirmed_at { get; set; }

        /// <summary>
        /// false	Delivery has been confirmed by customer
        /// </summary>
        public bool delivery_confirmed_by_customer { get; set; }

        /// <summary>
        /// 	When the delivery has been confirmed by customer
        /// </summary>
        public DateTime delivery_confirmed_by_customer_at { get; set; }

        /// <summary>
        /// Order TKB.RRSIIRQ3 BookingID 4283 has been delivered	Delivery note
        /// </summary>
        public string delivery_note { get; set; }

        public List<Object> delivery_confirmation {get; set;}
    }
}
