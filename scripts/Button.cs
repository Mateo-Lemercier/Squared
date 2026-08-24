using Godot;
using System;

public partial class Button : Area2D
{
    [Signal] public delegate void SwitchEventHandler( uint group, Vector2 position, bool active );

    [Export] private uint group = 0;
    private uint pushedCount = 0;


    private void OnBodyEntered() { OnBodyEntered( null ); }
    private void OnBodyExited()  { OnBodyExited( null ); }

    private void OnBodyEntered( Node2D body ) {
        pushedCount++;
        if ( pushedCount != 1 ) return;
        EmitSignal( SignalName.Switch, group, Position, true );
    }

    private void OnBodyExited( Node2D body )  {
        pushedCount--;
        if ( pushedCount != 0 ) return;
        EmitSignal( SignalName.Switch, group, Position, false );
    }
}
