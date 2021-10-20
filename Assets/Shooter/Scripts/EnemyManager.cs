using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    #region Environment - variables
    [SerializeField] private ShooterAgentBehaviour agentRef = null;
    public ShooterAgentBehaviour AgentRef => agentRef;

    private EnvironmentParameters envParams;
    #endregion

    #region Enemies - variables
    //Enemies values
    [SerializeField] private GameObject enemyPrefab = null;
    public float areaLenght = 28f;
    public Vector3 EnemiesSpawnArea => new Vector3(areaLenght,0,areaLenght);
    private List<Enemy> spawnedEnemies = new List<Enemy>();
    private int startingIndex = 0;
    private int amountEnemies = 0;
    #endregion

    //--------------------------

    #region Behaviour Cycle
    private void OnEnable()
    {
        agentRef.OnEnvironmentReset += ResetEnvInfos;
    }
    private void OnDisable()
    {
        agentRef.OnEnvironmentReset -= ResetEnvInfos;
    }
    #endregion


    //-------------------------

    #region Environment - Methods
    private void ResetEnvInfos()
    {
        //Called when the environment change his informations (example the agent died).

        if(envParams == null)
        {
            envParams = Academy.Instance.EnvironmentParameters;
        }

        //Used to take the current enemies amount value from the environment params.
        //The environment use a curriculum pattern that change the amount of enemies value based on the agent's growth.
        //The section about this value on the curriculum is called as arenaParam_amountEnemies.
        amountEnemies = Mathf.FloorToInt(envParams.GetWithDefault("arenaParam_amountEnemies", 4f));

        SetEnemiesActive(amountEnemies);
    }
    #endregion

    #region Enemy death - check wave cleared
    public bool isEveryEnemyDead()
    {
        int deathCounter = 0;

        for (int i = startingIndex; i < amountEnemies + startingIndex; i++)
        {
            if (!spawnedEnemies[i].isActiveAndEnabled)
                deathCounter++;
        }

        return deathCounter >= amountEnemies;
    }

    public void RegisterDeath()
    {
        if (isEveryEnemyDead())
        {
            agentRef.WinWave();
        }
    }
    #endregion

    #region Enemies spawn
    public void SetEnemiesActive(int inValue)
    {
        //Used to initialize and spawn enemies when called the first time. 
        if(spawnedEnemies == null || spawnedEnemies.Count == 0)
        {
            SpawnEnemies(8);
        }

        int counter = 0;
        startingIndex = Mathf.FloorToInt(UnityEngine.Random.Range(0f, spawnedEnemies.Count - inValue));

        foreach (var enemy in spawnedEnemies)
        {
            enemy.gameObject.SetActive(false);
        }

        for (int i = startingIndex; i < inValue + startingIndex; i++)
        {
            counter++;
            spawnedEnemies[i].Respawn();
        }
    }

    public void SpawnEnemies(int inValue)
    {
        for (int i = 0; i < inValue; i++)
        {
            Enemy enemyRef = Instantiate(enemyPrefab, this.transform).GetComponent<Enemy>();
            spawnedEnemies.Add(enemyRef);
            enemyRef.TakeArenaInfos(this, AgentRef);
            enemyRef.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Utils
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, EnemiesSpawnArea);
    }
    #endregion
}
