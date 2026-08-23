using MVCPlayWithMe.General;
using MySqlConnector;
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
                    list = await GetListBySanPhamId_ConnectOutAsync(sanPhamId, conn);
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
        /// Lấy danh sách media của 1 sản phẩm, sắp xếp theo DisplayOrder
        /// </summary>
        public static async Task<List<SanPhamMedia>> GetListBySanPhamId_ConnectOutAsync(int sanPhamId, MySqlConnection conn)
        {
            List<SanPhamMedia> list = new List<SanPhamMedia>();

            try
            {
                // ORDER BY MediaType DESC để đảm bảo rằng media type "video" sẽ được ưu tiên hiển thị trước media type "image" trong danh sách.
                using (MySqlCommand cmd = new MySqlCommand(
                    "SELECT * FROM tb_san_pham_media WHERE SanPhamId = @sanPhamId ORDER BY MediaType DESC, DisplayOrder ASC",
                    conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@sanPhamId", sanPhamId);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        list = await MapDataReaderToSanPhamMediaList(rdr);
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
        /// Lấy DisplayOrder lớn nhất của 1 sản phẩm
        /// Return -1 nếu sản phẩm chưa có media nào
        /// </summary>
        public static async Task<int> GetMaxDisplayOrderBySanPhamId(int sanPhamId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(
                        "SELECT COALESCE(MAX(DisplayOrder), 0) FROM tb_san_pham_media WHERE SanPhamId = @sanPhamId",
                        conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@sanPhamId", sanPhamId);

                        object result = await cmd.ExecuteScalarAsync();
                        return result != null ? Convert.ToInt32(result) : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                return -1;
            }
        }

        /// <summary>
        /// Lấy tất cả media của 1 combo id, sắp xếp theo DisplayOrder
        /// </summary>
        public static async Task<List<SanPhamMedia>> GetAllBySanPhamComboIdAsync(int comboId)
        {
            List<SanPhamMedia> list = new List<SanPhamMedia>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(
                        @"SELECT m.*
                        FROM tb_san_pham_media m
                        LEFT JOIN tb_san_pham sp ON m.SanPhamId = sp.Id
                        WHERE sp.ComboId = @comboId
                        AND (
                        (m.MediaType = 'image' AND m.DisplayOrder = 0) OR
                        (m.MediaType = 'video' AND m.DisplayOrder = 1));",
                        conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@comboId", comboId);

                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            list = await MapDataReaderToSanPhamMediaList(rdr);
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
        /// Insert media mới
        /// </summary>
        public static async Task<MySqlResultState> InsertAsync(SanPhamMedia media)
        {
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(@"
                        INSERT INTO tb_san_pham_media
                        (SanPhamId, MediaType, FileName, Title, AltText, Description, PosterImage, Width, Height, DisplayOrder)
                        VALUES
                        (@sanPhamId, @mediaType, @fileName, @title, @altText, @description, @posterImage, @width, @height, @displayOrder)",
                        conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@sanPhamId", MySqlDbType.Int32).Value = media.SanPhamId;
                        cmd.Parameters.Add("@mediaType", MySqlDbType.VarChar).Value = media.MediaType ?? "image";
                        cmd.Parameters.Add("@fileName", MySqlDbType.VarChar).Value = media.FileName;
                        cmd.Parameters.Add("@title", MySqlDbType.VarChar).Value = media.Title ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@altText", MySqlDbType.VarChar).Value = media.AltText ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@description", MySqlDbType.VarChar).Value = media.Description ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@posterImage", MySqlDbType.VarChar).Value = media.PosterImage ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@width", MySqlDbType.Int32).Value = media.Width;
                        cmd.Parameters.Add("@height", MySqlDbType.Int32).Value = media.Height;
                        cmd.Parameters.Add("@displayOrder", MySqlDbType.Int32).Value = media.DisplayOrder;

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new MySqlResultState(EMySqlResultState.OK, "Insert thành công");
                        }
                        return new MySqlResultState(EMySqlResultState.ERROR, "Insert thất bại");
                    }
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
                    using (MySqlCommand cmd = new MySqlCommand(@"
                        UPDATE tb_san_pham_media
                        SET Title = @title,
                            AltText = @altText,
                            Description = @description,
                            PosterImage = @posterImage,
                            DisplayOrder = @displayOrder
                        WHERE Id = @id",
                        conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = media.Id;
                        cmd.Parameters.Add("@title", MySqlDbType.VarChar).Value = media.Title ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@altText", MySqlDbType.VarChar).Value = media.AltText ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@description", MySqlDbType.VarChar).Value = media.Description ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@posterImage", MySqlDbType.VarChar).Value = media.PosterImage ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@displayOrder", MySqlDbType.Int32).Value = media.DisplayOrder;

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new MySqlResultState(EMySqlResultState.OK, "Update thành công");
                        }
                        return new MySqlResultState(EMySqlResultState.ERROR, "Update thất bại - không tìm thấy record");
                    }
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
        public static async Task<MySqlResultState> UpdateTextsAsync(SanPhamMedia media)
        {
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(@"
                        UPDATE tb_san_pham_media
                        SET Title = @title,
                            AltText = @altText,
                            Description = @description
                        WHERE Id = @id",
                        conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = media.Id;
                        cmd.Parameters.Add("@title", MySqlDbType.VarChar).Value = media.Title ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@altText", MySqlDbType.VarChar).Value = media.AltText ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@description", MySqlDbType.VarChar).Value = media.Description ?? (object)DBNull.Value;

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new MySqlResultState(EMySqlResultState.OK, "Update thành công");
                        }
                        return new MySqlResultState(EMySqlResultState.ERROR, "Update thất bại - không tìm thấy record");
                    }
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
                    using (MySqlCommand cmd = new MySqlCommand(
                        "DELETE FROM tb_san_pham_media WHERE Id = @id",
                        conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@id", id);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new MySqlResultState(EMySqlResultState.OK, "Delete thành công");
                        }
                        return new MySqlResultState(EMySqlResultState.ERROR, "Delete thất bại - không tìm thấy record");
                    }
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
                    using (MySqlCommand cmd = new MySqlCommand(
                        "DELETE FROM tb_san_pham_media WHERE SanPhamId = @sanPhamId",
                        conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@sanPhamId", sanPhamId);

                        await cmd.ExecuteNonQueryAsync();
                        return new MySqlResultState(EMySqlResultState.OK, "Delete thành công");
                    }
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
                    using (MySqlCommand cmd = new MySqlCommand(@"
                        UPDATE tb_san_pham_media
                        SET FileName = @newFileName
                        WHERE SanPhamId = @sanPhamId AND FileName = @oldFileName",
                        conn))
                    {
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
                    using (MySqlCommand cmd = new MySqlCommand(
                        "DELETE FROM tb_san_pham_media WHERE SanPhamId = @sanPhamId AND FileName = @fileName",
                        conn))
                    {
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
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    return new MySqlResultState(EMySqlResultState.ERROR, ex.Message);
                }
            }
        }

        /// <summary>
        /// Helper: Map MySqlDataReader sang List<SanPhamMedia>
        /// </summary>
        private static async Task<List<SanPhamMedia>> MapDataReaderToSanPhamMediaList(MySqlDataReader rdr)
        {
            List<SanPhamMedia> list = new List<SanPhamMedia>();

            int idIndex = rdr.GetOrdinal("Id");
            int sanPhamIdIndex = rdr.GetOrdinal("SanPhamId");
            int mediaTypeIndex = rdr.GetOrdinal("MediaType");
            int fileNameIndex = rdr.GetOrdinal("FileName");
            int titleIndex = rdr.GetOrdinal("Title");
            int altTextIndex = rdr.GetOrdinal("AltText");
            int descriptionIndex = rdr.GetOrdinal("Description");
            int posterImageIndex = rdr.GetOrdinal("PosterImage");
            int widthIndex = rdr.GetOrdinal("Width");
            int heightIndex = rdr.GetOrdinal("Height");
            int displayOrderIndex = rdr.GetOrdinal("DisplayOrder");

            while (await rdr.ReadAsync())
            {
                list.Add(new SanPhamMedia
                {
                    Id = MyMySql.GetInt32(rdr, idIndex),
                    SanPhamId = MyMySql.GetInt32(rdr, sanPhamIdIndex),
                    MediaType = MyMySql.GetString(rdr, mediaTypeIndex),
                    FileName = MyMySql.GetString(rdr, fileNameIndex),
                    Title = MyMySql.GetString(rdr, titleIndex),
                    AltText = MyMySql.GetString(rdr, altTextIndex),
                    Description = MyMySql.GetString(rdr, descriptionIndex),
                    PosterImage = MyMySql.GetString(rdr, posterImageIndex),
                    Width = (uint)MyMySql.GetInt32(rdr, widthIndex),
                    Height = (uint)MyMySql.GetInt32(rdr, heightIndex),
                    DisplayOrder = MyMySql.GetInt32(rdr, displayOrderIndex)
                });
            }

            return list;
        }
    }
}
