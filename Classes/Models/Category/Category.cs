using ExpenseTracker.Interfaces.Models;

namespace ExpenseTracker.Classes.Models.Category
{
    public class Category
    {
        public Category(
            string name,
            decimal budget)
        {
            Name = name;
            Budget = budget;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Budget { get; set; }

        public List<ITransaction> Transactions { get; set; }
    }
}
