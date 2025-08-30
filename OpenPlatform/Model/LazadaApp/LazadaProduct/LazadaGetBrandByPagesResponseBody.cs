using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct
{
    public class LazadaGetBrandByPagesResponseBody : CommonLazadaResponseHTTP
    {
        public GetBrandByPagesData data { get; set; }
    }
    public class GetBrandByPagesData
    {
        // enable total or not (no use)
        public bool enable_total { get; set; }

        public int start_row { get; set; }

        public int page_index { get; set; }

        // data module
        public List<LazadaBrandModule> module { get; set; }

        // total page(no use)
        public int total_page { get; set; }

        // page size
        public int page_size { get; set; }

        // total number of record
        public int total_record { get; set; }
    }

    public class LazadaBrandModule
    {
        // The actual name of the brand.
        public string name { get; set; }

        // A unique string identifier for the brand across different systems. For example: ADIDAS, NIKE, APPLE.
        public string global_identifier { get; set; }

        // The English name of the brand.
        public string name_en { get; set; }

        // brand id
        public long brand_id { get; set; }
    }
}