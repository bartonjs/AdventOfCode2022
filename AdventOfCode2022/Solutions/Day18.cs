using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day18
    {
        private const int GridSize = 50;

        private static byte[][][] Load()
        {
            byte[][][] grid = new byte[GridSize][][];

            for (int x = 0; x < GridSize; x++)
            {
                byte[][] slice = new byte[GridSize][];
                grid[x] = slice;

                for (int y = 0; y < GridSize; y++)
                {
                    byte[] piece = new byte[GridSize];
                    slice[y] = piece;
                }
            }

            foreach (string s in Data.Enumerate())
            {
                ReadOnlySpan<char> line = s;
                int commaIdx = line.IndexOf(',');
                int x = int.Parse(line.Slice(0, commaIdx));
                line = line.Slice(commaIdx + 1);
                commaIdx = line.IndexOf(',');
                int y = int.Parse(line.Slice(0, commaIdx));
                line = line.Slice(commaIdx + 1);
                int z = int.Parse(line);

                grid[x + 1][y + 1][z + 1] = 1;
            }

            return grid;
        }

        internal static void Problem1()
        {
            byte[][][] grid = Load();
            int faces = CountFaces(grid);

            Console.WriteLine(faces);
        }

        private static int CountFaces(byte[][][] grid)
        {
            int faces = 0;

            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    byte inside = 0;
                    byte[] piece = grid[x][y];

                    for (int z = 0; z < GridSize; z++)
                    {
                        if (piece[z] != inside)
                        {
                            faces++;
                            inside = piece[z];
                        }
                    }
                }
            }

            for (int x = 0; x < GridSize; x++)
            {
                byte[][] plane = grid[x];

                for (int z = 0; z < GridSize; z++)
                {
                    byte inside = 0;

                    for (int y = 0; y < GridSize; y++)
                    {
                        if (plane[y][z] != inside)
                        {
                            faces++;
                            inside = plane[y][z];
                        }
                    }
                }
            }

            for (int y = 0; y < GridSize; y++)
            {
                for (int z = 0; z < GridSize; z++)
                {
                    byte inside = 0;

                    for (int x = 0; x < GridSize; x++)
                    {
                        byte test = grid[x][y][z];

                        if (test != inside)
                        {
                            faces++;
                            inside = test;
                        }
                    }
                }
            }

            return faces;
        }

        internal static void Problem2()
        {
            byte[][][] grid = Load();

            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    for (int z = 0; z < GridSize; z++)
                    {
                        if (grid[x][y][z] == 0 && !CanReachZero(grid, x, y, z))
                        {
                            grid[x][y][z] = 1;
                        }
                    }
                }
            }

            int faces = CountFaces(grid);
            Console.WriteLine(faces);
        }

        private static bool CanReachZero(byte[][][] grid, int x, int y, int z)
        {
            Point3 test = new Point3(x, y, z);

            if (s_gScore.TryGetValue(test, out int endScore))
            {
                return endScore == 0;
            }

            return FindCheapestPath(grid, test) == 0;
        }

        private static Dictionary<Point3, int> s_gScore = new Dictionary<Point3, int>
        {
            { new Point3(0,0,0), 0 },
        };

        private static int FindCheapestPath(byte[][][] grid, Point3 start)
        {
            HashSet<Point3> openSet = new HashSet<Point3> { new Point3(0, 0, 0) };

            if (s_gScore.TryGetValue(start, out int endScore))
            {
                return endScore;
            }

            while (openSet.Count > 0)
            {
                var current = openSet.First();
                openSet.Remove(current);

                if (current == start)
                {
                    continue;
                }

                for (int deltaX = -1; deltaX < 2; deltaX++)
                {
                    for (int deltaY = -1; deltaY < 2; deltaY++)
                    {
                        for (int deltaZ = -1; deltaZ < 2; deltaZ++)
                        {
                            if (Math.Abs(deltaX) + Math.Abs(deltaY) + Math.Abs(deltaZ) != 1)
                            {
                                continue;
                            }

                            int x = current.X + deltaX;
                            int y = current.Y + deltaY;
                            int z = current.Z + deltaZ;

                            if (x < 0 || y < 0 || z < 0 ||
                                x >= GridSize || y >= GridSize || z >= GridSize)
                            {
                                continue;
                            }

                            if (grid[x][y][z] == 0)
                            {
                                Point3 neighbor = new Point3(x, y, z);
                                int score = 0;

                                ref int neighborCost =
                                    ref CollectionsMarshal.GetValueRefOrAddDefault(s_gScore, neighbor, out bool exists);

                                if (!exists || score < neighborCost)
                                {
                                    neighborCost = score;
                                    openSet.Add(neighbor);
                                }
                            }
                        }
                    }
                }
            }

            if (s_gScore.TryGetValue(start, out endScore))
            {
                return endScore;
            }

            s_gScore[start] = 1;
            return 1;
        }
    }
}