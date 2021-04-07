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

    //Position values
    [SerializeField] private float randomRangeX_Pos = 0f;
    [SerializeField] private float randomRangeX_Neg = 0f;
    [SerializeField] private float randomRangeZ_Pos = 0f;
    [SerializeField] private float randomRangeZ_Neg = 0f;
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
        
        transform.localPosition = new Vector3(UnityEngine.Random.Range(randomRangeX_Neg, randomRangeX_Pos), startPosition.y, UnityEngine.Random.Range(randomRangeZ_Neg, randomRangeZ_Pos));
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
