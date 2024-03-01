using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product
{
    public class TikiUpdateStatus : TikiUpdate
    {
        /// <summary>
        /// enum {1: enabled, 0: disabled}
        /// </summary>
        public int active { get; set; }
        public TikiUpdateStatus(int productId) : base(productId)
        {

        }

        public void UpdateStatus(int intStatus)
        {
            active = intStatus;
        }
    }
}
