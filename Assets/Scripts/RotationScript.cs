using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class RotationScript : MonoBehaviour
{
    private List<GameObject> _sideActivated;
    private Vector3 _localForward;
    private Vector3 _mouseRef;
    private bool _dragging = false;
    
    private float _sensitivity = 0.4f;
    private Vector3 _rotation;
    private float _rotationSpeed = 300f;
    private bool _automatic = false;
    
    private ReadCubeFaces _read;
    private CubeFacesState _state;
    private Quaternion _targetQ;
    
    // Start is called before the first frame update
    void Start()
    {
        _read = FindObjectOfType<ReadCubeFaces>();
        _state = FindObjectOfType<CubeFacesState>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_dragging && !_automatic)
        {
            RotateSide(_sideActivated);

            if (Input.GetMouseButtonUp(0))
            {
                _dragging = false;
                RotateToRight();
            }
        }

        if (_automatic)
        {
            RotateAutomatic();
        }
    }

    private void RotateSide(List<GameObject> side)
    {
        _rotation = Vector3.zero;

        Vector3 mouseOffset = (Input.mousePosition - _mouseRef);

        if (side == _state.front)
        {
            _rotation.x = (mouseOffset.x + mouseOffset.y) * _sensitivity * -1;
        }
        if (side == _state.back)
        {
            _rotation.x = (mouseOffset.x + mouseOffset.y) * _sensitivity * 1;
        }
        if (side == _state.up)
        {
            _rotation.y = (mouseOffset.x + mouseOffset.y) * _sensitivity * 1;
        }
        if (side == _state.down)
        {
            _rotation.y = (mouseOffset.x + mouseOffset.y) * _sensitivity * -1;
        }
        if (side == _state.left)
        {
            _rotation.z = (mouseOffset.x + mouseOffset.y) * _sensitivity * 1;
        }
        if (side == _state.right)
        {
            _rotation.z = (mouseOffset.x + mouseOffset.y) * _sensitivity * -1;
        }
        
        transform.Rotate(_rotation, Space.Self);

        _mouseRef = Input.mousePosition;
    }
    
    public void Rotate(List<GameObject> side)
    {
        _sideActivated = side;
        _mouseRef = Input.mousePosition;
        _dragging = true;
        _localForward = Vector3.zero - side[4].transform.parent.transform.localPosition;
    }

    public void StartAutoShuffle(List<GameObject> side, float angle)
    {
        _state.PickUp(side);

        Vector3 localForward = Vector3.zero - side[4].transform.parent.transform.localPosition;
        _targetQ = Quaternion.AngleAxis(angle, localForward) * transform.localRotation;
        _sideActivated = side;
        _automatic = true;
    }
    
    public void RotateToRight()
    {
        Vector3 v = transform.localEulerAngles;

        v.x = Mathf.Round(v.x / 90) * 90;
        v.y = Mathf.Round(v.y / 90) * 90;
        v.z = Mathf.Round(v.z / 90) * 90;

        _targetQ.eulerAngles = v;
        _automatic = true;
    }

    private void RotateAutomatic()
    {
        _dragging = false;
        var step = _rotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, _targetQ, step);

        if (Quaternion.Angle(transform.localRotation, _targetQ) <= 1)
        {
            transform.localRotation = _targetQ;
            
            _state.PutDown(_sideActivated, transform.parent);
            _read.ReadState();
            CubeFacesState.autoShuffle = false;
            _automatic = false;
            _dragging = false;
        }
    }
}
