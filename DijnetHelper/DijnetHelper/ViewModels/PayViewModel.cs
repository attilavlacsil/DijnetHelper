using System.Collections.Generic;
using DijnetHelper.Model;
using Bill = DijnetHelper.Logic.Model.Bill;

namespace DijnetHelper.ViewModels
{
    public class PayViewModel : ViewModel
    {
        private Bill bill;
        private List<Card> cards;
        private Card selectedCard;
        private bool isPaying;
        private string status;
        private bool isPaid;
        private string statusDetail;

        public PayViewModel()
        {
            cards = new List<Card>(0);
        }

        public Bill Bill
        {
            get => bill;
            set => SetProperty(ref bill, value);
        }

        public List<Card> Cards
        {
            get => cards;
            set => SetProperty(ref cards, value, onChanged: () => OnPropertyChanged(nameof(CanPay)));
        }

        public Card SelectedCard
        {
            get => selectedCard;
            set => SetProperty(ref selectedCard, value);
        }

        public bool IsPaying
        {
            get => isPaying;
            set => SetProperty(ref isPaying, value, onChanged: () => OnPropertyChanged(nameof(CanPay)));
        }

        public bool IsPaid
        {
            get => isPaid;
            set => SetProperty(ref isPaid, value, onChanged: () => OnPropertyChanged(nameof(CanPay)));
        }

        public string Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        public string StatusDetail
        {
            get => statusDetail;
            set => SetProperty(ref statusDetail, value);
        }

        public bool CanPay => Cards.Count > 0 && !IsPaying && !IsPaid;
    }
}
