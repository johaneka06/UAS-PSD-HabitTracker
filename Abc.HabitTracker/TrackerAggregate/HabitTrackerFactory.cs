using System;

namespace Abc.HabitTracker
{
    public class HabitTrackerFactory
    {
        public static AbcTracker Create(Guid habitId, Guid user_id, IBadgeRepository repo = null, IHabitRepository habitRepo = null)
        {
            AbcTracker habitTracker;

            habitTracker = new HabitTracker(habitId, user_id, repo, habitRepo);

            HabitStreakHandler dominating = new DominatingHandler(repo, new DominatingBadge());
            HabitStreakHandler workaholic = new WorkaholicHandler(repo, new WorkaholicBadge());
            HabitStreakHandler ec = new ECHandler(repo, new ECBadge());

            habitTracker.Attach(dominating);
            habitTracker.Attach(workaholic);
            habitTracker.Attach(ec);

            return habitTracker;
        }
    }
}