using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    /// <summary>
    /// The details shipping address. The shipping address has 2 private information `email` and `phone`, these fields are only available
    /// in orders with fulfillment_type of seller_delivery or cross_border.
    /// </summary>
    public class TikiShippingAddress
    {
        /// <summary>
        /// NGUYỄN THỊ Z	Recipient full name
        /// </summary>
        public string full_name { get; set; }

        /// <summary>
        /// 13 Nguyen Duc Canh	Address details
        /// </summary>
        public string street { get; set; }

        /// <summary>
        ///Đình Xuyên	Ward name
        /// </summary>
        public string ward { get; set; }

        /// <summary>
        /// VN034005007	Unique ward code
        /// </summary>
        public string ward_tiki_code { get; set; }

        /// <summary>
        /// Gia Lâm	District name
        /// </summary>
        public string district { get; set; }

        /// <summary>
        /// VN034005	Unique district code
        /// </summary>
        public string district_tiki_code { get; set; }

        /// <summary>
        /// Hà Nội	Region or city name
        /// </summary>
        public string region { get; set; }

        /// <summary>
        /// VN034	Unique region or city code
        /// </summary>
        public string region_tiki_code { get; set; }

        /// <summary>
        /// Việt Nam	Country name
        /// </summary>
        public string country { get; set; }

        /// <summary>
        /// VN	Unique country code
        /// </summary>
        public string country_id { get; set; }

        /// <summary>
        /// example@mail.com	Recipient email
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// 0123111111	Recipient phone number
        /// </summary>
        public string phone { get; set; }
    }
}
