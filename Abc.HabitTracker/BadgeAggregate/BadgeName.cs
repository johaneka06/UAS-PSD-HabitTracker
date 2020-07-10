using System;

namespace Abc.HabitTracker
{
    //Value object BadgeName
    public class BadgeName
    {
        private string _name;

        public string Name
        {
            get
            {
                return this._name;
            }
        }

        public BadgeName()
        {
            this._name = "";
        }

        public BadgeName(string name)
        {
            if(!name.Equals("Dominating") && !name.Equals("Workaholic") && !name.Equals("Epic Comeback")) 
                throw new Exception("Name must be Dominating, Workaholic, or Epic Comeback!");

            this._name = name;
        }

        public BadgeName ChangeName(string name)
        {
            if(!name.Equals("Dominating") && !name.Equals("Workaholic") && !name.Equals("Epic Comeback")) 
                throw new Exception("Name must be Dominating, Workaholic, or Epic Comeback!");
            
            return new BadgeName(name);
        }

        public override bool Equals(object obj)
        {
            var o = obj as BadgeName;
            if(o == null) return false;
            
            return o._name.Equals(this._name);
        }
        
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}