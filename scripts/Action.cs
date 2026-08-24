using Godot;
using System;

[GlobalClass]
public partial class Action : Resource
{
    [Export] public StringName   name        = "";
    [Export] public InputEvent[] inputEvents = [];
}
