using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeNotification
{
    /// <summary>
    /// Get notified when item status becomes BANNED or SHOPEE_DELETE, or marked as deboost,
    /// including the violation type, violation reason, suggestion and fix deadline time.
    /// </summary>
    public class ViolationItemPush
    {
        // Shopee's unique identifier for an item
        public long item_id { get; set; }

        // Name of the item
        public string item_name { get; set; }

        // Enumerated type that defines the current status of the item
        // Applicable values: NORMAL, BANNED, UNLIST, SELLER_DELETE, SHOPEE_DELETE, REVIEWING
        public string item_status { get; set; }

        // Indicates if the item's search ranking is lowered
        public bool deboost { get; set; }

        public List<ViolationItemPush_ItemStatusDetail> item_status_details { get; set; }

        public List<ViolationItemPush_DeboostDetails> deboost_details { get; set; }
    }
}