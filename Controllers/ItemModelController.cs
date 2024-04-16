using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.ItemModel;
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
            //ViewDataGetListProductName();
            //ViewDataGetListCombo();

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

        [HttpPost]
        public string GetItemObjectFromId(int id)
        {
            return JsonConvert.SerializeObject(sqler.GetItemFromId(id));
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
            //ViewDataGetListProductName();
            //ViewDataGetListCombo();

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

        [HttpPost]
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
        [HttpPost]
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

            return SaveImageVideo(path);
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


            var modelName = HttpUtility.UrlDecode(Request.Headers["encodeModelName"]);
            var modelId = Common.ConvertStringToInt32(Request.Headers["modelId"]);// Tạo mới modelId = -1
            // exist chỉ ảnh đại diện model đã tồn tại trên server
            var exist = Request.Headers["exist"];
            var quota = Common.ConvertStringToInt32(Request.Headers["quota"]);
            var itemId = Common.ConvertStringToInt32(Request.Headers["itemId"]);
            var discount = Common.ConvertStringToInt32(Request.Headers["discount"]); 
            var imageExtension = Request.Headers["imageExtension"];// phần mở rộng của ảnh không gồm '.'
            var listProIdMapping = Request.Headers["listProIdMapping"];// Mảng id sản phẩm mapping
            var listQuantityMapping = Request.Headers["listQuantityMapping"];// Mảng quantity sản phẩm mapping
            MySqlResultState result = null;
            // Lưu vào db
            Model model = new Model(modelId, itemId, modelName, quota, discount);
            List<int> mappingOnlyProductId  = Common.ConvertJsonArrayToListInt(listProIdMapping);
            List<int> mappingOnlyQuantity = Common.ConvertJsonArrayToListInt(listQuantityMapping);
            for(int i = 0; i < mappingOnlyProductId.Count(); i++)
            {
                model.mapping.Add(new Mapping(mappingOnlyProductId[i], mappingOnlyQuantity[i]));
            }

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

                // Model chỉ có ảnh không có video
                // Thêm watermark logo voi bé nhỏ và save ảnh phiên bản 320
                string newsaveToFileLoc = Common.AddWatermark_DeleteOriginalImageFunc(saveToFileLoc);
                Common.ReduceImageSizeAndSave(newsaveToFileLoc);
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
                    Common.DeleteNormalAnd320Image(files[0]);
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
            ProductSearchParameter searchParameter = new ProductSearchParameter();
            searchParameter.codeOrBarcode = codeOrBarcode;
            searchParameter.name = name;
            searchParameter.combo = combo;
            searchParameter.start = 0;
            searchParameter.offset = 1000000;// Lớn để lấy tất cả kết quả
            List<Product> ls = productSqler.SearchProductChangePage(searchParameter);
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
            ItemModelSearchParameter searchParameter = new ItemModelSearchParameter();
            searchParameter.name = namePara;
            int count = 0;
            count = sqler.SearchItemCount(searchParameter);
            return count.ToString();
        }

        [HttpGet]
        public string ChangePage(string namePara, int start, int offset)
        {
            List<Item> lsSearchResult;
            ItemModelSearchParameter searchParameter = new ItemModelSearchParameter();
            searchParameter.name = namePara;
            searchParameter.start = start;
            searchParameter.offset = offset;
            lsSearchResult = sqler.SearchItemChangePage(searchParameter);

            return JsonConvert.SerializeObject(lsSearchResult);
        }

        //[HttpGet]
        //public string GetItemFromId(int id)
        //{
        //    return JsonConvert.SerializeObject(sqler.GetItemFromId(id));
        //}

        // Lấy tên và id của item
        [HttpPost]
        public string GetListItem()
        {
            return JsonConvert.SerializeObject(sqler.GetListItemName());
        }
    }
}