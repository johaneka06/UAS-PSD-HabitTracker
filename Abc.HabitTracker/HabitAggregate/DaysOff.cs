using System;

namespace Abc.HabitTracker
{
    public class DaysOff
    {
        private string[] _offDay;

        public string[] Days_Off
        {
            get
            {
                return this._offDay;
            }
        }

        public DaysOff()
        {
            this._offDay = new string[7];
        }

        public DaysOff(string[] offDay)
        {
            if(checkRequirement(offDay)) this._offDay = offDay;
        }

        private bool checkRequirement(string[] offDay)
        {
            if (offDay.Length == 7) throw new Exception("Days must be less than 7!");
            else
            {
                for(int i = 0; i < offDay.Length; i++)
                {
                    string day = offDay[i];

                    if (!day.Equals("Mon") && !day.Equals("Tue") && !day.Equals("Wed") && !day.Equals("Thu") && !day.Equals("Fri") && !day.Equals("Sat") && !day.Equals("Sun"))
                        throw new Exception("Days must be Mon, Tue, Wed, Thu, Fri, Sat, or Sun!");

                    for (int j = i+1; j < offDay.Length; j++)
                    {
                        if (day.Equals(offDay[j])) throw new Exception("Day cannot be same!");
                    }
                }
            }

            return true;
        }

        public DaysOff UpdateDays(string[] newDays)
        {
            return new DaysOff(newDays);
        }

        public override bool Equals(object obj)
        {
            var o = obj as DaysOff;
            if(o == null) return false;

            return o._offDay == this._offDay;
        }
        
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}