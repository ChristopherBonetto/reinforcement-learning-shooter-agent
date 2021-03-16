using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int m_startingHealth = 100;
    private int m_currentHealth;

    private Vector3 m_startingPos;

    public void GetShot(int inDamage, ShootingAgent inShooter)
    {
        ApplyDamage(inDamage, inShooter);
    }

    private void ApplyDamage(int inDamage, ShootingAgent inShooter)
    {
        m_currentHealth -= inDamage;

        if(m_currentHealth <= 0)
        {
            Die(inShooter);
        }
    }

    private void Die(ShootingAgent inShooter)
    {
        inShooter.RegisterKill();
        Debug.Log(message:"died");
        Respawn();
    }

    private void Respawn()
    {
        transform.position = m_startingPos;
        m_currentHealth = m_startingHealth;
    }


    // Start is called before the first frame update
    void Start()
    {
        m_startingPos = transform.position;
        m_currentHealth = m_startingHealth;
    }


    //private void OnMouseDown()
    //{
    //    GetShot(30);
    //}
}
