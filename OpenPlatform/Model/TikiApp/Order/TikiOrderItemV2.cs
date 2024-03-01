using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    public class TikiOrderItemV2
    {
        /// <summary>
        /// 156453540	Unique Id of the order item
        /// </summary>
        public Int32 id { get; set; }

        /// <summary>
        /// 	Product information at the time of the purchasing
        /// </summary>
        public TikiOrderItemProduct product { get; set; }

        /// <summary>
        /// Seller basic information
        /// </summary>
        public TikiOrderItemSeller seller { get; set; }

        /// <summary>
        /// Confirmation status and SLAs
        /// </summary>
        public TikiOrderItemConfirmation confirmation { get; set; }

        public long parent_item_id { get; set; }

        public Double price { get; set; }

        public Int32 qty { get; set; }

        public DateTime fulfilled_at { get; set; }

        public bool is_virtual { get; set; }

        public bool is_ebook { get; set; }

        public bool is_bookcare { get; set; }

        public bool is_free_gift { get; set; }

        public bool is_fulfilled { get; set; }

        public Int32 backend_id { get; set; }

        public List<string> applied_rule_ids { get; set; }

        /// <summary>
        /// Invoice information: Value, discounts and fees of customer and seller in this item
        /// </summary>
        public TikiOrderItemInvoice invoice { get; set; }

        /// <summary>
        /// Information of the requisitions of the product items into Tiki warehouse (which will be delivered to customer later then)
        /// </summary>
        public TikiInventoryRequisition inventory_requisition { get; set; }

        /// <summary>
        /// Information of the withdrawals of the product items from Tiki warehouse
        /// </summary>
        public List<TikiInventoryWithdrawal> inventory_withdrawals { get; set; }

        public string seller_inventory_id { get; set; }

        public string seller_inventory_name { get; set; }

        public TikiSellerIncomeDetail seller_income_detail { get; set; }
    }
}
