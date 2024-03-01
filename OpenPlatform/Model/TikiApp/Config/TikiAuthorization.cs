namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Config
{
    /// <summary>
    /// Chứa thông tin ủy quyền connect tới shop Tiki
    /// </summary>
    public class TikiAuthorization
    {
        public TikiAuthorization()
        {
            access_token = string.Empty;
            expires_in = string.Empty;
            scope = string.Empty;
            token_type = string.Empty;
        }

        public TikiAuthorization(string inputAcces_Token, string inputExpires_In, string inputScope, string inputToken_Type)
        {
            access_token = inputAcces_Token;
            expires_in = inputExpires_In;
            scope = inputScope;
            token_type = inputToken_Type;
        }
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
    }
}
