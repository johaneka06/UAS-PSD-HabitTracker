using System;
using System.Collections.Generic;

namespace Abc.HabitTracker
{
    public class HabitLog : Log
    {
        public HabitLog() : base() { }
        public HabitLog(Guid userId, Guid habitId) : base (userId, habitId) { }
    }
    public class HabitTracker : AbcTracker
    {
        private Guid _userId;
        private Guid _habitId;
        private IHabitRepository _habit;
        private IBadgeRepository _badge;
        private Habit lastHabit;

        public HabitTracker(Guid habitId, Guid userId, IBadgeRepository badgeRepository, IHabitRepository habitRepository)
        {
            _userId = userId;
            _habitId = habitId;
            _habit = habitRepository;
            _badge = badgeRepository;
        }

        public override Habit Log(Log log)
        {
            HabitLog l = log as HabitLog;

            lastHabit = _habit.GetHabit(log.UserID, log.HabitID);
            
            if(lastHabit == null || lastHabit.HabitName == "") throw new Exception("No Data Found!");

            int longest = lastHabit.LongestStreak;
            int current = lastHabit.CurrentStreak;
            int count = lastHabit.LogCount;
            string[] offDay = lastHabit.DaysOff;
            DateTime today = DateTime.Now;

            Habit newestData;

            List<DateTime> list = new List<DateTime>(lastHabit.Logs);
            list.Add(today);
            DateTime[] currentLog = list.ToArray();

            if(lastHabit.Logs.Length != 0)
            {
                /*
                Streak destroyer: Logic to check if user hasn't logged anything for morethan one days, 
                then it will check through the gap day (last log day until yesterday) whether it is user's days off or not.
                */
                if((today - lastHabit.Logs[lastHabit.Logs.Length-1]).TotalDays != 1)
                {
                    bool flag = false;
                    for(DateTime day = lastHabit.Logs[lastHabit.Logs.Length-1]; day < today; day = day.AddDays(1))
                    {
                        for(int i = 0; i < offDay.Length; i++)
                        {
                            if(!day.DayOfWeek.ToString().Substring(0, 3).Equals(offDay[i]))
                            {
                                flag = true;
                                current = 0;
                                break;
                            }
                        }
                        if(flag) break;
                    }
                }
                /*
                Streak gainer: Logic to check if user logged anydata every day.
                */
                else if((today - lastHabit.Logs[lastHabit.Logs.Length - 1]).TotalDays == 1) current++;
                
                if(current > longest) longest = current;

                newestData = new Habit(lastHabit.HabitId, lastHabit.UserID, new HabitName(lastHabit.HabitName), new DaysOff(lastHabit.DaysOff), current,
                    longest, count++, currentLog, lastHabit.CreatedAt);
            }
            else 
            {
                longest = current;
                newestData = new Habit(lastHabit.HabitId, lastHabit.UserID, new HabitName(lastHabit.HabitName), new DaysOff(lastHabit.DaysOff), current,
                    longest, count++, currentLog, lastHabit.CreatedAt);
            }

            if(lastHabit.Logs.Length >= 4)
            {
                if(isDominating()) Broadcast(new DominatingStreak(_userId, _habitId));
                else if(isWorkaholic()) Broadcast(new WorkaholicStreak(_userId, _habitId));
                else if(isEpicComeback()) Broadcast(new ECStreak(_userId, _habitId));
            }

            return newestData;
            
        }

        private bool isDominating()
        {
            Habit currentHabit = _habit.GetHabit(_userId, _habitId);

            if(currentHabit.CurrentStreak < 4) return false;

            foreach(Badge badge in _badge.GetBadges(_userId))
            {
                if(badge.BadgeName.Equals("Dominating")) return false;
            }
            return true;
        }

        private bool isWorkaholic()
        {
            int count = 0;
            List<Habit> habitList = _habit.GetAllHabit(_userId);
            foreach(Habit h in habitList)
            {
                foreach(DateTime date in h.Logs)
                {
                    foreach(string day in h.DaysOff)
                    {
                        if(date.TimeOfDay.ToString().Substring(0, 3).Equals(day)) count++;
                    }
                }
            }

            if(count < 10) return false;

            foreach(Badge badge in _badge.GetBadges(_userId))
            {
                if(badge.BadgeName.Equals("Workaholic")) return false;
            }

            return true;
        }

        private bool isEpicComeback()
        {
            int idx = 0; //Temp field for starting 10 days count
            Habit habit = _habit.GetHabit(_userId, _habitId);

            for(int i = 0; i < habit.Logs.Length-1; i++)
            {
                if((habit.Logs[i+1].Date - habit.Logs[i].Date).TotalDays >= 10)
                {
                    idx = i;
                }
            }
            int count = 0;
            for(int i = idx; i < habit.Logs.Length - 1; i++)
            {
                if((habit.Logs[i+1].Date - habit.Logs[i].Date).TotalDays == 1) count++;
                if(count == 10) break;
            }

            if(count != 10) return false;

            foreach(Badge badge in _badge.GetBadges(_userId))
            {
                if(badge.BadgeName.Equals("Epic Comeback")) return false;
            }
            return true;
        }
    }
}