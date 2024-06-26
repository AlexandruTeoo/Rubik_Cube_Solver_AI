using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System.IO;

public class ReadCubeFaces : MonoBehaviour
{
    public Transform up;
    public Transform down;
    public Transform front;
    public Transform back;
    public Transform left;
    public Transform right;
    public GameObject emptyGameObject;

    private int _layerMask = 1 << 6; // se gaseste pe pozitia a 6-a in lista de layere
    private CubeFacesState _state;
    private Cube2D _cube2D;

    private List<GameObject> _frontFace = new List<GameObject>();
    private List<GameObject> _backFace = new List<GameObject>();
    private List<GameObject> _upFace = new List<GameObject>();
    private List<GameObject> _downFace = new List<GameObject>();
    private List<GameObject> _leftFace = new List<GameObject>();
    private List<GameObject> _rightFace = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        SetFaceTransforms();
        _state = FindObjectOfType<CubeFacesState>();
        _cube2D = FindObjectOfType<Cube2D>();
        ReadState();
        CubeFacesState.started = true;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void ReadState()
    {
        _state = FindObjectOfType<CubeFacesState>();
        _cube2D = FindObjectOfType<Cube2D>();

        _state.up = ReadFace(_upFace, up);
        _state.down = ReadFace(_downFace, down);
        _state.left = ReadFace(_leftFace, left);
        _state.right = ReadFace(_rightFace, right);
        _state.front = ReadFace(_frontFace, front);
        _state.back = ReadFace(_backFace, back);

        _cube2D.SetColor();
    }

    void SetFaceTransforms()
    {
        _upFace = BuildFaces(up, new Vector3(90, 90, 0));
        _leftFace = BuildFaces(left, new Vector3(0, 180, 0));
        _frontFace = BuildFaces(front, new Vector3(0, 90, 0));
        _rightFace = BuildFaces(right, new Vector3(0, 0, 0));
        _backFace = BuildFaces(back, new Vector3(0, 270, 0));
        _downFace = BuildFaces(down, new Vector3(270, 90, 0));
    }

    List<GameObject> BuildFaces(Transform faceTransform, Vector3 direction)
    {
        int faceCount = 0;

        List<GameObject> faces = new List<GameObject>();

        /*
        |0|1|2|
        |3|4|5|
        |6|7|8|
        */

        int[] dx = { -1, 0, 1, -1, 0, 1, -1, 0, 1 };
        int[] dy = { 1, 1, 1, 0, 0, 0, -1, -1, -1 };

        for (int i = 0; i < 9; ++i)
        {
            Vector3 startPosition = new Vector3(faceTransform.localPosition.x + dx[i],
                                                faceTransform.localPosition.y + dy[i],
                                                  faceTransform.localPosition.z);

            GameObject faceStart = Instantiate(emptyGameObject, startPosition, Quaternion.identity, faceTransform);
            faceStart.name = faceCount.ToString();
            faces.Add(faceStart);
            ++faceCount;
        }

        faceTransform.localRotation = Quaternion.Euler(direction);

        return faces;
    }

    public List<GameObject> ReadFace(List<GameObject> faceStart, Transform faceTransform)
    {
        List<GameObject> faces = new List<GameObject>();

        foreach (GameObject f in faceStart)
        {
            Vector3 face = f.transform.position;
            RaycastHit hit;

            if (Physics.Raycast(face, faceTransform.forward, out hit, Mathf.Infinity, _layerMask))
            {
                Debug.DrawRay(face, faceTransform.forward * hit.distance, Color.yellow);
                faces.Add(hit.collider.gameObject);
            }
            else
            {
                Debug.DrawRay(face, faceTransform.forward * 1000, Color.green);
            }
        }

        return faces;
    }

    // metoda pentru a scrie datele �n fisierul "cube_data.txt"
    private void WriteCubeDataToFile()
    {
        using (StreamWriter writer = new StreamWriter("cube_data.txt"))
        {
            WriteFaceData(_upFace, up, writer);
            WriteFaceData(_leftFace, left, writer);
            WriteFaceData(_frontFace, front, writer);
            WriteFaceData(_rightFace, right, writer);
            WriteFaceData(_backFace, back, writer);
            WriteFaceData(_downFace, down, writer);
        }
    }

    // metoda pentru a citi culorile pentru fiecare fata a cubului si a le salva in fisier
    public void ReadStateAndSaveToFile()
    {
        _state = FindObjectOfType<CubeFacesState>();
        _cube2D = FindObjectOfType<Cube2D>();
        
        WriteCubeDataToFile(); // citirea culorilor si salvarea lor in fisier
        
        _cube2D.SetColor(); // actualizarea culorilor �n interfata Unity
    }

    private void WriteFaceData(List<GameObject> faceStart, Transform faceTransform, StreamWriter writer)
    {
        foreach (GameObject f in faceStart)
        {
            Vector3 face = f.transform.position;
            RaycastHit hit;

            if (Physics.Raycast(face, faceTransform.forward, out hit, Mathf.Infinity, _layerMask))
            {
                Debug.DrawRay(face, faceTransform.forward * hit.distance, Color.yellow);
                Color cubeColor = hit.collider.gameObject.GetComponent<Renderer>().material.color;
                writer.Write(GetColorCode(cubeColor));
            }
            else
            {
                Debug.DrawRay(face, faceTransform.forward * 1000, Color.green);
            }
        }
    }

    // functie pentru a obtine codul de culoare corespunzator culorii
    private string GetColorCode(Color color)
    {
        if (color == Color.white)
            return "W"; // Alb
        else if (color == new Color(1.0f, 0.922f, 0.016f, 1.0f))
            return "Y"; // Galben
        else if (color == Color.red)
            return "R"; // Rosu
        else if (color == new Color(1.0f, 0.5f, 0.0f, 1.0f))
            return "O"; // Portocaliu
        else if (color == Color.green)
            return "G"; // Verde
        else if (color == Color.blue)
            return "B"; // Albastru
        else
            return "X"; // Cod pentru o culoare necunoscuta
    }
}