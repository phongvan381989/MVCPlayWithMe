using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder
{
    public class ShopeeGetOrderDetailInvoiceData
    {
        /// <summary>
        /// The number of the invoice. The number should be 9 digits. pt: número da NF-e.
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// The series number of the invoice.The series number should be 3 digits.pt: série da NF-e.
        /// </summary>
        public string series_number { get; set; }

        /// <summary>
        /// The access key of the invoice. The access key should be 44 digits. pt: chave de acesso da NF-e.
        /// </summary>
        public string access_key { get; set; }

        /// <summary>
        /// The issue date of the invoice. The issue date should be later than the order pay date. pt: data de emissão da NF-e.
        /// </summary>
        public long issue_date { get; set; }

        /// <summary>
        /// The total value of the invoice.pt: valor total da NF-e (R$).
        /// </summary>
        public float total_value { get; set; }

        /// <summary>
        /// The products total value of the invoice.pt: valor total dos produtos(R$) da NF-e.
        /// </summary>
        public float products_total_value { get; set; }

        /// <summary>
        /// The tax code for the invoice. The tax code should be 4 digits. pt: Código Fiscal de Operações e Prestações (CFOP) predominante na NF-e.
        /// </summary>
        public string tax_code { get; set; }
    }
}
