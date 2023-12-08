using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.General
{
    // Trả lại trạng thái kết quả
    // Nếu status: true; message = "Ok"
    // Ngược lại message = thông báo
    public class CommonResult
    {
        public Boolean status { get; set; }
        public string message { get; set; }

        public CommonResult()
        {
            status = true;
            message = "Ok";
        }

        public CommonResult(string mes)
        {
            status = false;
            message = mes;
        }
    }
}