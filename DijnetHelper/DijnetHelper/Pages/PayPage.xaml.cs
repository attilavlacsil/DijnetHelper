using System;
using System.Collections.Generic;
using System.Linq;
using DijnetHelper.Logic;
using DijnetHelper.Logic.Model;
using DijnetHelper.Model;
using Xamarin.Forms.Xaml;
using Bill = DijnetHelper.Logic.Model.Bill;

namespace DijnetHelper.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PayPage
    {
        private readonly IDijnetBrowser browser;

        public PayPage(IDijnetBrowser browser, Bill bill)
        {
            this.browser = browser ?? throw new ArgumentNullException(nameof(browser));

            InitializeComponent();
            ViewModel.Bill = bill;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            ViewModel.Status = "Loading cards...";

            List<Card> cards = await browser.SelectBillAndGetCardsAsync(ViewModel.Bill);

            if (cards.Count > 0)
            {
                ViewModel.Cards = cards;
                ViewModel.SelectedCard = cards.FirstOrDefault(x => x.Default) ?? cards[0];
                ViewModel.Status = null;
            }
            else
            {
                ViewModel.Status = "No cards found.";
            }
        }

        private async void ButtonPay_OnClicked(object sender, EventArgs e)
        {
            ViewModel.IsPaying = true;
            ViewModel.Status = "Paying...";

            Card selectedCard = ViewModel.SelectedCard;

            PayResult result = await browser.PayBillAsync(selectedCard);

            if (result.Success)
            {
                ViewModel.Bill.Status = BillStatus.Paid;
            }

            ViewModel.IsPaid = result.Success;
            ViewModel.Status = result.Success ? "Done!" : "Failed!";
            ViewModel.StatusDetail = result.Error;
            ViewModel.IsPaying = false;
        }

        private void PickerCards_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ViewModel.Status = null;
        }
    }
}