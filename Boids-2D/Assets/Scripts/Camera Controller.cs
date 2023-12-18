using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;
    private BoxCollider2D cameraBounds;
    private float leftBoundPosition;
    private float rightBoundPosition;
    private float topBoundPosition;
    private float bottomBoundPosition;
    void Awake()
    {
        if(GetComponent<Camera>() != null)
        {
            cam = GetComponent<Camera>();
        }

        if(GetComponent<BoxCollider2D>() != null)
        {
            cameraBounds = GetComponent<BoxCollider2D>();
        }
    }

    void Start()
    {
        float cameraWidth = cam.orthographicSize * 2 * cam.aspect;
        float cameraHeight = cam.orthographicSize * 2;

        cameraBounds.size = new Vector2(cameraWidth, cameraHeight);

        CheckCameraBounds();
    }

    void Update()
    {
        CheckCameraBounds();
    }

    void CheckCameraBounds()
    {
        if(cam.orthographicSize * 2 != cameraBounds.size.y)
        {
            float cameraWidth = cam.orthographicSize * 2 * cam.aspect;
            float cameraHeight = cam.orthographicSize * 2;

            cameraBounds.size = new Vector2(cameraWidth, cameraHeight);
        }

        float halfCameraWidth = cam.orthographicSize * cam.aspect;
        leftBoundPosition = transform.position.x - halfCameraWidth;
        rightBoundPosition = transform.position.x + halfCameraWidth;
        topBoundPosition = transform.position.y + cam.orthographicSize;
        bottomBoundPosition = transform.position.y - cam.orthographicSize;
    }
    public Camera getCamera()
    {
        return cam;
    }
    public float getLeftCameraBound()
    {
        return leftBoundPosition;
    }
    public float getRightCameraBound()
    {
        return rightBoundPosition;
    }
    public float getTopCameraBound()
    {
        return topBoundPosition;
    }
    public float getBottomCameraBound()
    {
        return bottomBoundPosition;
    }
}
