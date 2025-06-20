using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category
{
    public class TikiAttributeValueDataPage
    {
        public List<TikiAttributeValue> data { get; set; }

        public TikiPagingOrder paging { get; set; }
    }
}