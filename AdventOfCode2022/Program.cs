using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            MethodInfo method;
            int day;

            try
            {
                method = GetTarget(args, out day);

                if (method == null)
                {
                    Console.WriteLine($"No runnable problem found on day {day}.");
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error determining what part to run.");
                Console.WriteLine(e);
                return;
            }

            try
            {
                LoadData(day);
            }
            catch (Exception e)
            {
#if SAMPLE
                Console.WriteLine($"Error loading sample data for day {day}.");
#else
                Console.WriteLine($"Error loading data for day {day}.");
#endif
                Console.WriteLine(e);
                return;
            }

            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                object resp = method.Invoke(null, null);

                if (resp is Task t)
                {
                    await t.ConfigureAwait(false);
                }
            }
            catch (TargetInvocationException tie)
            {
                Console.WriteLine(tie.InnerException);
            }

            sw.Stop();

#if SAMPLE
            Console.WriteLine("**SAMPLE ANSWER**");
#endif
            Console.WriteLine($"Executed in {sw.Elapsed.TotalMilliseconds}ms.");
        }

        private static void LoadData(int day)
        {
#if SAMPLE
            string filename = $"Day{day:D2}_sample.txt";
#else
            string filename = $"Day{day:D2}.txt";
#endif

            string[] lines = File.ReadAllLines(Path.Combine("..", "..", "..", "Data", filename));
            typeof(Data).GetField("s_lines", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, lines);
        }

        private static MethodInfo GetTarget(string[] args, out int day)
        {
            const BindingFlags Flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            string rootNS = typeof(Program).Namespace;

            if (args?.Length > 0)
            {
                if (args.Length != 1)
                {
                    Console.WriteLine("Enter the day number, optionally with a -1 for part 1 (e.g. 14-1)");
                }

                int hyphen = args[0].IndexOf('-');
                int? part = null;

                if (hyphen >= 0)
                {
                    day = int.Parse(args[0].AsSpan(0, hyphen));
                    part = int.Parse(args[0].AsSpan(hyphen + 1));
                }
                else
                {
                    day = int.Parse(args[0]);
                }

                Type t = typeof(Program).Assembly.GetType($"{rootNS}.Solutions.Day{day:D2}", true);

                if (part.HasValue)
                {
                    return t.GetMethod("Problem" + part, Flags);
                }

                return t.GetMethod("Problem2", Flags) ?? t.GetMethod("Problem1", Flags);
            }

            for (int i = 25; i > 0; i--)
            {
                Type t = typeof(Program).Assembly.GetType($"{rootNS}.Solutions.Day{i:D2}", false);

                if (t != null)
                {
                    day = i;
                    return t.GetMethod("Problem2", Flags) ?? t.GetMethod("Problem1", Flags);
                }
            }

            throw new InvalidOperationException("Could not find a runnable day.");
        }
    }
}
