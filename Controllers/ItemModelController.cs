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
            ViewData["itemObject"] = JsonConvert.SerializeObject(sqler.GetItemFromId(id));

            return View();
        }

        // Thêm item vào db, không xử lý image/video
        [HttpGet]
        public string AddItem(string name, int status, int quota, string detail)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.authenFailMessage));
            }

            Item it = new Item(name, status, quota, detail);
            int id = sqler.AddItem(it);
            MySqlResultState result = new MySqlResultState();
            // Lấy id của sản phẩm vừa thêm mới thành công
            result.myAnything = id;

            return JsonConvert.SerializeObject(result);
        }

        [HttpGet]
        public string UpdateItem(int id, string name, int status, int quota, string detail)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.authenFailMessage));
            }

            Item it = new Item(id, name, status, quota, detail);

            MySqlResultState result = sqler.UpdateItem(it);

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fileType">isImage hoặc isVideo</param>
        /// <returns></returns>
        //[HttpPost]
        public string DeleteAllFileWithType(int id, string fileType)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.authenFailMessage));
            }
            string path = Common.GetAbsoluteItemMediaFolderPath(id);
            // Folder được tạo khi có image/video tương ứng
            if (path == null)
            {
                MySqlResultState rs = new MySqlResultState();
                return JsonConvert.SerializeObject(rs);
            }
            return DeleteAllFileWithTypeBasic(path, id, fileType);
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

            string path = Common.GetAbsoluteItemMediaFolderPath(itemId);
            if (path == null)
            {
                path = Common.CreateAbsoluteItemMediaFolderPath(itemId);
            }

            return UploadImageVideo(path);
        }

        /// <summary>
        /// Model gồm thông tin lưu db và ảnh đại diện. Thông tin model ở header.
        /// Dùng cho cả tạo mới và cập nhật model
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
            // exist chỉ ảnh đại diện model đã tồn tại trên server
            var exist = Request.Headers["exist"];
            var quota = Common.ConvertStringToInt32(Request.Headers["quota"]);
            var price = Common.ConvertStringToInt32(Request.Headers["price"]);
            var finish = Request.Headers["finish"];
            var itemId = Common.ConvertStringToInt32(Request.Headers["itemId"]);
            var status = Common.ConvertStringToInt32(Request.Headers["status"]);
            var quantity = Common.ConvertStringToInt32(Request.Headers["quantity"]); 
            var imageExtension = Request.Headers["imageExtension"];// phần mở rộng của ảnh không gồm '.'
            var listProIdMapping = Request.Headers["listProIdMapping"];// Mảng id sản phẩm mapping
            MySqlResultState result = null;
            // Lưu vào db
            Model model = new Model(modelId, itemId, modelName, 0, price, status, quota, quantity);
            model.mappingOnlyProductId = Common.ConvertJsonArrayToListInt(listProIdMapping);

            if (modelId != -1)
            {
                result = sqler.UpdateModel(model);

                // Xóa mapping cũ
                sqler.DeleteMapping(model);

                // Tạo mapping mới
                sqler.AddMapping(model);
            }
            else
            {
                result = new MySqlResultState();
                modelId = sqler.AddModel(model);
                result.myAnything = modelId;

                model.id = modelId;

                sqler.AddMapping(model);
            }
            if(result.State != EMySqlResultState.OK)
            {
                return JsonConvert.SerializeObject(result);
            }
            // Lưu ảnh vào thư mục
            if (exist != "true" && length > 0)
            {
                string path = Common.GetAbsoluteModelMediaFolderPath(itemId);
                if (path == null)
                {
                    path = Common.CreateAbsoluteModelMediaFolderPath(itemId);
                }

                // Tên ảnh lưu có định dạng. modelId.jpg
                var saveToFileLoc = string.Format("{0}{1}",
                                              path,
                                               modelId.ToString() + "." + imageExtension);

                // save the file.
                var fileStream = new FileStream(saveToFileLoc, FileMode.Create, FileAccess.ReadWrite);
                fileStream.Write(bytes, 0, length);
                fileStream.Close();
            }

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Xóa model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public string DeleteModel(int id)
        {
            // Xóa ảnh làm thumbnail
            MySqlResultState result = sqler.DeleteModel(id);
            string path = Common.GetAbsoluteModelMediaFolderPath(id);
            if (path != null)
            {
                string[] files = Directory.GetFiles(path, id.ToString() + ".*");
                if (files.Length != 0) // Chỉ có 1 ảnh
                {
                    System.IO.File.Delete(files[0]);
                }
            }
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

        [HttpGet]
        public string ChangePage(string namePara, int start, int offset)
        {
            List<Item> lsSearchResult;
            lsSearchResult = sqler.SearchItemChangePage(namePara, start, offset);

            return JsonConvert.SerializeObject(lsSearchResult);
        }

        [HttpGet]
        public string GetItemFromId(int id)
        {
            return JsonConvert.SerializeObject(sqler.GetItemFromId(id));
        }
    }
}