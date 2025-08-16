using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    /// <summary>
    /// Phục vụ mapping sản phẩm trên sàn và sản phẩm trong kho
    /// </summary>
    public class CommonForMapping
    {
        /// <summary>
        /// Id item
        /// </summary>
        public long itemId { get; set; }

        /// <summary>
        /// Id model
        /// </summary>
        public long modelId { get; set; }

        // Id của model trong db
        public int dbModelId { get; set; }

        public List<int> lsProductId { get; set; }
        public List<int> lsProductQuantity { get; set; }

        public CommonForMapping()
        {
            lsProductId = new List<int>();
            lsProductQuantity = new List<int>();
        }
    }
}