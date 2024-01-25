using ExpenseTracker.Enums;

namespace ExpenseTracker.Interfaces.Models
{
    public interface ITransaction
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public decimal Amount { get; set; }

        public TransactionTypes Type { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
