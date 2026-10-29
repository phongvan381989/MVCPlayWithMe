using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeNotification
{
    public class ShopeeReturnUpdatesPush : ShopeeBaseNotification
    {
        // Main Push message data
        public ShopeeReturnUpdatesPushData data { get; set; }
    }

    /// <summary>
    /// Main Push message data for Return Updates
    /// </summary>
    public class ShopeeReturnUpdatesPushData
    {
        /// <summary>
        /// Return by default. Shopee's unique serial number identifier for an order.
        /// </summary>
        public string order_sn { get; set; }

        /// <summary>
        /// Return by default. Shopee's unique serial number identifier for a Return Refund request.
        /// </summary>
        public string return_sn { get; set; }

        /// <summary>
        /// Array of updated values containing the fields that were changed
        /// </summary>
        public List<ShopeeReturnUpdatedValue> updated_values { get; set; }
    }

    /// <summary>
    /// Information about a field that was updated in the return request
    /// </summary>
    public class ShopeeReturnUpdatedValue
    {
        /// <summary>
        /// The field whose value is updated.
        /// Possible values:
        /// - return_status
        /// - return_solution
        /// - seller_proof_status
        /// - logistics_status
        /// </summary>
        public string update_field { get; set; }

        /// <summary>
        /// The value before the updates.
        /// Example: "JUDGING"
        /// </summary>
        public string old_value { get; set; }

        /// <summary>
        /// The value after the updates.
        /// Example: "PROCESSING"
        /// </summary>
        public string new_value { get; set; }

        /// <summary>
        /// The time of the updates (Unix timestamp).
        /// Example: 1732796767
        /// </summary>
        public long update_time { get; set; }
    }
}
