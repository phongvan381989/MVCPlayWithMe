using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product
{
    public class TikiUpdateQuantityResponse
    {
        // Trường hợp ok:
        //   {
        //    "state": "approved"
        //   }

        // Trường hợp không ok:
        //    {
        //    "errors": [
        //        "update error [Warehouse '[15936811]' are not found]"
        //        ],
        //    "meta": {
        //        "source": "meepo",
        //        "id": "3408590964569639190"
        //        }
        //    }
        public string state { get; set; }

        public List<string> errors { get; set; }

        public TikiUpdateQuantityMeta meta { get; set; }

    }
}
