using System;
using System.Collections.Generic;

namespace Abc.HabitTracker
{
    public interface IBadgeRepository
    {
        List<Badge> GetBadges(Guid userId);
        void GiveBadge(Guid userId, Badge badge);
    }
}