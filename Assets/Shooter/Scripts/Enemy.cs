using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public int startingHealth = 100;
    public EnemyManager enemyManager;
    public float speed = 1f;

    private EnvironmentParameters EnvironmentParameters;
    private int CurrentHealth;
    private Vector3 StartPosition;

    public float randomRangeX_Pos = 0f;
    public float randomRangeX_Neg = 0f;
    public float randomRangeZ_Pos = 0f;
    public float randomRangeZ_Neg = 0f;

    public ShooterAgentBehaviour Agent;
    private NavMeshAgent navAgent;

    private void Start()
    {
        StartPosition = transform.localPosition;
        CurrentHealth = startingHealth;

        EnvironmentParameters = Academy.Instance.EnvironmentParameters;
        speed = EnvironmentParameters.GetWithDefault("enemySpeed", 1f);

        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = speed;

        Agent.OnEnvironmentReset += Respawn;
    }

    private void FixedUpdate()
    {
        navAgent.destination = Agent.transform.localPosition;
    }

    public void GetShot(int damage, ShooterAgentBehaviour shooter)
    {
        ApplyDamage(damage, shooter);
    }

    private void ApplyDamage(int damage, ShooterAgentBehaviour shooter)
    {
        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
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

    public void Respawn()
    {
        CurrentHealth = startingHealth;

        speed = EnvironmentParameters.GetWithDefault("enemySpeed", 1f);
        navAgent.speed = speed;

        transform.localPosition = new Vector3(UnityEngine.Random.Range(randomRangeX_Neg, randomRangeX_Pos), StartPosition.y, UnityEngine.Random.Range(randomRangeZ_Neg, randomRangeZ_Pos));
    }
}
