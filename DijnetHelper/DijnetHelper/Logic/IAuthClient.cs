using System.Threading.Tasks;

namespace DijnetHelper.Logic
{
    public interface IAuthClient<TResult>
    {
        Task<TResult> LoginAsync(string userName, string password);
    }
}