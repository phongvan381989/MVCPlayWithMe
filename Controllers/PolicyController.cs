using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCPlayWithMe.General;

namespace MVCPlayWithMe.Controllers
{
    public class PolicyController : BasicController
    {

        public void ViewDataGetCommonInforOfVoiBeNho()
        {
            ViewData["webAddress"] = "voibenho.com";
            ViewData["httpsWebAddress"] = Common.httpsVoiBeNho;
            ViewData["hotline"] = "083 577 4489";
            ViewData["postAddress"] = "Số 28, Ngõ 3, Khu Tập Thể Đo Lường, Tổ Dân Phố 3A, phường Đông Ngạc, Hà Nội";
            ViewData["emailAddress"] = "playwithmebook@gmail.com";
            ViewData["ceoName"] = "HOÀNG THỊ HUỆ";
            ViewData["businessId"] = "01D-8014432";
            ViewData["inHaNoiFee"] = Common.ConvertIntToVNDFormat(Common.standardShipFeeInHaNoi);
            ViewData["outHaNoiFee"] = Common.ConvertIntToVNDFormat(Common.standardShipFeeOutHaNoi);
        }

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
