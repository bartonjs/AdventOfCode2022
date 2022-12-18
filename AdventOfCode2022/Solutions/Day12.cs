using System;
using System.Collections.Generic;
using System.Linq;
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
            return Utils.AStar(
                grid,
                start,
                end,
                Neighbors,
                (candidate, end, world) => candidate.ManhattanDistance(end));

            static IEnumerable<(Point Neighbor, int Cost)> Neighbors(Point from, int[][] world)
            {
                int curHeight = world[from.X][from.Y];

                if (from.X > 0 && world[from.X - 1][from.Y] - curHeight <= 1)
                {
                    yield return (new Point(from.X - 1, from.Y), 1);
                }

                if (from.X < world.Length - 1 && world[from.X + 1][from.Y] - curHeight <= 1)
                {
                    yield return (new Point(from.X + 1, from.Y), 1);
                }

                if (from.Y > 0 && world[from.X][from.Y - 1] - curHeight <= 1)
                {
                    yield return (new Point(from.X, from.Y - 1), 1);
                }

                if (from.Y < world[from.X].Length - 1 && world[from.X][from.Y + 1] - curHeight <= 1)
                {
                    yield return (new Point(from.X, from.Y + 1), 1);
                }
            }
        }
    }
}