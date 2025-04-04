using System;

public interface IGoal
{
    void RecordEvent();
    bool IsComplete();
    string GetStatus();
    string Serialize();
}

public abstract class Goal : IGoal
{
    protected string _name;
    public int _points;

    protected Goal(string name, int points)
    {
        _name = name;
        _points = points;
    }

    public abstract void RecordEvent();
    public abstract bool IsComplete();
    public abstract string GetStatus();
    public abstract string Serialize();
}

public class SimpleGoal : Goal
{
    private bool _completed;

    public SimpleGoal(string name, int points) : base(name, points) => _completed = false;

    public override void RecordEvent() => _completed = true;

    public override bool IsComplete() => _completed;

    public override string GetStatus() => _completed ? "[X]" : "[ ]";

    public override string Serialize() => $"SimpleGoal:{_name},{_points},{_completed}";
}
