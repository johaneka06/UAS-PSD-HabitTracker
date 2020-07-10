using System;

namespace Abc.HabitTracker
{
    public abstract class StreakResult
    {
        public Guid UserId {get; private set;}
        public Guid HabitId {get; private set;}

        public StreakResult(Guid userId, Guid habitId)
        {
            this.UserId = userId;
            this.HabitId = habitId;
        }
    }

    public class DominatingStreak : StreakResult
    {
        public DominatingStreak(Guid userId, Guid habitId) : base (userId, habitId) { }
    }

    public class WorkaholicStreak : StreakResult
    {
        public WorkaholicStreak(Guid userId, Guid habitId) : base (userId, habitId) { }
    }

    public class ECStreak : StreakResult
    {
        public ECStreak(Guid userId, Guid habitId) : base (userId, habitId) { }
    }
}