using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public GameObject boidBlueprint;
    public GameObject boidParent;
    public int numBoids;
    public float speed;

    public float viewingAngle;
    public float viewableRadius;
    public float protectableRadius;
    public float avoidFactor;
    public bool isSeperating;
    public bool isAligning;
    public bool isCohesive;

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
            boid.GetComponent<BoidController>().SetSpeed(speed);
            boid.GetComponent<BoidController>().SetViewingAngle(viewingAngle);
            boid.GetComponent<BoidController>().SetViewableRadius(viewableRadius);
            boid.GetComponent<BoidController>().SetProtectableRadius(protectableRadius);
            boid.GetComponent<BoidController>().SetIsSeperating(isSeperating);
            boid.GetComponent<BoidController>().SetIsAligning(isAligning);
            boid.GetComponent<BoidController>().SetIsCohesive(isCohesive);
            boid.GetComponent<BoidController>().SetAvoidFactor(avoidFactor);

            allBoids.Add(boid);
        }
    }

    public List<GameObject> getAllBoids()
    {
        return allBoids;
    }
}