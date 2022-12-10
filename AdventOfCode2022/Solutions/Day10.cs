using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day10
    {
        private static IEnumerable<(string, int)> LoadData()
        {
            Regex regex = GenEx();

            foreach (string s in Data.Enumerate())
            {
                if (s == "noop")
                {
                    yield return (s, 0);
                }
                else
                {
                    Match match = regex.Match(s);
                    yield return
                    (
                        match.Groups[1].Value,
                        match.Groups.Count > 1 ? int.Parse(match.Groups[2].ValueSpan) : 0);
                }
            }
        }

        [GeneratedRegex(@"^(.+) (-?\d+)?")]
        private static partial Regex GenEx();

        internal static void Problem1()
        {
            int x = 1;
            int cycleCount = 1;
            int score = 0;

            foreach ((string verb, int arg) in LoadData())
            {
                switch (verb)
                {
                    case "addx":
                        if (cycleCount is 19 or 59 or 99 or 139 or 179 or 219)
                        {
                            score += (cycleCount + 1) * x;
                        }

                        cycleCount += 2;
                        x += arg;
                        break;
                    case "noop":
                        cycleCount++;
                        break;
                }

                if (cycleCount is 20 or 60 or 100 or 140 or 180 or 220)
                {
                    score += cycleCount * x;
                }

                if (cycleCount > 220)
                {
                    break;
                }
            }

            Console.WriteLine(score);
        }

        internal static void Problem2()
        {
            int x = 1;
            int cycleCount = 1;

            foreach ((string verb, int arg) in LoadData())
            {
                switch (verb)
                {
                    case "addx":
                        DrawPixel(cycleCount, x);
                        cycleCount++;
                        x += arg;
                        DrawPixel(cycleCount, x);
                        cycleCount++;
                        break;
                    case "noop":
                        DrawPixel(cycleCount, x);
                        cycleCount++;
                        break;
                }
            }

            static void DrawPixel(int cycleCount, int x)
            {
                int col = (cycleCount - 1) % 40 + 1;

                if (col - x is -1 or 0 or 1)
                {
                    Console.Write('#');
                }
                else
                {
                    Console.Write('.');
                }

                if (col == 40)
                {
                    Console.WriteLine();
                }
            }
        }
    }
}