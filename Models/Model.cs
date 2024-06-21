using MVCPlayWithMe.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class Model : PriceQuantity
    {
        public int id { get; set; }
        public int itemId { get; set; }
        public string name { get; set; }
        //public int bookCoverPrice { get; set; }
        //public int price { get; set; }
        //public List<int> mappingOnlyProductId { get; set; }
        //public List<int> mappingOnlyQuantity { get; set; }
        public List<Mapping> mapping { get; set; }

        // image source thumbnail của sản phẩm được mapping.
        //public List<string> mappingProductImageSrc { get; set; }
        public string imageSrc { get; set; }

        /// <summary>
        /// 0: Bán bình thường, 1: Ngừng kinh doanh, 2: Hết hàng
        /// </summary>
        public int status { get; set; }
        public int quota { get; set; }

        public Model()
        {
            id = -1;
            quota = Common.quota;
            mapping = new List<Mapping>();
            //mappingProductImageSrc = new List<string>();
        }

        public Model(int inId, int inItemId, string inName,
            int inQuota, int inDiscount)
        {
            id = inId;
            itemId = inItemId;
            name = inName;

            quota = inQuota;
            discount = inDiscount;
            mapping = new List<Mapping>();
            //mappingProductImageSrc = new List<string>();
        }

        public void SetSrcImage()
        {
            imageSrc = Common.GetModelImageSrc(itemId, id);
        }

        //public void SetMappingProductImageSrc()
        //{
        //    mappingProductImageSrc = Common.GetListThumbnailImageSrd(mapping);
        //}

        //// Từ mapping tính được giá bìa model, số lượng tồn kho
        //public void SetPriceFromMappingPriceAndQuantity()
        //{
        //    if (mapping.Count() == 0)
        //        return;

        //    quantity = 0;
        //    bookCoverPrice = 0;
        //    foreach (var map in mapping)
        //    {
        //        bookCoverPrice = bookCoverPrice + map.product.bookCoverPrice;
        //        if (quantity < map.product.quantity)
        //            quantity = map.product.quantity;
        //    }

        //    // Giá sau chiết khấu
        //    price = bookCoverPrice * (100 - discount) / 100;
            
        //    // Giá làm tròn xuống
        //    price = Common.FloorMoney(price);
        //}
    }
}