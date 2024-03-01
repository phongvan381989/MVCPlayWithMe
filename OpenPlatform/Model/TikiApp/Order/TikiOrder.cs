using System;
using System.Collections.Generic;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    public class TikiOrder : GeneralOrder
    {
        public TikiOrder() : base(EnumCommerceTypeDeTail.TIKI)
        {
        }

        public long id { get; set; }

        /// <summary>
        /// 745467462	Unique order code
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// cross_border	How the order is fulfilled a.k.a the fulfillment model of the order
        /// See order fulfillment type
        /// </summary>
        public string fulfillment_type { get; set; }

        /// <summary>
        /// complete	The status where the order is in the process to customer hands
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 	All Order Item V2s details
        /// </summary>
        public List<TikiOrderItemV2> items { get; set; }

        /// <summary>
        /// 	All the status changes of this order
        /// </summary>
        public List<TikiStatusHistory> status_histories { get; set; }

        public bool is_virtual { get; set; }

        public List<TikiSibling> siblings { get; set; }

        public Int32 tikixu_point_earning { get; set; }

        public bool is_flower_gift { get; set; }

        public bool dropship_already { get; set; }

        /// <summary>
        /// 2020-08-10 18:50:17	When the order is created
        /// </summary>
        public DateTime created_at { get; set; }

        public List<Object> shipment_status_histories { get; set; }

        public TikiBillingAddress billing_address { get; set; }

        public string main_substate_text_en { get; set; }

        public string type { get; set; }

        public bool is_parent { get; set; }

        public List<Object> state_histories { get; set; }

        public string inventory_status { get; set; }

        public string platform { get; set; }

        public string linked_code { get; set; }

        /// <summary>
        /// true	If this order has at least 1 Order Item V2 with inventory_type=backorder
        /// </summary>
        public bool has_backorder_items { get; set; }

        public TikiMultisellerConfirmation multiseller_confirmation { get; set; }

        public string original_code { get; set; }

        /// <summary>
        /// 2020-08-18 11:10:02	The latest update time of the order
        /// </summary>
        public DateTime updated_at { get; set; }

        /// <summary>
        /// 	Shipping information of the order: Shipping status, plan, partner, address
        /// </summary>
        public TikiShippingInfo shipping { get; set; }

        public List<Object> children { get; set; }

        public List<Object> shipment_mappings { get; set; }

        public Int32 backend_id { get; set; }

        public string main_substate { get; set; }

        public string parent_code { get; set; }

        /// <summary>
        /// 	Payment information
        /// </summary>
        public TikiPaymentInfo payment { get; set; }

        public bool is_bookcare { get; set; }

        /// <summary>
        /// 896493516	The order code of the parent order
        /// The parent order is splited or modified into this (and others) order
        /// </summary>
        public string relation_code { get; set; }

        /// <summary>
        /// false	Is this order a replacement for a returned product or a servicing product (RMA flow)
        /// </summary>
        public bool is_rma { get; set; }

        /// <summary>
        /// 	Delivery confirmation
        /// </summary>
        public TikiDeliveryConfirmation delivery { get; set; }

        public string main_state { get; set; }

        /// <summary>
        /// 	The Tiki warehouse that fulfills the order
        /// </summary>
        public TikiWarehouseInfo tiki_warehouse { get; set; }

        public List<Object> labels { get; set; }

        public List<string> applied_rule_ids { get; set; }

        public string substate { get; set; }

        /// <summary>
        /// false	If the customer requested to export VAT bill
        /// </summary>
        public bool is_vat_exporting { get; set; }

        public string main_state_text { get; set; }

        public string main_substate_text { get; set; }

        /// <summary>
        /// 	The order invoice data: Total value, discounts, fees of customer and seller
        /// </summary>
        public TikiOrderInvoice invoice { get; set; }

        public string main_state_text_en { get; set; }

        /// <summary>
        /// 	Customer information
        /// </summary>
        public TikiCustomerInfo customer { get; set; }
        ///// <summary>
        ///// 	Installment information
        ///// </summary>
        //public InstallmentInfo installment_info { get; set; }
        ///// <summary>
        ///// 	Cancellation details
        ///// </summary>
        //public CancelInfo cancel_info { get; set; }
        ///// <summary>
        ///// 	RMA information
        ///// </summary>
        //public RMAInfo rma_info { get; set; }
    }
}
