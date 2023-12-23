using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BoidManager : MonoBehaviour
{
    public GameObject boidBlueprint;
    public GameObject boidParent;
    public int numBoids;
    public float speed;

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
            allBoids.Add(boid);
        }
    }

    public float getSpeed()
    {
        return speed;
    }
}