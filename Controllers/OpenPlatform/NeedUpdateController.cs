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
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    if (eType == Common.eTiki)
                    {
                        // Danh sách sản phẩm trong kho có thay đổi số lượng cần cập nhật
                        List<int> tikiProductIdList =
                        await ProductMySql.GetListProductOfNeedUpdatePriceConnectOutAsync(conn, EECommerceType.TIKI);

                        if (tikiProductIdList.Count > 0)
                        {
                            List<CommonItem> tikiList = await TikiProductSupporting.TikiGetListNeedUpdatePriceAndUpdate_BatchAsync(conn);
                            await CommonProductSupporting.UpdateZeroStatusOfNeedUpdatePrice_BatchConnectOutAsync(EECommerceType.TIKI, tikiProductIdList, tikiList, result, conn);
                        }
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
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    if (eType == Common.eTiki)
                    {
                        await TikiProductSupporting.TikiUpdatePriceAll_BatchAsync(conn);
                    }
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                }
            }
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

            return JsonConvert.SerializeObject(result);
        }
    }
}
