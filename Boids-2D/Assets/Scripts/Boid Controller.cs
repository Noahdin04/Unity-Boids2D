using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.UIElements;

public class BoidController : MonoBehaviour
{
    public GameObject cameraController;
    public GameObject boidManager;
    public Vector2 velocity;
    public float speed;
    public float minimumSpeed;
    public float maximumSpeed;
    public float direction;
    public List<GameObject> boidsInProtectionRadius = new List<GameObject>();
    public List<GameObject> boidsInViewingRadius = new List<GameObject>();

    void Start()
    {
        SetRandomPosition();
        SetRandomRotation();
        SetRandomColor();

        minimumSpeed = boidManager.GetComponent<BoidManager>().GetMinSpeed();
        maximumSpeed = boidManager.GetComponent<BoidManager>().GetMaxSpeed();

        speed = boidManager.GetComponent<BoidManager>().GetSpeed();
        velocity = new Vector3(Mathf.Cos(direction * Mathf.Deg2Rad), Mathf.Sin(direction * Mathf.Deg2Rad), 0f) * speed;
    }

    void FixedUpdate()
    {
        UpdateBoidLists();
        checkValidPosition();
        UpdateBoidPosition();
        UpdateBoidRotation();
        CheckBoidSpeed();
        UpdateBoidData();

        if(boidManager.GetComponent<BoidManager>().GetAvoidWalls())
        {
            AvoidWalls();
        }

        if(boidManager.GetComponent<BoidManager>().GetIsSeparating())
        {
            Seperate();
        }

        if(boidManager.GetComponent<BoidManager>().GetIsCentering())
        {
            Center();
        }

        if(boidManager.GetComponent<BoidManager>().GetIsAligning())
        {
            Align();
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
    void CheckBoidSpeed()
    {
        if(speed > maximumSpeed)
        {
            velocity = velocity.normalized * maximumSpeed;
        }

        if(speed < minimumSpeed)
        {
            velocity = velocity.normalized * minimumSpeed;
        }
    }

    void UpdateBoidData()
    {
        direction = transform.eulerAngles.z;
        speed = velocity.magnitude;
    }

    void AvoidWalls()
    {
        //Grabs the Script component from the edge controller game object
        CameraController cc = cameraController.GetComponent<CameraController>();

        float protectionRadius = boidManager.GetComponent<BoidManager>().GetWallAvoidanceRadius();

        float seperateStrength = boidManager.GetComponent<BoidManager>().GetWallAvoidanceStrength();

        if(transform.position.x - protectionRadius < cc.getLeftCameraBound())
        {
            Vector2 leftWallPositionDifference = new Vector2(cc.getLeftCameraBound(), transform.position.y) - (Vector2)transform.position;

            float normalizedDistance = Mathf.Clamp01(1f - leftWallPositionDifference.magnitude / boidManager.GetComponent<BoidManager>().GetProtectionRadius());

            velocity -= leftWallPositionDifference * normalizedDistance * seperateStrength;
        }

        if(transform.position.x + protectionRadius > cc.getRightCameraBound())
        {
            Vector2 rightWallPositionDifference = new Vector2(cc.getRightCameraBound(), transform.position.y) - (Vector2)transform.position;

            float normalizedDistance = Mathf.Clamp01(1f - rightWallPositionDifference.magnitude / boidManager.GetComponent<BoidManager>().GetProtectionRadius());
            
            velocity -= rightWallPositionDifference * normalizedDistance * seperateStrength;
        }

        if(transform.position.y + protectionRadius > cc.getTopCameraBound())
        {
            Vector2 topWallPositionDifference = new Vector2(transform.position.x, cc.getTopCameraBound()) - (Vector2)transform.position;

            float normalizedDistance = Mathf.Clamp01(1f - topWallPositionDifference.magnitude / boidManager.GetComponent<BoidManager>().GetProtectionRadius());
            
            velocity -= topWallPositionDifference * normalizedDistance * seperateStrength;
        }

        if(transform.position.y - protectionRadius < cc.getBottomCameraBound())
        {
            Vector2 bottomWallPositionDifference = new Vector2(transform.position.x, cc.getBottomCameraBound()) - (Vector2)transform.position;

            float normalizedDistance = Mathf.Clamp01(1f - bottomWallPositionDifference.magnitude / boidManager.GetComponent<BoidManager>().GetProtectionRadius());
            
            velocity -= bottomWallPositionDifference * normalizedDistance * seperateStrength;
        }
    }
    void UpdateBoidLists()
    {
        boidsInProtectionRadius.Clear();
        boidsInViewingRadius.Clear();

        if(boidManager.GetComponent<BoidManager>().GetBoids() != null)
        {
            foreach(GameObject boid in boidManager.GetComponent<BoidManager>().GetBoids())
            {
                if(boid != gameObject)
                {
                    if((boid.transform.position - transform.position).magnitude <= boidManager.GetComponent<BoidManager>().GetViewableRadius())
                    {
                        boidsInViewingRadius.Add(boid);

                        if((boid.transform.position - transform.position).magnitude <= boidManager.GetComponent<BoidManager>().GetProtectionRadius())
                        {
                            boidsInProtectionRadius.Add(boid);
                        }
                    }
                }
            }
        }
    }
    void Seperate()
    {
        foreach(GameObject boid in boidsInProtectionRadius)
        {
            Vector2 boidPositionDifference;

            float normalizedDistance;

            float seperateStrength = boidManager.GetComponent<BoidManager>().GetSeparationStrength();

            boidPositionDifference = boid.transform.position - transform.position;

            normalizedDistance = Mathf.Clamp01(1f - boidPositionDifference.magnitude / boidManager.GetComponent<BoidManager>().GetProtectionRadius());

            velocity -= boidPositionDifference * normalizedDistance * seperateStrength;
        }
    }

    void Center()
    {
        if(boidsInViewingRadius.Count != 0)
        {
            Vector2 boidsCenter = new Vector2(0f,0f);

            float centerStrength = boidManager.GetComponent<BoidManager>().GetCenterStrength();

            foreach(GameObject boid in boidsInViewingRadius)
            {
                boidsCenter += (Vector2)boid.transform.position;
            }

            boidsCenter /= boidsInViewingRadius.Count;

            velocity += (boidsCenter - (Vector2)transform.position) * centerStrength;
        }
    }

    void Align()
    {
        if(boidsInViewingRadius.Count != 0)
        {
            Vector2 boidsVelocity = new Vector2(0f,0f);

            float alignStrength = boidManager.GetComponent<BoidManager>().GetCenterStrength();

            foreach(GameObject boid in boidsInViewingRadius)
            {
                boidsVelocity += boid.GetComponent<BoidController>().GetVelocity();
            }

            boidsVelocity /= boidsInViewingRadius.Count;

            velocity += (boidsVelocity - velocity) * alignStrength;
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

    void OnDrawGizmosSelected()
    {
        foreach(GameObject boid in boidsInProtectionRadius)
        {
            Gizmos.DrawLine(transform.position, boid.transform.position);
        }
    }

    public Vector2 GetVelocity()
    {
        return velocity;
    }
}