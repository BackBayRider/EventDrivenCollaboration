using System;

namespace ShipRegistryCore.Application
{
    public class Capacity : IEquatable<Capacity>
    {
        public Capacity(int capacity)
        {
            Value = capacity;
        }
        
        public int Value { get; }

        public bool Equals(Capacity other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Capacity) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(Capacity left, Capacity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Capacity left, Capacity right)
        {
            return !Equals(left, right);
        }
    }
}