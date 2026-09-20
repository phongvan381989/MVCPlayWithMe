using MVCPlayWithMe.General;
using NUglify.Css;
using NUglify.JavaScript;
using System.Web;
using System.Web.Optimization;

namespace MVCPlayWithMe
{
    public class BundleConfig
    {
        public static void ChangeJsMinifierToNUglify(ScriptBundle scriptBundle)
        {
            scriptBundle.Transforms.Clear();
            scriptBundle.Transforms.Add(new NUglifyJsMinifier());
        }

        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // ========================================
            // Vẫn dùng CssMinify() => gọi tới WebGrease
            // ========================================

            var cssCommon = new StyleBundle("~/bundles/css/common");
            cssCommon.Include(
                "~/Content/Normalize.css",
                "~/Content/web.play.with.me.common.css",
                "~/Content/web.play.with.me.modal.common.css");
            //cssCommon.Transforms.Clear(); // Remove WebGrease default
            //cssCommon.Transforms.Add(new CssMinify());
            bundles.Add(cssCommon);

            var cssCommonAdmin = new StyleBundle("~/bundles/css/common_admin");
            cssCommonAdmin.Include(
                "~/Content/Normalize.css",
                "~/Content/web.play.with.me.common.css");
            //cssCommonAdmin.Transforms.Clear(); // Remove WebGrease default
            //cssCommonAdmin.Transforms.Add(new CssMinify());
            bundles.Add(cssCommonAdmin);


            // Page-specific CSS: Search page
            var cssSearch = new StyleBundle("~/bundles/css/search");
            cssSearch.Include("~/Content/Home/Search.css");
            //cssSearch.Transforms.Clear();
            //cssSearch.Transforms.Add(new CssMinify());
            bundles.Add(cssSearch);

            // Page-specific CSS: SanPham (product detail) page
            var cssSanPham = new StyleBundle("~/bundles/css/sanpham");
            cssSanPham.Include("~/Content/Home/SanPham.css");
            //cssSanPham.Transforms.Clear();
            //cssSanPham.Transforms.Add(new CssMinify());
            bundles.Add(cssSanPham);

            var cssCart = new StyleBundle("~/bundles/css/cart");
            cssCart.Include("~/Content/Home/Cart.css");
            //cssCart.Transforms.Clear();
            //cssCart.Transforms.Add(new CssMinify());
            bundles.Add(cssCart);

            var cssCheckout = new StyleBundle("~/bundles/css/checkout");
            cssCheckout.Include("~/Content/Home/Checkout.css");
            //cssCheckout.Transforms.Clear();
            //cssCheckout.Transforms.Add(new CssMinify());
            bundles.Add(cssCheckout);

            //< link rel = "stylesheet" href = "~/Content/web.play.with.me.modal.common.css?v=20260610" >
            //< link rel = "stylesheet" href = "~/Content/web.play.with.me.modal.input.css?v=20260610" >
            //< link rel = "stylesheet" href = "~/Content/Customer/AccountInfor.css?v=20260610" >
            var cssAccountInfor = new StyleBundle("~/bundles/css/customer/account_infor");
            cssAccountInfor.Include("~/Content/web.play.with.me.modal.common.css",
                "~/Content/web.play.with.me.modal.input.css",
                "~/Content/Customer/AccountInfor.css");
            //cssAccountInfor.Transforms.Clear();
            //cssAccountInfor.Transforms.Add(new CssMinify());
            bundles.Add(cssAccountInfor);

            // <link rel="stylesheet" href="~/Content/web.play.with.me.modal.input.css?v=20260610">
            var cssModalInput = new StyleBundle("~/bundles/css/modal/input");
            cssModalInput.Include("~/Content/web.play.with.me.modal.input.css");
            //cssModalInput.Transforms.Clear();
            //cssModalInput.Transforms.Add(new CssMinify());
            bundles.Add(cssModalInput);

            // <link rel="stylesheet" href="~/Content/Customer/Order.css?v=20260610">
            var cssCustomerOrder = new StyleBundle("~/bundles/css/customer/order");
            cssCustomerOrder.Include("~/Content/Customer/Order.css");
            //cssCustomerOrder.Transforms.Clear();
            //cssCustomerOrder.Transforms.Add(new CssMinify());
            bundles.Add(cssCustomerOrder);


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
            ChangeJsMinifierToNUglify(jsCommon);
            bundles.Add(jsCommon);

            // Page-specific JS: Search page
            var jsSearch = new ScriptBundle("~/bundles/js/search");
            jsSearch.Include("~/Scripts/Home/Search.js");
            ChangeJsMinifierToNUglify(jsSearch);
            bundles.Add(jsSearch);

            // Page-specific JS: SanPham (product detail) page
            var jsSanPham = new ScriptBundle("~/bundles/js/sanpham");
            jsSanPham.Include("~/Scripts/Home/SanPham.js");
            ChangeJsMinifierToNUglify(jsSanPham);
            bundles.Add(jsSanPham);

            var jsCart = new ScriptBundle("~/bundles/js/cart");
            jsCart.Include("~/Scripts/Home/Cart.js");
            ChangeJsMinifierToNUglify(jsCart);
            bundles.Add(jsCart);

            //< script defer src = "~/Scripts/web.play.with.me.customer.infor.cookie.js?v=20260610" ></ script >
            //< script defer src = "~/Scripts/web.play.with.me.address.js?v=20260610" ></ script >
            //< script defer src = "~/Scripts/Customer/CommonOrder.js?v=20260610" ></ script >
            //<script defer src="~/Scripts/Home/Checkout.js?v=20260610"></script>

            var jsCheckout = new ScriptBundle("~/bundles/js/checkout");
            jsCheckout.Include(
                "~/Scripts/web.play.with.me.customer.infor.cookie.js",
                "~/Scripts/web.play.with.me.address.js",
                "~/Scripts/Customer/CommonOrder.js",
                "~/Scripts/Home/Checkout.js");
            ChangeJsMinifierToNUglify(jsCheckout);
            bundles.Add(jsCheckout);

            var jsPaymentQR = new ScriptBundle("~/bundles/js/policy/paymentQR");
            jsPaymentQR.Include(
                "~/Scripts/web.play.with.me.common.js",
                "~/Scripts/Policy/PaymentQR.js");
            ChangeJsMinifierToNUglify(jsPaymentQR);
            bundles.Add(jsPaymentQR);

            //< script src = "~/Scripts/web.play.with.me.customer.infor.cookie.js?v=20260610" ></ script >
            //< script src = "~/Scripts/web.play.with.me.address.js?v=20260610" ></ script >
            //< script src = "~/Scripts/Customer/AccountInfor.js?v=20260610" ></ script >
            var jsAccountInfor = new ScriptBundle("~/bundles/js/customer/account_infor");
            jsAccountInfor.Include(
                "~/Scripts/web.play.with.me.customer.infor.cookie.js",
                "~/Scripts/web.play.with.me.address.js",
                "~/Scripts/Customer/AccountInfor.js");
            ChangeJsMinifierToNUglify(jsAccountInfor);
            bundles.Add(jsAccountInfor);

            //    <script src="~/Scripts/Customer/CreateCustomer.js?v=20260610"></script>
            var jsCreateCustomer = new ScriptBundle("~/bundles/js/customer/create_customer");
            jsCreateCustomer.Include("~/Scripts/Customer/CreateCustomer.js");
            ChangeJsMinifierToNUglify(jsCreateCustomer);
            bundles.Add(jsCreateCustomer);

            //< script src = "~/Scripts/web.play.with.me.common.js?v=20260916" ></ script >
            //< script src = "~/Scripts/web.play.with.me.customer.infor.cookie.js?v=20260610" ></ script >
            //< script src = "~/Scripts/cart-manager.js" ></ script >
            //< script src = "~/Scripts/Customer/Login.js?v=20260610" ></ script >
            var jsCustomerLogin = new ScriptBundle("~/bundles/js/customer/login");
            jsCustomerLogin.Include(
                "~/Scripts/web.play.with.me.common.js",
                "~/Scripts/web.play.with.me.customer.infor.cookie.js",
                "~/Scripts/cart-manager.js",
                "~/Scripts/Customer/Login.js");
            ChangeJsMinifierToNUglify(jsCustomerLogin);
            bundles.Add(jsCustomerLogin);

            //< script src = "~/Scripts/Customer/CommonOrder.js?v=20260610" ></ script >
            //< script src = "~/Scripts/Customer/Order.js?v=20260610" ></ script >
            var jsCustomerOrder = new ScriptBundle("~/bundles/js/customer/order");
            jsCustomerOrder.Include(
                "~/Scripts/Customer/CommonOrder.js",
                "~/Scripts/Customer/Order.js");
            ChangeJsMinifierToNUglify(jsCustomerOrder);
            bundles.Add(jsCustomerOrder);

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
            //BundleTable.EnableOptimizations = true;
        }
    }
}
