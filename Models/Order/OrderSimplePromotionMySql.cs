using MVCPlayWithMe.General;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MVCPlayWithMe.Models.Order
{
    /// <summary>
    /// Repository class cho tborder_simple_promotion
    /// Tất cả methods đều là static (stateless data access)
    /// </summary>
    public class OrderSimplePromotionMySql
    {
        /// <summary>
        /// Đọc 1 row promotion từ MySqlDataReader (dùng column index)
        /// </summary>
        private static OrderSimplePromotion ReadPromotionFromReader(MySqlDataReader rdr,
            int idIndex, int nameIndex, int minOrderValueIndex, int statusIndex,
            int typeIndex, int discountIndex, int discountTypeIndex, int timeIndex, int descriptionIndex)
        {
            return new OrderSimplePromotion
            {
                Id = MyMySql.GetInt32(rdr, idIndex),
                Name = MyMySql.GetString(rdr, nameIndex),
                MinOrderValue = MyMySql.GetInt32(rdr, minOrderValueIndex),
                Status = MyMySql.GetSByte(rdr, statusIndex),
                Type = MyMySql.GetSByte(rdr, typeIndex),
                Discount = MyMySql.GetInt32(rdr, discountIndex),
                DiscountType = MyMySql.GetSByte(rdr, discountTypeIndex),
                Time = MyMySql.GetDateTime(rdr, timeIndex),
                Description = MyMySql.GetString(rdr, descriptionIndex)
            };
        }

        /// <summary>
        /// Execute query và trả về danh sách promotion
        /// </summary>
        private static async Task<List<OrderSimplePromotion>> ExecuteQueryAsync(string query, string methodName)
        {
            List<OrderSimplePromotion> list = new List<OrderSimplePromotion>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            // Lấy column ordinal một lần duy nhất
                            int idIndex = rdr.GetOrdinal("Id");
                            int nameIndex = rdr.GetOrdinal("Name");
                            int minOrderValueIndex = rdr.GetOrdinal("MinOrderValue");
                            int statusIndex = rdr.GetOrdinal("Status");
                            int typeIndex = rdr.GetOrdinal("Type");
                            int discountIndex = rdr.GetOrdinal("Discount");
                            int discountTypeIndex = rdr.GetOrdinal("DiscountType");
                            int timeIndex = rdr.GetOrdinal("Time");
                            int descriptionIndex = rdr.GetOrdinal("Description");

                            while (await rdr.ReadAsync())
                            {
                                OrderSimplePromotion promo = ReadPromotionFromReader(rdr,
                                    idIndex, nameIndex, minOrderValueIndex, statusIndex,
                                    typeIndex, discountIndex, discountTypeIndex, timeIndex, descriptionIndex);
                                list.Add(promo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"{methodName} failed: {ex.Message}");
                list.Clear();
            }

            return list;
        }

        /// <summary>
        /// Lấy tất cả chương trình giảm giá
        /// </summary>
        /// <returns>Danh sách promotion</returns>
        public static async Task<List<OrderSimplePromotion>> GetAllPromotionsAsync()
        {
            return await ExecuteQueryAsync(
                "SELECT * FROM tborder_simple_promotion ORDER BY Id",
                nameof(GetAllPromotionsAsync));
        }

        /// <summary>
        /// Lấy tất cả chương trình giảm giá đang BẬT (Status = 0)
        /// </summary>
        /// <returns>Danh sách promotion đang hoạt động</returns>
        public static async Task<List<OrderSimplePromotion>> GetActivePromotionsAsync()
        {
            return await ExecuteQueryAsync(
                "SELECT * FROM tborder_simple_promotion WHERE Status = 0 ORDER BY MinOrderValue",
                nameof(GetActivePromotionsAsync));
        }

        /// <summary>
        /// Lấy chương trình giảm giá theo ID
        /// </summary>
        /// <param name="id">ID của promotion</param>
        /// <returns>Promotion hoặc null nếu không tìm thấy</returns>
        public static async Task<OrderSimplePromotion> GetPromotionByIdAsync(int id)
        {
            OrderSimplePromotion promo = null;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(
                        "SELECT * FROM tborder_simple_promotion WHERE Id = @Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);

                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            if (await rdr.ReadAsync())
                            {
                                // Lấy column ordinal
                                int idIndex = rdr.GetOrdinal("Id");
                                int nameIndex = rdr.GetOrdinal("Name");
                                int minOrderValueIndex = rdr.GetOrdinal("MinOrderValue");
                                int statusIndex = rdr.GetOrdinal("Status");
                                int typeIndex = rdr.GetOrdinal("Type");
                                int discountIndex = rdr.GetOrdinal("Discount");
                                int discountTypeIndex = rdr.GetOrdinal("DiscountType");
                                int timeIndex = rdr.GetOrdinal("Time");
                                int descriptionIndex = rdr.GetOrdinal("Description");

                                promo = ReadPromotionFromReader(rdr,
                                    idIndex, nameIndex, minOrderValueIndex, statusIndex,
                                    typeIndex, discountIndex, discountTypeIndex, timeIndex, descriptionIndex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetPromotionByIdAsync failed for Id={id}: {ex.Message}");
            }

            return promo;
        }

        /// <summary>
        /// Tính tổng giảm giá cho đơn hàng
        /// </summary>
        /// <param name="totalMoney">Tổng tiền hàng (tổng đơn giá × số lượng)</param>
        /// <param name="shipFee">Phí ship (để tính giảm Type 0)</param>
        /// <returns>Số tiền được giảm (VNĐ)</returns>
        public static async Task<int> CalculateTotalDiscountAsync(int totalMoney, int shipFee = 0)
        {
            // Tách riêng 2 loại giảm giá (giống client để dễ so sánh)
            int freeShipDiscount = 0;      // Giảm phí ship (Type = 0)
            int totalMoneyDiscount = 0;    // Giảm tổng tiền hàng (Type = 1)

            try
            {
                // Lấy tất cả promotion đang bật
                List<OrderSimplePromotion> promotions = await GetActivePromotionsAsync();

                foreach (var promo in promotions)
                {
                    if (promo.Type == 0)
                    {
                        // ===== TYPE 0: MIỄN PHÍ SHIP =====
                        // Điều kiện: totalMoney >= MinOrderValue (giống client)
                        // Giảm giá = shipFee (KHÔNG phải promo.Discount!)

                        if (totalMoney >= promo.MinOrderValue)
                        {
                            freeShipDiscount = shipFee; // ← Giảm bằng phí ship (giống client!)
                            break; // Chỉ áp dụng promotion đầu tiên thỏa điều kiện
                        }
                    }
                    else if (promo.Type == 1)
                    {
                        // ===== TYPE 1: GIẢM THEO BẬC 100K =====
                        // Điều kiện: totalMoney > MinOrderValue (STRICT >, giống client)
                        // Công thức: ((totalMoney - MinOrderValue) / 100,000 + 1) × Discount

                        if (totalMoney > promo.MinOrderValue)  // ← STRICT > (giống client!)
                        {
                            int extraAmount = totalMoney - promo.MinOrderValue;
                            int multiplier = (extraAmount / 100000) + 1;
                            totalMoneyDiscount = multiplier * promo.Discount;
                            break; // Chỉ áp dụng promotion đầu tiên thỏa điều kiện
                        }
                    }
                }

                // Log breakdown để dễ debug
                if (freeShipDiscount > 0 || totalMoneyDiscount > 0)
                {
                    MyLogger.GetInstance().Info($"Discount breakdown - Total: {totalMoney:N0}đ, ShipFee: {shipFee:N0}đ");
                    if (freeShipDiscount > 0)
                        MyLogger.GetInstance().Info($"  ✓ Free ship discount: {freeShipDiscount:N0}đ");
                    if (totalMoneyDiscount > 0)
                        MyLogger.GetInstance().Info($"  ✓ Total money discount: {totalMoneyDiscount:N0}đ");
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"CalculateTotalDiscountAsync failed: {ex.Message}");
            }

            // Tổng giảm giá = freeShipDiscount + totalMoneyDiscount (giống client)
            int totalDiscount = freeShipDiscount + totalMoneyDiscount;
            return totalDiscount;
        }

        /// <summary>
        /// Lấy mô tả các promotion áp dụng cho đơn hàng
        /// </summary>
        /// <param name="totalProductAmount">Tổng tiền hàng</param>
        /// <returns>Danh sách mô tả promotion</returns>
        public static async Task<List<string>> GetAppliedPromotionDescriptionsAsync(int totalProductAmount)
        {
            List<string> descriptions = new List<string>();

            try
            {
                List<OrderSimplePromotion> promotions = await GetActivePromotionsAsync();

                foreach (var promo in promotions)
                {
                    if (totalProductAmount >= promo.MinOrderValue)
                    {
                        if (!string.IsNullOrEmpty(promo.Description))
                        {
                            descriptions.Add(promo.Description);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetAppliedPromotionDescriptionsAsync failed: {ex.Message}");
            }

            return descriptions;
        }
    }
}
