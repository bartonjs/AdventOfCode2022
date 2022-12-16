using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day16
    {
        [GeneratedRegex(@"Valve (..) has flow rate=(\d+); tunnels? leads? to valves? (.*)")]
        private static partial Regex MatchRegex();

        [DebuggerDisplay("{Name} - {Rate}")]
        private class Valve
        {
            public string Name;
            public int Index;
            public int Rate;
            public List<string> Connections;
        }

        [DebuggerDisplay("{CurrentNode} - {Remaining}")]
        private struct State : IEquatable<State>
        {
            public string CurrentNode;
            public long Remaining;

            public bool Equals(State other)
            {
                return CurrentNode == other.CurrentNode && Remaining == other.Remaining;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (obj.GetType() != GetType()) return false;
                return Equals((State)obj);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(CurrentNode, Remaining);
            }
        }

        private static Dictionary<string, Valve> Load()
        {
            Regex regex = MatchRegex();
            Dictionary<string, Valve> valves = new Dictionary<string, Valve>();
            int index = 0;

            foreach (string s in Data.Enumerate())
            {
                Match m = regex.Match(s);

                Valve v = new Valve
                {
                    Name = m.Groups[1].Value,
                    Index = index,
                    Rate = int.Parse(m.Groups[2].ValueSpan),
                    Connections = new List<string>(),
                };

                index++;
                ReadOnlySpan<char> conns = m.Groups[3].ValueSpan;

                while (!conns.IsEmpty)
                {
                    int commaIdx = conns.IndexOf(',');

                    if (commaIdx >= 0)
                    {
                        v.Connections.Add(conns.Slice(0, commaIdx).ToString());
                        conns = conns.Slice(commaIdx + 2);
                    }
                    else
                    {
                        v.Connections.Add(conns.ToString());
                        break;
                    }
                }

                valves[v.Name] = v;
            }

            return valves;
        }

        private static Dictionary<string, Valve> Valves { get; } = Load();

        private static void Stuff(Dictionary<State, (int Score, int Remain)> states, Valve candidate, State currentState, int remain, int score)
        {
            currentState.CurrentNode = candidate.Name;

            ref (int Score, int Remain) cur = ref CollectionsMarshal.GetValueRefOrAddDefault(states, currentState, out bool exists);

            if (exists)
            {
                if (cur.Score > score)
                {
                    return;
                }
                
                if (cur.Score == score && cur.Remain >= remain)
                {
                    return;
                }
            }

            cur = (score, remain);

            int rm1 = remain - 1;

            if (remain > 3)
            {
                foreach (string conn in candidate.Connections)
                {
                    Valve next = Valves[conn];
                    Stuff(states, next, currentState, rm1, score);
                }
            }

            if (candidate.Rate > 0)
            {
                long me = 1L << candidate.Index;

                if ((currentState.Remaining & me) != 0)
                {
                    currentState.Remaining &= ~me;
                    int localScore = rm1 * candidate.Rate;
                    score += localScore;

                    ref (int Score, int Remain) cur2 = ref CollectionsMarshal.GetValueRefOrAddDefault(states, currentState, out _);

                    if (score > cur2.Score)
                    {
                        cur2 = (score, rm1);
                    }

                    if (currentState.Remaining == 0)
                    {
                        return;
                    }

                    if (remain > 3)
                    {
                        int rm2 = rm1 - 1;

                        foreach (string conn in candidate.Connections)
                        {
                            Valve next = Valves[conn];
                            Stuff(states, next, currentState, rm2, score);
                        }
                    }
                }
            }
        }

        private static KeyValuePair<State, (int Score, int Remain)> MaxState(Dictionary<State, (int, int)> states)
        {
            return states.MaxBy(kvp => kvp.Value.Item1);
        }

        internal static void Problem1()
        {
            Valve start = Valves["AA"];

            long available = 0;

            foreach (Valve v in Valves.Values)
            {
                if (v.Rate > 0)
                {
                    available |= (1L << v.Index);
                }
            }

            State startState = new State { Remaining = available };
            Dictionary<State, (int, int)> states = new();
            Stuff(states, start, startState, 30, 0);
            KeyValuePair<State, (int Score, int Remain)> pair = MaxState(states);
            Console.WriteLine($"{states.Count} known state(s).");
            Console.WriteLine(pair.Value);
        }

        internal static void Problem2a()
        {
        }
    }
}