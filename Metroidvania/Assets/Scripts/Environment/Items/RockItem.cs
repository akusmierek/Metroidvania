﻿using UnityEngine;

public class RockItem : Item
{
    protected override void OnTriggerEnter2D( Collider2D collider )
    {
        if ( !isShooted )
        {
            return;
        }

        if ( collidersToIgnore.Find( ( Collider2D x ) => x == collider ) )
        {
            return;
        }

        base.OnTriggerEnter2D( collider );

        CustomDestroy();
    }
}