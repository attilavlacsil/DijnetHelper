using System;
using System.Threading;
using DijnetHelper.Views;
using Unity;
using Unity.Resolution;
using Xamarin.Forms;

namespace DijnetHelper.Logic.Factory
{
    // singleton instance provider (per registration)
    public class DijnetBrowserFactory : Factory
    {
        private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        private bool registered;
        private IDijnetBrowser instance;

        public DijnetBrowserFactory(IUnityContainer container)
            : base(container)
        {
        }

        // use to register runtime dependencies, and hold an instance (thread-safe)
        public void Register(CookieWebView webView, string mainUrlPath, bool disposeExisting = true)
        {
            if (webView == null) throw new ArgumentNullException(nameof(webView));
            if (mainUrlPath == null) throw new ArgumentNullException(nameof(mainUrlPath));

            rwLock.EnterWriteLock();
            try
            {
                // existing instance must be disposed, if disposable
                if (disposeExisting && instance is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                instance = Container.Resolve<IDijnetBrowser>(nameof(DijnetBrowserFactory),
                    new ParameterOverride(typeof(WebView), webView),
                    new ParameterOverride(typeof(string), mainUrlPath));

                registered = true;
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        public IDijnetBrowser Create()
        {
            rwLock.EnterReadLock();
            try
            {
                if (!registered)
                {
                    throw new InvalidOperationException("Instance must be registered first.");
                }

                return instance;
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }
    }
}