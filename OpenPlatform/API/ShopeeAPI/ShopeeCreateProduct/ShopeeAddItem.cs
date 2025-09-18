using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.ProductModel;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeCreateProduct;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeCreateProduct
{
    public class ShopeeAddItem
    {
        //Get attribute của sách trẻ em, người lớn. Hiện tại đang giống nhau
        public static List<ShopeeAttribute> GetAttributeOfChildren_AudultBook(Product product)
        {
            List<ShopeeAttribute> attribute_list = new List<ShopeeAttribute>();
            ShopeeAttribute shopeeAttribute = null;
            /*
             * {
                        "attribute_id": 100669,
                        "attribute_info": {
                            "format_type": 1,
                            "input_type": 3,
                            "input_validation_type": 2,
                            "is_oem": false,
                            "support_search_value": false
                        },
                        "mandatory": true,
                        "multi_lang": [{
                                "language": "vn",
                                "value": "Nhà Phát Hành"
                            }
                        ],
                        "name": "Publishing Company"
                    }
             */
            shopeeAttribute = new ShopeeAttribute(100669);
            shopeeAttribute.attribute_value_list.Add(
                new ShopeeAttributeValue(0, product.publisherName, null));
            attribute_list.Add(shopeeAttribute);

            /*
             * {
                        "attribute_id": 100673,
                        "attribute_info": {
                            "format_type": 1,
                            "input_type": 2,
                            "input_validation_type": 2,
                            "is_oem": false,
                            "support_search_value": false
                        },
                        "attribute_value_list": [{
                                "multi_lang": [{
                                        "language": "vn",
                                        "value": "Khác"
                                    }
                                ],
                                "name": "Others",
                                "value_id": 3517
                            }, {
                                "multi_lang": [{
                                        "language": "vn",
                                        "value": "Tiếng Trung"
                                    }
                                ],
                                "name": "Chinese",
                                "value_id": 3531
                            }, {
                                "multi_lang": [{
                                        "language": "vn",
                                        "value": "Tiếng Anh"
                                    }
                                ],
                                "name": "English",
                                "value_id": 3539
                            }, {
                                "multi_lang": [{
                                        "language": "vn",
                                        "value": "Tiếng Bahasa Indonesia"
                                    }
                                ],
                                "name": "Bahasa Indonesia",
                                "value_id": 5470
                            }, {
                                "multi_lang": [{
                                        "language": "vn",
                                        "value": "Tiếng Việt"
                                    }
                                ],
                                "name": "Vietnamese",
                                "value_id": 5471
                            }, {
                                "multi_lang": [{
                                        "language": "vn",
                                        "value": "Tiếng Thái"
                                    }
                                ],
                                "name": "Thai",
                                "value_id": 5472
                            }, {
                                "multi_lang": [{
                                        "language": "vn",
                                        "value": "Tiếng Bồ Đào Nha"
                                    }
                                ],
                                "name": "Portuguese",
                                "value_id": 5473
                            }, {
                                "multi_lang": [{
                                        "language": "vn",
                                        "value": "Tiếng Bahasa Malaysia"
                                    }
                                ],
                                "name": "Bahasa Malaysia",
                                "value_id": 5575
                            }, {
                                "multi_lang": [{
                                        "language": "vn",
                                        "value": "Polish"
                                    }
                                ],
                                "name": "Polish",
                                "value_id": 6261
                            }
                        ],
                        "mandatory": true,
                        "multi_lang": [{
                                "language": "vn",
                                "value": "Ngôn ngữ"
                            }
                        ],
                        "name": "Language"
                    }
             */
            shopeeAttribute = new ShopeeAttribute(100673);
            if (product.language == "Tiếng Anh")
            {
                shopeeAttribute.attribute_value_list.Add(
                    new ShopeeAttributeValue(3539, null, null));
            }
            else
            {
                shopeeAttribute.attribute_value_list.Add(
                    new ShopeeAttributeValue(5471, null, null));
            }

            //if (product.language == "Tiếng Việt")
            //{
            //    shopeeAttribute.attribute_value_list.Add(
            //        new ShopeeAttributeValue(5471, null, null));
            //}
            //else if (product.language == "Tiếng Anh")
            //{
            //    shopeeAttribute.attribute_value_list.Add(
            //        new ShopeeAttributeValue(3539, null, null));
            //}
            //else
            //{
            //    shopeeAttribute.attribute_value_list.Add(
            //        new ShopeeAttributeValue(0, product.language, null));
            //}
            attribute_list.Add(shopeeAttribute);

            /*
             * {
                        "attribute_id": 100676,
                        "attribute_info": {
                            "format_type": 1,
                            "input_type": 1,
                            "input_validation_type": 0,
                            "is_oem": false,
                            "support_search_value": false
                        },
                        "attribute_value_list": [{
                                "multi_lang": [{
                                        "language": "vn",
                                        "value": "Nhập khẩu"
                                    }
                                ],
                                "name": "Import",
                                "value_id": 3506
                            }, {
                                "multi_lang": [{
                                        "language": "vn",
                                        "value": "Trong nước"
                                    }
                                ],
                                "name": "Local",
                                "value_id": 3518
                            }
                        ],
                        "mandatory": true,
                        "multi_lang": [{
                                "language": "vn",
                                "value": "Nhập khẩu/ trong nước"
                            }
                        ],
                        "name": "Import/Local"
                    }
             */
            shopeeAttribute = new ShopeeAttribute(100676);
            shopeeAttribute.attribute_value_list.Add(
                new ShopeeAttributeValue(3518, null, null));
            attribute_list.Add(shopeeAttribute);

            /*
             * {
                        "attribute_id": 101024,
                        "attribute_info": {
                            "format_type": 1,
                            "input_type": 3,
                            "input_validation_type": 1,
                            "is_oem": false,
                            "support_search_value": false
                        },
                        "mandatory": true,
                        "multi_lang": [{
                                "language": "vn",
                                "value": "Năm xuất bản"
                            }
                        ],
                        "name": "Year"
                    }
             */
            shopeeAttribute = new ShopeeAttribute(101024);
            shopeeAttribute.attribute_value_list.Add(
                new ShopeeAttributeValue(0, product.publishingTime.ToString(), null));
            attribute_list.Add(shopeeAttribute);

            /*
             * {
                        "attribute_id": 101269,
                        "attribute_info": {
                            "format_type": 1,
                            "input_type": 3,
                            "input_validation_type": 2,
                            "is_oem": false,
                            "support_search_value": false
                        },
                        "mandatory": true,
                        "multi_lang": [{
                                "language": "vn",
                                "value": "Số Trang"
                            }
                        ],
                        "name": "Total Pages"
                    }
             */
            shopeeAttribute = new ShopeeAttribute(101269);
            shopeeAttribute.attribute_value_list.Add(
                new ShopeeAttributeValue(0, product.pageNumber.ToString(), null));
            attribute_list.Add(shopeeAttribute);

            /*
             * {
                        "attribute_id": 101294,
                        "attribute_info": {
                            "format_type": 1,
                            "input_type": 3,
                            "input_validation_type": 2,
                            "is_oem": false,
                            "support_search_value": false
                        },
                        "mandatory": true,
                        "multi_lang": [{
                                "language": "vn",
                                "value": "Nhà xuất bản"
                            }
                        ],
                        "name": "Book printer/distributor"
                    }
             */
            shopeeAttribute = new ShopeeAttribute(101294);
            shopeeAttribute.attribute_value_list.Add(
                new ShopeeAttributeValue(0, product.publishingCompany, null));
            attribute_list.Add(shopeeAttribute);

            /*
             * {
                        "attribute_id": 100710,
                        "attribute_info": {
                            "format_type": 1,
                            "input_type": 2,
                            "input_validation_type": 2,
                            "is_oem": false,
                            "support_search_value": false
                        },
                        "attribute_value_list": [{
                                "multi_lang": [{
                                        "language": "vn",
                                        "value": "Bìa cứng"
                                    }
                                ],
                                "name": "Hard Cover",
                                "value_id": 3492
                            }, {
                                "multi_lang": [{
                                        "language": "vn",
                                        "value": "Bìa mềm"
                                    }
                                ],
                                "name": "Soft Cover",
                                "value_id": 3504
                            }
                        ],
                        "mandatory": false,
                        "multi_lang": [{
                                "language": "vn",
                                "value": "Loại nắp"
                            }
                        ],
                        "name": "Cover Type"
                    }
             */
            shopeeAttribute = new ShopeeAttribute(100710);
            if (product.hardCover == 1)
            {
                shopeeAttribute.attribute_value_list.Add(
                    new ShopeeAttributeValue(3492, null, null));
            }
            else
            {
                shopeeAttribute.attribute_value_list.Add(
                    new ShopeeAttributeValue(3504, null, null));
            }
            attribute_list.Add(shopeeAttribute);

            return attribute_list;
        }

        //
        public static ShopeeAddItemResponseHTTP ShopeeProductAddItem(
            ShopeeAddItem_RequestParameters requsetParameters)
        {
            string path = "/api/v2/product/add_item";
            JsonSerializerSettings SerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };
            string body = JsonConvert.SerializeObject(requsetParameters, SerializeSettings);
            MyLogger.GetInstance().Info(body);

            IRestResponse response = CommonShopeeAPI.ShopeePostMethod(path, body);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            ShopeeAddItemResponseHTTP objResponse = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    objResponse = JsonConvert.DeserializeObject<ShopeeAddItemResponseHTTP>(response.Content, settings);
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.Message);
                    return null;
                }
            }

            return objResponse;
        }
    }
}