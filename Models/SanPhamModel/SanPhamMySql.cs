using MVCPlayWithMe.General;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MVCPlayWithMe.Models.SanPhamModel
{
    public class SanPhamMySql
    {
        /// <summary>
        /// Thêm mới sản phẩm vào bảng tb_san_pham
        /// </summary>
        /// <param name="sanPham">Đối tượng sản phẩm cần insert</param>
        /// <returns>MySqlResultState với State, Message và LastInsertedId</returns>
        public static MySqlResultState Insert(SanPham sanPham)
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
                        cmd.Parameters.AddWithValue("@inCode", sanPham.Code ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inBarcode", sanPham.Barcode ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inName", sanPham.Name);
                        cmd.Parameters.AddWithValue("@inShortName", sanPham.ShortName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inComboId", sanPham.ComboId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inCategoryId", sanPham.CategoryId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inBookCoverPrice", sanPham.BookCoverPrice);
                        cmd.Parameters.AddWithValue("@inAuthor", sanPham.Author ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inTranslator", sanPham.Translator ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inPublisherId", sanPham.PublisherId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inPublishingCompany", sanPham.PublishingCompany ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inPublishingTime", sanPham.PublishingTime ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inProductLong", sanPham.ProductLong);
                        cmd.Parameters.AddWithValue("@inProductWide", sanPham.ProductWide);
                        cmd.Parameters.AddWithValue("@inProductHigh", sanPham.ProductHigh);
                        cmd.Parameters.AddWithValue("@inProductWeight", sanPham.ProductWeight);
                        cmd.Parameters.AddWithValue("@inPositionInWarehouse", sanPham.PositionInWarehouse ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inHardCover", sanPham.HardCover ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inMinAge", sanPham.MinAge ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inMaxAge", sanPham.MaxAge ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inParentId", sanPham.ParentId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inRepublish", sanPham.Republish ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inDetail", sanPham.Detail ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inStatus", sanPham.Status);
                        cmd.Parameters.AddWithValue("@inQuantity", sanPham.Quantity);
                        cmd.Parameters.AddWithValue("@inPageNumber", sanPham.PageNumber ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inDiscount", sanPham.Discount);
                        cmd.Parameters.AddWithValue("@inLanguage", sanPham.Language ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inDate", sanPham.Date ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inSoldQuantity", sanPham.SoldQuantity ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inURL", sanPham.URL ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inSEOKeyword", sanPham.SEOKeyword ?? (object)DBNull.Value);

                        // Execute và đọc LastInsertId từ SELECT
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
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
                        cmd.Parameters.AddWithValue("@inCode", sanPham.Code ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inBarcode", sanPham.Barcode ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inName", sanPham.Name);
                        cmd.Parameters.AddWithValue("@inShortName", sanPham.ShortName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inComboId", sanPham.ComboId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inCategoryId", sanPham.CategoryId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inBookCoverPrice", sanPham.BookCoverPrice);
                        cmd.Parameters.AddWithValue("@inAuthor", sanPham.Author ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inTranslator", sanPham.Translator ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inPublisherId", sanPham.PublisherId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inPublishingCompany", sanPham.PublishingCompany ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inPublishingTime", sanPham.PublishingTime ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inProductLong", sanPham.ProductLong);
                        cmd.Parameters.AddWithValue("@inProductWide", sanPham.ProductWide);
                        cmd.Parameters.AddWithValue("@inProductHigh", sanPham.ProductHigh);
                        cmd.Parameters.AddWithValue("@inProductWeight", sanPham.ProductWeight);
                        cmd.Parameters.AddWithValue("@inPositionInWarehouse", sanPham.PositionInWarehouse ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inHardCover", sanPham.HardCover ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inMinAge", sanPham.MinAge ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inMaxAge", sanPham.MaxAge ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inParentId", sanPham.ParentId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inRepublish", sanPham.Republish ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inDetail", sanPham.Detail ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inStatus", sanPham.Status);
                        cmd.Parameters.AddWithValue("@inQuantity", sanPham.Quantity);
                        cmd.Parameters.AddWithValue("@inPageNumber", sanPham.PageNumber ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inDiscount", sanPham.Discount);
                        cmd.Parameters.AddWithValue("@inLanguage", sanPham.Language ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inDate", sanPham.Date ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inSoldQuantity", sanPham.SoldQuantity ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inURL", sanPham.URL ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inSEOKeyword", sanPham.SEOKeyword ?? (object)DBNull.Value);

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
                        cmd.Parameters.AddWithValue("@inId", id);

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

                        cmd.Parameters.AddWithValue("@inId", sanPham.Id);
                        cmd.Parameters.AddWithValue("@inCode", sanPham.Code ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inBarcode", sanPham.Barcode ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inName", sanPham.Name);
                        cmd.Parameters.AddWithValue("@inShortName", sanPham.ShortName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inComboId", sanPham.ComboId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inCategoryId", sanPham.CategoryId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inBookCoverPrice", sanPham.BookCoverPrice);
                        cmd.Parameters.AddWithValue("@inAuthor", sanPham.Author ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inTranslator", sanPham.Translator ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inPublisherId", sanPham.PublisherId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inPublishingCompany", sanPham.PublishingCompany ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inPublishingTime", sanPham.PublishingTime ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inProductLong", sanPham.ProductLong);
                        cmd.Parameters.AddWithValue("@inProductWide", sanPham.ProductWide);
                        cmd.Parameters.AddWithValue("@inProductHigh", sanPham.ProductHigh);
                        cmd.Parameters.AddWithValue("@inProductWeight", sanPham.ProductWeight);
                        cmd.Parameters.AddWithValue("@inPositionInWarehouse", sanPham.PositionInWarehouse ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inHardCover", sanPham.HardCover ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inMinAge", sanPham.MinAge ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inMaxAge", sanPham.MaxAge ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inParentId", sanPham.ParentId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inRepublish", sanPham.Republish ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inDetail", sanPham.Detail ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inStatus", sanPham.Status);
                        cmd.Parameters.AddWithValue("@inQuantity", sanPham.Quantity);
                        cmd.Parameters.AddWithValue("@inPageNumber", sanPham.PageNumber ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inDiscount", sanPham.Discount);
                        cmd.Parameters.AddWithValue("@inLanguage", sanPham.Language ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inDate", sanPham.Date ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inSoldQuantity", sanPham.SoldQuantity ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inURL", sanPham.URL ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@inSEOKeyword", sanPham.SEOKeyword ?? (object)DBNull.Value);

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

                        cmd.Parameters.AddWithValue("@inId", id);

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
                Language = MyMySql.GetString(rdr, "Language"),
                Date = MyMySql.GetDateTime(rdr, "Date"),
                SoldQuantity = MyMySql.GetInt32(rdr, "SoldQuantity"),
                URL = MyMySql.GetString(rdr, "URL"),
                SEOKeyword = MyMySql.GetString(rdr, "SEOKeyword")
            };

            return sanPham;
        }
    }
}
