using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cube2D : MonoBehaviour
{
    private CubeFacesState _state;

    public Transform up;
    public Transform down;
    public Transform front;
    public Transform back;
    public Transform left;
    public Transform right;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetColor()
    {
        _state = FindObjectOfType<CubeFacesState>();

        Update2DCube(_state.front, front);
        Update2DCube(_state.back, back);
        Update2DCube(_state.left, left);
        Update2DCube(_state.right, right);
        Update2DCube(_state.up, up);
        Update2DCube(_state.down, down);
    }

    void Update2DCube(List<GameObject> face, Transform side)
    {
        int i = 0;

        foreach (Transform s in side)
        {
            if (i < face.Count && face[i] != null)
            {
                if (face[i].name[0] == 'F')
                {
                    s.GetComponent<Image>().color = Color.green;
                }

                if (face[i].name[0] == 'B')
                {
                    s.GetComponent<Image>().color = Color.blue;
                }

                if (face[i].name[0] == 'U')
                {
                    s.GetComponent<Image>().color = Color.white;
                }

                if (face[i].name[0] == 'D')
                {
                    s.GetComponent<Image>().color = Color.yellow;
                }

                if (face[i].name[0] == 'R')
                {
                    s.GetComponent<Image>().color = Color.red;
                }

                if (face[i].name[0] == 'L')
                {
                    s.GetComponent<Image>().color = new Color(1, 0.5f, 0, 1);
                }
            }

            ++i;
        }
    }
}