using System;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.SanPhamModel;

namespace MVCPlayWithMe.Controllers
{
    public class FeedController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Facebook Product Catalog Feed
        /// URL: /feed/facebook (không cần .xml extension)
        /// Format: RSS 2.0 with Google Shopping namespace
        /// Cache 1 tiếng để Facebook crawl không làm đơ server
        /// </summary>
        [HttpGet]
        [OutputCache(Duration = 3600)] // Cache 1 tiếng
        public async Task<ContentResult> Facebook()
        {
            try
            {
                // Load sản phẩm đang kinh doanh (Status = 0)
                var allSanPhams = await SanPhamMySql.GetSanPhamForFeedInfoAsync();

                // Build XML
                var sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.AppendLine("<rss version=\"2.0\" xmlns:g=\"http://base.google.com/ns/1.0\">");
                sb.AppendLine("<channel>");
                sb.AppendLine("<title>Tiệm sách Voi Bé Nhỏ</title>");
                sb.AppendLine($"<link>{Common.httpsVoiBeNho}</link>");
                sb.AppendLine("<description>Toàn bộ sách của Voibenho</description>");

                foreach (var sp in allSanPhams)
                {
                    // Escape ký tự đặc biệt trong tên sách (XML entities)
                    string title = SecurityElement.Escape(sp.Name.Length > 150 ? sp.Name.Substring(0, 150) : sp.Name);
                    string description = SecurityElement.Escape((sp.Detail ?? sp.Name).Length > 1000 ? (sp.Detail ?? sp.Name).Substring(0, 1000) : (sp.Detail ?? sp.Name));

                    // Image URL
                    // Ảnh phải > 500x500px, không được là ảnh trắng xóa
                    string imageUrl = Common.GenerateAbsoluteFallbackImageUrl();
                    if (!string.IsNullOrEmpty(sp.CoverImageFileName))
                    {
                        imageUrl = Common.GenerateAbsoluteSanPhamMediaUrl(sp.CoverImageFileName, sp.Id);
                    }

                    // Product URL
                    string productUrl = Common.GenerateAbsoluteSanPhamUrlForCustomer(sp.Name, sp.Id);

                    // Giá phải để dạng "150000 VND" - Facebook mới hiểu
                    string price = $"{sp.SalePrice} VND";

                    sb.AppendLine("<item>");
                    sb.AppendLine($"<g:id>{sp.Id}</g:id>");
                    sb.AppendLine($"<g:title>{title}</g:title>");
                    sb.AppendLine($"<g:description>{description}</g:description>");
                    sb.AppendLine($"<g:link>{productUrl}</g:link>");
                    sb.AppendLine($"<g:image_link>{imageUrl}</g:image_link>");
                    sb.AppendLine($"<g:availability>{(sp.Quantity > 0 ? "in stock" : "out of stock")}</g:availability>");
                    sb.AppendLine("<g:condition>new</g:condition>");
                    sb.AppendLine($"<g:price>{price}</g:price>");
                    sb.AppendLine("<g:brand>Voibenho</g:brand>");

                    // Optional: Category
                    if (!string.IsNullOrEmpty(sp.CategoryName))
                    {
                        string category = SecurityElement.Escape(Common.GenerateCategoryFromTikiCategory(sp.CategoryName));
                        sb.AppendLine($"<g:product_type>{category}</g:product_type>");
                    }

                    sb.AppendLine("</item>");
                }

                sb.AppendLine("</channel></rss>");

                return Content(sb.ToString(), "application/xml", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"Facebook Feed error: {ex.Message}");

                // Return error XML
                var errorXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                              "<rss version=\"2.0\"><channel><title>Error</title></channel></rss>";
                return Content(errorXml, "application/xml");
            }
        }
    }
}
