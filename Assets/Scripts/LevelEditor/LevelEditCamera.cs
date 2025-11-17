using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelEditCamera : MonoBehaviour
{
    Vector3 startPos;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float zoomSpeed = 10f;
    [SerializeField] float zoomOuterBound = 300f;
    [SerializeField] float zoomInnerBound = 10f;
    Vector3 mouseStartPos = Vector3.zero;
    Camera myCam;

    public bool invertDrag = false;

    private void Start()
    {
        startPos = transform.position;
        myCam = Camera.main;
    }

    void Update()
    {
        MouseMovement();
        ZoomHandler();
        //ReturnToStart();
    }

    private void MouseMovement()
    {
        if (Input.GetMouseButtonDown((int)MouseButton.Middle))
        {
            mouseStartPos = Input.mousePosition;
        }

        if (Input.GetMouseButton((int)MouseButton.Middle))
        {
            Vector3 dir = (mouseStartPos - Input.mousePosition);

            Vector3 newPos = transform.position;

            int invertCoefficent = 1;
            if(invertDrag)
            {
                invertCoefficent = -1;
            }
            newPos.x += dir.x * moveSpeed * Time.deltaTime * invertCoefficent;
            newPos.y += dir.y * moveSpeed * Time.deltaTime * invertCoefficent;

            transform.position = newPos;
        }
    }

    private void ZoomHandler()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            float targetSize = myCam.orthographicSize - zoomSpeed;
            targetSize = Mathf.Clamp(targetSize, zoomInnerBound, zoomOuterBound);
            myCam.orthographicSize = targetSize;
        }
        else if(Input.mouseScrollDelta.y < 0)
        {
            float targetSize = myCam.orthographicSize + zoomSpeed;
            targetSize = Mathf.Clamp(targetSize, zoomInnerBound, zoomOuterBound);
            myCam.orthographicSize = targetSize;
        }
    }

    private void ReturnToStart()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.position = startPos;
        }
    }
}
