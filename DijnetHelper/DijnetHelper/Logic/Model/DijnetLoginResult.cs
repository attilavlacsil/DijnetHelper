using System.Net;

namespace DijnetHelper.Logic.Model
{
    public class DijnetLoginResult
    {
        public bool Success { get; set; }
        public CookieContainer CookieContainer { get; set; }
        public string Url { get; set; }
        public string Error { get; set; }
    }
}