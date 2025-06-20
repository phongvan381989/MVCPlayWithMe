using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models.ProductModel
{
    public class ProductCommonInfoWithCombo : BasicObject
    {
        public int comboId { get; set; }
        public string comboCode { get; set; }
        public string comboName { get; set; }
        public int categoryId { get; set; }
        public string categoryName { get; set; }
        public int bookCoverPrice { get; set; }
        public float discount { get; set; }
        public string author { get; set; }
        public string translator { get; set; }
        public int publisherId { get; set; }
        public string publisherName { get; set; }
        public string publishingCompany { get; set; }
        public int publishingTime { get; set; }
        ///// <summary>
        ///// publishingTimeyyyyMMdd định dạng string yyyy-MM-dd của publishingTime
        ///// </summary>
        //public string publishingTimeyyyyMMdd { get; set; }
        public int productLong { get; set; }
        public int productWide { get; set; }
        public int productHigh { get; set; }
        public int productWeight { get; set; }
        public string positionInWarehouse { get; set; }

        // Theo Tiki có các loại bìa như sau:
        /*
         * {
                "GET": "https://api.tiki.vn/integration/v2/attributes/1300/values?limit=150",
                "Giai Thich": "Lay gia tri cua attribute id 130, hình thức bìa",
                {
                "data": [
                    {
                        "id": 8208187,
                        "value": "",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 462007,
                        "value": "Art and Craft Kit",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 180373,
                        "value": "B Format Paperback",
                        "position": 72,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 190137,
                        "value": "B-format Paperback",
                        "position": 86,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 178267,
                        "value": "Bath book",
                        "position": 12,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 148970,
                        "value": "Bìa cứng",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 8218450,
                        "value": "Bìa cứng áo ôm",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 8224711,
                        "value": "Bìa Cứng, Rời",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 4416043,
                        "value": "Bìa Da",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 878933,
                        "value": "Bìa Da Công Nghiệp Microfiber",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 162353,
                        "value": "Bìa gập",
                        "position": 1,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 148966,
                        "value": "Bìa mềm",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 8208188,
                        "value": "Bìa mềm tay gấp",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 8227791,
                        "value": "Bìa mềm, bìa áo + bìa keo",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 8227790,
                        "value": "Bìa mềm, cán sần, phủ nhũ",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 8209868,
                        "value": "Bìa mềm, không tay gấp",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 8210099,
                        "value": "Bìa mềm, rời",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 8221853,
                        "value": "Bìa Mềm, Rời",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 162351,
                        "value": "Bìa rời",
                        "position": 2,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 8230678,
                        "value": "Bìa vải",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 184595,
                        "value": "Big book",
                        "position": 82,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 162837,
                        "value": "Board",
                        "position": 6,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 190145,
                        "value": "Board + sound panel",
                        "position": 90,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 151640,
                        "value": "Board book",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179245,
                        "value": "Board book with CD",
                        "position": 26,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179151,
                        "value": "Board book with sound panel",
                        "position": 23,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 209965,
                        "value": "BOOK WITH CD",
                        "position": 95,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179811,
                        "value": "Book with cookery kit",
                        "position": 50,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179835,
                        "value": "Book with cupcake kit",
                        "position": 62,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179247,
                        "value": "Box",
                        "position": 27,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179249,
                        "value": "Box of cards",
                        "position": 28,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179831,
                        "value": "Box of cards with envelopes",
                        "position": 60,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179251,
                        "value": "Box of cards with pen",
                        "position": 29,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 289585,
                        "value": "Boxed set - Hardback",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 413467,
                        "value": "Boxset",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179853,
                        "value": "Cards",
                        "position": 71,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 177605,
                        "value": "CD",
                        "position": 9,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 178855,
                        "value": "CD-Audio",
                        "position": 13,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179849,
                        "value": "Cloth book",
                        "position": 69,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179253,
                        "value": "Clothbound hardback",
                        "position": 30,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179149,
                        "value": "Clothbound hardback with slipcase",
                        "position": 22,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179255,
                        "value": "Concealed spiral binding",
                        "position": 31,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 190141,
                        "value": "Counterpack holds 48 Mini Activity Books",
                        "position": 88,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 178859,
                        "value": "Digital",
                        "position": 15,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 178857,
                        "value": "DVD",
                        "position": 14,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179155,
                        "value": "Flexi",
                        "position": 24,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179653,
                        "value": "Flexiback",
                        "position": 49,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 362095,
                        "value": "Hard cover",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 162831,
                        "value": "Hardback",
                        "position": 3,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179829,
                        "value": "Hardback + jacket",
                        "position": 59,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179843,
                        "value": "Hardback + padding",
                        "position": 66,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 190139,
                        "value": "Hardback + sound panel",
                        "position": 87,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179147,
                        "value": "Hardback with CD - English Learner's Edition",
                        "position": 21,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179833,
                        "value": "Hardback with concealed spiral binding",
                        "position": 61,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179257,
                        "value": "Hardback with dust jacket",
                        "position": 32,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179259,
                        "value": "Hardback with padded cover",
                        "position": 33,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179261,
                        "value": "Hardback with ribbon marker",
                        "position": 34,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179263,
                        "value": "Hardback with sound panel",
                        "position": 35,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179265,
                        "value": "Hardback with spiral binding",
                        "position": 36,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 148968,
                        "value": "Hardcover",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 190147,
                        "value": "Hardcover + slipcase",
                        "position": 91,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 184349,
                        "value": "Hardcover Picture Book",
                        "position": 81,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 190149,
                        "value": "Hardcover with CD",
                        "position": 92,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 184347,
                        "value": "Hardcover with dust jacket",
                        "position": 80,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179837,
                        "value": "Jigsaw box",
                        "position": 63,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 178261,
                        "value": "Kit or Box Set",
                        "position": 10,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 4403301,
                        "value": "Leatherbound Harback",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179267,
                        "value": "Library edition hardback",
                        "position": 37,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 732043,
                        "value": "Loose-leaf",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 190131,
                        "value": "Mass Market",
                        "position": 84,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 149162,
                        "value": "Mass market paperback",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 190133,
                        "value": "Mass Market Paperbound",
                        "position": 85,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179269,
                        "value": "Mini padded hardback",
                        "position": 38,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 234827,
                        "value": "Mix Media Pack",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 219797,
                        "value": "Mixed media format",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 183541,
                        "value": "Mixed media product",
                        "position": 79,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 180375,
                        "value": "Multiple copy pack",
                        "position": 73,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179063,
                        "value": "Multiple-item retail product, boxed",
                        "position": 16,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179085,
                        "value": "Multiple-item retail product, part(s) enclosed",
                        "position": 19,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179065,
                        "value": "Multiple-item retail product, shrinkwrapped",
                        "position": 17,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179087,
                        "value": "Multiple-item retail product, slip-cased",
                        "position": 20,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 403367,
                        "value": "Novelty",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179827,
                        "value": "Offer 2 books",
                        "position": 58,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179821,
                        "value": "Offer 2 books & a box of cards",
                        "position": 55,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179819,
                        "value": "Offer 3 books",
                        "position": 54,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179851,
                        "value": "Offer 3 paperbacks with slipcase",
                        "position": 70,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179845,
                        "value": "Offer 4 books",
                        "position": 67,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179817,
                        "value": "Offer 4 mini books",
                        "position": 53,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179839,
                        "value": "Offer 5 books",
                        "position": 64,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179641,
                        "value": "Offer 5 hardback books in slipcase",
                        "position": 48,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179823,
                        "value": "Offer Book, tear-off pad and box of cards",
                        "position": 56,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179847,
                        "value": "Offer Box set of 3 books",
                        "position": 68,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 180809,
                        "value": "Offer Four paperback books in carry case",
                        "position": 75,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179271,
                        "value": "Offer Hardback with ribbon marker",
                        "position": 39,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 162839,
                        "value": "Pack",
                        "position": 7,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179273,
                        "value": "Pack of 48 cards",
                        "position": 40,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179825,
                        "value": "Pack with supplies and book",
                        "position": 57,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179275,
                        "value": "Pad HB",
                        "position": 41,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 162833,
                        "value": "Padded hardback",
                        "position": 4,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179277,
                        "value": "Padded hardback with poster",
                        "position": 42,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179279,
                        "value": "Padded hardback with ribbon marker",
                        "position": 43,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 148964,
                        "value": "Paperback",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 190151,
                        "value": "Paperback + stickers",
                        "position": 93,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179841,
                        "value": "Paperback with CD",
                        "position": 65,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 162829,
                        "value": "Perfect bound paperback",
                        "position": 2,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 732041,
                        "value": "Pop Up Book",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 182553,
                        "value": "Poster",
                        "position": 78,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 184983,
                        "value": "Rag book",
                        "position": 83,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179067,
                        "value": "Record book",
                        "position": 18,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179281,
                        "value": "Reduced size hardback",
                        "position": 44,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 162835,
                        "value": "Sewn paperback",
                        "position": 5,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 174183,
                        "value": "Spiral bound",
                        "position": 8,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179283,
                        "value": "Spiral hardback",
                        "position": 45,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179285,
                        "value": "Spiral HB",
                        "position": 46,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 190143,
                        "value": "Spiral HB + Xylophone",
                        "position": 89,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 180823,
                        "value": "Spiral-bound",
                        "position": 76,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179159,
                        "value": "Spiral-bound book",
                        "position": 25,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179287,
                        "value": "Spiral-bound hardback",
                        "position": 47,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 180807,
                        "value": "Spiral-bound hardback with ink pad",
                        "position": 74,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179815,
                        "value": "Tin of cards",
                        "position": 52,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 179813,
                        "value": "Tin with wipe-clean cards and pen",
                        "position": 51,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 151638,
                        "value": "Trade paperback",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 661273,
                        "value": "Tùy chọn phiên bản bìa cứng hoặc bìa mềm",
                        "position": 0,
                        "attribute_code": "book_cover"
                    },
                    {
                        "id": 182027,
                        "value": "Wallchart",
                        "position": 77,
                        "attribute_code": "book_cover"
                    }
                ],
                "paging": {
                    "total": 124,
                    "per_page": 150,
                    "current_page": 1,
                    "last_page": 1,
                    "from": 1,
                    "to": 124
                }
            }
            }

         */
        /// <summary>
        /// Bìa cứng: 1, Bìa mềm: 0
        /// </summary>
        public int hardCover { get; set; }

        // Theo Tiki có các tuổi như sau:
        /*
         * {
                "GET": "https://api.tiki.vn/integration/v2/attributes/2532/values?limit=50",
                "Giai Thich": "Lay gia tri cua attribute id 2532, tuổi",
                "data": [{
                        "attribute_code": "age_group",
                        "id": 8249461,
                        "position": 0,
                        "value": "Người lớn"
                    }, {
                        "attribute_code": "age_group",
                        "id": 8226225,
                        "position": 0,
                        "value": "Từ 0 - 3 tuổi"
                    }, {
                        "attribute_code": "age_group",
                        "id": 8206577,
                        "position": 0,
                        "value": "Từ 10 - 12 tuổi"
                    }, {
                        "attribute_code": "age_group",
                        "id": 8241317,
                        "position": 0,
                        "value": "Từ 13 - 18 tuổi"
                    }, {
                        "attribute_code": "age_group",
                        "id": 8226253,
                        "position": 0,
                        "value": "Từ 4 - 6 tuổi"
                    }, {
                        "attribute_code": "age_group",
                        "id": 8226236,
                        "position": 0,
                        "value": "Từ 7 - 9 tuổi"
                    }
                ],
                "paging": {
                    "current_page": 1,
                    "from": 1,
                    "last_page": 1,
                    "per_page": 50,
                    "to": 6,
                    "total": 6
                }
            }
         */
        /// <summary>
        /// Tuổi nhỏ nhất nên dùng. Đơn vị tháng
        /// </summary>
        public int minAge { get; set; }

        /// <summary>
        /// Tuổi lớn nhất nên dùng. Đơn vị tháng
        /// </summary>
        public int maxAge { get; set; }

        /// <summary>
        /// Tái bản lần thứ mấy
        /// </summary>
        public int republish { get; set; }

        // Theo Tiki có các ngôn ngữ sau:
        /*
         * {
                "GET": "https://api.tiki.vn/integration/v2/attributes/2693/values?limit=50",
                "Giai Thich": "Lay gia tri cua attribute id 2693, ngôn ngữ",
                "data": [{
                        "attribute_code": "language_book",
                        "id": 8252683,
                        "position": 0,
                        "value": "Song ngữ"
                    }, {
                        "attribute_code": "language_book",
                        "id": 8252681,
                        "position": 0,
                        "value": "Tiếng Anh"
                    }, {
                        "attribute_code": "language_book",
                        "id": 8252680,
                        "position": 0,
                        "value": "Tiếng Việt"
                    }, {
                        "attribute_code": "language_book",
                        "id": 8252682,
                        "position": 0,
                        "value": "Đa ngữ"
                    }
                ],
                "paging": {
                    "current_page": 1,
                    "from": 1,
                    "last_page": 1,
                    "per_page": 50,
                    "to": 4,
                    "total": 4
                }
            }
         */
        public string language { get; set; }

        /// <summary>
        /// Trạng thái sản phẩm.
        /// 0: Đang kinh doanh bình thường
        /// 1: Nhà phát hành tạm thời hết hàng,
        /// 2: Ngừng kinh doanh,
        /// </summary>
        public int status { get; set; }

        public int pageNumber { get; set; }

        public ProductCommonInfoWithCombo()
        {

        }

        public ProductCommonInfoWithCombo(
                int comboIdIn,
                int categoryIdIn,
                int bookCoverPriceIn,
                float discountIn,
                string authorIn,
                string translatorIn,
                int publisherIdIn,
                string publishingCompanyIn,
                int publishingTimeIn,
                int productLongIn,
                int productWideIn,
                int productHighIn,
                int productWeightIn,
                string positionInWarehouseIn,
                int hardCoverIn,
                string bookLangugeIn,
                int minAgeIn,
                int maxAgeIn,
                int republishIn,
                int statusIn,
                int pageNumberIn
            )
        {
            comboId = comboIdIn;
            bookCoverPrice = bookCoverPriceIn;
            discount = discountIn;
            author = authorIn;
            translator = translatorIn;
            publisherId = publisherIdIn;
            publishingCompany = publishingCompanyIn;
            publishingTime = publishingTimeIn;
            productLong = productLongIn;
            productWide = productWideIn;
            productHigh = productHighIn;
            productWeight = productWeightIn;
            positionInWarehouse = positionInWarehouseIn;
            categoryId = categoryIdIn;
            hardCover = hardCoverIn;
            language = bookLangugeIn;
            minAge = minAgeIn;
            maxAge = maxAgeIn;
            republish = republishIn;
            status = statusIn;
            pageNumber = pageNumberIn;
        }
    }
}