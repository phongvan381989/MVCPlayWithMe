using System.Web.Optimization;
using MVCPlayWithMe.General;
using NUglify;

public class NUglifyJsMinifier : IBundleTransform
{
    public void Process(BundleContext context, BundleResponse response)
    {
        // Nếu EnableOptimizations = false (đang Debug), DỪNG NGAY KHÔNG LÀM GÌ CẢ
        if (!BundleTable.EnableOptimizations)
        {
            return;
        }

        // Sử dụng NUglify để nén JS (hỗ trợ đầy đủ ES6+)
        var result = Uglify.Js(response.Content);

        // Nếu nén thành công và không có lỗi cú pháp nghiêm trọng
        if (!result.HasErrors)
        {
            response.Content = result.Code;
        }
        else
        {
            // Trong trường hợp có lỗi, giữ nguyên code gốc chưa nén
            // để ứng dụng không bị sập (NullReferenceException)
            System.Diagnostics.Debug.WriteLine("NUglify JS Minify Errors: " + string.Join("\n", result.Errors));
            MyLogger.GetInstance().Error("NUglify JS Minify Errors: " + string.Join("\n", result.Errors));
        }

        response.ContentType = "text/javascript";
    }
}
