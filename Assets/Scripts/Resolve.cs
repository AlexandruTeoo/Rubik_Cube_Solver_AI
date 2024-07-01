using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UDebug = UnityEngine.Debug;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System.Diagnostics;
using static AStarSolver;
using static AStarBidSolver;
using static IDAStarSolver;
using System;

public class Resolve : MonoBehaviour
{
    public Button solveButton;
    public TMP_Text stepsText;
    public TMP_Text solutionText;
    private SolutionSaver solutionSaver;
    private AutoShuffle resolve;

    // Start is called before the first frame update
    void Start()
    {
        solutionSaver = new SolutionSaver();
        resolve = FindObjectOfType<AutoShuffle>();

        // Add a click event to the button
        solveButton.onClick.AddListener(OnSolveButtonClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnSolveButtonClick()
    {
        FindObjectOfType<ReadCubeFaces>().ReadStateAndSaveToFile();

        string startState = ReadCubeStateFromFile("cube_data.txt");
        UDebug.Log(startState);

        List<string> shuffleMoves = AutoShuffle.initialMoveList;
        UpdateTextPanel(stepsText, "Shuffle Steps", shuffleMoves);

        List<string> AStarSolution = SolveAndSaveSolution(startState, AStarSolver.AStarSearch, "a_star.txt");

        List<string> AStarBidSolution = SolveAndSaveSolution(startState, AStarBidSolver.AStarBidirectionalSearch, "a_star_bid.txt");

        List<string> IDAStarSolution = SolveAndSaveSolution(startState, IDAStarSolver.IDAStarSearch, "ida_star.txt");
        UpdateTextPanel(solutionText, "IDA* Solution", IDAStarSolution);
        StartCoroutine(ExecuteSolutionMoves(IDAStarSolution, 3.0f));
    }

    string ReadCubeStateFromFile(string filePath)
    {
        try
        {
            string state = File.ReadAllText(filePath).Trim();
            if (state.Length == 54)
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

    List<string> SolveAndSaveSolution(string startState, Func<string, List<string>> solveMethod, string filePath)
    {
        if (string.IsNullOrEmpty(startState)) return null;

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
        return solutionMoves;
    }

    private void UpdateTextPanel(TMP_Text textPanel, string title, List<string> moves)
    {
        if (moves == null || moves.Count == 0)
        {
            textPanel.text = $"{title}: No solution found!";
        }
        else
        {
            textPanel.text = $"{title}: {string.Join(", ", moves)}";
        }
    }

    private IEnumerator ExecuteSolutionMoves(List<string> solutionMoves, float delay)
    {
        if (solutionMoves != null)
        {
            foreach (string move in solutionMoves)
            {
                yield return new WaitForSeconds(delay);
                resolve.Move(move);
            }
        }
    }

    private static long GetStableMemoryUsage()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        return GC.GetTotalMemory(true);
    }
}
