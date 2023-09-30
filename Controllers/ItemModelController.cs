using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPlayWithMe.Controllers
{
    public class ItemModelController : BasicController
    {
        public ItemModelMySql sqler;
        public ProductMySql productSqler;
        
        public ItemModelController()
        {
            sqler = new ItemModelMySql();
            productSqler = new ProductMySql();
        }
        // GET: ItemModel
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }

            ViewDataGetListItemName();
            ViewDataGetListProductName();
            ViewDataGetListCombo();

            return View();
        }

        public ActionResult Search()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">item id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateDelete(int id)
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }

            ViewDataGetListItemName();
            ViewDataGetListProductName();
            ViewDataGetListCombo();

            return View();
        }

        public string AddItem(string name, int status, int quota, string detail)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.authenFailMessage));
            }

            Item it = new Item(name, status, quota, detail);

            MySqlResultState result = sqler.AddItem(it);
            // Lấy id của sản phẩm vừa thêm mới thành công
            result.myAnything = sqler.GetMaxItemId();

            return JsonConvert.SerializeObject(result);
        }

        [HttpGet]
        public string UpModelNoName(int itemId, string listProIdMapping, int status, int quota)
        {
            MySqlResultState result = null;
            int modelId = -1;
            // Lưu vào db
            Model model = new Model(modelId, itemId, "", 0, 0, status, quota, 0);
            model.mapping = Common.ConvertJsonArrayToListInt(listProIdMapping);
            result = sqler.AddModel(model);
            modelId = sqler.GetMaxModelId();
            result.myAnything = modelId;
            model.id = modelId;

            sqler.AddMapping(model);
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Tương tự hàm UploadFile tạo mới sản phẩm nhập kho
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public string UploadFileItem(object obj)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.authenFailMessage));
            }

            var itemId =Common.ConvertStringToInt32(Request.Headers["productId"]);

            string path = Common.GetItemMediaFolderPath(itemId);
            if (path == null)
            {
                path = Common.CreateItemMediaFolderPath(itemId);
            }

            return UploadImageVideo(path);
        }

        /// <summary>
        /// Nhận thông tin 1 model.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public string UploadFileModel(object obj)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.authenFailMessage));
            }

            var length = Request.ContentLength;
            var bytes = new byte[length];
            Request.InputStream.Read(bytes, 0, length);


            var modelName = Request.Headers["modelName"];
            var modelId = Common.ConvertStringToInt32(Request.Headers["modelId"]);// Tạo mới modelId = -1
            var exist = Request.Headers["exist"];
            var quota = Common.ConvertStringToInt32(Request.Headers["quota"]);
            var finish = Request.Headers["finish"];
            var itemId = Common.ConvertStringToInt32(Request.Headers["itemId"]);
            var status = Common.ConvertStringToInt32(Request.Headers["status"]);
            var quantity = Common.ConvertStringToInt32(Request.Headers["quantity"]); 
            var imageExtension = Request.Headers["imageExtension"];// phần mở rộng của ảnh không gồm '.'
            var listProIdMapping = Request.Headers["listProIdMapping"];// Mảng id sản phẩm mapping
            MySqlResultState result = null;
            // Lưu vào db
            Model model = new Model(modelId, itemId, modelName, 0, 0, status, quota, quantity);
            if(exist == "true")
            {
                result = sqler.UpdateModel(model);
            }
            else
            {
                result = sqler.AddModel(model);
                modelId = sqler.GetMaxModelId();
                result.myAnything = modelId;

                model.mapping = Common.ConvertJsonArrayToListInt(listProIdMapping);
                model.id = modelId;

                sqler.AddMapping(model);
            }
            if(result.State != EMySqlResultState.OK)
            {
                return JsonConvert.SerializeObject(result);
            }
            string path = Common.GetModelMediaFolderPath(itemId);
            if(path == null)
            {
                path = Common.CreateModelMediaFolderPath(itemId);
            }
            // Lưu ảnh vào thư mục
            if (exist != "true")
            {
                // Tên ảnh lưu có định dạng. modelId.jpg
                var saveToFileLoc = string.Format("{0}{1}",
                                              path,
                                               modelId.ToString() + "." + imageExtension);

                // save the file.
                var fileStream = new FileStream(saveToFileLoc, FileMode.Create, FileAccess.ReadWrite);
                fileStream.Write(bytes, 0, length);
                fileStream.Close();
            }

            //if (finish == "true")
            //{
            //    var listModelId = Request.Headers["listModelId"];
            //    //List<string> lsModelIdStr = JsonConvert.DeserializeObject<List<string>>(listModelId);

            //    // Danh sách modelId mới nhất
            //    List<int> lsModelId = JsonConvert.DeserializeObject<List<int>>(listModelId);
            //    //foreach(var str in lsModelIdStr)
            //    //{
            //    //    lsModelId.Add(Common.ConvertStringToInt32(str));
            //    //}
            //    if (lsModelId.Count() > 0)
            //    {

            //        // Xóa model trong db
            //        List<Model> lsDeletedModel = sqler.DeleteOldModel(lsModelId, itemId);

            //        // Xóa ảnh model tương ứng trong thư mục
            //        foreach (var mod in lsDeletedModel)
            //        {
            //            Common.DeleteImageVideoWithoutExtension(path + mod.id.ToString() + ".jpg");
            //        }
            //    }
            //}

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        ///  TÌm kiếm các sản phẩm trong kho
        /// </summary>
        /// <param name="codeOrBarcode"></param>
        /// <param name="name"></param>
        /// <param name="combo"></param>
        /// <returns></returns>
        [HttpGet]
        public string SearchProduct(string codeOrBarcode, string name, string combo)
        {
            List<Product> ls = productSqler.SearProduct(codeOrBarcode, name, combo);

            return JsonConvert.SerializeObject(ls);
        }

        /// <summary>
        /// Tìm kiếm sản phẩm trên sàn
        /// </summary>
        /// <param name="namePara"></param>
        /// <returns></returns>
        [HttpGet]
        public string SearchItemCount(string namePara)
        {
            // Đếm số sản phẩm trong kết quả tìm kiếm
            int count = 0;
            count = sqler.SearchItemCount(namePara);
            return count.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namePara"></param>
        /// <param name="page"> Tính từ 1. Nên cần -1</param>
        /// <returns></returns>
        [HttpGet]
        public string ChangePage(string namePara, int start, int offset)
        {
            List<Item> lsSearchResult;
            lsSearchResult = sqler.SearchItemChangePage(namePara, start, offset);

            return JsonConvert.SerializeObject(lsSearchResult);
        }
    }
}