using ExpenseTracker.Classes.Models.Transaction;
using ExpenseTracker.Enums;
using ExpenseTracker.Interfaces;
using ExpenseTracker.Interfaces.Models;

namespace ExpenseTracker.Services
{
    public class RecurringTransactionFactory : ITransactionFactory
    {
        private RecurringTypes _recurringType;
        private DateTime _endDate;
        private DateTime _nextExecutionDate;

        public RecurringTransactionFactory(
            RecurringTypes recurringType,
            DateTime endDate,
            DateTime nextExecutionDate)
        {
            _recurringType = recurringType;
            _endDate = endDate;
            _nextExecutionDate = nextExecutionDate;
            
        }
        public ITransaction CreateTransaction(
            string name,
            decimal amount,
            TransactionTypes type,
            DateTime timestamp)
        {
            return new RecurringTransaction(
                name,
                amount,
                type,
                timestamp,
                _recurringType,
                _endDate,
                _nextExecutionDate);
        }
    }
}
