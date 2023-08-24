using MVCPlayWithMe.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class Item
    {
        public int id { get; set; }
        public string name { get; set; }

        /// <summary>
        /// 0: Bán bình thường, 1: Ngừng kinh doanh, 2: Hết hàng
        /// </summary>
        public int status { get; set; }

        public string detail { get; set; }

        public int quota { get; set; }

        public List<Model> models { get; set; }

        public Item()
        {
            id = -1;
            quota = Common.quota;
            models = new List<Model>();
        }

        public Item(string inName,
            int inStatus,
            int inQuota,
            string inDetail
            )
        {
            id = -1;
            name = inName;
            status = inStatus;
            quota = inQuota;
            detail = inDetail;
            models = new List<Model>();
        }
    }
}