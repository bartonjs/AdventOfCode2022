using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day15
    {
        [GeneratedRegex(@"Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)")]
        private static partial Regex MatchRegex();

        private static (List<(int, int, int)> sensors, HashSet<(int, int)> beacons) LoadGrid()
        {
            Regex matcher = MatchRegex();
            List<(int, int, int)> sensors = new List<(int, int, int)>();
            HashSet<(int, int)> beacons = new HashSet<(int, int)>();

            foreach (string s in Data.Enumerate())
            {
                Match m = matcher.Match(s);

                int sensorX = int.Parse(m.Groups[1].ValueSpan);
                int sensorY = int.Parse(m.Groups[2].ValueSpan);
                int beaconX = int.Parse(m.Groups[3].ValueSpan);
                int beaconY = int.Parse(m.Groups[4].ValueSpan);

                int sensorDistance = Math.Abs(sensorX - beaconX) + Math.Abs(sensorY - beaconY);

                sensors.Add((sensorX, sensorY, sensorDistance));
                beacons.Add((beaconX, beaconY));
            }

            return (sensors, beacons);
        }

        internal static void Problem1()
        {
            HashSet<int> points = new HashSet<int>();
            (List<(int, int, int)> sensors, HashSet<(int, int)> beacons) = LoadGrid();

#if SAMPLE
            const int TargetY = 10;
#else
            const int TargetY = 2000000;
#endif

            foreach (var sensor in sensors)
            {
                for (int i = 0; i < sensor.Item3; i++)
                {
                    int yPlus = sensor.Item2 + i;
                    int yMinus = sensor.Item2 - i;

                    if (yPlus == TargetY || yMinus == TargetY)
                    {
                        int delta = sensor.Item3 - i;

                        for (int j = 0; j <= delta; j++)
                        {
                            points.Add(sensor.Item1 + j);
                            points.Add(sensor.Item1 - j);
                        }
                    }
                }
            }

            foreach (var beacon in beacons)
            {
                if (beacon.Item2 == TargetY)
                {
                    points.Remove(beacon.Item1);
                }
            }

            Console.WriteLine(points.Count);
#if SAMPLE
            Console.WriteLine(string.Join(", ", points.OrderBy(x => x)));
#endif
        }

        internal static void Problem2()
        {
            const int Scale = 4000000;

            (List<(int, int, int)> sensors, HashSet<(int, int)> beacons) = LoadGrid();

            for (int outer = 0; outer < sensors.Count; outer++)
            {
                (int, int, int) reference = sensors[outer];

                int x = reference.Item1;
                int y = reference.Item2 - reference.Item3 - 1;

                while (y < reference.Item2)
                {
                    if (Check(reference, x, y, sensors))
                    {
                        return;
                    }

                    x++;
                    y++;
                }


                while (x > reference.Item1)
                {
                    if (Check(reference, x, y, sensors))
                    {
                        return;
                    }

                    x--;
                    y++;
                }

                while (y > reference.Item2)
                {
                    if (Check(reference, x, y, sensors))
                    {
                        return;
                    }

                    x--;
                    y--;
                }

                while (x < reference.Item1)
                {
                    if (Check(reference, x, y, sensors))
                    {
                        return;
                    }

                    x++;
                    y--;
                }

                Debug.Assert(x == reference.Item1);
            }

            static bool Check((int, int, int) sensor, int x, int y, List<(int, int, int)> sensors)
            {
#if SAMPLE
                const int Size = 20;
#else
                const int Size = 4000000;
#endif
                if (x >= 0 && x <= Size && y >= 0 && y <= Size)
                {
                    if (!sensors.Any(s => Touches(s, x, y)))
                    {
                        Console.WriteLine(
                            $"Empty spot at ({x}, {y})  => {(long)x * Scale + y}");
                        return true;
                    }
                }

                return false;
            }

            static bool Touches((int, int, int) sensor, int x, int y)
            {
                int distance = Math.Abs(sensor.Item1 - x) + Math.Abs(sensor.Item2 - y);
                return distance <= sensor.Item3;
            }
        }

        internal static void Problem2BruteForce()
        {
#if SAMPLE
            const int Size = 20;
#else
            const int Size = 4000000;
#endif
            const int Scale = 4000000;

            (List<(int, int, int)> sensors, HashSet<(int, int)> beacons) = LoadGrid();
            List<Thread> threads = new List<Thread>();

            int sliceSize = Size / Math.Max(1, Environment.ProcessorCount - 1);

            for (int cpu = 0; cpu < Environment.ProcessorCount; cpu++)
            {
                Thread t = new Thread(
                    id =>
                    {
                        int tid = (int)id;
                        int start = sliceSize * tid;
                        int stop = Math.Min(start + sliceSize, Size);
                        bool[] row = new bool[Size + 1];

                        Console.WriteLine($"Thread {tid} scanning {start}-{stop}");

                        for (int targetY = start; targetY <= stop; targetY++)
                        {
                            Array.Clear(row);

                            foreach (var sensor in sensors)
                            {
                                int yDiff = Math.Abs(sensor.Item2 - targetY);

                                int delta = sensor.Item3 - yDiff;

                                if (delta >= 0)
                                {
                                    int xMin = Math.Max(sensor.Item1 - delta, 0);
                                    int xMax = Math.Min(sensor.Item1 + delta, Size);
                                    row.AsSpan(xMin, xMax - xMin + 1).Fill(true);
                                }
                            }

                            int firstFalse = row.AsSpan().IndexOf(false);

                            if (firstFalse >= 0)
                            {
                                Console.WriteLine(
                                    $"Empty spot at ({firstFalse}, {targetY}) => {(long)firstFalse * Scale + targetY}");
                            }

                            if ((targetY % 10000) == 0)
                            {
                                Console.WriteLine($"{tid}:{targetY}");
                            }

                        }

                        Console.WriteLine($"Thread {tid} is done.");
                    });

                t.Start(cpu);
                threads.Add(t);
            }

            foreach (Thread t in threads)
            {
                t.Join();
            }
        }
    }
}