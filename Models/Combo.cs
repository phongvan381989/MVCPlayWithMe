using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.ProductModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class Combo : BasicIdName
    {
        public Combo(int idInput, string nameInput) : base(idInput, nameInput)
        {
            products = new List<Product>();
            imageSrc = new List<string>();
        }

        public Combo(int idInput, string nameInput, string codeInput, Byte statusInput, string detailInput) : base(idInput, nameInput)
        {
            products = new List<Product>();
            imageSrc = new List<string>();
            code = codeInput;
            status = statusInput;
            detail = detailInput;
        }

        public string code { get; set; }

        public Byte status { get; set; }

        public string detail { get; set; }

        // danh sách sản phẩm thuộc combo
        public List<Product> products { get; set; }

        // Ảnh đầu tiên tên 0.* sẽ là ảnh dùng làm avartar
        public List<string> imageSrc { get; set; }
        //public List<string> videoSrc { get; set; }

        public void SetSrcImageVideo()
        {
            imageSrc = Common.GetComboImageSrc(id.ToString());
            //videoSrc = Common.GetProductVideoSrc(id.ToString());
        }
    }
}
