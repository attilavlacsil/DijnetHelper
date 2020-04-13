using System.Net;
using System.Threading.Tasks;
using DijnetHelper.iOS.Renderers;
using DijnetHelper.Views;
using Foundation;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CookieWebView), typeof(CookieWebViewRenderer))]
namespace DijnetHelper.iOS.Renderers
{
    public class CookieWebViewRenderer : WkWebViewRenderer
    {
        private WKHttpCookieStore cookieStore;

        protected override async void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                // https://stackoverflow.com/a/26577303 => MinimumOSVersion=11.0
                cookieStore = Configuration.WebsiteDataStore.HttpCookieStore;

                // TODO might be good to clear cache
                // if element is new, load cookies into platform
                if (e.NewElement is CookieWebView newElement && newElement.CookieContainer != null)
                {
                    await RemoveAllCookies();
                    await LoadAllCookiesAsync(newElement);
                }
            }
        }

        // no OnElementPropertyChanged

        private async Task RemoveAllCookies()
        {
            var cookies = await cookieStore.GetAllCookiesAsync();
            foreach (var cookie in cookies)
            {
                await cookieStore.DeleteCookieAsync(cookie);
            }
        }

        private async Task LoadAllCookiesAsync(CookieWebView element)
        {
            foreach (Cookie cookie in element.Cookies)
            {
                await cookieStore.SetCookieAsync(new NSHttpCookie(cookie));
            }
        }
    }
}