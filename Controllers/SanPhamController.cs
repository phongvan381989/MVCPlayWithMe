using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.SanPhamModel;
using MVCPlayWithMe.OpenPlatform.Model;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Org.BouncyCastle.Math.Field;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MVCPlayWithMe.Controllers
{
    public class SanPhamController : BasicController
    {
        public SanPhamController() : base()
        {
        }

        /// <summary>
        /// Trang tạo mới sản phẩm
        /// </summary>
        public async Task<ActionResult> Create()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            return View();
        }

        /// <summary>
        /// Trang tìm kiếm và liệt kê sản phẩm
        /// </summary>
        public async Task<ActionResult> Search()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            return View();
        }

        /// <summary>
        /// Trang cập nhật/xóa sản phẩm
        /// </summary>
        public async Task<ActionResult> UpdateDelete(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            ViewBag.SanPhamId = id;
            return View();
        }

        /// <summary>
        /// Lấy sản phẩm theo ID
        /// </summary>
        [HttpPost]
        public async Task<string> GetSanPhamFromId(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }

            SanPham sanPham = await SanPhamMySql.GetByIdAsync(id);
            return JsonConvert.SerializeObject(sanPham);
        }

        /// <summary>
        /// Lấy danh sách tất cả sản phẩm
        /// </summary>
        [HttpPost]
        public async Task<string> GetAllSanPham()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<SanPham>());
            }

            List<SanPham> list = await SanPhamMySql.GetAllAsync();
            return JsonConvert.SerializeObject(list);
        }

        /// <summary>
        /// Thêm mới sản phẩm
        /// </summary>
        [HttpPost]
        public async Task<string> AddNewSanPham()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            // Đọc JSON từ request body
            Request.InputStream.Position = 0;
            string json = new StreamReader(Request.InputStream).ReadToEnd();
            SanPham sanPham = JsonConvert.DeserializeObject<SanPham>(json);

            MySqlResultState result = await SanPhamMySql.InsertAsync(sanPham);

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Cập nhật 1 field riêng lẻ của sản phẩm
        /// </summary>
        [HttpPost]
        public async Task<string> UpdateSanPhamField(int sanPhamId, string fieldName, string fieldValue)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();

            try
            {
                //// Whitelist: allowed columns
                //var allowedColumns = new HashSet<string>
                //{
                //    "Code", "Barcode", "Name", "ShortName", "ComboId", "CategoryId", "BookCoverPrice",
                //    "Author", "Translator", "PublisherId", "PublishingCompany", "PublishingTime",
                //    "ProductLong", "ProductWide", "ProductHigh", "ProductWeight", "PositionInWarehouse",
                //    "HardCover", "MinAge", "MaxAge", "ParentId", "Republish", "Detail", "Status",
                //    "Quantity", "PageNumber", "Discount", "Language", "Date", "SoldQuantity", "URL", "SEOKeyword"
                //};

                //// Validate column name
                //if (!allowedColumns.Contains(fieldName))
                //{
                //    result.State = EMySqlResultState.ERROR;
                //    result.Message = $"Field '{fieldName}' không hợp lệ.";
                //    return JsonConvert.SerializeObject(result);
                //}

                //// Parse value theo kiểu dữ liệu
                //object paramValue;

                //// String nullable fields
                //if (new[] { "Code", "Barcode", "ShortName", "Author", "Translator", "PublishingCompany",
                //            "PositionInWarehouse", "Detail", "Language", "URL", "SEOKeyword" }.Contains(fieldName))
                //{
                //    paramValue = string.IsNullOrWhiteSpace(fieldValue) ? (object)DBNull.Value : fieldValue;
                //}
                //// String NOT NULL fields
                //else if (fieldName == "Name")
                //{
                //    paramValue = fieldValue;
                //}
                //// Int NOT NULL fields (default 0)
                //else if (new[] { "BookCoverPrice", "ProductLong", "ProductWide", "ProductHigh", "ProductWeight",
                //                 "Status", "Quantity" }.Contains(fieldName))
                //{
                //    paramValue = int.TryParse(fieldValue, out int intVal) ? intVal : 0;
                //}
                //// Int nullable fields (-1 = NULL)
                //else if (new[] { "ComboId", "CategoryId", "PublisherId", "PublishingTime", "HardCover",
                //                 "MinAge", "MaxAge", "ParentId", "Republish", "PageNumber", "SoldQuantity" }.Contains(fieldName))
                //{
                //    paramValue = int.TryParse(fieldValue, out int intVal) && intVal != -1 ? (object)intVal : DBNull.Value;
                //}
                //// Float field
                //else if (fieldName == "Discount")
                //{
                //    paramValue = float.TryParse(fieldValue, out float floatVal) ? floatVal : 0;
                //}
                //// DateTime field
                //else if (fieldName == "Date")
                //{
                //    paramValue = DateTime.TryParse(fieldValue, out DateTime dateVal) ? (object)dateVal : DBNull.Value;
                //}
                //else
                //{
                //    paramValue = fieldValue;
                //}

                // UPDATE chỉ 1 field
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    string sql = $"UPDATE tb_san_pham SET {fieldName} = @value WHERE id = @sanPhamId";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@value", fieldValue);
                        cmd.Parameters.AddWithValue("@sanPhamId", sanPhamId);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            result.State = EMySqlResultState.OK;
                            result.Message = $"Cập nhật {fieldName} thành công.";
                        }
                        else
                        {
                            result.State = EMySqlResultState.ERROR;
                            result.Message = "Không tìm thấy sản phẩm để cập nhật.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Cập nhật sản phẩm
        /// </summary>
        [HttpPost]
        public async Task<string> UpdateSanPham()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            // Đọc JSON từ request body
            Request.InputStream.Position = 0;
            string json = new StreamReader(Request.InputStream).ReadToEnd();
            SanPham sanPham = JsonConvert.DeserializeObject<SanPham>(json);

            MySqlResultState result = await SanPhamMySql.UpdateAsync(sanPham);

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Xóa sản phẩm
        /// </summary>
        [HttpPost]
        public async Task<string> DeleteSanPham(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();

            try
            {
                // Xóa media folder nếu có
                string path = Common.GetAbsoluteSanPhamMediaFolderPath(id.ToString());
                if (path != null)
                {
                    Common.DeleteMediaFolder(path);
                }

                // Xóa tất cả media metadata trong tb_san_pham_media
                MySqlResultState mediaResult = await SanPhamMediaMySql.DeleteBySanPhamIdAsync(id);
                if (mediaResult.State == EMySqlResultState.OK)
                {
                    MyLogger.GetInstance().Info($"Deleted all media metadata for SanPhamId: {id}");
                }

                // Xóa tất cả mapping trong tb_san_pham_mapping
                MySqlResultState mappingResult = await SanPhamMappingMySql.DeleteBySanPhamBanIdAsync(id);
                if (mappingResult.State == EMySqlResultState.OK)
                {
                    MyLogger.GetInstance().Info($"Deleted all kho mappings for SanPhamId: {id}");
                }

                // Xóa sản phẩm trong tb_san_pham
                result = await SanPhamMySql.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Upload media (ảnh/video) cho sản phẩm
        /// Header cần có: fileName (tên file gốc), productId, sanPhamName (tên sản phẩm)
        /// Tên file sẽ là: {slug-san-pham}-{slug-ten-file-goc}.webp
        /// VD: Harry Potter + "Ảnh Bìa.jpg" -> "harry-potter-anh-bia.webp"
        /// Nếu trùng tên (không kể extension) -> XÓA file cũ, lưu file mới
        /// </summary>
        [HttpPost]
        public async Task<string> UploadMedia(string sanPhamId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            if (string.IsNullOrWhiteSpace(sanPhamId))
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.INVALID, "ID sản phẩm không hợp lệ"));
            }

            string path = Common.GetAbsoluteSanPhamMediaFolderPath(sanPhamId);
            if (path == null)
            {
                path = Common.CreateAbsoluteSanPhamMediaFolderPath(sanPhamId);
            }

            MySqlResultState result = new MySqlResultState();

            try
            {
                var length = Request.ContentLength;
                var bytes = new byte[length];
                Request.InputStream.Read(bytes, 0, length);

                // Decode headers (vì client encode để hỗ trợ tiếng Việt)
                var originalFileName = HttpUtility.UrlDecode(Request.Headers["fileName"]);
                var sanPhamName = HttpUtility.UrlDecode(Request.Headers["sanPhamName"]);
                string ext = Path.GetExtension(originalFileName).ToLower();


                if (length > 0)
                {
                    // Tạo tên file mới: {slug-san-pham}-{slug-ten-file-goc}.ext
                    string newFileName = Common.GenerateImageFileName(sanPhamName, originalFileName);
                    if (!Common.ImageExtensions.Contains(ext))
                    {
                        newFileName = Common.GenerateVideoFileName(sanPhamName, originalFileName);
                    }

                    // XÓA tất cả file cùng tên (không kể extension) nếu có
                    // VD: Xóa harry-potter-anh-bia.jpg, harry-potter-anh-bia.webp
                    Common.DeleteImageVideoWithoutExtension(Path.Combine(path, newFileName));

                    // Lưu file mới
                    string saveToFileLoc = Path.Combine(path, newFileName);
                    using (var fileStream = new FileStream(saveToFileLoc, FileMode.Create, FileAccess.ReadWrite))
                    {
                        fileStream.Write(bytes, 0, length);
                    }

                    // Convert ảnh sang WebP (sẽ xóa file gốc JPG/PNG)
                    string finalFileName = newFileName;
                    string mediaType = "video";
                    string videoPosterName = "";
                    int mediaWidth = 0;
                    int mediaHeight = 0;

                    if (Common.ImageExtensions.Contains(ext))
                    {
                        // Image: Convert sang WebP + lấy kích thước
                        var (width, height, fileName) = Common.ConvertSanPhamImageToWebP(saveToFileLoc);
                        finalFileName = fileName;
                        mediaWidth = width;
                        mediaHeight = height;
                        mediaType = "image";
                    }
                    else
                    {
                        // Video: extract poster thumbnail + lấy kích thước video
                        videoPosterName = Path.GetFileNameWithoutExtension(finalFileName) + "-video-poster.webp";
                        var (width, height) = Common.ExtractVideoPoster(saveToFileLoc, videoPosterName);
                        mediaWidth = width;
                        mediaHeight = height;
                    }

                    // Insert metadata vào tb_san_pham_media
                    if (int.TryParse(sanPhamId, out int sanPhamIdInt))
                    {
                        MySqlResultState insertResult = await SanPhamMediaMySql.InsertAsync(new SanPhamMedia
                        {
                            SanPhamId = sanPhamIdInt,
                            MediaType = mediaType,
                            FileName = finalFileName,
                            Title = "",
                            AltText = "",
                            Description = "",
                            PosterImage = videoPosterName,
                            Width = mediaWidth,
                            Height = mediaHeight,
                            DisplayOrder = 38 //  hardcode 38 để ưu tiên hiển thị sau các ảnh khác (1-37) trong gallery
                        });

                        if (insertResult.State != EMySqlResultState.OK)
                        {
                            MyLogger.GetInstance().Warn($"Failed to insert media metadata: {insertResult.Message}");
                        }
                    }

                    result.State = EMySqlResultState.OK;
                    result.Message = "Upload thành công: " + finalFileName;
                    MyLogger.GetInstance().Info($"Uploaded: {originalFileName} -> {finalFileName}");
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
                MyLogger.GetInstance().Warn($"Upload failed: {ex.Message}");
            }

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Đổi tên media (ảnh/video) của sản phẩm
        /// Rename tất cả versions: file gốc (nếu còn), WebP full size, WebP thumbnail
        /// </summary>
        [HttpPost]
        public async Task<string> RenameMedia(string sanPhamId, string oldFileName, string newFileNameWithoutExt)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();

            try
            {
                string path = Common.GetAbsoluteSanPhamMediaFolderPath(sanPhamId);
                if (path == null)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Không tìm thấy thư mục media.";
                    return JsonConvert.SerializeObject(result);
                }

                // Validate tên mới
                if (string.IsNullOrWhiteSpace(newFileNameWithoutExt))
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Tên file mới không được để trống.";
                    return JsonConvert.SerializeObject(result);
                }

                // Lấy extension từ file cũ
                string extension = Path.GetExtension(oldFileName);
                //string oldFileNameWithoutExt = Path.GetFileNameWithoutExtension(oldFileName);

                //// Tạo slug cho tên mới
                //string newFileSlug = Common.ConvertToSlug(newFileName, 80);
                //if (string.IsNullOrWhiteSpace(newFileSlug))
                //    newFileSlug = "renamed";

                string newFileNameWithExt = newFileNameWithoutExt + extension;

                // Kiểm tra tên mới có trùng với file khác không
                string newFullPath = Path.Combine(path, newFileNameWithExt);
                if (System.IO.File.Exists(newFullPath) && !oldFileName.Equals(newFileNameWithExt, StringComparison.OrdinalIgnoreCase))
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = $"Tên file '{newFileNameWithExt}' đã tồn tại. Vui lòng chọn tên khác.";
                    return JsonConvert.SerializeObject(result);
                }

                // Rename file gốc
                string oldFullPath = Path.Combine(path, oldFileName);
                if (System.IO.File.Exists(oldFullPath))
                {
                    System.IO.File.Move(oldFullPath, newFullPath);
                }

                //// Rename WebP full size
                //string oldWebpPath = Path.ChangeExtension(oldFullPath, ".webp");
                //string newWebpPath = Path.Combine(path, Path.ChangeExtension(newFileNameWithExt, ".webp"));
                //if (System.IO.File.Exists(oldWebpPath))
                //{
                //    System.IO.File.Move(oldWebpPath, newWebpPath);
                //}

                // Rename WebP thumbnail trong _320
                //string parentDir = Directory.GetParent(path).FullName;
                //string folderName = new DirectoryInfo(path).Name;
                //string thumbFolder = Path.Combine(parentDir, folderName + "_320");
                string thumbFolder = Path.GetDirectoryName(path) + "_320";

                string oldThumbPath = Path.Combine(thumbFolder, oldFileName);
                string newThumbPath = Path.Combine(thumbFolder, newFileNameWithExt);
                if(!Common.ImageExtensions.Contains(extension))
                {
                    oldThumbPath = Path.Combine(thumbFolder, Path.GetFileNameWithoutExtension(oldFileName) + "-video-poster.webp");
                    newThumbPath = Path.Combine(thumbFolder, newFileNameWithoutExt + "-video-poster.webp");
                }

                if (System.IO.File.Exists(oldThumbPath))
                {
                    System.IO.File.Move(oldThumbPath, newThumbPath);
                }

                // Update FileName trong database (nếu có metadata)
                if (int.TryParse(sanPhamId, out int sanPhamIdInt))
                {
                    MySqlResultState dbResult = await SanPhamMediaMySql.UpdateFileNameAsync(sanPhamIdInt, oldFileName, newFileNameWithExt);
                    if (dbResult.State == EMySqlResultState.OK)
                    {
                        //MyLogger.GetInstance().Info($"Updated metadata FileName: {oldFileName} → {newFileNameWithExt}");
                    }
                    else
                    {
                        MyLogger.GetInstance().Warn($"Failed to update metadata FileName: {dbResult.Message}");
                    }
                }

                result.State = EMySqlResultState.OK;
                result.Message = $"Đổi tên thành công: {oldFileName} → {newFileNameWithExt}";
                MyLogger.GetInstance().Info($"Renamed: {oldFileName} → {newFileNameWithExt}");
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Xóa media (ảnh/video) cụ thể của sản phẩm
        /// </summary>
        [HttpPost]
        public async Task<string> DeleteMedia(string sanPhamId, string fileName)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();

            try
            {
                string path = Common.GetAbsoluteSanPhamMediaFolderPath(sanPhamId);
                if (path != null)
                {

                    string ext = Path.GetExtension(fileName).ToLower();
                    string pathFileName = Path.Combine(path, fileName);
                    // Là ảnh
                    if (Common.ImageExtensions.Contains(ext))
                    {
                        string pathThumbFileName = Path.Combine(Path.GetDirectoryName(path) + "_320", fileName);

                        // Xóa ảnh gốc
                        if (System.IO.File.Exists(pathFileName))
                        {
                            System.IO.File.Delete(pathFileName);
                        }

                        // Xóa ảnh 320 (thumbnail)
                        if (System.IO.File.Exists(pathThumbFileName))
                        {
                            System.IO.File.Delete(pathThumbFileName);
                        }
                    }
                    else// Là video
                    {
                        System.IO.File.Delete(pathFileName);
                    }

                    // Xóa metadata trong database (nếu có)
                    if (int.TryParse(sanPhamId, out int sanPhamIdInt))
                    {
                        MySqlResultState dbResult = await SanPhamMediaMySql.DeleteByFileNameAsync(sanPhamIdInt, fileName);
                        if (dbResult.State == EMySqlResultState.OK)
                        {
                            MyLogger.GetInstance().Info($"Deleted metadata for fileName: {fileName}");
                        }
                        else
                        {
                            MyLogger.GetInstance().Warn($"Failed to delete metadata: {dbResult.Message}");
                        }
                    }

                    result.State = EMySqlResultState.OK;
                    result.Message = "Xóa media thành công.";
                }
                else
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Không tìm thấy thư mục media.";
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return JsonConvert.SerializeObject(result);
        }

        ///// <summary>
        ///// Lấy danh sách media của sản phẩm
        ///// </summary>
        //[HttpPost]
        //public async Task<string> GetMediaList(string sanPhamId)
        //{
        //    if ((await AuthentAdministratorAsync()) == null)
        //    {
        //        return JsonConvert.SerializeObject(new List<MediaFileInfo>());
        //    }

        //    List<MediaFileInfo> mediaList = new List<MediaFileInfo>();

        //    try
        //    {
        //        string path = Common.GetAbsoluteSanPhamMediaFolderPath(sanPhamId);
        //        if (path != null)
        //        {
        //            List<SanPhamMedia> listMedia = await SanPhamMediaMySql.GetListBySanPhamIdAsync(int.Parse(sanPhamId));
        //            string relativePathPrefix = Common.SanPhamMediaFolderPath + sanPhamId + "/";
        //            foreach (var obj in listMedia)
        //            {
        //                string relativePath = Common.SanPhamMediaFolderPath + sanPhamId + "/" + obj.FileName;

        //                mediaList.Add(new MediaFileInfo
        //                {
        //                    FileName = obj.FileName,
        //                    FilePath = relativePathPrefix + obj.FileName,
        //                    IsImage = obj.MediaType== "image" ? true : false
        //                });
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        MyLogger.GetInstance().Warn(ex.ToString());
        //    }

        //    return JsonConvert.SerializeObject(mediaList);
        //}

        #region SanPhamMedia API Methods

        /// <summary>
        /// Lấy metadata (Alt text, Title, etc.) của media theo FileName
        /// </summary>
        [HttpPost]
        public async Task<string> GetMediaMetadata(int sanPhamId, string fileName)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }

            List<SanPhamMedia> list = await SanPhamMediaMySql.GetListBySanPhamIdAsync(sanPhamId);
            SanPhamMedia media = list.FirstOrDefault(m => m.FileName == fileName);
            return JsonConvert.SerializeObject(media);
        }

        /// <summary>
        /// Lấy tất cả media metadata của sản phẩm
        /// </summary>
        [HttpPost]
        public async Task<string> GetAllMediaMetadata(int sanPhamId)
        {
            List<SanPhamMedia> list = await SanPhamMediaMySql.GetListBySanPhamIdAsync(sanPhamId);
            return JsonConvert.SerializeObject(list);
        }

        /// <summary>
        /// Thêm media metadata mới
        /// </summary>
        [HttpPost]
        public async Task<string> AddMediaMetadata()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            Request.InputStream.Position = 0;
            string json = new StreamReader(Request.InputStream).ReadToEnd();
            SanPhamMedia media = JsonConvert.DeserializeObject<SanPhamMedia>(json);

            MySqlResultState result = await SanPhamMediaMySql.InsertAsync(media);
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Cập nhật media metadata (Title, AltText, Description, PosterImage, DisplayOrder)
        /// </summary>
        [HttpPost]
        public async Task<string> UpdateMediaMetadata()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            Request.InputStream.Position = 0;
            string json = new StreamReader(Request.InputStream).ReadToEnd();
            SanPhamMedia media = JsonConvert.DeserializeObject<SanPhamMedia>(json);

            MySqlResultState result = await SanPhamMediaMySql.UpdateAsync(media);
            return JsonConvert.SerializeObject(result);
        }

        #endregion

        #region SanPhamMapping API Methods

        /// <summary>
        /// Lấy tất cả sản phẩm kho (tbproducts) - load 1 lần cho filtering
        /// </summary>
        [HttpPost]
        public async Task<string> GetAllTbProducts()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<TbProduct>());
            }

            List<TbProduct> list = await SanPhamMappingMySql.GetAllTbProductsAsync();
            return JsonConvert.SerializeObject(list);
        }

        /// <summary>
        /// Search sản phẩm kho (tbproducts) - dùng cho autocomplete
        /// </summary>
        [HttpPost]
        public async Task<string> SearchTbProducts(string keyword)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<TbProduct>());
            }

            if (string.IsNullOrWhiteSpace(keyword))
            {
                return JsonConvert.SerializeObject(new List<TbProduct>());
            }

            List<TbProduct> list = await SanPhamMappingMySql.SearchTbProductsAsync(keyword, 20);
            return JsonConvert.SerializeObject(list);
        }

        /// <summary>
        /// Lấy danh sách mapping của sản phẩm bán
        /// </summary>
        [HttpPost]
        public async Task<string> GetKhoMappingList(int sanPhamBanId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<SanPhamMapping>());
            }

            List<SanPhamMapping> list = await SanPhamMappingMySql.GetListBySanPhamBanIdAsync(sanPhamBanId);
            return JsonConvert.SerializeObject(list);
        }

        /// <summary>
        /// Thêm mapping mới
        /// </summary>
        [HttpPost]
        public async Task<string> AddKhoMapping()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            Request.InputStream.Position = 0;
            string json = new StreamReader(Request.InputStream).ReadToEnd();
            SanPhamMapping mapping = JsonConvert.DeserializeObject<SanPhamMapping>(json);

            MySqlResultState result = await SanPhamMappingMySql.InsertAsync(mapping);
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Update số lượng của mapping
        /// </summary>
        [HttpPost]
        public async Task<string> UpdateKhoMappingQuantity(int mappingId, int quantity)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = await SanPhamMappingMySql.UpdateQuantityAsync(mappingId, quantity);
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Xóa mapping
        /// </summary>
        [HttpPost]
        public async Task<string> DeleteKhoMapping(int mappingId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = await SanPhamMappingMySql.DeleteAsync(mappingId);
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Chép dữ liệu từ sản phẩm kho (tbproducts) sang sản phẩm bán (tb_san_pham)
        /// Chỉ dùng khi có đúng 1 mapping
        /// </summary>
        [HttpPost]
        public async Task<string> CopyDataFromKhoProduct(int sanPhamBanId, int sanPhamKhoId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = await SanPhamMappingMySql.CopyDataFromKhoProductAsync(sanPhamBanId, sanPhamKhoId);
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Chép toàn bộ ảnh từ sản phẩm kho sang sản phẩm bán
        /// Không chép video
        /// Xóa ảnh/video của sản phẩm bán cũ, copy ảnh mới và sinh phiên bản 320
        /// Chỉ dùng khi có đúng 1 mapping
        /// </summary>
        [HttpPost]
        public async Task<string> CopyImagesFromKhoProduct(string sanPhamName, int sanPhamBanId, int sanPhamKhoId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();

            try
            {
                // 1. Lấy đường dẫn folder ảnh kho (source)
                string sourceFolder = Common.GetAbsoluteProductMediaFolderPath(sanPhamKhoId.ToString());
                if (sourceFolder == null || !Directory.Exists(sourceFolder))
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Sản phẩm kho không có ảnh để copy";
                    return JsonConvert.SerializeObject(result);
                }

                // 2. Xóa toàn bộ ảnh cũ của sản phẩm bán (bao gồm cả folder _320)
                string path = Common.GetAbsoluteSanPhamMediaFolderPath(sanPhamBanId.ToString());
                if (path != null)
                {
                    Common.DeleteAllMediaFileInclude320(path);
                }

                // Xóa metadata của ảnh
                await SanPhamMediaMySql.DeleteBySanPhamIdAsync(sanPhamBanId);

                // 3. Tạo folder mới cho sản phẩm bán (bao gồm cả folder _320)
                string destFolder = Common.CreateAbsoluteSanPhamMediaFolderPath(sanPhamBanId.ToString());

                // 4. Copy tất cả ảnh từ kho sang bán
                string[] sourceFiles = Directory.GetFiles(sourceFolder);
                int copiedCount = 0;

                foreach (string sourceFile in sourceFiles)
                {
                    string fileName = Path.GetFileName(sourceFile);
                    string extension = Path.GetExtension(sourceFile).ToLower();

                    // Chỉ copy ảnh (không copy video)
                    if (Common.ImageExtensions.Contains(extension))
                    {
                        // Tạo tên file mới: {slug-san-pham}-{slug-ten-file-goc}.ext
                        string newFileName = Common.GenerateImageFileName(sanPhamName, fileName);
                        string destFile = Path.Combine(destFolder, newFileName);
                        System.IO.File.Copy(sourceFile, destFile, overwrite: true);

                        //string webpImage = Common.ConvertSanPhamImageToWebP(destFile);
                        var (width, height, webpImage) = Common.ConvertSanPhamImageToWebP(destFile);

                        int mediaWidth = width;
                        int mediaHeight = height;

                        await SanPhamMediaMySql.InsertAsync(new SanPhamMedia
                        {
                            SanPhamId = sanPhamBanId,
                            //MediaType = "image", db sinh gia tri mac dinh la "image" nen khong can set
                            FileName = webpImage,
                            Title = "",
                            AltText = "",
                            Description = "",
                            PosterImage = "",
                            Width = mediaWidth,
                            Height = mediaHeight,
                            DisplayOrder = copiedCount
                        });

                        copiedCount++;
                    }
                }

                if (copiedCount == 0)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Không tìm thấy ảnh nào để copy";
                    return JsonConvert.SerializeObject(result);
                }
                result.State = EMySqlResultState.OK;
                result.Message = $"Chép {copiedCount} ảnh thành công";
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                result.State = EMySqlResultState.ERROR;
                result.Message = ex.Message;
            }

            return JsonConvert.SerializeObject(result);
        }

        #endregion

        #region Price Calculation API Methods

        /// <summary>
        /// Tính giá bán thực tế từ mapping sản phẩm kho + TaxAndFee
        /// </summary>
        /// <param name="sanPhamId">ID sản phẩm bán (tb_san_pham)</param>
        /// <param name="platform">Tên sàn: PLAYWITHME, SHOPEE, TIKI, LAZADA</param>
        [HttpPost]
        public async Task<string> CalculateAndUpdateSalePrice(int sanPhamId, string platform = "PLAYWITHME")
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                // 1. Load mappings với sản phẩm kho
                var mappings = await SanPhamMappingMySql.GetListBySanPhamBanIdAsync(sanPhamId);

                if (mappings == null || mappings.Count == 0)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Chưa có mapping sản phẩm kho. Vui lòng mapping trước khi tính giá.";
                    return JsonConvert.SerializeObject(result);
                }

                // 2. Load sản phẩm hiện tại
                var sanPham = await SanPhamMySql.GetByIdAsync(sanPhamId);
                if (sanPham == null)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Không tìm thấy sản phẩm";
                    return JsonConvert.SerializeObject(result);
                }

                // 3. Tính tổng giá bìa và tổng giá nhập từ mapping
                int tongGiaBia = 0;
                decimal tongGiaNhap = 0;

                foreach (var mapping in mappings)
                {
                    tongGiaBia += mapping.SanPhamKhoBookCoverPrice * mapping.Quantity;
                    decimal giaNhap = mapping.SanPhamKhoBookCoverPrice * (100 - (decimal)mapping.SanPhamKhoDiscount) / 100;
                    tongGiaNhap += giaNhap * mapping.Quantity;
                }

                // 4. Tính chiết khấu thực tế từ mapping
                float chietKhauThucTe = 0;
                if (tongGiaBia > 0)
                {
                    chietKhauThucTe = (float)((tongGiaBia - tongGiaNhap) / tongGiaBia * 100);
                }

                // 5. Kiểm tra và update BookCoverPrice và Discount nếu khác
                bool needUpdate = false;
                bool bookCoverPriceChanged = (sanPham.BookCoverPrice != tongGiaBia);
                bool discountChanged = (Math.Abs(sanPham.Discount - chietKhauThucTe) > 0.1f);

                if (bookCoverPriceChanged || discountChanged)
                {
                    needUpdate = true;
                    await SanPhamMySql.UpdateBookCoverPriceAndDiscountAsync(sanPhamId, tongGiaBia, chietKhauThucTe);
                }

                // 8. Load TaxAndFee của sàn
                var taxAndFee = await TaxAndFeeMySql.GetByNameAsync(platform);

                if (taxAndFee == null)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = $"Không tìm thấy TaxAndFee cho sàn {platform}";
                    return JsonConvert.SerializeObject(result);
                }

                // 9. Tính giá bán
                int salePrice = PriceCalculator.CalculateSalePrice(mappings, taxAndFee);

                if (salePrice <= 0)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Giá bán tính được <= 0. Kiểm tra lại dữ liệu.";
                    return JsonConvert.SerializeObject(result);
                }

                // 10. Update SalePrice vào tb_san_pham
                result = await SanPhamMySql.UpdateSalePriceAsync(sanPhamId, salePrice);

                if (result.State == EMySqlResultState.OK)
                {
                    // 11. Trả về breakdown để hiển thị
                    var breakdown = PriceCalculator.GetPriceBreakdown(mappings, taxAndFee);

                    string updateInfo = "";
                    if (needUpdate)
                    {
                        updateInfo = $" (Đã cập nhật: Giá bìa = {tongGiaBia:N0} đ, Chiết khấu = {chietKhauThucTe:F1}%)";
                    }

                    result.State = EMySqlResultState.OK;
                    result.Message = $"Tính giá thành công! Giá bán: {salePrice:N0} đ{updateInfo}";

                    // Return thêm chi tiết
                    return JsonConvert.SerializeObject(new
                    {
                        State = (int)result.State,
                        Message = result.Message,
                        SalePrice = salePrice,
                        BookCoverPrice = tongGiaBia,
                        Discount = chietKhauThucTe,
                        Breakdown = breakdown
                    });
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = ex.Message;
            }

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Lấy danh sách TaxAndFee (cho dropdown chọn sàn)
        /// </summary>
        [HttpGet]
        public async Task<string> GetTaxAndFeeList()
        {
            try
            {
                var list = await TaxAndFeeMySql.GetAllAsync();
                return JsonConvert.SerializeObject(new
                {
                    state = (int)EMySqlResultState.OK,
                    data = list
                });
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                return JsonConvert.SerializeObject(new
                {
                    state = (int)EMySqlResultState.EXCEPTION,
                    message = ex.Message
                });
            }
        }

        #endregion
    }

    ///// <summary>
    ///// Class chứa thông tin file media
    ///// </summary>
    //public class MediaFileInfo
    //{
    //    public string FileName { get; set; }
    //    public string FilePath { get; set; }
    //    public bool IsImage { get; set; }
    //}
}
