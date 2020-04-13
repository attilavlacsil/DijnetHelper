using System.Threading.Tasks;
using DijnetHelper.Logic.Model;
using Xamarin.Essentials;

namespace DijnetHelper.Logic
{
    // store user credentials in a secure way
    public class DijnetCredentialManager : ICredentialManager<DijnetCredential>
    {
        private const string RememberKey = "dijnet_remember";
        private const string RememberYesValue = "yes";
        private const string AutoLoginKey = "dijnet_autologin";
        private const string AutoLoginYesValue = "yes";
        private const string UserNameKey = "dijnet_username";
        private const string PasswordKey = "dijnet_password";

        public async Task<DijnetCredential> GetStoredCredentialAsync()
        {
            string userName = await SecureStorage.GetAsync(UserNameKey);
            string password = await SecureStorage.GetAsync(PasswordKey);
            bool remember = await SecureStorage.GetAsync(RememberKey) == RememberYesValue;
            bool autoLogin = await SecureStorage.GetAsync(AutoLoginKey) == AutoLoginYesValue;

            return new DijnetCredential(userName, password, remember, autoLogin);
        }

        public async Task UpdateCredentialAsync(DijnetCredential credential)
        {
            if (credential.Remember)
            {
                await SecureStorage.SetAsync(RememberKey, RememberYesValue);
                await SecureStorage.SetAsync(UserNameKey, credential.UserName);
                await SecureStorage.SetAsync(PasswordKey, credential.Password);
                if (credential.AutoLogin)
                {
                    await SecureStorage.SetAsync(AutoLoginKey, AutoLoginYesValue);
                }
                else
                {
                    SecureStorage.Remove(AutoLoginKey);
                }
            }
            else
            {
                SecureStorage.Remove(RememberKey);
                SecureStorage.Remove(UserNameKey);
                SecureStorage.Remove(PasswordKey);
                SecureStorage.Remove(AutoLoginKey);
            }
        }
    }
}
