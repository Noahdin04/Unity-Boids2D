using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public GameObject boidBlueprint;
    public GameObject boidParent;
    public List<GameObject> boids = new List<GameObject>();
    public int numBoids;
    public float speed;
    public float minimumSpeed;
    public float maximumSpeed;
    public bool separate;
    public float protectionRadius;
    public float separationStrength;

    public List<GameObject> allBoids = new List<GameObject>();

    void Start()
    {
        spawnBoids();
    }

    void spawnBoids()
    {
        for(int i = 0; i < numBoids; i++)
        {
            GameObject boid = Instantiate(boidBlueprint, boidParent.transform);
            boid.SetActive(true);
            boids.Add(boid);
        }
    }
    public List<GameObject> GetBoids()
    {
        return boids;
    }
    public float GetSpeed()
    {
        return speed;
    }
    public float GetMinSpeed()
    {
        return minimumSpeed;
    }
    public float GetMaxSpeed()
    {
        return maximumSpeed;
    }

    public bool GetIsSeparating()
    {
        return separate;
    }
    public float GetProtectionRadius()
    {
        return protectionRadius;
    }
    public float GetSeparationStrength()
    {
        return separationStrength;
    }
}