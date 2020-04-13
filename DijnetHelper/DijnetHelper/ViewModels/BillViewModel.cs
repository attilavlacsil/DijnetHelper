using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Bill = DijnetHelper.Logic.Model.Bill;

namespace DijnetHelper.ViewModels
{
    public class BillViewModel : ViewModel
    {
        private string status;
        private List<Bill> bills;
        private bool isRefreshing;
        private Bill selectedBill;
        private bool isEnabled;
        private ICommand logoutCommand;
        private ICommand billRefreshCommand;

        public BillViewModel()
        {
            bills = new List<Bill>(0);
        }

        public string Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        public bool IsEnabled
        {
            get => isEnabled;
            set => SetProperty(ref isEnabled, value, onChanged: () => (LogoutCommand as Command)?.ChangeCanExecute());
        }

        public List<Bill> Bills
        {
            get => bills;
            set => SetProperty(ref bills, value);
        }

        public Bill SelectedBill
        {
            get => selectedBill;
            set => SetProperty(ref selectedBill, value);
        }

        public bool IsRefreshing
        {
            get => isRefreshing;
            set => SetProperty(ref isRefreshing, value);
        }

        public ICommand LogoutCommand
        {
            get => logoutCommand;
            set => SetProperty(ref logoutCommand, value);
        }

        public ICommand BillRefreshCommand
        {
            get => billRefreshCommand;
            set => SetProperty(ref billRefreshCommand, value);
        }
    }
}
