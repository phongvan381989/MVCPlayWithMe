using System.Collections.Generic;

namespace MVCPlayWithMe.OpenPlatform.Model.TikTokApp.TikTokOrder
{
    /// <summary>
    /// TikTok Shop Order Status Constants
    /// These values match exactly with TikTok Shop API order_status field
    /// </summary>
    public static class TikTokOrderStatus
    {
        /// <summary>
        /// The order has been placed, but payment has not been completed.
        /// </summary>
        public const string UNPAID = "UNPAID";

        /// <summary>
        /// The order has been accepted and is awaiting fulfillment.
        /// The buyer may still cancel without the seller's approval.
        /// If order_type=PRE_ORDER, the product is still awaiting release so payment will only be
        /// authorized 1 day before the release, but the seller should start preparing for the release.
        /// </summary>
        public const string ON_HOLD = "ON_HOLD";

        /// <summary>
        /// The order is ready to be shipped, but no items have been shipped yet.
        /// </summary>
        public const string AWAITING_SHIPMENT = "AWAITING_SHIPMENT";

        /// <summary>
        /// Some items in the order have been shipped, but not all.
        /// </summary>
        public const string PARTIALLY_SHIPPING = "PARTIALLY_SHIPPING";

        /// <summary>
        /// Shipping has been arranged, but the package is waiting to be collected by the carrier.
        /// </summary>
        public const string AWAITING_COLLECTION = "AWAITING_COLLECTION";

        /// <summary>
        /// The package has been collected by the carrier and delivery is in progress.
        /// </summary>
        public const string IN_TRANSIT = "IN_TRANSIT";

        /// <summary>
        /// The package has been delivered to the buyer.
        /// </summary>
        public const string DELIVERED = "DELIVERED";

        /// <summary>
        /// The order has been completed, and no further returns or refunds are allowed.
        /// </summary>
        public const string COMPLETED = "COMPLETED";

        /// <summary>
        /// The order has been cancelled.
        /// </summary>
        public const string CANCELLED = "CANCELLED";

        /// <summary>
        /// All valid order statuses
        /// </summary>
        public static readonly string[] AllStatuses = new string[]
        {
            UNPAID,
            ON_HOLD,
            AWAITING_SHIPMENT,
            PARTIALLY_SHIPPING,
            AWAITING_COLLECTION,
            IN_TRANSIT,
            DELIVERED,
            COMPLETED,
            CANCELLED
        };

        /// <summary>
        /// Check if a status string is valid
        /// </summary>
        public static bool IsValidStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
                return false;

            foreach (var s in AllStatuses)
            {
                if (s == status)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Get Vietnamese description for order status
        /// </summary>
        public static string GetVietnameseDescription(string status)
        {
            switch (status)
            {
                case UNPAID:
                    return "Chưa thanh toán";
                case ON_HOLD:
                    return "Đang chờ xử lý";
                case AWAITING_SHIPMENT:
                    return "Chờ giao hàng";
                case PARTIALLY_SHIPPING:
                    return "Đang giao một phần";
                case AWAITING_COLLECTION:
                    return "Chờ lấy hàng";
                case IN_TRANSIT:
                    return "Đang vận chuyển";
                case DELIVERED:
                    return "Đã giao hàng";
                case COMPLETED:
                    return "Hoàn thành";
                case CANCELLED:
                    return "Đã hủy";
                default:
                    return status;
            }
        }

        /// <summary>
        /// Statuses that can be fulfilled (shipped)
        /// </summary>
        public static readonly string[] FulfillableStatuses = new string[]
        {
            AWAITING_SHIPMENT
        };

        /// <summary>
        /// Statuses that indicate order is in shipping/delivery process
        /// </summary>
        public static readonly string[] ShippingStatuses = new string[]
        {
            PARTIALLY_SHIPPING,
            AWAITING_COLLECTION,
            IN_TRANSIT
        };

        /// <summary>
        /// Statuses that indicate order is finalized (cannot be changed)
        /// </summary>
        public static readonly string[] FinalStatuses = new string[]
        {
            COMPLETED,
            CANCELLED
        };

        /// <summary>
        /// Check if order can be cancelled
        /// </summary>
        public static bool CanBeCancelled(string status)
        {
            return status == UNPAID || status == ON_HOLD || status == AWAITING_SHIPMENT;
        }

        /// <summary>
        /// Check if order can be shipped
        /// </summary>
        public static bool CanBeShipped(string status)
        {
            return status == AWAITING_SHIPMENT;
        }

        /// <summary>
        /// Check if order is in final state (completed or cancelled)
        /// </summary>
        public static bool IsFinal(string status)
        {
            return status == COMPLETED || status == CANCELLED;
        }

        /// <summary>
        /// Check if order needs action from seller
        /// </summary>
        public static bool NeedsAction(string status)
        {
            return status == AWAITING_SHIPMENT || status == AWAITING_COLLECTION;
        }
    }
}
