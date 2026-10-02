using Godot;
using System;

[Tool]
public partial class Crate : RigidBody2D
{
    private bool waitingToSwitch = false;
    private uint bodyCount = 0;


    public void OnToggleGroup( uint group ) {
        Sprite2D sprite = GetNode<Sprite2D>( "Sprite2D" );
        if ( group != sprite.Frame ) return;
        if ( bodyCount == 0 ) {
            SwitchTangibility();
            return;
        }
        waitingToSwitch ^= true;
    }

    public void SwitchTangibility() {
        CollisionLayer ^= (int)LayerNames.PlayerActive;
        CollisionLayer ^= (int)LayerNames.PlayerInactive;
        CollisionMask  ^= (int)LayerNames.PlayerActive;
        CollisionMask  ^= (int)LayerNames.PlayerInactive;

        CollisionMask ^= (int)LayerNames.PlatformActive;
        CollisionMask ^= (int)LayerNames.PlatformInactive;
        CollisionMask ^= (int)LayerNames.DamageActive;
        CollisionMask ^= (int)LayerNames.DamageInactive;

        Area2D area = GetNode<Area2D>( "Area2D" );
        area.CollisionMask ^= (int)LayerNames.PlayerActive;
        area.CollisionMask ^= (int)LayerNames.PlayerInactive;

        Color modulate = Modulate;
        modulate.A = 1.0f + StaticVariables.InactiveModulateStrength - modulate.A;
        Modulate = modulate;

        ZIndex = 11 + 9 - ZIndex; // TODO Remove Magic Numbers ( Crate ZIndexes )

        waitingToSwitch = false;
    }

    private void OnBodyEntered( Node2D body ) {
        bodyCount++;
    }

    private void OnBodyExited( Node2D body ) {
        bodyCount--;
        if ( bodyCount != 0 || waitingToSwitch == false ) return;
        SwitchTangibility();
    }
}
