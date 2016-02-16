using System;
using System.IdentityModel;
using System.IdentityModel.Configuration;
using System.IdentityModel.Protocols.WSTrust;
using System.Security.Claims;

namespace mysts
{
    class CustomSecurityTokenService : SecurityTokenService
    {
        public CustomSecurityTokenService(SecurityTokenServiceConfiguration configuration)
            : base(configuration)
        {
        }

        protected override ClaimsIdentity GetOutputClaimsIdentity(ClaimsPrincipal principal, RequestSecurityToken request, Scope scope)
        {
            if (principal == null)
            {
                throw new InvalidRequestException("The caller's principal is null.");
            }

            // check github
            string ak = principal.FindFirst(Constants.CLAIM_TYPE_GITHUB_AK).Value;
            string openid = Utility.GetOpenId(ak);

            // check account
            ADAccountInfo info = AccountHelper.GetHelper().GetAccount(openid);

            if (info == null)
            {
                throw new InvalidRequestException("wrong github login or not binded, cannot login.");
            }


            var claims = new[]
            {
                new Claim(Constants.CLAIM_TYPE_PRIMARY_SID, info.primarysid),
                new Claim(System.IdentityModel.Claims.ClaimTypes.Upn, info.upnUpper),
                new Claim(System.IdentityModel.Claims.ClaimTypes.Upn, info.upnLower),
                new Claim(System.IdentityModel.Claims.ClaimTypes.Name, info.name),
            };

            var id = new ClaimsIdentity(claims);

            return id;
        }

        protected override Scope GetScope(ClaimsPrincipal principal, RequestSecurityToken request)
        {
            ValidateAppliesTo(request.AppliesTo);

            var scope = new Scope(request.AppliesTo.Uri.AbsoluteUri, SecurityTokenServiceConfiguration.SigningCredentials);

            if (Uri.IsWellFormedUriString(request.ReplyTo, UriKind.Absolute))
            {
                if (request.AppliesTo.Uri.Host != new Uri(request.ReplyTo).Host)
                    scope.ReplyToAddress = request.AppliesTo.Uri.AbsoluteUri;
                else
                    scope.ReplyToAddress = request.ReplyTo;
            }
            else
            {
                Uri resultUri = null;
                if (Uri.TryCreate(request.AppliesTo.Uri, request.ReplyTo, out resultUri))
                    scope.ReplyToAddress = resultUri.AbsoluteUri;
                else
                    scope.ReplyToAddress = request.AppliesTo.Uri.ToString();
            }

            scope.TokenEncryptionRequired = false;
            scope.SymmetricKeyEncryptionRequired = false;

            return scope;
        }

        private void ValidateAppliesTo(EndpointReference appliesTo)
        {
            if (appliesTo == null)
            {
                throw new InvalidRequestException("The AppliesTo is null.");
            }

            // should check applies to, but for convienence, not
        }
    }
}