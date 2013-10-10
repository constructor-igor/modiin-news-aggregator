using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using InstaSharp;
using InstaSharp.Model;
using InstaSharp.Model.Responses;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AggregatorTests.Feasibility
{
    // http://instasharp.org/
    // http://www.gotdotnet.ru/blogs/squall/13573/
    [TestFixture]
    [Explicit]
    public class InstagramAPITests
    {
        const string clientId = "YOUR NEW ID";
        const string clientSecret = "YOUR NEW SECRET";
        const string redirectUri = "https://twitter.com/modiin_ru"; //"http://kakveselo.ru"; // Any URL
        const string oAuthUri = "https://api.instagram.com/oauth";
        const string apiUri = "https://api.instagram.com/v1";
        const string login = "login";
        const string password = "password";

        [Test]
        public void SearchPicturesByTag()
        {
            var config = new InstaSharp.InstagramConfig(apiUri, oAuthUri, clientId, clientSecret, redirectUri);

            var tags = new InstaSharp.Endpoints.Tags.Unauthenticated(config);
            var pictures = tags.Recent("modiin");

            dynamic dyn = JsonConvert.DeserializeObject(pictures.Json);
            foreach (var data in dyn.data)
            {
                string message = String.Format("{0} - {1}",
                    data.filter,
                    data.images.standard_resolution.url);
                Trace.WriteLine(message);
            }
        }
        [Test]
        public void Test()
        {
            var config = new InstaSharp.InstagramConfig(apiUri, oAuthUri, clientId, clientSecret, redirectUri);

            //            var media = new InstaSharp.Endpoints.Media.Unauthenticated(config);
            //            media.Search()

            //            var tags = new InstaSharp.Endpoints.Tags.Unauthenticated(config);
            //            var pictures = tags.Recent("modiin");
            //
            //            dynamic dyn = JsonConvert.DeserializeObject(pictures.Json);
            //            foreach (var data in dyn.data)
            //            {
            //                Console.WriteLine("{0} - {1}",
            //                    data.filter,
            //                    data.images.standard_resolution.url);
            //            }

            AuthInfo oauthResponse = GetInstagramAuth(oAuthUri, clientId, redirectUri, config, login, password);
            var allMedia = GeAllMyMedia(config, oauthResponse);
        }

        private static AuthInfo GetInstagramAuth(string oAuthUri, string clientId, string redirectUri, InstagramConfig config, string login, string password)
        {
            throw new NotImplementedException();
//            var scopes = new List<Auth.Scope> { Auth.Scope.basic };
//
//            var link = InstaSharp.Auth.AuthLink(oAuthUri, clientId, redirectUri, scopes);
//
//            // Логинимся по указанному узлу
//            CookieAwareWebClient client = new CookieAwareWebClient();
//            // Зашли на страницу логина
//            var result = client.DownloadData(link);
//            var html = System.Text.Encoding.Default.GetString(result);
//
//            // Берем токен
//            string csr = "";
//            string pattern = @"csrfmiddlewaretoken""\svalue=""(.+)""";
//            var r = new System.Text.RegularExpressions.Regex(pattern);
//            var m = r.Match(html);
//            csr = m.Groups[1].Value;
//
//            // Логинимся
//            string loginLink = string.Format(
//                "https://instagram.com/accounts/login/?next=/oauth/authorize/%3Fclient_id%3D{0}%26redirect_uri%3D{1}%26response_type%3Dcode%26scope%3Dbasic", clientId, redirectUri);
//
//            var parameters = new NameValueCollection
//            {
//                {"csrfmiddlewaretoken", csr},
////                {"username", login},
////                {"password", password}
//            };
//
//            // Нужно добавить секретные кукисы, полученные перед логином
//
//            // Нужны заголовки что ли
//            string agent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
//            client.Headers["Referer"] = loginLink;
//            client.Headers["Host"] = "instagram.com";
//            //client.Headers["Connection"] = "Keep-Alive";
//            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";
//            //client.Headers["Content-Length"] = "88";
//            client.Headers["User-Agent"] = agent;
//            client.Headers["Accept-Language"] = "ru-RU";
//            //client.Headers["Accept-Encoding"] = "gzip, deflate";
//            client.Headers["Accept"] = "text/html, application/xhtml+xml, */*";
//            client.Headers["Cache-Control"] = "no-cache";
//
//            // Запрос
//            var result2 = client.UploadValues(loginLink, "POST", parameters);
//
//            // Постим данные, Получаем code
//            // New link не на апи, а на instagram
//            string newPostLink = string.Format("https://instagram.com/oauth/authorize/?client_id={0}&redirect_uri={1}&response_type=code&scope=basic", clientId, redirectUri);
//
//            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(newPostLink);
//            request.AllowAutoRedirect = false;
//            request.CookieContainer = client.CookieContainer;
//            request.Referer = newPostLink;
//            request.Method = "POST";
//            request.ContentType = "application/x-www-form-urlencoded";
//            request.UserAgent = agent;
//
//            string postData = String.Format("csrfmiddlewaretoken={0}&allow=Authorize", csr);
//            request.ContentLength = postData.Length;
//
//            ASCIIEncoding encoding = new ASCIIEncoding();
//            byte[] loginDataBytes = encoding.GetBytes(postData);
//            request.ContentLength = loginDataBytes.Length;
//            Stream stream = request.GetRequestStream();
//            stream.Write(loginDataBytes, 0, loginDataBytes.Length);
//
//            // send the request
//            var response = request.GetResponse();
//            string location = response.Headers["Location"];
//
//            // Теперь вытаскиваем код и получаем аутентификацию
//            pattern = @"kakveselo.ru\?code=(.+)";
//            r = new System.Text.RegularExpressions.Regex(pattern);
//            m = r.Match(location);
//            string code = m.Groups[1].Value;
//
//            // Наконец, получаем токен аутентификации
//            var auth = new InstaSharp.Auth(config); //.OAuth(InstaSharpConfig.config);
//
//            // now we have to call back to instagram and include the code they gave us
//            // along with our client secret
//            var oauthResponse = auth.RequestToken(code);
//
//            return oauthResponse;
        }
        private static List<Media> GeAllMyMedia(InstagramConfig config, AuthInfo oauthResponse)
        {
            // Формируем запросы
            var users = new InstaSharp.Endpoints.Users.Authenticated(config, oauthResponse);
            //var tags = new InstaSharp.Endpoints.Tags.Unauthenticated(config);
            //var comm = new InstaSharp.Endpoints.Comments.Unauthenticated(config);
            //var media = new InstaSharp.Endpoints.Media.Unauthenticated(config);

            // Берем все картинки пользователя
            var allMedia = new List<Media>();

            string nextMaxId = "";
            do
            {
                MediasResponse recent = string.IsNullOrEmpty(nextMaxId)
                                            ? users.Recent()
                                            : users.Recent(nextMaxId);

                nextMaxId = "";

                if (recent != null)
                {
                    allMedia.AddRange(recent.Data);
                    if (recent.Pagination != null)
                    {
                        nextMaxId = recent.Pagination.NextMaxId;
                    }
                }
            } while (!string.IsNullOrEmpty(nextMaxId));
            return allMedia;
        }
    }

    /// <summary>
    /// Web client with cookies enabled.
    /// </summary>

//    class CookieAwareWebClient : WebClient
//    {
//        private readonly CookieContainer container = new CookieContainer();
//        public CookieContainer CookieContainer { get; private set; }
//
//        protected override WebRequest GetWebRequest(Uri address)
//        {
//            var request = base.GetWebRequest(address);
//            if (request is HttpWebRequest)
//            {
//                (request as HttpWebRequest).CookieContainer = container;
//            }
//
//            return request;
//        }
//
//        protected override WebResponse GetWebResponse(WebRequest request)
//        {
//            var response = base.GetWebResponse(request);
//            if (response is HttpWebResponse)
//            {
//                container.Add((response as HttpWebResponse).Cookies);
//            }
//
//            return response;
//        }
//    }

}
