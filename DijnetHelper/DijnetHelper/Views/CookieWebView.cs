using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Xamarin.Forms;

namespace DijnetHelper.Views
{
    // WebView wrapper to handle cookies from a CookieContainer (with reflection)
    // platform renderer must take care of injecting cookies
    public class CookieWebView : WebView
    {
        public static readonly BindableProperty CookieContainerProperty = BindableProperty.Create(nameof(CookieContainer), typeof(CookieContainer), typeof(CookieWebView));

        public CookieContainer CookieContainer
        {
            get => (CookieContainer)GetValue(CookieContainerProperty);
            set => SetValue(CookieContainerProperty, value);
        }

        // extact every cookie from CookieContainer
        public IEnumerable<Cookie> Cookies
        {
            get
            {
                if (CookieContainer == null || CookieContainer.Count == 0)
                {
                    yield break;
                }

                var domainTableField = CookieContainer.GetType().GetRuntimeFields().First(x => x.Name == "m_domainTable");
                var domains = (IDictionary)domainTableField.GetValue(CookieContainer);

                foreach (var val in domains.Values)
                {
                    var type = val.GetType().GetRuntimeFields().First(x => x.Name == "m_list");
                    var values = (IDictionary)type.GetValue(val);
                    foreach (CookieCollection cookies in values.Values)
                    {
                        foreach (var cookie in cookies.Cast<Cookie>())
                        {
                            yield return cookie;
                        }
                    }
                }
            }
        }
    }
}
