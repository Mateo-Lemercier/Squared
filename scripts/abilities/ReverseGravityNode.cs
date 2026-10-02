using Godot;
using System;

[GlobalClass]
public partial class ReverseGravityNode : AbilityNode
{
    private bool reversed = false;
    public bool Reversed() => reversed;


    public override void _PhysicsProcess( double delta ) {
        if ( Input.IsActionJustPressed( "reverse-gravity" ) == false ) return;

        Player player = GetPlayer();
        if ( player is null ) return;

        RotatePlayerUpDirection( player );
        UpdateActionMap( player );
        reversed ^= true;
    }


    private void RotatePlayerUpDirection( Player player ) {
        player.UpDirection = -player.UpDirection;
        player.velocity    = -player.velocity; // rotate velocity to counteract the rotation triggered by the UpDirection changing
    }

    private void UpdateActionMap( Player player ) {
        if ( player.HasNode( "NoGravityNode" ) &&
             player.GetNode<NoGravityNode>( "NoGravityNode" ).GravityEnabled() == false ) {
            return;
        }

        if ( reversed ) {
            ResourceLoader.Load<ActionMap>( "res://resources/actions/actionmap-default-movement.tres" ).OverrideInputMap();
            return;
        }

        ResourceLoader.Load<ActionMap>( "res://resources/actions/actionmap-reverse-movement.tres" ).OverrideInputMap();
    }
}
