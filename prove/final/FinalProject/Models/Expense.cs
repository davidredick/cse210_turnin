// Expense.cs - Where your money goes to die, usually at Amazon or Starbucks
public class Expense : Transaction
{
    // The sad class - where money says goodbye
    // Like watching your friends leave after a party

    // Constructor: documenting financial regrets since 2025
    public Expense(string description, string category, decimal amount, DateTime date)
        : base(description, category, amount, date) { } // Passing the painful numbers up to the parent class
}

