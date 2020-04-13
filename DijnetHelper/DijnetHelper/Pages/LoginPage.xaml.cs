using System;
using System.Net;
using System.Threading.Tasks;
using DijnetHelper.Logic;
using DijnetHelper.Logic.Factory;
using DijnetHelper.Logic.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DijnetHelper.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage
    {
        private readonly ICredentialManager<DijnetCredential> credentialManager;
        private readonly IAuthClient<DijnetLoginResult> authClient;
        private readonly DijnetBrowserFactory dijnetBrowserFactory;
        private readonly InvoicePageFactory invoicePageFactory;

        private bool firstAutoLogin = true;

        public LoginPage(
            ICredentialManager<DijnetCredential> credentialManager,
            IAuthClient<DijnetLoginResult> authClient,
            DijnetBrowserFactory dijnetBrowserFactory,
            InvoicePageFactory invoicePageFactory)
        {
            this.credentialManager = credentialManager ?? throw new ArgumentNullException(nameof(credentialManager));
            this.authClient = authClient ?? throw new ArgumentNullException(nameof(authClient));
            this.dijnetBrowserFactory = dijnetBrowserFactory ?? throw new ArgumentNullException(nameof(dijnetBrowserFactory));
            this.invoicePageFactory = invoicePageFactory ?? throw new ArgumentNullException(nameof(invoicePageFactory));

            InitializeComponent();
            ViewModel.Status = "Loading...";
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            DijnetCredential dijnetCredential = await credentialManager.GetStoredCredentialAsync();

            if (firstAutoLogin)
            {
                firstAutoLogin = false;

                bool success = await TryAutoLoginAsync(dijnetCredential);
                if (!success)
                {
                    EnableManualLogin(dijnetCredential);
                }
            }
            else
            {
                EnableManualLogin(dijnetCredential);
            }

            ViewModel.IsLoggingIn = false;
        }

        private async Task<bool> TryAutoLoginAsync(DijnetCredential dijnetCredential)
        {
            if (dijnetCredential.IsValid && dijnetCredential.AutoLogin)
            {
                ViewModel.Status = "Logging in...";

                DijnetLoginResult loginResult = await authClient.LoginAsync(dijnetCredential.UserName, dijnetCredential.Password);
                if (loginResult.Success)
                {
                    await OnLoginSuccess(loginResult.CookieContainer, loginResult.Url);
                    return true;
                }
            }

            return false;
        }

        private void EnableManualLogin(DijnetCredential dijnetCredential)
        {
            if (dijnetCredential.IsValid)
            {
                ViewModel.UserName = dijnetCredential.UserName;
                ViewModel.Password = dijnetCredential.Password;
                ViewModel.Remember = dijnetCredential.Remember;
                ViewModel.AutoLogin = dijnetCredential.AutoLogin;
            }
            else
            {
                ViewModel.UserName = null;
                ViewModel.Password = null;
                ViewModel.Remember = false;
                ViewModel.AutoLogin = false;
            }

            ViewModel.Status = null;
            ViewModel.IsManualLogin = true;
        }

        private async Task OnLoginSuccess(CookieContainer cookieContainer, string url)
        {
            WebView.CookieContainer = cookieContainer;
            dijnetBrowserFactory.Register(WebView, url);

            var page = invoicePageFactory.Create();
            NavigationPage.SetHasBackButton(page, false);
            await Navigation.PushAsync(page, true);
        }

        private async void ButtonLogin_OnClicked(object sender, EventArgs e)
        {
            ViewModel.IsLoggingIn = true;
            ViewModel.Status = "Logging in...";
            ViewModel.StatusDetail = null;

            string userName = ViewModel.UserName;
            string password = ViewModel.Password;

            DijnetLoginResult loginResult = await authClient.LoginAsync(userName, password);

            if (loginResult.Success)
            {
                bool remember = ViewModel.Remember;
                bool autoLogin = ViewModel.AutoLogin;
                DijnetCredential credential = new DijnetCredential(userName, password, remember, autoLogin);

                await credentialManager.UpdateCredentialAsync(credential);
                await OnLoginSuccess(loginResult.CookieContainer, loginResult.Url);
            }
            else
            {
                ViewModel.Status = "Failed to log in!";
                ViewModel.StatusDetail = loginResult.Error;
                ViewModel.IsLoggingIn = false;
            }
        }
    }
}