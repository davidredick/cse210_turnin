// Income.cs - The rare and elusive positive cash flow, often spotted near payday
public class Income : Transaction
{
    // Constructor: creating that brief moment of financial optimism
    public Income(string description, string category, decimal amount, DateTime date)
        : base(description, category, amount, date) { } // Passing the happy numbers up to the parent class
}
