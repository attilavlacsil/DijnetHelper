using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DijnetHelper.Logic.Model;
using Newtonsoft.Json.Linq;

namespace DijnetHelper.Logic
{
    public class DijnetAuthClient : IAuthClient<DijnetLoginResult>
    {
        private const string LoginAddress = "https://www.dijnet.hu/ekonto/login/login_check_ajax";

        // form login to obtain cookies
        public async Task<DijnetLoginResult> LoginAsync(string userName, string password)
        {
            var cookieContainer = new CookieContainer();

            var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true,
                AllowAutoRedirect = false
            };

            using (var client = new HttpClient(handler))
            {
                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "username", userName },
                    { "password", password }
                });

                HttpResponseMessage response = await client.PostAsync(LoginAddress, content);
                return await ParseResponseAsync(response, cookieContainer);
            }
        }

        private async Task<DijnetLoginResult> ParseResponseAsync(HttpResponseMessage response, CookieContainer cookieContainer)
        {
            DijnetLoginResult result = new DijnetLoginResult();

            if (!response.IsSuccessStatusCode)
            {
                // TODO log comm error

                result.Error = $"Communication error ({response.StatusCode})";
                return result;
            }

            string responseContent = (await response.Content.ReadAsStringAsync()).Trim();
            // TODO log responseContent

            JObject json = JObject.Parse(responseContent);
            if (!json.TryGetValue("success", StringComparison.InvariantCultureIgnoreCase, out var successToken))
            {
                // TODO log format error

                result.Error = "Format error (success)";
                return result;
            }

            if (!successToken.Value<bool>())
            {
                // TODO log success false

                if (json.TryGetValue("error", StringComparison.InvariantCultureIgnoreCase, out var errorToken))
                {
                    result.Error = errorToken.Value<string>();
                }

                result.Error = "Server returned failed login";
                return result;
            }

            if (!json.TryGetValue("url", StringComparison.InvariantCultureIgnoreCase, out var urlToken))
            {
                // TODO log format error

                result.Error = "Format error (url)";
                return result;
            }

            string url = urlToken.Value<string>();
            if (string.IsNullOrEmpty(url))
            {
                // TODO log format error

                result.Error = "Format error (url)";
                return result;
            }

            result.Success = true;
            result.CookieContainer = cookieContainer;
            result.Url = url;
            return result;
        }
    }
}
