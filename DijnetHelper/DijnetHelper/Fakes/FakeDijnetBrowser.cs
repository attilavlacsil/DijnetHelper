using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DijnetHelper.Logic;
using DijnetHelper.Logic.Model;
using DijnetHelper.Model;
using Bill = DijnetHelper.Logic.Model.Bill;

namespace DijnetHelper.Fakes
{
    // to support testing
    public class FakeDijnetBrowser : IDijnetBrowser
    {
        private int invoiceCounter = 1;

        public async Task<List<Bill>> GetBillsAsync()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500));

            int invoiceCount;

            switch (invoiceCounter++ % 3)
            {
                case 1:
                    invoiceCount = 3;
                    break;
                case 2:
                    invoiceCount = 30;
                    break;
                default:
                    invoiceCount = 0;
                    break;
            }

            List<Bill> invoices = Enumerable.Range(1, invoiceCount).Select(i => new Bill
            {
                Provider = $"Provider {i}",
                ProviderId = $"Provider #{i}",
                Id = $"ID{i}",
                IssueDate = DateTime.Now,
                TotalPrice = new Price { Value = 1234 * i, Currency = "Ft" },
                DueDate = DateTime.Now.AddDays(i),
                PriceToPay = new Price { Value = 1234 * i, Currency = "Ft" },
                ElementId = i
            }).ToList();

            return invoices;
        }

        public async Task<List<Card>> SelectBillAndGetCardsAsync(Bill bill)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500));

            List<Card> cards;

            switch (bill.ElementId % 3)
            {
                case 1:
                    cards = new List<Card>
                    {
                        new Card { Id = "carda", Name = "Card A" },
                        new Card { Id = "cardb", Name = "Card B", Default = true },
                        new Card { Id = "cardc", Name = "Card C" }
                    };
                    break;
                case 2:
                    cards = new List<Card>
                    {
                        new Card { Id = "cardx", Name = "Card X", Default = true }
                    };
                    break;
                default:
                    cards = new List<Card>(0);
                    break;
            }

            return cards;
        }

        public async Task<PayResult> PayBillAsync(Card card)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500));

            // payment is successful if card is 'cardx' or non-default
            bool success = card.Id == "cardx" || !card.Default;

            PayResult result = new PayResult
            {
                Success = success,
                Error = success ? null : "Error detail about failed payment."
            };

            return result;
        }

        public async Task<bool> LogoutAsync()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(200));
            return true;
        }
    }
}
