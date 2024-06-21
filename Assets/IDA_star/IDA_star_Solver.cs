using System;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public static class IDA_star_Solver
{
    private static readonly Dictionary<string, int[]> moveMappings = A_star_Solver.GetMoveMappings();

    public static List<string> IDAStarSearch(string startState)
    {
        int threshold = A_star_Solver.CalculateHeuristic(startState);
        int maxIterations = 5000; // Seta?i limita de itera?ii dup? cum este necesar
        int iterationCount = 0;

        while (true)
        {
            HashSet<string> visited = new HashSet<string>();
            (int cost, List<string> path) result = Search(startState, 0, threshold, visited);
            if (result.cost == 0) return result.path;
            if (result.cost == int.MaxValue) return null;
            threshold = result.cost;

            iterationCount++;
            if (iterationCount >= maxIterations)
            {
                Debug.LogError("Maximum iterations reached, stopping the search to avoid infinite loop.");
                return null;
            }
        }
    }

    private static (int, List<string>) Search(string state, int g, int threshold, HashSet<string> visited)
    {
        int f = g + A_star_Solver.CalculateHeuristic(state);
        if (f > threshold) return (f, null);
        if (A_star_Solver.IsGoalState(state)) return (0, new List<string>());

        int min = int.MaxValue;
        List<string> bestPath = null;

        foreach (var move in moveMappings.Keys)
        {
            string nextState = A_star_Solver.ApplyMove(state, move);
            if (visited.Contains(nextState)) continue;

            visited.Add(nextState);
            var (cost, path) = Search(nextState, g + 1, threshold, visited);
            visited.Remove(nextState);

            if (cost == 0)
            {
                if (path != null) path.Insert(0, move);
                return (0, path);
            }

            if (cost < min)
            {
                min = cost;
                bestPath = path;
            }
        }

        return (min, bestPath);
    }
}
