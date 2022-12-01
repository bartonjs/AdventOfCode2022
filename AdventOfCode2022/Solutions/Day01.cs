using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.Solutions
{
    internal class Day01
    {
        internal static void Problem1()
        {
            List<int> calories = new List<int>();
            int cur = 0;

            foreach (string s in Data.Enumerate())
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    calories.Add(cur);
                    cur = 0;
                    continue;
                }

                cur += int.Parse(s);
            }

            calories.Add(cur);

            Console.WriteLine(calories.Max());
        }

        internal static void Problem2()
        {
            List<int> calories = new List<int>();
            int cur = 0;

            foreach (string s in Data.Enumerate())
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    calories.Add(cur);
                    cur = 0;
                    continue;
                }

                cur += int.Parse(s);
            }

            calories.Add(cur);

            calories.Sort();
            calories.Reverse();
            
            Console.WriteLine(calories.Take(3).Sum());
        }
    }
}