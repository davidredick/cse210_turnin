
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

// StorageManager.cs - Where financial data goes to hibernate between app sessions
public class StorageManager
{
    // Figures out where to store your financial regrets
    private static string GetDataFilePath(string fileName = "budgetData.json")
    {
        string appDataPath = AppDomain.CurrentDomain.BaseDirectory; // The app's home base
        return Path.Combine(appDataPath, fileName); // The final resting place of your transaction data
    }

    // Preserves your meticulously crafted category system for posterity
    public static void SaveCategories(List<Category> categories)
    {
        try
        {
            string filePath = GetDataFilePath("categories.json"); // Where your organizational obsession lives
            var options = new JsonSerializerOptions { WriteIndented = true }; // Make it pretty, because even JSON deserves dignity
            string json = JsonSerializer.Serialize(categories, options); // Transform objects into text
            File.WriteAllText(filePath, json); // Commit your financial organization to disk
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving categories: {ex.Message}"); // When even saving categories fails you
            throw; // Pass the problem up the chain, like a hot potato
        }
    }

    // Resurrects your category system from its JSON tomb
    public static List<Category> LoadCategories()
    {
        try
        {
            string filePath = GetDataFilePath("categories.json"); // The address of your category archive
            if (File.Exists(filePath)) // Check if your past self left you any categories
            {
                string json = File.ReadAllText(filePath); // Read the ancient texts
                var options = new JsonSerializerOptions { WriteIndented = true }; // Prepare the decoder ring
                return JsonSerializer.Deserialize<List<Category>>(json, options) ?? new List<Category>(); // Bring categories back to life
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading categories: {ex.Message}"); // When the past refuses to be remembered
        }

        // Return default categories if none exist - everyone needs a starting point
        return GetDefaultCategories(); // Plan B: pre-made categories for the organizationally challenged
    }

    private static List<Category> GetDefaultCategories()
    {
        var categories = new List<Category>();

        // Income categories
        var incomeCategory = new Category("Income", CategoryType.Income);
        categories.Add(incomeCategory);

        // Income subcategories
        var salaryCategory = new Category("Salary", CategoryType.Income, incomeCategory.Id);
        categories.Add(salaryCategory);
        categories.Add(new Category("Bonus", CategoryType.Income, salaryCategory.Id));
        categories.Add(new Category("Commission", CategoryType.Income, salaryCategory.Id));

        var investmentCategory = new Category("Investments", CategoryType.Income, incomeCategory.Id);
        categories.Add(investmentCategory);
        categories.Add(new Category("Dividends", CategoryType.Income, investmentCategory.Id));
        categories.Add(new Category("Interest", CategoryType.Income, investmentCategory.Id));
        categories.Add(new Category("Capital Gains", CategoryType.Income, investmentCategory.Id));

        var otherIncomeCategory = new Category("Other Income", CategoryType.Income, incomeCategory.Id);
        categories.Add(otherIncomeCategory);
        categories.Add(new Category("Gifts", CategoryType.Income, otherIncomeCategory.Id));
        categories.Add(new Category("Refunds", CategoryType.Income, otherIncomeCategory.Id));
        categories.Add(new Category("Freelance", CategoryType.Income, otherIncomeCategory.Id));

        // Expense categories
        var expenseCategory = new Category("Expenses", CategoryType.Expense);
        categories.Add(expenseCategory);

        // Housing expenses
        var housingCategory = new Category("Housing", CategoryType.Expense, expenseCategory.Id);
        categories.Add(housingCategory);
        categories.Add(new Category("Rent/Mortgage", CategoryType.Expense, housingCategory.Id));
        categories.Add(new Category("Property Taxes", CategoryType.Expense, housingCategory.Id));
        categories.Add(new Category("Home Insurance", CategoryType.Expense, housingCategory.Id));
        categories.Add(new Category("Home Repairs", CategoryType.Expense, housingCategory.Id));

        // Utilities expenses
        var utilitiesCategory = new Category("Utilities", CategoryType.Expense, expenseCategory.Id);
        categories.Add(utilitiesCategory);
        categories.Add(new Category("Electricity", CategoryType.Expense, utilitiesCategory.Id));
        categories.Add(new Category("Water", CategoryType.Expense, utilitiesCategory.Id));
        categories.Add(new Category("Gas", CategoryType.Expense, utilitiesCategory.Id));
        categories.Add(new Category("Internet", CategoryType.Expense, utilitiesCategory.Id));
        categories.Add(new Category("Phone", CategoryType.Expense, utilitiesCategory.Id));

        // Food expenses
        var foodCategory = new Category("Food", CategoryType.Expense, expenseCategory.Id);
        categories.Add(foodCategory);
        categories.Add(new Category("Groceries", CategoryType.Expense, foodCategory.Id));
        categories.Add(new Category("Restaurants", CategoryType.Expense, foodCategory.Id));
        categories.Add(new Category("Coffee Shops", CategoryType.Expense, foodCategory.Id));

        // Transportation expenses
        var transportationCategory = new Category("Transportation", CategoryType.Expense, expenseCategory.Id);
        categories.Add(transportationCategory);
        categories.Add(new Category("Car Payment", CategoryType.Expense, transportationCategory.Id));
        categories.Add(new Category("Gas", CategoryType.Expense, transportationCategory.Id));
        categories.Add(new Category("Car Insurance", CategoryType.Expense, transportationCategory.Id));
        categories.Add(new Category("Car Maintenance", CategoryType.Expense, transportationCategory.Id));
        categories.Add(new Category("Public Transit", CategoryType.Expense, transportationCategory.Id));

        // Other common expense categories
        categories.Add(new Category("Healthcare", CategoryType.Expense, expenseCategory.Id));
        categories.Add(new Category("Entertainment", CategoryType.Expense, expenseCategory.Id));
        categories.Add(new Category("Shopping", CategoryType.Expense, expenseCategory.Id));
        categories.Add(new Category("Education", CategoryType.Expense, expenseCategory.Id));
        categories.Add(new Category("Personal Care", CategoryType.Expense, expenseCategory.Id));
        categories.Add(new Category("Subscriptions", CategoryType.Expense, expenseCategory.Id));
        categories.Add(new Category("Gifts & Donations", CategoryType.Expense, expenseCategory.Id));
        categories.Add(new Category("Travel", CategoryType.Expense, expenseCategory.Id));

        return categories;
    }

    public static void SaveTransactions(List<Transaction> transactions)
    {
        try
        {
            string filePath = GetDataFilePath();
            var options = new JsonSerializerOptions {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };

            // Add a custom converter to handle polymorphic serialization
            options.Converters.Add(new TransactionJsonConverter());

            string json = JsonSerializer.Serialize(transactions, options);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving transactions: {ex.Message}");
            throw;
        }
    }

    public static List<Transaction> LoadTransactions()
    {
        try
        {
            string filePath = GetDataFilePath();
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions {
                    WriteIndented = true,
                    Converters = { new JsonStringEnumConverter() }
                };

                // Add a custom converter to handle polymorphic deserialization
                options.Converters.Add(new TransactionJsonConverter());

                return JsonSerializer.Deserialize<List<Transaction>>(json, options) ?? new List<Transaction>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading transactions: {ex.Message}");
        }
        return new List<Transaction>();
    }
}

// Custom JSON converter for Transaction polymorphism
public class TransactionJsonConverter : JsonConverter<Transaction>
{
    public override bool CanConvert(Type typeToConvert) =>
        typeof(Transaction).IsAssignableFrom(typeToConvert);

    public override Transaction Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var root = jsonDoc.RootElement;

        // Check for type discriminator
        if (!root.TryGetProperty("$type", out var typeProperty))
            throw new JsonException("Missing type discriminator property '$type'");

        string typeDiscriminator = typeProperty.GetString() ?? "";

        // Clone the JSON without the converter to avoid infinite recursion
        var newOptions = new JsonSerializerOptions(options);
        // Create a new list without the TransactionJsonConverter
        var filteredConverters = new List<JsonConverter>();
        foreach (var converter in newOptions.Converters)
        {
            if (!(converter is TransactionJsonConverter))
            {
                filteredConverters.Add(converter);
            }
        }
        // Clear and re-add the filtered converters
        newOptions.Converters.Clear();
        foreach (var converter in filteredConverters)
        {
            newOptions.Converters.Add(converter);
        }

        var json = root.GetRawText();

        return typeDiscriminator switch
        {
            "Income" => JsonSerializer.Deserialize<Income>(json, newOptions)!,
            "Expense" => JsonSerializer.Deserialize<Expense>(json, newOptions)!,
            _ => throw new JsonException($"Unknown transaction type: {typeDiscriminator}")
        };
    }

    public override void Write(Utf8JsonWriter writer, Transaction value, JsonSerializerOptions options)
    {
        // Clone the JSON without the converter to avoid infinite recursion
        var newOptions = new JsonSerializerOptions(options);
        // Create a new list without the TransactionJsonConverter
        var filteredConverters = new List<JsonConverter>();
        foreach (var converter in newOptions.Converters)
        {
            if (!(converter is TransactionJsonConverter))
            {
                filteredConverters.Add(converter);
            }
        }
        // Clear and re-add the filtered converters
        newOptions.Converters.Clear();
        foreach (var converter in filteredConverters)
        {
            newOptions.Converters.Add(converter);
        }

        // Start the JSON object
        writer.WriteStartObject();

        // Add type discriminator
        writer.WriteString("$type", value.GetType().Name);

        // Add all the properties from the base class
        writer.WriteString("Description", value.Description);
        writer.WriteString("Category", value.Category);
        writer.WriteNumber("Amount", value.Amount);
        writer.WriteString("Date", value.Date.ToString("o"));

        // End the JSON object
        writer.WriteEndObject();
    }
}



