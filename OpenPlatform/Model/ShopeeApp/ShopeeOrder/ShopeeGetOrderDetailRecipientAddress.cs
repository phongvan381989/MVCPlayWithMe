using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder
{
    public class ShopeeGetOrderDetailRecipientAddress
    {
        /// <summary>
        /// Recipient's name for the address.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Recipient's phone number input when order was placed.
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// The town of the recipient's address. Whether there is a town will depend on the region and/or country.
        /// </summary>
        public string town { get; set; }

        /// <summary>
        /// The district of the recipient's address. Whether there is a district will depend on the region and/or country.
        /// </summary>
        public string district { get; set; }

        /// <summary>
        /// The city of the recipient's address. Whether there is a city will depend on the region and/or country.
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// The state/province of the recipient's address. Whether there is a state/province will depend on the region and/or country
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// The two-digit code representing the region of the Recipient.
        /// </summary>
        public string region { get; set; }

        /// <summary>
        /// Recipient's postal code.
        /// </summary>
        public string zipcode { get; set; }

        /// <summary>
        /// The full address of the recipient, including country, state, even street, and etc.
        /// </summary>
        public string full_address { get; set; }
    }
}
