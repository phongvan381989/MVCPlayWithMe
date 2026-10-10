using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.ProductModel;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Product;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using static MVCPlayWithMe.General.Common;

namespace MVCPlayWithMe.Controllers.OpenPlatform
{
    public class TikiOriginalSkuController : BasicController
    {
        // GET: TikiOriginalSku
        public async Task<ActionResult> Index()
        {
            return View();
        }

        /// <summary>
        /// Lấy tất cả tracking SKU, hoặc filter theo ECommerce
        /// </summary>
        [HttpPost]
        public async Task<string> GetAllTrackOriginalSku(int? eCommerce = null)
        {
            List<TrackOriginalSku> list = new List<TrackOriginalSku>();

            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(list);
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    if (eCommerce.HasValue && eCommerce.Value >= 0)
                    {
                        // Lấy theo sàn cụ thể
                        list = await TrackOriginalSkuMySql.GetTrackOriginalSkuByECommerceAsync(
                            (EECommerceType)eCommerce.Value,
                            conn);
                    }
                    else
                    {
                        // Lấy tất cả (cần viết thêm hàm GetAll nếu cần)
                        // Tạm thời lấy từng sàn
                        var tikiList = await TrackOriginalSkuMySql.GetTrackOriginalSkuByECommerceAsync(EECommerceType.TIKI, conn);
                        var shopeeList = await TrackOriginalSkuMySql.GetTrackOriginalSkuByECommerceAsync(EECommerceType.SHOPEE, conn);
                        var lazadaList = await TrackOriginalSkuMySql.GetTrackOriginalSkuByECommerceAsync(EECommerceType.LAZADA, conn);

                        list.AddRange(tikiList);
                        list.AddRange(shopeeList);
                        list.AddRange(lazadaList);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetAllTrackOriginalSku error: {ex.ToString()}");
            }

            return JsonConvert.SerializeObject(list);
        }

        /// <summary>
        /// Lấy tracking SKU theo ProductId
        /// </summary>
        [HttpPost]
        public async Task<string> GetByProductId(int productId)
        {
            List<TrackOriginalSku> list = new List<TrackOriginalSku>();

            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(list);
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    list = await TrackOriginalSkuMySql.GetTrackOriginalSkuByProductIdAsync(productId, conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetByProductId error: {ex.ToString()}");
            }

            return JsonConvert.SerializeObject(list);
        }

        /// <summary>
        /// Lấy tracking SKU theo OriginalSku và ECommerce
        /// </summary>
        [HttpPost]
        public async Task<string> GetBySku(string sku, int eCommerce)
        {
            List<TrackOriginalSku> list = new List<TrackOriginalSku>();

            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(list);
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    list = await TrackOriginalSkuMySql.GetTrackOriginalSkuBySkuAsync(
                        sku,
                        (EECommerceType)eCommerce,
                        conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetBySku error: {ex.ToString()}");
            }

            return JsonConvert.SerializeObject(list);
        }

        /// <summary>
        /// Xóa tracking SKU theo OriginalSku và ECommerce
        /// </summary>
        [HttpPost]
        public async Task<string> DeleteBySku(string sku, int eCommerce)
        {
            MySqlResultState result = new MySqlResultState();

            if ((await AuthentAdministratorAsync()) == null)
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = MySqlResultState.authenFailMessage;
                return JsonConvert.SerializeObject(result);
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    result = await TrackOriginalSkuMySql.DeleteTrackOriginalSkuAsync(
                        sku,
                        (EECommerceType)eCommerce,
                        conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"DeleteBySku error: {ex.ToString()}");
                result.State = EMySqlResultState.ERROR;
                result.Message = ex.Message;
            }

            return JsonConvert.SerializeObject(result);
        }

        private async Task TikiUpdateMapping_VariantSomething(string originalSku,
            MySqlResultState result,
            int productIdOnTiki,
            MySqlConnection conn)
        {
            List<TrackOriginalSku> list = await TrackOriginalSkuMySql.GetTrackOriginalSkuBySkuAsync(originalSku, EECommerceType.TIKI, conn);
            if (list == null || list.Count == 0)
            {
                result.State = EMySqlResultState.DONT_EXIST;
                result.Message = "Không tìm thấy original Sku";
                return;
            }

            TikiForUpdatingVariant tikiForUpdatingVariant = null;
            if (list[0].IsVariant == 1) // Là variant, ta cần cập nhật thông tin variant
            {
                tikiForUpdatingVariant = new TikiForUpdatingVariant();
                tikiForUpdatingVariant.product_id = productIdOnTiki;
                if (list.Count == 1) // Đây là sản phẩm lẻ không phải combo
                {
                    TrackOriginalSku track = list[0];
                    Product product = await ProductMySql.GetProductFromIdAsync(track.ProductId, conn);
                    if (product == null)
                    {
                        return;
                    }

                    tikiForUpdatingVariant.attributes.product_length = TikiCreateProduct.TikiGetStringOneDimension(product.productLong);
                    tikiForUpdatingVariant.attributes.product_width = TikiCreateProduct.TikiGetStringOneDimension(product.productWide);
                    tikiForUpdatingVariant.attributes.product_height = TikiCreateProduct.TikiGetStringOneDimension(product.productHigh);
                    tikiForUpdatingVariant.attributes.product_weight_kg = TikiCreateProduct.TikiGetStringWeight(product.productWeight);

                    // Không cập nhật sô trang, và dimensions vì dùng chung cho các biến thể
                    //if (product.pageNumber > 0)
                    //{
                    //    tikiForUpdatingVariant.attributes.number_of_page = product.pageNumber.ToString();
                    //}
                    //tikiForUpdatingVariant.attributes.dimensions =
                    //    TikiCreateProduct.TikiGetStringDimensions(product.productLong, product.productWide, product.productHigh);

                    if (product.imageSrc.Count > 0)
                    {
                        tikiForUpdatingVariant.image = Common.httpsVoiBeNho + product.imageSrc[0];
                        for (int i = 1; i < product.imageSrc.Count; i++)
                        {
                            tikiForUpdatingVariant.images.Add(Common.httpsVoiBeNho + product.imageSrc[i]);
                        }
                    }
                }
                else // Đây là sản phẩm combo
                {
                    TrackOriginalSku track = list[0];
                    Product product = await ProductMySql.GetProductFromIdAsync(track.ProductId, conn);
                    if (product == null)
                    {
                        return;
                    }
                    Combo combo = await ComboMySql.GetComboAsync(product.comboId);
                    combo.SetSrcImageVideo();

                    tikiForUpdatingVariant.attributes.product_length = TikiCreateProduct.TikiGetStringOneDimension(product.productLong);
                    tikiForUpdatingVariant.attributes.product_width = TikiCreateProduct.TikiGetStringOneDimension(product.productWide);
                    tikiForUpdatingVariant.attributes.product_height = TikiCreateProduct.TikiGetStringOneDimension(combo.products.Sum(p => p.productHigh));
                    tikiForUpdatingVariant.attributes.product_weight_kg = TikiCreateProduct.TikiGetStringWeight(combo.products.Sum(p => p.productWeight));

                    // Không cập nhật sô trang, và dimensions vì dùng chung cho các biến thể
                    //int sumPageNumber = combo.products.Sum(p => p.pageNumber);
                    //if (sumPageNumber > 0)
                    //{
                    //    tikiForUpdatingVariant.attributes.number_of_page = sumPageNumber.ToString();
                    //}
                    //tikiForUpdatingVariant.attributes.dimensions =
                    //    TikiCreateProduct.TikiGetStringDimensions(product.productLong, product.productWide, combo.products.Sum(p => p.productHigh));

                    if (combo.imageSrc.Count > 0)
                    {
                        tikiForUpdatingVariant.image = Common.httpsVoiBeNho + combo.imageSrc[0];
                        for (int i = 1; i < combo.imageSrc.Count; i++)
                        {
                            tikiForUpdatingVariant.images.Add(Common.httpsVoiBeNho + combo.imageSrc[i]);
                        }
                    }
                }
            }
            if(tikiForUpdatingVariant != null)
            {
                // Gửi yêu cầu cập nhật variant
                await TikiUpdateStock.TikiProductUpdateSomethingForVariant(tikiForUpdatingVariant, result);
            }
        }

        // Từ Original SKU lấy productId trên sàn Tiki và lưu vào tb_track_original_sku
        private async Task TikiGetEcoProIdFromOriginalSku(string originalSku,
            MySqlResultState result,
            MySqlConnection conn)
        {
            // TODO: Thêm logic xử lý ở đây
            // 1. Lấy thông tin variant từ sàn TMĐT theo sku
            await GetListProductTiki.GetProductIdByOriginalSku(originalSku, result);
            if (result.State != EMySqlResultState.OK)
            {
                return;
            }

            int producIdOnTiki = result.myAnything;

            // Lấy id trên sàn lưu vào tb_track_original_sku
            await TrackOriginalSkuMySql.UpdateProOnEcoIdAsync(originalSku,
                EECommerceType.TIKI
                , producIdOnTiki,
                result,
                conn);
        }

        /// <summary>
        /// Cập nhật variant: Lấy sản phẩm trên sàn tương ứng, lưu db, lưu mapping.
        /// Sau đó cập nhật ảnh, kích thước, cân nặng,... cho biến thể
        /// </summary>
        [HttpPost]
        public async Task<string> UpdateVariant(string sku, int eCommerce)
        {
            MySqlResultState result = new MySqlResultState();

            if ((await AuthentAdministratorAsync()) == null)
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = MySqlResultState.authenFailMessage;
                return JsonConvert.SerializeObject(result);
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    if (eCommerce == (int)EECommerceType.TIKI)
                    {
                        // TODO: Thêm logic xử lý ở đây
                        // 1. Lấy thông tin variant từ sàn TMĐT theo sku
                        await TikiGetEcoProIdFromOriginalSku(sku, result, conn);

                        if(result.State != EMySqlResultState.OK)
                        {
                            return JsonConvert.SerializeObject(result);
                        }

                        int producIdOnTiki = result.myAnything;

                        // 2. Cập nhật ảnh, kích thước, cân nặng,... cho biến thể
                        await TikiUpdateMapping_VariantSomething(sku, result, producIdOnTiki, conn);
                    }
                    else
                    {
                        result.State = EMySqlResultState.OK;
                        result.Message = "Chức năng đang được phát triển";
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"UpdateVariant error: {ex.ToString()}");
                result.State = EMySqlResultState.ERROR;
                result.Message = ex.Message;
            }

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Lấy sản phẩm: Từ Original SKU lấy sản phẩm tương ứng trên sàn và lưu vào db, lưu mapping
        /// proOnEcoId: là id sản phẩm trên sàn (từ cột Pro On Eco ID) nếu có, nếu không có thì để -1
        /// </summary>
        [HttpPost]
        public async Task<string> GetProduct(string sku, int eCommerce, long proOnEcoId = -1)
        {
            MySqlResultState result = new MySqlResultState();

            if ((await AuthentAdministratorAsync()) == null)
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = MySqlResultState.authenFailMessage;
                return JsonConvert.SerializeObject(result);
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    if (eCommerce == (int)EECommerceType.TIKI)
                    {
                        int productIdOnTiki = -1;

                        // Nếu proOnEcoId > 0 → dùng proOnEcoId (đã có ID sản phẩm trên sàn)
                        // Nếu proOnEcoId <= 0 → dùng sku để lấy ID từ sàn
                        if (proOnEcoId > 0)
                        {
                            productIdOnTiki = (int)proOnEcoId;
                        }
                        else
                        {
                            await TikiGetEcoProIdFromOriginalSku(sku, result, conn);
                            if (result.State != EMySqlResultState.OK)
                            {
                                return JsonConvert.SerializeObject(result);
                            }
                            productIdOnTiki = result.myAnything;
                        }

                        // 2. Lưu vào tbtikiitem
                        var pro = await GetListProductTiki.GetProductFromOneShop(productIdOnTiki);

                        if (pro == null || pro.created_by != TikiConstValues.cstrCreatedBy)
                        {
                            result.State = EMySqlResultState.DONT_EXIST;
                            result.Message = "Không tìm thấy sản phẩm trên sàn Tiki";
                            return JsonConvert.SerializeObject(result);
                        }

                        CommonItem item = new CommonItem(pro);
                        if (!string.IsNullOrEmpty(item.imageSrc)) // Không có ảnh đại diện, có thể đây là sản phẩm cha ảo
                        {
                            // Không tồn tại trong DB ta insert
                            await TikiMySql.TikiInsertIfDontExistConnectOutAsync(item, conn);

                            // 3. Lưu mapping, và cập nhật trạng thái tb_track_original_sku để biết đã lưu sản phẩm sàn vào db
                            result = await TikiMySql.TikiMappingInsertFromtbTrackOriginalSKUAsync(productIdOnTiki, conn);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetProduct error: {ex.ToString()}");
                result.State = EMySqlResultState.ERROR;
                result.Message = ex.Message;
            }

            return JsonConvert.SerializeObject(result);
        }
    }
}
