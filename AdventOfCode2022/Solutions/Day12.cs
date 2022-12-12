using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day12
    {
        private static (int[][], (int,int), (int,int)) LoadData()
        {
            List<int[]> grid = new List<int[]>();
            (int, int) start = default;
            (int, int) end = default;

            foreach (string line in Data.Enumerate())
            {
                int startIdx = line.IndexOf('S');
                int endIdx = line.IndexOf('E');
                int[] row = line.Select(c => c - 'a').ToArray();

                if (startIdx >= 0)
                {
                    row[startIdx] = 0;
                    start = (grid.Count, startIdx);
                }

                if (endIdx >= 0)
                {
                    row[endIdx] = 25;
                    end = (grid.Count, endIdx);
                }

                grid.Add(row);
            }

            return (grid.ToArray(), start, end);
        }

        [GeneratedRegex(@"^(.) (\d+)")]
        private static partial Regex Regex();

        internal static void Problem1()
        {
            (int[][] grid, (int, int) start, (int, int) end) = LoadData();

            Console.WriteLine(FindCheapestPath(grid, start, end));
        }

        internal static void Problem2()
        {
            (int[][] grid, (int, int) start, (int, int) end) = LoadData();
            int lowest = int.MaxValue;

            for (int row = 0; row < grid.Length; row++)
            {
                int[] rowData = grid[row];

                for (int col = 0; col < rowData.Length; col++)
                {
                    if (rowData[col] == 0)
                    {
                        lowest = Math.Min(lowest, FindCheapestPath(grid, (row, col), end));
                    }
                }
            }

            Console.WriteLine(lowest);
        }

        private static int FindCheapestPath(int[][] grid, (int,int) start, (int,int) end)
        {
            int colMax = grid[ 0 ].Length;

            HashSet<(int, int)> openSet = new HashSet<(int, int)> { start };
            Dictionary<(int, int), int> gScore = new Dictionary<(int, int), int>();

#if SAMPLE
            Dictionary<(int, int), (int, int)> cameFrom = new Dictionary<(int, int), (int, int)>();
#endif

            gScore[start] = 0;

            while (openSet.Count > 0)
            {
                var current = openSet.First();
                openSet.Remove(current);

                if (current == end)
                {
                    continue;
                }

                int curHeight = grid[current.Item1][current.Item2];

                for (int deltaRow = -1; deltaRow < 2; deltaRow++)
                {
                    for (int deltaCol = -1; deltaCol < 2; deltaCol++)
                    {
                        if (Math.Abs(deltaRow) + Math.Abs(deltaCol) != 1)
                        {
                            continue;
                        }

                        int row = current.Item1 + deltaRow;
                        int col = current.Item2 + deltaCol;

                        if (row < 0 || row >= grid.Length || col < 0 || col >= colMax)
                        {
                            continue;
                        }

                        int testHeight = grid[row][col];
                        (int, int) neighbor = (row, col);

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

                (int, int) cur = end;

                while (cur != start)
                {
                    (int, int) from = cameFrom[cur];

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