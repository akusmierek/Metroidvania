﻿using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance = null;

    public bool isGamePaused = false;

    [SerializeField] private float slowmoTimeScale = 0.1f;
    [SerializeField] private float timeChangeDecreaseSpeed = 2f;
    [SerializeField] private float timeChangeIncreaseSpeed = 5f;

    private IEnumerator runningCoroutine = null;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }


    public void Pause()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
    }


    public void Resume()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
    }


    public void TurnSlowmoOn()
    {
        if (runningCoroutine != null)
            StopCoroutine(runningCoroutine);
        runningCoroutine = Decrease();
        StartCoroutine(runningCoroutine);
    }


    public void TurnSlowmoOff()
    {
        if (runningCoroutine != null)
            StopCoroutine(runningCoroutine);
        runningCoroutine = Increase();
        StartCoroutine(runningCoroutine);
    }


    private IEnumerator Decrease()
    {
        while (Time.timeScale > slowmoTimeScale)
        {
            float t = Time.timeScale - timeChangeDecreaseSpeed * Time.unscaledDeltaTime;
            Time.timeScale = t > slowmoTimeScale ? t : slowmoTimeScale;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            yield return null;
        }

        Time.timeScale = slowmoTimeScale;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        StopCoroutine(runningCoroutine);
    }


    private IEnumerator Increase()
    {
        while (Time.timeScale < 1f)
        {
            Time.timeScale += timeChangeIncreaseSpeed * Time.unscaledDeltaTime;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            yield return null;
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        StopCoroutine(runningCoroutine);
    }
}
