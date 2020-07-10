using System;
using System.Collections.Generic;

namespace Abc.HabitTracker
{
    public interface IHabitRepository
    {
        Habit InsertNewHabit(Habit habit);
        Habit Log(Guid userId, Guid habitId, Habit newest);
        List<Habit> GetAllHabit(Guid userId);
        Habit GetHabit(Guid userId, Guid habitId);
        Habit DeleteHabit(Guid userId, Guid habit);
        Habit UpdateHabit(Guid userId, Guid habitId, string name);
        Habit UpdateHabit(Guid userId, Guid habitId, string name, string[] daysOff);
    }
}