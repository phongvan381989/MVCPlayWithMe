using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.ProductModel;
using MVCPlayWithMe.Models.SanPhamModel;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MVCPlayWithMe.General.Common;

namespace MVCPlayWithMe.Controllers.ProductSupporting
{
    public class VBNProductSupporting
    {
        // Tương tự GetListNeedUpdateQuantity của sàn và thêm cập nhật trạng thái PWM = 0 ở tbneedupdatequantity
        public static async Task VBNGetListNeedUpdateQuantityAndUpdateAsync(MySqlConnection conn)
        {
            // Cập nhật số lượng cho voibenho
            List<int> vbnProductIdList =
                await ProductMySql.GetListProductOfNeedUpdateQuantityConnectOutAsync(conn, EECommerceType.PLAY_WITH_ME);
            if (vbnProductIdList.Count > 0)
            {
                List<SanPham> listSP = await SanPhamMySql.GetListNeedUpdateQuantityConnectOutAsync(conn);
                if (listSP.Count == 0)
                {
                    return;
                }

                List<SanPham> failedSPs = await SanPhamMySql.BulkUpdateQuantityAsync(conn, listSP);

                // Danh sách sản phẩm kho chưa cập nhật số lượng thành công
                List<int> listProductIdUpdateFail = new List<int>();

                foreach (var sp in failedSPs)
                {
                    foreach (var mapping in sp.Mappings)
                    {
                        if (vbnProductIdList.Contains(mapping.SanPhamKhoId) &&
                            !listProductIdUpdateFail.Contains(mapping.SanPhamKhoId))
                        {
                            listProductIdUpdateFail.Add(mapping.SanPhamKhoId);
                        }
                    }
                }

                // Danh sách sản phẩm cập nhật thành công
                List<int> listProductIdUpdateSuccess = new List<int>();
                foreach (var id in vbnProductIdList)
                {
                    if (!listProductIdUpdateFail.Contains(id))
                    {
                        listProductIdUpdateSuccess.Add(id);
                    }
                }

                await ProductMySql.UpdateZeroStatusOfNeedUpdateQuantityConnectOutAsync(listProductIdUpdateSuccess, EECommerceType.PLAY_WITH_ME, conn);
            }
        }

        // Cập nhật số lượng sản phẩm của 1 model từ itemId, modelId
        public static async Task<MySqlResultState> VBNUpdateQuantityOfOneItemModelAsync(int spId,
            MySqlConnection conn)
        {
            MySqlResultState result = await SanPhamMySql.UpdateQuantityAsync(spId);

            return result;
        }
    }
}
