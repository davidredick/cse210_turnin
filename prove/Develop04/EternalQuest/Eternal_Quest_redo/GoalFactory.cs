using System;

public static class GoalFactory
{
    public static Goal CreateGoal(string type, string name, int points, int required = 0, int bonus = 0)
    {
        return type switch
        {
            "Simple" => new SimpleGoal(name, points),
            "Eternal" => new EternalGoal(name, points),
            "Checklist" => new ChecklistGoal(name, points, required, bonus),
            _ => throw new ArgumentException("Unknown goal type"),
        };
    }
}
