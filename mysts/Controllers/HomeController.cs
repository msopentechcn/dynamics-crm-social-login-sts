using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Security.Claims;
using System.IdentityModel.Configuration;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;

namespace mysts.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var query = new Dictionary<string, string>();
            query.Add("client_id", Constants.GITHUB_CLIENT_ID);
            query.Add("redirect_uri", Constants.GITHUB_OAUTH_HOST + "/Home/GetAccessToken" + Request.Url.Query);
            query.Add("scope", "");
            query.Add("state", Constants.GITHUB_OAUTH_STATE);

            string url = Utility.BuildUrl(Constants.GITHUB_AUTH_URL, query);
            return Redirect(url);
        }

        public ActionResult GetAccessToken(string code)
        {
            var query = new Dictionary<string, string>();
            query.Add("client_id", Constants.GITHUB_CLIENT_ID);
            query.Add("client_secret", Constants.GITHUB_CLIENT_SEC);
            query.Add("code", code);
            query.Add("state", Constants.GITHUB_OAUTH_STATE);

            // send request
            JObject resp = Utility.MakeJsonHttpRequest(Constants.GITHUB_AK_URL, query);
            string accessToken = (string)resp["access_token"];

            // call sts and return
            // build cliam
            var claim = new ClaimsPrincipal();
            var id = new ClaimsIdentity();
            id.AddClaim(new Claim(Constants.CLAIM_TYPE_GITHUB_AK, accessToken));
            claim.AddIdentity(id);

            // send claim
            var sigingCredentials = new X509SigningCredentials(Utility.GetCertificate(Constants.CERTIFICATE_NAME));

            var config = new SecurityTokenServiceConfiguration(Constants.ISSUER_NAME, sigingCredentials);
            var sts = new CustomSecurityTokenService(config);

            var requestMessage = (SignInRequestMessage)WSFederationMessage.CreateFromUri(Request.Url);
            var responesMessage = FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(requestMessage, claim, sts);

            var formData = responesMessage.WriteFormPost();
            return new ContentResult() { Content = formData, ContentType = "text/html" };
        }

        public ActionResult GenerateBindingUrl()
        {
            return View();
        }

        public ActionResult Binding()
        {
            var query = new Dictionary<string, string>();
            query.Add("client_id", Constants.GITHUB_CLIENT_ID);
            query.Add("redirect_uri", Constants.GITHUB_OAUTH_HOST + "/Home/DoBinding" + Request.Url.Query);
            query.Add("scope", "");
            query.Add("state", Constants.GITHUB_OAUTH_STATE);

            string url = Utility.BuildUrl(Constants.GITHUB_AUTH_URL, query);
            return Redirect(url);
        }

        public ActionResult DoBinding(string code, string upn_lower, string upn_upper, string primary_sid, string name)
        {
            var query = new Dictionary<string, string>();
            query.Add("client_id", Constants.GITHUB_CLIENT_ID);
            query.Add("client_secret", Constants.GITHUB_CLIENT_SEC);
            query.Add("code", code);
            query.Add("state", Constants.GITHUB_OAUTH_STATE);

            // send request
            JObject resp = Utility.MakeJsonHttpRequest(Constants.GITHUB_AK_URL, query);
            string accessToken = (string)resp["access_token"];

            // get openid
            string openid = Utility.GetOpenId(accessToken);

            AccountHelper.GetHelper().AddAccount(openid, new ADAccountInfo(upn_lower, upn_upper, primary_sid, name));

            return View();
        }
    }
}