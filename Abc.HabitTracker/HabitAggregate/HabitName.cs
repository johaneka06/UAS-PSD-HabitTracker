using System;

namespace Abc.HabitTracker
{
    //Value object HabitName
    public class HabitName
    {
        private string _name;

        public string Name
        {
            get
            {
                return this._name;
            }
        }

        public HabitName()
        {
            this._name = "";
        }

        public HabitName(string name)
        {
            if(name.Length < 2 || name.Length > 100) throw new Exception("Name must be 2 - 100 charcaters long!");

            this._name = name;
        }

        public HabitName ChangeHabitName(string name)
        {
            if(name.Length < 2 || name.Length > 100) throw new Exception("Name must be 2 - 100 charcaters long!");

            return new HabitName(name);
        }

        public override bool Equals(object obj)
        {
            var o = obj as HabitName;
            if(o == null) return false;
            
            return o._name.Equals(this._name);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}