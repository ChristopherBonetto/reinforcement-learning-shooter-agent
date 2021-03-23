using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Enemy[] enemies;
    public ShooterAgentBehaviour agent;

    private int EnemyCount;
    private EnvironmentParameters EnvironmentParameters;
    private int startingPoint = 0;

    private void Start()
    {
        EnvironmentParameters = Academy.Instance.EnvironmentParameters;
        EnemyCount = Mathf.FloorToInt(EnvironmentParameters.GetWithDefault("amountEnemies", 4f));

        SetEnemiesActive();
    }

    public bool isEveryEnemyDead()
    {
        int deathCounter = 0;

        for (int i = startingPoint; i < EnemyCount + startingPoint; i++)
        {
            if (!enemies[i].isActiveAndEnabled)
                deathCounter++;
        }

        return deathCounter >= EnemyCount;
    }

    public void RegisterDeath()
    {
        if (isEveryEnemyDead())
        {
            SetEnemiesActive();
            agent.EndEpisode();
        }
    }

    public void SetEnemiesActive()
    {
        int counter = 0;
        EnemyCount = Mathf.FloorToInt(EnvironmentParameters.GetWithDefault("amountEnemies", 4f));

        startingPoint = Mathf.FloorToInt(UnityEngine.Random.Range(0f, enemies.Length - EnemyCount));

        foreach (var enemy in enemies)
        {
            enemy.gameObject.SetActive(false);
        }

        for (int i = startingPoint; i < EnemyCount + startingPoint; i++)
        {
            counter++;
            enemies[i].gameObject.SetActive(true);
        }
    }
}
