
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

// ReportGenerator.cs - Turning financial chaos into neatly formatted spreadsheets since 2025
public class ReportGenerator
{
    // The two flavors of financial truth we support
    public enum ReportFormat
    {
        Text,  // For the old-school crowd who like their financial data in plain English
        CSV    // For the spreadsheet enthusiasts who need to pivot table their way to enlightenment
    }

    // The main report generation method - where financial data becomes a work of art
    public static string GenerateReport(List<Transaction> transactions, string filePath, ReportFormat format)
    {
        string? directory = Path.GetDirectoryName(filePath); // Extract the folder path
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) // If the folder doesn't exist
        {
            Directory.CreateDirectory(directory); // Create it, because we're helpful like that
        }

        // Get categories and build hierarchy - can't have a proper report without proper organization
        var categories = StorageManager.LoadCategories(); // Fetch the category system
        var categoryMap = BuildCategoryMap(categories); // Create a lookup table for quick access
        BuildCategoryHierarchy(categories, categoryMap); // Reconstruct the family tree

        switch (format) // Choose your financial documentation style
        {
            case ReportFormat.CSV: // For the Excel aficionados
                return GenerateCSVReport(transactions, categories, categoryMap, filePath); // Comma-separated financial truth
            case ReportFormat.Text: // For the plaintext purists
            default:
                return GenerateTextReport(transactions, categories, categoryMap, filePath); // Human-readable financial reality
        }
    }

    private static Dictionary<string, Category> BuildCategoryMap(List<Category> categories)
    {
        var categoryMap = new Dictionary<string, Category>();
        foreach (var category in categories)
        {
            categoryMap[category.Id] = category;
            category.Subcategories.Clear();
        }
        return categoryMap;
    }

    private static void BuildCategoryHierarchy(List<Category> categories, Dictionary<string, Category> categoryMap)
    {
        foreach (var category in categories.Where(c => !string.IsNullOrEmpty(c.ParentId)))
        {
            if (category.ParentId != null && categoryMap.ContainsKey(category.ParentId))
            {
                categoryMap[category.ParentId].Subcategories.Add(category);
            }
        }
    }

    private static string BuildCategoryPath(Category category, Dictionary<string, Category> categoryMap)
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

    private static string GenerateTextReport(List<Transaction> transactions, List<Category> categories, Dictionary<string, Category> categoryMap, string filePath)
    {
        decimal totalIncome = transactions.Where(t => t is Income).Sum(t => t.Amount);
        decimal totalExpenses = transactions.Where(t => t is Expense).Sum(t => t.Amount);
        decimal balance = totalIncome - totalExpenses;

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("===== Budget Report =====");
            writer.WriteLine($"Report Date: {DateTime.Now.ToShortDateString()}");
            writer.WriteLine();

            // Group transactions by category type
            var incomeTransactions = transactions.Where(t => t is Income).ToList();
            var expenseTransactions = transactions.Where(t => t is Expense).ToList();

            // Write income by category
            writer.WriteLine("===== INCOME =====");
            if (incomeTransactions.Count > 0)
            {
                // Get root income categories
                var rootIncomeCategories = categories
                    .Where(c => c.Type == CategoryType.Income && string.IsNullOrEmpty(c.ParentId))
                    .ToList();

                foreach (var rootCategory in rootIncomeCategories)
                {
                    WriteCategorySummary(writer, rootCategory, incomeTransactions, categoryMap, 0);
                }
            }
            else
            {
                writer.WriteLine("No income transactions found.");
            }

            writer.WriteLine($"\nTotal Income: ${totalIncome:N2}");

            // Write expenses by category
            writer.WriteLine("\n===== EXPENSES =====");
            if (expenseTransactions.Count > 0)
            {
                // Get root expense categories
                var rootExpenseCategories = categories
                    .Where(c => c.Type == CategoryType.Expense && string.IsNullOrEmpty(c.ParentId))
                    .ToList();

                foreach (var rootCategory in rootExpenseCategories)
                {
                    WriteCategorySummary(writer, rootCategory, expenseTransactions, categoryMap, 0);
                }
            }
            else
            {
                writer.WriteLine("No expense transactions found.");
            }

            writer.WriteLine($"\nTotal Expenses: ${totalExpenses:N2}");

            // Write balance
            writer.WriteLine("\n===== SUMMARY =====");
            writer.WriteLine($"Total Income: ${totalIncome:N2}");
            writer.WriteLine($"Total Expenses: ${totalExpenses:N2}");
            writer.WriteLine($"Remaining Balance: ${balance:N2}");
            writer.WriteLine("=========================");
        }

        return filePath;
    }

    private static decimal WriteCategorySummary(StreamWriter writer, Category category, List<Transaction> transactions, Dictionary<string, Category> categoryMap, int level)
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
            subcategoriesAmount += WriteCategorySummary(writer, subcategory, transactions, categoryMap, level + 1);
        }

        // Calculate total for this category including subcategories
        decimal totalForCategory = categoryAmount + subcategoriesAmount;

        // Only write categories with transactions
        if (totalForCategory > 0)
        {
            // Write the category with its amount
            writer.WriteLine($"{indent}{category.Name}: ${totalForCategory:N2}");

            // Write transactions for this category
            if (categoryTransactions.Count > 0)
            {
                foreach (var transaction in categoryTransactions)
                {
                    writer.WriteLine($"{indent}    - {transaction.Date.ToShortDateString()}: {transaction.Description} ${transaction.Amount:N2}");
                }
            }
        }

        return totalForCategory;
    }

    private static string GenerateCSVReport(List<Transaction> transactions, List<Category> categories, Dictionary<string, Category> categoryMap, string filePath)
    {
        decimal totalIncome = transactions.Where(t => t is Income).Sum(t => t.Amount);
        decimal totalExpenses = transactions.Where(t => t is Expense).Sum(t => t.Amount);
        decimal balance = totalIncome - totalExpenses;

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write header
            writer.WriteLine("Type,Date,Category Path,Description,Amount");

            // Write transactions with full category paths
            foreach (var transaction in transactions)
            {
                string type = transaction is Income ? "Income" : "Expense";
                string date = transaction.Date.ToShortDateString();

                // Get category path
                string categoryPath = "Uncategorized";
                if (categoryMap.ContainsKey(transaction.Category))
                {
                    categoryPath = BuildCategoryPath(categoryMap[transaction.Category], categoryMap);
                }

                // Escape commas in fields
                categoryPath = categoryPath.Replace(",", ";");
                string description = transaction.Description.Replace(",", ";");
                string amount = transaction.Amount.ToString("N2");

                writer.WriteLine($"{type},{date},\"{categoryPath}\",\"{description}\",{amount}");
            }

            writer.WriteLine();

            // Write category summaries
            writer.WriteLine("Category Summaries");
            writer.WriteLine("Category Type,Category Path,Amount");

            // Income categories
            var rootIncomeCategories = categories
                .Where(c => c.Type == CategoryType.Income && string.IsNullOrEmpty(c.ParentId))
                .ToList();

            foreach (var rootCategory in rootIncomeCategories)
            {
                WriteCSVCategorySummary(writer, rootCategory, transactions, categoryMap, "Income");
            }

            // Expense categories
            var rootExpenseCategories = categories
                .Where(c => c.Type == CategoryType.Expense && string.IsNullOrEmpty(c.ParentId))
                .ToList();

            foreach (var rootCategory in rootExpenseCategories)
            {
                WriteCSVCategorySummary(writer, rootCategory, transactions, categoryMap, "Expense");
            }

            // Write overall summary
            writer.WriteLine();
            writer.WriteLine("Overall Summary");
            writer.WriteLine($"Total Income,,{totalIncome:N2}");
            writer.WriteLine($"Total Expenses,,{totalExpenses:N2}");
            writer.WriteLine($"Remaining Balance,,{balance:N2}");
        }

        return filePath;
    }

    private static decimal WriteCSVCategorySummary(StreamWriter writer, Category category, List<Transaction> transactions, Dictionary<string, Category> categoryMap, string categoryType)
    {
        // Get transactions for this category
        var categoryTransactions = transactions.Where(t => t.Category == category.Id).ToList();

        // Calculate amount for this category
        decimal categoryAmount = categoryTransactions.Sum(t => t.Amount);

        // Calculate amounts for subcategories
        decimal subcategoriesAmount = 0;
        foreach (var subcategory in category.Subcategories)
        {
            subcategoriesAmount += WriteCSVCategorySummary(writer, subcategory, transactions, categoryMap, categoryType);
        }

        // Calculate total for this category including subcategories
        decimal totalForCategory = categoryAmount + subcategoriesAmount;

        // Only write categories with transactions
        if (totalForCategory > 0)
        {
            // Get category path
            string categoryPath = BuildCategoryPath(category, categoryMap);
            categoryPath = categoryPath.Replace(",", ";");

            // Write the category with its amount
            writer.WriteLine($"{categoryType},\"{categoryPath}\",{totalForCategory:N2}");
        }

        return totalForCategory;
    }
}

