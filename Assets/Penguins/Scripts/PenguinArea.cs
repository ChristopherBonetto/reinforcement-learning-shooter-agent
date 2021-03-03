using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using TMPro;
using System;

public class PenguinArea : MonoBehaviour
{
    public PenguinAgent penguinAgent;
    public GameObject penguinBaby;
    public Fish fishPrefab;
    public TextMeshProUGUI cumulativeRewardText;

    public float fishSpeed { get { return fishSpeed; } private set { fishSpeed = value; } }
    public float feedRadius { get { return feedRadius; } private set { feedRadius = value; } }

    private List<Fish> fishList = new List<Fish>();

    public void Awake()
    {
        Academy.Instance.OnEnvironmentReset += EnvironmentReset;
    }

    private void EnvironmentReset()
    {
        RemoveAllFish();
        PlacePenguin();
        PlaceBaby();
        SpawFish(4, fishSpeed);
    }
    
    public void RemoveSpecificFish(Fish fishRef)
    {
        if (!fishList.Contains(fishRef)) return;

        fishList.Remove(fishRef);
    }

    public static Vector3 ChooseRandomPosition(Vector3 center, float minAngle, float maxAngle, float minRadius, float maxRadius)
    {
        float radius = minRadius;

        if(maxRadius > minRadius)
        {
            radius = UnityEngine.Random.Range(minRadius, maxRadius);
        }

        return center + Quaternion.Euler(0f, UnityEngine.Random.Range(minAngle, maxAngle), 0f) * Vector3.forward * radius;
    }

    private void SpawFish(int count, float fishSpeed)
    {
        for(int i = 0; i < count; i++)
        {
            Fish fish = Instantiate(fishPrefab.gameObject).GetComponent<Fish>();
            fish.transform.position = ChooseRandomPosition(transform.position, 100f, 260f, 2f, 13f) + Vector3.up * 0.5f;
            fish.transform.rotation = Quaternion.Euler(0f, 360f, 0f);
            fish.transform.parent = this.transform;
            fishList.Add(fish);
            //fish.fishSpeed = fishSpeed;
        }
    }

    private void PlaceBaby()
    {
        penguinBaby.transform.position = ChooseRandomPosition(transform.position, -45f, 45f, 4f, 9f) + Vector3.up * 0.5f;
        penguinBaby.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    private void PlacePenguin()
    {
        penguinAgent.transform.position = ChooseRandomPosition(transform.position, 0f, 360f, 0f, 9f) + Vector3.up * 0.5f;
        penguinAgent.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
    }

    private void RemoveAllFish()
    {
        if(fishList != null)
        {
            for(int i = 0; i < fishList.Count; i++)
            {
                if(fishList[i] != null)
                {
                    Destroy(fishList[i].gameObject);
                }
            }
        }

        fishList = new List<Fish>();
    }

    private void Update()
    {
        cumulativeRewardText.text = penguinAgent.GetCumulativeReward().ToString();
    }
}
