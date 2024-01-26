using ExpenseTracker.Classes.Models.Category;
using ExpenseTracker.Classes.Models.Transaction;
using ExpenseTracker.Enums;
using ExpenseTracker.Interfaces;
using ExpenseTracker.Interfaces.Models;
using ExpenseTracker.Services;
using Microsoft.SqlServer.Server;
using System.Data.SqlClient;
using System.Globalization;
using System.Transactions;

namespace ExpenseTracker // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DisplayMenuUI();
            while (true)
            {
                
            }
        }

        static void GetTransactions()
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

        static void InsertTransaction()
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
                    insertQuery = "INSERT INTO Transactions(t_name, category_id, t_amount, t_type, t_timestamp, t_recurringtype, t_enddate, t_nextexecution) VALUES('" + transaction.Name + "', " + categoryId + ", " + transaction.Amount + ", " + (int)transaction.Type + ", '" + transaction.TimeStamp.ToString(format) + "', "+ (int)transaction.RecurringType + ", '" + transaction.EndDate.ToString(format) + "', '" + transaction.NextExecutionDate.ToString(format) + "')";
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

        static void DeleteTransaction()
        {
            DatabaseConnection dbConnnection = DatabaseConnection.Instance;
        }

        static void UpdateTransaction()
        {
            DatabaseConnection dbConnnection = DatabaseConnection.Instance;
        }

        static void GetCategories()
        {
            DisplayCategories();

            Console.WriteLine();
            Console.WriteLine("Press Enter to exit...");
            string? exit = Console.ReadLine();
        }

        static void DisplayCategories()
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


        static void InsertCategory()
        {
            DatabaseConnection dbConnnection = DatabaseConnection.Instance;
            Console.WriteLine("Enter category name:");
            string? categoryName = Console.ReadLine();
            decimal categoryBudget = ReadDecimal("Enter category budget:");

            Category category = new Category(categoryName, categoryBudget);
            // Console.WriteLine("Add a transaction to the category");

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

        static void DeleteCategory()
        {
            DatabaseConnection dbConnnection = DatabaseConnection.Instance;
        }

        static void UpdateCategory()
        {
            DatabaseConnection dbConnnection = DatabaseConnection.Instance;
        }

        static void ExecuteRecurringTransactions()
        {
            DatabaseConnection dbConnnection = DatabaseConnection.Instance;
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

                switch (userInput)
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
        enum MenuType { Main, Transaction, Category }

        static string[] GetCurrentMenuOptions(MenuType currentMenuType, params string[][] menuOptions)
        {
            switch (currentMenuType)
            {
                case MenuType.Main:
                    return menuOptions[0];
                case MenuType.Transaction:
                    return menuOptions[1];
                case MenuType.Category:
                    return menuOptions[2];
                default:
                    return new string[0];
            }
        }

        static void HandleMainMenuOption(int currentIndex, ref MenuType currentMenuType)
        {
            switch (currentIndex)
            {
                case 0: // Transactions
                    currentMenuType = MenuType.Transaction;
                    break;
                case 1: // Categories
                    currentMenuType = MenuType.Category;
                    break;
                case 2: // Exit
                    Environment.Exit(0);
                    break;
            }
        }

        static void RedirectToSubMenuFunctions(int index, MenuType menuType)
        {
            switch (menuType)
            {
                case MenuType.Transaction:
                    switch (index)
                    {
                        case 0:
                            InsertTransaction();
                            break;
                        case 1:
                            UpdateTransaction();
                            break;
                        case 2:
                            DeleteTransaction();
                            break;
                        case 3:
                            GetTransactions();
                            break;
                    }
                    break;

                case MenuType.Category:
                    switch (index)
                    {
                        case 0:
                            InsertCategory();
                            break;
                        case 1:
                            UpdateCategory();
                            break;
                        case 2:
                            DeleteCategory();
                            break;
                        case 3:
                            GetCategories();
                            break;
                    }
                    break;
            }
        }

        static void HandleSubMenuOption(int currentIndex, ref MenuType currentMenuType)
        {
            switch (currentMenuType)
            {
                case MenuType.Transaction:
                    if (currentIndex == 4) // Back option
                        currentMenuType = MenuType.Main;
                    else
                        RedirectToSubMenuFunctions(currentIndex, currentMenuType);
                    break;
                case MenuType.Category:
                    if (currentIndex == 4) // Back option
                        currentMenuType = MenuType.Main;
                    else
                        RedirectToSubMenuFunctions(currentIndex, currentMenuType);
                    break;
            }
        }

        static int GetLongestMenuOptionLength(string[] menuOptions)
        {
            int maxLength = 0;
            foreach (var option in menuOptions)
            {
                if (option.Length > maxLength)
                    maxLength = option.Length;
            }
            return maxLength;
        }

        private static void DisplayHeader()
        {
            Console.WriteLine(" _____________________________________________________________________________________________________________________ ");
            Console.WriteLine("|                                                                                                                     |");
            Console.WriteLine("|                                                                                                                     |");
            Console.WriteLine("|                                                  EXPENSE TRACKER                                                    |");
            Console.WriteLine("|                                                                                                                     |");
            Console.WriteLine("|_____________________________________________________________________________________________________________________|");
        }

        private static void DisplayMenuSubHeader()
        {
            Console.WriteLine();
            Console.WriteLine(" ________________________________________ SELECT AN OPTION FROM BELOW LIST ___________________________________________");
            Console.WriteLine();
        }

        private static void DisplaySubMenuSubHeader(string subMenuName)
        {
            Console.WriteLine();
            Console.WriteLine($"                                                   {subMenuName} MENU                                               ");
            Console.WriteLine();
        }

        private static void DisplayMenuUI()
        {
            string[] mainMenuOptions = { "Transactions", "Categories", "Exit" };
            string[] transactionMenuOptions = { "Create", "Update", "Delete", "View", "Back" };
            string[] categoryMenuOptions = { "Create", "Update", "Delete", "View", "Back" };

            int currentMenuIndex = 0;
            MenuType currentMenuType = MenuType.Main;

            ConsoleKeyInfo keyInfo;

            do
            {
                Console.Clear();
                DisplayHeader();
                DisplayMenuSubHeader();

                if (currentMenuType is not MenuType.Main)
                {
                    DisplaySubMenuSubHeader(currentMenuType.ToString().ToUpper());
                }

                string[] currentMenuOptions = GetCurrentMenuOptions(currentMenuType, mainMenuOptions, transactionMenuOptions, categoryMenuOptions);
                
                // Calculate center alignment for each menu item
                int windowWidth = Console.WindowWidth;
                int longestMenuOptionLength = GetLongestMenuOptionLength(currentMenuOptions);
                int leftPadding = (windowWidth - longestMenuOptionLength) / 2;
                
                // Display menu options
                for (int i = 0; i < currentMenuOptions.Length; i++)
                {
                    if (i == currentMenuIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    Console.SetCursorPosition(leftPadding, Console.CursorTop);
                    Console.WriteLine(currentMenuOptions[i].PadRight(longestMenuOptionLength));
                    Console.ResetColor();
                }

                keyInfo = Console.ReadKey();

                // Handle user input
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (currentMenuType == MenuType.Main)
                            currentMenuIndex = (currentMenuIndex == 0) ? mainMenuOptions.Length - 1 : currentMenuIndex - 1;
                        else
                            currentMenuIndex = (currentMenuIndex == 0) ? transactionMenuOptions.Length - 1 : currentMenuIndex - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        if (currentMenuType == MenuType.Main)
                            currentMenuIndex = (currentMenuIndex + 1) % mainMenuOptions.Length;
                        else
                            currentMenuIndex = (currentMenuIndex + 1) % transactionMenuOptions.Length;
                        break;
                    case ConsoleKey.Enter:
                        if (currentMenuType == MenuType.Main)
                            HandleMainMenuOption(currentMenuIndex, ref currentMenuType);
                        else
                            HandleSubMenuOption(currentMenuIndex, ref currentMenuType);
                        break;
                }

            } while (keyInfo.Key != ConsoleKey.Escape || currentMenuType != MenuType.Main); // Continue until 'Escape' is pressed on the main menu

            Console.WriteLine("Exiting the application...");        
        }
    }
}