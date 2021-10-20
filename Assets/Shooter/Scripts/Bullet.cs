using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{

    [SerializeField] private float m_bulletForce = 80f;

    private Rigidbody m_rb;
    private Vector3 m_dir;


    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        Destroy(this.gameObject, 0.25f);   
    }

    public void InitBullet(Vector3 inDir)
    {
        m_dir = inDir;
        m_rb.velocity = m_dir * m_bulletForce;
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }

    //void Update()
    //{
    //    m_rb.MovePosition(transform.position + Vector3.forward * m_bulletForce * Time.deltaTime);
    //}
}
