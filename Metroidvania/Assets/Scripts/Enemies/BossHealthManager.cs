﻿using System.Collections;
using UnityEngine;
using Cinemachine;


public class BossHealthManager : HealthManager
{
    [SerializeField] private float normalBrightness = 0.2f;
    [SerializeField] private float hitBrightness = 1f;
    [SerializeField] private float brightnessChangeTime = 0.5f;
    [SerializeField] private float deathTime = 3f;
    [SerializeField] private Transform pointsDropPosition = null;
    [SerializeField] private SpriteRenderer sprite = null;
    [SerializeField] private GameObject collider = null;
    [SerializeField] private ParticleSystem[] deathParticles = null;
    [SerializeField] private GameObject eyesLight = null;
    [SerializeField] private GameObject firstExplosionOneShot = null;
    [SerializeField] private ParticleSystem[] firstExplosionLooping = null;
    [SerializeField] private GameObject secondExplosionOneShot = null;
    [SerializeField] private ParticleSystem[] secondExplosionLooping = null;
    [SerializeField] private GameObject thirdExplosionOneShot = null;
    [SerializeField] private ParticleSystem[] thirdExplosionLooping = null;
    [SerializeField] private GameObject lastExplosionForceField = null;
    [SerializeField] private CinemachineImpulseSource LastExplosionImpulse = null;

    [Header("Death Light")]
    [SerializeField] private UnityEngine.Experimental.Rendering.LWRP.Light2D deathLight = null;
    [SerializeField] private float increaseTime = 0.1f;
    [SerializeField] private float decreaseTime = 1f;
    [SerializeField] private float minOuterRadius = 0f;
    [SerializeField] private float maxOuterRadius = 40f;
    [SerializeField] private float minIntensity = 0f;
    [SerializeField] private float maxIntensity = 20f;

    [Header("Armor Parts")]
    [SerializeField] private GameObject[] armorParts = null;
    [SerializeField] private int numOfParts = 12;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.7f;
    [SerializeField] private float shootForce = 100f;


    override public void ChangeColorOnDamage()
    {
        LeanTween.value( gameObject, hitBrightness, normalBrightness, brightnessChangeTime ).setOnUpdate( ( float v ) => { spriteRenderer.material.SetFloat( "_Brightness", v ); } );
    }


    public void Death()
    {
        OnDeath.Invoke();
        StartCoroutine( DeathCoroutine() );
        StartCoroutine( DropPoints() );
    }


    private IEnumerator DeathCoroutine()
    {
        eyesLight.SetActive( true );

        foreach ( ParticleSystem p in deathParticles )
        {
            p.Play();
        }

        LastExplosionImpulse.GenerateImpulse();

        yield return new WaitForSeconds( deathTime / 4f );

        firstExplosionOneShot.SetActive( true );

        foreach ( ParticleSystem p in firstExplosionLooping )
        {
            p.Play();
        }

        LastExplosionImpulse.GenerateImpulse();

        yield return new WaitForSeconds( deathTime / 4f );

        secondExplosionOneShot.SetActive( true );

        foreach ( ParticleSystem p in secondExplosionLooping )
        {
            p.Play();
        }

        LastExplosionImpulse.GenerateImpulse();

        yield return new WaitForSeconds( deathTime / 4f );

        thirdExplosionOneShot.SetActive( true );

        foreach ( ParticleSystem p in thirdExplosionLooping )
        {
            p.Play();
        }

        LastExplosionImpulse.GenerateImpulse();

        yield return new WaitForSeconds( deathTime / 4f );

        LeanTween.value( minOuterRadius, maxOuterRadius, increaseTime ).setOnUpdate( ( float v ) => { deathLight.pointLightOuterRadius = v; } );
        LeanTween.value( minIntensity, maxIntensity, increaseTime ).setOnUpdate( ( float v ) => { deathLight.intensity = v; } ).setOnComplete( () => { LeanTween.value( maxIntensity, minIntensity, decreaseTime ).setOnUpdate( ( float v ) => { deathLight.intensity = v; } ); } );

        foreach ( ParticleSystem p in deathParticles )
        {
            p.Stop();
        }

        foreach ( ParticleSystem p in firstExplosionLooping )
        {
            p.Stop();
        }

        foreach ( ParticleSystem p in secondExplosionLooping )
        {
            p.Stop();
        }

        foreach ( ParticleSystem p in thirdExplosionLooping )
        {
            p.Stop();
        }

        LastExplosionImpulse.GenerateImpulse();

        eyesLight.SetActive( false );
        firstExplosionOneShot.SetActive( false );
        secondExplosionOneShot.SetActive( false );
        thirdExplosionOneShot.SetActive( false );
        lastExplosionForceField.SetActive( true );

        sprite.enabled = false;
        collider.SetActive( false );

        for ( int i = 0; i < numOfParts; i++ )
        {
            GameObject objectToInstantiate = armorParts[ Random.Range( 0, armorParts.Length ) ];
            GameObject instantiated = Instantiate( objectToInstantiate, pointsDropPosition.position, Quaternion.AngleAxis( Random.Range( 0f, 360f ), Vector3.forward ), null );
            float randomScale = Random.Range( minScale, maxScale );
            instantiated.transform.localScale = new Vector3( randomScale, randomScale, 1f );
            Vector2 direction = Random.insideUnitCircle;
            direction.y += 1.5f;
            instantiated.GetComponent<Rigidbody2D>().mass = randomScale;
            instantiated.GetComponent<Rigidbody2D>().AddTorque( Random.Range( 0f, 5f ), ForceMode2D.Impulse );
            instantiated.GetComponent<Rigidbody2D>().AddForce( direction * shootForce, ForceMode2D.Impulse );
        }
    }


    private IEnumerator DropPoints()
    {
        int count = Random.Range( minPoints, maxPoints );

        for ( int i = 0; i < count; i++ )
        {
            Transform inst = Instantiate( point, pointsDropPosition.position, transform.rotation );
            Vector2 dropForce = Random.insideUnitCircle * pointsDropForce;
            dropForce.y = Mathf.Abs( dropForce.y );
            inst.GetComponent<Rigidbody2D>().AddForce( dropForce, ForceMode2D.Impulse );
            yield return new WaitForSeconds( deathTime / count );
        }
    }


    override public void TakeDamage( int damage )
    {
        currentHP -= damage;
        ChangeColorOnDamage();
        OnTakeDamage.Invoke();
    }
}