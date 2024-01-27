using ExpenseTracker.Application;
using ExpenseTracker.Classes.Static;

namespace ExpenseTracker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Tracker.ExecuteRecurringTransactions();
            UIDrawer.DisplayMenuUI();
        }

    }
}