namespace DijnetHelper.Logic.Model
{
    public class DijnetCredential
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Remember { get; set; }
        public bool AutoLogin { get; set; }

        public bool IsValid => !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password);

        public DijnetCredential(string userName, string password, bool remember, bool autoLogin)
        {
            UserName = userName;
            Password = password;
            Remember = remember;
            AutoLogin = autoLogin;
        }
    }
}