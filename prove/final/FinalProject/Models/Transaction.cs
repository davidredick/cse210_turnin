
using System;

// Transaction.cs - The abstract representation of money's comings and goings, mostly goings
public abstract class Transaction
{
    public string Description { get; set; } // What you tell yourself to justify the purchase
    public string Category { get; set; } // This now stores the category ID, not "impulse buy" as it should
    public decimal Amount { get; set; } // The number that brings joy or pain
    public DateTime Date { get; set; } // When the financial damage occurred

    // Constructor: creating financial memories since 2025
    public Transaction(string description, string categoryId, decimal amount, DateTime date)
    {
        Description = description; // Your financial alibi
        Category = categoryId; // For sorting your regrets
        Amount = amount; // The cold, hard truth
        Date = date; // The crime scene timestamp
    }

    // Basic string representation - keeping it simple for those who can't handle the full truth
    public override string ToString()
    {
        return $"{Date.ToShortDateString()} - {Description} ${Amount}"; // The receipt of shame or glory
    }

    // Fancy version with category path - for the financially sophisticated
    public string ToString(string categoryPath)
    {
        return $"{Date.ToShortDateString()} - {categoryPath}: {Description} ${Amount}"; // The detailed evidence
    }
}

