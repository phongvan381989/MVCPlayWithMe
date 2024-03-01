using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeGetItemBaseInfoItemLogisticInfo
    {
        /// <summary>
        /// The identity of logistic channel.
        /// </summary>
        public int logistic_id { get; set; }

        /// <summary>
        /// The name of logistic.
        /// </summary>
        public string logistic_name { get; set; }

        /// <summary>
        /// Related to shopee.logistics.GetLogistics result.logistics.enabled only affect current item.
        /// </summary>
        public Boolean enabled { get; set; }

        /// <summary>
        /// Only needed when logistics fee_type = CUSTOM_PRICE.
        /// </summary>
        public float shipping_fee { get; set; }

        ///If specify logistic fee_type is SIZE_SELECTION size_id is required.
        public long size_id { get; set; }

        /// <summary>
        /// when seller chooses this option, the shipping fee of this channel on item will be set to 0. Default value is False.
        /// </summary>
        public Boolean is_free { get; set; }

        /// <summary>
        /// Estimated shipping fee calculated by weight. Don't exist if channel is no-integrated.s
        /// </summary>
        public int estimated_shipping_fee { get; set; }
    }
}
