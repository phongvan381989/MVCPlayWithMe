using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeGetItemBaseInfoItemComplaintPolicy
    {
        /// <summary>
        /// Time for a warranty claim.Value should be in one of ONE_YEAR TWO_YEARS OVER_TWO_YEARS.
        /// </summary>
        public string warranty_time { get; set; }

        /// <summary>
        /// Whether to exclude warranty complaints for entrepreneurs.If True means "I exclude warranty complaints for entrepreneur"
        /// </summary>
        public Boolean exclude_entrepreneur_warranty { get; set; }

        /// <summary>
        /// Address ID for complaint.Fetch detailed addresses using v2.logistics.get_address_list.
        /// </summary>
        public int complaint_address_id { get; set; }

        /// <summary>
        /// Additional information for complaint policy
        /// </summary>
        public string additional_information { get; set; }
    }
}
