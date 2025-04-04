using System;

// Program.cs - The main entry point where financial dreams come to life (or die trying)
class Program
{
    static void Main()
    {
        // Create our financial wizard - less Gandalf, more Excel
        BudgetManager budgetManager = new BudgetManager();
        budgetManager.LoadBudget(); // Load previous financial decisions we'll probably regret

        bool running = true;
        while (running) // Like your bills, this loop never seems to end
        {
            Console.Clear(); // Clear the screen, if only we could clear debt this easily
            Console.WriteLine("===== BudgetBuddy ====="); // Your financial friend that won't judge your latte habit
            Console.WriteLine("1. Add Income"); // The happy button
            Console.WriteLine("2. Add Expense"); // The reality button
            Console.WriteLine("3. View Budget Summary"); // The truth hurts button
            Console.WriteLine("4. Modify Transaction"); // The "oops, that wasn't takeout" button
            Console.WriteLine("5. Manage Categories"); // For the organizationally obsessed
            Console.WriteLine("6. Generate Report"); // Evidence for your financial advisor
            Console.WriteLine("7. Exit"); // Escape from financial reality
            Console.Write("Choose an option: ");

            string? choice = Console.ReadLine(); // User input: where the magic or chaos begins
            switch (choice)
            {
                case "1":
                    budgetManager.AddIncome(); // Money coming in - savor this rare moment
                    break;
                case "2":
                    budgetManager.AddExpense(); // Money going out - the usual suspect
                    break;
                case "3":
                    budgetManager.DisplaySummary(); // The financial mirror of truth
                    break;
                case "4":
                    budgetManager.ModifyTransaction(); // Rewriting financial history
                    break;
                case "5":
                    budgetManager.ManageCategories(); // For those who color-code their sock drawer
                    break;
                case "6":
                    budgetManager.GenerateReport(); // Spreadsheet therapy
                    break;
                case "7":
                    budgetManager.SaveBudget(); // Save before fleeing the financial reality
                    running = false; // Freedom at last
                    break;
                default:
                    Console.WriteLine("Invalid option. Press Enter to continue..."); // User error handling with a hint of judgment
                    Console.ReadLine();
                    break;
            }
        }
    }
}




