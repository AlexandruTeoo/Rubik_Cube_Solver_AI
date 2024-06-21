using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UDebug = UnityEngine.Debug;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Diagnostics;
using static A_star_Solver;
using static A_star_bid_Solver;
using static IDA_star_Solver;
using System;

public class Resolve : MonoBehaviour
{
    public Button solveButton;
    private SolutionSaver solutionSaver;

    // Start is called before the first frame update
    void Start()
    {
        solutionSaver = new SolutionSaver();

        // Add a click event to the button
        solveButton.onClick.AddListener(OnSolveButtonClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnSolveButtonClick()
    {
        // Call the function to save the cube colors and send the file to Python
        FindObjectOfType<ReadCubeFaces>().ReadStateAndSaveToFile();

        string startState = ReadCubeStateFromFile("cube_data.txt");
        UDebug.Log(startState);

        // Solve using A* algorithm
        SolveAndSaveSolution(startState, A_star_Solver.AStarSearch, "a_star.txt");

        // Solve using A* bidirectional algorithm
        SolveAndSaveSolution(startState, A_star_bid_Solver.AStarBidirectionalSearch, "a_star_bid.txt");

        // Solve using IDA* algorithm with maxIterations
        SolveAndSaveSolution(startState, IDA_star_Solver.IDAStarSearch, "ida_star.txt");
    }

    string ReadCubeStateFromFile(string filePath)
    {
        try
        {
            string state = File.ReadAllText(filePath).Trim();
            if (state.Length == 54) // Ensure the state has 54 characters
            {
                return state;
            }
            else
            {
                UDebug.LogError("Invalid cube state in file.");
                return null;
            }
        }
        catch (Exception ex)
        {
            UDebug.LogError("Error reading cube state from file: " + ex.Message);
            return null;
        }
    }

    void SolveAndSaveSolution(string startState, Func<string, List<string>> solveMethod, string filePath)
    {
        if (string.IsNullOrEmpty(startState)) return;

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        long initialMemoryUsage = GetStableMemoryUsage();

        List<string> solutionMoves = solveMethod(startState);

        stopwatch.Stop();
        long finalMemoryUsage = GetStableMemoryUsage();
        long memoryUsed = finalMemoryUsage - initialMemoryUsage;

        if (solutionMoves != null)
        {
            UDebug.Log("Solution found!");
            foreach (string move in solutionMoves)
            {
                UDebug.Log(move);
            }
        }
        else
        {
            UDebug.LogError("No solution found!");
        }

        solutionSaver.SaveSolution(filePath, solutionMoves, stopwatch.ElapsedMilliseconds, memoryUsed);
    }

    private static long GetStableMemoryUsage()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        return GC.GetTotalMemory(true);
    }
}
