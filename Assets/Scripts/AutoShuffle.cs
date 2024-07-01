using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoShuffle : MonoBehaviour
{
    public static List<string> moveList = new List<string>() { };
    public static List<string> initialMoveList = new List<string>() { };
    private readonly List<string> _allMoves = new List<string>()
    {
        "U", "U'", "U2",
        "D", "D'", "D2",
        "F", "F'", "F2",
        "B", "B'", "B2",
        "L", "L'", "L2",
        "R", "R'", "R2",
    };
    private CubeFacesState _state;
    private ReadCubeFaces _read;

    void Start()
    {
        _state = FindObjectOfType<CubeFacesState>();
        _read = FindObjectOfType<ReadCubeFaces>();
    }

    void Update()
    {
        if (moveList.Count > 0 && !CubeFacesState.autoShuffle && CubeFacesState.started)
        {
            Move(moveList[0]);
            moveList.RemoveAt(0);
        }
    }

    public void Shuffle()
    {
        List<string> moves = new List<string>();
        int shuffleLen = Random.Range(6, 7);

        for (int i = 0; i < shuffleLen; ++i)
        {
            int randomMove = Random.Range(0, _allMoves.Count);
            moves.Add(_allMoves[randomMove]);
            Debug.Log( _allMoves[randomMove]);
        }

        moveList = moves;
        initialMoveList = new List<string>(moves);
    }

    public void SetState(CubeFacesState state)
    {
        _state = state;
    }

    public void Move(string move)
    {
        _read.ReadState();
        CubeFacesState.autoShuffle = true;

        if (move == "U")
        {
            RotateSide(_state.up, -90);
        }
        if (move == "U'")
        {
            RotateSide(_state.up, 90);
        }
        if (move == "U2")
        {
            RotateSide(_state.up, -180);
        }
        if (move == "D")
        {
            RotateSide(_state.down, -90);
        }
        if (move == "D'")
        {
            RotateSide(_state.down, 90);
        }
        if (move == "D2")
        {
            RotateSide(_state.down, -180);
        }
        if (move == "F")
        {
            RotateSide(_state.front, -90);
        }
        if (move == "F'")
        {
            RotateSide(_state.front, 90);
        }
        if (move == "F2")
        {
            RotateSide(_state.front, -180);
        }
        if (move == "B")
        {
            RotateSide(_state.back, -90);
        }
        if (move == "B'")
        {
            RotateSide(_state.back, 90);
        }
        if (move == "B2")
        {
            RotateSide(_state.back, -180);
        }
        if (move == "L")
        {
            RotateSide(_state.left, -90);
        }
        if (move == "L'")
        {
            RotateSide(_state.left, 90);
        }
        if (move == "L2")
        {
            RotateSide(_state.left, -180);
        }
        if (move == "R")
        {
            RotateSide(_state.right, -90);
        }
        if (move == "R'")
        {
            RotateSide(_state.right, 90);
        }
        if (move == "R2")
        {
            RotateSide(_state.right, -180);
        }
    }

    void RotateSide(List<GameObject> side, float angle)
    {
        RotationScript r = side[4].transform.parent.GetComponent<RotationScript>();
        r.StartAutoShuffle(side, angle);
    }
}
