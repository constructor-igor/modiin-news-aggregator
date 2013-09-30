using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;

namespace AggregatorTests.Feasibility
{
    [TestFixture]
    // http://jenyay.net/Programming/LJServer
    public class LiveJournalAPITests
    {
        [Test]
        public void FlatLJServer_ClearAuthorization_NotNull()
        {
            var flatLjServer = new FlatLJServer();
            flatLjServer.LoginClear("", "");
            Assert.Pass();
        }
        [Test]
        public void FlatLJServer_ClearMD5Authorization_NotNull()
        {
            var flatLjServer = new FlatLJServer();
            flatLjServer.LoginClearMD5("", "");
            Assert.Pass();
        }

        [Test]
        public void FlatLJServer_ChallengeAuthorization_NotNull()
        {
            var flatLjServer = new FlatLJServer();
            flatLjServer.LoginChallenge("", "");
            Assert.Pass();
        }

        [Test]
        public void FlatLJServer_GetTodayPosts_NotNull()
        {
            var flatLjServer = new FlatLJServer();
            flatLjServer.LoginChallenge("", "");
            flatLjServer.GetPosts();
            Assert.Pass();
        }

    }

    public interface ILJServer
    {
        string ServerUri { get; }
    }
    abstract public class LJServer: ILJServer
    {
        protected IWebProxy proxy { get; set; }
        public string ServerUri { get; private set; }
        protected string ContentType { get; set; }
        private readonly CookieCollection cookies;
        protected LJServer(string serverUri, string contentType)
        {
            ServerUri = serverUri;
            ContentType = contentType;
            proxy = null;
            cookies = new CookieCollection();
        }
        protected string SendRequest(string textRequest)
        {
            // Convert request to в byte[]
            byte[] byteArray = Encoding.UTF8.GetBytes(textRequest);

            var request = (HttpWebRequest)WebRequest.Create(ServerUri);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Method = "POST";
            request.ContentLength = textRequest.Length;
            request.ContentType = ContentType;

            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(cookies);

            request.Proxy = proxy;

            if (cookies["ljsession"] != null)
            {
                request.Headers.Add("X-LJ-Auth", "cookie");
            }

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(byteArray, 0, textRequest.Length);

            var response = (HttpWebResponse)request.GetResponse();

            // Read answer
            using (Stream responseStream = response.GetResponseStream())
            {
                var readStream = new StreamReader(responseStream, Encoding.UTF8);

                string currResponse = readStream.ReadToEnd();

//            // 
//            _log.WriteLine("\r\n*** Response:");
//            _log.WriteLine(currResponse);
//
//            // 
//            _log.WriteLine("\r\n*** Cookies:");
//            for (int i = 0; i < response.Cookies.Count; i++)
//            {
//                _log.WriteLine(response.Cookies[i].ToString());
//            }

                readStream.Close();

                return currResponse;
            }            
        }

        public void LoginClear(string user, string password)
        {
            string request = string.Format("mode=login&auth_method=clear&user={0}&password={1}", user, password);

            SendRequest(request);
        }

        public void LoginClearMD5(string user, string password)
        {
            string request = string.Format("mode=login&auth_method=clear&user={0}&hpassword={1}", user, ComputeMD5(password));
            SendRequest(request);
        }

        public void LoginChallenge(string user, string password)
        {
            string challenge = SendRequest("mode=getchallenge");
            string auth_response = GetAuthResponse(password, challenge);
            string request = string.Format ("mode=login&auth_method=challenge&auth_challenge={0}&auth_response={1}&user={2}", challenge, auth_response, user);
            SendRequest (request);
        }

        protected string ComputeMD5(string text)
        {
            var md5 = new MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(text));

            var sb = new StringBuilder();

            foreach (byte hashByte in hashBytes)
                sb.Append(Convert.ToString(hashByte, 16).PadLeft(2, '0'));

            return sb.ToString();
        }
        protected string GetAuthResponse(string password, string challenge)
        {
            // md5 от пароля
            string hpass = ComputeMD5(password);

            string constr = challenge + hpass;
            string auth_response = ComputeMD5(constr);

            return auth_response;
        }
    }

    public class FlatLJServer : LJServer
    {
        public FlatLJServer()
            : base(@"http://www.livejournal.com/interface/flat", @"application/x-www-form-urlencoded")
        {
        }

        public void GetPosts()
        {
            const string user = "";
            const string challenge = "";
            const string auth_response = "";
            string request = string.Format("mode={0}&user={1}&auth_method={2}&auth_challenge={3}&auth_response={4}&usejournal={5}",
                "getdaycount", user, "challenge", challenge, auth_response, "modiin_ru");

            string posts = SendRequest(request);
        }
    }

    public class XmlLJServer : LJServer
    {
        public XmlLJServer()
            : base(@"http://www.livejournal.com/interface/xmlrpc", @"text/xml")
        {
        }
    }
}