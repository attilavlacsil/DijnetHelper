using DijnetHelper.Fakes;
using DijnetHelper.Logic;
using DijnetHelper.Logic.Factory;
using DijnetHelper.Logic.Model;
using DijnetHelper.Pages;
using Unity;
using Unity.Injection;

namespace DijnetHelper
{
    public static class UnityForms
    {
        public static void RegisterTypes(IUnityContainer container)
        {
            // start page
            container.RegisterType<App>(new InjectionConstructor(new ResolvedParameter<LoginPage>()));

            // only one instance from webview parent
            container.RegisterType<LoginPage>(TypeLifetime.PerContainer);

            container.RegisterType<ICredentialManager<DijnetCredential>, DijnetCredentialManager>();

            container.RegisterType<DijnetBrowserFactory>(TypeLifetime.PerContainer, new InjectionConstructor(
                new InjectionParameter<IUnityContainer>(container)));
            container.RegisterType<InvoicePageFactory>(new InjectionConstructor(
                new InjectionParameter<IUnityContainer>(container)));
            container.RegisterType<PayPageFactory>(new InjectionConstructor(
                new InjectionParameter<IUnityContainer>(container)));

            // this is used to resolve ctor parameters
            container.RegisterFactory<IDijnetBrowser>(c =>
            {
                var factory = c.Resolve<DijnetBrowserFactory>();
                return factory.Create();
            });

            RegisterUpstreamDependencies(container);
        }

        // FAKE_AUTH and FAKE_BROWSER to fake dependencies
        private static void RegisterUpstreamDependencies(IUnityContainer container)
        {
#if FAKE_AUTH
            container.RegisterType<IAuthClient<DijnetLoginResult>, FakeDijnetAuthClient>();
#else
            container.RegisterType<IAuthClient<DijnetLoginResult>, DijnetAuthClient>();
#endif

            // this is used by factory only
#if FAKE_BROWSER
            container.RegisterType<IDijnetBrowser, FakeDijnetBrowser>(nameof(DijnetBrowserFactory));
#else
            container.RegisterType<IDijnetBrowser, DijnetBrowser>(nameof(DijnetBrowserFactory));
#endif
        }
    }
}
