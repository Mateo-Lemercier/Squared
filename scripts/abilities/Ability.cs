using Godot;
using System;

[GlobalClass]
public abstract partial class Ability : Resource
{
    public abstract void Ready( Player player );
    public abstract void Process( Player player );
    public abstract void PhysicsProcess( Player player );
}
