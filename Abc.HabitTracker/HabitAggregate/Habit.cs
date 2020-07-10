using System;

namespace Abc.HabitTracker
{
    //Entity Habit
    public class Habit
    {
        private Guid _habitId;
        private HabitName _name;
        private DaysOff _off;
        private int _currentStreak;
        private int _longestStreak;
        private int _logCount;
        private Guid _userId;
        private DateTime[] _logs;
        private DateTime _createdAt;

        public Guid HabitId
        {
            get
            {
                return this._habitId;
            }
        }

        public string HabitName
        {
            get
            {
                return this._name.Name;
            }
        }

        public string[] DaysOff
        {
            get
            {
                return this._off.Days_Off;
            }
        }

        public int CurrentStreak
        {
            get
            {
                return this._currentStreak;
            }
        }

        public int LongestStreak
        {
            get
            {
                return this._longestStreak;
            }
        }

        public int LogCount
        {
            get
            {
                return this._logCount;
            }
        }

        public Guid UserID
        {
            get
            {
                return this._userId;
            }
        }

        public DateTime[] Logs
        {
            get
            {
                return this._logs;
            }
        }

        public DateTime CreatedAt
        {
            get
            {
                return this._createdAt;
            }
        }

        public Habit(Guid id, Guid userId, HabitName name, DaysOff off, DateTime createdAt)
        {
            this._habitId = id;
            this._userId = userId;
            this._name = name;
            this._off = off;
            this._createdAt = createdAt;
        }

        public Habit(Guid id, Guid userId, HabitName name, DaysOff off, int currentStreak, int longestStreak, int logCount, DateTime[] logs, DateTime createdAt)
        {
            this._habitId = id;
            this._userId = userId;
            this._name = name;
            this._off = off;
            this._currentStreak = currentStreak;
            this._longestStreak = longestStreak;
            this._logCount = logCount;
            this._logs = logs;
            this._createdAt = createdAt;
        }

        public Habit() { }

        public static Habit CreateHabit(Guid user, string name, string[] off)
        {
            return new Habit(Guid.NewGuid(), user, new HabitName(name), new DaysOff(off), DateTime.Now);
        }

        public void ChangeHabitName(Guid id, Guid userId, string name)
        {
            this._name = this._name.ChangeHabitName(name);
        }

        public void ChangeDaysOff(Guid id, Guid userId, string[] off)
        {
            this._off = this._off.UpdateDays(off);
        }

        public override bool Equals(object obj)
        {
           var o = obj as Habit;
           if(o == null) return false;

           return o._habitId.Equals(this._habitId); 
           
        }
        
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}