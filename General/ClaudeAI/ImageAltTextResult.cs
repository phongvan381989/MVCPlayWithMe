using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCPlayWithMe.Models.SanPhamModel;
using MySqlConnector;

namespace MVCPlayWithMe.General.ClaudeAI
{
    /// <summary>
    /// Loại ảnh sách (để tối ưu SEO khác nhau)
    /// </summary>
    public enum BookImageType
    {
        /// <summary>
        /// Ảnh bìa sách (cover) - ảnh chính, quan trọng nhất cho SEO
        /// </summary>
        Cover = 0,

        /// <summary>
        /// Ảnh trang trong sách - nội dung, minh họa
        /// </summary>
        InsidePage = 1,

        /// <summary>
        /// Ảnh gáy sách
        /// </summary>
        Spine = 2,

        /// <summary>
        /// Ảnh mặt sau sách (back cover) - thường có tóm tắt
        /// </summary>
        BackCover = 3,

        /// <summary>
        /// Ảnh tổng thể sách (toàn bộ, nhiều góc)
        /// </summary>
        FullView = 4
    }

    public class ImageAltTextResult
    {
        /// <summary>
        /// Title/Caption ngắn gọn cho thẻ title (5-10 từ)
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Alt text tối ưu SEO cho thẻ img (50-125 ký tự, bao gồm tên sách, tác giả, loại sách)
        /// </summary>
        public string AltText { get; set; }

        /// <summary>
        /// Description chi tiết cho meta description (20-40 từ, mô tả màu sắc, hình ảnh, đặc điểm)
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Số trang (chỉ có khi imageType = InsidePage)
        /// Format: "5" (1 trang) hoặc "2-3" (2 trang spread)
        /// Null nếu không detect được hoặc không phải ảnh trang trong
        /// </summary>
        public string PageNumber { get; set; }

        /// <summary>
        /// Thông báo lỗi nếu có
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Có lỗi hay không
        /// </summary>
        public bool HasError => !string.IsNullOrEmpty(Error);

        /// <summary>
        /// Gửi ảnh đến Claude AI và lấy alt text (title, alt, description)
        /// Ảnh phải là webp, kích thước < 1000x1000, size < 100KB
        /// </summary>
        /// <param name="imagePath">Đường dẫn tuyệt đối đến file ảnh webp</param>
        /// <param name="apiKey">Anthropic API key</param>
        /// <param name="imageType">Loại ảnh sách (bìa, trang trong, gáy...) - mặc định là bìa</param>
        /// <param name="bookName">Tên sách (nếu có) - giúp alt text chính xác hơn</param>
        /// <param name="author">Tác giả (nếu có)</param>
        /// <param name="bookFormat">Loại sách: "bìa cứng", "bìa mềm", "truyện tranh"... (nếu có)</param>
        /// <param name="publisher">Nhà xuất bản (nếu có)</param>
        /// <param name="minAge">Độ tuổi tối thiểu (nếu có, dùng cho sách trẻ em)</param>
        /// <param name="maxAge">Độ tuổi tối đa (nếu có, dùng cho sách trẻ em)</param>
        /// <returns>ImageAltTextResult chứa title, alt text, description</returns>
        public static async Task<ImageAltTextResult> GenerateImageAltTextAsync(
            string imagePath,
            string apiKey,
            BookImageType imageType = BookImageType.Cover,
            string bookName = null,
            string author = null,
            string bookFormat = null,
            string publisher = null,
            int? minAge = null,
            int? maxAge = null)
        {
            var result = new ImageAltTextResult();
            string prompt = null;
            string responseJson = null;

            try
            {
                // Kiểm tra file tồn tại
                if (!File.Exists(imagePath))
                {
                    result.Error = "File không tồn tại: " + imagePath;
                    return result;
                }

                //// Kiểm tra kích thước file < 100KB
                //var fileInfo = new FileInfo(imagePath);
                //if (fileInfo.Length > 100 * 1024)
                //{
                //    result.Error = $"File quá lớn: {fileInfo.Length / 1024}KB (yêu cầu < 100KB)";
                //    return result;
                //}

                // Kiểm tra định dạng webp
                string extension = Path.GetExtension(imagePath).ToLower();
                if (extension != ".webp")
                {
                    result.Error = $"File không phải định dạng webp: {extension}";
                    return result;
                }

                // Đọc file ảnh và chuyển sang base64
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                string base64Image = Convert.ToBase64String(imageBytes);

                // Detect MIME type thực tế từ file bytes (không dựa vào extension)
                string mediaType = DetectImageMediaType(imageBytes);

                // Tạo Anthropic client
                var client = new Anthropic.SDK.AnthropicClient(apiKey);

                // Tạo prompt tùy theo loại ảnh và thông tin sẵn có
                prompt = GetPromptForImageType(imageType, bookName, author,
                    bookFormat, publisher, minAge, maxAge);

                // Tạo messages
                var messages = new List<Anthropic.SDK.Messaging.Message>
                {
                    new Anthropic.SDK.Messaging.Message
                    {
                        Role = Anthropic.SDK.Messaging.RoleType.User,
                        Content = new List<Anthropic.SDK.Messaging.ContentBase>
                        {
                            new Anthropic.SDK.Messaging.ImageContent
                            {
                                Source = new Anthropic.SDK.Messaging.ImageSource
                                {
                                    Type = Anthropic.SDK.Messaging.SourceType.base64,
                                    MediaType = mediaType,  // Dùng MIME type detect được
                                    Data = base64Image
                                }
                            },
                            new Anthropic.SDK.Messaging.TextContent
                            {
                                Text = prompt
                            }
                        }
                    }
                };

                // Tạo parameters
                var parameters = new Anthropic.SDK.Messaging.MessageParameters
                {
                    Messages = messages,
                    Model = Anthropic.SDK.Constants.AnthropicModels.Claude46Sonnet,
                    MaxTokens = 1024,
                    Stream = false
                };

                // Gọi API
                var response = await client.Messages.GetClaudeMessageAsync(parameters);

                // Lấy text response
                var textContent = response.Content
                    .OfType<Anthropic.SDK.Messaging.TextContent>()
                    .FirstOrDefault();

                if (textContent == null)
                {
                    result.Error = "Không nhận được response từ Claude";
                    return result;
                }

                string responseText = textContent.Text.Trim();

                // Parse JSON response
                // Xử lý trường hợp response có thể bọc trong ```json```
                if (responseText.StartsWith("```json"))
                {
                    responseText = responseText.Substring(7); // Bỏ ```json
                }
                if (responseText.StartsWith("```"))
                {
                    responseText = responseText.Substring(3); // Bỏ ```
                }
                if (responseText.EndsWith("```"))
                {
                    responseText = responseText.Substring(0, responseText.Length - 3); // Bỏ ```
                }
                responseText = responseText.Trim();

                // Lưu responseJson để log
                responseJson = responseText;

                var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                result.Title = jsonResult.ContainsKey("title") ? jsonResult["title"] : "";
                result.AltText = jsonResult.ContainsKey("altText") ? jsonResult["altText"] : "";
                result.Description = jsonResult.ContainsKey("description") ? jsonResult["description"] : "";
                result.PageNumber = jsonResult.ContainsKey("pageNumber") ? jsonResult["pageNumber"] : null;
            }
            catch (Exception ex)
            {
                result.Error = $"Lỗi khi gọi Claude API: {ex.Message}";
                MyLogger.GetInstance().Error($"GenerateImageAltTextAsync error: {ex}");
            }
            finally
            {
                // Insert log và chờ hoàn thành
                await InsertLogAsync(imagePath, prompt, responseJson);
            }

            return result;
        }

        /// <summary>
        /// Phần JSON format chung cho tất cả prompts
        /// </summary>
        private static string GetJsonFormatSection(bool includePageNumber = false)
        {
            if (includePageNumber)
            {
                return @"{
                            ""title"": ""[Tiêu đề ngắn gọn]"",
                            ""altText"": ""[Alt text tối ưu SEO]"",
                            ""description"": ""[Mô tả chi tiết]"",
                            ""pageNumber"": ""[Số trang - VD: '5' hoặc '2-3', null nếu không thấy]""
                        }";
            }

            return @"{
                        ""title"": ""[Tiêu đề ngắn gọn]"",
                        ""altText"": ""[Alt text tối ưu SEO]"",
                        ""description"": ""[Mô tả chi tiết]""
                    }";
        }

        /// <summary>
        /// Yêu cầu chung cho tất cả prompts
        /// </summary>
        private static string GetCommonRequirements()
        {
            return "\n\nTrả về ĐÚNG format JSON, không thêm text nào khác";
        }

        /// <summary>
        /// Tạo phần thông tin sách từ parameters
        /// </summary>
        private static string GetBookInfoSection(
            BookImageType imageType,
            string bookName,
            string author,
            string bookFormat,
            string publisher,
            int? minAge = null,
            int? maxAge = null)
        {
            var bookInfoLines = new List<string>();
            if (!string.IsNullOrEmpty(bookName))
                bookInfoLines.Add($"- Tên sách: {bookName}");
            if (imageType == BookImageType.Cover)
            {
                if (!string.IsNullOrEmpty(author))
                    bookInfoLines.Add($"- Tác giả: {author}");
                if (!string.IsNullOrEmpty(bookFormat))
                    bookInfoLines.Add($"- Loại: {bookFormat}");
                if (!string.IsNullOrEmpty(publisher))
                    bookInfoLines.Add($"- Nhà xuất bản: {publisher}");

                // Thêm độ tuổi phù hợp (cho sách trẻ em)
                // Dùng hàm SanPham.GetAgeRangeText() để convert tháng → năm
                string ageRange = SanPham.GetAgeRangeText(minAge, maxAge);
                if (ageRange != string.Empty)
                {
                    bookInfoLines.Add($"- Độ tuổi phù hợp: {ageRange}");
                }
            }

            if (imageType == BookImageType.Cover)
            {
                return bookInfoLines.Count > 0
                ? "\n\nTHÔNG TIN SÁCH (dùng chính xác những thông tin này, KHÔNG đọc lại từ ảnh):\n" + string.Join("\n", bookInfoLines)
                : "";
            }

            return bookInfoLines.Count > 0
                ? "\n\n" + string.Join("\n", bookInfoLines)
                : "";
        }

        /// <summary>
        /// Tạo prompt phù hợp với từng loại ảnh sách
        /// </summary>
        private static string GetPromptForImageType(
            BookImageType imageType,
            string bookName = null,
            string author = null,
            string bookFormat = null,
            string publisher = null,
            int? minAge = null,
            int? maxAge = null)
        {
            string bookInfo = GetBookInfoSection(imageType, bookName, author, bookFormat, publisher, minAge, maxAge);
            string jsonFormat = GetJsonFormatSection();
            string commonReqs = GetCommonRequirements();

            switch (imageType)
            {
                case BookImageType.Cover:
                    return $@"Hãy phân tích ảnh BÌA SÁCH này và trả về JSON với format sau:{bookInfo}

{jsonFormat}

Yêu cầu cho Title (5-10 từ):
- Format ngắn gọn: ""[Tên sách] - [Tác giả]"" hoặc chỉ ""[Tên sách]""
- SỬ DỤNG thông tin từ THÔNG TIN SÁCH
- Ví dụ: ""Harry Potter - J.K. Rowling"", ""Đắc Nhân Tâm""

Yêu cầu cho AltText (ẢNH BÌA - TỐI ƯU SEO):
- KHÔNG bắt đầu bằng ""ảnh bìa"", ""hình ảnh"", ""bức ảnh""
- SỬ DỤNG chính xác tên sách, tác giả, loại sách từ THÔNG TIN SÁCH (nếu có)
- KHÔNG tự ý đọc hoặc suy đoán tên sách từ ảnh nếu đã có thông tin
- Nếu có ĐỘ TUỔI PHÙ HỢP trong THÔNG TIN SÁCH: BẮT BUỘC bao gồm trong alt text
- Format LINH HOẠT, TÊN SÁCH luôn ở đầu:
  * ""[Tên sách] [loại sách] cho bé [độ tuổi] của [Tác giả]""
  * ""[Tên sách] [độ tuổi] - [Tác giả], bìa [màu sắc]""
  * ""[Tên sách] [loại sách] của [Tác giả], [màu sắc]"" (nếu không có độ tuổi)
- VIẾT TỰ NHIÊN, không cứng nhắc một format duy nhất
- Độ dài: 50-125 ký tự
- Ví dụ:
  * ""Harry Potter bìa cứng cho bé 8-12 tuổi của J.K. Rowling, bìa đỏ đậm""
  * ""Ehon Hạt Mầm cho bé 3-6 tuổi - NXB Kim Đồng, bìa màu vàng sáng""
  * ""Đắc Nhân Tâm - Dale Carnegie, bìa vàng bìa mềm"" (không có độ tuổi)

Yêu cầu cho Description (20-40 từ):
- Mô tả hình ảnh NHÌN THẤY trên bìa: màu sắc, hình vẽ, bố cục, tình trạng sách
- KHÔNG lặp lại thông tin đã có (tên, tác giả) - chỉ mô tả visual{commonReqs}";

                case BookImageType.InsidePage:
                    // Inside Page cần detect page number
                    string insidePageJsonFormat = GetJsonFormatSection(includePageNumber: true);
                    return $@"Hãy phân tích ảnh TRANG TRONG SÁCH này và trả về JSON với format sau:{bookInfo}

{insidePageJsonFormat}

Yêu cầu cho Title (5-10 từ):
- CHỈ mô tả nội dung chính của trang: chủ đề/bài học/cảnh minh họa
- KHÔNG bắt đầu bằng ""Trang trong"", ""Ảnh trang"", ""Hình ảnh""
- Ví dụ: ""Bài học về chia sẻ và tình bạn"", ""Minh họa rừng thần kỳ""

Yêu cầu cho AltText (ẢNH TRANG TRONG - SEO):
- XỬ LÝ TÊN SÁCH DÀI:
  * Nếu TÊN SÁCH ngắn (<40 ký tự): Dùng tên đầy đủ
  * Nếu TÊN SÁCH dài (>40 ký tự): RÚT GỌN (giữ từ khóa chính) hoặc BỎ TÊN, chỉ mô tả nội dung
  * VD rút gọn: ""Sách Ehon Kích Thích Thị Giác Cho Bé - Hạt Mầm Của Mẹ"" → ""Ehon Hạt Mầm""
- Format LINH HOẠT:
  * ""[Tên/Tên rút gọn]: [mô tả nội dung] trang [số]""
  * ""[Tên/Tên rút gọn] chương X: [nội dung]""
  * ""[Mô tả nội dung chi tiết]"" (bỏ tên nếu quá dài)
- KHÔNG bắt buộc ""Trang trong cuốn..."", viết TỰ NHIÊN
- Độ dài: 50-125 ký tự
- Ví dụ:
  * ""Harry Potter: Minh họa Harry đi tàu Hogwarts trang 23""
  * ""Ehon Hạt Mầm: Cảnh động vật với màu sắc rực rỡ""
  * ""Minh họa màu nước về tình bạn và chia sẻ trang 5""

Yêu cầu cho Description (20-40 từ):
- Mô tả chi tiết nội dung NHÌN THẤY: văn bản (OCR nếu đọc được), hình minh họa, bố cục, màu sắc, số trang
- KHÔNG lặp lại ""Trang trong cuốn..."" - đi thẳng vào mô tả visual
- Ví dụ: ""Trang sách minh họa màu nước với cảnh các bạn nhỏ cùng chơi trong vườn hoa, có đoạn văn về tình bạn ở góc dưới""

QUAN TRỌNG - Yêu cầu cho PageNumber:
- Đọc CHÍNH XÁC số trang xuất hiện trong ảnh (thường ở góc trên/dưới trang)
- Format trả về:
  * Nếu 1 trang: ""5"" (chỉ số, không có chữ ""trang"")
  * Nếu 2 trang liền nhau (spread): ""2-3"" (dạng range)
  * Nếu KHÔNG THẤY số trang: null (không phải string ""null"", mà JSON null)
- Ví dụ pageNumber hợp lệ: ""5"", ""12"", ""2-3"", ""10-11"", null
- KHÔNG thêm chữ ""trang"", ""page"", chỉ GHI SỐ{commonReqs}";

                case BookImageType.BackCover:
                    return $@"Hãy phân tích ảnh MẶT SAU SÁCH này và trả về JSON với format sau:{bookInfo}

{jsonFormat}

Yêu cầu cho Title (5-10 từ):
- Mô tả nội dung chính trên mặt sau
- KHÔNG bắt đầu bằng ""Mặt sau"", ""Ảnh mặt sau""
- Ví dụ: ""Tóm tắt và barcode ISBN"", ""Giới thiệu tác giả""

Yêu cầu cho AltText (ẢNH MẶT SAU - SEO):
- XỬ LÝ TÊN SÁCH DÀI:
  * Nếu TÊN SÁCH ngắn (<40 ký tự): Dùng tên đầy đủ
  * Nếu TÊN SÁCH dài (>40 ký tự): RÚT GỌN hoặc BỎ TÊN, tập trung mô tả nội dung
- Format LINH HOẠT:
  * ""[Tên/Tên rút gọn]: [nội dung] trên bìa sau""
  * ""[Tên/Tên rút gọn] bìa sau - [mô tả]""
  * ""Bìa sau với [chi tiết nội dung]"" (bỏ tên nếu quá dài)
- KHÔNG bắt buộc ""Mặt sau sách..."", viết TỰ NHIÊN
- Nội dung chính: tóm tắt/barcode/giá/review/giới thiệu tác giả...
- Độ dài: 50-125 ký tự
- Ví dụ:
  * ""Harry Potter: Tóm tắt nội dung và barcode trên bìa sau""
  * ""Ehon Hạt Mầm bìa sau với thông tin NXB và giá""
  * ""Bìa sau với tóm tắt câu chuyện và barcode ISBN""

Yêu cầu cho Description (20-40 từ):
- Mô tả những gì NHÌN THẤY trên mặt sau: tóm tắt, giới thiệu, barcode, giá, logo NXB, review...
- KHÔNG lặp lại ""Mặt sau sách..."" - đi thẳng vào mô tả{commonReqs}";

                case BookImageType.Spine:
                    return $@"Hãy phân tích ảnh GÁY SÁCH này và trả về JSON với format sau:{bookInfo}

{jsonFormat}

Yêu cầu cho Title (5-10 từ):
- Format: ""[Tên sách] - [Tác giả]"" hoặc chỉ ""[Tên sách]""
- SỬ DỤNG thông tin từ THÔNG TIN SÁCH
- Ví dụ: ""Harry Potter - J.K. Rowling""

Yêu cầu cho AltText (ẢNH GÁY SÁCH - SEO):
- XỬ LÝ TÊN SÁCH DÀI:
  * Nếu TÊN SÁCH ngắn (<40 ký tự): Dùng tên đầy đủ
  * Nếu TÊN SÁCH dài (>40 ký tự): RÚT GỌN hoặc BỎ TÊN
- Format LINH HOẠT:
  * ""[Tên/Tên rút gọn] - [Tác giả], gáy [màu/độ dày]""
  * ""[Tên/Tên rút gọn] gáy [màu sắc] [độ dày]""
  * ""Gáy sách [màu sắc] [độ dày] với [chi tiết]"" (bỏ tên nếu quá dài)
- KHÔNG bắt buộc ""Gáy sách..."" ở đầu, viết TỰ NHIÊN
- Độ dài: 40-100 ký tự
- Ví dụ:
  * ""Harry Potter - J.K. Rowling, gáy đỏ 3cm với chữ vàng""
  * ""Ehon Hạt Mầm gáy vàng nổi bật, dày 1.5cm""
  * ""Gáy sách xanh dương mỏng 1cm với logo NXB""

Yêu cầu cho Description (20-40 từ):
- Mô tả NHÌN THẤY: chữ in trên gáy, màu sắc, độ dày sách, logo NXB, họa tiết
- KHÔNG lặp lại ""Gáy sách..."" - đi thẳng vào mô tả{commonReqs}";

                case BookImageType.FullView:
                    return $@"Hãy phân tích ảnh TOÀN CẢNH SÁCH này và trả về JSON với format sau:{bookInfo}

{jsonFormat}

Yêu cầu cho Title (5-10 từ):
- Mô tả góc nhìn/cách bày trí
- KHÔNG bắt đầu bằng ""Ảnh toàn cảnh"", ""Hình ảnh""
- Ví dụ: ""Nhiều góc nhìn bìa và gáy"", ""Xoay 360 độ""

Yêu cầu cho AltText (ẢNH TOÀN CẢNH - SEO):
- XỬ LÝ TÊN SÁCH DÀI:
  * Nếu TÊN SÁCH ngắn (<40 ký tự): Dùng tên đầy đủ
  * Nếu TÊN SÁCH dài (>40 ký tự): RÚT GỌN hoặc BỎ TÊN
- Format LINH HOẠT:
  * ""[Tên/Tên rút gọn]: [góc nhìn/cách bày trí]""
  * ""[Tên/Tên rút gọn] nhiều góc độ - [mô tả]""
  * ""Toàn cảnh sách [chi tiết góc chụp]"" (bỏ tên nếu quá dài)
- Mô tả cách bày trí: để ngang/dựng đứng/nhiều góc độ/xoay 360...
- VIẾT TỰ NHIÊN, không template cứng
- Độ dài: 50-125 ký tự
- Ví dụ:
  * ""Harry Potter: Nhiều góc nhìn với bìa, gáy và mặt sau""
  * ""Ehon Hạt Mầm toàn cảnh xoay 360 độ, nền trắng""
  * ""Toàn cảnh sách nhiều góc độ trên nền gỗ""

Yêu cầu cho Description (20-40 từ):
- Mô tả toàn cảnh NHÌN THẤY: những phần nào của sách hiển thị, góc chụp, bố cục, background
- KHÔNG lặp lại tên sách - chỉ mô tả visual{commonReqs}";

                default:
                    return GetPromptForImageType(BookImageType.Cover);
            }
        }

        /// <summary>
        /// Insert log vào database (không throw exception để không ảnh hưởng main flow)
        /// </summary>
        private static async Task InsertLogAsync(string imagePath, string prompt, string responseJson)
        {
            try
            {
                using (var conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string sql = @"INSERT INTO tb_claude_alttext_log
                                   (ImagePath, Prompt, ResponseJson, CreatedDate)
                                   VALUES (@ImagePath, @Prompt, @ResponseJson, NOW())";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@ImagePath", MySqlDbType.VarChar).Value = imagePath ?? "";
                        cmd.Parameters.Add("@Prompt", MySqlDbType.Text).Value = prompt ?? "";
                        cmd.Parameters.Add("@ResponseJson", MySqlDbType.Text).Value = responseJson ?? "";

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error nhưng không throw để không ảnh hưởng main flow
                MyLogger.GetInstance().Warn($"InsertLogAsync error (non-critical): {ex.Message}");
            }
        }

        /// <summary>
        /// Detect MIME type thực tế của ảnh từ file bytes (magic number)
        /// </summary>
        /// <param name="imageBytes">Byte array của ảnh</param>
        /// <returns>MIME type string (image/webp, image/jpeg, image/png, image/gif)</returns>
        private static string DetectImageMediaType(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length < 12)
            {
                return "image/webp"; // Fallback mặc định
            }

            // WebP: RIFF....WEBP (bytes 0-3: RIFF, bytes 8-11: WEBP)
            if (imageBytes.Length >= 12 &&
                imageBytes[0] == 0x52 && imageBytes[1] == 0x49 &&
                imageBytes[2] == 0x46 && imageBytes[3] == 0x46 &&
                imageBytes[8] == 0x57 && imageBytes[9] == 0x45 &&
                imageBytes[10] == 0x42 && imageBytes[11] == 0x50)
            {
                return "image/webp";
            }

            // JPEG: FF D8 FF
            if (imageBytes[0] == 0xFF && imageBytes[1] == 0xD8 && imageBytes[2] == 0xFF)
            {
                return "image/jpeg";
            }

            // PNG: 89 50 4E 47 0D 0A 1A 0A
            if (imageBytes.Length >= 8 &&
                imageBytes[0] == 0x89 && imageBytes[1] == 0x50 &&
                imageBytes[2] == 0x4E && imageBytes[3] == 0x47 &&
                imageBytes[4] == 0x0D && imageBytes[5] == 0x0A &&
                imageBytes[6] == 0x1A && imageBytes[7] == 0x0A)
            {
                return "image/png";
            }

            // GIF: 47 49 46 38 (GIF8)
            if (imageBytes.Length >= 4 &&
                imageBytes[0] == 0x47 && imageBytes[1] == 0x49 &&
                imageBytes[2] == 0x46 && imageBytes[3] == 0x38)
            {
                return "image/gif";
            }

            // Fallback: Nếu không detect được, trả về webp
            return "image/webp";
        }
    }

}
