using System;

namespace DijnetHelper.Model
{
    public class Bill
    {
        public string Provider { get; set; }
        public string ProviderId { get; set; }
        public string Id { get; set; }
        public DateTime IssueDate { get; set; }
        public Price TotalPrice { get; set; }
        public DateTime DueDate { get; set; }
        public Price PriceToPay { get; set; }
        public BillStatus Status { get; set; }
    }
}
