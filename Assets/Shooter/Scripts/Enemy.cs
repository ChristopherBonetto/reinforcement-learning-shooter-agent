using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    //External arena ref
    private EnemyManager enemyManager;
    private Transform m_target;

    //Enemy general values
    [SerializeField] private int startingHealth = 100;
    private int currentHealth = 0;
    private float currentSpeed = 0;

    private Vector3 startPosition;

    //Components and other params
    private NavMeshAgent navAgent;
    private EnvironmentParameters m_envParams;

    #region Behaviour Cycle
    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        startPosition = transform.localPosition;
        m_envParams = Academy.Instance.EnvironmentParameters;
    }
    private void FixedUpdate()
    {
        navAgent.SetDestination(m_target.transform.position);

        if (Input.GetKeyDown(KeyCode.E))
        {
            GetShot(currentHealth, m_target.GetComponent<ShooterAgentBehaviour>());
        }
    }
    #endregion

    #region TakeDamage and Die
    public void GetShot(int damage, ShooterAgentBehaviour shooter)
    {
        ApplyDamage(damage, shooter);
    }

    private void ApplyDamage(int damage, ShooterAgentBehaviour shooter)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die(shooter);
        }
    }
    private void Die(ShooterAgentBehaviour shooter)
    {
        shooter.RegisterKill();

        gameObject.SetActive(false);

        enemyManager.RegisterDeath();
    }
    #endregion

    #region Respawn
    public void Respawn()
    {
        currentHealth = startingHealth;
        ResetEnvInfos();

        gameObject.SetActive(true);

        Vector3? foundLocation = FindFreeSpace(enemyManager.transform.localPosition, enemyManager.areaLenght);

        if(foundLocation != null)
        {
            transform.localPosition = foundLocation.Value;
        }
        else
        {
            Debug.LogError("Navmesh point not found");
            Die(m_target.GetComponent<ShooterAgentBehaviour>());
        }
    }

    public Vector3? FindFreeSpace(Vector3 inCenter, float inRadius)
    {
        Vector3 randomPos = UnityEngine.Random.insideUnitSphere * inRadius + inCenter;

        NavMeshHit hit;

        // from randomPos find a nearest point on NavMesh surface in range of maxDistance
        NavMesh.SamplePosition(randomPos, out hit, inRadius, NavMesh.AllAreas);

        return hit.position;
    }
    #endregion

    #region Refresh env information
    private void ResetEnvInfos()
    {
        currentSpeed = m_envParams.GetWithDefault("arenaParam_enemiesSpeed", 1f);
        navAgent.speed = currentSpeed;
    }
    #endregion

    #region Spawned enemy
    public void TakeArenaInfos(EnemyManager inEnemyManager, Transform inTarget)
    {
        enemyManager = inEnemyManager;
        m_target = inTarget;
        m_envParams = Academy.Instance.EnvironmentParameters;
    }
    #endregion
}
