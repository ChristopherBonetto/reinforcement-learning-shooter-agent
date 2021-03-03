using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System;

public class PenguinAgent : Agent
{
    public PenguinArea penguinArea = null;

    public GameObject hearthPrefab;
    public GameObject regurgitatedFishPrefab;

    private GameObject baby;

    private bool isFull;

    RayPerceptionSensorComponent3D perceptionComponent;

    public override void OnActionReceived(float[] vectorAction)
    {
        float forward = vectorAction[0];
        float leftOrRight = 0f;

        if(vectorAction[1] == 1)
        {
            leftOrRight = -1f;
        }
        else if(vectorAction[1] == 2)
        {
            leftOrRight = 1f;
        }

        AddReward(-1 / MaxStep);
    }

    public override void OnEpisodeBegin()
    {
        isFull = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(isFull);

        sensor.AddObservation(Vector3.Distance(baby.transform.position, transform.position));

        sensor.AddObservation((baby.transform.position - transform.position).normalized);

        sensor.AddObservation(transform.forward);

        sensor.AddObservation(perceptionComponent);
    }

    public override void Initialize()
    {
        penguinArea = GetComponentInParent<PenguinArea>();
        baby = penguinArea.penguinBaby;
        perceptionComponent = GetComponent<RayPerceptionSensorComponent3D>();
    }

    public void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, baby.transform.position) < penguinArea.feedRadius)
        {
            RegurgitateFish();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("fish") && collision.gameObject.TryGetComponent<Fish>(out Fish fish))
        {
            EatFish(fish);
        }
        else if (collision.transform.CompareTag("baby"))
        {
            RegurgitateFish();
        }
    }

    private void EatFish(Fish fish)
    {
        if (isFull) return;

        isFull = true;

        penguinArea.RemoveSpecificFish(fish);

    }

    private void RegurgitateFish()
    {
        if (!isFull) return;

        isFull = false;

        GameObject regurgitatedFishObj = Instantiate(regurgitatedFishPrefab);
        regurgitatedFishObj.transform.parent = transform.parent;
        regurgitatedFishObj.transform.position = baby.transform.position;

        Destroy(regurgitatedFishObj, 4f);

        GameObject heart = Instantiate(hearthPrefab);
        heart.transform.parent = transform.parent;
        heart.transform.position = baby.transform.position + Vector3.up;

        Destroy(heart, 4f);

        AddReward(1f);
    }
}
