using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.ItemModel;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web.UI;
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
                        cmd.Parameters.Add("@inHardCover", MySqlDbType.Byte).Value = (SByte)sanPham.HardCover;
                        cmd.Parameters.Add("@inMinAge", MySqlDbType.Int32).Value = sanPham.MinAge ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inMaxAge", MySqlDbType.Int32).Value = sanPham.MaxAge ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inParentId", MySqlDbType.Int32).Value = sanPham.ParentId ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inRepublish", MySqlDbType.Int32).Value = sanPham.Republish ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inDetail", MySqlDbType.VarChar).Value = sanPham.Detail ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inStatus", MySqlDbType.Byte).Value = (SByte)sanPham.Status;
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
                        cmd.Parameters.Add("@inHardCover", MySqlDbType.Byte).Value = (SByte)sanPham.HardCover;
                        cmd.Parameters.Add("@inMinAge", MySqlDbType.Int32).Value = sanPham.MinAge ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inMaxAge", MySqlDbType.Int32).Value = sanPham.MaxAge ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inParentId", MySqlDbType.Int32).Value = sanPham.ParentId ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inRepublish", MySqlDbType.Int32).Value = sanPham.Republish ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inDetail", MySqlDbType.VarChar).Value = sanPham.Detail ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inStatus", MySqlDbType.Byte).Value = (SByte)sanPham.Status;
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

                        List<SanPham> list = await ExecuteReaderByIndexAsync(cmd);
                        if (list != null && list.Count > 0)
                        {
                            sanPham = list[0];
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
        /// Lấy danh sách sản phẩm theo ComboId
        /// </summary>
        public static async Task<List<SanPham>> GetByComboIdAsync(int comboId)
        {
            List<SanPham> list = new List<SanPham>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(
                        "SELECT * FROM tb_san_pham WHERE ComboId = @inComboId ORDER BY Id ASC", conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@inComboId", MySqlDbType.Int32).Value = comboId;

                        list = await ExecuteReaderByIndexAsync(cmd);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return list;
        }

        public static async Task<List<int>> GetIdsOnlyByComboIdAsync(int comboId)
        {
            List<int> list = new List<int>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(
                        "SELECT Id FROM tb_san_pham WHERE ComboId = @inComboId ORDER BY Id ASC", conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@inComboId", MySqlDbType.Int32).Value = comboId;

                        using(MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            int idIndex = rdr.GetOrdinal("Id");
                            while (rdr.Read())
                            {
                                list.Add(MyMySql.GetInt32(rdr, idIndex));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return list;
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
            List<SanPham> list = null;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(
                        "SELECT * FROM tb_san_pham ORDER BY Id DESC", conn))
                    {
                        cmd.CommandType = CommandType.Text;

                        list = await ExecuteReaderByIndexAsync(cmd);
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
                        cmd.Parameters.Add("@inHardCover", MySqlDbType.Byte).Value = (SByte)sanPham.HardCover;
                        cmd.Parameters.Add("@inMinAge", MySqlDbType.Int32).Value = sanPham.MinAge ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inMaxAge", MySqlDbType.Int32).Value = sanPham.MaxAge ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inParentId", MySqlDbType.Int32).Value = sanPham.ParentId ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inRepublish", MySqlDbType.Int32).Value = sanPham.Republish ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inDetail", MySqlDbType.VarChar).Value = sanPham.Detail ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@inStatus", MySqlDbType.Byte).Value = (SByte)sanPham.Status;
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

        public static async Task<MySqlResultState> UpdateStatusAsync(int sanPhamId, ESanPhamStatus status)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = "UPDATE tb_san_pham SET Status = @status WHERE Id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@status", MySqlDbType.Byte).Value = (SByte)status;
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = sanPhamId;

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
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
                HardCover = (ESanPhamCoverType)MyMySql.GetSByte(rdr, "HardCover"),
                MinAge = MyMySql.GetInt32(rdr, "MinAge"),
                MaxAge = MyMySql.GetInt32(rdr, "MaxAge"),
                ParentId = MyMySql.GetInt32(rdr, "ParentId"),
                Republish = MyMySql.GetInt32(rdr, "Republish"),
                Detail = MyMySql.GetString(rdr, "Detail"),
                Status = (ESanPhamStatus)MyMySql.GetSByte(rdr, "Status"),
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
            List<SanPham> list = null;

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

                        list = await ExecuteReaderByIndexAsync(cmd);
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
            List<SanPham> list = null;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("sp_tbSanPham_GetSanPhamWithVariants", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@inId", MySqlDbType.Int32).Value = id;

                        list = await ExecuteReaderByIndexAsync(cmd, true);
                    }
                    if(list.Count == 0)
                    {
                        return list;
                    }

                    // Lấy metadata
                    if (list.Count > 0)
                    {
                        foreach (var sanPham in list)
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

            return list;
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

        /// <summary>
        /// Execute MySqlCommand và đọc kết quả theo INDEX
        /// GetOrdinal() 1 lần để lấy index theo tên cột, sau đó dùng index để đọc nhiều rows
        /// Tối ưu performance khi đọc nhiều rows (GetOrdinal chỉ gọi 1 lần thay vì mỗi row)
        /// </summary>
        /// <param name="cmd">MySqlCommand đã được setup (query + parameters)</param>
        /// <returns>Danh sách SanPham</returns>
        public static async Task<List<SanPham>> ExecuteReaderByIndexAsync(MySqlCommand cmd,
            Boolean readComboPublisherCategoryName = false)
        {
            List<SanPham> list = new List<SanPham>();

            try
            {
                using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                {
                    // Lấy index của các cột 1 lần duy nhất (GetOrdinal)
                    int idxId = rdr.GetOrdinal("Id");
                    int idxCode = rdr.GetOrdinal("Code");
                    int idxBarcode = rdr.GetOrdinal("Barcode");
                    int idxName = rdr.GetOrdinal("Name");
                    int idxShortName = rdr.GetOrdinal("ShortName");
                    int idxComboId = rdr.GetOrdinal("ComboId");
                    int idxCategoryId = rdr.GetOrdinal("CategoryId");
                    int idxBookCoverPrice = rdr.GetOrdinal("BookCoverPrice");
                    int idxAuthor = rdr.GetOrdinal("Author");
                    int idxTranslator = rdr.GetOrdinal("Translator");
                    int idxPublisherId = rdr.GetOrdinal("PublisherId");
                    int idxPublishingCompany = rdr.GetOrdinal("PublishingCompany");
                    int idxPublishingTime = rdr.GetOrdinal("PublishingTime");
                    int idxProductLong = rdr.GetOrdinal("ProductLong");
                    int idxProductWide = rdr.GetOrdinal("ProductWide");
                    int idxProductHigh = rdr.GetOrdinal("ProductHigh");
                    int idxProductWeight = rdr.GetOrdinal("ProductWeight");
                    int idxPositionInWarehouse = rdr.GetOrdinal("PositionInWarehouse");
                    int idxHardCover = rdr.GetOrdinal("HardCover");
                    int idxMinAge = rdr.GetOrdinal("MinAge");
                    int idxMaxAge = rdr.GetOrdinal("MaxAge");
                    int idxParentId = rdr.GetOrdinal("ParentId");
                    int idxRepublish = rdr.GetOrdinal("Republish");
                    int idxDetail = rdr.GetOrdinal("Detail");
                    int idxStatus = rdr.GetOrdinal("Status");
                    int idxQuantity = rdr.GetOrdinal("Quantity");
                    int idxPageNumber = rdr.GetOrdinal("PageNumber");
                    int idxDiscount = rdr.GetOrdinal("Discount");
                    int idxSalePrice = rdr.GetOrdinal("SalePrice");
                    int idxLanguage = rdr.GetOrdinal("Language");
                    int idxDate = rdr.GetOrdinal("Date");
                    int idxSoldQuantity = rdr.GetOrdinal("SoldQuantity");
                    int idxURL = rdr.GetOrdinal("URL");
                    int idxSEOKeyword = rdr.GetOrdinal("SEOKeyword");
                    int idxComboName = 0;
                    int idxPublisherName = 0;
                    int idxCategoryName = 0;
                    if (readComboPublisherCategoryName)
                    {
                        idxComboName = rdr.GetOrdinal("ComboName");
                        idxPublisherName = rdr.GetOrdinal("PublisherName");
                        idxCategoryName = rdr.GetOrdinal("CategoryName");
                    }    

                    // Đọc từng row bằng index (nhanh hơn lookup theo tên mỗi lần)
                    while (await rdr.ReadAsync())
                    {
                        SanPham sanPham = new SanPham
                        {
                            Id = rdr.GetInt32(idxId),
                            Code = rdr.IsDBNull(idxCode) ? null : rdr.GetString(idxCode),
                            Barcode = rdr.IsDBNull(idxBarcode) ? null : rdr.GetString(idxBarcode),
                            Name = rdr.IsDBNull(idxName) ? null : rdr.GetString(idxName),
                            ShortName = rdr.IsDBNull(idxShortName) ? null : rdr.GetString(idxShortName),
                            ComboId = rdr.IsDBNull(idxComboId) ? (int?)null : rdr.GetInt32(idxComboId),
                            CategoryId = rdr.IsDBNull(idxCategoryId) ? (int?)null : rdr.GetInt32(idxCategoryId),
                            BookCoverPrice = rdr.GetInt32(idxBookCoverPrice),
                            Author = rdr.IsDBNull(idxAuthor) ? null : rdr.GetString(idxAuthor),
                            Translator = rdr.IsDBNull(idxTranslator) ? null : rdr.GetString(idxTranslator),
                            PublisherId = rdr.IsDBNull(idxPublisherId) ? (int?)null : rdr.GetInt32(idxPublisherId),
                            PublishingCompany = rdr.IsDBNull(idxPublishingCompany) ? null : rdr.GetString(idxPublishingCompany),
                            PublishingTime = rdr.IsDBNull(idxPublishingTime) ? (int?)null : rdr.GetInt32(idxPublishingTime),
                            ProductLong = rdr.GetInt32(idxProductLong),
                            ProductWide = rdr.GetInt32(idxProductWide),
                            ProductHigh = rdr.GetInt32(idxProductHigh),
                            ProductWeight = rdr.GetInt32(idxProductWeight),
                            PositionInWarehouse = rdr.IsDBNull(idxPositionInWarehouse) ? null : rdr.GetString(idxPositionInWarehouse),
                            HardCover = (ESanPhamCoverType)(rdr.IsDBNull(idxHardCover) ? (sbyte)0 : rdr.GetSByte(idxHardCover)),
                            MinAge = rdr.IsDBNull(idxMinAge) ? (int?)null : rdr.GetInt32(idxMinAge),
                            MaxAge = rdr.IsDBNull(idxMaxAge) ? (int?)null : rdr.GetInt32(idxMaxAge),
                            ParentId = rdr.IsDBNull(idxParentId) ? (int?)null : rdr.GetInt32(idxParentId),
                            Republish = rdr.IsDBNull(idxRepublish) ? (int?)null : rdr.GetInt32(idxRepublish),
                            Detail = rdr.IsDBNull(idxDetail) ? null : rdr.GetString(idxDetail),
                            Status = (ESanPhamStatus)(rdr.IsDBNull(idxStatus) ? (sbyte)0 : rdr.GetSByte(idxStatus)),
                            Quantity = rdr.GetInt32(idxQuantity),
                            PageNumber = rdr.IsDBNull(idxPageNumber) ? (int?)null : rdr.GetInt32(idxPageNumber),
                            Discount = rdr.IsDBNull(idxDiscount) ? 0 : rdr.GetFloat(idxDiscount),
                            SalePrice = rdr.GetInt32(idxSalePrice),
                            Language = rdr.IsDBNull(idxLanguage) ? null : rdr.GetString(idxLanguage),
                            Date = rdr.IsDBNull(idxDate) ? (DateTime?)null : rdr.GetDateTime(idxDate),
                            SoldQuantity = rdr.IsDBNull(idxSoldQuantity) ? (int?)null : rdr.GetInt32(idxSoldQuantity),
                            URL = rdr.IsDBNull(idxURL) ? null : rdr.GetString(idxURL),
                            SEOKeyword = rdr.IsDBNull(idxSEOKeyword) ? null : rdr.GetString(idxSEOKeyword)
                        };
                        if (readComboPublisherCategoryName)
                        {
                            sanPham.ComboName = rdr.IsDBNull(idxComboName) ? null : rdr.GetString(idxComboName);
                            sanPham.PublisherName = rdr.IsDBNull(idxPublisherName) ? null : rdr.GetString(idxPublisherName);
                            sanPham.CategoryName = rdr.IsDBNull(idxCategoryName) ? null : rdr.GetString(idxCategoryName);
                        }

                        list.Add(sanPham);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"ExecuteReaderByIndexAsync error: {ex.Message}");
                list.Clear();
            }

            return list;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm cho trang Search - chỉ các cột cần thiết
        /// Không lấy Detail, Author, Translator, dimensions, v.v. để tối ưu performance
        /// </summary>
        /// <returns>Danh sách SanPhamSearchInfo</returns>
        public static async Task<List<AdminSanPhamSearchInfo>> GetAllForSearchAsync()
        {
            List<AdminSanPhamSearchInfo> list = new List<AdminSanPhamSearchInfo>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    // SELECT chỉ các cột cần thiết
                    string query = @"
                        SELECT
                            Id, Code, Barcode, Name, ShortName,
                            ComboId, CategoryId, PublisherId,
                            BookCoverPrice, Discount, SalePrice,
                            Quantity, Status
                        FROM tb_san_pham
                        ORDER BY ComboId DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;

                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            // Lấy index các cột 1 lần
                            int idxId = rdr.GetOrdinal("Id");
                            int idxCode = rdr.GetOrdinal("Code");
                            int idxBarcode = rdr.GetOrdinal("Barcode");
                            int idxName = rdr.GetOrdinal("Name");
                            int idxShortName = rdr.GetOrdinal("ShortName");
                            int idxComboId = rdr.GetOrdinal("ComboId");
                            int idxCategoryId = rdr.GetOrdinal("CategoryId");
                            int idxPublisherId = rdr.GetOrdinal("PublisherId");
                            int idxBookCoverPrice = rdr.GetOrdinal("BookCoverPrice");
                            int idxDiscount = rdr.GetOrdinal("Discount");
                            int idxSalePrice = rdr.GetOrdinal("SalePrice");
                            int idxQuantity = rdr.GetOrdinal("Quantity");
                            int idxStatus = rdr.GetOrdinal("Status");

                            // Đọc từng row
                            while (await rdr.ReadAsync())
                            {
                                AdminSanPhamSearchInfo info = new AdminSanPhamSearchInfo
                                {
                                    Id = rdr.GetInt32(idxId),
                                    Code = rdr.IsDBNull(idxCode) ? null : rdr.GetString(idxCode),
                                    Barcode = rdr.IsDBNull(idxBarcode) ? null : rdr.GetString(idxBarcode),
                                    Name = rdr.IsDBNull(idxName) ? null : rdr.GetString(idxName),
                                    ShortName = rdr.IsDBNull(idxShortName) ? null : rdr.GetString(idxShortName),
                                    ComboId = rdr.IsDBNull(idxComboId) ? (int?)null : rdr.GetInt32(idxComboId),
                                    CategoryId = rdr.IsDBNull(idxCategoryId) ? (int?)null : rdr.GetInt32(idxCategoryId),
                                    PublisherId = rdr.IsDBNull(idxPublisherId) ? (int?)null : rdr.GetInt32(idxPublisherId),
                                    BookCoverPrice = rdr.GetInt32(idxBookCoverPrice),
                                    Discount = rdr.IsDBNull(idxDiscount) ? 0 : rdr.GetFloat(idxDiscount),
                                    SalePrice = rdr.GetInt32(idxSalePrice),
                                    Quantity = rdr.GetInt32(idxQuantity),
                                    Status = rdr.GetSByte(idxStatus)
                                };

                                list.Add(info);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetAllForSearchAsync error: {ex.Message}");
                list.Clear();
            }

            return list;
        }

        /// <summary>
        /// Search sản phẩm với keyset pagination (Load More pattern)
        /// Dùng sp.Id < @lastId ORDER BY DESC để cursor-based pagination
        /// Fetch limit+1 items để check hasMore
        /// </summary>
        public static async Task<(List<SanPhamBasicInfo> items, bool hasMore)> SearchSanPhamWithCursorAsync(
            SanPhamSearchParameter searchParameter,
            MySqlConnection conn)
        {
            List<SanPhamBasicInfo> ls = new List<SanPhamBasicInfo>();
            bool hasMore = false;

            // Keyset pagination
            int lastId;
            int limit;

            if (searchParameter.page.HasValue && searchParameter.page.Value > 0)
            {
                // Page mode: Load tất cả items từ page 1 đến page N
                lastId = 0;
                limit = SanPhamSearchParameter.itemsPerPage * searchParameter.page.Value; // Page 1: 30, Page 2: 60, Page 3: 90, ...
            }
            else
            {
                // Keyset mode: Load More với lastId cursor
                lastId = searchParameter.lastId ?? 0;
                limit = searchParameter.limit ?? SanPhamSearchParameter.itemsPerPage;
            }

            try
            {
                string sql = @"
                    SELECT
                        sp.Id,
                        sp.Name,
                        sp.ShortName,
                        sp.BookCoverPrice,
                        sp.SalePrice,
                        sp.Quantity,
                        sp.Status,
                        media.FileName AS CoverImageFileName,
                        media.AltText AS CoverImageAltText,
                        media.Title AS CoverImageTitle
                    FROM tb_san_pham sp
                    LEFT JOIN tbcategory cat ON sp.CategoryId = cat.Id
                    LEFT JOIN tbpublisher pub ON sp.PublisherId = pub.Id
                    LEFT JOIN LATERAL (
                        SELECT FileName, AltText, Title
                        FROM tb_san_pham_media
                        WHERE SanPhamId = sp.Id
                          AND MediaType = 'image'
                        ORDER BY DisplayOrder ASC
                        LIMIT 1
                    ) media ON true
                    WHERE sp.Status = 0";

                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;

                    // Add filters
                    if (!string.IsNullOrEmpty(searchParameter.name))
                    {
                        sql += " AND sp.Name LIKE @inNamePara";
                        cmd.Parameters.Add("@inNamePara", MySqlDbType.VarChar).Value = "%" + searchParameter.name + "%";
                    }
                    if (!string.IsNullOrEmpty(searchParameter.author))
                    {
                        sql += " AND sp.Author = @inAuthor";
                        cmd.Parameters.Add("@inAuthor", MySqlDbType.VarChar).Value = searchParameter.author;
                    }
                    if (!string.IsNullOrEmpty(searchParameter.translator))
                    {
                        sql += " AND sp.Translator = @inTranslator";
                        cmd.Parameters.Add("@inTranslator", MySqlDbType.VarChar).Value = searchParameter.translator;
                    }
                    if (!string.IsNullOrEmpty(searchParameter.category))
                    {
                        sql += " AND cat.Name = @inCategory";
                        cmd.Parameters.Add("@inCategory", MySqlDbType.VarChar).Value = searchParameter.category;
                    }
                    if (!string.IsNullOrEmpty(searchParameter.publishingCompany))
                    {
                        sql += " AND sp.PublishingCompany = @inPublishingCompany";
                        cmd.Parameters.Add("@inPublishingCompany", MySqlDbType.VarChar).Value = searchParameter.publishingCompany;
                    }
                    if (!string.IsNullOrEmpty(searchParameter.publisher))
                    {
                        sql += " AND pub.Name = @inPublisher";
                        cmd.Parameters.Add("@inPublisher", MySqlDbType.VarChar).Value = searchParameter.publisher;
                    }

                    // Keyset cursor: sp.Id < @lastId (vì ORDER BY DESC)
                    if (lastId > 0)
                    {
                        sql += " AND sp.Id < @inLastId";
                        cmd.Parameters.Add("@inLastId", MySqlDbType.Int32).Value = lastId;
                    }

                    // Fetch limit+1 để check hasMore
                    sql += " ORDER BY sp.Id DESC LIMIT @inLimit";
                    cmd.Parameters.Add("@inLimit", MySqlDbType.Int32).Value = limit + 1;

                    cmd.CommandText = sql;

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        // Cache column ordinals (1 lần)
                        int ordId = rdr.GetOrdinal("Id");
                        int ordName = rdr.GetOrdinal("Name");
                        int ordShortName = rdr.GetOrdinal("ShortName");
                        int ordBookCoverPrice = rdr.GetOrdinal("BookCoverPrice");
                        int ordSalePrice = rdr.GetOrdinal("SalePrice");
                        int ordQuantity = rdr.GetOrdinal("Quantity");
                        int ordStatus = rdr.GetOrdinal("Status");
                        int ordCoverImageFileName = rdr.GetOrdinal("CoverImageFileName");
                        int ordCoverImageAltText = rdr.GetOrdinal("CoverImageAltText");
                        int ordCoverImageTitle = rdr.GetOrdinal("CoverImageTitle");

                        while (await rdr.ReadAsync())
                        {
                            SanPhamBasicInfo info = new SanPhamBasicInfo
                            {
                                Id = rdr.GetInt32(ordId),                      // Index access
                                Name = MyMySql.GetString(rdr, ordName),        // Index access
                                ShortName = MyMySql.GetString(rdr, ordShortName),
                                BookCoverPrice = MyMySql.GetInt32(rdr, ordBookCoverPrice),
                                SalePrice = MyMySql.GetInt32(rdr, ordSalePrice),
                                Quantity = MyMySql.GetInt32(rdr, ordQuantity),
                                Status = MyMySql.GetInt32(rdr, ordStatus),
                                CoverImageFileName = MyMySql.GetString(rdr, ordCoverImageFileName),
                                CoverImageAltText = MyMySql.GetString(rdr, ordCoverImageAltText),
                                CoverImageTitle = MyMySql.GetString(rdr, ordCoverImageTitle)
                            };
                            ls.Add(info);
                        }
                    }
                }

                // Check hasMore: nếu fetch được limit+1 items → còn items phía sau
                if (ls.Count > limit)
                {
                    hasMore = true;
                    ls.RemoveAt(ls.Count - 1); // Xóa item thừa
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                ls.Clear();
                hasMore = false;
            }

            return (ls, hasMore);
        }
    }
}
