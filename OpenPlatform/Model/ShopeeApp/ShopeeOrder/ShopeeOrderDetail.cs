using System;
using System.Collections.Generic;
namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder
{
    public class ShopeeOrderDetail : GeneralOrder
    {
        public ShopeeOrderDetail() : base(EnumCommerceTypeDeTail.SHOPEE_ITEM)
        {
        }

        /// <summary>
        /// Return by default. Shopee's unique identifier for an order.
        /// </summary>
        public string order_sn { get; set; }

        /// <summary>
        /// Return by default. The two-digit code representing the region where the order was made.
        /// </summary>
        public string region { get; set; }

        /// <summary>
        /// Return by default. The three-digit code representing the currency unit for which the order was paid.
        /// </summary>
        public string currency { get; set; }

        /// <summary>
        /// Return by default. This value indicates whether the order was a COD (cash on delivery) order.
        /// </summary>
        public Boolean cod { get; set; }

        /// <summary>
        /// The total amount paid by the buyer for the order. This amount includes the total sale price of items, shipping cost beared by buyer; and offset by Shopee promotions if applicable. This value will only return after the buyer has completed payment for the order.
        /// </summary>
        public float total_amount { get; set; }

        /// <summary>
        /// Return by default. Enumerated type that defines the current status of the order.
        /// </summary>
        public string order_status { get; set; }

        /// <summary>
        /// The logistics service provider that the buyer selected for the order to deliver items.
        /// </summary>
        public string shipping_carrier { get; set; }

        /// <summary>
        /// The payment method that the buyer selected to pay for the order.
        /// </summary>
        public string payment_method { get; set; }

        /// <summary>
        /// The estimated shipping fee is an estimation calculated by Shopee based on specific logistics courier's standard.
        /// </summary>
        public float estimated_shipping_fee { get; set; }

        /// <summary>
        /// Return by default. Message to seller.
        /// </summary>
        public string message_to_seller { get; set; }

        /// <summary>
        /// Return by default. Timestamp that indicates the date and time that the order was created.
        /// </summary>
        public long create_time { get; set; }

        /// <summary>
        /// Return by default. Timestamp that indicates the last time that there was a change in value of order, such as order status changed from 'Paid' to 'Completed'.
        /// </summary>
        public long update_time { get; set; }

        /// <summary>
        /// Return by default. Shipping preparation time set by the seller when listing item on Shopee.
        /// </summary>
        public int days_to_ship { get; set; }

        /// <summary>
        /// Return by default. The deadline to ship out the parcel
        /// </summary>
        public int ship_by_date { get; set; }

        /// <summary>
        /// The user id of buyer of this order
        /// </summary>
        public long buyer_user_id { get; set; }

        /// <summary>
        /// The name of buyer
        /// </summary>
        public string buyer_username { get; set; }

        /// <summary>
        /// This object contains detailed breakdown for the recipient address.
        /// </summary>
        public ShopeeGetOrderDetailRecipientAddress recipient_address { get; set; }

        /// <summary>
        /// The actual shipping fee of the order if available from external logistics partners.
        /// </summary>
        public float actual_shipping_fee { get; set; }

        /// <summary>
        /// Only work for cross-border order.This value indicates whether the order contains goods that are required to declare at customs. "T" means true and it will mark as "T" on the shipping label; "F" means false and it will mark as "P" on the shipping label. This value is accurate ONLY AFTER the order trackingNo is generated, please capture this value AFTER your retrieve the trackingNo.
        /// </summary>
        public Boolean goods_to_declare { get; set; }

        /// <summary>
        /// The note seller made for own reference.
        /// </summary>
        public string note { get; set; }

        /// <summary>
        /// Update time for the note.
        /// </summary>
        public long note_update_time { get; set; }

        /// <summary>
        /// This object contains the detailed breakdown for the result of this API call.
        /// </summary>
        public List<ShopeeGetOrderDetailItem> item_list { get; set; }

        /// <summary>
        /// The time when the order status is updated from UNPAID to PAID. This value is NULL when order is not paid yet.
        /// </summary>
        public long pay_time { get; set; }

        /// <summary>
        /// For Indonesia orders only. The name of the dropshipper.
        /// </summary>
        public string dropshipper { get; set; }

        /// <summary>
        /// The phone number of dropshipper, could be empty.
        /// </summary>
        public string dropshipper_phone { get; set; }

        /// <summary>
        /// To indicate whether this order is split to fullfil order(forder) level. Call GetForderInfo if it's "true"
        /// </summary>
        public Boolean split_up { get; set; }

        /// <summary>
        /// Cancel reason from buyer, could be empty.
        /// </summary>
        public string buyer_cancel_reason { get; set; }

        /// <summary>
        /// Could be one of buyer, seller, system or Ops.
        /// </summary>
        public string cancel_by { get; set; }

        /// <summary>
        /// Use this field to get reason for buyer, seller, and system cancellation.
        /// </summary>
        public string cancel_reason { get; set; }

        /// <summary>
        /// Use this filed to judge whether the actual_shipping_fee is confirmed.
        /// </summary>
        public Boolean actual_shipping_fee_confirmed { get; set; }

        /// <summary>
        /// Buyer's CPF number for taxation and invoice purposes. Only for Brazil order.
        /// </summary>
        public string buyer_cpf_id { get; set; }

        /// <summary>
        /// Use this field to indicate the order is fulfilled by shopee or seller. Applicable values: fulfilled_by_shopee, fulfilled_by_cb_seller, fulfilled_by_local_seller.
        /// </summary>
        public string fulfillment_flag { get; set; }

        /// <summary>
        /// The timestamp when pickup is done.
        /// </summary>
        public long pickup_done_time { get; set; }

        /// <summary>
        /// The list of package under an order
        /// </summary>
        public List<ShopeeGetOrderDetailPackage> package_list { get; set; }

        /// <summary>
        /// The invoice data of the order. pt: Nota Fiscal eletrônica (NF-e) do pedido.
        /// </summary>
        public ShopeeGetOrderDetailInvoiceData invoice_data { get; set; }

        /// <summary>
        /// For non masking order, the logistics service provider that the buyer selected for the order to deliver items. For masking order, the logistics service type that the buyer selected for the order to deliver items.
        /// </summary>
        public string checkout_shipping_carrier { get; set; }

        /// <summary>
        /// Shopee charges the reverse shipping fee for the returned order.The value of this field will be non-negative.
        /// </summary>
        public float reverse_shipping_fee { get; set; }

        public string booking_sn { get; set; }

        // Đơn ở trạng thái: UNPAID, READY_TO_SHIP => chưa được sàn sinh mã đơn.
        // Nhà bán chưa xác nhận đơn, khách hủy (trạng thái sẽ là CANCELLED) => chưa được sinh mã đơn
        // Ngược lại đã được sinh mã đơn.
        public Boolean WasBornTrackingNumber()
        {
            if(order_status != "UNPAID" && order_status != "READY_TO_SHIP" && order_status != "CANCELLED")
            {
                return true;
            }

            return false;
        }
    }
}
