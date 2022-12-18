using System;

namespace AdventOfCode2022
{
    internal struct Point : IEquatable<Point>
    {
        internal int X;
        internal int Y;

        internal Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        internal int ManhattanDistance(Point other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        public override string ToString() => $"({X}, {Y})";

        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Point other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(Point a, Point b) => a.Equals(b);
        public static bool operator !=(Point a, Point b) => !a.Equals(b);
    }

    internal struct LongPoint : IEquatable<LongPoint>
    {
        internal long X;
        internal long Y;

        internal LongPoint(long x, long y)
        {
            X = x;
            Y = y;
        }

        internal long ManhattanDistance(LongPoint other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        public override string ToString() => $"({X}, {Y})";

        public bool Equals(LongPoint other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is LongPoint other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(LongPoint a, LongPoint b) => a.Equals(b);
        public static bool operator !=(LongPoint a, LongPoint b) => !a.Equals(b);
    }

    internal struct Point3 : IEquatable<Point3>
    {
        internal int X;
        internal int Y;
        internal int Z;

        internal Point3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        internal int ManhattanDistance(Point3 other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
        }

        public override string ToString() => $"({X}, {Y}, {Z})";

        public bool Equals(Point3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is Point3 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public static bool operator ==(Point3 a, Point3 b) => a.Equals(b);
        public static bool operator !=(Point3 a, Point3 b) => !a.Equals(b);
    }

    internal struct LongPoint3 : IEquatable<LongPoint3>
    {
        internal long X;
        internal long Y;
        internal long Z;

        internal LongPoint3(long x, long y, long z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        internal long ManhattanDistance(LongPoint3 other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
        }

        public override string ToString() => $"({X}, {Y}, {Z})";

        public bool Equals(LongPoint3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is LongPoint3 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public static bool operator ==(LongPoint3 a, LongPoint3 b) => a.Equals(b);
        public static bool operator !=(LongPoint3 a, LongPoint3 b) => !a.Equals(b);
    }
}