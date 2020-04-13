using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DijnetHelper.Logic;
using DijnetHelper.Logic.Factory;
using DijnetHelper.Model;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Bill = DijnetHelper.Logic.Model.Bill;

namespace DijnetHelper.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BillPage
    {
        private readonly IDijnetBrowser browser;
        private readonly PayPageFactory payPageFactory;

        public BillPage(IDijnetBrowser browser, PayPageFactory payPageFactory)
        {
            this.browser = browser ?? throw new ArgumentNullException(nameof(browser));
            this.payPageFactory = payPageFactory ?? throw new ArgumentNullException(nameof(payPageFactory));

            InitializeComponent();

            ViewModel.BillRefreshCommand = new Command(async () => { await RefreshBillsAsync(); });
            ViewModel.LogoutCommand = new Command(async () => { await LogoutAsync(); }, () => ViewModel.IsEnabled);
            ViewModel.IsEnabled = true;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (ViewModel.SelectedBill == null || ViewModel.SelectedBill.Status == BillStatus.Paid)
            {
                await UpdateBillsAsync();
            }

            ViewModel.SelectedBill = null;
        }

        // logout on back button
        protected override bool OnBackButtonPressed()
        {
            MainThread.BeginInvokeOnMainThread(async () => { await LogoutAsync(); });
            return true;
        }

        private async Task UpdateBillsAsync()
        {
            ViewModel.Bills = new List<Bill>(0);
            ViewModel.Status = "Loading...";

            List<Bill> bills = await browser.GetBillsAsync();

            if (bills != null)
            {
                ViewModel.Bills = bills;
                ViewModel.Status = bills.Count == 0 ? "No bills, pull down to refresh." : null;
            }
            else
            {
                ViewModel.Status = "Failed to load bills, pull down to refresh.";
            }
        }

        private async Task RefreshBillsAsync()
        {
            ViewModel.IsRefreshing = true;

            await UpdateBillsAsync();
            
            ViewModel.IsRefreshing = false;
        }

        private async Task LogoutAsync()
        {
            var confirmed = await DisplayAlert("Logout", "Want to logout?", "Yes", "No");
            if (!confirmed)
            {
                return;
            }

            ViewModel.IsEnabled = false;

            bool result = await browser.LogoutAsync();
            if (!result)
            {
                // TODO log
            }

            await Navigation.PopAsync(true);
        }

        private async void ButtonPay_OnClicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Bill bill = button.CommandParameter as Bill;
            ViewModel.SelectedBill = bill;

            var page = payPageFactory.Create(bill);
            await Navigation.PushAsync(page, true);
        }
    }
}