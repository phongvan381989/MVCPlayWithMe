
namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Config
{
    /// <summary>
    /// Chứa cấu hình để connect tới shop Tiki gồm: appID, homeAddress, secretAppCode
    /// </summary>
    public class TikiConfigApp
    {
        public const string constUsingApp = "Đang Sử Dụng";
        public const string constNotUsingApp = "Không Sử Dụng";
        public TikiConfigApp()
        {
            Empty();
        }
        public string appID { get; set; }
        public string homeAddress { get; set; }
        public string secretAppCode { get; set; }
        public string usingApp { get; set; } // Nhận 1 trong 2 giá trị: Đang Sử Dụng hoặc Không Sử Dụng
        public TikiAuthorization tikiAu{get; set;}

        public TikiConfigApp(string inputAppID, string inputHomeAddress, string inputSecretAppCode, string inputUsingApp, TikiAuthorization tikiAuthorization)
        {
            appID = inputAppID;
            homeAddress = inputHomeAddress;
            secretAppCode = inputSecretAppCode;
            usingApp = inputUsingApp;
            tikiAu = tikiAuthorization;
        }

        public TikiConfigApp(TikiConfigApp dataTikiConfigApp)
        {
            appID = dataTikiConfigApp.appID;
            homeAddress = dataTikiConfigApp.homeAddress;
            secretAppCode = dataTikiConfigApp.secretAppCode;
            usingApp = dataTikiConfigApp.usingApp;
            tikiAu = dataTikiConfigApp.tikiAu;
        }
        public void Empty()
        {
            appID = string.Empty;
            homeAddress = string.Empty;
            secretAppCode = string.Empty;
            usingApp = constNotUsingApp;
            tikiAu = new TikiAuthorization();
        }

        public void SetAllValue(string inputAppID, string inputHomeAddress, string inputSecretAppCode, string inputUsingApp, TikiAuthorization inputTikiAu)
        {
            appID = inputAppID;
            homeAddress = inputHomeAddress;
            secretAppCode = inputSecretAppCode;
            usingApp = inputUsingApp;
            tikiAu = inputTikiAu;
        }
    }
}
