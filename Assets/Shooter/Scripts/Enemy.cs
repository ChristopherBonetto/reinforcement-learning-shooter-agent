using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    #region External ref - variables
    private EnemyManager enemyManager = null;
    private ShooterAgentBehaviour target = null;
    private Tuple<float,Vector3> targetLastPosition;
    private EnvironmentParameters m_envParams;
    #endregion

    #region Enemy Behaviour - variables
    [SerializeField] private int startingHealth = 100;
    [SerializeField] private float targetLastPosDelay = 0.5f;
    private int currentHealth = 0;
    private float currentSpeed = 0f;
    #endregion

    #region Components - variables
    private NavMeshAgent navAgent;
    #endregion

    //--------------------------------------

    #region Behaviour Cycle
    private void Awake()
    {
        //Take components
        navAgent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        //Take the environment's parameters
        m_envParams = Academy.Instance.EnvironmentParameters;
    }
    private void FixedUpdate()
    {
        //If the enemy can go to the target set his destination.
        CheckTargetPosition();
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

        //Try to found a free space to respawn
        Vector3? foundLocation = FindFreeSpace(enemyManager.transform.localPosition, enemyManager.areaLenght);

        if(foundLocation != null)
        {
            if(foundLocation.Value != Vector3.positiveInfinity || foundLocation.Value != Vector3.negativeInfinity)
            {
                transform.localPosition = foundLocation.Value;
            }
        }
        else
        {
            Debug.LogError("Navmesh point not found");
            Die(target);
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
    
    private void CheckTargetPosition()
    {
        //Used in update to refresh the target position and tell to the agent component the new destination 

        if (navAgent == null || !navAgent.isOnNavMesh) return;

        if(target != null)
        {
            if (target != enemyManager.AgentRef)
                target = enemyManager.AgentRef;

            if (targetLastPosition == null) return;
            if (Time.time < targetLastPosition.Item1 + targetLastPosDelay) return;

            navAgent.SetDestination(targetLastPosition.Item2);
            targetLastPosition = new Tuple<float, Vector3>(Time.time, target.transform.position);
        }
    }
    
    private void ResetEnvInfos()
    {
        //Used to take the current speed value from the environment params.
        //The environment use a curriculum pattern that change the speed value based on the agent's growth.
        //The section about this value on the curriculum is called as arenaParam_enemiesSpeed.

        currentSpeed = m_envParams.GetWithDefault("arenaParam_enemiesSpeed", 1f);
        navAgent.speed = currentSpeed;
    }
    #endregion

    #region Spawned enemy
    public void TakeArenaInfos(EnemyManager inEnemyManager, ShooterAgentBehaviour inTarget)
    {
        //Starting values taken then the enemy was spawned for the first time.

        enemyManager = inEnemyManager;
        target = inTarget;
        targetLastPosition = new Tuple<float, Vector3>(Time.time,target.transform.position);
        m_envParams = Academy.Instance.EnvironmentParameters;
    }
    #endregion
}
