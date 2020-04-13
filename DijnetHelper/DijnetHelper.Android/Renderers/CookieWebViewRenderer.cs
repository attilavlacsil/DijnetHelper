using System.ComponentModel;
using System.Net;
using Android.Content;
using Android.Webkit;
using DijnetHelper.Droid.Renderers;
using DijnetHelper.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using WebView = Xamarin.Forms.WebView;

[assembly: ExportRenderer(typeof(CookieWebView), typeof(CookieWebViewRenderer))]
namespace DijnetHelper.Droid.Renderers
{
    public class CookieWebViewRenderer : WebViewRenderer
    {
        private readonly CookieManager cookieManager;

        public CookieWebViewRenderer(Context context)
            :base(context)
        {
            cookieManager = CookieManager.Instance;
            if (!cookieManager.AcceptCookie())
            {
                cookieManager.SetAcceptCookie(true);
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                Control?.ClearCache(true);

                // if element is new, load cookies into platform
                if (e.NewElement is CookieWebView element)
                {
                    cookieManager.RemoveAllCookie();
                    LoadAllCookies(element);
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            // if CookieContainer has changed, load cookies into platform
            if (e.PropertyName == nameof(CookieWebView.CookieContainer) && Element is CookieWebView element)
            {
                cookieManager.RemoveAllCookie();
                LoadAllCookies(element);
            }
        }

        private void LoadAllCookies(CookieWebView element)
        {
            foreach (Cookie cookie in element.Cookies)
            {
                cookieManager.SetCookie(cookie.Domain, $"{cookie.Name}={cookie.Value}");
            }
        }
    }
}