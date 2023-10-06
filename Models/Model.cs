﻿using MVCPlayWithMe.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class Model
    {
        public int id { get; set; }
        public int itemId { get; set; }
        public string name { get; set; }
        public int bookCoverPrice { get; set; }
        public int price { get; set; }
        public List<int> mappingOnlyProductId { get; set; }
        public List<Product> mapping { get; set; }

        // image source thumbnail của sản phẩm được mapping.
        //public List<string> mappingProductImageSrc { get; set; }
        public string imageSrc { get; set; }

        /// <summary>
        /// 0: Bán bình thường, 1: Ngừng kinh doanh, 2: Hết hàng
        /// </summary>
        public int status { get; set; }
        public int quota { get; set; }
        public int quantity { get; set; }

        public Model()
        {
            id = -1;
            quota = Common.quota;
            mapping = new List<Product>();
            //mappingProductImageSrc = new List<string>();
        }

        public Model(int inId, int inItemId, string inName, int inBookCoverPrice, int inPrice, int inStatus,
            int inQuota, int inQuantity)
        {
            id = inId;
            itemId = inItemId;
            name = inName;
            bookCoverPrice = inBookCoverPrice;
            price = inPrice;
            status = inStatus;
            quota = inQuota;
            quantity = inQuantity;
            mapping = new List<Product>();
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
    }
}