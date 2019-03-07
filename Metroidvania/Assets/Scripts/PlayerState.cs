﻿using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState instance = null;

    // States

    [HideInInspector] public bool isHoldingItemState = false;
    [HideInInspector] public bool isPullingItemState = false;
    [HideInInspector] public bool isJumpingState = false;
    [HideInInspector] public bool isDashingState = false;

    // Skills

    public bool hasDoubleJump = false;
    public bool hasDash = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }
}