﻿using UnityEngine;
using MyBox;

public class AutoColor : MonoBehaviour
{
    [Separator( "Fade In" )]
    [SerializeField] private bool fadeIn = false;
    [SerializeField, ConditionalField( nameof( fadeIn ) )] private float fadeInTime = 1f;
    [SerializeField, ConditionalField( nameof( fadeIn ) )] private float minFadeInA = 0f;
    [SerializeField, ConditionalField( nameof( fadeIn ) )] private float maxFadeInA = 1f;

    [Separator( "Fade Out" )]
    [SerializeField] private bool fadeOut = false;
    [SerializeField, ConditionalField( nameof( fadeOut ) )] private float fadeOutTime = 1f;
    [SerializeField, ConditionalField( nameof( fadeOut ) )] private float minFadeOutA = 0f;
    [SerializeField, ConditionalField( nameof( fadeOut ) )] private float maxFadeOutA = 1f;

    [Separator( "Bools" )]
    [SerializeField] private bool fadeOnStart = false;
    [SerializeField] private bool destroyOnEnd = false;


    private SpriteRenderer _renderer;


    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();

        if ( fadeOnStart )
        {
            DoFade();
        }
    }


    [ButtonMethod]
    public void DoFade()
    {
        if ( fadeIn )
        {
            FadeIn();
        }
        else if ( fadeOut )
        {
            FadeOut();
        }
    }


    private void FadeIn()
    {
        Color rendererColor = _renderer.color;

        LTDescr tween = LeanTween.value( gameObject, minFadeInA, maxFadeInA, fadeInTime )
            .setOnUpdate( ( float v ) => { _renderer.color = new Color( rendererColor.r, rendererColor.g, rendererColor.b, v ); } );

        if ( fadeOut )
        {
            tween.setOnComplete( () => { FadeOut(); } );
        }
        else if ( destroyOnEnd )
        {
            tween.setOnComplete( () => { Destroy( gameObject ); } ); 
        }
    }


    private void FadeOut()
    {
        Color rendererColor = _renderer.color;

        LTDescr tween = LeanTween.value( gameObject, maxFadeOutA, minFadeOutA, fadeOutTime )
            .setOnUpdate( ( float v ) => { _renderer.color = new Color( rendererColor.r, rendererColor.g, rendererColor.b, v ); } );

        if ( destroyOnEnd )
        {
            tween.setOnComplete( () => { Destroy( gameObject ); } );
        }
    }
}