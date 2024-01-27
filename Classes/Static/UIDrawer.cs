using ExpenseTracker.Application;

namespace ExpenseTracker.Classes.Static
{
    public static class UIDrawer
    {
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
                            Tracker.InsertTransaction();
                            break;
                        case 1:
                            Tracker.UpdateTransaction();
                            break;
                        case 2:
                            Tracker.DeleteTransaction();
                            break;
                        case 3:
                            Tracker.DisplayTransactions();
                            WaitForExit();
                            break;
                    }
                    break;

                case MenuType.Category:
                    switch (index)
                    {
                        case 0:
                            Tracker.InsertCategory();
                            break;
                        case 1:
                            Tracker.UpdateCategory();
                            break;
                        case 2:
                            Tracker.DeleteCategory();
                            break;
                        case 3:
                            Tracker.GetCategories();
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

        public static void WaitForExit()
        {
            Console.WriteLine();
            Console.WriteLine("Press Enter to exit...");
            string? exit = Console.ReadLine();
        }

        public static void DisplayMenuUI()
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
    }
}
