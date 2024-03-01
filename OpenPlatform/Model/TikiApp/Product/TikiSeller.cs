using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product
{
    /// <summary>
    ///  {
    /// "id": 1,
    /// "name": "Tiki Trading",
    /// "code": "TIKI",
    /// "logo": "http://uat.tikicdn.com/ts/seller/0e/9d/fd/61dfb9c92f03fefbb799e829e4b54e67.jpg",
    /// "slug": "tiki",
    /// "status": 1,
    /// "store_id": 1,
    /// "listdata_id": 632463,
    /// "vat_limit_supported": 0,
    /// "is_all_vat_supported": 0
    /// }
/// </summary>
public class TikiSeller
    {
        public Int32 id { get; set; }

        public string name { get; set; }

        public string code { get; set; }

        public string logo { get; set; }

        public string slug { get; set; }

        public Int32 status { get; set; }

        public Int32 store_id { get; set; }

        public Int32 listdata_id { get; set; }

        public Int32 vat_limit_supported { get; set; }

        public Int32 is_all_vat_supported { get; set; }
    }
}
