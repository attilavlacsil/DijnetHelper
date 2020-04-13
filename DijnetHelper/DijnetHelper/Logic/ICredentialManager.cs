using System.Threading.Tasks;

namespace DijnetHelper.Logic
{
    public interface ICredentialManager<TCredential>
    {
        Task<TCredential> GetStoredCredentialAsync();
        Task UpdateCredentialAsync(TCredential credential);
    }
}