using System;
using System.Collections.Generic;
using System.IO;

// Base Goal Class
public abstract class Goal
{
    protected string _name;
    protected int _points;

    public Goal(string name, int points)
    {
        _name = name; // Because calling it "thingToDo" sounded less epic.
        _points = points; // Yes, points are made-up. But aren't they always?
    }

    public abstract void RecordEvent(); // Like your mom writing your achievements on the fridge.
    public abstract bool IsComplete(); // Spoiler alert: Eternal goals never finish. Shocker.
    public abstract string GetStatus(); // The "Are we there yet?" of methods.
    public abstract string Serialize(); // Because your goals need to live forever in a boring text file.
}

// Simple Goal Class
public class SimpleGoal : Goal
{
    private bool _completed;

    public SimpleGoal(string name, int points) : base(name, points)
    {
        _completed = false; // Starting with dreams unfulfilled, classic.
    }

    public override void RecordEvent()
    {
        _completed = true; // Done! Never to speak of it again.
    }

    public override bool IsComplete()
    {
        return _completed; // Basically just checking if you've peaked yet.
    }

    public override string GetStatus()
    {
        return _completed ? "[X]" : "[ ]"; // The mighty checkbox of destiny.
    }

    public override string Serialize()
    {
        return $"SimpleGoal:{_name},{_points},{_completed}"; // Dear diary, today I achieved greatness.
    }
}

// Eternal Goal Class
public class EternalGoal : Goal
{
    private int _count;

    public EternalGoal(string name, int points) : base(name, points)
    {
        _count = 0; // Zero. The number of times you thought you'd actually keep this going.
    }

    public override void RecordEvent()
    {
        _count++; // You're really milking this, aren't you?
    }

    public override bool IsComplete()
    {
        return false; // Like your laundry, never truly done.
    }

    public override string GetStatus()
    {
        return $"Completed {_count} times"; // Counting is fun, said no one ever.
    }

    public override string Serialize()
    {
        return $"EternalGoal:{_name},{_points},{_count}"; // The never-ending story (without the flying dog).
    }
}

// Checklist Goal Class
public class ChecklistGoal : Goal
{
    private int _required;
    private int _completed;
    private int _bonus;

    public ChecklistGoal(string name, int points, int required, int bonus) : base(name, points)
    {
        _required = required; // How many more? Are we there yet?
        _completed = 0; // Still at the starting line, champ.
        _bonus = bonus; // Incentives, because achieving things wasn't enough.
    }

    public override void RecordEvent()
    {
        _completed++; // Incrementally closer to becoming your best self. Maybe.
    }

    public override bool IsComplete()
    {
        return _completed >= _required; // Almost... almost... nope, keep going.
    }

    public override string GetStatus()
    {
        return $"Completed {_completed}/{_required} times"; // Just like a punch card, but less rewarding.
    }

    public override string Serialize()
    {
        return $"ChecklistGoal:{_name},{_points},{_completed},{_required},{_bonus}"; // For historical records of your procrastination.
    }

    public int GetBonus()
    {
        return _completed == _required ? _bonus : 0; // Congrats on hitting your imaginary quota.
    }
}

// Main Program Class
class Program
{
    static List<Goal> goals = new List<Goal>();
    static int totalScore = 0; // Keepin' score because life is definitely a competition.

    static void Main()
    {
        int level = 1; // Level 1: Just slightly above zero.
        int levelThreshold = 1000; // A completely arbitrary number for leveling up. Enjoy!

        goals.Add(new SimpleGoal("Run Marathon", 1000));
        goals.Add(new EternalGoal("Read Scriptures", 100));
        goals.Add(new ChecklistGoal("Temple Visit", 50, 10, 500));

        goals[1].RecordEvent();
        totalScore += goals[1]._points; // Points! Like money, but worthless.

        foreach (Goal goal in goals)
        {
            Console.WriteLine($"{goal.GetStatus()} {goal._name}"); // Displaying goals like trophies you'll never dust.
        }

        Console.WriteLine($"Total Score: {totalScore}"); // The moment you've all been waiting for: validation.

        SaveGoals("goals.txt"); // Writing your successes to a file, as if they'll be read by historians.
        LoadGoals("goals.txt"); // Re-living past glory through file loading.

        if (totalScore >= level * levelThreshold)
        {
            level++;
            Console.WriteLine($"Congratulations! You've reached Level {level}!"); // Leveling up, because real-world achievements don't come with pop-ups.
        }
    }

    static void SaveGoals(string filename)
    {
        using (StreamWriter writer = new StreamWriter(filename))
        {
            writer.WriteLine(totalScore); // Saving your imaginary bank account.
            foreach (Goal goal in goals)
            {
                writer.WriteLine(goal.Serialize()); // Goals stored safely in text, where no one can find them.
            }
        }
    }

    static void LoadGoals(string filename)
    {
        goals.Clear(); // Out with the old, in with the older.
        string[] lines = File.ReadAllLines(filename);
        totalScore = int.Parse(lines[0]); // Remember that score you cared about? It's back.

        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(':');
            string type = parts[0];
            string[] details = parts[1].Split(',');

            switch (type)
            {
                case "SimpleGoal":
                    goals.Add(new SimpleGoal(details[0], int.Parse(details[1])));
                    break;
                case "EternalGoal":
                    goals.Add(new EternalGoal(details[0], int.Parse(details[1])));
                    break;
                case "ChecklistGoal":
                    goals.Add(new ChecklistGoal(details[0], int.Parse(details[1]), int.Parse(details[3]), int.Parse(details[4])));
                    break;
            }
        }
    }
}