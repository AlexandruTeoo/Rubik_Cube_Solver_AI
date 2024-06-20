using System;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using static A_star_Solver;

public static class A_star_bid_Solver
{
    private static readonly Dictionary<string, int[]> moveMappings = A_star_Solver.GetMoveMappings();

    public static List<string> AStarBidirectionalSearch(string startState)
    {
        var start = A_star_Solver.InitializeCubeState(startState);
        var goal = A_star_Solver.InitializeCubeState("WWWWWWWWWOOOOOOOOOGGGGGGGGGRRRRRRRRRBBBBBBBBBYYYYYYYYY");

        var forwardQueue = new PriorityQueue<CubeState>();
        var backwardQueue = new PriorityQueue<CubeState>();

        var forwardClosed = new HashSet<string>();
        var backwardClosed = new HashSet<string>();

        forwardQueue.Enqueue(start, start.TotalCost);
        backwardQueue.Enqueue(goal, goal.TotalCost);

        var forwardParents = new Dictionary<string, CubeState>();
        var backwardParents = new Dictionary<string, CubeState>();

        forwardParents[start.State] = null;
        backwardParents[goal.State] = null;

        while (forwardQueue.Count > 0 && backwardQueue.Count > 0)
        {
            var forwardCurrent = forwardQueue.Dequeue();
            if (backwardClosed.Contains(forwardCurrent.State))
            {
                UnityEngine.Debug.Log($"Meeting point found at state: {forwardCurrent.State}");
                return ConstructBidirectionalPath(forwardParents, backwardParents, forwardCurrent.State);
            }

            forwardClosed.Add(forwardCurrent.State);
            UnityEngine.Debug.Log($"Forward exploring state: {forwardCurrent.State}");

            foreach (var neighbor in A_star_Solver.GetSuccessors(forwardCurrent))
            {
                if (!forwardClosed.Contains(neighbor.State))
                {
                    forwardQueue.Enqueue(neighbor, neighbor.TotalCost);
                    forwardParents[neighbor.State] = forwardCurrent;
                }
            }

            var backwardCurrent = backwardQueue.Dequeue();
            if (forwardClosed.Contains(backwardCurrent.State))
            {
                UnityEngine.Debug.Log($"Meeting point found at state: {backwardCurrent.State}");
                return ConstructBidirectionalPath(forwardParents, backwardParents, backwardCurrent.State);
            }

            backwardClosed.Add(backwardCurrent.State);
            UnityEngine.Debug.Log($"Backward exploring state: {backwardCurrent.State}");

            foreach (var neighbor in A_star_Solver.GetSuccessors(backwardCurrent))
            {
                if (!backwardClosed.Contains(neighbor.State))
                {
                    backwardQueue.Enqueue(neighbor, neighbor.TotalCost);
                    backwardParents[neighbor.State] = backwardCurrent;
                }
            }
        }

        return null; // No solution found
    }

    private static List<string> ConstructBidirectionalPath(Dictionary<string, CubeState> forwardParents, Dictionary<string, CubeState> backwardParents, string meetingPoint)
    {
        var forwardPath = new List<string>();
        var backwardPath = new List<string>();

        // Reconstruie?te calea de la start la meetingPoint
        var current = forwardParents[meetingPoint];
        while (current != null)
        {
            if (current.Move != null)
            {
                forwardPath.Add(current.Move);
            }
            current = forwardParents[current.State];
        }
        forwardPath.Reverse();

        // Reconstruie?te calea de la meetingPoint la goal
        current = backwardParents[meetingPoint];
        while (current != null)
        {
            if (current.Move != null)
            {
                backwardPath.Add(ReverseMove(current.Move));
            }
            current = backwardParents[current.State];
        }

        // Îmbin?m cele dou? c?i
        var path = new List<string>();
        path.AddRange(forwardPath);
        path.AddRange(backwardPath);

        return path;
    }

    private static string ReverseMove(string move)
    {
        if (move.Length == 1)
        {
            return move + "'";
        }
        else if (move.Length == 2 && move[1] == '\'')
        {
            return move[0].ToString();
        }
        return move;
    }

    private static long GetStableMemoryUsage()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        return GC.GetTotalMemory(true);
    }
}

public class PriorityQueue<T>
{
    private readonly SortedList<int, Queue<T>> _list = new SortedList<int, Queue<T>>();

    public void Enqueue(T item, int priority)
    {
        if (!_list.ContainsKey(priority))
        {
            _list[priority] = new Queue<T>();
        }
        _list[priority].Enqueue(item);
    }

    public T Dequeue()
    {
        if (_list.Count == 0)
        {
            throw new InvalidOperationException("The priority queue is empty");
        }

        var pair = _list.Values[0];
        var v = pair.Dequeue();
        if (pair.Count == 0)
        {
            _list.RemoveAt(0);
        }

        return v;
    }

    public bool Contains(T item)
    {
        foreach (var queue in _list.Values)
        {
            if (queue.Contains(item))
            {
                return true;
            }
        }
        return false;
    }

    public int Count
    {
        get
        {
            int count = 0;
            foreach (var queue in _list.Values)
            {
                count += queue.Count;
            }
            return count;
        }
    }
}