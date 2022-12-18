using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day12
    {
        private static (int[][], Point, Point) LoadData()
        {
            List<int[]> grid = new List<int[]>();
            Point start = default;
            Point end = default;

            foreach (string line in Data.Enumerate())
            {
                int startIdx = line.IndexOf('S');
                int endIdx = line.IndexOf('E');
                int[] row = line.Select(c => c - 'a').ToArray();

                if (startIdx >= 0)
                {
                    row[startIdx] = 0;
                    start = new Point(grid.Count, startIdx);
                }

                if (endIdx >= 0)
                {
                    row[endIdx] = 25;
                    end = new Point(grid.Count, endIdx);
                }

                grid.Add(row);
            }

            return (grid.ToArray(), start, end);
        }

        [GeneratedRegex(@"^(.) (\d+)")]
        private static partial Regex Regex();

        internal static void Problem1()
        {
            (int[][] grid, Point start, Point end) = LoadData();

            Console.WriteLine(FindCheapestPath(grid, start, end));
        }

        internal static void Problem2()
        {
            (int[][] grid, Point start, Point end) = LoadData();
            int lowest = int.MaxValue;

            for (int x = 0; x < grid.Length; x++)
            {
                int[] colData = grid[x];

                for (int y = 0; y < colData.Length; y++)
                {
                    if (colData[y] == 0)
                    {
                        lowest = Math.Min(lowest, FindCheapestPath(grid, new Point(x, y), end));
                    }
                }
            }

            Console.WriteLine(lowest);
        }

        private static int FindCheapestPath(int[][] grid, Point start, Point end)
        {
            int rowMax = grid[0].Length;

            HashSet<Point> openSet = new HashSet<Point> { start };
            Dictionary<Point, int> gScore = new Dictionary<Point, int>();

#if SAMPLE
            Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();
#endif

            gScore[start] = 0;

            while (openSet.Count > 0)
            {
                Point current = openSet.First();
                openSet.Remove(current);

                if (current == end)
                {
                    continue;
                }

                int curHeight = grid[current.X][current.Y];

                for (int deltaX = -1; deltaX < 2; deltaX++)
                {
                    for (int deltaY = -1; deltaY < 2; deltaY++)
                    {
                        if (Math.Abs(deltaX) + Math.Abs(deltaY) != 1)
                        {
                            continue;
                        }

                        int x = current.X + deltaX;
                        int y = current.Y + deltaY;

                        if (x < 0 || x >= grid.Length || y < 0 || y >= rowMax)
                        {
                            continue;
                        }

                        int testHeight = grid[x][y];
                        Point neighbor = new Point(x, y);

                        if (testHeight - curHeight <= 1)
                        {
                            int score = gScore[current] + 1;

                            ref int neighborCost =
                                ref CollectionsMarshal.GetValueRefOrAddDefault(gScore, neighbor, out bool exists);

                            if (!exists || score < neighborCost)
                            {
                                neighborCost = score;
#if SAMPLE
                                cameFrom[neighbor] = current;
#endif

                                openSet.Add(neighbor);
                            }
                        }
                    }
                }
            }

            if (gScore.TryGetValue(end, out int endScore))
            {
#if SAMPLE
                Console.WriteLine("===");

                Point cur = end;

                while (cur != start)
                {
                    Point from = cameFrom[cur];

                    Console.WriteLine($" {cur} from {from}");
                    cur = from;
                }

                Console.WriteLine($"Path length: {endScore}");
#endif

                return endScore;
            }
#if SAMPLE
            else
            {
                Console.WriteLine("... No solution.");
            }
#endif

            return int.MaxValue;
        }
    }
}