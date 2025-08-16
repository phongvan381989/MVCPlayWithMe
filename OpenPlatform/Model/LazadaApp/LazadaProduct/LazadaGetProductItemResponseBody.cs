using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct
{
    class LazadaGetProductItemResponseBody : CommonLazadaResponseHTTP
    {
        public LazadaProduct data { get; set; }
    }
}
