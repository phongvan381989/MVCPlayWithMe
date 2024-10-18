using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPlayWithMe.Controllers
{
    public class PolicyController : BasicController
    {
        // GET: Policy
        public ActionResult InforCustomerPolicy()
        {
            ViewDataGetCommonInforOfVoiBeNho();
            ViewData["title"] = "Chính sách bảo mật thông tin";
            return View();
        }

        public ActionResult GuaranteePolicy()
        {
            ViewDataGetCommonInforOfVoiBeNho();
            ViewData["title"] = "Chính sách bảo hành";
            return View();
        }

        public ActionResult ReturnRefundPolicy()
        {
            ViewDataGetCommonInforOfVoiBeNho();
            ViewData["title"] = "Chính sách đổi trả, hoàn tiền";
            return View();
        }

        public ActionResult CheckPolicy()
        {
            ViewDataGetCommonInforOfVoiBeNho();
            ViewData["title"] = "Chính sách kiểm hàng";
            return View();
        }

        public ActionResult ComplaintPolicy()
        {
            ViewDataGetCommonInforOfVoiBeNho();
            ViewData["title"] = "Chính sách xử lý khiếu nại";
            return View();
        }

        public ActionResult PayPolicy()
        {
            ViewDataGetCommonInforOfVoiBeNho();
            ViewData["title"] = "Chính sách thanh toán";
            return View();
        }

        public ActionResult OrderPolicy()
        {
            ViewDataGetCommonInforOfVoiBeNho();
            //ViewData["title"] = "Chính sách bảo hành";
            return View();
        }

        public ActionResult ObligationPolicy()
        {
            ViewDataGetCommonInforOfVoiBeNho();
            ViewData["title"] = "Nghĩa vụ các bên trong giao dịch";
            return View();
        }

        public ActionResult TransportPolicy()
        {
            ViewDataGetCommonInforOfVoiBeNho();
            ViewData["title"] = "Chính sách vận chuyển";
            return View();
        }

        public ActionResult IntroducePolicy()
        {
            ViewDataGetCommonInforOfVoiBeNho();
            ViewData["title"] = "Giới thiệu tiệm sách và đồ chơi voi bé nhỏ";
            return View();
        }
    }
}
