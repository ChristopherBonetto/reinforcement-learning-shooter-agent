using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System;

public class ShootingAgent : Agent
{
    [SerializeField] private Transform m_shootingPoint;
    [SerializeField] private int m_damage = 100;
    [SerializeField] private int m_shootStepsDelay = 50;

    private bool canShoot = true;
    private int m_currentTimeToShoot = 0;

    private Vector3 m_startingPos;
    private Rigidbody m_rb;


    #region Mono Cycle
    private void FixedUpdate()
    {
        if (!canShoot)
        {
            m_currentTimeToShoot--;

            if(m_currentTimeToShoot <= 0)
            {
                canShoot = true;
            }
        }
    }
    #endregion


    #region ML overrided methods
    public override void Initialize()
    {
        m_startingPos = transform.position;
        m_rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("Episode Begin");

        transform.position = m_startingPos;
        m_rb.velocity = Vector3.zero;
        canShoot = true;
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetKey(KeyCode.P) ? 1 : 0;
        Debug.Log("Heuristic" + actionsOut[0]);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        Debug.Log("Action received");

        if (Mathf.RoundToInt(vectorAction[0]) >= 1)
        {
            Debug.Log("Shoot");
            Shoot();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
    }
    #endregion

    #region Combat
    public void Shoot()
    {
        if (!canShoot)
            return;

        int layerMask = 1 << LayerMask.NameToLayer("Enemy");
        Vector3 direction = transform.forward;

        Debug.Log("SHOOT");
        Debug.DrawRay(m_shootingPoint.position, direction * 200f, Color.green, 2f);

        if (Physics.Raycast(m_shootingPoint.position, direction, out var hit, 200f, layerMask))
        {
            hit.transform.TryGetComponent<Enemy>(out var enemy);

            if (enemy)
                enemy.GetShot(m_damage, this);
        }

        canShoot = false;
        m_currentTimeToShoot = m_shootStepsDelay;
    }
    #endregion


    public void RegisterKill()
    {
        AddReward(1.0f);
        EndEpisode();
    }
}
