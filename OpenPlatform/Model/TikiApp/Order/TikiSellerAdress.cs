using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    /// <summary>
    /// Seller adress
    /// </summary>
    public class TikiSellerAdress
    {
        public string street { get; set; }

        public string ward { get; set; }

        public string district { get; set; }

        public string region { get; set; }

        public string country { get; set; }
    }
}
