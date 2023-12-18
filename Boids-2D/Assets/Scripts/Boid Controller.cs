using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    public GameObject cameraController;
    public GameObject boidManager;
    public float speed;
    public Vector3 velocity;
    public bool isSeperating;
    public bool isAligning;
    public bool isCohesive;
    public List<GameObject> nearbyBoids;
    public float viewingAngle;
    public float viewableRadius;
    public float protectableRadius;
    public List<float> nearbyBoidAngles;
    public float avoidFactor;
    
    //GIZMOS!!!!!

    public bool drawSphere;
    public bool drawViewingAngles;
    public bool drawToNearbyBoids;

    void Start()
    {
        //Sets the boids rotation along the z axis to a random float between 0 and 360
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Random.Range(0,360));

        SetRandomPosition();
        SetRandomColor();
        velocity = CalculateVelocity();
    }

    void FixedUpdate()
    {
        UpdateBoidPosition();
    }

    void UpdateBoidPosition()
    {
        transform.position += velocity * speed * Time.deltaTime;

        if(isSeperating || isAligning || isCohesive)
        {
            UpdateNearbyBoids();
        }

        if(isSeperating)
        {
            for(int i = 0; i < nearbyBoids.Count;i++)
            {
                Vector3 directionToNearbyBoid = nearbyBoids[i].transform.position - transform.position;

                //Finds the float value of the distance in position between the potential nearby boid and the current boid.
                float distanceToNearbyBoid = directionToNearbyBoid.magnitude;

                //Checks if the distanceToNearbyBoid is within the viewableRadius.
                if(distanceToNearbyBoid <= protectableRadius)
                {
                    velocity += directionToNearbyBoid * avoidFactor;
                }
            }
        }
    }

    void UpdateNearbyBoids()
    {
        //Clears the lists to account for any potential duplicate values
        nearbyBoids.Clear();
        nearbyBoidAngles.Clear();

        //Loops through all the boids in the scene to find which ones are inside the viewing angle and the viewing radius
        foreach (GameObject boid in boidManager.GetComponent<BoidManager>().getAllBoids())
        {
            //Checks that the potential nearby boid isn't the main boid.
            if(boid.transform != transform)
            {
                //Finds the difference in position between the potential nearby boid and the current boid as a Vector3
                Vector3 directionToNearbyBoid = boid.transform.position - transform.position;

                //Finds the float value of the distance in position between the potential nearby boid and the current boid.
                float distanceToNearbyBoid = directionToNearbyBoid.magnitude;

                //Checks if the distanceToNearbyBoid is within the viewableRadius.
                if(distanceToNearbyBoid <= viewableRadius)
                {
                    //Finds the forward direction of the boid
                    Vector2 forwardDirection = Quaternion.Euler(0f, 0f, transform.eulerAngles.z) * Vector2.right;

                    //Finds the angle to the nearby boid in degrees.
                    float angleToNearbyBoid = Vector2.SignedAngle(forwardDirection, directionToNearbyBoid);

                    //Check if the angle to the nearby boid is less than half the viewing angle
                    if(Mathf.Abs(angleToNearbyBoid) <= viewingAngle / 2f)
                    {
                        nearbyBoids.Add(boid);
                        nearbyBoidAngles.Add(angleToNearbyBoid);
                    }
                }
            }
        }
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

    void SetRandomColor()
    {
        //Finds the sprite renderer which holds the color attribute for the boid
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        //Creates a random color and sets it as the boids color.
        spriteRenderer.color = new Color(Random.Range(0,30)/255f,Random.Range(30,160)/255f,1);
    }

    Vector3 CalculateVelocity()
    {
        //Finds the angle the boid is facing
        float angle = transform.eulerAngles.z;

        //Finds the direction the boid is facing as a Vector3
        Vector3 direction = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle),Mathf.Sin(Mathf.Deg2Rad * angle),0f);

        //Finds the velocity as a Vector3 by multiplying the direction by speed
        Vector3 velocity = speed * direction;

        return velocity;
    }

    public void SetSpeed(float newSpeed)
    {
        //Sets the boids speed to the new desired speed.
        speed = newSpeed;
    }

    public void SetIsSeperating(bool seperating)
    {
        isSeperating = seperating;
    }

    public void SetIsAligning(bool aligning)
    {
        isAligning = aligning;
    }

    public void SetIsCohesive(bool cohesive)
    {
        isCohesive = cohesive;
    }

    public void SetViewingAngle(float angle)
    {
        viewingAngle = angle;
    }

    public void SetViewableRadius(float radius)
    {
        viewableRadius = radius;
    }

    public void SetProtectableRadius(float radius)
    {
        protectableRadius = radius;
    }

    public void SetAvoidFactor(float factor)
    {
        avoidFactor = factor;
    }

    //GIZMOS!!!!!


    void OnDrawGizmosSelected()
    {
        if(drawSphere)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, viewableRadius);
        }

        if(drawViewingAngles)
        {
            DrawViewingAngle();
        }

        if(drawToNearbyBoids)
        {
            for(int i = 0; i < nearbyBoids.Count;i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, nearbyBoids[i].transform.position);
            }
        }
    }

    void DrawViewingAngle()
    {
        // Get the forward direction of the object based on the current rotation
        Vector2 objectForwardDirection = Quaternion.Euler(0f, 0f, transform.eulerAngles.z) * Vector2.right;

        // Draw the lines representing the edges of the viewing angle
        float halfViewingAngle = viewingAngle / 2f;
        Quaternion leftRayRotation = Quaternion.Euler(0f, 0f, -halfViewingAngle);
        Quaternion rightRayRotation = Quaternion.Euler(0f, 0f, halfViewingAngle);

        Vector2 leftRayDirection = leftRayRotation * objectForwardDirection;
        Vector2 rightRayDirection = rightRayRotation * objectForwardDirection;

        // Convert Vector2 to Vector3 before addition
        Vector3 leftRayEnd = transform.position + new Vector3(leftRayDirection.x, leftRayDirection.y, 0) * viewableRadius;
        Vector3 rightRayEnd = transform.position + new Vector3(rightRayDirection.x, rightRayDirection.y, 0) * viewableRadius;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, leftRayEnd);
        Gizmos.DrawLine(transform.position, rightRayEnd);
    }
}