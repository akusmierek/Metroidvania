﻿using UnityEngine;

public class Walker_Sight : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float minSpeed = 0.5f;
    [SerializeField] private int damage = 20;
    [SerializeField] private float sightRange = 5f;
    [SerializeField] private float boost = 1.5f;
    [SerializeField] private LayerMask playerLayerMask = 0;
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private EnemyHealthManager healthManager = null;

    private int direction = 1;
    private float timeWalkingTooSlow = 0f;
    private bool playerInRange = false;


    private void Update()
    {
        if (!healthManager.isBeingKnockbacked)
        {
            CheckPlayerInSight();
            Move();
        }
    }


    private void Move()
    {
        if (Mathf.Abs(_rigidbody.velocity.x) < minSpeed)
        {
            timeWalkingTooSlow += Time.fixedDeltaTime;
        }
        if (timeWalkingTooSlow > 0.1f)
        {
            ChangeDirection();
            timeWalkingTooSlow = 0f;
        }

        float s = playerInRange ? boost : 1f;
        s = s * speed * direction;
        _rigidbody.velocity = new Vector2(s, _rigidbody.velocity.y);
    }


    private void CheckPlayerInSight()
    {
        if (Physics2D.Raycast(transform.position, new Vector2(direction, 0f), sightRange, playerLayerMask))
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }
    }


    private void ChangeDirection()
    {
        direction = direction > 0 ? -1 : 1;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("StopMark"))
        {
            ChangeDirection();
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.collider.GetComponent<PlayerHealthManager>().TakeDamage(damage, transform.position.x);
        }
        else if (healthManager.isBeingKnockbacked && !collision.gameObject.CompareTag("Item"))
        {
            healthManager.isBeingKnockbacked = false;
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, new Vector2(direction * sightRange, 0f));
    }
}
