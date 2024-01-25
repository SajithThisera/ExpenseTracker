using ExpenseTracker.Classes.Models.Transaction;
using ExpenseTracker.Enums;
using ExpenseTracker.Interfaces;
using ExpenseTracker.Interfaces.Models;

namespace ExpenseTracker.Services
{
    public class SingleTransactionFactory : ITransactionFactory
    {
        public ITransaction CreateTransaction(
            string name,
            decimal amount,
            TransactionTypes type,
            DateTime timestamp)
        {
            return new SingleTransaction(
                name,
                amount,
                type,
                timestamp);
        }
    }
}
