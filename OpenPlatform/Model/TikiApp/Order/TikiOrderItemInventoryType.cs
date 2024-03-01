using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    public class TikiOrderItemInventoryType
    {
        public enum EnumOrderItemInventoryType
        {
            backorder,
            instock,
            preorder
        }
        static public string[] ArrayStringOrderItemInventoryType =
        {
            "backorder",
            "instock",
            "preorder"
        };
    }
}
