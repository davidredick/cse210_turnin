using System.Collections.Generic;
using System.Linq;

public class GoalManager
{
    private readonly List<Goal> _goals = new();
    public int TotalScore { get; private set; }

    public void AddGoal(Goal goal) => _goals.Add(goal);

    public void RecordEvent(int index)
    {
        if (index < _goals.Count)
        {
            _goals[index].RecordEvent();
            TotalScore += _goals[index]._points;
        }
    }

    public IEnumerable<Goal> GetGoals() => _goals;

    public int GoalTypeCount(string type)
    {
        return _goals.FindAll(g => g.GetType().Name.StartsWith(type)).Count;
    }
}
