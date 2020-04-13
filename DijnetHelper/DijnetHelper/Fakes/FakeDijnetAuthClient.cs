using System;
using System.Net;
using System.Threading.Tasks;
using DijnetHelper.Logic;
using DijnetHelper.Logic.Model;

namespace DijnetHelper.Fakes
{
    public class FakeDijnetAuthClient : IAuthClient<DijnetLoginResult>
    {
        public async Task<DijnetLoginResult> LoginAsync(string userName, string password)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(200));

            if (userName == "error")
            {
                return new DijnetLoginResult
                {
                    Error = "Login error."
                };
            }

            return new DijnetLoginResult
            {
                Success = true,
                CookieContainer = new CookieContainer(),
                Url = "url"
            };
        }
    }
}
