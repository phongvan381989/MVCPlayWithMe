using MVCPlayWithMe.General;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MVCPlayWithMe.Models.SanPhamModel
{
    public class SanPhamMySql
    {
        /// <summary>
        /// Thêm mới sản phẩm vào bảng tb_san_pham
        /// </summary>
        /// <param name="sanPham">Đối tượng sản phẩm cần insert</param>
        /// <returns>MySqlResultState với State, Message và LastInsertedId</returns>
        public static async Task<MySqlResultState> Insert(SanPham sanPham)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand("sp_tbSanPham_Insert", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add IN parameters
                        cmd.Parameters.Add("@inCode", MySqlDbType.VarChar).Value = sanPham.Code ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inBarcode", MySqlDbType.VarChar).Value = sanPham.Barcode ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inName", MySqlDbType.VarChar).Value = sanPham.Name;
                        cmd.Parameters.Add("@inShortName", MySqlDbType.VarChar).Value = sanPham.ShortName ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inComboId", MySqlDbType.Int32).Value = sanPham.ComboId ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inCategoryId", MySqlDbType.Int32).Value = sanPham.CategoryId ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inBookCoverPrice", MySqlDbType.Int32).Value = sanPham.BookCoverPrice;
                        cmd.Parameters.Add("@inAuthor", MySqlDbType.VarChar).Value = sanPham.Author ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inTranslator", MySqlDbType.VarChar).Value = sanPham.Translator ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inPublisherId", MySqlDbType.Int32).Value = sanPham.PublisherId ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inPublishingCompany", MySqlDbType.VarChar).Value = sanPham.PublishingCompany ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inPublishingTime", MySqlDbType.Int32).Value = sanPham.PublishingTime ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inProductLong", MySqlDbType.Int32).Value = sanPham.ProductLong;
                        cmd.Parameters.Add("@inProductWide", MySqlDbType.Int32).Value = sanPham.ProductWide;
                        cmd.Parameters.Add("@inProductHigh", MySqlDbType.Int32).Value = sanPham.ProductHigh;
                        cmd.Parameters.Add("@inProductWeight", MySqlDbType.Int32).Value = sanPham.ProductWeight;
                        cmd.Parameters.Add("@inPositionInWarehouse", MySqlDbType.VarChar).Value = sanPham.PositionInWarehouse ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inHardCover", MySqlDbType.Int32).Value = sanPham.HardCover ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inMinAge", MySqlDbType.Int32).Value = sanPham.MinAge ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inMaxAge", MySqlDbType.Int32).Value = sanPham.MaxAge ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inParentId", MySqlDbType.Int32).Value = sanPham.ParentId ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inRepublish", MySqlDbType.Int32).Value = sanPham.Republish ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inDetail", MySqlDbType.VarChar).Value = sanPham.Detail ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inStatus", MySqlDbType.Int32).Value = sanPham.Status;
                        cmd.Parameters.Add("@inQuantity", MySqlDbType.Int32).Value = sanPham.Quantity;
                        cmd.Parameters.Add("@inPageNumber", MySqlDbType.Int32).Value = sanPham.PageNumber ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inDiscount", MySqlDbType.Float).Value = sanPham.Discount;
                        cmd.Parameters.Add("@inSalePrice", MySqlDbType.Int32).Value = sanPham.SalePrice;
                        cmd.Parameters.Add("@inLanguage", MySqlDbType.VarChar).Value = sanPham.Language ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inDate", MySqlDbType.Date).Value = sanPham.Date ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inSoldQuantity", MySqlDbType.Int32).Value = sanPham.SoldQuantity ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inURL", MySqlDbType.VarChar).Value = sanPham.URL ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inSEOKeyword", MySqlDbType.VarChar).Value = sanPham.SEOKeyword ?? (object)DBNull.Value;

                        // Execute và đọc LastInsertId từ SELECT
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            // Đọc LastInsertId từ resultset
                            if (rdr.Read())
                            {
                                int lastId = rdr.GetInt32("LastId");
                                sanPham.Id = lastId; // Set Id cho object

                                result.State = EMySqlResultState.OK;
                                result.Message = $"Thêm sản phẩm thành công. ID: {lastId}";
                            }
                            else
                            {
                                result.State = EMySqlResultState.EXCEPTION;
                                result.Message = "Không lấy được ID sau khi insert.";
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = ex.Message;
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Thêm mới sản phẩm (async version)
        /// </summary>
        /// <param name="sanPham">Đối tượng sản phẩm cần insert</param>
        /// <returns>MySqlResultState với State, Message và LastInsertedId</returns>
        public static async Task<MySqlResultState> InsertAsync(SanPham sanPham)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("sp_tbSanPham_Insert", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add IN parameters
                        cmd.Parameters.Add("@inCode", MySqlDbType.VarChar).Value = sanPham.Code ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inBarcode", MySqlDbType.VarChar).Value = sanPham.Barcode ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inName", MySqlDbType.VarChar).Value = sanPham.Name;
                        cmd.Parameters.Add("@inShortName", MySqlDbType.VarChar).Value = sanPham.ShortName ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inComboId", MySqlDbType.Int32).Value = sanPham.ComboId ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inCategoryId", MySqlDbType.Int32).Value = sanPham.CategoryId ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inBookCoverPrice", MySqlDbType.Int32).Value = sanPham.BookCoverPrice;
                        cmd.Parameters.Add("@inAuthor", MySqlDbType.VarChar).Value = sanPham.Author ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inTranslator", MySqlDbType.VarChar).Value = sanPham.Translator ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inPublisherId", MySqlDbType.Int32).Value = sanPham.PublisherId ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inPublishingCompany", MySqlDbType.VarChar).Value = sanPham.PublishingCompany ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inPublishingTime", MySqlDbType.Int32).Value = sanPham.PublishingTime ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inProductLong", MySqlDbType.Int32).Value = sanPham.ProductLong;
                        cmd.Parameters.Add("@inProductWide", MySqlDbType.Int32).Value = sanPham.ProductWide;
                        cmd.Parameters.Add("@inProductHigh", MySqlDbType.Int32).Value = sanPham.ProductHigh;
                        cmd.Parameters.Add("@inProductWeight", MySqlDbType.Int32).Value = sanPham.ProductWeight;
                        cmd.Parameters.Add("@inPositionInWarehouse", MySqlDbType.VarChar).Value = sanPham.PositionInWarehouse ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inHardCover", MySqlDbType.Int32).Value = sanPham.HardCover ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inMinAge", MySqlDbType.Int32).Value = sanPham.MinAge ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inMaxAge", MySqlDbType.Int32).Value = sanPham.MaxAge ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inParentId", MySqlDbType.Int32).Value = sanPham.ParentId ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inRepublish", MySqlDbType.Int32).Value = sanPham.Republish ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inDetail", MySqlDbType.VarChar).Value = sanPham.Detail ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inStatus", MySqlDbType.Int32).Value = sanPham.Status;
                        cmd.Parameters.Add("@inQuantity", MySqlDbType.Int32).Value = sanPham.Quantity;
                        cmd.Parameters.Add("@inPageNumber", MySqlDbType.Int32).Value = sanPham.PageNumber ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inDiscount", MySqlDbType.Float).Value = sanPham.Discount;
                        cmd.Parameters.Add("@inSalePrice", MySqlDbType.Int32).Value = sanPham.SalePrice;
                        cmd.Parameters.Add("@inLanguage", MySqlDbType.VarChar).Value = sanPham.Language ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inDate", MySqlDbType.Date).Value = sanPham.Date ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inSoldQuantity", MySqlDbType.Int32).Value = sanPham.SoldQuantity ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inURL", MySqlDbType.VarChar).Value = sanPham.URL ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inSEOKeyword", MySqlDbType.VarChar).Value = sanPham.SEOKeyword ?? (object)DBNull.Value;

                        // Execute và đọc LastInsertId từ SELECT
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            // Đọc LastInsertId từ resultset
                            if (await rdr.ReadAsync())
                            {
                                int lastId = rdr.GetInt32("LastId");
                                sanPham.Id = lastId; // Set Id cho object

                                result.State = EMySqlResultState.OK;
                                result.Message = $"Thêm sản phẩm thành công. ID: {lastId}";
                            }
                            else
                            {
                                result.State = EMySqlResultState.EXCEPTION;
                                result.Message = "Không lấy được ID sau khi insert.";
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = ex.Message;
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Lấy sản phẩm theo Id
        /// </summary>
        /// <param name="id">Id sản phẩm</param>
        /// <returns>SanPham hoặc null nếu không tìm thấy</returns>
        public static async Task<SanPham> GetByIdAsync(int id)
        {
            SanPham sanPham = null;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(
                        "SELECT * FROM tb_san_pham WHERE Id = @inId", conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@inId", MySqlDbType.Int32).Value = id;

                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            if (await rdr.ReadAsync())
                            {
                                sanPham = ConvertRowFromDataReader(rdr);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return sanPham;
        }

        /// <summary>
        /// Lấy thông tin cơ bản sản phẩm kèm ảnh bìa (lightweight DTO)
        /// Dùng cho cart, checkout để tối ưu performance
        /// </summary>
        /// <param name="id">Id sản phẩm</param>
        /// <returns>SanPhamBasicInfo hoặc null nếu không tìm thấy</returns>
        public static async Task<SanPhamBasicInfo> GetSanPhamBasicInfo_ConnectOutAsync(int id, MySqlConnection conn)
        {
            SanPhamBasicInfo info = null;

            try
            {
                string query = @"
                    SELECT
                        sp.Id,
                        sp.Name,
                        sp.ShortName,
                        sp.BookCoverPrice,
                        sp.SalePrice,
                        sp.Quantity,
                        sp.Status,
                        (SELECT FileName
                            FROM tb_san_pham_media
                            WHERE SanPhamId = sp.Id
                            AND MediaType = 'image'
                            ORDER BY DisplayOrder ASC
                            LIMIT 1) AS CoverImageFileName
                    FROM tb_san_pham sp
                    WHERE sp.Id = @inId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@inId", MySqlDbType.Int32).Value = id;

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        if (await rdr.ReadAsync())
                        {
                            info = new SanPhamBasicInfo
                            {
                                Id = rdr.GetInt32("Id"),
                                Name = MyMySql.GetString(rdr, "Name"),
                                ShortName = MyMySql.GetString(rdr, "ShortName"),
                                BookCoverPrice = MyMySql.GetInt32(rdr, "BookCoverPrice"),
                                SalePrice = MyMySql.GetInt32(rdr, "SalePrice"),
                                Quantity = MyMySql.GetInt32(rdr, "Quantity"),
                                Status = MyMySql.GetInt32(rdr, "Status"),
                                CoverImageFileName = MyMySql.GetString(rdr, "CoverImageFileName")
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetSanPhamBasicInfoAsync error: {ex.Message}");
            }

            return info;
        }

        /// <summary>
        /// Lấy danh sách tất cả sản phẩm
        /// </summary>
        /// <returns>Danh sách sản phẩm</returns>
        public static async Task<List<SanPham>> GetAllAsync()
        {
            List<SanPham> list = new List<SanPham>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(
                        "SELECT * FROM tb_san_pham ORDER BY Id DESC", conn))
                    {
                        cmd.CommandType = CommandType.Text;

                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                list.Add(ConvertRowFromDataReader(rdr));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                list.Clear();
            }

            return list;
        }

        /// <summary>
        /// Cập nhật sản phẩm
        /// </summary>
        /// <param name="sanPham">Đối tượng sản phẩm cần update</param>
        /// <returns>MySqlResultState</returns>
        public static async Task<MySqlResultState> UpdateAsync(SanPham sanPham)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("sp_tbSanPham_Update", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@inId", MySqlDbType.Int32).Value = sanPham.Id;
                        cmd.Parameters.Add("@inCode", MySqlDbType.VarChar).Value = sanPham.Code ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inBarcode", MySqlDbType.VarChar).Value = sanPham.Barcode ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inName", MySqlDbType.VarChar).Value = sanPham.Name;
                        cmd.Parameters.Add("@inShortName", MySqlDbType.VarChar).Value = sanPham.ShortName ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inComboId", MySqlDbType.Int32).Value = sanPham.ComboId ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inCategoryId", MySqlDbType.Int32).Value = sanPham.CategoryId ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inBookCoverPrice", MySqlDbType.Int32).Value = sanPham.BookCoverPrice;
                        cmd.Parameters.Add("@inAuthor", MySqlDbType.VarChar).Value = sanPham.Author ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inTranslator", MySqlDbType.VarChar).Value = sanPham.Translator ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inPublisherId", MySqlDbType.Int32).Value = sanPham.PublisherId ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inPublishingCompany", MySqlDbType.VarChar).Value = sanPham.PublishingCompany ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inPublishingTime", MySqlDbType.Int32).Value = sanPham.PublishingTime ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inProductLong", MySqlDbType.Int32).Value = sanPham.ProductLong;
                        cmd.Parameters.Add("@inProductWide", MySqlDbType.Int32).Value = sanPham.ProductWide;
                        cmd.Parameters.Add("@inProductHigh", MySqlDbType.Int32).Value = sanPham.ProductHigh;
                        cmd.Parameters.Add("@inProductWeight", MySqlDbType.Int32).Value = sanPham.ProductWeight;
                        cmd.Parameters.Add("@inPositionInWarehouse", MySqlDbType.VarChar).Value = sanPham.PositionInWarehouse ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inHardCover", MySqlDbType.Int32).Value = sanPham.HardCover ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inMinAge", MySqlDbType.Int32).Value = sanPham.MinAge ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inMaxAge", MySqlDbType.Int32).Value = sanPham.MaxAge ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inParentId", MySqlDbType.Int32).Value = sanPham.ParentId ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inRepublish", MySqlDbType.Int32).Value = sanPham.Republish ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inDetail", MySqlDbType.VarChar).Value = sanPham.Detail ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inStatus", MySqlDbType.Int32).Value = sanPham.Status;
                        cmd.Parameters.Add("@inQuantity", MySqlDbType.Int32).Value = sanPham.Quantity;
                        cmd.Parameters.Add("@inPageNumber", MySqlDbType.Int32).Value = sanPham.PageNumber ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inDiscount", MySqlDbType.Float).Value = sanPham.Discount;
                        cmd.Parameters.Add("@inSalePrice", MySqlDbType.Int32).Value = sanPham.SalePrice;
                        cmd.Parameters.Add("@inLanguage", MySqlDbType.VarChar).Value = sanPham.Language ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inDate", MySqlDbType.Date).Value = sanPham.Date ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inSoldQuantity", MySqlDbType.Int32).Value = sanPham.SoldQuantity ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inURL", MySqlDbType.VarChar).Value = sanPham.URL ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inSEOKeyword", MySqlDbType.VarChar).Value = sanPham.SEOKeyword ?? (object)DBNull.Value;

                        await cmd.ExecuteNonQueryAsync();

                        result.State = EMySqlResultState.OK;
                        result.Message = $"Cập nhật sản phẩm thành công. ID: {sanPham.Id}";
                    }
                }
            }
            catch (MySqlException ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = ex.Message;
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Cập nhật chỉ SalePrice của sản phẩm (không update các trường khác)
        /// </summary>
        /// <param name="sanPhamId">ID sản phẩm</param>
        /// <param name="salePrice">Giá bán thực tế mới</param>
        /// <returns>MySqlResultState</returns>
        public static async Task<MySqlResultState> UpdateSalePriceAsync(int sanPhamId, int salePrice)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = "UPDATE tb_san_pham SET SalePrice = @salePrice WHERE Id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@salePrice", MySqlDbType.Int32).Value = salePrice;
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = sanPhamId;

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            result.State = EMySqlResultState.OK;
                            result.Message = $"Cập nhật SalePrice thành công. ID: {sanPhamId}, SalePrice: {salePrice:N0} đ";
                        }
                        else
                        {
                            result.State = EMySqlResultState.EXCEPTION;
                            result.Message = $"Không tìm thấy sản phẩm với ID: {sanPhamId}";
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = ex.Message;
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Cập nhật chỉ BookCoverPrice và Discount của sản phẩm (không update các trường khác)
        /// Dùng khi tính giá tự động từ mapping sản phẩm kho
        /// </summary>
        /// <param name="sanPhamId">ID sản phẩm</param>
        /// <param name="bookCoverPrice">Giá bìa mới</param>
        /// <param name="discount">Chiết khấu mới (0-100)</param>
        /// <returns>MySqlResultState</returns>
        public static async Task<MySqlResultState> UpdateBookCoverPriceAndDiscountAsync(int sanPhamId, int bookCoverPrice, float discount)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = "UPDATE tb_san_pham SET BookCoverPrice = @bookCoverPrice, Discount = @discount WHERE Id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@bookCoverPrice", MySqlDbType.Int32).Value = bookCoverPrice;
                        cmd.Parameters.Add("@discount", MySqlDbType.Float).Value = discount;
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = sanPhamId;

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            result.State = EMySqlResultState.OK;
                            result.Message = $"Cập nhật BookCoverPrice và Discount thành công. ID: {sanPhamId}, BookCoverPrice: {bookCoverPrice:N0} đ, Discount: {discount:F1}%";
                        }
                        else
                        {
                            result.State = EMySqlResultState.EXCEPTION;
                            result.Message = $"Không tìm thấy sản phẩm với ID: {sanPhamId}";
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = ex.Message;
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Xóa sản phẩm theo ID
        /// </summary>
        /// <param name="id">ID sản phẩm cần xóa</param>
        /// <returns>MySqlResultState</returns>
        public static async Task<MySqlResultState> DeleteAsync(int id)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("sp_tbSanPham_Delete", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@inId", MySqlDbType.Int32).Value = id;

                        await cmd.ExecuteNonQueryAsync();

                        result.State = EMySqlResultState.OK;
                        result.Message = $"Xóa sản phẩm thành công. ID: {id}";
                    }
                }
            }
            catch (MySqlException ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = ex.Message;
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Convert MySqlDataReader row sang đối tượng SanPham
        /// </summary>
        /// <param name="rdr">MySqlDataReader</param>
        /// <returns>SanPham object</returns>
        private static SanPham ConvertRowFromDataReader(MySqlDataReader rdr)
        {
            SanPham sanPham = new SanPham
            {
                Id = MyMySql.GetInt32(rdr, "Id"),
                Code = MyMySql.GetString(rdr, "Code"),
                Barcode = MyMySql.GetString(rdr, "Barcode"),
                Name = MyMySql.GetString(rdr, "Name"),
                ShortName = MyMySql.GetString(rdr, "ShortName"),
                ComboId = MyMySql.GetInt32(rdr, "ComboId"),
                CategoryId = MyMySql.GetInt32(rdr, "CategoryId"),
                BookCoverPrice = MyMySql.GetInt32(rdr, "BookCoverPrice"),
                Author = MyMySql.GetString(rdr, "Author"),
                Translator = MyMySql.GetString(rdr, "Translator"),
                PublisherId = MyMySql.GetInt32(rdr, "PublisherId"),
                PublishingCompany = MyMySql.GetString(rdr, "PublishingCompany"),
                PublishingTime = MyMySql.GetInt32(rdr, "PublishingTime"),
                ProductLong = MyMySql.GetInt32(rdr, "ProductLong"),
                ProductWide = MyMySql.GetInt32(rdr, "ProductWide"),
                ProductHigh = MyMySql.GetInt32(rdr, "ProductHigh"),
                ProductWeight = MyMySql.GetInt32(rdr, "ProductWeight"),
                PositionInWarehouse = MyMySql.GetString(rdr, "PositionInWarehouse"),
                HardCover = MyMySql.GetInt32(rdr, "HardCover"),
                MinAge = MyMySql.GetInt32(rdr, "MinAge"),
                MaxAge = MyMySql.GetInt32(rdr, "MaxAge"),
                ParentId = MyMySql.GetInt32(rdr, "ParentId"),
                Republish = MyMySql.GetInt32(rdr, "Republish"),
                Detail = MyMySql.GetString(rdr, "Detail"),
                Status = MyMySql.GetInt32(rdr, "Status"),
                Quantity = MyMySql.GetInt32(rdr, "Quantity"),
                PageNumber = MyMySql.GetInt32(rdr, "PageNumber"),
                Discount = rdr.IsDBNull(rdr.GetOrdinal("Discount")) ? 0 : rdr.GetFloat(rdr.GetOrdinal("Discount")),
                SalePrice = MyMySql.GetInt32(rdr, "SalePrice"),
                Language = MyMySql.GetString(rdr, "Language"),
                Date = MyMySql.GetDateTime(rdr, "Date"),
                SoldQuantity = MyMySql.GetInt32(rdr, "SoldQuantity"),
                URL = MyMySql.GetString(rdr, "URL"),
                SEOKeyword = MyMySql.GetString(rdr, "SEOKeyword")
            };

            return sanPham;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm cùng ComboId (dùng làm variants/phân loại)
        /// </summary>
        /// <param name="comboId">ComboId</param>
        /// <returns>Danh sách sản phẩm cùng combo, sắp xếp theo Id ASC</returns>
        public static async Task<List<SanPham>> GetListByComboIdAsync(int comboId)
        {
            List<SanPham> list = new List<SanPham>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(
                        "SELECT * FROM tb_san_pham WHERE ComboId = @inComboId AND Status = 0 ORDER BY Id ASC", conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@inComboId", MySqlDbType.Int32).Value = comboId;

                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                list.Add(ConvertRowFromDataReader(rdr));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                list.Clear();
            }

            return list;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm cùng ComboId (variants) trong 1 query
        /// Tối ưu performance bằng cách gọi stored procedure
        /// Sản phẩm chính (với id truyền vào) sẽ nằm trong list variants
        /// Sản phẩm chính sẽ có Mappings và MediaList được load đầy đủ, các sản phẩm khác chỉ load thông tin cơ bản
        /// </summary>
        /// <param name="id">ID sản phẩm</param>
        /// <returns>
        /// Danh sách sản phẩm cùng combo (bao gồm cả sản phẩm chính).
        /// Trả về list rỗng nếu không tìm thấy.
        /// </returns>
        public static async Task<List<SanPham>> GetSanPhamWithVariantsAsync(int id)
        {
            List<SanPham> variants = new List<SanPham>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("sp_tbSanPham_GetSanPhamWithVariants", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@inId", MySqlDbType.Int32).Value = id;

                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                variants.Add(ConvertRowFromDataReader(rdr));
                            }
                        }
                    }
                    if(variants.Count == 0)
                    {
                        return variants;
                    }

                    if (variants[0].CategoryId > 0)
                    {
                        // Lấy tên của các category và publisher cho tất cả sản phẩm trong variants
                        // Chỉ cần lấy của 1 sản phẩm trong variants, vì tất cả sản phẩm cùng combo sẽ có cùng category
                        string categoryName = string.Empty;
                        string publisherName = string.Empty;


                        using (MySqlCommand cmd = new MySqlCommand(
                            "SELECT Name FROM webplaywithme.tbcategory WHERE Id = @inId;", conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Add("@inId", MySqlDbType.Int32).Value = variants[0].CategoryId;

                            using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                            {
                                while (await rdr.ReadAsync())
                                {
                                    categoryName = MyMySql.GetString(rdr, "Name");
                                }
                            }
                        }

                        using (MySqlCommand cmd = new MySqlCommand(
                            "SELECT Name FROM webplaywithme.tbpublisher WHERE Id = @inId;", conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Add("@inId", MySqlDbType.Int32).Value = variants[0].PublisherId;

                            using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                            {
                                while (await rdr.ReadAsync())
                                {
                                    publisherName = MyMySql.GetString(rdr, "Name");
                                }
                            }
                        }

                        // Gán tên category và publisher cho tất cả sản phẩm trong variants
                        foreach (var sanPham in variants)
                        {
                            sanPham.CategoryName = categoryName;
                            sanPham.PublisherName = publisherName;
                        }
                    }

                    // Lấy metadata
                    if (variants.Count > 0)
                    {
                        foreach (var sanPham in variants)
                        {
                            if (sanPham.Id == id)
                            {
                                sanPham.MediaList = await SanPhamMediaMySql.GetListBySanPhamId_ConnectOutAsync(sanPham.Id, conn);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return variants;
        }

        #region Update Quantity After Sale

        /// <summary>
        /// [TRANSACTION] Trừ số lượng sản phẩm sau khi bán (dùng trong transaction)
        /// </summary>
        /// <param name="conn">MySqlConnection đã mở</param>
        /// <param name="transaction">MySqlTransaction hiện tại</param>
        /// <param name="items">Danh sách (productId, quantity) đã mua</param>
        /// <returns>MySqlResultState</returns>
        public static async Task<MySqlResultState> UpdateQuantityAfterSaleTransactionAsync(
            MySqlConnection conn,
            MySqlTransaction transaction,
            List<(int productId, int quantity)> items)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                string updateQuery = "UPDATE tb_san_pham SET Quantity = Quantity - @quantity WHERE Id = @sanPhamId";

                using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn, transaction))
                {
                    cmd.Parameters.Add("@sanPhamId", MySqlDbType.Int32);
                    cmd.Parameters.Add("@quantity", MySqlDbType.Int32);

                    foreach (var (productId, quantity) in items)
                    {
                        cmd.Parameters["@sanPhamId"].Value = productId;
                        cmd.Parameters["@quantity"].Value = quantity;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                result.State = EMySqlResultState.OK;
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return result;
        }

        /// <summary>
        /// Trừ số lượng sản phẩm sau khi bán (tự mở connection, không dùng transaction)
        /// </summary>
        /// <param name="items">Danh sách (productId, quantity) đã mua</param>
        /// <returns>MySqlResultState</returns>
        public static async Task<MySqlResultState> UpdateQuantityAfterSaleAsync(
            List<(int productId, int quantity)> items)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string updateQuery = "UPDATE tb_san_pham SET Quantity = Quantity - @quantity WHERE Id = @sanPhamId";

                    using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.Add("@sanPhamId", MySqlDbType.Int32);
                        cmd.Parameters.Add("@quantity", MySqlDbType.Int32);

                        foreach (var (productId, quantity) in items)
                        {
                            cmd.Parameters["@sanPhamId"].Value = productId;
                            cmd.Parameters["@quantity"].Value = quantity;
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    result.State = EMySqlResultState.OK;
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return result;
        }

        #endregion
    }
}
