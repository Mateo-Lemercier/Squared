using Godot;
using System;

[GlobalClass]
public partial class ReJump : Ability
{
    [Export] private uint  reJumpCount    = 1;
    [Export] private float reJumpCooldown = 0.3f;

    private uint  reJumpLeft = 0;
    private float cooldown;


    public override void Ready( Player player ) {
        //
    }

    public override void Process( Player player ) {
        //
    }

    public override void PhysicsProcess( Player player ) {
        HandleReJump( player );
    }


    private void HandleReJump( Player player ) {
        if ( player.canJump ) {
            reJumpLeft = reJumpCount;
            cooldown   = reJumpCooldown;
            return;
        }
        if ( reJumpLeft == 0 ) return;

        cooldown -= player.deltaF;

        bool reJump =
            Input.IsActionJustPressed( "jump" ) ||
            ( Input.IsActionPressed( "jump" ) && cooldown <= 0.0f );

        if ( reJump == false ) return;

        player.Jump();
        reJumpLeft--;
        cooldown = reJumpCooldown;
    }
}
