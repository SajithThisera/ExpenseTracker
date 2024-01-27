using ExpenseTracker.Enums;
using ExpenseTracker.Interfaces.Models;

namespace ExpenseTracker.Classes.Models.Transaction
{
    public class TransactionBase : ITransaction
    {
        public TransactionBase(
            string name,
            decimal amount,
            TransactionTypes type,
            DateTime timestamp)
        {
            Name = name;
            Amount = amount;
            Type = type;
            TimeStamp = timestamp;
        }

        public TransactionBase()
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Amount { get; set; }

        public TransactionTypes Type { get; set; }

        public DateTime TimeStamp { get; set; }

        public ITransaction Update(
            string name,
            decimal amount)
        {
            Name = name;
            Amount = amount;

            return this;
        }
    }
}
