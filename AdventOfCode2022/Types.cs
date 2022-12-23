using System;
using System.Collections.Generic;

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

        public Point North()
        {
            return new Point(X, Y - 1);
        }

        public Point South()
        {
            return new Point(X, Y + 1);
        }

        public Point West()
        {
            return new Point(X - 1, Y);
        }

        public Point East()
        {
            return new Point(X + 1, Y);
        }
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

    internal abstract class Plane<T>
    {
        internal abstract ref T this[Point point] { get; }
        internal abstract bool ContainsPoint(Point point);
        internal abstract IEnumerable<Point> AllPoints();

        internal bool TryGetValue(Point point, out T value)
        {
            if (ContainsPoint(point))
            {
                value = this[point];
                return true;
            }

            value = default;
            return false;
        }
    }

    internal sealed class FixedPlane<T> : Plane<T>
    {
        private readonly int _width;
        private readonly int _height;
        private readonly T[,] _data;

        internal FixedPlane(int width, int height)
        {
            _data = new T[width, height];
            _width = width;
            _height = height;
        }

        internal override ref T this[Point point] => ref _data[point.X, point.Y];

        internal override bool ContainsPoint(Point point)
        {
            return point.X >= 0 && point.X < _width && point.Y >= 0 && point.Y < _height;
        }

        internal override IEnumerable<Point> AllPoints()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    yield return new Point(x, y);
                }
            }
        }
    }

    internal sealed class DynamicPlane<T> : Plane<T>
    {
        private readonly int _width;
        private readonly List<T[]> _data;

        internal DynamicPlane(int width, int heightHint = 0)
        {
            _data = new List<T[]>(heightHint);
            _width = width;
        }

        internal DynamicPlane(T[] row0, int heightHint = 0)
        {
            _data = new List<T[]>(heightHint);
            _width = row0.Length;

            _data.Add(row0);
        }

        internal override ref T this[Point point] => ref _data[point.Y][point.X];

        internal override bool ContainsPoint(Point point)
        {
            return point.X >= 0 && point.X < _width && point.Y >= 0 && point.Y < _data.Count;
        }

        internal override IEnumerable<Point> AllPoints()
        {
            int height = Height;

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    yield return new Point(x, y);
                }
            }
        }

        internal int Height => _data.Count;

        internal T[] PushY()
        {
            T[] row = new T[_width];
            _data.Add(row);

            return row;
        }

        internal void PushY(T[] row)
        {
            if (row.Length != _width)
            {
                throw new ArgumentException("Bad width");
            }

            _data.Add(row);
        }
    }
}