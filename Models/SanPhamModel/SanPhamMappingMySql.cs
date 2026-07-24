using MVCPlayWithMe.General;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MVCPlayWithMe.Models.SanPhamModel
{
    public class SanPhamMappingMySql
    {
        /// <summary>
        /// Lấy danh sách mapping của 1 sản phẩm bán (có JOIN với tbproducts)
        /// </summary>
        public static async Task<List<SanPhamMapping>> GetListBySanPhamBanIdAsync(int sanPhamBanId)
        {
            List<SanPhamMapping> list = new List<SanPhamMapping>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    list = await GetListBySanPhamBanId_ConnectOutAsync(sanPhamBanId, conn);
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    list.Clear();
                }
            }
            return list;
        }

        /// <summary>
        /// Lấy danh sách mapping của 1 sản phẩm bán (có JOIN với tbproducts)
        /// </summary>
        public static async Task<List<SanPhamMapping>> GetListBySanPhamBanId_ConnectOutAsync(int sanPhamBanId, MySqlConnection conn)
        {
            List<SanPhamMapping> list = new List<SanPhamMapping>();
            try
            {
                MySqlCommand cmd = new MySqlCommand(@"
                    SELECT m.Id, m.SanPhamBanId, m.SanPhamKhoId, m.Quantity,
                            p.Code AS SanPhamKhoCode, p.Name AS SanPhamKhoName,
                            p.Quantity AS SanPhamKhoQuantity, p.BookCoverPrice AS SanPhamKhoBookCoverPrice,
                            p.Discount AS SanPhamKhoDiscount
                    FROM tb_san_pham_mapping m
                    LEFT JOIN tbproducts p ON m.SanPhamKhoId = p.Id
                    WHERE m.SanPhamBanId = @sanPhamBanId
                    ORDER BY m.Id", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@sanPhamBanId", sanPhamBanId);

                using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                {
                    while (await rdr.ReadAsync())
                    {
                        list.Add(new SanPhamMapping
                        {
                            Id = MyMySql.GetInt32(rdr, "Id"),
                            SanPhamBanId = MyMySql.GetInt32(rdr, "SanPhamBanId"),
                            SanPhamKhoId = MyMySql.GetInt32(rdr, "SanPhamKhoId"),
                            Quantity = MyMySql.GetInt32(rdr, "Quantity"),
                            SanPhamKhoCode = MyMySql.GetString(rdr, "SanPhamKhoCode"),
                            SanPhamKhoName = MyMySql.GetString(rdr, "SanPhamKhoName"),
                            SanPhamKhoQuantity = MyMySql.GetInt32(rdr, "SanPhamKhoQuantity"),
                            SanPhamKhoBookCoverPrice = MyMySql.GetInt32(rdr, "SanPhamKhoBookCoverPrice"),
                            SanPhamKhoDiscount = (float)rdr.GetDouble(rdr.GetOrdinal("SanPhamKhoDiscount"))
                        });
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
        /// Insert mapping mới
        /// </summary>
        public static async Task<MySqlResultState> InsertAsync(SanPhamMapping mapping)
        {
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand(@"
                        INSERT INTO tb_san_pham_mapping
                        (SanPhamBanId, SanPhamKhoId, Quantity)
                        VALUES (@sanPhamBanId, @sanPhamKhoId, @quantity)",
                        conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@sanPhamBanId", mapping.SanPhamBanId);
                    cmd.Parameters.AddWithValue("@sanPhamKhoId", mapping.SanPhamKhoId);
                    cmd.Parameters.AddWithValue("@quantity", mapping.Quantity);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        return new MySqlResultState(EMySqlResultState.OK, "Thêm mapping thành công");
                    }
                    return new MySqlResultState(EMySqlResultState.ERROR, "Thêm mapping thất bại");
                }
                catch (MySqlException ex) when (ex.Number == 1062) // Duplicate key
                {
                    return new MySqlResultState(EMySqlResultState.ERROR, "Sản phẩm kho này đã được map rồi");
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    return new MySqlResultState(EMySqlResultState.ERROR, ex.Message);
                }
            }
        }

        /// <summary>
        /// Update số lượng của mapping
        /// </summary>
        public static async Task<MySqlResultState> UpdateQuantityAsync(int mappingId, int quantity)
        {
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand(@"
                        UPDATE tb_san_pham_mapping
                        SET Quantity = @quantity
                        WHERE Id = @id",
                        conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", mappingId);
                    cmd.Parameters.AddWithValue("@quantity", quantity);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        return new MySqlResultState(EMySqlResultState.OK, "Cập nhật thành công");
                    }
                    return new MySqlResultState(EMySqlResultState.ERROR, "Không tìm thấy mapping");
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    return new MySqlResultState(EMySqlResultState.ERROR, ex.Message);
                }
            }
        }

        /// <summary>
        /// Delete mapping theo ID
        /// </summary>
        public static async Task<MySqlResultState> DeleteAsync(int mappingId)
        {
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand(
                        "DELETE FROM tb_san_pham_mapping WHERE Id = @id",
                        conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", mappingId);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        return new MySqlResultState(EMySqlResultState.OK, "Xóa mapping thành công");
                    }
                    return new MySqlResultState(EMySqlResultState.ERROR, "Không tìm thấy mapping");
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    return new MySqlResultState(EMySqlResultState.ERROR, ex.Message);
                }
            }
        }

        /// <summary>
        /// Delete tất cả mapping của 1 sản phẩm bán (khi xóa sản phẩm)
        /// </summary>
        public static async Task<MySqlResultState> DeleteBySanPhamBanIdAsync(int sanPhamBanId)
        {
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand(
                        "DELETE FROM tb_san_pham_mapping WHERE SanPhamBanId = @sanPhamBanId",
                        conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@sanPhamBanId", sanPhamBanId);

                    await cmd.ExecuteNonQueryAsync();
                    return new MySqlResultState(EMySqlResultState.OK, "Xóa mapping thành công");
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    return new MySqlResultState(EMySqlResultState.ERROR, ex.Message);
                }
            }
        }

        /// <summary>
        /// Lấy tất cả sản phẩm kho (tbproducts) - dùng cho table với filtering
        /// </summary>
        public static async Task<List<TbProduct>> GetAllTbProductsAsync()
        {
            List<TbProduct> list = new List<TbProduct>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand(@"
                        SELECT Id, Code, Barcode, Name, Quantity, ComboId
                        FROM tbproducts
                        ORDER BY Name",
                        conn);
                    cmd.CommandType = CommandType.Text;

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            list.Add(new TbProduct
                            {
                                Id = MyMySql.GetInt32(rdr, "Id"),
                                Code = MyMySql.GetString(rdr, "Code"),
                                Barcode = MyMySql.GetString(rdr, "Barcode"),
                                Name = MyMySql.GetString(rdr, "Name"),
                                Quantity = MyMySql.GetInt32(rdr, "Quantity"),
                                ComboId = MyMySql.GetInt32(rdr, "ComboId")
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    list.Clear();
                }
            }
            return list;
        }

        /// <summary>
        /// Search sản phẩm kho (tbproducts) theo keyword - dùng cho autocomplete
        /// </summary>
        public static async Task<List<TbProduct>> SearchTbProductsAsync(string keyword, int limit = 20)
        {
            List<TbProduct> list = new List<TbProduct>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand(@"
                        SELECT Id, Code, Barcode, Name, Quantity
                        FROM tbproducts
                        WHERE (Name LIKE @keyword OR Code LIKE @keyword OR Barcode LIKE @keyword)
                        ORDER BY Name
                        LIMIT @limit",
                        conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                    cmd.Parameters.AddWithValue("@limit", limit);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            list.Add(new TbProduct
                            {
                                Id = MyMySql.GetInt32(rdr, "Id"),
                                Code = MyMySql.GetString(rdr, "Code"),
                                Barcode = MyMySql.GetString(rdr, "Barcode"),
                                Name = MyMySql.GetString(rdr, "Name"),
                                Quantity = MyMySql.GetInt32(rdr, "Quantity")
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    list.Clear();
                }
            }
            return list;
        }

        /// <summary>
        /// Chép dữ liệu từ sản phẩm kho (tbproducts) sang sản phẩm bán (tb_san_pham)
        /// Giữ nguyên: Id, SoldQuantity, URL, SEOKeyword
        /// Copy: tất cả fields còn lại (bao gồm Quantity, Date)
        /// </summary>
        public static async Task<MySqlResultState> CopyDataFromKhoProductAsync(int sanPhamBanId, int sanPhamKhoId)
        {
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();

                    // Load data từ tbproducts (sản phẩm kho)
                    MySqlCommand cmdSelect = new MySqlCommand(@"
                        SELECT Code, Barcode, Name, ComboId, CategoryId, BookCoverPrice,
                               Author, Translator, PublisherId, PublishingCompany, PublishingTime,
                               ProductLong, ProductWide, ProductHigh, ProductWeight,
                               PositionInWarehouse, HardCover, MinAge, MaxAge,
                               ParentId, Republish, Detail, Status, PageNumber,
                               Discount, Language, Quantity, Date
                        FROM tbproducts
                        WHERE Id = @sanPhamKhoId",
                        conn);
                    cmdSelect.CommandType = CommandType.Text;
                    cmdSelect.Parameters.AddWithValue("@sanPhamKhoId", sanPhamKhoId);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmdSelect.ExecuteReaderAsync())
                    {
                        if (!await rdr.ReadAsync())
                        {
                            return new MySqlResultState(EMySqlResultState.ERROR, "Không tìm thấy sản phẩm kho");
                        }

                        // Đóng reader trước khi thực hiện UPDATE
                        string code = MyMySql.GetString(rdr, "Code");
                        string barcode = MyMySql.GetString(rdr, "Barcode");
                        string name = MyMySql.GetString(rdr, "Name");
                        int comboId = MyMySql.GetInt32(rdr, "ComboId");
                        int categoryId = MyMySql.GetInt32(rdr, "CategoryId");
                        int bookCoverPrice = MyMySql.GetInt32(rdr, "BookCoverPrice");
                        string author = MyMySql.GetString(rdr, "Author");
                        string translator = MyMySql.GetString(rdr, "Translator");
                        int publisherId = MyMySql.GetInt32(rdr, "PublisherId");
                        string publishingCompany = MyMySql.GetString(rdr, "PublishingCompany");
                        int publishingTime = MyMySql.GetInt32(rdr, "PublishingTime");
                        int productLong = MyMySql.GetInt32(rdr, "ProductLong");
                        int productWide = MyMySql.GetInt32(rdr, "ProductWide");
                        int productHigh = MyMySql.GetInt32(rdr, "ProductHigh");
                        int productWeight = MyMySql.GetInt32(rdr, "ProductWeight");
                        string positionInWarehouse = MyMySql.GetString(rdr, "PositionInWarehouse");
                        int? hardCover = rdr.IsDBNull(rdr.GetOrdinal("HardCover")) ? (int?)null : MyMySql.GetInt32(rdr, "HardCover");
                        int? minAge = rdr.IsDBNull(rdr.GetOrdinal("MinAge")) ? (int?)null : MyMySql.GetInt32(rdr, "MinAge");
                        int? maxAge = rdr.IsDBNull(rdr.GetOrdinal("MaxAge")) ? (int?)null : MyMySql.GetInt32(rdr, "MaxAge");
                        int? parentId = rdr.IsDBNull(rdr.GetOrdinal("ParentId")) ? (int?)null : MyMySql.GetInt32(rdr, "ParentId");
                        int? republish = rdr.IsDBNull(rdr.GetOrdinal("Republish")) ? (int?)null : MyMySql.GetInt32(rdr, "Republish");
                        string detail = MyMySql.GetString(rdr, "Detail");
                        int status = MyMySql.GetInt32(rdr, "Status");
                        int? pageNumber = rdr.IsDBNull(rdr.GetOrdinal("PageNumber")) ? (int?)null : MyMySql.GetInt32(rdr, "PageNumber");
                        float discount = (float)rdr.GetDouble(rdr.GetOrdinal("Discount"));
                        string language = MyMySql.GetString(rdr, "Language");
                        int quantity = MyMySql.GetInt32(rdr, "Quantity");
                        DateTime? date = rdr.IsDBNull(rdr.GetOrdinal("Date")) ? (DateTime?)null : MyMySql.GetDateTime(rdr, "Date");

                        rdr.Close();

                        // Update vào tb_san_pham (giữ nguyên Id, SoldQuantity, URL, SEOKeyword)
                        MySqlCommand cmdUpdate = new MySqlCommand(@"
                            UPDATE tb_san_pham SET
                                Code = @code,
                                Barcode = @barcode,
                                Name = @name,
                                ComboId = @comboId,
                                CategoryId = @categoryId,
                                BookCoverPrice = @bookCoverPrice,
                                Author = @author,
                                Translator = @translator,
                                PublisherId = @publisherId,
                                PublishingCompany = @publishingCompany,
                                PublishingTime = @publishingTime,
                                ProductLong = @productLong,
                                ProductWide = @productWide,
                                ProductHigh = @productHigh,
                                ProductWeight = @productWeight,
                                PositionInWarehouse = @positionInWarehouse,
                                HardCover = @hardCover,
                                MinAge = @minAge,
                                MaxAge = @maxAge,
                                ParentId = @parentId,
                                Republish = @republish,
                                Detail = @detail,
                                Status = @status,
                                PageNumber = @pageNumber,
                                Discount = @discount,
                                Language = @language,
                                Quantity = @quantity,
                                Date = @date
                            WHERE Id = @sanPhamBanId",
                            conn);
                        cmdUpdate.CommandType = CommandType.Text;
                        cmdUpdate.Parameters.AddWithValue("@sanPhamBanId", sanPhamBanId);
                        cmdUpdate.Parameters.AddWithValue("@code", code ?? (object)DBNull.Value);
                        cmdUpdate.Parameters.AddWithValue("@barcode", barcode ?? (object)DBNull.Value);
                        cmdUpdate.Parameters.AddWithValue("@name", name);
                        cmdUpdate.Parameters.AddWithValue("@comboId", comboId);
                        cmdUpdate.Parameters.AddWithValue("@categoryId", categoryId);
                        cmdUpdate.Parameters.AddWithValue("@bookCoverPrice", bookCoverPrice);
                        cmdUpdate.Parameters.AddWithValue("@author", author ?? (object)DBNull.Value);
                        cmdUpdate.Parameters.AddWithValue("@translator", translator ?? (object)DBNull.Value);
                        cmdUpdate.Parameters.AddWithValue("@publisherId", publisherId);
                        cmdUpdate.Parameters.AddWithValue("@publishingCompany", publishingCompany ?? (object)DBNull.Value);
                        cmdUpdate.Parameters.AddWithValue("@publishingTime", publishingTime);
                        cmdUpdate.Parameters.AddWithValue("@productLong", productLong);
                        cmdUpdate.Parameters.AddWithValue("@productWide", productWide);
                        cmdUpdate.Parameters.AddWithValue("@productHigh", productHigh);
                        cmdUpdate.Parameters.AddWithValue("@productWeight", productWeight);
                        cmdUpdate.Parameters.AddWithValue("@positionInWarehouse", positionInWarehouse ?? (object)DBNull.Value);
                        cmdUpdate.Parameters.AddWithValue("@hardCover", hardCover ?? (object)DBNull.Value);
                        cmdUpdate.Parameters.AddWithValue("@minAge", minAge ?? (object)DBNull.Value);
                        cmdUpdate.Parameters.AddWithValue("@maxAge", maxAge ?? (object)DBNull.Value);
                        cmdUpdate.Parameters.AddWithValue("@parentId", parentId ?? (object)DBNull.Value);
                        cmdUpdate.Parameters.AddWithValue("@republish", republish ?? (object)DBNull.Value);
                        cmdUpdate.Parameters.AddWithValue("@detail", detail ?? (object)DBNull.Value);
                        cmdUpdate.Parameters.AddWithValue("@status", status);
                        cmdUpdate.Parameters.AddWithValue("@pageNumber", pageNumber ?? (object)DBNull.Value);
                        cmdUpdate.Parameters.AddWithValue("@discount", discount);
                        cmdUpdate.Parameters.AddWithValue("@language", language ?? (object)DBNull.Value);
                        cmdUpdate.Parameters.AddWithValue("@quantity", quantity);
                        cmdUpdate.Parameters.AddWithValue("@date", date ?? (object)DBNull.Value);

                        int rowsAffected = await cmdUpdate.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new MySqlResultState(EMySqlResultState.OK, "Chép dữ liệu thành công");
                        }
                        return new MySqlResultState(EMySqlResultState.ERROR, "Không tìm thấy sản phẩm bán để cập nhật");
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    return new MySqlResultState(EMySqlResultState.ERROR, ex.Message);
                }
            }
        }
    }
}
