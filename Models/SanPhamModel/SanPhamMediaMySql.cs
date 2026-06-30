using MVCPlayWithMe.General;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MVCPlayWithMe.Models.SanPhamModel
{
    public class SanPhamMediaMySql
    {
        /// <summary>
        /// Lấy danh sách media của 1 sản phẩm, sắp xếp theo DisplayOrder
        /// </summary>
        public static async Task<List<SanPhamMedia>> GetListBySanPhamIdAsync(int sanPhamId)
        {
            List<SanPhamMedia> list = new List<SanPhamMedia>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand(
                        "SELECT * FROM tb_san_pham_media WHERE SanPhamId = @sanPhamId ORDER BY DisplayOrder",
                        conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@sanPhamId", sanPhamId);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            list.Add(new SanPhamMedia
                            {
                                Id = MyMySql.GetInt32(rdr, "Id"),
                                SanPhamId = MyMySql.GetInt32(rdr, "SanPhamId"),
                                MediaType = MyMySql.GetString(rdr, "MediaType"),
                                FileName = MyMySql.GetString(rdr, "FileName"),
                                Title = MyMySql.GetString(rdr, "Title"),
                                AltText = MyMySql.GetString(rdr, "AltText"),
                                Description = MyMySql.GetString(rdr, "Description"),
                                PosterImage = MyMySql.GetString(rdr, "PosterImage"),
                                DisplayOrder = MyMySql.GetInt32(rdr, "DisplayOrder")
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
        /// Lấy 1 media theo ID
        /// </summary>
        public static async Task<SanPhamMedia> GetByIdAsync(int id)
        {
            SanPhamMedia media = null;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand(
                        "SELECT * FROM tb_san_pham_media WHERE Id = @id",
                        conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        if (await rdr.ReadAsync())
                        {
                            media = new SanPhamMedia
                            {
                                Id = MyMySql.GetInt32(rdr, "Id"),
                                SanPhamId = MyMySql.GetInt32(rdr, "SanPhamId"),
                                MediaType = MyMySql.GetString(rdr, "MediaType"),
                                FileName = MyMySql.GetString(rdr, "FileName"),
                                Title = MyMySql.GetString(rdr, "Title"),
                                AltText = MyMySql.GetString(rdr, "AltText"),
                                Description = MyMySql.GetString(rdr, "Description"),
                                PosterImage = MyMySql.GetString(rdr, "PosterImage"),
                                DisplayOrder = MyMySql.GetInt32(rdr, "DisplayOrder")
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                }
            }
            return media;
        }

        /// <summary>
        /// Insert media mới
        /// </summary>
        public static async Task<MySqlResultState> InsertAsync(SanPhamMedia media)
        {
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand(@"
                        INSERT INTO tb_san_pham_media
                        (SanPhamId, MediaType, FileName, Title, AltText, Description, PosterImage, DisplayOrder)
                        VALUES
                        (@sanPhamId, @mediaType, @fileName, @title, @altText, @description, @posterImage, @displayOrder)",
                        conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@sanPhamId", media.SanPhamId);
                    cmd.Parameters.AddWithValue("@mediaType", media.MediaType ?? "image");
                    cmd.Parameters.AddWithValue("@fileName", media.FileName);
                    cmd.Parameters.AddWithValue("@title", media.Title ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@altText", media.AltText ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@description", media.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@posterImage", media.PosterImage ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@displayOrder", media.DisplayOrder);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        return new MySqlResultState(EMySqlResultState.OK, "Insert thành công");
                    }
                    return new MySqlResultState(EMySqlResultState.ERROR, "Insert thất bại");
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    return new MySqlResultState(EMySqlResultState.ERROR, ex.Message);
                }
            }
        }

        /// <summary>
        /// Update media (chỉ update Title, AltText, Description, PosterImage, DisplayOrder)
        /// </summary>
        public static async Task<MySqlResultState> UpdateAsync(SanPhamMedia media)
        {
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand(@"
                        UPDATE tb_san_pham_media
                        SET Title = @title,
                            AltText = @altText,
                            Description = @description,
                            PosterImage = @posterImage,
                            DisplayOrder = @displayOrder
                        WHERE Id = @id",
                        conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", media.Id);
                    cmd.Parameters.AddWithValue("@title", media.Title ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@altText", media.AltText ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@description", media.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@posterImage", media.PosterImage ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@displayOrder", media.DisplayOrder);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        return new MySqlResultState(EMySqlResultState.OK, "Update thành công");
                    }
                    return new MySqlResultState(EMySqlResultState.ERROR, "Update thất bại - không tìm thấy record");
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    return new MySqlResultState(EMySqlResultState.ERROR, ex.Message);
                }
            }
        }

        /// <summary>
        /// Delete media theo ID
        /// </summary>
        public static async Task<MySqlResultState> DeleteAsync(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand(
                        "DELETE FROM tb_san_pham_media WHERE Id = @id",
                        conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", id);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        return new MySqlResultState(EMySqlResultState.OK, "Delete thành công");
                    }
                    return new MySqlResultState(EMySqlResultState.ERROR, "Delete thất bại - không tìm thấy record");
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    return new MySqlResultState(EMySqlResultState.ERROR, ex.Message);
                }
            }
        }

        /// <summary>
        /// Delete tất cả media của 1 sản phẩm
        /// </summary>
        public static async Task<MySqlResultState> DeleteBySanPhamIdAsync(int sanPhamId)
        {
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand(
                        "DELETE FROM tb_san_pham_media WHERE SanPhamId = @sanPhamId",
                        conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@sanPhamId", sanPhamId);

                    await cmd.ExecuteNonQueryAsync();
                    return new MySqlResultState(EMySqlResultState.OK, "Delete thành công");
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    return new MySqlResultState(EMySqlResultState.ERROR, ex.Message);
                }
            }
        }

        /// <summary>
        /// Update FileName khi rename file (đổi tên file trên disk)
        /// </summary>
        public static async Task<MySqlResultState> UpdateFileNameAsync(int sanPhamId, string oldFileName, string newFileName)
        {
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand(@"
                        UPDATE tb_san_pham_media
                        SET FileName = @newFileName
                        WHERE SanPhamId = @sanPhamId AND FileName = @oldFileName",
                        conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@sanPhamId", sanPhamId);
                    cmd.Parameters.AddWithValue("@oldFileName", oldFileName);
                    cmd.Parameters.AddWithValue("@newFileName", newFileName);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        return new MySqlResultState(EMySqlResultState.OK, $"Updated {rowsAffected} record(s)");
                    }
                    return new MySqlResultState(EMySqlResultState.OK, "No metadata record found (OK - file đổi tên nhưng chưa có metadata)");
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    return new MySqlResultState(EMySqlResultState.ERROR, ex.Message);
                }
            }
        }

        /// <summary>
        /// Delete media metadata theo FileName (khi xóa file trên disk)
        /// </summary>
        public static async Task<MySqlResultState> DeleteByFileNameAsync(int sanPhamId, string fileName)
        {
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand(
                        "DELETE FROM tb_san_pham_media WHERE SanPhamId = @sanPhamId AND FileName = @fileName",
                        conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@sanPhamId", sanPhamId);
                    cmd.Parameters.AddWithValue("@fileName", fileName);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        return new MySqlResultState(EMySqlResultState.OK, $"Deleted {rowsAffected} record(s)");
                    }
                    return new MySqlResultState(EMySqlResultState.OK, "No metadata record found (OK - file xóa nhưng chưa có metadata)");
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
