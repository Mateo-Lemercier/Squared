using Godot;
using System;

// 1   -122
// 2   -174
// 3   -213

[Tool]
public partial class Player : CharacterBody2D
{
    static public float JumpStrength = -213.0f;

    [Export] public float GravityStrength { get; private set; } = 1.0f;
    public float gravityStrength;

    [Export] public float jumpHeight   = 1.0f;

    [Export] public float Acceleration { get; private set; } = 16.0f;
    [Export] public float Deceleration { get; private set; } = 12.5f;
    [Export] public float MaxSpeed     { get; private set; } = 80.0f;
    public float acceleration;
    public float deceleration;
    public float maxSpeed;

    public Vector2 velocity;
    public float   deltaF;

    private bool canJump = false;
    public bool CanJump() => canJump;


    public override void _Ready() {
        if ( Engine.IsEditorHint() ) return;

        gravityStrength = GravityStrength;
        acceleration    = Acceleration;
        deceleration    = Deceleration;
        maxSpeed        = MaxSpeed;
    }

    public override void _Process( double deltaD ) {
        if ( Engine.IsEditorHint() ) return;

        deltaF = (float)deltaD;
    }

    public override void _PhysicsProcess( double deltaD ) {
        if ( Engine.IsEditorHint() ) return;

        velocity = GetRotatedVelocity();
        deltaF = (float)deltaD;

        HandleWalk();
        HandleJump();
        HandleGravity();

        SetRotatedVelocity( velocity );

        MoveAndSlide();
        HandleCollisions();
    }

    public Vector2 GetRotatedVelocity() {
        return new Vector2(
             UpDirection.X * Velocity.Y - UpDirection.Y * Velocity.X,
            -UpDirection.X * Velocity.X - UpDirection.Y * Velocity.Y
        );
    }

    public void SetRotatedVelocity( Vector2 velocity ) {
        Velocity = new Vector2(
            -UpDirection.X * velocity.Y - UpDirection.Y * velocity.X,
             UpDirection.X * velocity.X - UpDirection.Y * velocity.Y
        );
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


    private void HandleCollisions() {
        for ( int i = 0; i < GetSlideCollisionCount(); i++ ) {
            // KinematicCollision2D collision = GetSlideCollision( i );
            // if ( collision.GetCollider() is not RigidBody2D ) continue;
            // ((RigidBody2D)collision.GetCollider()).ApplyCentralImpulse( -collision.GetNormal() * pushForce /* Velocity.Length() */ );
        }
    }


    public void Die( Node2D body ) {
        Modulate = Modulate.Inverted();
    }
}
