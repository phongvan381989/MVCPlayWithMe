using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MVCPlayWithMe.General
{
    /// <summary>
    /// Utility class để generate SEO-friendly slugs từ Vietnamese text
    /// </summary>
    public static class SlugHelper
    {
        // Vietnamese character mapping (có dấu → không dấu)
        private static readonly Dictionary<char, char> VietnameseMap = new Dictionary<char, char>
        {
            // a
            {'á','a'}, {'à','a'}, {'ả','a'}, {'ã','a'}, {'ạ','a'},
            {'ă','a'}, {'ắ','a'}, {'ằ','a'}, {'ẳ','a'}, {'ẵ','a'}, {'ặ','a'},
            {'â','a'}, {'ấ','a'}, {'ầ','a'}, {'ẩ','a'}, {'ẫ','a'}, {'ậ','a'},
            // A
            {'Á','A'}, {'À','A'}, {'Ả','A'}, {'Ã','A'}, {'Ạ','A'},
            {'Ă','A'}, {'Ắ','A'}, {'Ằ','A'}, {'Ẳ','A'}, {'Ẵ','A'}, {'Ặ','A'},
            {'Â','A'}, {'Ấ','A'}, {'Ầ','A'}, {'Ẩ','A'}, {'Ẫ','A'}, {'Ậ','A'},
            // đ
            {'đ','d'}, {'Đ','D'},
            // e
            {'é','e'}, {'è','e'}, {'ẻ','e'}, {'ẽ','e'}, {'ẹ','e'},
            {'ê','e'}, {'ế','e'}, {'ề','e'}, {'ể','e'}, {'ễ','e'}, {'ệ','e'},
            {'É','E'}, {'È','E'}, {'Ẻ','E'}, {'Ẽ','E'}, {'Ẹ','E'},
            {'Ê','E'}, {'Ế','E'}, {'Ề','E'}, {'Ể','E'}, {'Ễ','E'}, {'Ệ','E'},
            // i
            {'í','i'}, {'ì','i'}, {'ỉ','i'}, {'ĩ','i'}, {'ị','i'},
            {'Í','I'}, {'Ì','I'}, {'Ỉ','I'}, {'Ĩ','I'}, {'Ị','I'},
            // o
            {'ó','o'}, {'ò','o'}, {'ỏ','o'}, {'õ','o'}, {'ọ','o'},
            {'ô','o'}, {'ố','o'}, {'ồ','o'}, {'ổ','o'}, {'ỗ','o'}, {'ộ','o'},
            {'ơ','o'}, {'ớ','o'}, {'ờ','o'}, {'ở','o'}, {'ỡ','o'}, {'ợ','o'},
            {'Ó','O'}, {'Ò','O'}, {'Ỏ','O'}, {'Õ','O'}, {'Ọ','O'},
            {'Ô','O'}, {'Ố','O'}, {'Ồ','O'}, {'Ổ','O'}, {'Ỗ','O'}, {'Ộ','O'},
            {'Ơ','O'}, {'Ớ','O'}, {'Ờ','O'}, {'Ở','O'}, {'Ỡ','O'}, {'Ợ','O'},
            // u
            {'ú','u'}, {'ù','u'}, {'ủ','u'}, {'ũ','u'}, {'ụ','u'},
            {'ư','u'}, {'ứ','u'}, {'ừ','u'}, {'ử','u'}, {'ữ','u'}, {'ự','u'},
            {'Ú','U'}, {'Ù','U'}, {'Ủ','U'}, {'Ũ','U'}, {'Ụ','U'},
            {'Ư','U'}, {'Ứ','U'}, {'Ừ','U'}, {'Ử','U'}, {'Ữ','U'}, {'Ự','U'},
            // y
            {'ý','y'}, {'ỳ','y'}, {'ỷ','y'}, {'ỹ','y'}, {'ỵ','y'},
            {'Ý','Y'}, {'Ỳ','Y'}, {'Ỷ','Y'}, {'Ỹ','Y'}, {'Ỵ','Y'},
        };

        /// <summary>
        /// Generate slug từ text
        /// </summary>
        /// <param name="text">Text gốc (Vietnamese)</param>
        /// <param name="keepPrefix">
        /// true: giữ prefix cho categories ("Sách thiếu nhi" → "sach-thieu-nhi")
        /// false: bỏ prefix cho publishers ("Nhà xuất bản Kim Đồng" → "kim-dong")
        /// </param>
        /// <returns>Slug (ASCII, lowercase, dashes)</returns>
        public static string GenerateSlug(string text, bool keepPrefix = true)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Lowercase và trim
            text = text.ToLower().Trim();

            // Remove prefix nếu cần
            if (!keepPrefix)
            {
                // Remove "nhà xuất bản " from publishers
                if (text.StartsWith("nhà xuất bản "))
                    text = text.Substring("nhà xuất bản ".Length);
            }

            // Convert Vietnamese chars → ASCII
            var result = new StringBuilder();
            foreach (char c in text)
            {
                if (VietnameseMap.TryGetValue(c, out char mapped))
                {
                    result.Append(mapped);
                }
                else if (char.IsLetterOrDigit(c))
                {
                    result.Append(c);
                }
                else if (char.IsWhiteSpace(c) || c == '-')
                {
                    result.Append('-');
                }
                // Ignore other special chars
            }

            // Clean up: multiple dashes → single dash
            string slug = Regex.Replace(result.ToString(), "-+", "-");

            // Trim leading/trailing dashes
            slug = slug.Trim('-');

            return slug;
        }

        /// <summary>
        /// Generate slug cho Category
        /// - Nếu có dấu ">", lấy phần sau dấu ">" cuối cùng
        /// - Thêm prefix "Sách " nếu chưa có
        /// Example: "Sách tiếng Việt>Sách văn học>Truyện cổ tích" → "Sách Truyện cổ tích" → "sach-truyen-co-tich"
        /// </summary>
        public static string GenerateCategorySlug(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                return string.Empty;

            string processedName = categoryName.Trim();

            // Nếu có dấu ">", lấy phần sau dấu ">" cuối cùng
            if (processedName.Contains(">"))
            {
                int lastIndex = processedName.LastIndexOf('>');
                processedName = processedName.Substring(lastIndex + 1).Trim();
            }

            // Thêm prefix "Sách " nếu chưa có
            if (!processedName.StartsWith("Sách ", StringComparison.OrdinalIgnoreCase) &&
                !processedName.StartsWith("Sách", StringComparison.OrdinalIgnoreCase))
            {
                processedName = "Sách " + processedName;
            }

            // Generate slug (keepPrefix = true để giữ "sách")
            return GenerateSlug(processedName, keepPrefix: true);
        }

        /// <summary>
        /// Generate slug cho Publisher (bỏ prefix "Nhà xuất bản")
        /// </summary>
        public static string GeneratePublisherSlug(string publisherName)
        {
            return GenerateSlug(publisherName, keepPrefix: false);
        }
    }
}
