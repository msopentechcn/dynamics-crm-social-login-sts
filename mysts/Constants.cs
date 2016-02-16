namespace mysts
{
    public class Constants
    {
        public const string GITHUB_AUTH_URL = "https://github.com/login/oauth/authorize";
        public const string GITHUB_AK_URL = "https://github.com/login/oauth/access_token";
        public const string GITHUB_INFO_URL = "https://api.github.com/user";

        // configure these as your github oauth app's settings
        public const string GITHUB_OAUTH_HOST = "https://sts.crm-test.chinacloudapp.cn";
        public const string GITHUB_CLIENT_ID = "11881f43b283ecbab339";
        public const string GITHUB_CLIENT_SEC = "03ec6a60e9563e049d07e2e938224ad28fb484ce";
        public const string GITHUB_OAUTH_STATE = "STATE";

        // same in the federationmetada.xml's entityID
        public const string ISSUER_NAME = "sts.crm-test.chinacloudapp.cn";

        // certificate name
        public const string CERTIFICATE_NAME = "CN=*.crm-test.chinacloudapp.cn";

        // the custom claim type
        public const string CLAIM_TYPE_GITHUB_AK = "github-access-token";
        public const string CLAIM_TYPE_PRIMARY_SID = "http://schemas.microsoft.com/ws/2008/06/identity/claims/primarysid";

        // the only person can login
        public const string PREDEFINED_GITHUB_ID = "3130526";

        // the login info in active directory
        public const string PREDEFINED_PRIMARY_SID = "S-1-5-21-1699405123-2838992688-2767796788-500";
        public const string PREDEFINED_UPN_UPPERCASE = "rapidhere@CRMTEST.local";
        public const string PREDEFINED_UPN_LOWERCASE = "rapidhere@crmtest.local";
        public const string PREDEFINED_NAME = "CRMTEST\\rapidhere";
    }
}