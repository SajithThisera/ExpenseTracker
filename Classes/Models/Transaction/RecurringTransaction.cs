using ExpenseTracker.Enums;

namespace ExpenseTracker.Classes.Models.Transaction
{
    public class RecurringTransaction : TransactionBase
    {
        public RecurringTransaction(
            string name,
            decimal amount,
            TransactionTypes type,
            DateTime timestamp,
            RecurringTypes recurringType,
            DateTime endDate,
            DateTime nextExecutionDate)
            : base(name, amount, type, timestamp)
        {
            RecurringType = recurringType;
            EndDate = endDate;
            NextExecutionDate = nextExecutionDate;
        }

        public RecurringTransaction()
        {
        }

        public RecurringTypes RecurringType { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime NextExecutionDate { get; set; }
    }
}
