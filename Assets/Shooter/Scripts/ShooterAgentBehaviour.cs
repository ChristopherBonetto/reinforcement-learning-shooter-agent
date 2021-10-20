using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class ShooterAgentBehaviour : Agent
{
    #region Shoot - variables
    [Header("Shoot values")]
    [SerializeField] private Transform m_shootingPoint;
    [SerializeField] private int m_damage = 100;
    public int m_shootStepsDelay = 50;
    [SerializeField] private GameObject m_bulletObj = null;

    private bool m_canShoot = true;
    private int m_currentStepsToShoot = 0;
    #endregion

    #region Agent - variables
    [Header("Agent values"),Space]
    [SerializeField] private float m_movementSpeed = 3f;
    [SerializeField] private float rotationSpeed = 3f;
    private Vector3 m_startingPos;
    #endregion

    #region Score - variables
    [Header("Agent scores"),Space]
    [SerializeField] private TextMeshProUGUI m_killsText = null;
    private int m_killsCounter = 0;
    [SerializeField] private TextMeshProUGUI m_lostText = null;
    private int m_lostCounter = 0;
    [SerializeField] private TextMeshProUGUI m_wonText = null;
    private int m_wonCounter = 0;
    #endregion

    #region Environment - variables
    private EnvironmentParameters m_envParameters;
    public event Action OnEnvironmentReset;
    #endregion

    #region Component - variables
    private Rigidbody m_rb;
    #endregion

    #region Mono Cycle
    private void FixedUpdate()
    {
        if (!m_canShoot)
        {
            m_currentStepsToShoot--;

            if(m_currentStepsToShoot <= 0)
            {
                m_canShoot = true;
            }
        }
        
        AddReward(-1f / MaxStep);
    }
    #endregion


    #region ML overrided methods
    public override void Initialize()
    {
        m_startingPos = transform.localPosition;
        m_rb = GetComponent<Rigidbody>();
        m_rb.freezeRotation = true;
        
        m_envParameters = Academy.Instance.EnvironmentParameters;
    }

    public override void OnEpisodeBegin()
    {
        OnEnvironmentReset?.Invoke();

        m_shootStepsDelay = Mathf.FloorToInt(m_envParameters.GetWithDefault("shootingFrequenzy", 20f));

        transform.localPosition = m_startingPos;
        m_rb.velocity = Vector3.zero;
        m_canShoot = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localRotation.y);
        sensor.AddObservation(m_canShoot);
    }

    public override void Heuristic(float[] actionsOut)
    {
        //Used to take inputs
        actionsOut[0] = Input.GetKey(KeyCode.P) ? 1f : 0f;
        actionsOut[1] = Input.GetAxis("Horizontal");
        actionsOut[2] = Input.GetAxis("Vertical");
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        if (Mathf.RoundToInt(vectorAction[0]) >= 1)
        {
            Shoot();
        }

        float x = vectorAction[1] * m_movementSpeed;
        float y = vectorAction[2] * m_movementSpeed;

        Vector3 move = transform.forward * y;
        m_rb.velocity = new Vector3(move.x, m_rb.velocity.y, move.z);

        transform.Rotate(Vector3.up, vectorAction[1] * rotationSpeed);
    }
    #endregion

    #region Combat
    public void Shoot()
    {
        if (!m_canShoot)
            return;

        int layerMask = 1 << LayerMask.NameToLayer("Enemy");
        Vector3 direction = transform.forward;

        Instantiate(m_bulletObj, m_shootingPoint.position, this.transform.localRotation).TryGetComponent<Bullet>(out var bullet);
        bullet.InitBullet(direction);

        Debug.DrawRay(transform.position, direction, Color.blue, 1f);

        if (Physics.Raycast(m_shootingPoint.position, direction, out var hit, 200f, layerMask))
        {
            hit.transform.GetComponent<Enemy>().GetShot(m_damage, this);
        }
        else
        {
            AddReward(-0.033f);
        }

        m_canShoot = false;
        m_currentStepsToShoot = m_shootStepsDelay;
    }
    public void RegisterKill()
    {
        m_killsCounter++;
        m_killsText.text = "Kills: " + m_killsCounter.ToString();

        AddReward(1.0f / m_envParameters.GetWithDefault("amountEnemies", 1));
    }
    #endregion

    #region Death
    private void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            AddReward(-1f);

            m_lostCounter++;
            m_lostText.text = "Lost:" + m_lostCounter.ToString();  

            EndEpisode();
        }
    }
    #endregion

    #region Won

    public void WinWave()
    {
        this.EndEpisode();

        m_wonCounter++;
        m_wonText.text = "Won:" + m_wonCounter.ToString();
    }

    #endregion
}
