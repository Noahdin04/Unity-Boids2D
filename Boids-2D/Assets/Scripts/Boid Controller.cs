using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    public GameObject cameraController;
    public GameObject boidManager;
    public Vector2 velocity;
    public float speed;
    public float direction;

    void Start()
    {
        SetRandomPosition();
        SetRandomRotation();
        SetRandomColor();

        speed = boidManager.GetComponent<BoidManager>().getSpeed();
        velocity = new Vector3(Mathf.Cos(direction * Mathf.Deg2Rad), Mathf.Sin(direction * Mathf.Deg2Rad), 0f) * speed;
    }

    void FixedUpdate()
    {
        UpdateBoidRotation();
        UpdateBoidPosition();
        UpdateBoidData();
    }

    void UpdateBoidPosition()
    {
        transform.position += new Vector3(velocity.x, velocity.y, 0) * Time.fixedDeltaTime;
    }

    void UpdateBoidRotation()
    {
        Vector2 differenceToNextPosition = velocity * Time.fixedDeltaTime;

        float directionToNewPosition = Mathf.Atan2(differenceToNextPosition.y, differenceToNextPosition.x) * Mathf.Rad2Deg;

        transform.eulerAngles = new Vector3(0,0,directionToNewPosition);
    }

    void UpdateBoidData()
    {
        direction = transform.eulerAngles.z;
        speed = velocity.magnitude;
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if(collider != null && cameraController != null)
        {
            if(collider == cameraController.GetComponent<BoxCollider2D>())
            {
                checkValidPosition();
            }
        }
    }

    void checkValidPosition()
    {
        //Grabs the Script component from the edge controller game object
        CameraController cc = cameraController.GetComponent<CameraController>();

        if(transform.position.x < cc.getLeftCameraBound())
        {
            transform.position = new Vector3(cc.getRightCameraBound(),transform.position.y,0f);
        }
        if(transform.position.x > cc.getRightCameraBound())
        {
            transform.position = new Vector3(cc.getLeftCameraBound(),transform.position.y,0f);
        }
        if(transform.position.y < cc.getBottomCameraBound())
        {
            transform.position = new Vector3(transform.position.x,cc.getTopCameraBound(),0f);
        }
        if(transform.position.y > cc.getTopCameraBound())
        {
            transform.position = new Vector3(transform.position.x,cc.getBottomCameraBound(),0f);
        }
    }

    void SetRandomPosition()
    {
        //Grabs the Script component from the edge controller game object
        CameraController cc = cameraController.GetComponent<CameraController>();

        //Finds a random float value between the x positions of the edges of the camera in world point.
        float randX = Random.Range(cc.getLeftCameraBound(),cc.getRightCameraBound());

        //Finds a random float value between the y positions of the edges of the camera in world point.
        float randY = Random.Range(cc.getBottomCameraBound(),cc.getTopCameraBound());

        //Uses the previous two values to change the position of the boid to match the random position withing the bounds of the camera.
        transform.position = new Vector3(randX,randY,0);
    }

    void SetRandomRotation()
    {
        float direction = Random.Range(-180f,180f);
        Vector3 newDirection = new Vector3(0,0,direction);
        transform.eulerAngles = newDirection;

        UpdateBoidData();
    }

    void SetRandomColor()
    {
        //Finds the sprite renderer which holds the color attribute for the boid
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        //Creates a random color and sets it as the boids color.
        spriteRenderer.color = new Color(Random.Range(0,30)/255f,Random.Range(30,160)/255f,1);
    }
}