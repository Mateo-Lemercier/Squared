using Godot;
using System;

[GlobalClass]
public partial class ReverseGravity : Ability
{
    [Export] private ActionMap defaultMovement;
    [Export] private ActionMap reverseMovement;

    private bool reversed = false;


    public override void Ready( Player player ) {
        //
    }

    public override void Process( Player player ) {
        //
    }

    public override void PhysicsProcess( Player player ) {
        HandleReverseGravity( player );
    }


    private void HandleReverseGravity( Player player ) {
        if ( Input.IsActionJustPressed( "power-action" ) == false ) return;
        player.UpDirection = -player.UpDirection;
        player.velocity    = -player.velocity;
        reversed           = !reversed;
        ActionMap newMovement = reversed ? reverseMovement : defaultMovement;
        newMovement.OverrideInputMap();
    }
}
