using System;
using System.Collections.Generic;
using System.IO;

public static class GoalPersistence
{
    public static void Save(IEnumerable<Goal> goals, string filename)
    {
        using StreamWriter writer = new(filename);
        foreach (var goal in goals)
            writer.WriteLine(goal.Serialize());
    }

    public static List<Goal> Load(string filename)
    {
        var goals = new List<Goal>();
        foreach (var line in File.ReadAllLines(filename))
        {
            var parts = line.Split(':');
            var details = parts[1].Split(',');

            goals.Add(parts[0] switch
            {
                "SimpleGoal" => new SimpleGoal(details[0], int.Parse(details[1])),
                "EternalGoal" => new EternalGoal(details[0], int.Parse(details[1])),
                "ChecklistGoal" => new ChecklistGoal(details[0], int.Parse(details[1]), int.Parse(details[3]), int.Parse(details[4])),
                _ => throw new Exception("Unknown goal type"),
            });
    }
        return goals;
    }
}