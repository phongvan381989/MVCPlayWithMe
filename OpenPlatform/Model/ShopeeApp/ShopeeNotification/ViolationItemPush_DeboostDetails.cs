using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeNotification
{
    public class ViolationItemPush_DeboostDetails
    {
        // Violation types defined by Shopee
        // Applicable values: 
        //Prohibited Listing
        //Counterfeit and IP Infringement
        //Spam
        //Inappropriate Image
        //Insufficient Information
        //Mall Listing Improvement
        //Other Listing Improvement
        public string violation_type { get; set; }

        // The reason for violation
        public string violation_reason { get; set; }

        // Shopee provides you with suggestions for modifying items
        public string suggestion { get; set; }

        public List<ViolationItemPush_DeboostDetails_SuggestedCategory> suggested_category { get; set; }

        // Action required deadline (nullable if no deadline)
        public long? fix_deadline_time { get; set; }

        // Latest update time
        public long update_time { get; set; }
    }
}