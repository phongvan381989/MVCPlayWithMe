using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Product;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using static MVCPlayWithMe.General.Common;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    public class CommonItem
    {
        //
        public string eType { get; set; }
        /// <summary>
        /// Does it contain model.
        /// SHopee có has_model = false, ta vẫn tạo commonModel danh sách, nhưng chỉ có 1 phần tử
        /// </summary>
        public Boolean has_model { get; set; }

        /// <summary>
        /// Unique product ID
        /// Shopee: Nếu sản phẩm không có phân loại, đây là id sản phẩm, modelId = -1, nếu có phân loại thì modelId != -1
        /// Tiki: Đây là id sản phẩm, modelId = -1
        /// </summary>
        public long itemId { get; set; }

        // Item id tương ứng trong tbShopeeItem
        public int dbItemId { get; set; }

        // Lưu supper_id sàn Tiki, khi sản phẩm không có super_id giá trị này mặc định là 0.
        public int tikiSuperId { get; set; }

        //// Tiki cấu trúc sản phẩm cũng thể hiện item/model nhưng không rõ ràng, có thể lấy thông tin từ modelId
        //// trực tiếp (không như bên shopee lấy phải gián tiếp qua item).
        //// Ngoài ra, sản phẩm không có sản phẩm cha(item) sẽ có super_id = 0,
        //// khác với shopee bắt buộc có item, rồi mới có model
        //// Class CommonItem này đang tương
        //// ứng với model trên sàn Tiki, vì thế thêm trường tikiSuperName
        //// Vấn đề lịch sử để lại, giờ chuyển item/model rõ ràng như Shopee thì hợp lý hơn nhưng lười
        //public string tikiSuperName { get; set; }

        /// <summary>
        /// SKU of product
        /// </summary>
        public string sku { get; set; }

        // tiki có thuộc tính này
        public string superSku { get; set; }
        /// <summary>
        /// Name of product
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Thuộc tính theo SHOPEE
        /// Enumerated type that defines the current status of the item.
        /// Applicable values: NORMAL, DELETED, BANNED and UNLIST.
        /// </summary>
        public string item_status { get; set; }

        public Boolean bActive { get; set; }

        ///// <summary>
        ///// product is hidden.
        ///// </summary>
        //public string strHidden { get; set; }

        ///// <summary>
        ///// the sell price of a product
        ///// </summary>
        //public int price { get; set; }

        ///// <summary>
        ///// the price before discount of a product
        ///// </summary>
        //public int market_price { get; set; }

        //public int quantity_sellable { get; set; }

        /// <summary>
        /// Đường dẫn chứa ảnh đại diện
        /// </summary>
        public string imageSrc { get; set; }

        /// <summary>
        ///  Lấy url ảnh, video của item shopee phục vụ sinh item trên voibenho
        /// </summary>
        public List<string> imageSrcList { get; set; }
        public string videoSrc { get; set; }

        public List<CommonModel> models { get; set; }

        public string detail { get; set; }

        // Shopee cập nhật số lượng trả về kết quả ở đây
        public MySqlResultState result { get; set; }

        public CommonItem()
        {
            eType = Common.eShopee;
            models = new List<CommonModel>();
            bActive = true;
        }

        public CommonItem(string inEtype)
        {
            eType = inEtype;
            models = new List<CommonModel>();
            bActive = true;
        }

        public static ShopeeGetModelList_Model GetModelFromModelListResponse(ShopeeGetModelListResponse obj, int tierIndex)
        {
            ShopeeGetModelList_Model model = null;
            foreach(var m in obj.model)
            {
                if(m.tier_index[0] == tierIndex)
                {
                    model = m;
                    break;
                }
            }

            return model;
        }

        private Boolean ConvertModelStatusToInt(string status)
        {
            if (status == "MODEL_NORMAL")
                return true;

            return false;
        }

        public CommonItem(ShopeeGetItemBaseInfoItem pro)
        {
            try
            {
                eType = Common.eShopee;
                models = new List<CommonModel>();

                itemId = pro.item_id;
                sku = pro.item_sku;
                name = pro.item_name;
                item_status = pro.item_status;
                if (pro.item_status == "NORMAL")
                    bActive = true;
                else
                    bActive = false;
                has_model = pro.has_model;
                imageSrc = pro.image.image_url_list[0];

                // Lấy url ảnh, video của item shopee phục vụ sinh item trên voibenho
                imageSrcList = new List<string>();
                foreach (var s in pro.image.image_url_list)
                {
                    imageSrcList.Add(s);
                }
                if (pro.video_info != null && pro.video_info.Count > 0)
                {
                    videoSrc = pro.video_info[0].video_url;
                }

                if (pro.description_type == "normal")
                {
                    detail = pro.description;
                }
                else
                {
                    for (int j = 0; j < pro.description_info.extended_description.field_list.Count; j++)
                    {
                        if (pro.description_info.extended_description.field_list[j].field_type == "text")
                        {
                            detail = detail + pro.description_info.extended_description.field_list[j].text;
                        }
                    }
                }
                if (!has_model)
                {
                    CommonModel commonModel = new CommonModel();
                    commonModel.modelId = -1;// Thực tế trên sàn shopee không có model id
                    commonModel.price = (int)pro.price_info[0].current_price;
                    commonModel.market_price = (int)pro.price_info[0].original_price;
                    if (pro.stock_info_v2 != null)
                        commonModel.quantity_sellable = pro.stock_info_v2.seller_stock[0].stock;
                    else
                        commonModel.quantity_sellable = 0;

                    // Lấy tên file ảnh
                    // Từ url lấy được đường dẫn đầy đủ của ảnh
                    commonModel.imageSrc = pro.image.image_url_list[0];
                    commonModel.bActive = bActive;

                    models.Add(commonModel);
                }
                else
                {
                    ShopeeGetModelListResponse obj = ShopeeGetModelList.ShopeeProductGetModelList(pro.item_id);
                    if (obj != null)
                    {
                        ShopeeGetModelList_TierVariation tierVar = obj.tier_variation[0];
                        int count = tierVar.option_list.Count;
                        for (int i = 0; i < count; i++)
                        {
                            CommonModel commonModel = new CommonModel();
                            ShopeeGetModelList_Model model = GetModelFromModelListResponse(obj, i);
                            ShopeeGetModelList_TierVariation_Option option = tierVar.option_list[i];
                            commonModel.modelId = model.model_id;
                            commonModel.name = tierVar.name + "--" + option.option;
                            if (option.image != null)
                                commonModel.imageSrc = option.image.image_url;
                            commonModel.quantity_sellable = model.stock_info_v2.seller_stock[0].stock;
                            commonModel.price = (int)model.price_info[0].current_price;
                            commonModel.market_price = (int)model.price_info[0].original_price;
                            if (model.model_status == "MODEL_NORMAL")
                                commonModel.bActive = true;
                            else
                                commonModel.bActive = false;

                            models.Add(commonModel);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Info("CommonItem from ShopeeGetItemBaseInfoItem crashed. " + ex.ToString());
                MyLogger.GetInstance().Info(JsonConvert.SerializeObject(pro));
            }
        }

        public CommonItem(TikiProduct pro)
        {
            eType = Common.eTiki;
            models = new List<CommonModel>();
            itemId = pro.product_id;
            tikiSuperId = pro.super_id;
            sku = pro.sku;
            superSku = pro.super_sku;
            name = pro.name;

            if (pro.active == 1)
                bActive = true;
            else
                bActive = false;
            has_model = false;
            //imageSrc = pro.thumbnail;
            if (pro.images.Count > 0)
            {
                // Tìm ảnh đại diện theo thumbnail
                if (!string.IsNullOrWhiteSpace(pro.thumbnail))
                {
                    // Tìm ảnh giống tên với thumbnail
                    // Tìm tên file từ thumbnail
                    string thumbnailFileName = Path.GetFileName(pro.thumbnail);

                    // Lặp qua danh sách Images để tìm URL khớp
                    string imageFileName = null;
                    foreach (var image in pro.images)
                    {
                        imageFileName = Path.GetFileName(image.url);
                        if (imageFileName == thumbnailFileName)
                        {
                            imageSrc = image.url;
                            break;
                        }
                    }
                }
                else
                {
                    // Tìm ảnh đại diện theo index/position. Tìm position giá trị nhỏ nhất
                    int position = Int32.MaxValue;
                    foreach (var image in pro.images)
                    {
                        if (image.position < position)
                        {
                            position = image.position;
                            imageSrc = image.url;
                            if (position == 0)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            CommonModel commonModel = new CommonModel();
            commonModel.modelId = -1;
            commonModel.price = pro.price;
            commonModel.market_price = pro.market_price;

            commonModel.quantity_sellable = TikiUpdateStock.GetQuantityFromTikiProduct(pro);

            // Lấy tên file ảnh
            // Từ url lấy được đường dẫn đầy đủ của ảnh
            commonModel.imageSrc = imageSrc;

            models.Add(commonModel);
        }

        // Nếu Id và Supper Id bằng nhau, đây là sản phẩm cha chung ảo
        // Nếu là sản phẩm tiki và là cha chung ảo thì trả true ngược lại false
        public Boolean TikiCheckVirtalParent()
        {
            if (eType == eTiki && (int)itemId == tikiSuperId)
            {
                return true;
            }

            return false;
        }

        static private Boolean SumProductQuantity(Dictionary<string, int> sumDic, Dictionary<string, int> paraDic)
        {
            if (sumDic == null)
                sumDic = new Dictionary<string, int>();

            sumDic.Clear();
            foreach (var e in paraDic)
            {
                if(sumDic.ContainsKey(e.Key))
                {
                    sumDic[e.Key] = sumDic[e.Key] + e.Value;
                }
                else
                {
                    sumDic.Add(e.Key, e.Value);
                }
            }
            return true;
        }
    }
}
