namespace MVCPlayWithMe.Models.SanPhamModel
{
    /// <summary>
    /// Model cho mapping giữa sản phẩm bán (tb_san_pham) và sản phẩm kho (tbproducts)
    /// </summary>
    public class SanPhamMapping
    {
        public int Id { get; set; }
        public int SanPhamBanId { get; set; }
        public int SanPhamKhoId { get; set; }
        public int Quantity { get; set; }

        // Thông tin từ JOIN với tbproducts (dùng để hiển thị)
        public string SanPhamKhoCode { get; set; }
        public string SanPhamKhoName { get; set; }
        public int SanPhamKhoQuantity { get; set; }
    }

    /// <summary>
    /// Model đơn giản cho sản phẩm kho (tbproducts) - chỉ các field cần thiết
    /// </summary>
    public class TbProduct
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int ComboId { get; set; }
    }
}
