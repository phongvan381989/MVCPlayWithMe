using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.ProductModel;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.Controllers.ProductSupporting;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using static MVCPlayWithMe.General.Common;

namespace MVCPlayWithMe.Controllers.OpenPlatform
{
    public class NeedUpdateController : BasicController
    {
        // GET: NeedUpdate
        public async Task<ActionResult> Index()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }
            return View();
        }

        /// <summary>
        /// Cập nhật giá sản phẩm có thay đổi lên sàn
        /// </summary>
        /// <param name="eType">ALL, TIKI, SHOPEE, LAZADA, PLAYWITHME</param>
        [HttpPost]
        public async Task<string> UpdateChangedPrice(string eType)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            // TODO: User sẽ implement logic
            MySqlResultState result = new MySqlResultState();
            result.State = EMySqlResultState.OK;
            result.Message = $"Chức năng đang được phát triển (Sàn: {eType})";
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    // Danh sách sản phẩm trong kho có thay đổi số lượng cần cập nhật
                    List<int> tikiProductIdList =
                await ProductMySql.GetListProductOfNeedUpdateQuantityConnectOutAsync(conn, EECommerceType.TIKI);
                    if (tikiProductIdList.Count > 0)
                    {
                        List<CommonItem> tikiList = await TikiProductSuporting.TikiGetListNeedUpdateQuantityAndUpdateAsync(conn);
                        await CommonProductSupporting.UpdateZeroStatusOfNeedUpdateQuantityConnectOutAsync(EECommerceType.TIKI, tikiProductIdList, tikiList, conn);
                    }
                }
                catch(Exception ex)
                {
                    Common.SetResultException(ex, result);
                }
            }

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Cập nhật giá toàn bộ sản phẩm lên sàn
        /// </summary>
        /// <param name="eType">ALL, TIKI, SHOPEE, LAZADA, PLAYWITHME</param>
        [HttpPost]
        public async Task<string> UpdateAllPrice(string eType)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            // TODO: User sẽ implement logic
            MySqlResultState result = new MySqlResultState();
            result.State = EMySqlResultState.OK;
            result.Message = $"Chức năng đang được phát triển (Sàn: {eType})";

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Cập nhật tồn kho sản phẩm có thay đổi lên sàn
        /// </summary>
        /// <param name="eType">ALL, TIKI, SHOPEE, LAZADA, PLAYWITHME</param>
        [HttpPost]
        public async Task<string> UpdateChangedStock(string eType)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            // TODO: User sẽ implement logic
            MySqlResultState result = new MySqlResultState();
            result.State = EMySqlResultState.OK;
            result.Message = $"Chức năng đang được phát triển (Sàn: {eType})";

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Cập nhật tồn kho toàn bộ sản phẩm lên sàn
        /// </summary>
        /// <param name="eType">ALL, TIKI, SHOPEE, LAZADA, PLAYWITHME</param>
        [HttpPost]
        public async Task<string> UpdateAllStock(string eType)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            // TODO: User sẽ implement logic
            MySqlResultState result = new MySqlResultState();
            result.State = EMySqlResultState.OK;
            result.Message = $"Chức năng đang được phát triển (Sàn: {eType})";

            return JsonConvert.SerializeObject(result);
        }
    }
}
