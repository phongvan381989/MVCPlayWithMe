using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class Publisher : BasicIdName
    {
        // Chiết khấu so với giá bìa khi nhập sách.
        // Dùng để tham khảo khi tính giá bán thực tế.Giá trị mặc định là 20
        public float discount { get; set; }

        public string detail { get; set; }

        public Publisher(int idInput, string nameInput,
            float discountInput, string detalInput) : base(idInput, nameInput)
        {
            discount = discountInput;
            detail = detalInput;
        }
    }
}