using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

// Category.cs - Because "Stuff I Shouldn't Have Bought" isn't specific enough
public class Category
{
    public string Name { get; set; } = ""; // The label for your financial life choices
    public CategoryType Type { get; set; } // Income or Expense, but mostly Expense
    public string Id { get; set; } = ""; // Unique identifier, like a social security number for your spending habits
    public string? ParentId { get; set; } // Family tree for your money decisions

    [JsonIgnore] // Hide from JSON because even serializers don't need to see all our financial chaos
    public List<Category> Subcategories { get; set; } = new List<Category>(); // For the financially OCD

    // Default constructor: for when JSON deserializer needs to recreate your financial organization system
    public Category()
    {
        Id = Guid.NewGuid().ToString(); // Generate a unique ID, as if your spending habits weren't unique enough
    }

    // The constructor that creates financial sorting buckets
    public Category(string name, CategoryType type, string? parentId = null)
    {
        Name = name; // What you call this money pit
        Type = type; // Whether it fills or empties your wallet
        Id = Guid.NewGuid().ToString(); // Unique ID, because even bad decisions deserve identification
        ParentId = parentId; // Who does this category report to in the hierarchy?
    }

    // For when you just want the name without the family history
    public string GetFullPath()
    {
        return Name; // Just the name, ma'am
    }

    // For when you want to show off your category's prestigious lineage
    public string GetFullPathWithParent(string parentPath)
    {
        return string.IsNullOrEmpty(parentPath) ? Name : $"{parentPath} → {Name}"; // Family tree notation with fancy arrow
    }

    // Basic string representation - keeping it simple
    public override string ToString()
    {
        return Name; // Identity crisis averted
    }

    // Determines how deep in the hierarchy rabbit hole we've gone
    public int GetLevel(Dictionary<string, Category> categoryMap)
    {
        if (string.IsNullOrEmpty(ParentId))
            return 0; // You're at the top of the food chain

        int level = 1; // Start counting the generations
        string? currentParentId = ParentId; // Begin the ancestry search

        while (!string.IsNullOrEmpty(currentParentId) && categoryMap.ContainsKey(currentParentId))
        {
            level++; // Another generation discovered
            currentParentId = categoryMap[currentParentId].ParentId; // Climb further up the family tree

            // Safety check to prevent infinite loops (in case your categories have time paradoxes)
            if (level > 10) break; // If we're 10 levels deep, someone's been having too much fun with categories
        }

        return level; // How many financial sorting boxes deep are we?
    }
}

// The two states of money: coming or going (mostly going)
public enum CategoryType
{
    Income,  // The mythical state where money enters your account
    Expense  // The default state of all financial transactions
}
