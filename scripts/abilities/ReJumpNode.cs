using Godot;
using System;

[GlobalClass]
public partial class ReJumpNode : AbilityNode
{
    [Export] private uint  reJumpsCount   = 1;
    [Export] private float reJumpCooldown = 0.3f; // TODO Remove [Export] -> Calculate best cooldown based on JumpStrength and GravityStrength

    private uint  reJumps   = 0;
    private float elapsed   = 0.0f;
    private bool justJumped = true;


    public override void _PhysicsProcess( double delta ) {
        Player player = GetPlayer();
        if ( player is null ) return;

        if ( player.CanJump() ) {
            ResetValues();
            return;
        }

        if ( CanReJump() == false ) return;
        if ( ReJumpPressed( delta ) == false ) return;

        ReJump( player );
    }


    private void ResetValues() {
        reJumps    = 0;
        elapsed    = 0.0f;
        justJumped = true;
    }

    private bool CanReJump() {
        if ( justJumped ) {
            justJumped = false;
            return false;
        }
        return reJumps < reJumpsCount;
    }

    private bool ReJumpPressed( double delta ) {
        if ( Input.IsActionJustPressed( "jump" ) ) return true;
        elapsed += (float)delta;
        return Input.IsActionPressed( "jump" ) && ( elapsed >= reJumpCooldown );
    }

    private void ReJump( Player player ) {
        // TODO See how to do differently ?
        player.velocity = player.GetRotatedVelocity();
        player.Jump();
        player.SetRotatedVelocity( player.velocity );
        reJumps++;
        elapsed = 0.0f;
    }
}
