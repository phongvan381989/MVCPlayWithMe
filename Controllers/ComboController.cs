using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.ProductModel;
using MVCPlayWithMe.Models.SanPhamModel;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Product;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MVCPlayWithMe.Controllers
{
    public class ComboController : BasicController
    {
        // GET: Combo
        public ActionResult Index()
        {
            return View();
        }

        // GET: Combo
        public async Task<ActionResult> Create()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }
            //ViewDataGetListCombo();
            return View();
        }

        public async Task<string> CreateCombo(string name, string code, Byte status)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.INVALID, "Tên không hợp lệ."));
            }

            //if (string.IsNullOrWhiteSpace(code))
            //{
            //    result = new MySqlResultState(EMySqlResultState.INVALID, "Mã không hợp lệ.");
            //    return JsonConvert.SerializeObject(result);
            //}

            MySqlResultState result = await ComboMySql.CreateNewComboAsync(name, code, status);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> DeleteCombo(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = await ComboMySql.DeleteComboAsync(id);

            // Xóa media của combo
            // Tạo đường dẫn Media/Combo/{ComboId}/
            string comboFolderPath = System.Web.HttpContext.Current.Server.MapPath($"{Common.ComboMediaFolderPath}{id}/");

            Common.DeleteMediaFolder(comboFolderPath);

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> UpdateCombo(int id, string name, string code, Byte status, string description = null)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            if (code == null)
            {
                code = string.Empty;
            }

            MySqlResultState result = await ComboMySql.UpdateComboAsync(id, name, code, status, description);
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> LoadCombo()
        {
            StringBuilder sb = new StringBuilder();
            List<Combo> ls = await ComboMySql.GetListComboAsync();
            if (ls != null && ls.Count() > 0)
            {
                sb.Append(@"<tr>
                            <th> Tên Nhà Phát Hành </th>
                          </tr>");
                foreach (var pub in ls)
                {
                    sb.Append(@"<tr>");
                    sb.Append("<td>" + pub.name + @"</td >");
                    sb.Append(@"</ tr>");
                }
            }
            return sb.ToString();
        }

        [HttpPost]
        public async Task<string> GetListCombo()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<Combo>());
            }

            List<Combo> ls = await ComboMySql.GetListComboAsync();
            return JsonConvert.SerializeObject(ls);
        }

        // Lấy commbo đang kinh doanh
        [HttpPost]
        public async Task<string> GetActiveListCombo()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<Combo>());
            }

            List<Combo> ls = await ComboMySql.GetActiveListComboAsync();
            return JsonConvert.SerializeObject(ls);
        }

        [HttpPost]
        public async Task<string> GetCombo(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }

            Combo combo = await ComboMySql.GetComboAsync(id);
            return JsonConvert.SerializeObject(combo);
        }

        public async Task<ActionResult> UpdateDelete(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> MappingOfCombo(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            return View();
        }

        //private List<CommonItem> ShopeeGetListMappingOfCombo(int id, MySqlConnection conn, ProductController productController)
        //{
        //    // Danh sách sản phẩm Shopee
        //    List<CommonItem> shopeeList = sqler.ShopeeGetListMappingOfCombo(id, conn);
        //    //productController.ShopeeGetStatusImageSrcQuantitySellable(shopeeList);
        //    return shopeeList;
        //}

        //private List<CommonItem> TikiGetListMappingOfComboAsync(int id, MySqlConnection conn, ProductController productController)
        //{
        //    // Danh sách sản phẩm Tiki
        //    List<CommonItem> tikiList = TikiMySql.TikiGetListMappingOfComboAsync(id, conn);
        //    //productController.TikiGetStatusImageSrcQuantitySellable(tikiList);
        //    return tikiList;
        //}

        public static async Task<List<CommonItem>> GetListMappingOfComboCoreAsync(int id, MySqlConnection conn)
        {
            List<CommonItem> ls = new List<CommonItem>();
            try
            {
                List<CommonItem> tikiList = await TikiMySql.TikiGetListMappingOfComboAsync(id, conn);
                List<CommonItem> shopeeList = await ShopeeMySql.ShopeeGetListMappingOfComboAsync(id, conn);
                List<CommonItem> lazadaList = await LazadaMySql.LazadaGetListMappingOfComboAsync(id, conn);
                List<CommonItem> vbnList = await SanPhamMySql.GetListMappingOfComboAsync(id, conn);
                ls.AddRange(tikiList);
                ls.AddRange(shopeeList);
                ls.AddRange(lazadaList);
                ls.AddRange(vbnList);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return ls;
        }

        [HttpPost]
        public async Task <string> GetListMappingOfCombo(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<CommonItem>());
            }

            List<CommonItem> ls = new List<CommonItem>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    ls = await GetListMappingOfComboCoreAsync(id, conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            // Lấy danh sách sản phẩm
            return JsonConvert.SerializeObject(ls);
        }

        /// <summary>
        /// Upload ảnh cho combo
        /// </summary>
        [HttpPost]
        public async Task<string> UploadComboImage(HttpPostedFileBase file, int comboId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return "Lỗi xác thực.";
            }

            if (file == null || file.ContentLength == 0)
            {
                return "Ok. Không có file nào được gửi.";
            }

            if (comboId <= 0)
            {
                return "Lỗi: Combo ID không hợp lệ.";
            }

            try
            {
                // Tạo đường dẫn Media/Combo/{ComboId}/
                string comboFolderPath = System.Web.HttpContext.Current.Server.MapPath($"{Common.ComboMediaFolderPath}{comboId}/");

                // Tạo thư mục nếu chưa có
                if (!System.IO.Directory.Exists(comboFolderPath))
                {
                    System.IO.Directory.CreateDirectory(comboFolderPath);
                    // Tạo thư mục _320 cho thumbnail
                    System.IO.Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath($"{Common.ComboMediaFolderPath}{comboId}_320/"));
                }

                // Lấy tên file
                string fileName = System.IO.Path.GetFileName(file.FileName);
                string filePath = System.IO.Path.Combine(comboFolderPath, fileName);

                // Lưu file
                file.SaveAs(filePath);

                // Tạo thumbnail 320px nếu là ảnh
                if (Common.ImageExtensions.Contains(System.IO.Path.GetExtension(filePath).ToLower()))
                {
                    Common.ReduceImageSizeTo320AndSave(filePath);
                }

                return $"Ok. Upload thành công: {fileName}";
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"UploadComboImage error: {ex.ToString()}");
                return $"Lỗi: {ex.Message}";
            }
        }

        /// <summary>
        /// Lấy danh sách ảnh của combo
        /// </summary>
        [HttpGet]
        public async Task<string> GetComboImages(int comboId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<string>());
            }

            List<string> images = new List<string>();
            try
            {
                string comboFolderPath = System.Web.HttpContext.Current.Server.MapPath($"{Common.ComboMediaFolderPath}{comboId}/");

                if (System.IO.Directory.Exists(comboFolderPath))
                {
                    var files = System.IO.Directory.GetFiles(comboFolderPath)
                        .Where(f => Common.ImageExtensions.Contains(System.IO.Path.GetExtension(f).ToLower()))
                        .OrderBy(f => f)
                        .ToList();

                    foreach (var file in files)
                    {
                        string fileName = System.IO.Path.GetFileName(file);
                        images.Add($"/Media/Combo/{comboId}/{fileName}");
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetComboImages error: {ex.ToString()}");
            }

            return JsonConvert.SerializeObject(images);
        }
    }
}
