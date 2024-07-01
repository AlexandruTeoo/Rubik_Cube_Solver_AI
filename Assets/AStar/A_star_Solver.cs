using System;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Linq;

public static class AStarSolver
{
    private static readonly Dictionary<string, int[]> moveMappings = new Dictionary<string, int[]>
    {
        // U move (Up face clockwise)
        { "U", new[]
            { 6, 3, 0, 7, 4, 1, 8, 5, 2,
            18, 19, 20, 12, 13, 14, 15, 16, 17, 
            27, 28, 29, 21, 22, 23, 24, 25, 26, 
            36, 37, 38, 30, 31, 32, 33, 34, 35, 
            9, 10, 11, 39, 40, 41, 42, 43, 44, 
            45, 46, 47, 48, 49, 50, 51, 52, 53 } 
        },

        // U' move (Up face counter-clockwise)
        { "U'", new[]
            { 2, 5, 8, 1, 4, 7, 0, 3, 6, 
            36, 37, 38, 12, 13, 14, 15, 16, 17, 
            9, 10, 11, 21, 22, 23, 24, 25, 26, 
            18, 19, 20, 30, 31, 32, 33, 34, 35, 
            27, 28, 29, 39, 40, 41, 42, 43, 44, 
            45, 46, 47, 48, 49, 50, 51, 52, 53 } 
        },

        // U2 move (Up face 180 degrees)
        { "U2", new[]
            { 8, 7, 6, 5, 4, 3, 2, 1, 0, 
            27, 28 ,29, 12 ,13, 14, 15, 16, 17, 
            36, 37, 38, 21, 22, 23, 24, 25, 26, 
            9, 10, 11, 30, 31, 32, 33, 34, 35, 
            18, 19, 20, 39, 40, 41, 42, 43, 44, 
            45, 46, 47, 48, 49, 50, 51, 52, 53 } 
        },

        // L move (Left face clockwise)
        { "L", new[]
            { 44, 1, 2, 41, 4, 5, 38, 7, 8, 
            15, 12, 9, 16, 13, 10, 17, 14, 11, 
            0, 19, 20, 3, 22, 23, 6, 25, 26, 
            27, 28, 29, 30, 31, 32, 33, 34, 35, 
            36, 37, 51, 39, 40, 48, 42, 43, 45,
            18, 46, 47, 21, 49, 50, 24, 52, 53 } 
        },

        // L' move (Left face counter-clockwise)
        { "L'", new[]
            { 18, 1, 2, 21, 4, 5, 24, 7, 8, 
            11, 14, 17, 10, 13, 16, 9, 12, 15, 
            45, 19, 20, 48, 22, 23, 51, 25, 26, 
            27, 28, 29, 30, 31, 32, 33, 34, 35, 
            36, 37, 6, 39, 40, 3, 42, 43, 0,
            44, 46, 47, 41, 49, 50, 38, 52, 53 } 
        },

        // L2 move (Left face 180 degrees)
        { "L2", new[]
            { 45, 1, 2, 48, 4, 5, 51, 7, 8, 
            17, 16, 15, 14, 13, 12, 11, 10, 9, 
            44, 19, 20, 41, 22, 23, 38, 25, 26, 
            27, 28, 29, 30, 31, 32, 33, 34, 35, 
            36, 37, 24, 39, 40, 21, 42, 43, 18, 
            0, 46, 47, 3, 49, 50, 6, 52, 53 } 
        },

        // F move (Front face clockwise)
        { "F", new[]
            { 0, 1, 2, 3, 4, 5, 17, 14, 11, 
            9, 10, 45, 12, 13, 46, 15, 16, 47, 
            24, 21, 18, 25, 22, 19, 26, 23, 20, 
            6, 28, 29, 7, 31, 32, 8, 34, 35, 
            36, 37, 38, 39, 40, 41, 42, 43, 44, 
            33, 30, 27, 48, 49, 50, 51, 52, 53 } 
        },

        // F' move (Front face counter-clockwise)
        { "F'", new[]
            { 0, 1, 2, 3, 4, 5, 27, 30, 33, 
            9, 10, 8, 12, 13, 7, 15, 16, 6, 
            20, 23, 26, 19, 22, 25, 18, 21, 24, 
            47, 28, 29, 46, 31, 32, 45, 34, 35,
            36, 37, 38, 39, 40, 41, 42, 43, 44, 
            11, 14, 17, 48, 49, 50, 51, 52, 53 }
        },

        // F2 move (Front face 180 degrees)
        { "F2", new[]
            { 0, 1, 2, 3, 4, 5, 47, 46, 45, 
            9, 10, 33, 12, 13, 30, 15, 16, 27, 
            26, 25, 24, 23, 22, 21, 20, 19, 18, 
            17, 28, 29, 14, 31, 32, 11, 34, 35,
            36, 37, 38, 39, 40, 41, 42, 43, 44, 
            8, 7, 6, 48, 49, 50, 51, 52, 53 }
        },
        
        // R move (Right face clockwise)
        { "R", new[]
            { 0, 1, 20, 3, 4, 23, 6, 7, 26, 
            9, 10, 11, 12, 13, 14, 15, 16, 17, 
            18, 19, 47, 21, 22, 50, 24, 25, 53, 
            33, 30, 27, 34, 31, 28, 35, 32, 29, 
            8, 37, 38, 5, 40, 41, 2, 43, 44, 
            45, 46, 42, 48, 49, 39, 51, 52, 36 } 
        },

        // R' move (Right face counter-clockwise)
        { "R'", new[]
            { 0, 1, 42, 3, 4, 39, 6, 7, 36,
            9, 10, 11, 12, 13, 14, 15, 16, 17,
            18, 19, 2, 21, 22, 5, 24, 25, 8,
            29, 32, 35, 28, 31, 34, 27, 30, 33,
            53, 37, 38, 50, 40, 41, 47, 43, 44,
            45, 46, 20, 48, 49, 23, 51, 52, 26 }
        },

        // R2 move (Right face 180 degrees)
        { "R2", new[]
            { 0, 1, 47, 3, 4, 50, 6, 7, 53,
            9, 10, 11, 12, 13, 14, 15, 16, 17, 
            18, 19, 42, 21, 22, 39, 24, 25, 36, 
            35, 34, 33, 32, 31, 30, 29, 28, 27, 
            26, 37, 38, 23, 40, 41, 20, 43, 44, 
            45, 46, 2, 48, 49, 5, 51, 52, 8 } 
        },
    
        // B move (Back face clockwise)
        { "B", new[]
            { 29, 32, 35, 3, 4, 5, 6, 7, 8, 
            2, 10, 11, 1, 13, 14, 0, 16, 17, 
            18, 19, 20, 21, 22, 23, 24, 25, 26, 
            27, 28, 53, 30, 31, 52, 33, 34, 51, 
            42, 39, 36, 43, 40, 37, 44, 41, 38, 
            45, 46, 47, 48, 49, 50, 9, 12, 15 } 
        },

        // B' move (Back face counter-clockwise)
        { "B'", new[]
            { 15, 12, 9, 3, 4, 5, 6, 7, 8, 
            51, 10, 11, 52, 13, 14, 53, 16, 17,
            18, 19, 20, 21, 22, 23, 24, 25, 26,
            27, 28, 0, 30, 31, 1, 33, 34, 2, 
            38, 41, 44, 37, 40, 43, 36, 39, 42, 
            45, 46, 47, 48, 49, 50, 35, 32, 29 }
        },

        // B2 move (Back face 180 degrees)
        { "B2", new[]
            { 53, 52, 51, 3, 4, 5, 6, 7, 8, 
            35, 10, 11, 32, 13, 14, 29, 16, 17,
            18, 19, 20, 21, 22, 23, 24, 25, 26,
            27, 28, 15, 30, 31, 12, 33, 34, 9, 
            44, 43, 42, 41, 40, 39, 38, 37, 36, 
            45, 46, 47, 48, 49, 50, 2, 1, 0 }
        },

        // D move (Down face clockwise)
        { "D", new[]
            { 0, 1, 2, 3, 4, 5, 6, 7, 8, 
            9, 10, 11, 12, 13, 14, 42, 43, 44, 
            18, 19, 20, 21, 22, 23, 15, 16, 17, 
            27, 28, 29, 30, 31, 32, 24, 25, 26, 
            36, 37, 38, 39, 40, 41, 33, 34, 35, 
            51, 48, 45, 52, 49, 46, 53, 50, 47 } 
        },

        // D' move (Down face counter-clockwise)
        { "D'", new[]
            { 0, 1, 2, 3, 4, 5, 6, 7, 8, 
            9, 10, 11, 12, 13, 14, 24, 25, 26, 
            18, 19, 20, 21, 22, 23, 33, 34, 35, 
            27, 28, 29, 30, 31, 32, 42, 43, 44, 
            36, 37, 38, 39, 40, 41, 15, 16, 17, 
            47, 50, 53, 46, 49, 52, 45, 48, 51 } 
        },

        // D2 move (Down face 180 degrees)
        { "D2", new[]
            { 0, 1, 2, 3, 4, 5, 6, 7, 8, 
            9, 10, 11, 12, 13, 14, 33, 34, 35, 
            18, 19, 20, 21, 22, 23, 42, 43, 44, 
            27, 28, 29, 30, 31, 32, 15, 16, 17, 
            36, 37, 38, 39, 40, 41, 24, 25, 26, 
            53, 52, 51, 50, 49, 48, 47, 46, 45 } 
        }
    };

    public static Dictionary<string, int[]> GetMoveMappings()
    {
        return new Dictionary<string, int[]>(moveMappings);
    }

    private static string RotateFace(string state, int[] mapping)
    {
        char[] newState = new char[54];
        for (int i = 0; i < 54; i++)
        {
            if (mapping[i] < 0 || mapping[i] >= 54)
            {
                throw new IndexOutOfRangeException($"Index {mapping[i]} is out of bounds for array of size 54.");
            }
            newState[i] = state[mapping[i]];
        }
        return new string(newState);
    }

    public static string ApplyMove(string state, string move)
    {
        if (moveMappings.ContainsKey(move))
        {
            return RotateFace(state, moveMappings[move]);
        }
        return state;
    }

    public static bool IsGoalState(string state)
    {
        for (int i = 0; i < 54; i += 9)
        {
            char faceColor = state[i];
            for (int j = i; j < i + 9; j++)
            {
                if (state[j] != faceColor)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private static readonly int[] correctPositions = {
        0, 1, 2, 3, 4, 5, 6, 7, 8,          // Up (White)
        9, 10, 11, 12, 13, 14, 15, 16, 17,  // Left (Orange)
        18, 19, 20, 21, 22, 23, 24, 25, 26, // Front (Green)
        27, 28, 29, 30, 31, 32, 33, 34, 35, // Right (Red)
        36, 37, 38, 39, 40, 41, 42, 43, 44, // Back (Blue)
        45, 46, 47, 48, 49, 50, 51, 52, 53  // Down (Yellow)
    };

    private static readonly Vector3[] piecePositions = new Vector3[54]
    {
        // Up (White) face
        new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(2, 1, 0),
        new Vector3(0, 1, 1), new Vector3(1, 1, 1), new Vector3(2, 1, 1),
        new Vector3(0, 1, 2), new Vector3(1, 1, 2), new Vector3(2, 1, 2),

        // Left (Orange) face
        new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 0, 2),
        new Vector3(0, 1, 0), new Vector3(0, 1, 1), new Vector3(0, 1, 2),
        new Vector3(0, 2, 0), new Vector3(0, 2, 1), new Vector3(0, 2, 2),

        // Front (Green) face
        new Vector3(0, 0, 2), new Vector3(1, 0, 2), new Vector3(2, 0, 2),
        new Vector3(0, 1, 2), new Vector3(1, 1, 2), new Vector3(2, 1, 2),
        new Vector3(0, 2, 2), new Vector3(1, 2, 2), new Vector3(2, 2, 2),

        // Right (Red) face
        new Vector3(2, 0, 0), new Vector3(2, 0, 1), new Vector3(2, 0, 2),
        new Vector3(2, 1, 0), new Vector3(2, 1, 1), new Vector3(2, 1, 2),
        new Vector3(2, 2, 0), new Vector3(2, 2, 1), new Vector3(2, 2, 2),

        // Back (Blue) face
        new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(2, 0, 0),
        new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(2, 1, 0),
        new Vector3(0, 2, 0), new Vector3(1, 2, 0), new Vector3(2, 2, 0),

        // Down (Yellow) face
        new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(2, 0, 0),
        new Vector3(0, 0, 1), new Vector3(1, 0, 1), new Vector3(2, 0, 1),
        new Vector3(0, 0, 2), new Vector3(1, 0, 2), new Vector3(2, 0, 2)
    };

    private static int CalculateManhattanDistance(int pos1, int pos2)
    {
        Vector3 p1 = piecePositions[pos1];
        Vector3 p2 = piecePositions[pos2];
        return (int)(Math.Abs(p1.x - p2.x) + Math.Abs(p1.y - p2.y) + Math.Abs(p1.z - p2.z));
    }

    public static int CalculateHeuristic(string state)
    {
        int heuristic = 0;

        for (int i = 0; i < 54; i++)
        {
            if (state[i] != state[correctPositions[i]])
            {
                int targetPos = Array.IndexOf(state.ToCharArray(), state[correctPositions[i]]);
                heuristic += CalculateManhattanDistance(i, targetPos);
            }
        }

        return heuristic;
    }

    public static void TestMoves()
    {
        string initialState = "WWWWWWWWWOOOOOOOOOGGGGGGGGGRRRRRRRRRBBBBBBBBBYYYYYYYYY";
        //string initialState = "RWWGWWGWWGOOGOOGOOYRRYGGYGGBBBRRRRRROOWBBWBBWBYYBYYOYY";

        string stateAfterU = ApplyMove(initialState, "U");
        UnityEngine.Debug.Log("State after U: " + stateAfterU);

        string stateAfterLPrime = ApplyMove(stateAfterU, "L'");
        UnityEngine.Debug.Log("State after L': " + stateAfterLPrime);

        // ##############
        // RWWGWWGWWGOOGOOGOOYRRYGGYGGBBBRRRRRROOWBBWBBWBYYBYYOYY

        stateAfterLPrime = ApplyMove(stateAfterLPrime, "L");
        UnityEngine.Debug.Log("State after L: " + stateAfterLPrime);
        
        stateAfterU = ApplyMove(stateAfterLPrime, "U'");
        UnityEngine.Debug.Log("State after U': " + stateAfterU);

        UnityEngine.Debug.Log("Start Simulation: ");

        string stateAfterUPrime = ApplyMove(initialState, "U'");
        UnityEngine.Debug.Log("State after U': " + stateAfterUPrime);

        string stateAfterU2 = ApplyMove(initialState, "U2");
        UnityEngine.Debug.Log("State after U2: " + stateAfterU2);

        stateAfterU = ApplyMove(initialState, "L");
        UnityEngine.Debug.Log("State after L: " + stateAfterU);

        stateAfterLPrime = ApplyMove(initialState, "L'");
        UnityEngine.Debug.Log("State after L': " + stateAfterLPrime);

        stateAfterU2 = ApplyMove(initialState, "L2");
        UnityEngine.Debug.Log("State after L2: " + stateAfterU2);

        stateAfterU = ApplyMove(initialState, "F");
        UnityEngine.Debug.Log("State after F: " + stateAfterU);

        stateAfterUPrime = ApplyMove(initialState, "F'");
        UnityEngine.Debug.Log("State after F': " + stateAfterUPrime);

        stateAfterU2 = ApplyMove(initialState, "F2");
        UnityEngine.Debug.Log("State after F2: " + stateAfterU2);

        stateAfterU = ApplyMove(initialState, "R");
        UnityEngine.Debug.Log("State after R: " + stateAfterU);

        stateAfterUPrime = ApplyMove(initialState, "R'");
        UnityEngine.Debug.Log("State after R': " + stateAfterUPrime);

        stateAfterU2 = ApplyMove(initialState, "R2");
        UnityEngine.Debug.Log("State after R2: " + stateAfterU2);

        stateAfterU = ApplyMove(initialState, "B");
        UnityEngine.Debug.Log("State after B: " + stateAfterU);

        stateAfterUPrime = ApplyMove(initialState, "B'");
        UnityEngine.Debug.Log("State after B': " + stateAfterUPrime);

        stateAfterU2 = ApplyMove(initialState, "B2");
        UnityEngine.Debug.Log("State after B2: " + stateAfterU2);

        stateAfterU = ApplyMove(initialState, "D");
        UnityEngine.Debug.Log("State after D: " + stateAfterU);

        stateAfterUPrime = ApplyMove(initialState, "D'");
        UnityEngine.Debug.Log("State after D': " + stateAfterUPrime);

        stateAfterU2 = ApplyMove(initialState, "D2");
        UnityEngine.Debug.Log("State after D2: " + stateAfterU2);
    }

    // #################################

    public static List<string> AStarSearch(string startState)
    {
        // incepe masurarea timpului
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // incepe masurarea memoriei
        long initialMemoryUsage = GetStableMemoryUsage();

        CubeState start = InitializeCubeState(startState);
        PriorityQueue<CubeState> openSet = new PriorityQueue<CubeState>();
        HashSet<string> closedSet = new HashSet<string>();
        List<string> exploredStates = new List<string>();

        openSet.Enqueue(start, start.TotalCost);

        int iterationCount = 0;
        int maxIterations = 5000;

        while (openSet.Count > 0)
        {
            CubeState current = openSet.Dequeue();

            exploredStates.Add(current.State);
            UnityEngine.Debug.Log($"Iteration: {iterationCount}, Current State: {current.State}, Heuristic: {current.HCost}");

            if (IsGoalState(current.State))
            {
                // opreste masurarea timpului
                stopwatch.Stop();

                // opreste masurarea memoriei
                long finalMemoryUsage = GetStableMemoryUsage();
                long memoryUsedDuringSearch = finalMemoryUsage - initialMemoryUsage;

                UnityEngine.Debug.Log("Solution found!");
                UnityEngine.Debug.Log($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
                UnityEngine.Debug.Log($"Memory used: {memoryUsedDuringSearch / 1024} KB");

                return ReconstructPath(current);
            }

            closedSet.Add(current.State);

            foreach (CubeState successor in GetSuccessors(current))
            {
                if (closedSet.Contains(successor.State))
                {
                    continue;
                }

                if (!openSet.Contains(successor))
                {
                    openSet.Enqueue(successor, successor.TotalCost);
                }
            }

            iterationCount++;
            if (iterationCount >= maxIterations)
            {
                // opreste masurarea timpului
                stopwatch.Stop();

                // opreste masurarea memoriei
                long finalMemoryUsage = GetStableMemoryUsage();
                long memoryUsedDuringSearch = finalMemoryUsage - initialMemoryUsage;

                UnityEngine.Debug.LogError("Maximum iterations reached, stopping the search to avoid infinite loop.");
                UnityEngine.Debug.Log($"Explored States: {exploredStates.Count}");
                UnityEngine.Debug.Log($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
                UnityEngine.Debug.Log($"Memory used: {memoryUsedDuringSearch / 1024} KB");

                return null;
            }
        }

        // opreste masurarea timpului
        stopwatch.Stop();

        // opreste masurarea memoriei
        long finalMemoryUsageAfterFailure = GetStableMemoryUsage();
        long memoryUsedDuringSearchFailure = finalMemoryUsageAfterFailure - initialMemoryUsage;

        UnityEngine.Debug.LogError("No solution found!");
        UnityEngine.Debug.Log($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
        UnityEngine.Debug.Log($"Memory used: {memoryUsedDuringSearchFailure / 1024} KB");

        return null; // nu s-a gasit nicio solutie
    }

    private static long GetStableMemoryUsage()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        return GC.GetTotalMemory(true);
    }

    public static CubeState InitializeCubeState(string state)
    {
        CubeState initialState = new CubeState
        {
            State = state,
            GCost = 0,
            HCost = CalculateHeuristic(state),
            Parent = null
        };
        initialState.TotalCost = initialState.GCost + initialState.HCost;
        return initialState;
    }

    public static IEnumerable<CubeState> GetSuccessors(CubeState current)
    {
        foreach (string move in moveMappings.Keys)
        {
            string newState = ApplyMove(current.State, move);
            CubeState successor = new CubeState
            {
                State = newState,
                GCost = current.GCost + 1,
                HCost = CalculateHeuristic(newState),
                Parent = current,
                Move = move
            };
            successor.TotalCost = successor.GCost + successor.HCost;
            yield return successor;
        }
    }

    private static List<string> ReconstructPath(CubeState current)
    {
        List<string> path = new List<string>();
        List<string> moves = new List<string>();
        while (current != null)
        {
            if (current.Move != null)
            {
                moves.Insert(0, current.Move);
            }
            path.Insert(0, current.State);
            current = current.Parent;
        }

        return moves;
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