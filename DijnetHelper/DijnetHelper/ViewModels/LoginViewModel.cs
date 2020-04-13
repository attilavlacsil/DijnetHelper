namespace DijnetHelper.ViewModels
{
    public class LoginViewModel : ViewModel
    {
        private string userName;
        private string password;
        private bool remember;
        private bool autoLogin;

        private string status;
        private string statusDetail;
        private bool isManualLogin;
        private bool isLoggingIn;

        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value, onChanged: () => OnPropertiesChanged(nameof(IsValid), nameof(CanLogIn)));
        }

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value, onChanged: () => OnPropertiesChanged(nameof(IsValid), nameof(CanLogIn)));
        }

        public bool Remember
        {
            get => remember;
            set => SetProperty(ref remember, value, onChanged: () => { if (!value) AutoLogin = false; });
        }

        public bool AutoLogin
        {
            get => autoLogin;
            set => SetProperty(ref autoLogin, value, onChanged: () => { if (value) Remember = true; });
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

        public bool IsManualLogin
        {
            get => isManualLogin;
            set => SetProperty(ref isManualLogin, value);
        }

        public bool IsLoggingIn
        {
            get => isLoggingIn;
            set => SetProperty(ref isLoggingIn, value, onChanged: () => OnPropertyChanged(nameof(CanLogIn)));
        }

        public bool IsValid => !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password);

        public bool CanLogIn => IsValid && !IsLoggingIn;
    }
}
