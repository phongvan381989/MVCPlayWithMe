using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeGetItemBaseInfoItemTaxInfo
    {
        /// <summary>
        /// value shuold be one of NO_INVOICES VAT_MARGIN_SCHEME_INVOICES VAT_INVOICES NON_VAT_INVOICES and if value is NON_VAT_INVOICE vat_rate should be null (Only for PL region)
        /// </summary>
        public string invoice_option { get; set; }

        /// <summary>
        /// value should be one of 0% 5% 8% 23% NO_VAT_RATE(Only for PL region)
        /// </summary>
        public string vat_rate { get; set; }

        /// <summary>
        /// HS Code. (Only for IN region)
        /// </summary>
        public string hs_code { get; set; }

        /// <summary>
        /// Tax Code. (Only for IN region)
        /// </summary>
        public string tax_code { get; set; }
    }
}
