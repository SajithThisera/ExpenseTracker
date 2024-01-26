using ExpenseTracker.Classes.Models.Category;
using ExpenseTracker.Classes.Models.Transaction;
using ExpenseTracker.Enums;
using ExpenseTracker.Interfaces.Models;
using ExpenseTracker.Interfaces;
using ExpenseTracker.Services;
using System.Data.SqlClient;
using System.Globalization;
using ExpenseTracker.Classes.Static;

namespace ExpenseTracker.Application
{
    public static class Tracker
    {
        internal static void DisplayTransactions()
        {
            DatabaseConnection dbConnnection = DatabaseConnection.Instance;

            string readQuery = "SELECT * FROM Transactions";
            SqlCommand readCommand = new SqlCommand(readQuery, dbConnnection.connection);

            try
            {
                dbConnnection.connection.Open();
                SqlDataReader dataReader = readCommand.ExecuteReader();
                Console.WriteLine();
                Console.WriteLine(" _____________________________________________________________________________________________________________________ ");
                Console.WriteLine("Id \t|CatId\t|Name\t|Amount\t|Type\t|TimeStamp\t\t|Freq\t|EndDate\t\t|NextExecution");
                Console.WriteLine();
                while (dataReader.Read())
                {
                    Console.WriteLine(dataReader.GetValue(0).ToString()
                        + "\t" + dataReader.GetValue(1).ToString()
                        + "\t" + dataReader.GetValue(2).ToString()
                        + "\t" + dataReader.GetValue(3).ToString()
                        + "\t" + dataReader.GetValue(4).ToString()
                        + "\t" + dataReader.GetValue(5).ToString()
                        + "\t" + dataReader.GetValue(6).ToString()
                        + "\t" + dataReader.GetValue(7).ToString()
                        + "\t" + dataReader.GetValue(8).ToString());
                }

                dataReader.Close();
                dbConnnection.connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine("Press Enter to exit...");
            string? exit = Console.ReadLine();
        }

        internal static void InsertTransaction()
        {
            DatabaseConnection dbConnnection = DatabaseConnection.Instance;

            Console.WriteLine("Available categories:");
            Console.WriteLine();
            DisplayCategories();
            Console.WriteLine();
            Console.WriteLine("Enter category id:");
            int categoryId = ReadInt("");

            Console.WriteLine("Enter transaction name:");
            string? transactionName = Console.ReadLine();

            decimal transactionAmount = ReadDecimal("Enter transaction amount:");
            TransactionTypes transactionType = ReadTransactionType("Enter transaction type:");

            Console.WriteLine("Is this a recurring transaction?");
            bool isRecurringTransaction = ReadYesNoResponse();
            string insertQuery = null;

            string format = "yyyy-MM-dd HH:mm:ss";

            if (isRecurringTransaction)
            {
                RecurringTypes recurringType = ReadRecurrnigType("Enter recurring type:");
                DateTime endDate = ReadDateTime("Enter transaction end date");

                DateTime timestamp = DateTime.Now;
                DateTime nextExecutionOn = timestamp;
                switch (recurringType)
                {
                    case RecurringTypes.Daily:
                        nextExecutionOn = nextExecutionOn.AddDays(1);
                        break;
                    case RecurringTypes.Weekly:
                        nextExecutionOn = nextExecutionOn.AddDays(7);
                        break;
                    case RecurringTypes.Monthly:
                        nextExecutionOn = nextExecutionOn.AddDays(30);
                        break;
                    case RecurringTypes.Anual:
                        nextExecutionOn = nextExecutionOn.AddDays(365);
                        break;
                }

                ITransactionFactory recurringTransactionFactory = new RecurringTransactionFactory(recurringType, endDate, nextExecutionOn);
                ITransaction recurringTransaction = recurringTransactionFactory.CreateTransaction(transactionName, transactionAmount, transactionType, timestamp);

                if (recurringTransaction is RecurringTransaction transaction)
                {
                    insertQuery = "INSERT INTO Transactions(t_name, category_id, t_amount, t_type, t_timestamp, t_recurringtype, t_enddate, t_nextexecution) VALUES('" + transaction.Name + "', " + categoryId + ", " + transaction.Amount + ", " + (int)transaction.Type + ", '" + transaction.TimeStamp.ToString(format) + "', " + (int)transaction.RecurringType + ", '" + transaction.EndDate.ToString(format) + "', '" + transaction.NextExecutionDate.ToString(format) + "')";
                }

            }
            else
            {
                DateTime timestamp = DateTime.Now;
                ITransactionFactory singleTransactionFactory = new SingleTransactionFactory();
                ITransaction singleTransaction = singleTransactionFactory.CreateTransaction(transactionName, transactionAmount, transactionType, timestamp);

                insertQuery = "INSERT INTO Transactions(t_name, category_id, t_amount, t_type, t_timestamp) VALUES('" + singleTransaction.Name + "', " + categoryId + ", " + singleTransaction.Amount + ", " + (int)singleTransaction.Type + ", '" + singleTransaction.TimeStamp.ToString(format) + "')";
            }

            SqlCommand insertCommand = new SqlCommand(insertQuery, dbConnnection.connection);

            try
            {
                dbConnnection.connection.Open();
                insertCommand.ExecuteNonQuery();
                Console.WriteLine("Execution succeeded!");

                dbConnnection.connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        internal static void DeleteTransaction()
        {
            DatabaseConnection dbConnnection = DatabaseConnection.Instance;
        }

        internal static void UpdateTransaction()
        {
            DatabaseConnection dbConnnection = DatabaseConnection.Instance;
        }

        internal static void GetCategories()
        {
            DisplayCategories();

            UIDrawer.WaitForExit();
        }

        internal static void DisplayCategories()
        {
            DatabaseConnection dbConnnection = DatabaseConnection.Instance;
            Console.WriteLine();
            Console.WriteLine(" _____________________________________________________________________________________________________________________ ");
            string readQuery = "SELECT * FROM Categories";
            SqlCommand readCommand = new SqlCommand(readQuery, dbConnnection.connection);

            try
            {
                dbConnnection.connection.Open();
                SqlDataReader dataReader = readCommand.ExecuteReader();

                Console.WriteLine("Id \t|Name\t|Budget");
                Console.WriteLine();
                while (dataReader.Read())
                {
                    Console.WriteLine(dataReader.GetValue(0).ToString()
                        + "\t" + dataReader.GetValue(1).ToString()
                        + "\t" + dataReader.GetValue(2).ToString());
                }

                dataReader.Close();
                dbConnnection.connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        internal static void InsertCategory()
        {
            DatabaseConnection dbConnnection = DatabaseConnection.Instance;
            Console.WriteLine("Enter category name:");
            string? categoryName = Console.ReadLine();
            decimal categoryBudget = ReadDecimal("Enter category budget:");

            Category category = new Category(categoryName, categoryBudget);

            string insertQuery = "INSERT INTO Categories(c_name, c_budget) VALUES('" + category.Name + "', " + category.Budget + ")";

            SqlCommand insertCommand = new SqlCommand(insertQuery, dbConnnection.connection);

            try
            {
                dbConnnection.connection.Open();
                insertCommand.ExecuteNonQuery();
                Console.WriteLine("Execution succeeded!");

                dbConnnection.connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        internal static void DeleteCategory()
        {
            DatabaseConnection dbConnnection = DatabaseConnection.Instance;
        }

        internal static void UpdateCategory()
        {
            DatabaseConnection dbConnnection = DatabaseConnection.Instance;

            Console.WriteLine("Available categories:");
            Console.WriteLine();
            DisplayCategories();
            Console.WriteLine();
            Console.WriteLine("Enter category id:");
            int categoryId = ReadInt("");
            Category oldCategory = GetCategoryById(categoryId);

            if (oldCategory is not null)
            {
                Console.WriteLine();
                bool isChangeName = ReadYesNoResponse($"Change name {oldCategory.Name}?");
                if (isChangeName)
                {
                    Console.WriteLine("Enter new category name:");
                    string? categoryName = Console.ReadLine();
                    oldCategory.Name = string.IsNullOrEmpty(categoryName) ? oldCategory.Name : categoryName;
                }

                Console.WriteLine();
                bool isChangeBudget = ReadYesNoResponse($"Change budget {oldCategory.Budget}?");
                if (isChangeBudget)
                {
                    decimal categoryBudget = ReadDecimal("Enter category budget:");
                    oldCategory.Budget = categoryBudget;
                }

                string updateQuery = "UPDATE Categories SET c_name = '" + oldCategory.Name + "', c_budget = " + oldCategory.Budget + " WHERE c_id = " + oldCategory.Id + "";

                SqlCommand updateCommand = new SqlCommand(updateQuery, dbConnnection.connection);

                try
                {
                    dbConnnection.connection.Open();
                    updateCommand.ExecuteNonQuery();
                    Console.WriteLine("Execution succeeded!");

                    dbConnnection.connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Category not found!");
                UIDrawer.WaitForExit();
            }
        }

        private static Category GetCategoryById(int id)
        {

            DatabaseConnection dbConnnection = DatabaseConnection.Instance;

            string filterQuery = $"SELECT * FROM Categories WHERE c_id = {id};";
            SqlCommand readCommand = new SqlCommand(filterQuery, dbConnnection.connection);

            Category result = new Category();

            try
            {
                dbConnnection.connection.Open();
                SqlDataReader dataReader = readCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    result.Id = dataReader.GetInt32(dataReader.GetOrdinal("c_id"));
                    result.Name = dataReader.GetString(dataReader.GetOrdinal("c_name"));
                    result.Budget = dataReader.GetDecimal(dataReader.GetOrdinal("c_budget"));
                }

                dataReader.Close();
                dbConnnection.connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        private static List<RecurringTransaction> GetRecurringTransactionCategoryId(int id)
        {
            DatabaseConnection dbConnnection = DatabaseConnection.Instance;

            string filterQuery = $"SELECT * FROM Transactions WHERE category_id = {id} AND t_recurringtype IS NOT NULL;";
            SqlCommand readCommand = new SqlCommand(filterQuery, dbConnnection.connection);

            List<RecurringTransaction> results = new List<RecurringTransaction>();

            try
            {
                dbConnnection.connection.Open();
                SqlDataReader dataReader = readCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    RecurringTransaction transaction = new RecurringTransaction();

                    transaction.Id = dataReader.GetInt32(dataReader.GetOrdinal("t_id"));
                    transaction.Name = dataReader.GetString(dataReader.GetOrdinal("t_name"));
                    transaction.Amount = dataReader.GetDecimal(dataReader.GetOrdinal("t_amount"));
                    transaction.Type = (TransactionTypes)dataReader.GetInt32(dataReader.GetOrdinal("t_type"));
                    transaction.TimeStamp = dataReader.GetDateTime(dataReader.GetOrdinal("t_timestamp"));
                    transaction.RecurringType = (RecurringTypes)dataReader.GetInt32(dataReader.GetOrdinal("t_recurringtype"));
                    transaction.EndDate = dataReader.GetDateTime(dataReader.GetOrdinal("t_enddate"));
                    transaction.NextExecutionDate = dataReader.GetDateTime(dataReader.GetOrdinal("t_nextexecution"));

                    results.Add(transaction);
                }

                dataReader.Close();
                dbConnnection.connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return results;
        }

        private static List<SingleTransaction> GetSingleTransactionsByCategoryId(int id)
        {
            DatabaseConnection dbConnnection = DatabaseConnection.Instance;

            string filterQuery = $"SELECT * FROM Transactions WHERE category_id = {id} AND t_recurringtype IS NULL;";
            SqlCommand readCommand = new SqlCommand(filterQuery, dbConnnection.connection);

            List<SingleTransaction> results = new List<SingleTransaction>();

            try
            {
                dbConnnection.connection.Open();
                SqlDataReader dataReader = readCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    SingleTransaction transaction = new SingleTransaction();

                    transaction.Id = dataReader.GetInt32(dataReader.GetOrdinal("t_id"));
                    transaction.Name = dataReader.GetString(dataReader.GetOrdinal("t_name"));
                    transaction.Amount = dataReader.GetDecimal(dataReader.GetOrdinal("t_amount"));
                    transaction.Type = (TransactionTypes)dataReader.GetInt32(dataReader.GetOrdinal("t_type"));
                    transaction.TimeStamp = dataReader.GetDateTime(dataReader.GetOrdinal("t_timestamp"));

                    results.Add(transaction);
                }

                dataReader.Close();
                dbConnnection.connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return results;
        }

        public static void ExecuteRecurringTransactions()
        {
            DatabaseConnection dbConnnection = DatabaseConnection.Instance;

            string filterQuery = "SELECT * FROM Transactions WHERE t_nextexecution is not null AND CAST(t_nextexecution AS DATE) < CAST(GETDATE() AS DATE);";
            SqlCommand readCommand = new SqlCommand(filterQuery, dbConnnection.connection);

            List<RecurringTransaction> results = new List<RecurringTransaction>();

            try
            {
                dbConnnection.connection.Open();
                SqlDataReader dataReader = readCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    RecurringTransaction transaction = new RecurringTransaction();

                    transaction.Id = dataReader.GetInt32(dataReader.GetOrdinal("t_id"));
                    transaction.Name = dataReader.GetString(dataReader.GetOrdinal("t_name"));
                    transaction.Amount = dataReader.GetDecimal(dataReader.GetOrdinal("t_amount"));
                    transaction.Type = (TransactionTypes)dataReader.GetInt32(dataReader.GetOrdinal("t_type"));
                    transaction.TimeStamp = dataReader.GetDateTime(dataReader.GetOrdinal("t_timestamp"));
                    transaction.RecurringType = (RecurringTypes)dataReader.GetInt32(dataReader.GetOrdinal("t_recurringtype"));
                    transaction.EndDate = dataReader.GetDateTime(dataReader.GetOrdinal("t_enddate"));
                    transaction.NextExecutionDate = dataReader.GetDateTime(dataReader.GetOrdinal("t_nextexecution"));

                    results.Add(transaction);
                }

                dataReader.Close();
                dbConnnection.connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            GenerateTransactions(results);
        }

        private static void GenerateTransactions(List<RecurringTransaction> input_transactions)
        {
            string format = "yyyy-MM-dd HH:mm:ss";
            int categoryId = 1; // TODO: Handle properly may be we need to add this to the base transaction
            DatabaseConnection dbConnnection = DatabaseConnection.Instance;

            foreach (var tr in input_transactions)
            {
                DateTime nextRecurr = tr.NextExecutionDate;

                while (nextRecurr < DateTime.UtcNow)
                {
                    string insertQuery;
                    switch (tr.RecurringType)
                    {
                        case RecurringTypes.Daily:
                            {
                                nextRecurr = nextRecurr.AddDays(1);
                                break;
                            }
                        case RecurringTypes.Weekly:
                            {
                                nextRecurr = nextRecurr.AddDays(7);
                                break;
                            }
                        case RecurringTypes.Monthly:
                            {
                                nextRecurr = nextRecurr.AddDays(30);
                                break;
                            }
                        case RecurringTypes.Anual:
                            {
                                nextRecurr = nextRecurr.AddDays(365);
                                break;
                            }
                    }

                    // Insert a new transaction
                    ITransactionFactory recurringTransactionFactory = new RecurringTransactionFactory(tr.RecurringType, tr.EndDate, nextRecurr);
                    ITransaction recurringTransaction = recurringTransactionFactory.CreateTransaction(tr.Name, tr.Amount, tr.Type, DateTime.UtcNow);
                    if (recurringTransaction is RecurringTransaction transaction)
                    {
                        insertQuery = "INSERT INTO Transactions(t_name, category_id, t_amount, t_type, t_timestamp, t_recurringtype, t_enddate, t_nextexecution) VALUES('" + transaction.Name + "', " + categoryId + ", " + transaction.Amount + ", " + (int)transaction.Type + ", '" + transaction.TimeStamp.ToString(format) + "', " + (int)transaction.RecurringType + ", '" + transaction.EndDate.ToString(format) + "', '" + transaction.NextExecutionDate.ToString(format) + "')";

                        SqlCommand insertCommand = new SqlCommand(insertQuery, dbConnnection.connection);
                        try
                        {
                            dbConnnection.connection.Open();
                            insertCommand.ExecuteNonQuery();
                            Console.WriteLine("Execution succeeded!");
                            dbConnnection.connection.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }

        private static decimal ReadDecimal(string title)
        {
            while (true)
            {
                Console.WriteLine(title);
                string userInput = Console.ReadLine();

                decimal result;

                // We use CultureInfo.InvariantCulture to ensure that
                // decimal separator is dot (.). i.e. we expect 5.5 input
                if (decimal.TryParse(userInput,
                                    NumberStyles.Any,
                                    CultureInfo.InvariantCulture,
                                    out result))
                {
                    return result;
                }

                Console.WriteLine("Sorry, incorrect format. Enter it again, please.");
            }
        }

        private static DateTime ReadDateTime(string title)
        {
            while (true)
            {
                Console.Write($"{title} (e.g. 10/22/2024): ");

                string userInput = Console.ReadLine();

                DateTime result;
                if (DateTime.TryParse(userInput,
                                    out result))
                {
                    return result;
                }

                Console.WriteLine("Sorry, incorrect format. Enter it again, please.");
            }
        }

        private static int ReadInt(string title)
        {
            while (true)
            {
                Console.WriteLine(title);

                string userInput = Console.ReadLine();

                int result;

                if (int.TryParse(userInput,
                                    NumberStyles.Any,
                                    CultureInfo.InvariantCulture,
                                    out result))
                {
                    return result;
                }

                Console.WriteLine("Sorry, incorrect format. Enter it again, please.");
            }
        }

        private static TransactionTypes ReadTransactionType(string title)
        {
            while (true)
            {
                Console.WriteLine(title);
                Console.WriteLine("0 : If the transaction is an income");
                Console.WriteLine("1 : If the transaction is an expense");
                Console.WriteLine("Enter the corrensponding value:");

                string userInput = Console.ReadLine();

                int result;

                // We use CultureInfo.InvariantCulture to ensure that
                // decimal separator is dot (.). i.e. we expect 5.5 input
                if (int.TryParse(userInput,
                                    NumberStyles.Any,
                                    CultureInfo.InvariantCulture,
                                    out result))
                {
                    switch (result)
                    {
                        case 0:
                            return TransactionTypes.Income;
                        case 1:
                            return TransactionTypes.Expense;
                        default:
                            Console.WriteLine("Sorry, invalid type. Enter either 0 or 1 again, please.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Sorry, incorrect format. Enter it again, please.");
                }
            }
        }

        private static bool ReadYesNoResponse(string? title = null)
        {
            while (true)
            {
                if (!String.IsNullOrEmpty(title))
                {
                    Console.WriteLine(title);
                }

                Console.WriteLine("Y : If yes");
                Console.WriteLine("N : If No");
                Console.WriteLine("Enter the corrensponding value:");
                string userInput = Console.ReadLine();

                switch (userInput.ToUpper())
                {
                    case "Y":
                        return true;
                    case "N":
                        return false;
                    default:
                        Console.WriteLine("Sorry, invalid type. Enter either Y or N again, please.");
                        break;
                }
            }
        }

        private static RecurringTypes ReadRecurrnigType(string title = null)
        {
            while (true)
            {
                if (!String.IsNullOrEmpty(title))
                {
                    Console.WriteLine(title);
                }
                Console.WriteLine("0 : If the transaction is recurring daily");
                Console.WriteLine("1 : If the transaction is recurring weekly");
                Console.WriteLine("2 : If the transaction is recurring monthly");
                Console.WriteLine("3 : If the transaction is recurring annually");
                Console.WriteLine("Enter the corrensponding value:");

                string userInput = Console.ReadLine();

                int value;

                if (int.TryParse(userInput,
                                    NumberStyles.Any,
                                    CultureInfo.InvariantCulture,
                                    out value))
                {
                    switch (value)
                    {
                        case 0:
                            return RecurringTypes.Daily;
                        case 1:
                            return RecurringTypes.Weekly;
                        case 2:
                            return RecurringTypes.Monthly;
                        case 3:
                            return RecurringTypes.Anual;
                        default:
                            Console.WriteLine("Sorry, value out of the valid range. Enter a valid value 0 - 3 again, please.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Sorry, incorrect format. Enter it again, please.");
                }
            }
        }
    }
}
