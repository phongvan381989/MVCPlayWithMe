using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.ItemModel;
using MVCPlayWithMe.Models.ProductModel;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MVCPlayWithMe.Controllers
{
    public class ItemModelController : BasicController
    {
        public ItemModelMySql sqler;

        public ItemModelController()
        {
            sqler = new ItemModelMySql();
        }

        // GET: ItemModel
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Create()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            await ViewDataGetListItemNameAsync();
            //ViewDataGetListProductName();
            //ViewDataGetListCombo();

            return View();
        }

        public async Task<ActionResult> Search()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            return View();
        }

        [HttpPost]
        public async Task<string> GetItemObjectFromId(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }

            return JsonConvert.SerializeObject(await sqler.GetItemFromIdAsync(id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">item id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> UpdateDelete(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }
            //ViewDataGetListProductName();
            //ViewDataGetListCombo();

            return View();
        }

        // Thêm item vào db, không xử lý image/video
        [HttpGet]
        public async Task<string> AddItem(string name, int status, int quota, string detail, int categoryId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }

            Item it = new Item(name, status, quota, detail, categoryId);
            int id = await sqler.AddItemAsync(it);
            MySqlResultState result = new MySqlResultState();
            result.myAnything = id;

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> UpdateItem(int id, string name, int status, int quota, string detail, int categoryId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }

            Item it = new Item(id, name, status, quota, detail, categoryId);
            MySqlResultState result = await sqler.UpdateItemAsync(it);

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fileType">isImage hoặc isVideo</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> DeleteAllFileWithType(int id, string fileType)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }
            string path = Common.GetAbsoluteItemMediaFolderPath(id);
            // Folder được tạo khi có image/video tương ứng
            if (path == null)
            {
                MySqlResultState rs = new MySqlResultState();
                return JsonConvert.SerializeObject(rs);
            }
            return await DeleteAllFileWithTypeBasicAsync(path, id, fileType);
        }

        /// <summary>
        /// Tương tự hàm UploadFile tạo mới sản phẩm nhập kho
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> UploadFileItem(object obj)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }

            var itemId =Common.ConvertStringToInt32(Request.Headers["productId"]);

            string path = Common.GetAbsoluteItemMediaFolderPath(itemId);
            if (path == null)
            {
                path = Common.CreateAbsoluteItemMediaFolderPath(itemId);
            }

            return JsonConvert.SerializeObject(SaveImageVideo(path));
        }

        /// <summary>
        /// Model gồm thông tin lưu db và ảnh đại diện. Thông tin model ở header.
        /// Dùng cho cả tạo mới và cập nhật model
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> UploadFileModel(object obj)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
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
            var discount = Request.Headers["discount"] == null ? 0 : float.Parse(Request.Headers["discount"]);
            var imageExtension = Request.Headers["imageExtension"];// phần mở rộng của ảnh không gồm '.'
            var listProIdMapping = Request.Headers["listProIdMapping"];// Mảng id sản phẩm mapping
            var listQuantityMapping = Request.Headers["listQuantityMapping"];// Mảng quantity sản phẩm mapping
            MySqlResultState result = null;
            // Lưu vào db
            Model model = new Model(modelId, itemId, modelName, quota, discount);
            List<int> mappingOnlyProductId = Common.ConvertJsonArrayToListInt(listProIdMapping);
            List<int> mappingOnlyQuantity = Common.ConvertJsonArrayToListInt(listQuantityMapping);
            for (int i = 0; i < mappingOnlyProductId.Count(); i++)
            {
                model.mapping.Add(new Mapping(mappingOnlyProductId[i], mappingOnlyQuantity[i]));
            }

            if (modelId != -1)
            {
                result = await sqler.UpdateModelAsync(model);
                await sqler.DeleteMappingAsync(model.id);
                await sqler.AddMappingAsync(model);
            }
            else
            {
                result = new MySqlResultState();
                modelId = await sqler.AddModelAsync(model);
                result.myAnything = modelId;
                model.id = modelId;
                await sqler.AddMappingAsync(model);
            }
            if (result.State != EMySqlResultState.OK)
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

                var saveToFileLoc = string.Format("{0}{1}", path, modelId.ToString() + "." + imageExtension);

                var fileStream = new FileStream(saveToFileLoc, FileMode.Create, FileAccess.ReadWrite);
                fileStream.Write(bytes, 0, length);
                fileStream.Close();

                // Model chỉ có ảnh không có video
                // Thêm watermark logo voi bé nhỏ và save ảnh phiên bản 320
                string newsaveToFileLoc = Common.AddWatermark_DeleteOriginalImageFunc(saveToFileLoc);
                Common.ReduceImageSizeTo320AndSave(newsaveToFileLoc);
            }

            return JsonConvert.SerializeObject(result);
        }

        // Xóa model sản phẩm, mapping sản phẩm trong kho tương ứng,
        // mapping sản phẩm trên Shopee, Tiki, Lazada tương ứng
        [HttpGet]
        public async Task<string> DeleteModel(int itemId, int modelId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }

            MySqlResultState result = await sqler.DeleteModelAsync(modelId);

            // Xóa ảnh làm thumbnail
            Common.DeleteImageModelInclude320(itemId, modelId);
            return JsonConvert.SerializeObject(result);
        }

        // Xóa item, model thuộc item, mapping sản phẩm trong kho tương ứng,
        // mapping sản phẩm trên Shopee, Tiki, Lazada tương ứng
        public async Task<string> DeleteItem(int itemId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }

            MySqlResultState result = await sqler.DeleteItemAsync(itemId);

            // Xóa media file
            Common.DeleteMediaItemInclude320(itemId);
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Tìm kiếm sản phẩm trên sàn
        /// </summary>
        /// <param name="namePara"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> SearchItemCount(int publisherId, string namePara)
        {
            ItemModelSearchParameter searchParameter = new ItemModelSearchParameter();
            searchParameter.name = namePara;
            searchParameter.publisherId = publisherId;
            int count = await sqler.SearchItemCountAsync(searchParameter);
            return count.ToString();
        }

        //[HttpGet]
        //public string ChangePage(int publisherId, string namePara, int start, int offset)
        //{
        //    List<Item> lsSearchResult;
        //    ItemModelSearchParameter searchParameter = new ItemModelSearchParameter();
        //    searchParameter.publisherId = publisherId;
        //    searchParameter.name = namePara;
        //    searchParameter.start = start;
        //    searchParameter.offset = offset;
        //    lsSearchResult = sqler.SearchItemPage(searchParameter);

        //    return JsonConvert.SerializeObject(lsSearchResult);
        //}

        // Tương tự hàm ChangePage nhưng trả về toàn bộ kết quả tới người dùng
        [HttpGet]
        public async Task<string> SearchItem(int publisherId, string namePara)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }

            ItemModelSearchParameter searchParameter = new ItemModelSearchParameter();
            searchParameter.publisherId = publisherId;
            searchParameter.name = namePara;
            searchParameter.start = 0;
            searchParameter.offset = 1000000;
            List<Item> lsSearchResult = await sqler.SearchItemPageIncludeMappingAsync(searchParameter);

            return JsonConvert.SerializeObject(lsSearchResult);
        }


        //[HttpGet]
        //public string GetItemFromId(int id)
        //{
        //    return JsonConvert.SerializeObject(sqler.GetItemFromId(id));
        //}

        // Lấy tên và id của item
        [HttpPost]
        public async Task<string> GetListItem()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }

            return JsonConvert.SerializeObject(await sqler.GetListItemNameAsync());
        }

        // Cập nhật chiết khấu
        [HttpPost]
        public async Task<string> UpdateDiscount(int modelId, float discount)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }
            return JsonConvert.SerializeObject(await sqler.UpdateDiscountAsync(modelId, discount));
        }

        // Cập nhật mapping
        [HttpPost]
        public async Task<string> UpdateMapping(int modelId, string listProIdMapping, string listQuantityMapping)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }
            List<int> mappingOnlyProductId = JsonConvert.DeserializeObject<List<int>>(listProIdMapping);
            List<int> mappingOnlyQuantity = JsonConvert.DeserializeObject<List<int>>(listQuantityMapping);

            return JsonConvert.SerializeObject(
                await sqler.UpdateMappingAsync(modelId, mappingOnlyProductId, mappingOnlyQuantity));
        }

        // Cập nhật tên model
        [HttpPost]
        public async Task<string> UpdateModelName(int modelId, string name)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }
            return JsonConvert.SerializeObject(await sqler.UpdateModelNameAsync(modelId, name));
        }

        // Cập nhật tên item
        [HttpPost]
        public async Task<string> UpdateItemName(int itemId, string name)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }
            return JsonConvert.SerializeObject(await sqler.UpdateItemNameAsync(itemId, name));
        }

        // Cập nhật chiết khấu cho danh sách item/model
        // listItemId có dạng: 1,2,3
        [HttpPost]
        public async Task<string> UpdateDiscountForListItem(float discount, string listItemId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }
            return JsonConvert.SerializeObject(await sqler.UpdateDiscountForListItemAsync(discount, listItemId));
        }

        // Cập nhật chiết khấu cho danh sách item/model
        // listItemId có dạng: 1,2,3
        [HttpPost]
        public async Task<string> UpdateDiscountForListModelId(float discount, string listModelId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }
            return JsonConvert.SerializeObject(await sqler.UpdateDiscountForListModleIdAsync(discount, listModelId));
        }

        // Cập nhật tên item
        [HttpPost]
        public async Task<string> UpdateItemCategory(int itemId, int categoryId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }
            return JsonConvert.SerializeObject(await sqler.UpdateItemCategoryAsync(itemId, categoryId));
        }

        // Lấy được item id từ model id
        [HttpPost]
        public async Task<string> GetVBNItemIdFromModelId(int modelId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }

            return JsonConvert.SerializeObject(await sqler.GetVBNItemIdFromModelIdAsync(modelId));
        }
    }
}