using System.Collections.Generic;
using System.Threading.Tasks;
using DijnetHelper.Logic.Model;
using DijnetHelper.Model;
using Bill = DijnetHelper.Logic.Model.Bill;

namespace DijnetHelper.Logic
{
    public interface IDijnetBrowser
    {
        Task<List<Bill>> GetBillsAsync();
        Task<List<Card>> SelectBillAndGetCardsAsync(Bill bill);
        Task<PayResult> PayBillAsync(Card card);
        Task<bool> LogoutAsync();
    }
}