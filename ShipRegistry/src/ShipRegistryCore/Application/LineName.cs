using System;

namespace ShipRegistryCore.Application
{
    public class LineName : IEquatable<LineName>
    {
        private readonly string _name;

        public LineName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "A line must have a non-null or empty name");
             _name = name;
        }

        public override string ToString()
        {
            return _name;
        }

        public bool Equals(LineName other)
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
            return Equals((LineName) obj);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }

        public static bool operator ==(LineName left, LineName right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LineName left, LineName right)
        {
            return !Equals(left, right);
        }
    }
}