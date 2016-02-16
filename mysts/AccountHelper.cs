using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mysts
{
    public class ADAccountInfo
    {
        public ADAccountInfo(string _upnLower="", string _upnUpper="", string _psid="", string _name="")
        {
            upnLower = _upnLower;
            upnUpper = _upnUpper;
            primarysid = _psid;
            name = _name;
        }

        public string upnLower;
        public string upnUpper;
        public string primarysid;
        public string name;
    }

    public class AccountHelper
    {
        protected Dictionary<string, ADAccountInfo> accounts;
        
        private AccountHelper()
        {
            accounts = new Dictionary<string, ADAccountInfo>();

            // set predefined system user
            AddAccount(Constants.PREDEFINED_GITHUB_ID, new ADAccountInfo(
                Constants.PREDEFINED_UPN_LOWERCASE,
                Constants.PREDEFINED_UPN_UPPERCASE,
                Constants.PREDEFINED_PRIMARY_SID,
                Constants.PREDEFINED_NAME));
        }

        public void AddAccount(string openid, ADAccountInfo info)
        {
            accounts.Add(openid, info);
        }

        public ADAccountInfo GetAccount(string openid)
        {
            return accounts[openid];
        }

        private static AccountHelper _theHelper = new AccountHelper();

        public static AccountHelper GetHelper()
        {
            return _theHelper;
        }
    }
}