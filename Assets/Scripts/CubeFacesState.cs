using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CubeFacesState
{
    /*public List<GameObject> front = new List<GameObject>();
    public List<GameObject> back = new List<GameObject>();
    public List<GameObject> up = new List<GameObject>();
    public List<GameObject> down = new List<GameObject>();
    public List<GameObject> right = new List<GameObject>();
    public List<GameObject> left = new List<GameObject>();*/

    public List<char> up;
    public List<char> down;
    public List<char> front;
    public List<char> back;
    public List<char> left;
    public List<char> right;

    public static bool autoShuffle = false;
    public static bool started = false;
    
    // Start is called before the first frame update 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickUp(List<GameObject> cubeSide)
    {
        foreach (GameObject cs in cubeSide)
        {
            if (cs != cubeSide[4])
            {
                cs.transform.parent.transform.parent = cubeSide[4].transform.parent;
            }
        } 
    }

    public void PutDown(List<GameObject> littleCubes, Transform pivot)
    {
        foreach (GameObject lc in littleCubes)
        {
            if (lc != littleCubes[4])
            {
                lc.transform.parent.transform.parent = pivot;
            }
        }
    }

    // Constructorul implicit
    public CubeFacesState()
    {
        // Ini?ializ?ri implicite
    }

    // Constructor care prime?te un string pentru a ini?ializa starea cubului
    public CubeFacesState(string state)
    {
        if (state.Length != 54)
        {
            throw new ArgumentException("State must be a string of length 54.");
        }

        // Ini?ializeaz? fiecare fa?? a cubului pe baza stringului dat
        up = state.Substring(0, 9).ToList();
        right = state.Substring(9, 9).ToList();
        front = state.Substring(18, 9).ToList();
        left = state.Substring(27, 9).ToList();
        back = state.Substring(36, 9).ToList();
        down = state.Substring(45, 9).ToList();
    }

    // Metod? pentru a returna starea cubului ca string
    public override string ToString()
    {
        return new string(up.ToArray()) + new string(right.ToArray()) + new string(front.ToArray()) +
               new string(left.ToArray()) + new string(back.ToArray()) + new string(down.ToArray());
    }
}
