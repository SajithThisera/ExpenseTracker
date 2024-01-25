using ExpenseTracker.Enums;

namespace ExpenseTracker.Classes.Models.Transaction
{
    public class SingleTransaction : TransactionBase
    {
        public SingleTransaction(
            string name,
            decimal amount,
            TransactionTypes type,
            DateTime timestamp)
            : base(name, amount, type, timestamp)
        {
        }
    }
}
