using Godot;
using System;

[GlobalClass]
public partial class ActionMap : Resource
{
    [Export] public Action[] actions = [];

    public void OverrideInputMap() {
        foreach ( Action action in actions ) {
            InputMap.ActionEraseEvents( action.name );
            foreach ( InputEvent inputEvent in action.inputEvents ) {
                InputMap.ActionAddEvent( action.name, inputEvent );
            }
        }
    }
}
