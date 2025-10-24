using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCircle : MonoBehaviour
{
    public float rotateSpeed = 100f; 
    Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        RotateCircle();
        MoveWithCursor();
    }

    void RotateCircle()
    {
        transform.Rotate(Vector3.back, rotateSpeed * Time.deltaTime);
    }

    void MoveWithCursor()
    {
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        transform.position = mouseWorldPos;
    }
}
