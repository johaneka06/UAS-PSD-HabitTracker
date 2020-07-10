using System;

namespace Abc.HabitTracker
{
    public abstract class HabitStreakHandler : IObserver<StreakResult>
    {
        protected IBadgeRepository badgeRepository;
        protected IBadgeGainer _gainer;

        public HabitStreakHandler(IBadgeRepository badgeRepo = null, IBadgeGainer gainer = null)
        {
            this.badgeRepository = badgeRepo;
            this._gainer = gainer;
        }

        public abstract void Update(StreakResult e);
    }

    public class DominatingHandler : HabitStreakHandler
    {
        public DominatingHandler(IBadgeRepository repo = null, IBadgeGainer gainer = null) : base (repo, gainer) { }

        public override void Update(StreakResult e)
        {
            if(badgeRepository == null) return;

            DominatingStreak bdg = e as DominatingStreak;
            if(bdg == null) return;

            badgeRepository.GiveBadge(bdg.UserId, _gainer.Gain());
        }
    }

    public class WorkaholicHandler : HabitStreakHandler
    {
        public WorkaholicHandler(IBadgeRepository repo = null, IBadgeGainer gainer = null) : base (repo, gainer) { }

        public override void Update(StreakResult e)
        {
            if(badgeRepository == null) return;

            WorkaholicStreak bdg = e as WorkaholicStreak;
            if(bdg == null) return;

            badgeRepository.GiveBadge(bdg.UserId, _gainer.Gain());
        }
    }

    public class ECHandler : HabitStreakHandler
    {
        public ECHandler(IBadgeRepository repo = null, IBadgeGainer gainer = null) : base (repo, gainer) { }

        public override void Update(StreakResult e)
        {
            if(badgeRepository == null) return;

            ECStreak bdg = e as ECStreak;
            if(bdg == null) return;

            badgeRepository.GiveBadge(bdg.UserId, _gainer.Gain());
        }
    }
}