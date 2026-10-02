using Godot;
using System;

[GlobalClass]
public partial class NoGravityNode : AbilityNode
{
    private bool gravityEnabled = true;
    public bool GravityEnabled() => gravityEnabled;


    public override void _PhysicsProcess( double delta ) {
        if ( Input.IsActionJustPressed( "switch-gravity" ) == false ) return;

        Player player = GetPlayer();
        if ( player is null ) return;

        // TODO DO BETTER
        // Switch Player Gravity Strength
        player.gravityStrength = player.GravityStrength - player.gravityStrength;

        // TODO DO BETTER
        // Switch Player Acceleration & Deceleration
        player.acceleration = player.Acceleration - player.acceleration;
        player.deceleration = player.Deceleration - player.deceleration;

        UpdateActionMap( player );
        gravityEnabled ^= true;
    }


    private void UpdateActionMap( Player player ) {
        if ( gravityEnabled ) {
            ResourceLoader.Load<ActionMap>( "res://resources/actions/actionmap-no-movement.tres" ).OverrideInputMap();
            return;
        }

        if ( player.HasNode( "ReverseGravityNode" ) &&
             player.GetNode<ReverseGravityNode>( "ReverseGravityNode" ).Reversed() ) {
            ResourceLoader.Load<ActionMap>( "res://resources/actions/actionmap-reverse-movement.tres" ).OverrideInputMap();
            return;
        }

        ResourceLoader.Load<ActionMap>( "res://resources/actions/actionmap-default-movement.tres" ).OverrideInputMap();
    }
}
