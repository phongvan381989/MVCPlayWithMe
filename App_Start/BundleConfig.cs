using System.Web;
using System.Web.Optimization;
using NUglify.JavaScript;
using NUglify.Css;

namespace MVCPlayWithMe
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // ========================================
            // CSS BUNDLES (with NUglify minifier)
            // ========================================

            var cssCommon = new StyleBundle("~/bundles/css/common");
            cssCommon.Include(
                "~/Content/Normalize.css",
                "~/Content/web.play.with.me.common.css",
                "~/Content/web.play.with.me.modal.common.css");
            cssCommon.Transforms.Clear(); // Remove WebGrease default
            cssCommon.Transforms.Add(new CssMinify());
            bundles.Add(cssCommon);

            var cssCommonAdmin = new StyleBundle("~/bundles/css/commonAdmin");
            cssCommonAdmin.Include(
                "~/Content/Normalize.css",
                "~/Content/web.play.with.me.common.css");
            cssCommonAdmin.Transforms.Clear(); // Remove WebGrease default
            cssCommonAdmin.Transforms.Add(new CssMinify());
            bundles.Add(cssCommonAdmin);


            // Page-specific CSS: Search page
            var cssSearch = new StyleBundle("~/bundles/css/search");
            cssSearch.Include("~/Content/Home/Search.css");
            cssSearch.Transforms.Clear();
            cssSearch.Transforms.Add(new CssMinify());
            bundles.Add(cssSearch);

            // Page-specific CSS: SanPham (product detail) page
            var cssSanPham = new StyleBundle("~/bundles/css/sanpham");
            cssSanPham.Include("~/Content/Home/SanPham.css");
            cssSanPham.Transforms.Clear();
            cssSanPham.Transforms.Add(new CssMinify());
            bundles.Add(cssSanPham);

            var cssCart = new StyleBundle("~/bundles/css/cart");
            cssCart.Include("~/Content/Home/Cart.css");
            cssCart.Transforms.Clear();
            cssCart.Transforms.Add(new CssMinify());
            bundles.Add(cssCart);

            var cssCheckout = new StyleBundle("~/bundles/css/checkout");
            cssCheckout.Include("~/Content/Home/Checkout.css");
            cssCheckout.Transforms.Clear();
            cssCheckout.Transforms.Add(new CssMinify());
            bundles.Add(cssCheckout);

            // KHÔNG bundle PhotoSwipe CSS - async load riêng, chỉ cần khi user click ảnh

            // ========================================
            // JAVASCRIPT BUNDLES (with NUglify minifier)
            // ========================================

            // Common JS: Utilities + Cart (dùng cho tất cả pages)
            var jsCommon = new ScriptBundle("~/bundles/js/common");
            jsCommon.Include(
                "~/Scripts/web.play.with.me.common.js",
                "~/Scripts/cart-manager.js",
                "~/Scripts/web.play.with.me.action.common.js");
            jsCommon.Transforms.Clear();
            jsCommon.Transforms.Add(new NUglifyJsMinifier());
            bundles.Add(jsCommon);

            // Page-specific JS: Search page
            var jsSearch = new ScriptBundle("~/bundles/js/search");
            jsSearch.Include("~/Scripts/Home/Search.js");
            jsSearch.Transforms.Clear();
            jsSearch.Transforms.Add(new NUglifyJsMinifier());
            bundles.Add(jsSearch);

            // Page-specific JS: SanPham (product detail) page
            var jsSanPham = new ScriptBundle("~/bundles/js/sanpham");
            jsSanPham.Include("~/Scripts/Home/SanPham.js");
            jsSanPham.Transforms.Clear(); 
            jsSanPham.Transforms.Add(new NUglifyJsMinifier());
            bundles.Add(jsSanPham);

            var jsCart = new ScriptBundle("~/bundles/js/cart");
            jsCart.Include("~/Scripts/Home/Cart.js");
            jsCart.Transforms.Clear();
            jsCart.Transforms.Add(new NUglifyJsMinifier());
            bundles.Add(jsCart);

            var jsCheckout = new ScriptBundle("~/bundles/js/checkout");
            jsCheckout.Include("~/Scripts/Home/Checkout.js");
            jsCheckout.Transforms.Clear();
            jsCheckout.Transforms.Add(new NUglifyJsMinifier());
            bundles.Add(jsCheckout);

            var jsPaymentQR = new ScriptBundle("~/bundles/js/policy/paymentQR");
            jsPaymentQR.Include(
                "~/Scripts/web.play.with.me.common.js",
                "~/Scripts/Policy/PaymentQR.js");
            jsPaymentQR.Transforms.Clear();
            jsPaymentQR.Transforms.Add(new NUglifyJsMinifier());
            bundles.Add(jsPaymentQR);

            // KHÔNG bundle: PhotoSwipe JS (defer riêng), tracking scripts (inline)

            // ========================================
            // BUNDLE CONFIGURATION
            // ========================================

            // ========================================
            // NUGLIFY CONFIGURATION
            // ========================================

            // ✅ NUglify: Modern minifier support ES6+ (arrow functions, template literals, const/let, etc.)
            // ✅ Replace WebGrease (old, không support ES6) → NUglify (modern, maintained)

            // NUglify tự động được dùng khi:
            // - Production: debug="false" trong Web.config
            // - Or force enable: BundleTable.EnableOptimizations = true

            // Development (debug="true"):
            //   → Serve individual files, KHÔNG minify (dễ debug)
            // Production (debug="false"):
            //   → Combine + minify với NUglify

            // Optional: Force enable minification ngay cả debug mode (test locally)
            BundleTable.EnableOptimizations = true;
        }
    }
}
