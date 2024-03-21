
using System;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Config
{
    /// <summary>
    /// Chứa cấu hình để connect tới shop Tiki gồm: appID, homeAddress, secretAppCode
    /// </summary>
    public class TikiConfigApp
    {
        public TikiConfigApp()
        {
            Empty();
        }
        public string appID { get; set; }
        public string homeAddress { get; set; }
        public string secretAppCode { get; set; }
        public TikiAuthorization tikiAu{get; set;}

        public TikiConfigApp(string inputAppID, string inputHomeAddress, string inputSecretAppCode, string inputUsingApp, TikiAuthorization tikiAuthorization)
        {
            appID = inputAppID;
            homeAddress = inputHomeAddress;
            secretAppCode = inputSecretAppCode;
            tikiAu = tikiAuthorization;
        }

        public TikiConfigApp(TikiConfigApp dataTikiConfigApp)
        {
            appID = dataTikiConfigApp.appID;
            homeAddress = dataTikiConfigApp.homeAddress;
            secretAppCode = dataTikiConfigApp.secretAppCode;
            tikiAu = dataTikiConfigApp.tikiAu;
        }
        public void Empty()
        {
            appID = string.Empty;
            homeAddress = string.Empty;
            secretAppCode = string.Empty;
            tikiAu = new TikiAuthorization();
        }

        public void SetAllValue(string inputAppID, string inputHomeAddress, string inputSecretAppCode, string inputUsingApp, TikiAuthorization inputTikiAu)
        {
            appID = inputAppID;
            homeAddress = inputHomeAddress;
            secretAppCode = inputSecretAppCode;
            tikiAu = inputTikiAu;
        }

        /// <summary>
        /// 1.Given an app credentials with id = 7590139168389961, and secret = tfSl0c6VFv3fAB_z9F-m22IhEnmwq6ew
        /// 2.Join them with a semi-colon we have 7590139168389961:tfSl0c6VFv3fAB_z9F-m22IhEnmwq6ew
        /// 3.Encode the result with Base64 we have
        /// </summary>
        /// <param name="appID">inhouse app ID</param>
        /// <returns></returns>
        public string Tiki_GetAppCredentialBase64Format()
        {
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(appID + ":" + secretAppCode);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
