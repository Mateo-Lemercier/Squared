using Godot;
using System;

[GlobalClass]
public abstract partial class AbilityNode : Node
{
    public Player GetPlayer() {
        Node parent = GetParent();
        if ( parent is null )       return null;
        if ( parent is not Player ) return null;
        return (Player)parent;
    }
}
