using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace AdventOfCode2022
{
    internal static class Utils
    {
        internal delegate TCost AStarEstimator<in TWorld, in TPosition, out TCost>(
            TPosition candidate,
            TPosition end,
            TWorld world);

        internal delegate IEnumerable<(TPosition Neighbor, TCost Cost)> AStarNeighbors<in TWorld, TPosition, TCost>(
            TPosition from,
            TWorld world);

        internal static TCost AStar<TWorld, TPosition, TCost>(
            TWorld world,
            TPosition start,
            TPosition end,
            AStarNeighbors<TWorld, TPosition, TCost> neighbors,
            AStarEstimator<TWorld, TPosition, TCost> estimator,
            List<TPosition> pathToFill = null,
            Dictionary<TPosition, TCost> gScore = null)
            where TCost : INumber<TCost>, IMinMaxValue<TCost>
            where TPosition : IEquatable<TPosition>
        {
            PriorityQueue<TPosition, TCost> openSet = new PriorityQueue<TPosition, TCost>();
            HashSet<TPosition> openSetFilter = new HashSet<TPosition>();
            Dictionary<TPosition, TPosition> cameFrom = (pathToFill is null) ? null : new Dictionary<TPosition, TPosition>();
            gScore ??= new Dictionary<TPosition, TCost>();

            gScore[start] = TCost.Zero;

            openSet.Enqueue(start, TCost.Zero);
            openSetFilter.Add(start);

            while (openSet.Count > 0)
            {
                TPosition current = openSet.Dequeue();

                if (current.Equals(end))
                {
                    break;
                }

                openSetFilter.Remove(current);
                TCost currentScore = gScore[current];

                foreach ((TPosition neighbor, TCost cost) in neighbors(current, world))
                {
                    TCost tentative = currentScore + cost;

                    ref TCost neighborBest =
                        ref CollectionsMarshal.GetValueRefOrAddDefault(gScore, neighbor, out bool exists);

                    if (!exists || tentative < neighborBest)
                    {
                        neighborBest = tentative;

                        if (cameFrom is not null)
                        {
                            cameFrom[neighbor] = current;
                        }

                        TCost estimatedCost = tentative + estimator(neighbor, end, world);

                        if (openSetFilter.Add(neighbor))
                        {
                            openSet.Enqueue(neighbor, estimatedCost);
                        }
                    }
                }
            }

            if (gScore.TryGetValue(end, out TCost finalCost))
            {
                if (pathToFill is not null)
                {
                    Stack<TPosition> stack = new Stack<TPosition>();
                    TPosition current = end;

                    while (true)
                    {
                        stack.Push(current);

                        if (current.Equals(start))
                        {
                            break;
                        }

                        current = cameFrom[current];
                    }

                    pathToFill.AddRange(stack);
                }

                return finalCost;
            }

            return TCost.MaxValue;
        }

        internal static (bool Found, TNode Value) BreadthFirstSearch<TWorld, TNode>(
            TWorld world,
            TNode start,
            Predicate<TNode> predicate,
            Func<TNode, TWorld, IEnumerable<TNode>> children,
            bool childrenDoNotRepeat = false)
            where TNode : IEquatable<TNode>
        {
            HashSet<TNode> dedup = null;
            Queue<TNode> queue = new Queue<TNode>();
            queue.Enqueue(start);

            if (!childrenDoNotRepeat)
            {
                dedup = new HashSet<TNode>();
            }

            while (queue.Count > 0)
            {
                TNode item = queue.Dequeue();

                if (predicate(item))
                {
                    return (true, item);
                }

                foreach (TNode next in children(item, world))
                {
                    if (childrenDoNotRepeat || dedup.Add(next))
                    {
                        queue.Enqueue(next);
                    }
                }
            }

            return (false, default(TNode));
        }
    }
}