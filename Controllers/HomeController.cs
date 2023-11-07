using MVCPlayWithMe.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPlayWithMe.Controllers
{
    public class HomeController : Controller
    {
        public ItemModelMySql itemModelsqler;

        public HomeController()
        {
            itemModelsqler = new ItemModelMySql();
        }
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}

        //// Xóa bỏ item có 1 model bất kỳ chưa được mapping sản phẩm trong kho
        //private void RemoveItemHasNoMapping(List<Item>  listItem)
        //{
        //    bool isNoMapping = false;
        //    int length = listItem.Count();

        //    for(int i = length -1; i >= 0; i--)
        //    {
        //        Item item = listItem[i];
        //        isNoMapping = false;
        //        foreach(var model in item.models)
        //        {
        //            if(model.mapping.Count() == 0)
        //            {
        //                isNoMapping = true;
        //                break;
        //            }
        //        }

        //        if(isNoMapping)
        //        {
        //            listItem.RemoveAt(i);
        //        }
        //    }
        //}

        // GET: AllProduts
        public ActionResult Index()
        {
            //ItemModelSearchParameter searchParameter = new ItemModelSearchParameter();
            //searchParameter.hasMapping = 1;
            // Lấy 20 sản phẩm
            //List<Item> listItem = sqler.SearchItemChangePage(searchParameter);
            //RemoveItemHasNoMapping(listItem);
            return View(/*listItem*/);
        }

        /// <summary>
        /// Tìm kiếm sản phẩm trên sàn
        /// </summary>
        /// <param name="namePara"></param>
        /// <returns></returns>
        [HttpGet]
        public string SearchItemCount(string namePara, int hasMapping)
        {
            // Đếm số sản phẩm trong kết quả tìm kiếm
            ItemModelSearchParameter searchParameter = new ItemModelSearchParameter();
            searchParameter.name = namePara;
            searchParameter.hasMapping = hasMapping;
            int count = 0;
            count = itemModelsqler.SearchItemCount(searchParameter);
            return count.ToString();
        }

        [HttpGet]
        public string ChangePage(string namePara, int hasMapping, int start, int offset)
        {
            List<Item> lsSearchResult;
            ItemModelSearchParameter searchParameter = new ItemModelSearchParameter();
            searchParameter.name = namePara;
            searchParameter.start = start;
            searchParameter.offset = offset;
            searchParameter.hasMapping = hasMapping;
            lsSearchResult = itemModelsqler.SearchItemChangePage(searchParameter);

            return JsonConvert.SerializeObject(lsSearchResult);
        }

        [HttpGet]
        public ActionResult Item(/*int id*/)
        {
            int id = 583;
            ViewData["itemObject"] = JsonConvert.SerializeObject(itemModelsqler.GetItemFromId(id));
            return View();
        }
    }
}