using System;
using System.Collections.Generic;

using Abc.HabitTracker.Database.Postgresql;

namespace Abc.HabitTracker
{
    public abstract class AbcTracker : IObservable<StreakResult>
    {
        private Guid _id;
        protected IHabitRepository habitRepository;
        protected IBadgeRepository badgeRepository;

        public Guid ID
        {
            get
            {
                return this._id;
            }
        }

        public AbcTracker()
        {
            _id = Guid.NewGuid();
        }

        public abstract Habit Log(Log log);
        
        protected List<IObserver<StreakResult>> _observer = new List<IObserver<StreakResult>>();
        public void Attach(IObserver<StreakResult> obs)
        {
            _observer.Add(obs);
        }

        public void Broadcast(StreakResult res)
        {
            foreach(var obs in _observer)
            {
                obs.Update(res);
            }
        }

        public override bool Equals(object obj)
        {
            var tracker = obj as AbcTracker;   
            if(tracker == null) return false;
            
            return this._id == tracker._id;
        }
        
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public abstract class Log
    {
        protected Guid user;
        protected Guid habit;

        public Guid UserID
        {
            get
            {
                return this.user;
            }
        }

        public Guid HabitID
        {
            get
            {
                return this.habit;
            }
        }

        public Log(Guid userId, Guid habitId)
        {
            this.user = userId;
            this.habit = habitId;
        }

        public Log()
        {
            this.user = new Guid();
            this.habit = new Guid();
        }
    }
}