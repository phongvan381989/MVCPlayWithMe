using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using System.Web.Mvc;
using System.Collections.Generic;
using System.IO;

namespace MVCPlayWithMe.Controllers
{
    public class BasicController : Controller
    {
        public Administrator AuthentAdministrator()
        {
            CookieResultState cookieResult = Cookie.SetAndGetUserIdCookie(HttpContext);

            /// Check cookie đã được lưu trong db
            return Cookie.GetAdministratorFromCookieUId(cookieResult);
        }

        /// <summary>
        /// Nơi đến khi xác thực thất bại hoặc logout
        /// </summary>
        /// <returns></returns>
        public ActionResult AuthenticationFail()
        {
            return View("~/Views/Administrator/Login.cshtml");
        }

        public void ViewDataGetListPublisher()
        {
            PublisherMySql sqlPubliser = new PublisherMySql();
            List<Publisher> ls = sqlPubliser.GetListPublisher();
            ViewData["lsPublisher"] = ls;
        }

        public void ViewDataGetListCombo()
        {
            ComboMySql sqler = new ComboMySql();
            List<Combo> ls = sqler.GetListCombo();
            ViewData["lsCombo"] = ls;
        }

        public void ViewDataGetListCategory()
        {
            CategoryMySql sqler = new CategoryMySql();
            List<Category> ls = sqler.GetListCategory();
            ViewData["lsCategory"] = ls;
        }
        
        public void ViewDataGetListProductName()
        {
            ProductMySql sqler = new ProductMySql();
            List<ProductIdName> ls = sqler.GetListParent();
            ViewData["lsProductName"] = ls;
        }

        public void ViewDataGetListPublishingCompany()
        {
            ProductMySql sqler = new ProductMySql();
            List<string> ls = sqler.GetListPublishingCompany();
            ViewData["lsPublishingCompany"] = ls;
        }

        public void ViewDataGetListAuthor()
        {
            ProductMySql sqler = new ProductMySql();
            List<string> listAuthor = sqler.GetListAuthor();
            ViewData["lsAuthor"] = listAuthor;
        }

        public void ViewDataGetListTranslator()
        {
            ProductMySql sqler = new ProductMySql();
            List<string> listTranslator = sqler.GetListTranslator();
            ViewData["lsTranslator"] = listTranslator;
        }

        public void ViewDataGetListProductLong()
        {
            ProductMySql sqler = new ProductMySql();
            List<int> lsProductLong = sqler.GetListDifferenceIntValue(1);
            ViewData["lsProductLong"] = lsProductLong;
        }

        public void ViewDataGetListProductWide()
        {
            ProductMySql sqler = new ProductMySql();
            List<int> lsProductWide = sqler.GetListDifferenceIntValue(2);
            ViewData["lsProductWide"] = lsProductWide;
        }

        public void ViewDataGetListProductHigh()
        {
            ProductMySql sqler = new ProductMySql();
            List<int> lsProductHigh = sqler.GetListDifferenceIntValue(3);
            ViewData["lsProductHigh"] = lsProductHigh;
        }

        public void ViewDataGetListProductWeight()
        {
            ProductMySql sqler = new ProductMySql();
            List<int> lsProductWeight = sqler.GetListDifferenceIntValue(4);
            ViewData["lsProductWeight"] = lsProductWeight;
        }

        public void ViewDataGetListMinAge()
        {
            ProductMySql sqler = new ProductMySql();
            List<int> lsMinAge = sqler.GetListDifferenceIntValue(5);
            ViewData["lsMinAge"] = lsMinAge;
        }

        public void ViewDataGetListMaxAge()
        {
            ProductMySql sqler = new ProductMySql();
            List<int> lsMaxAge = sqler.GetListDifferenceIntValue(6);
            ViewData["lsMaxAge"] = lsMaxAge;
        }

        public void ViewDataGetListPublishingTime()
        {
            ProductMySql sqler = new ProductMySql();
            List<int> lsPublishingTime = sqler.GetListDifferenceIntValue(7);
            ViewData["lsPublishingTime"] = lsPublishingTime;
        }
    }
}