 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectFace : MonoBehaviour
{
    private CubeFacesState _state;
    private ReadCubeFaces _read;
    
    private int _layerMask = 1 << 6;
    
    // Start is called before the first frame update
    void Start()
    {
        _read = FindObjectOfType<ReadCubeFaces>();
        _state = FindObjectOfType<CubeFacesState>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !CubeFacesState.autoShuffle)
        {
            _read.ReadState();

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.0f, _layerMask))
            {
                GameObject face = hit.collider.gameObject;

                List<List<GameObject>> cubeSides = new List<List<GameObject>>()
                {
                    _state.up,
                    _state.down,
                    _state.left,
                    _state.right,
                    _state.front,
                    _state.back
                };

                foreach (List<GameObject> cs in cubeSides)
                {
                    if (cs.Contains(face))
                    {
                        _state.PickUp(cs);
                        
                        cs[4].transform.parent.GetComponent<RotationScript>().Rotate(cs);
                    }
                }
            }

        }
    }
}
