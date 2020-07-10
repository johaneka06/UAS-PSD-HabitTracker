using System;

namespace Abc.HabitTracker
{
    public class DominatingBadge : IBadgeGainer
    {
        public Badge Gain()
        {
            return Badge.CreateBadge("Dominating", "4+ streak");
        }
    }

    public class WorkaholicBadge : IBadgeGainer
    {
        public Badge Gain()
        {
            return Badge.CreateBadge("Workaholic", "Doing some works on daysoff");
        }
    }

    public class ECBadge : IBadgeGainer
    {
        public Badge Gain()
        {
            return Badge.CreateBadge("Epic Comeback", "10 streak after 10 days without logging");
        }
    }
}