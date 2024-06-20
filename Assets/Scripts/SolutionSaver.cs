using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SolutionSaver
{
    public void SaveSolution(string filePath, List<string> solutionMoves, long timeTaken, long memoryUsed)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Steps to solve the cube:");
            if (solutionMoves != null)
            {
                foreach (var move in solutionMoves)
                {
                    writer.WriteLine(move);
                }
                writer.WriteLine();
                writer.WriteLine($"Time taken: {timeTaken} ms");
                writer.WriteLine($"Memory used: {memoryUsed / 1024} KB");
            }
            else
            {
                writer.WriteLine("No solution found.");
            }
        }
    }
}
