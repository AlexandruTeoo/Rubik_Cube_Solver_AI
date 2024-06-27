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
        // Incepe masurarea timpului
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // Incepe masurarea memoriei
        long initialMemoryUsage = GetStableMemoryUsage();

        CubeState start = InitializeCubeState(startState);
        CubeState goal = InitializeCubeState(GetSolvedState());

        PriorityQueue<CubeState> openSetStart = new PriorityQueue<CubeState>();
        PriorityQueue<CubeState> openSetGoal = new PriorityQueue<CubeState>();

        HashSet<string> closedSetStart = new HashSet<string>();
        HashSet<string> closedSetGoal = new HashSet<string>();

        Dictionary<string, CubeState> cameFromStart = new Dictionary<string, CubeState>();
        Dictionary<string, CubeState> cameFromGoal = new Dictionary<string, CubeState>();

        openSetStart.Enqueue(start, start.TotalCost);
        openSetGoal.Enqueue(goal, goal.TotalCost);

        int iterationCount = 0;
        int maxIterations = 5000;

        while (openSetStart.Count > 0 && openSetGoal.Count > 0)
        {
            if (openSetStart.Count > 0)
            {
                CubeState currentStart = openSetStart.Dequeue();
                closedSetStart.Add(currentStart.State);

                if (closedSetGoal.Contains(currentStart.State))
                {
                    stopwatch.Stop();
                    long finalMemoryUsage = GetStableMemoryUsage();
                    long memoryUsedDuringSearch = finalMemoryUsage - initialMemoryUsage;

                    UnityEngine.Debug.Log("Solution found!");
                    UnityEngine.Debug.Log($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
                    UnityEngine.Debug.Log($"Memory used: {memoryUsedDuringSearch / 1024} KB");

                    CubeState meetingNode = cameFromGoal[currentStart.State];
                    if (meetingNode == null ) 
                    {
                        UnityEngine.Debug.LogError("No solution!");
                        return null;
                    }
                    return ReconstructPath(currentStart, meetingNode);
                }

                foreach (CubeState successor in GetSuccessors(currentStart))
                {
                    if (!closedSetStart.Contains(successor.State) && !openSetStart.Contains(successor))
                    {
                        cameFromStart[successor.State] = currentStart;
                        openSetStart.Enqueue(successor, successor.TotalCost);
                    }
                }
            }

            if (openSetGoal.Count > 0)
            {
                CubeState currentGoal = openSetGoal.Dequeue();
                closedSetGoal.Add(currentGoal.State);

                if (closedSetStart.Contains(currentGoal.State))
                {
                    stopwatch.Stop();
                    long finalMemoryUsage = GetStableMemoryUsage();
                    long memoryUsedDuringSearch = finalMemoryUsage - initialMemoryUsage;

                    UnityEngine.Debug.Log("Solution found!");
                    UnityEngine.Debug.Log($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
                    UnityEngine.Debug.Log($"Memory used: {memoryUsedDuringSearch / 1024} KB");

                    CubeState meetingNode = cameFromStart[currentGoal.State];
                    return ReconstructPath(meetingNode, currentGoal);
                }

                foreach (CubeState successor in GetSuccessors(currentGoal))
                {
                    if (!closedSetGoal.Contains(successor.State) && !openSetGoal.Contains(successor))
                    {
                        cameFromGoal[successor.State] = currentGoal;
                        openSetGoal.Enqueue(successor, successor.TotalCost);
                    }
                }
            }

            iterationCount++;
            if (iterationCount >= maxIterations)
            {
                stopwatch.Stop();
                long finalMemoryUsage = GetStableMemoryUsage();
                long memoryUsedDuringSearch = finalMemoryUsage - initialMemoryUsage;

                UnityEngine.Debug.LogError("Maximum iterations reached, stopping the search to avoid infinite loop.");
                UnityEngine.Debug.Log($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
                UnityEngine.Debug.Log($"Memory used: {memoryUsedDuringSearch / 1024} KB");

                return null;
            }
        }

        stopwatch.Stop();
        long finalMemoryUsageAfterFailure = GetStableMemoryUsage();
        long memoryUsedDuringSearchFailure = finalMemoryUsageAfterFailure - initialMemoryUsage;

        UnityEngine.Debug.LogError("No solution found!");
        UnityEngine.Debug.Log($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
        UnityEngine.Debug.Log($"Memory used: {memoryUsedDuringSearchFailure / 1024} KB");

        return null;
    }

    private static long GetStableMemoryUsage()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        return GC.GetTotalMemory(true);
    }

    private static string GetSolvedState()
    {
        return "WWWWWWWWWOOOOOOOOOGGGGGGGGGRRRRRRRRRBBBBBBBBBYYYYYYYYY";
    }

    private static CubeState InitializeCubeState(string state)
    {
        CubeState initialState = new CubeState
        {
            State = state,
            GCost = 0,
            HCost = A_star_Solver.CalculateHeuristic(state),
            Parent = null
        };
        initialState.TotalCost = initialState.GCost + initialState.HCost;
        return initialState;
    }

    private static IEnumerable<CubeState> GetSuccessors(CubeState current)
    {
        foreach (string move in moveMappings.Keys)
        {
            string newState = A_star_Solver.ApplyMove(current.State, move);
            CubeState successor = new CubeState
            {
                State = newState,
                GCost = current.GCost + 1,
                HCost = A_star_Solver.CalculateHeuristic(newState),
                Parent = current,
                Move = move
            };
            successor.TotalCost = successor.GCost + successor.HCost;
            yield return successor;
        }
    }

    private static List<string> ReconstructPath(CubeState start, CubeState goal)
    {
        List<string> path = new List<string>();

        while (start != null)
        {
            if (start.Move != null)
            {
                path.Insert(0, start.Move);
            }
            start = start.Parent;
        }

        while (goal != null)
        {
            if (goal.Move != null)
            {
                path.Add(goal.Move);
            }
            goal = goal.Parent;
        }

        return path;
    }

    public class CubeState
    {
        public string State { get; set; }
        public int GCost { get; set; }
        public int HCost { get; set; }
        public int TotalCost { get; set; }
        public CubeState Parent { get; set; }
        public string Move { get; set; }
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
}