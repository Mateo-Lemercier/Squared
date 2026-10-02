using Godot;
using System;

[GlobalClass]
public partial class IntangibleNode : AbilityNode
{
    private bool tangible = true;
    public bool Tangible() => tangible;


    public override void _PhysicsProcess( double delta ) {
        if ( Input.IsActionJustPressed( "switch-tangibility" ) == false ) return;

        Player player = GetPlayer();
        if ( player is null ) return;

        InvertCollisionLayer( player );
        InvertCollisionMasks( player );
        InvertTransparency( player );
        tangible ^= true;
    }


    private void InvertCollisionLayer( Player player ) {
        player.CollisionLayer ^= (int)LayerNames.PlayerActive;
        player.CollisionLayer ^= (int)LayerNames.PlayerInactive;
    }

    private void InvertCollisionMasks( Player player ) {
        player.CollisionMask ^= (int)LayerNames.PlayerActive;
        player.CollisionMask ^= (int)LayerNames.PlayerInactive;
        player.CollisionMask ^= (int)LayerNames.PlatformActive;
        player.CollisionMask ^= (int)LayerNames.PlatformInactive;
        InvertDamageCollisionMask( player );
    }

    private void InvertDamageCollisionMask( Player player ) {
        Area2D playerArea = player.GetNode<Area2D>( "DamageArea2D" );
        playerArea.CollisionMask ^= (int)LayerNames.DamageActive;
        playerArea.CollisionMask ^= (int)LayerNames.DamageInactive;
    }

    private void InvertTransparency( Player player ) {
        Color modulate = player.Modulate;
        modulate.A = 1.0f + StaticVariables.InactiveModulateStrength - modulate.A;
        player.Modulate = modulate;
    }
}
