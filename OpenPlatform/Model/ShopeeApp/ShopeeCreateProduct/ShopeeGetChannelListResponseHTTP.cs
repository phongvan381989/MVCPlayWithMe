using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeCreateProduct
{
    public class ShopeeLogisticWeightLimit
    {
        // The max weight for an item on this logistic channel.If the value is 0 or null, that means there is no limit.
        public float item_max_weight { get; set; }

        // The min weight for an item on this logistic channel. If the value is 0 or null, that means there is no limit.
        public float item_min_weight { get; set; }
    }

    public class ShopeeLogisticSize
    {
        // The identity of size.
        public string size_id { get; set; }

        // The name of size.
        public string name { get; set; }

        // The pre-defined shipping fee for the specific size.
        public float default_price { get; set; }
    }

    public class ShopeeLogisticItemMaxDimension
    {
        // The max height limit.
        public float height { get; set; }

        // The max width limit
        public float width { get; set; }

        // The max length limit.
        public float length { get; set; }

        // The unit for the limit.
        public string unit { get; set; }

        // The sum of the item's dimension
        public float dimension_sum { get; set; }

    }

    public class ShopeeLogisticVolumeLimit
    {
        // The max volume for an item on this logistic channel.If the value is 0 or null, that means there is no limit for the item weight.
        public float item_max_volume { get; set; }

        // The min volume for an item on this logistic channel. If the value is 0 or null, that means there is no limit for the item weight.
        public float item_min_volume { get; set; }
    }

    public class ShopeeLogisticCcapability
    {
        // Indicate If it's a Seller logistics channel, if it's a Seller logistics channel will return true, otherwise it will return false.
        public Boolean seller_logistics { get; set; }
    }

    public class ShopeeLogisticsChannel
    {
        // The identity of logistic channel.
        public int logistics_channel_id { get; set; }

        // The name of logistic channel.
        public string logistics_channel_name { get; set; }

        // This is to indicate whether this logistic channel supports COD
        public string cod_enabled { get; set; }

        // Whether this logistic channel is enabled on shop level.
        public Boolean enabled { get; set; }

        // SIZE_SELECTION
        // SIZE_INPUT
        // FIXED_DEFAULT_PRICE
        // CUSTOM_PRICE
        public string fee_type { get; set; }

        // Only for fee_type is SIZE_SELECTION
        public List<ShopeeLogisticSize> size_list { get; set; }

        // The weight limit for this logistic channel.
        public ShopeeLogisticWeightLimit weight_limit { get; set; }

        // The dimension limit for this logistic channel.
        public ShopeeLogisticItemMaxDimension item_max_dimension { get; set; }

        // The limit of item volume.
        public ShopeeLogisticVolumeLimit volume_limit { get; set; }

        // For checkout channels, this field indicates its corresponding fulfillment channels.
        public string logistics_description { get; set; }

        // Indicates whether the logistic channel is force enabled on Shop Level. If true, sellers cannot close this channel
        public Boolean force_enable { get; set; }

        // Indicate the parent logistic channel ID. If it’s 0, it indicates the channel is a checkout(masked) channel; if it’s not 0, indicate the channel is a fulfillment channel and has a checkout channel(checkout channel’s channel_id equals this mask_channel_id) on top of it. Multiple channels may share the same mask_channel_id.
        public int mask_channel_id { get; set; }

        // Indicate whether the channel is blocked to use seller cover shipping fee function.
        // if the channel does not allow sellers to cover shipping fee, then the block_seller_cover_shipping_fee field will return true, otherwise it will return false.
        public Boolean block_seller_cover_shipping_fee { get; set; }

        // Indicate whether this channel support cross border shipping
        public Boolean support_cross_border { get; set; }

        // Indicate If seller has set the Seller logistics configuration if set will return true, otherwise it will return false or null.
        public Boolean seller_logistic_has_configuration { get; set; }

        // The capability of one logistic channel.
        public ShopeeLogisticCcapability logistics_capability { get; set; }
    }

    public class ShopeeGetChannelListResponse
    {
        // The list of logistics channel.
        public List<ShopeeLogisticsChannel> logistics_channel_list { get; set; }
    }

    public class ShopeeGetChannelListResponseHTTP : CommonResponseHTTP
    {
        public ShopeeGetChannelListResponse response { get; set; }
    }
}