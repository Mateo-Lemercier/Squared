using Godot;
using System;

public partial class Player : CharacterBody2D
{
    static public float JumpStrength = -122.0f;

    [Export] public Ability[] abilities = [];

    [Export] public float gravityStrength = 1.0f;

    [Export] public float jumpHeight   = 1.3f;

    [Export] public float acceleration = 16.0f;
    [Export] public float deceleration = 12.5f;
    [Export] public float maxSpeed     = 80.0f;

    public Vector2 velocity;
    public float   deltaF;

    public bool    canJump = false;


    public override void _Ready() {
        foreach ( Ability ability in abilities ) {
            ability.Ready( this );
        }
    }

    public override void _Process( double deltaD ) {
        deltaF = (float)deltaD;

        foreach ( Ability ability in abilities ) {
            ability.Process( this );
        }
    }

    public override void _PhysicsProcess( double deltaD ) {
        // Rotate Velocity based on UpDirection for easier operations
        velocity = new Vector2(
            UpDirection.X * Velocity.Y - UpDirection.Y * Velocity.X,
            -UpDirection.X * Velocity.X - UpDirection.Y * Velocity.Y
        );
        deltaF   = (float)deltaD;

        foreach ( Ability ability in abilities ) {
            ability.PhysicsProcess( this );
        }

        HandleWalk();
        HandleJump();
        HandleGravity();

        // Undo the rotation made to Velocity
        Velocity = new Vector2(
            -UpDirection.X * velocity.Y - UpDirection.Y * velocity.X,
            UpDirection.X * velocity.X - UpDirection.Y * velocity.Y
        );
        MoveAndSlide();
    }


    private void HandleWalk() {
        float direction = 0.0f;
        if ( Input.IsActionPressed( "move-left" ) )  direction -= 1.0f;
        if ( Input.IsActionPressed( "move-right" ) ) direction += 1.0f;

        float maxDelta = ( direction == 0.0f ) ? deceleration : ( IsOnFloor() ? acceleration : maxSpeed );
        velocity.X = Mathf.MoveToward( velocity.X, direction * maxSpeed, maxDelta );
    }


    private void HandleJump() {
        if ( canJump == false ) {
            canJump = IsOnFloor();
            return;
        }
        if ( Input.IsActionPressed( "jump" ) == false ) return;
        Jump();
        canJump = false;
    }

    public void Jump() {
        velocity.Y = JumpStrength * jumpHeight;
    }

    private void HandleGravity() {
        if ( IsOnFloor() ) return;
        velocity.Y += gravityStrength * GetGravity().Length() * deltaF;
    }
}
