using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

// BudgetManager.cs - The financial command center where dreams meet reality (and reality usually wins)
public class BudgetManager
{
    private List<Transaction> transactions = new List<Transaction>(); // The ledger of financial decisions, good and bad
    private List<Category> categories = new List<Category>(); // The organizational system for your money habits
    private Dictionary<string, Category> categoryMap = new Dictionary<string, Category>(); // For quick category lookups, because time is money

    // Loads your financial history from storage - brace yourself
    public void LoadBudget()
    {
        transactions = StorageManager.LoadTransactions(); // Resurrect past financial decisions
        categories = StorageManager.LoadCategories(); // Load your organizational system

        // Build category map and hierarchy - the family tree of your spending habits
        BuildCategoryHierarchy(); // Connect the dots between parent and child categories
    }

    // Reconstructs the category hierarchy - like assembling a financial family tree
    private void BuildCategoryHierarchy()
    {
        // Clear existing data - start with a clean slate
        categoryMap.Clear(); // Out with the old

        // Build the category map for quick lookup - the address book of your categories
        foreach (var category in categories)
        {
            categoryMap[category.Id] = category; // Register each category by its ID
            category.Subcategories.Clear(); // Remove any existing children
        }

        // Build the hierarchy - establish the parent-child relationships
        foreach (var category in categories.Where(c => !string.IsNullOrEmpty(c.ParentId)))
        {
            if (category.ParentId != null && categoryMap.ContainsKey(category.ParentId)) // If we can find the parent
            {
                categoryMap[category.ParentId].Subcategories.Add(category); // Add this category as a child
            }
        }
    }

    // Preserves your financial data for future reference (or regret)
    public void SaveBudget()
    {
        StorageManager.SaveTransactions(transactions); // Archive your spending history
        StorageManager.SaveCategories(categories); // Save your organizational masterpiece
    }

    public void AddIncome()
    {
        // Get income categories
        var allIncomeCategories = categories.Where(c => c.Type == CategoryType.Income).ToList();

        if (allIncomeCategories.Count == 0)
        {
            Console.WriteLine("No income categories available. Please add categories first.");
            return;
        }

        // Get leaf categories (those that can be selected)
        var selectableCategories = new List<Category>();
        var categoryPaths = new Dictionary<int, string>();
        int index = 1;

        // First, add all categories without subcategories
        foreach (var category in allIncomeCategories)
        {
            // Skip root categories with subcategories
            if (category.Subcategories.Count > 0)
                continue;

            selectableCategories.Add(category);

            // Build the full path for display
            string path = BuildCategoryPath(category);
            categoryPaths[index] = path;
            index++;
        }

        // Display income categories with their full paths
        Console.Clear();
        Console.WriteLine("===== Select Income Category =====");

        for (int i = 0; i < selectableCategories.Count; i++)
        {
            Console.WriteLine($"{i+1}. {categoryPaths[i+1]}");
        }

        // Get category selection
        Console.Write("\nEnter category number: ");
        if (!int.TryParse(Console.ReadLine(), out int categoryIndex) ||
            categoryIndex < 1 || categoryIndex > selectableCategories.Count)
        {
            Console.WriteLine("Invalid category selection. Operation cancelled.");
            return;
        }

        Category selectedCategory = selectableCategories[categoryIndex - 1];
        string categoryPath = categoryPaths[categoryIndex];

        // Get description
        Console.Write("\nEnter income description: ");
        string? descriptionInput = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(descriptionInput))
        {
            Console.WriteLine("Invalid description. Operation cancelled.");
            return;
        }
        string description = descriptionInput;

        // Get amount
        Console.Write("Enter amount: ");
        string? amountInput = Console.ReadLine();
        if (!decimal.TryParse(amountInput, out decimal amount))
        {
            Console.WriteLine("Invalid amount. Operation cancelled.");
            return;
        }

        transactions.Add(new Income(description, selectedCategory.Id, amount, DateTime.Now));
        SaveBudget(); // Save after adding income

        Console.WriteLine($"\nIncome added to category: {categoryPath}");
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    public void AddExpense()
    {
        // Get expense categories
        var allExpenseCategories = categories.Where(c => c.Type == CategoryType.Expense).ToList();

        if (allExpenseCategories.Count == 0)
        {
            Console.WriteLine("No expense categories available. Please add categories first.");
            return;
        }

        // Get leaf categories (those that can be selected)
        var selectableCategories = new List<Category>();
        var categoryPaths = new Dictionary<int, string>();
        int index = 1;

        // First, add all categories without subcategories
        foreach (var category in allExpenseCategories)
        {
            // Skip root categories with subcategories
            if (category.Subcategories.Count > 0)
                continue;

            selectableCategories.Add(category);

            // Build the full path for display
            string path = BuildCategoryPath(category);
            categoryPaths[index] = path;
            index++;
        }

        // Display expense categories with their full paths
        Console.Clear();
        Console.WriteLine("===== Select Expense Category =====");

        for (int i = 0; i < selectableCategories.Count; i++)
        {
            Console.WriteLine($"{i+1}. {categoryPaths[i+1]}");
        }

        // Get category selection
        Console.Write("\nEnter category number: ");
        if (!int.TryParse(Console.ReadLine(), out int categoryIndex) ||
            categoryIndex < 1 || categoryIndex > selectableCategories.Count)
        {
            Console.WriteLine("Invalid category selection. Operation cancelled.");
            return;
        }

        Category selectedCategory = selectableCategories[categoryIndex - 1];
        string categoryPath = categoryPaths[categoryIndex];

        // Get description
        Console.Write("\nEnter expense description: ");
        string? descriptionInput = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(descriptionInput))
        {
            Console.WriteLine("Invalid description. Operation cancelled.");
            return;
        }
        string description = descriptionInput;

        // Get amount
        Console.Write("Enter amount: ");
        string? amountInput = Console.ReadLine();
        if (!decimal.TryParse(amountInput, out decimal amount))
        {
            Console.WriteLine("Invalid amount. Operation cancelled.");
            return;
        }

        transactions.Add(new Expense(description, selectedCategory.Id, amount, DateTime.Now));
        SaveBudget(); // Save after adding expense

        Console.WriteLine($"\nExpense added to category: {categoryPath}");
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    public void DisplaySummary()
    {
        Console.Clear();
        decimal totalIncome = 0;
        decimal totalExpenses = 0;

        Console.WriteLine("===== Budget Summary =====");
        Console.WriteLine($"Date: {DateTime.Now.ToShortDateString()}\n");

        // Group transactions by category type and category
        var incomeTransactions = transactions.Where(t => t is Income).ToList();
        var expenseTransactions = transactions.Where(t => t is Expense).ToList();

        // Display income by category
        Console.WriteLine("===== INCOME =====");
        if (incomeTransactions.Count > 0)
        {
            // Get root income categories
            var rootIncomeCategories = categories
                .Where(c => c.Type == CategoryType.Income && string.IsNullOrEmpty(c.ParentId))
                .ToList();

            foreach (var rootCategory in rootIncomeCategories)
            {
                DisplayCategorySummary(rootCategory, incomeTransactions, 0, ref totalIncome);
            }
        }
        else
        {
            Console.WriteLine("No income transactions found.");
        }

        Console.WriteLine($"\nTotal Income: ${totalIncome:N2}");

        // Display expenses by category
        Console.WriteLine("\n===== EXPENSES =====");
        if (expenseTransactions.Count > 0)
        {
            // Get root expense categories
            var rootExpenseCategories = categories
                .Where(c => c.Type == CategoryType.Expense && string.IsNullOrEmpty(c.ParentId))
                .ToList();

            foreach (var rootCategory in rootExpenseCategories)
            {
                DisplayCategorySummary(rootCategory, expenseTransactions, 0, ref totalExpenses);
            }
        }
        else
        {
            Console.WriteLine("No expense transactions found.");
        }

        Console.WriteLine($"\nTotal Expenses: ${totalExpenses:N2}");

        // Display balance
        decimal balance = totalIncome - totalExpenses;
        Console.WriteLine("\n===== SUMMARY =====");
        Console.WriteLine($"Total Income: ${totalIncome:N2}");
        Console.WriteLine($"Total Expenses: ${totalExpenses:N2}");
        Console.WriteLine($"Remaining Balance: ${balance:N2}");

        Console.WriteLine("\nPress Enter to continue...");
        Console.ReadLine();
    }

    private decimal DisplayCategorySummary(Category category, List<Transaction> transactions, int level, ref decimal totalAmount)
    {
        string indent = new string(' ', level * 4);

        // Get transactions for this category
        var categoryTransactions = transactions.Where(t => t.Category == category.Id).ToList();

        // Calculate amount for this category
        decimal categoryAmount = categoryTransactions.Sum(t => t.Amount);

        // Calculate amounts for subcategories
        decimal subcategoriesAmount = 0;
        foreach (var subcategory in category.Subcategories)
        {
            subcategoriesAmount += DisplayCategorySummary(subcategory, transactions, level + 1, ref totalAmount);
        }

        // Calculate total for this category including subcategories
        decimal totalForCategory = categoryAmount + subcategoriesAmount;

        // Only display categories with transactions
        if (totalForCategory > 0)
        {
            // Display the category with its amount
            Console.WriteLine($"{indent}{category.Name}: ${totalForCategory:N2}");

            // Display transactions for this category
            if (categoryTransactions.Count > 0)
            {
                foreach (var transaction in categoryTransactions)
                {
                    Console.WriteLine($"{indent}    - {transaction.Date.ToShortDateString()}: {transaction.Description} ${transaction.Amount:N2}");
                }
            }
        }

        // Add to the total amount
        totalAmount += categoryAmount;

        return totalForCategory;
    }

    public void GenerateReport()
    {
        Console.Clear();
        Console.WriteLine("===== Generate Report =====");

        // Choose report format
        Console.WriteLine("Select report format:");
        Console.WriteLine("1. Text (.txt)");
        Console.WriteLine("2. CSV (.csv)");
        Console.Write("Enter your choice: ");

        if (!int.TryParse(Console.ReadLine(), out int formatChoice) || formatChoice < 1 || formatChoice > 2)
        {
            Console.WriteLine("Invalid format selection. Operation cancelled.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return;
        }

        ReportGenerator.ReportFormat format = formatChoice == 1 ?
            ReportGenerator.ReportFormat.Text :
            ReportGenerator.ReportFormat.CSV;

        // Get file path
        string defaultExtension = format == ReportGenerator.ReportFormat.Text ? ".txt" : ".csv";
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string defaultDirectory = Path.Combine("C:", "budget_reports");
        string defaultFileName = $"myreport_{timestamp}{defaultExtension}";
        string defaultFullPath = Path.Combine(defaultDirectory, defaultFileName);

        // Display file save dialog simulation
        Console.Clear();
        Console.WriteLine("===== Save Report =====");
        Console.WriteLine("\nPlease specify where to save your report:\n");

        // Show default path
        Console.WriteLine($"Default location: {defaultFullPath}");
        Console.WriteLine("\nOptions:");
        Console.WriteLine("1. Use default location");
        Console.WriteLine("2. Specify a different location");
        Console.WriteLine("3. Save to current directory");
        Console.Write("\nEnter your choice (1-3): ");

        string? saveChoice = Console.ReadLine();
        string filePath;

        switch (saveChoice)
        {
            case "1":
                // Use default location
                filePath = defaultFullPath;
                break;

            case "2":
                // Specify a different location
                Console.WriteLine("\nEnter the full path where you want to save the file:");
                Console.WriteLine("Example: C:\\MyDocuments\\BudgetReports\\report.csv");
                string? customPath = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(customPath))
                {
                    Console.WriteLine("No path specified. Using default location.");
                    filePath = defaultFullPath;
                }
                else
                {
                    filePath = customPath;

                    // Add extension if missing
                    if (!filePath.EndsWith(defaultExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        filePath += defaultExtension;
                    }
                }
                break;

            case "3":
                // Save to current directory
                filePath = defaultFileName;
                break;

            default:
                Console.WriteLine("Invalid choice. Using default location.");
                filePath = defaultFullPath;
                break;
        }

        try
        {
            // Ensure directory exists
            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                try
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine($"\nError: You don't have permission to create the directory '{directory}'.");
                    Console.WriteLine("Saving to the current directory instead.");
                    filePath = Path.GetFileName(filePath);
                }
                catch (Exception dirEx)
                {
                    Console.WriteLine($"\nError creating directory: {dirEx.Message}");
                    Console.WriteLine("Saving to the current directory instead.");
                    filePath = Path.GetFileName(filePath);
                }
            }

            // Generate the report
            string savedPath = ReportGenerator.GenerateReport(transactions, filePath, format);
            Console.WriteLine($"\nReport successfully saved as:\n{savedPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError generating report: {ex.Message}");
            Console.WriteLine("\nTrying to save to the current directory instead...");

            try
            {
                // Fallback to current directory
                string fallbackPath = Path.GetFileName(filePath);
                if (string.IsNullOrEmpty(fallbackPath))
                {
                    fallbackPath = $"myreport_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}{defaultExtension}";
                }

                string savedPath = ReportGenerator.GenerateReport(transactions, fallbackPath, format);
                Console.WriteLine($"\nReport successfully saved as:\n{savedPath}");
            }
            catch (Exception fallbackEx)
            {
                Console.WriteLine($"\nFailed to save report: {fallbackEx.Message}");
            }
        }

        Console.WriteLine("\nPress Enter to continue...");
        Console.ReadLine();
    }

    public void DisplayTransactions()
    {
        Console.Clear();
        Console.WriteLine("===== All Transactions =====");

        if (transactions.Count == 0)
        {
            Console.WriteLine("\nNo transactions found.");
            return;
        }

        for (int i = 0; i < transactions.Count; i++)
        {
            var transaction = transactions[i];
            string categoryPath = "Uncategorized";

            // Get category path if category exists
            if (categoryMap.ContainsKey(transaction.Category))
            {
                categoryPath = BuildCategoryPath(categoryMap[transaction.Category]);
            }

            // Display transaction with category path
            string type = transaction is Income ? "Income" : "Expense";
            Console.WriteLine($"{i+1}. [{type}] {transaction.Date.ToShortDateString()} - {categoryPath}: {transaction.Description} ${transaction.Amount:N2}");
        }

        Console.WriteLine("============================");
    }

    public void ModifyTransaction()
    {
        DisplayTransactions();

        if (transactions.Count == 0)
        {
            Console.WriteLine("No transactions to modify. Press Enter to continue...");
            Console.ReadLine();
            return;
        }

        Console.Write("Enter the number of the transaction to modify: ");
        if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > transactions.Count)
        {
            Console.WriteLine("Invalid selection. Operation cancelled.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return;
        }

        Transaction transaction = transactions[index - 1];

        Console.WriteLine("\nWhat would you like to do?");
        Console.WriteLine("1. Edit transaction");
        Console.WriteLine("2. Delete transaction");
        Console.Write("Enter your choice: ");

        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                EditSelectedTransaction(transaction);
                break;
            case "2":
                DeleteSelectedTransaction(index - 1, transaction);
                break;
            default:
                Console.WriteLine("Invalid choice. Operation cancelled.");
                break;
        }

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    private void EditSelectedTransaction(Transaction transaction)
    {
        // Get appropriate categories based on transaction type
        List<Category> availableCategories;
        if (transaction is Income)
        {
            availableCategories = categories.Where(c => c.Type == CategoryType.Income).ToList();
        }
        else
        {
            availableCategories = categories.Where(c => c.Type == CategoryType.Expense).ToList();
        }

        // Display current category and available categories
        Console.WriteLine($"Current category: {transaction.Category}");
        Console.WriteLine("Available categories:");
        for (int i = 0; i < availableCategories.Count; i++)
        {
            Console.WriteLine($"{i+1}. {availableCategories[i].Name}");
        }

        // Get new category
        Console.Write("Enter new category number (leave blank to keep current): ");
        string? categoryInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(categoryInput) &&
            int.TryParse(categoryInput, out int categoryIndex) &&
            categoryIndex >= 1 && categoryIndex <= availableCategories.Count)
        {
            transaction.Category = availableCategories[categoryIndex - 1].Name;
        }

        // Get new description
        Console.Write($"Enter new description (current: {transaction.Description}): ");
        string? descInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(descInput))
        {
            transaction.Description = descInput;
        }

        // Get new amount
        Console.Write($"Enter new amount (current: {transaction.Amount}): ");
        string? amountInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(amountInput) && decimal.TryParse(amountInput, out decimal amount))
        {
            transaction.Amount = amount;
        }

        // Get new date
        Console.Write($"Enter new date (current: {transaction.Date.ToShortDateString()}, format: MM/dd/yyyy): ");
        string? dateInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(dateInput) && DateTime.TryParse(dateInput, out DateTime date))
        {
            transaction.Date = date;
        }

        SaveBudget(); // Save after editing transaction
        Console.WriteLine("Transaction updated successfully.");
    }

    private void DeleteSelectedTransaction(int index, Transaction transaction)
    {
        Console.Write($"Are you sure you want to delete this transaction? (y/n): {transaction} ");
        string? confirm = Console.ReadLine()?.ToLower();

        if (confirm == "y" || confirm == "yes")
        {
            transactions.RemoveAt(index);
            SaveBudget(); // Save after deleting transaction
            Console.WriteLine("Transaction deleted successfully.");
        }
        else
        {
            Console.WriteLine("Deletion cancelled.");
        }
    }

    // Helper method to build the full category path
    private string BuildCategoryPath(Category category)
    {
        if (string.IsNullOrEmpty(category.ParentId))
            return category.Name;

        var path = new List<string>();
        path.Add(category.Name);

        string? currentParentId = category.ParentId;
        while (!string.IsNullOrEmpty(currentParentId) && categoryMap.ContainsKey(currentParentId))
        {
            path.Add(categoryMap[currentParentId].Name);
            currentParentId = categoryMap[currentParentId].ParentId;
        }

        path.Reverse();
        return string.Join(" → ", path);
    }

    public void ManageCategories()
    {
        bool managing = true;
        while (managing)
        {
            Console.Clear();
            Console.WriteLine("===== Category Management =====");
            Console.WriteLine("1. Add New Category");
            Console.WriteLine("2. View All Categories");
            Console.WriteLine("3. Edit Category");
            Console.WriteLine("4. Delete Category");
            Console.WriteLine("5. Return to Main Menu");
            Console.Write("Choose an option: ");

            string? choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    AddCategory();
                    break;
                case "2":
                    ViewCategories();
                    break;
                case "3":
                    EditCategory();
                    break;
                case "4":
                    DeleteCategory();
                    break;
                case "5":
                    managing = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Press Enter to continue...");
                    Console.ReadLine();
                    break;
            }
        }
    }

    private void ViewCategories()
    {
        Console.Clear();
        Console.WriteLine("===== Categories =====");

        // Display income categories hierarchically
        Console.WriteLine("\nINCOME CATEGORIES:");
        var rootIncomeCategories = categories.Where(c => c.Type == CategoryType.Income && string.IsNullOrEmpty(c.ParentId)).ToList();
        foreach (var category in rootIncomeCategories)
        {
            DisplayCategoryHierarchy(category, 0);
        }

        // Display expense categories hierarchically
        Console.WriteLine("\nEXPENSE CATEGORIES:");
        var rootExpenseCategories = categories.Where(c => c.Type == CategoryType.Expense && string.IsNullOrEmpty(c.ParentId)).ToList();
        foreach (var category in rootExpenseCategories)
        {
            DisplayCategoryHierarchy(category, 0);
        }

        Console.WriteLine("\nPress Enter to continue...");
        Console.ReadLine();
    }

    private void DisplayCategoryHierarchy(Category category, int level)
    {
        // Create indentation based on level
        string indent = new string(' ', level * 4);
        string prefix = level == 0 ? string.Empty : "└─ ";

        // Display the current category
        Console.WriteLine($"{indent}{prefix}{category.Name}");

        // Display subcategories
        foreach (var subcategory in category.Subcategories)
        {
            DisplayCategoryHierarchy(subcategory, level + 1);
        }
    }

    private void AddCategory()
    {
        Console.Clear();
        Console.WriteLine("===== Add New Category =====");

        // Get category type
        Console.WriteLine("Select category type:");
        Console.WriteLine("1. Income");
        Console.WriteLine("2. Expense");
        Console.Write("Enter your choice: ");

        if (!int.TryParse(Console.ReadLine(), out int typeChoice) || typeChoice < 1 || typeChoice > 2)
        {
            Console.WriteLine("Invalid type selection. Operation cancelled.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return;
        }

        CategoryType type = typeChoice == 1 ? CategoryType.Income : CategoryType.Expense;

        // Get parent category (optional)
        string? parentId = null;
        bool selectParent = false;

        Console.WriteLine("\nDo you want to add this as a subcategory? (y/n): ");
        string? parentChoice = Console.ReadLine()?.ToLower();
        if (parentChoice == "y" || parentChoice == "yes")
        {
            selectParent = true;
        }

        if (selectParent)
        {
            // Get potential parent categories
            var potentialParents = categories
                .Where(c => c.Type == type)
                .OrderBy(c => c.GetLevel(categoryMap))
                .ToList();

            if (potentialParents.Count == 0)
            {
                Console.WriteLine("No existing categories found to use as parent.");
            }
            else
            {
                // Display potential parent categories with their levels
                Console.WriteLine("\nSelect parent category:");
                for (int i = 0; i < potentialParents.Count; i++)
                {
                    int level = potentialParents[i].GetLevel(categoryMap);
                    if (level >= 2) // Don't allow creating categories deeper than 3 levels
                    {
                        continue;
                    }

                    string path = BuildCategoryPath(potentialParents[i]);
                    Console.WriteLine($"{i+1}. {path}");
                }

                Console.Write("Enter parent category number (or 0 for no parent): ");
                if (int.TryParse(Console.ReadLine(), out int parentIndex) &&
                    parentIndex > 0 && parentIndex <= potentialParents.Count)
                {
                    var selectedParent = potentialParents[parentIndex - 1];
                    int parentLevel = selectedParent.GetLevel(categoryMap);

                    if (parentLevel >= 2)
                    {
                        Console.WriteLine("Cannot create a category at this depth (maximum 3 levels).");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        return;
                    }

                    parentId = selectedParent.Id;
                }
            }
        }

        // Get category name
        Console.Write("\nEnter category name: ");
        string? nameInput = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(nameInput))
        {
            Console.WriteLine("Invalid name. Operation cancelled.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return;
        }

        string name = nameInput.Trim();

        // Check if category already exists at the same level with the same parent
        if (categories.Any(c => c.Type == type &&
                          c.ParentId == parentId &&
                          c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine("A category with this name already exists at this level. Operation cancelled.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return;
        }

        // Add the new category
        var newCategory = new Category(name, type, parentId);
        categories.Add(newCategory);

        // Rebuild the hierarchy
        BuildCategoryHierarchy();

        SaveBudget(); // Save after adding category

        // Show the full path of the new category
        string categoryPath = BuildCategoryPath(newCategory);
        Console.WriteLine($"\nCategory added successfully: {categoryPath}");
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    private void EditCategory()
    {
        Console.Clear();
        Console.WriteLine("===== Edit Category =====");

        // Choose category type to edit
        Console.WriteLine("Select category type to edit:");
        Console.WriteLine("1. Income Categories");
        Console.WriteLine("2. Expense Categories");
        Console.Write("Enter your choice: ");

        if (!int.TryParse(Console.ReadLine(), out int typeChoice) || typeChoice < 1 || typeChoice > 2)
        {
            Console.WriteLine("Invalid type selection. Operation cancelled.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return;
        }

        CategoryType type = typeChoice == 1 ? CategoryType.Income : CategoryType.Expense;

        // Get categories of selected type
        var filteredCategories = categories.Where(c => c.Type == type).ToList();
        if (filteredCategories.Count == 0)
        {
            Console.WriteLine($"No {type} categories found. Operation cancelled.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return;
        }

        // Display categories with their full paths
        Console.WriteLine($"\nSelect {type} category to edit:");
        var categoryPaths = new Dictionary<int, string>();
        for (int i = 0; i < filteredCategories.Count; i++)
        {
            string path = BuildCategoryPath(filteredCategories[i]);
            categoryPaths[i+1] = path;
            Console.WriteLine($"{i+1}. {path}");
        }

        // Get category selection
        Console.Write("\nEnter category number: ");
        if (!int.TryParse(Console.ReadLine(), out int categoryIndex) ||
            categoryIndex < 1 || categoryIndex > filteredCategories.Count)
        {
            Console.WriteLine("Invalid category selection. Operation cancelled.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return;
        }

        Category selectedCategory = filteredCategories[categoryIndex - 1];
        string currentPath = categoryPaths[categoryIndex];

        // Get new name
        Console.WriteLine($"\nCurrent category: {currentPath}");
        Console.Write($"Enter new name for '{selectedCategory.Name}' (leave blank to keep current): ");
        string? nameInput = Console.ReadLine();

        string newName = selectedCategory.Name;
        if (!string.IsNullOrWhiteSpace(nameInput))
        {
            newName = nameInput.Trim();

            // Check if new name already exists at the same level with the same parent
            if (categories.Any(c => c != selectedCategory &&
                              c.Type == type &&
                              c.ParentId == selectedCategory.ParentId &&
                              c.Name.Equals(newName, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("A category with this name already exists at this level. Operation cancelled.");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                return;
            }
        }

        // Option to change parent (move category)
        Console.WriteLine("\nDo you want to move this category to a different parent? (y/n): ");
        string? moveChoice = Console.ReadLine()?.ToLower();

        if (moveChoice == "y" || moveChoice == "yes")
        {
            // Get potential parent categories (excluding the current category and its descendants)
            var potentialParents = categories
                .Where(c => c.Type == type && c.Id != selectedCategory.Id)
                .OrderBy(c => c.GetLevel(categoryMap))
                .ToList();

            // Remove descendants of the selected category
            potentialParents = potentialParents
                .Where(c => !IsDescendantOf(c, selectedCategory))
                .ToList();

            if (potentialParents.Count == 0)
            {
                Console.WriteLine("No suitable parent categories found.");
            }
            else
            {
                // Display potential parent categories
                Console.WriteLine("\nSelect new parent category:");
                Console.WriteLine("0. No parent (top level)");

                for (int i = 0; i < potentialParents.Count; i++)
                {
                    int level = potentialParents[i].GetLevel(categoryMap);
                    if (level >= 2) // Don't allow creating categories deeper than 3 levels
                    {
                        continue;
                    }

                    string path = BuildCategoryPath(potentialParents[i]);
                    Console.WriteLine($"{i+1}. {path}");
                }

                Console.Write("Enter parent category number: ");
                if (int.TryParse(Console.ReadLine(), out int parentIndex))
                {
                    if (parentIndex == 0)
                    {
                        // Set as top-level category
                        selectedCategory.ParentId = null;
                    }
                    else if (parentIndex > 0 && parentIndex <= potentialParents.Count)
                    {
                        var newParent = potentialParents[parentIndex - 1];
                        int parentLevel = newParent.GetLevel(categoryMap);

                        if (parentLevel >= 2)
                        {
                            Console.WriteLine("Cannot move to this parent (maximum 3 levels).");
                        }
                        else
                        {
                            selectedCategory.ParentId = newParent.Id;
                        }
                    }
                }
            }
        }

        // Update category name
        selectedCategory.Name = newName;

        // Update transactions using this category
        foreach (var transaction in transactions)
        {
            if (transaction.Category == selectedCategory.Id)
            {
                // Keep using the same category ID
            }
        }

        // Rebuild the hierarchy
        BuildCategoryHierarchy();

        SaveBudget(); // Save after editing category

        // Show the updated path
        string newPath = BuildCategoryPath(selectedCategory);
        Console.WriteLine($"\nCategory updated successfully: {newPath}");
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    // Helper method to check if a category is a descendant of another
    private bool IsDescendantOf(Category potentialDescendant, Category ancestor)
    {
        if (string.IsNullOrEmpty(potentialDescendant.ParentId))
            return false;

        if (potentialDescendant.ParentId == ancestor.Id)
            return true;

        if (categoryMap.ContainsKey(potentialDescendant.ParentId))
            return IsDescendantOf(categoryMap[potentialDescendant.ParentId], ancestor);

        return false;
    }

    private void DeleteCategory()
    {
        Console.Clear();
        Console.WriteLine("===== Delete Category =====");

        // Choose category type to delete
        Console.WriteLine("Select category type to delete:");
        Console.WriteLine("1. Income Categories");
        Console.WriteLine("2. Expense Categories");
        Console.Write("Enter your choice: ");

        if (!int.TryParse(Console.ReadLine(), out int typeChoice) || typeChoice < 1 || typeChoice > 2)
        {
            Console.WriteLine("Invalid type selection. Operation cancelled.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return;
        }

        CategoryType type = typeChoice == 1 ? CategoryType.Income : CategoryType.Expense;

        // Get categories of selected type
        var filteredCategories = categories.Where(c => c.Type == type).ToList();
        if (filteredCategories.Count == 0)
        {
            Console.WriteLine($"No {type} categories found. Operation cancelled.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return;
        }

        // Display categories with their full paths
        Console.WriteLine($"\nSelect {type} category to delete:");
        var categoryPaths = new Dictionary<int, string>();
        for (int i = 0; i < filteredCategories.Count; i++)
        {
            string path = BuildCategoryPath(filteredCategories[i]);
            categoryPaths[i+1] = path;
            Console.WriteLine($"{i+1}. {path}");
        }

        // Get category selection
        Console.Write("\nEnter category number: ");
        if (!int.TryParse(Console.ReadLine(), out int categoryIndex) ||
            categoryIndex < 1 || categoryIndex > filteredCategories.Count)
        {
            Console.WriteLine("Invalid category selection. Operation cancelled.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return;
        }

        Category selectedCategory = filteredCategories[categoryIndex - 1];
        string categoryPath = categoryPaths[categoryIndex];

        // Check if category has subcategories
        if (selectedCategory.Subcategories.Count > 0)
        {
            Console.WriteLine("This category has subcategories and cannot be deleted. Delete the subcategories first.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return;
        }

        // Check if category is in use by transactions
        bool categoryInUse = transactions.Any(t => t.Category == selectedCategory.Id);
        if (categoryInUse)
        {
            Console.WriteLine("This category is in use by one or more transactions and cannot be deleted.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return;
        }

        // Confirm deletion
        Console.Write($"\nAre you sure you want to delete the category '{categoryPath}'? (y/n): ");
        string? confirm = Console.ReadLine()?.ToLower();

        if (confirm == "y" || confirm == "yes")
        {
            categories.Remove(selectedCategory);

            // Rebuild the hierarchy
            BuildCategoryHierarchy();

            SaveBudget(); // Save after deleting category
            Console.WriteLine("\nCategory deleted successfully.");
        }
        else
        {
            Console.WriteLine("\nDeletion cancelled.");
        }

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }
}



