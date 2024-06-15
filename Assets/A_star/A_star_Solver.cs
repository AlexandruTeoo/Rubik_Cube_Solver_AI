using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class A_star_Solver
{
    private static string goalState = "WWWWWWWWWYYYYYYYYYOOOOOOOOORRRRRRRRRGGGGGGGGGBBBBBBBBB";

    private static int Heuristic(string state)
    {
        int misplacedCount = 0;
        for (int i = 0; i < state.Length; i++)
        {
            if (state[i] != goalState[i])
                misplacedCount++;
        }
        return misplacedCount;
    }

    private static List<string> GetNeighbors(string state)
    {
        List<string> neighbors = new List<string>();

        List<string> moves = new List<string>
        {
            "U", "U'", "U2",
            "D", "D'", "D2",
            "F", "F'", "F2",
            "B", "B'", "B2",
            "L", "L'", "L2",
            "R", "R'", "R2"
        };

        foreach (string move in moves)
        {
            string newState = state;
            ApplyMove(ref newState, move);
            neighbors.Add(newState);
        }

        return neighbors;
    }

    private static void ApplyMove(ref string state, string move)
    {
        GameObject autoShuffleGO = new GameObject("AutoShuffle");
        AutoShuffle autoShuffle = autoShuffleGO.AddComponent<AutoShuffle>();

        CubeFacesState cubeState = new CubeFacesState(state);
        autoShuffle.SetState(cubeState);

        autoShuffle.Move(move);

        state = cubeState.ToString();

        GameObject.Destroy(autoShuffleGO); // Distruge instan?a temporar? dup? utilizare
    }

    public static List<string> A_star_search(string startState)
    {
        PriorityQueue<string> frontier = new PriorityQueue<string>();
        frontier.Enqueue(startState, 0);

        Dictionary<string, int> costSoFar = new Dictionary<string, int>();
        costSoFar[startState] = 0;

        Dictionary<string, string> cameFrom = new Dictionary<string, string>();
        cameFrom[startState] = null;

        while (frontier.Count > 0)
        {
            string currentState = frontier.Dequeue();

            if (currentState == goalState)
            {
                List<string> path = new List<string>();
                string reconstructState = currentState;
                while (reconstructState != null)
                {
                    path.Add(reconstructState);
                    reconstructState = cameFrom[reconstructState];
                }
                path.Reverse();
                return path;
            }

            foreach (string next in GetNeighbors(currentState))
            {
                int newCost = costSoFar[currentState] + 1;
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    int priority = newCost + Heuristic(next);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = currentState;
                }
            }
        }

        return null; // Dac? nu g?sim o solu?ie
    }

    private class PriorityQueue<T>
    {
        private List<Tuple<T, int>> elements = new List<Tuple<T, int>>();

        public int Count { get { return elements.Count; } }

        public void Enqueue(T item, int priority)
        {
            elements.Add(Tuple.Create(item, priority));
        }

        public T Dequeue()
        {
            int bestIndex = 0;
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].Item2 < elements[bestIndex].Item2)
                {
                    bestIndex = i;
                }
            }

            T bestItem = elements[bestIndex].Item1;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }
}
