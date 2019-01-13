using System;

namespace ShipRegistryApplication
{
    public class ShipName : IEquatable<ShipName>
    {
        private readonly string _name;

        public ShipName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "A ship must have a non-empty name");
            
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }
        
        public bool Equals(ShipName other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_name, other._name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ShipName) obj);
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }

        public static bool operator ==(ShipName left, ShipName right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ShipName left, ShipName right)
        {
            return !Equals(left, right);
        }

   }
}