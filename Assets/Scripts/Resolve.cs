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

public class Resolve : MonoBehaviour
{
    public Button solveButton;

    // Start is called before the first frame update
    void Start()
    {
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

        // Simulate solving the cube using A* algorithm
        string startState = "GBYWWYBBRBGWYYWBYOORRBORYGRGBOGRRROOYRYYGWOOBWOWGBWGOO"; // Starea initiala

        List<string> solutionPath_A_star = A_star_Solver.A_star_search(startState); // A* solution

        //List<string> solutionPath_A_star = A_star_Solver.A_star_search(startState); // A* bidirectional solution

        //List<string> solutionPath_A_star = A_star_Solver.A_star_search(startState); // IDA* solution

        if (solutionPath_A_star != null)
        {
            UDebug.Log("Solution found!");
            foreach (string state in solutionPath_A_star)
            {
                UDebug.Log(state);
            }
        }
        else
        {
            UDebug.LogError("No solution found!");
        }
    }
}
