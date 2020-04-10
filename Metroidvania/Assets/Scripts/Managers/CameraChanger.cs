﻿using UnityEngine;
using Cinemachine;

[RequireComponent( typeof( PolygonCollider2D ) )]
public class CameraChanger : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cameraToSet = null;

    private PlayerState playerState = null;


    private void Start()
    {
        playerState = PlayerState.instance;
    }


    private void OnTriggerEnter2D( Collider2D collider )
    {
        if ( collider.CompareTag( "Player" ) && collider.name != "Shield" )
        {
            if ( playerState.currentVirtualCamera != null && playerState.currentVirtualCamera != cameraToSet )
                playerState.currentVirtualCamera.Priority = 10;
            cameraToSet.Priority = 11;
            playerState.currentVirtualCamera = cameraToSet;
            collider.GetComponent<Telekinesis>().cameraOffset = cameraToSet.GetComponent<CinemachineCameraOffset>();
        }
    }
}
