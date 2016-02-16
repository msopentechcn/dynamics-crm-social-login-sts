using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace mysts
{
    public class Utility
    {

        static internal string GetOpenId(string accessToken)
        {
            var query = new Dictionary<string, string>();
            query.Add("access_token", accessToken);

            JObject resp = MakeJsonHttpRequest(Constants.GITHUB_INFO_URL, query, "GET");

            if (((string)resp["id"]).Length > 0)
            {
                return (string)resp["id"];
            }

            return "";
        }

        static internal JObject MakeJsonHttpRequest(string url, Dictionary<string, string> query, string method = "POST")
        {
            url = BuildUrl(url, query);
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.Accept = "application/json";
            request.UserAgent = "dynamics-crm-test";

            Stream bodyStream = SafeRequest(request).GetResponseStream();
            var reader = new StreamReader(bodyStream);
            string content = reader.ReadToEnd();

            try
            {
                return JObject.Parse(content);
            }
            catch (Exception)
            {
                return JObject.Parse("{}");
            }
        }

        static internal HttpWebResponse SafeRequest(HttpWebRequest request)
        {
            try
            {
                return (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                var resp = e.Response as HttpWebResponse;

                if (resp == null)
                    throw;

                return resp;
            }
        }

        static internal string BuildUrl(string baseUrl, Dictionary<string, string> query)
        {
            string ret = baseUrl;
            ret += "?";
            foreach (var q in query)
            {
                ret += HttpUtility.UrlEncode(q.Key);
                ret += "=";
                ret += HttpUtility.UrlEncode(q.Value);
                ret += "&";
            }

            // remove trailing `&` or `?`
            return ret.Substring(0, ret.Length - 1);
        }

        public static X509Certificate2 GetCertificate(string subjectName)
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            X509Certificate2Collection certificates = null;
            store.Open(OpenFlags.ReadOnly);

            try
            {
                certificates = store.Certificates;
                var certs = certificates.OfType<X509Certificate2>().Where(x => x.SubjectName.Name.Equals(subjectName, StringComparison.OrdinalIgnoreCase)).ToList();
                // var certs = certificates.OfType<X509Certificate2>().ToList();

                if (certs.Count == 0)
                    throw new ApplicationException(string.Format("No certificate was found for subject Name {0}", subjectName));
                else if (certs.Count > 1)
                    throw new ApplicationException(string.Format("There are multiple certificates for subject Name {0}", subjectName));

                return new X509Certificate2(certs[0]);
            }
            finally
            {
                if (certificates != null)
                {
                    for (var i = 0; i < certificates.Count; i++)
                    {
                        var cert = certificates[i];
                        cert.Reset();
                    }
                }
                store.Close();
            }
        }
    }
}