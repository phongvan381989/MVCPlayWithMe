using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    /// <summary>
    /// When a customer places an order and Tiki accepts to process their order, seller needs to confirm
    //    the order as soon as possible:

    // Confirm seller have enough saleable quantity for the products in the order – available confirmation
    // This is necessary for all kinds of operation models except for Fulfillment by Tiki
    // In other words, all Order Item V2 with inventory_type=backorder needs to be confirmed by the seller
    // Confirm seller have done packaging the products for the order – pickup confirmation
    // This is necessary only for Drop Shipping operation model
    /// </summary>
    public class TikiOrderItemConfirmation
    {
        /// <summary>
        /// seller_confirmed	Confirmation status of this Order Item V2
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 2020-05-16 08:33:35	When the latest confirmation happens
        /// </summary>
        public DateTime confirmed_at { get; set; }

        /// <summary>
        /// 2020-05-16 12:00:00	Available confirmation deadline
        /// </summary>
        public DateTime available_confirm_sla { get; set; }

        /// <summary>
        /// 2020-05-16 16:00:00	Pickup confirmation deadly (only for drop shipping)
        /// </summary>
        public DateTime pickup_confirm_sla { get; set; }

        /// <summary>
        /// 2020-05-16 08:33:35	When the latest confirmation happens
        /// </summary>
        public List<TikiOrderItemConfirmationHistory> histories { get; set; }
    }
}
