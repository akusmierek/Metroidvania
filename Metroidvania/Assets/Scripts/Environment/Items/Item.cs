﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;


public class Item : MonoBehaviour
{
    #region Inspector variables

    [SerializeField, MustBeAssigned] protected Rigidbody2D _rigidbody = null;
    [SerializeField, MustBeAssigned] private Collider2D _collider = null;
    [SerializeField, MustBeAssigned] private GameObject destroyEffect = null;
    [SerializeField, MustBeAssigned] private GameObject itemHighlight = null;
    [SerializeField] private int baseDamage = 10;
    [SerializeField] private float knockbackForce = 100f;
    [SerializeField] private UnityEvent OnStartPulling = null;
    [SerializeField] private UnityEvent OnPullingCompleted = null;
    [SerializeField] private UnityEvent OnRelease = null;

    #endregion

    [HideInInspector] public bool isSpawned = false;

    #region Private variables

    private Transform itemHolder = null;
    private bool isPulling = false;
    protected bool isShooted = false;
    private float pullSpeedUp = 0f;
    private float maxPullSpeed = 0f;
    private float pullingTime = 1f;
    private float gravityScaleCopy = 0f;
    protected List<Collider2D> collidersToIgnore = new List<Collider2D>();

    #endregion


    public virtual void SetFree()
    {
        transform.SetParent( null );
        _rigidbody.simulated = true;
        OnRelease.Invoke();
    }


    public virtual void StartPulling( Transform t, float s, float ms )
    {
        itemHolder = t;
        pullSpeedUp = s;
        maxPullSpeed = ms;

        isPulling = true;

        OnStartPulling.Invoke();

        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody.gravityScale = 0f;

        _collider.enabled = false;
    }


    public virtual void AbortPulling()
    {
        PlayerState.instance.isPullingItemState = false;
        _rigidbody.gravityScale = gravityScaleCopy;
        _collider.enabled = true;
        gameObject.layer = LayerMask.NameToLayer( "Items" );
        isPulling = false;
    }


    public virtual void Shoot( Vector2 direction, float power )
    {
        isShooted = true;

        _rigidbody.AddForce( direction * power, ForceMode2D.Impulse );

        _collider.enabled = true;
        _collider.isTrigger = true;

        gameObject.layer = LayerMask.NameToLayer( "ShootItem" );

        Collider2D[] overlapedColliders = Physics2D.OverlapCircleAll( transform.position, 0.5f );
        foreach ( Collider2D collider in overlapedColliders )
        {
            if ( !collider.CompareTag( "Enemy" ) )
            {
                collidersToIgnore.Add( collider );
            }
        }
    }


    public virtual void OnHover( bool start )
    {
        itemHighlight.SetActive( start );
    }


    protected virtual void Awake()
    {
        gravityScaleCopy = _rigidbody.gravityScale;
    }


    protected virtual void Update()
    {
        if ( isPulling )
        {
            Pull();
        }
    }


    protected virtual void OnTriggerEnter2D( Collider2D collider )
    {
        GameObject go = collider.gameObject;

        if ( go.CompareTag( "Enemy" ) )
        {
            go.GetComponent<HitManager>().TakeHit( baseDamage, _rigidbody.velocity.normalized, knockbackForce );
        }
        else if ( go.CompareTag( "DestroyablePlanks" ) )
        {
            go.GetComponent<CustomDestroy>().Destroy( _rigidbody.velocity, transform.position );
        }
    }


    protected virtual void Pull()
    {
        pullingTime += Time.deltaTime;

        UpdateVelocity();

        if ( IsItemOnPlace() )
        {
            FinishPulling();
        }
    }


    protected virtual void UpdateVelocity()
    {
        Vector2 direction = itemHolder.position - transform.position;
        direction.Normalize();

        float speed = _rigidbody.velocity.magnitude;

        if ( speed < maxPullSpeed )
        {
            speed += pullSpeedUp * pullingTime;
        }

        _rigidbody.velocity = direction * speed;
    }


    protected virtual bool IsItemOnPlace()
    {
        float distance = Vector2.Distance( transform.position, itemHolder.position );

        return distance < 1f;
    }


    protected virtual void FinishPulling()
    {
        PlayerState.instance.isHoldingItemState = true;

        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        _rigidbody.simulated = false;

        transform.SetParent( itemHolder );
        transform.position = itemHolder.position;

        pullingTime = 1f;

        OnPullingCompleted.Invoke();

        AbortPulling();
    }


    protected virtual void CustomDestroy()
    {
        Transform effect = Instantiate( destroyEffect, transform.position, transform.rotation ).transform;
        float angle = Mathf.Atan2( _rigidbody.velocity.y, _rigidbody.velocity.x ) * Mathf.Rad2Deg;
        effect.rotation = Quaternion.AngleAxis( angle, Vector3.forward );
        Destroy( gameObject );
    }
}
