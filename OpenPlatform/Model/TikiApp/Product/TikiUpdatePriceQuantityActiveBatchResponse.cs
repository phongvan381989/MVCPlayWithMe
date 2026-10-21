using Newtonsoft.Json;
using System.Collections.Generic;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product
{
    /// <summary>
    /// Response từ Tiki API khi cập nhật giá/số lượng/trạng thái sản phẩm
    /// </summary>
    public class TikiUpdatePriceQuantityActiveBatchResponse
    {
        public List<TikiUpdatePriceQuantityActiveResultItem> data { get; set; }

        public TikiUpdatePriceQuantityActiveBatchResponse()
        {
            data = new List<TikiUpdatePriceQuantityActiveResultItem>();
        }
    }

    /// <summary>
    /// Kết quả cập nhật cho 1 sản phẩm
    /// </summary>
    public class TikiUpdatePriceQuantityActiveResultItem
    {
        public bool is_success { get; set; }
        public int product_id { get; set; }
        public string original_sku { get; set; }
        public string code { get; set; }
        public string message { get; set; }
    }
}
