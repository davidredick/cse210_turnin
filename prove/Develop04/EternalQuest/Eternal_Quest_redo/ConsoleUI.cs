using System;

public class ConsoleUI : IUserInterface
{
    public void Display(string message) => Console.WriteLine(message);
    public string GetInput() => Console.ReadLine();
}
