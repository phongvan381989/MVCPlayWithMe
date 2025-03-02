using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeNotification
{
    public class ViolationItemPush_DeboostDetails_SuggestedCategory
    {
        // Suggested category ID for the item
        public long category_id { get; set; }

        // Suggested category name
        public string category_name { get; set; }
    }
}