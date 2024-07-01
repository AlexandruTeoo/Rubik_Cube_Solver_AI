using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCube : MonoBehaviour
{
    private Vector2 firstPressPosition;
    private Vector2 secondPressPosition;
    private Vector2 currentSwipe;
    private Vector3 previousMousePosition;
    private Vector3 mouseDelta; // diferenta dintre pozitiile mouse-ului din punctul curent la punctul precedent 

    public GameObject target;
    private float speed = 200f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Swipe();
        Drag();
    }

    void Drag()
    {
        if (Input.GetMouseButton(1)) // daca click-ul este mentinut in pozitia apasat
        {
            mouseDelta += Input.mousePosition - previousMousePosition;
            mouseDelta *= 0.5f; // reducem viteza de rotire
            transform.rotation = Quaternion.Euler(mouseDelta.y, -mouseDelta.x, 0) * transform.rotation;
        }
        else
        {
            if (transform.rotation != target.transform.rotation)
            {
                var step = speed * Time.deltaTime;

                transform.rotation = Quaternion.RotateTowards(transform.rotation, target.transform.rotation, step);
            }
        }

        previousMousePosition = Input.mousePosition;
    }
    
    void Swipe()
    {
        if (Input.GetMouseButtonDown(1))
        {
            firstPressPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y); // iau pozitiile lui x si y din din sist.
                                                                                            // xOy la apasarea primului click
        }

        if (Input.GetMouseButtonUp(1))
        {
            secondPressPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y); // iau pozitiile lui x si y din din sist.
                                                                                            // xOy la apasarea celui de al doilea click
            currentSwipe = new Vector2(secondPressPosition.x - firstPressPosition.x, secondPressPosition.y - firstPressPosition.y); // vector cu diferenta dintre al doilea si primul click
            currentSwipe.Normalize();

            if (YLeftSwipe(currentSwipe))
            {
                target.transform.Rotate(0, 90, 0, Space.World);
            }
            else if (YRightSwipe(currentSwipe))
            {
                target.transform.Rotate(0, -90, 0, Space.World);
            }
            else if (XLeftSwipe(currentSwipe))
            {
                target.transform.Rotate(90, 0, 0, Space.World);
            }
            else if (ZRightSwipe(currentSwipe))
            {
                target.transform.Rotate(-90, 0, 0, Space.World);
            }
            else if (ZLeftSwipe(currentSwipe))
            {
                target.transform.Rotate(0, 0, 90, Space.World);
            }
            else if (XRightSwipe(currentSwipe))
            {
                target.transform.Rotate(0, 0, -90, Space.World);
            }
        }
    }

    bool YLeftSwipe(Vector2 swipe)
    {
        return swipe.x < 0 && swipe.y > -0.5f && swipe.y < 0.5f;
    }
    bool YRightSwipe(Vector2 swipe)
    {
        return swipe.x > 0 && swipe.y > -0.5f && swipe.y < 0.5f;
    }
    bool XLeftSwipe(Vector2 swipe)
    {
        return swipe.y > 0 && swipe.x < 0f;
    }
    bool XRightSwipe(Vector2 swipe)
    {
        return swipe.y > 0 && swipe.x > 0f;
    }
    bool ZLeftSwipe(Vector2 swipe)
    {
        return swipe.y < 0 && swipe.x < 0f;
    }
    bool ZRightSwipe(Vector2 swipe)
    {
        return swipe.y < 0 && swipe.x > 0f;
    }
}
