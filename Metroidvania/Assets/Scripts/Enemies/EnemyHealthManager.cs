﻿using UnityEngine;
using UnityEngine.Events;

public class EnemyHealthManager : MonoBehaviour
{
    private const float knockbackForce = 5000f;

    [SerializeField] private int initialHP = 100;
    [SerializeField] private bool canBeKnockbacked = true;
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private UnityEvent OnDeath = null;

    [HideInInspector] public bool isBeingKnockbacked = false;

    private int HP;


    private void Awake()
    {
        HP = initialHP;
    }


    private void Death()
    {
        Debug.Log("Enemy dead");
        OnDeath.Invoke();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Spikes"))
        {
            Death();
        }
    }


    public void Knockback(float xPos, float force = knockbackForce)
    {
        isBeingKnockbacked = true;

        _rigidbody.AddForce(new Vector2(0.5f * Mathf.Sign(transform.position.x - xPos), 1f) * force);
    }


    public void TakeDamage(int damage, float xPos)
    {
        HP -= damage;

        if (canBeKnockbacked)
        {
            Knockback(xPos);
        }

        if (HP < 0)
        {
            Death();
        }
    }
}