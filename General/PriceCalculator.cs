using MVCPlayWithMe.Models.SanPhamModel;
using MVCPlayWithMe.OpenPlatform.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVCPlayWithMe.General
{
    /// <summary>
    /// Helper class tính giá bán thực tế từ mapping sản phẩm kho + TaxAndFee
    /// </summary>
    public static class PriceCalculator
    {
        /// <summary>
        /// AI viết lại công thức tính giá bán thực tế theo CaculateSalePriceCore_Ver2:
        /// Lợi nhuận tính trên DOANH THU, không phải giá vốn
        /// Giá bán = Chi phí / (1 - Lợi nhuận% - Thuế% - Phí%)
        /// Tính giá bán thực tế cho sản phẩm dựa trên mapping và TaxAndFee
        /// </summary>
        /// <param name="mappings">Danh sách mapping sản phẩm kho</param>
        /// <param name="taxAndFee">Thông tin thuế phí của sàn</param>
        /// <returns>Giá bán thực tế (VND)</returns>
        public static int CalculateSalePrice(List<SanPhamMapping> mappings, TaxAndFee taxAndFee)
        {
            if (mappings == null || mappings.Count == 0)
            {
                return 0;
            }

            if (taxAndFee == null)
            {
                throw new ArgumentNullException(nameof(taxAndFee), "TaxAndFee không được null");
            }

            // BƯỚC 1: Tính tổng giá nhập từ tất cả sản phẩm kho
            decimal tongGiaNhap = 0;
            int bookCoverPriceSum = 0;
            foreach (var mapping in mappings)
            {
                // Giá nhập = BookCoverPrice * (1 - Discount)
                bookCoverPriceSum = bookCoverPriceSum + mapping.SanPhamKhoBookCoverPrice;
                decimal giaNhapMotSp = mapping.SanPhamKhoBookCoverPrice * (100 - (decimal)mapping.SanPhamKhoDiscount) / 100;

                // Nhân với số lượng trong mapping
                tongGiaNhap += giaNhapMotSp * mapping.Quantity;
            }

            // BƯỚC 2: Tính giá bán theo công thức Ver2
            // Ver2: Lợi nhuận tính trên DOANH THU, không phải giá vốn
            // Giá bán = Chi phí / (1 - Lợi nhuận% - Thuế% - Phí%)
            // SalePrice = (TotalCost + PackingCost) * 100 / (100 - ExpectedProfit - Tax - Fee)

            decimal tongChiPhi = tongGiaNhap + taxAndFee.packingCost;
            decimal salePrice = tongChiPhi * 100 / (100 - (decimal)taxAndFee.expectedPercentProfit - (decimal)taxAndFee.tax - (decimal)taxAndFee.fee);

            // Làm tròn xuống
            int salePriceInt = (int)Math.Floor(salePrice);

            // Làm tròn lên bội của 100 VND
            if (salePriceInt % 100 != 0)
            {
                salePriceInt = (salePriceInt / 100 + 1) * 100;
            }

            // Nếu giá bán thực tế >= giá bìa thì set giá bán thực tế = giá bìa
            if(salePriceInt > bookCoverPriceSum)
            {
                salePriceInt = bookCoverPriceSum;
            }

            return salePriceInt;
        }

        /// <summary>
        /// Tính giá bán tối thiểu (dùng minPercentProfit thay vì expectedPercentProfit)
        /// Ver2: Lợi nhuận tính trên DOANH THU
        /// </summary>
        public static int CalculateMinSalePrice(List<SanPhamMapping> mappings, TaxAndFee taxAndFee)
        {
            if (mappings == null || mappings.Count == 0)
            {
                return 0;
            }

            if (taxAndFee == null)
            {
                throw new ArgumentNullException(nameof(taxAndFee));
            }

            decimal tongGiaNhap = 0;

            foreach (var mapping in mappings)
            {
                decimal giaNhapMotSp = mapping.SanPhamKhoBookCoverPrice * (100 - (decimal)mapping.SanPhamKhoDiscount) / 100;
                tongGiaNhap += giaNhapMotSp * mapping.Quantity;
            }

            decimal tongChiPhi = tongGiaNhap + taxAndFee.packingCost;
            decimal salePrice = tongChiPhi * 100 / (100 - (decimal)taxAndFee.minPercentProfit - (decimal)taxAndFee.tax - (decimal)taxAndFee.fee);

            // Làm tròn xuống
            int salePriceInt = (int)Math.Floor(salePrice);

            // Làm tròn lên bội của 100 VND
            if (salePriceInt % 100 != 0)
            {
                salePriceInt = (salePriceInt / 100 + 1) * 100;
            }

            return salePriceInt;
        }

        /// <summary>
        /// Tính thông tin chi tiết về giá (cho debug/display)
        /// Ver2: Lợi nhuận, thuế, phí tính trên DOANH THU (giá bán), không phải giá vốn
        /// </summary>
        public static PriceBreakdown GetPriceBreakdown(List<SanPhamMapping> mappings, TaxAndFee taxAndFee)
        {
            if (mappings == null || mappings.Count == 0 || taxAndFee == null)
            {
                return null;
            }

            var breakdown = new PriceBreakdown();

            // Bước 1: Tính tổng giá nhập
            foreach (var mapping in mappings)
            {
                decimal giaNhap = mapping.SanPhamKhoBookCoverPrice * (100 - (decimal)mapping.SanPhamKhoDiscount) / 100;
                breakdown.TongGiaNhap += giaNhap * mapping.Quantity;
            }

            // Bước 2: Tổng chi phí
            breakdown.ChiPhiDongGoi = taxAndFee.packingCost;
            breakdown.TongChiPhi = breakdown.TongGiaNhap + breakdown.ChiPhiDongGoi;

            // Bước 3: Tính giá bán theo Ver2
            decimal salePrice = breakdown.TongChiPhi * 100 / (100 - (decimal)taxAndFee.expectedPercentProfit - (decimal)taxAndFee.tax - (decimal)taxAndFee.fee);

            int salePriceInt = (int)Math.Floor(salePrice);
            if (salePriceInt % 100 != 0)
            {
                salePriceInt = (salePriceInt / 100 + 1) * 100;
            }
            breakdown.GiaBanThucTe = salePriceInt;

            // Bước 4: Tính ngược lại lợi nhuận, thuế, phí từ giá bán
            // Ver2: Lợi nhuận = Giá bán × %
            breakdown.LoiNhuanMongMuon = breakdown.GiaBanThucTe * (decimal)taxAndFee.expectedPercentProfit / 100;
            breakdown.Thue = breakdown.GiaBanThucTe * (decimal)taxAndFee.tax / 100;
            breakdown.PhiSan = breakdown.GiaBanThucTe * (decimal)taxAndFee.fee / 100;

            // Giá trước thuế phí = Giá bán - Thuế - Phí
            breakdown.GiaTruocThuePhi = breakdown.GiaBanThucTe - breakdown.Thue - breakdown.PhiSan;

            return breakdown;
        }
    }

    /// <summary>
    /// Chi tiết phân tích giá (cho debug)
    /// </summary>
    public class PriceBreakdown
    {
        public decimal TongGiaNhap { get; set; }
        public decimal ChiPhiDongGoi { get; set; }
        public decimal TongChiPhi { get; set; }
        public decimal LoiNhuanMongMuon { get; set; }
        public decimal GiaTruocThuePhi { get; set; }
        public decimal Thue { get; set; }
        public decimal PhiSan { get; set; }
        public int GiaBanThucTe { get; set; }
    }
}
