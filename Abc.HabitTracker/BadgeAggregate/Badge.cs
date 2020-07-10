using System;

namespace Abc.HabitTracker
{
    //Entity Badge
    public class Badge
    {
        private Guid _badgeId;
        private BadgeName _name;
        private Guid _userId;
        private string _desc;
        private DateTime _received;

        public Guid BadgeID
        {
            get
            {
                return this._badgeId;
            }
        }

        public string BadgeName
        {
            get
            {
                return this._name.Name;
            }
        }

        public string BadgeDesc
        {
            get
            {
                return this._desc;
            }
        }

        public Guid UserID
        {
            get
            {
                return this._userId;
            }
        }

        public DateTime ReceivedAt
        {
            get
            {
                return this._received;
            }
        }

        public Badge()
        {
            this._name = null;
            this._desc = "";
            this._received = new DateTime();
        }

        public Badge(Guid id, BadgeName name, string desc, Guid userId, DateTime received)
        {
            this._badgeId = id;
            this._name = name;
            this._desc = desc;
            this._userId = userId;
            this._received = received;
        }

        public Badge(Guid id, BadgeName name, string desc, DateTime received)
        {
            this._badgeId = id;
            this._name = name;
            this._desc = desc;
            this._received = received;
        }

        public static Badge CreateBadge(string name, string desc, Guid userId, DateTime received)
        {
            BadgeName bn = new BadgeName(name);
            return new Badge(Guid.NewGuid(), bn, desc, userId, received);
        }

        public static Badge CreateBadge(string name, string desc)
        {
            BadgeName bn = new BadgeName(name);
            return new Badge(Guid.NewGuid(), bn, desc, DateTime.Now);
        }

        public void ChangeBadgeName(string name)
        {
            this._name = this._name.ChangeName(name);
        }

        public override bool Equals(object obj)
        {
            var b = obj as Badge;
            if(b == null) return false;

            return b._badgeId.Equals(this._badgeId);
        }
        
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}