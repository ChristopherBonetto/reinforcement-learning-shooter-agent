using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private ShooterAgentBehaviour m_agent = null;

    private EnvironmentParameters m_envParams;

    //Enemies values
    [SerializeField] private GameObject m_enemyPrefab = null;
    public float areaLenght = 28f;
    public Vector3 EnemiesSpawnArea => new Vector3(areaLenght,0,areaLenght);
    private List<Enemy> m_enemies = new List<Enemy>();
    private int m_startingIndex = 0;
    private int m_amountEnemies = 0;

    #region Behaviour Cycle
    private void OnEnable()
    {
        m_agent.OnEnvironmentReset += ResetEnvInfos;
    }
    private void OnDisable()
    {
        m_agent.OnEnvironmentReset -= ResetEnvInfos;
    }
    #endregion

    private void ResetEnvInfos()
    {
        if(m_envParams == null)
        {
            m_envParams = Academy.Instance.EnvironmentParameters;
        }

        m_amountEnemies = Mathf.FloorToInt(m_envParams.GetWithDefault("arenaParam_amountEnemies", 4f));

        SetEnemiesActive(m_amountEnemies);
    }

    #region Enemy death - check wave cleared
    public bool isEveryEnemyDead()
    {
        int deathCounter = 0;

        for (int i = m_startingIndex; i < m_amountEnemies + m_startingIndex; i++)
        {
            if (!m_enemies[i].isActiveAndEnabled)
                deathCounter++;
        }

        return deathCounter >= m_amountEnemies;
    }

    public void RegisterDeath()
    {
        if (isEveryEnemyDead())
        {
            m_agent.WinWave();
        }
    }
    #endregion

    public void SetEnemiesActive(int inValue)
    {
        if(m_enemies == null || m_enemies.Count == 0)
        {
            SpawnEnemies(8);
        }

        int counter = 0;

        m_startingIndex = Mathf.FloorToInt(UnityEngine.Random.Range(0f, m_enemies.Count - inValue));

        foreach (var enemy in m_enemies)
        {
            enemy.gameObject.SetActive(false);
        }

        for (int i = m_startingIndex; i < inValue + m_startingIndex; i++)
        {
            counter++;
            m_enemies[i].Respawn();
        }
    }

    public void SpawnEnemies(int inValue)
    {
        for (int i = 0; i < inValue; i++)
        {
            Enemy enemyRef = Instantiate(m_enemyPrefab, this.transform).GetComponent<Enemy>();
            m_enemies.Add(enemyRef);
            enemyRef.TakeArenaInfos(this, m_agent.transform);
            enemyRef.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, EnemiesSpawnArea);
    }
}
