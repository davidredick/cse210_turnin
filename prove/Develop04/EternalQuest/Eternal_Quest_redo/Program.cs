using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        IUserInterface ui = new ConsoleUI();
        var goalManager = new GoalManager();
        var gamificationEngine = new GamificationEngine();

        bool running = true;
        while (running)
        {
            ui.Display("\nEternal Quest Main Menu:\n" +
                       "1. View Goals\n" +
                       "2. Manage Goals\n" +
                       "3. Record Goal Event\n" +
                       "4. Save Goals\n" +
                       "5. Load Goals\n" +
                       "6. Exit");
            string choice = ui.GetInput();

            switch (choice)
            {
                case "1":
                    foreach (var goal in goalManager.GetGoals())
                        ui.Display(goal.GetStatus());
                    ui.Display($"Total Score: {goalManager.TotalScore}");
                    gamificationEngine.DisplayProgressBar(goalManager.TotalScore, 1000);
                    break;
                case "2":
                    ManageGoals(ui, goalManager);
                    break;
                case "3":
                    ui.Display("Enter goal index to record:");
                    int index = int.Parse(ui.GetInput());
                    goalManager.RecordEvent(index);
                    break;
                case "4":
                    GoalPersistence.Save(goalManager.GetGoals(), "goals.txt");
                    ui.Display("Goals saved successfully.");
                    break;
                case "5":
                    goalManager.LoadGoals("goals.txt");
                    ui.Display("Goals loaded successfully.");
                    break;
                case "6":
                    ui.Display("Exiting... Thanks for playing the eternal game!");
                    return;
                default:
                    ui.Display("Invalid choice, please try again.");
                    break;
            }
        }
    }

    static void ManageGoals(IUserInterface ui, GoalManager manager)
    {
        ui.Display("\nManage Goals:\n" +
                   "1. Add Goal\n" +
                   "2. Modify Goal (Coming soon!)\n" +
                   "3. Erase Goal");
        string choice = ui.GetInput();

        switch (choice)
        {
            case "1":
                AddGoal(ui, manager);
                break;
            case "2":
                ui.Display("Modify functionality coming soon...");
                break;
            case "3":
                ui.Display("Enter index of goal to erase:");
                int eraseIndex = int.Parse(ui.GetInput());
                manager.EraseGoal(eraseIndex);
                ui.Display("Goal erased!");
                break;
            default:
                ui.Display("Invalid selection.");
                break;
        }
    }

    static void AddGoal(IUserInterface ui, GoalManager manager)
    {
        ui.Display("\nAdd Goal Type:\n" +
                   "1. Simple Goal\n" +
                   "2. Eternal Goal\n" +
                   "3. Checklist Goal");
        string typeChoice = ui.GetInput();

        string goalType = typeChoice switch
        {
            "1" => "Simple",
            "2" => "Eternal",
            "3" => "Checklist",
            _ => null
        };

        if (goalType == null)
        {
            ui.Display("Unknown goal type chosen.");
            return;
        }

        if (manager.GoalTypeCount(goalType) >= 10)
        {
            ui.Display($"Maximum limit of 10 reached for {goalType} goals.");
            return;
        }

        ui.Display("Enter goal name:");
        string name = ui.GetInput();
        ui.Display("Enter points:");
        int points = int.Parse(ui.GetInput());

        int required = 0, bonus = 0;
        if (typeChoice == "3")
        {
            ui.Display("Enter how many times it needs to be completed:");
            required = int.Parse(ui.GetInput());
            ui.Display("Enter bonus points:");
            bonus = int.Parse(ui.GetInput());
        }

        manager.AddGoal(GoalFactory.CreateGoal(goalType, name, points, required, bonus));
        ui.Display($"{goalType} goal '{name}' added!");
    }
}