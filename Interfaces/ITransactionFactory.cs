using ExpenseTracker.Enums;
using ExpenseTracker.Interfaces.Models;

namespace ExpenseTracker.Interfaces
{
    public interface ITransactionFactory
    {
        ITransaction CreateTransaction(string name,
            decimal amount,
            TransactionTypes type,
            DateTime timestamp);

    }
}
