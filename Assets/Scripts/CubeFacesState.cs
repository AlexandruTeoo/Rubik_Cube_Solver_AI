using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CubeFacesState : MonoBehaviour
{
    public List<GameObject> front = new List<GameObject>();
    public List<GameObject> back = new List<GameObject>();
    public List<GameObject> up = new List<GameObject>();
    public List<GameObject> down = new List<GameObject>();
    public List<GameObject> right = new List<GameObject>();
    public List<GameObject> left = new List<GameObject>();

    /*public List<char> up;
    public List<char> down;
    public List<char> front;
    public List<char> back;
    public List<char> left;
    public List<char> right;*/

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

    string GetSideString(List<GameObject> side)
    {
        string sideString = "";
        foreach (GameObject face in side)
        {
            sideString += face.name[0].ToString();
        }
        return sideString;
    }

    public string GetStateString()
    {
        string stateString = "";
        stateString += GetSideString(up);
        stateString += GetSideString(right);
        stateString += GetSideString(front);
        stateString += GetSideString(down);
        stateString += GetSideString(left);
        stateString += GetSideString(back);
        return stateString;
    }

    // Metod? pentru a returna starea cubului ca string
    /*public override string ToString()
    {
        return new string(up.ToArray()) + new string(right.ToArray()) + new string(front.ToArray()) +
               new string(left.ToArray()) + new string(back.ToArray()) + new string(down.ToArray());
    }*/
}
