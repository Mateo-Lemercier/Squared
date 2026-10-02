using Godot;
using System;

[Tool]
public partial class Level : Node2D
{
    [Signal] public delegate void ToggleGroupEventHandler( uint group );

    public TilesManager tileMaps;


    public override void _Ready() {
        tileMaps = GetNode<TilesManager>( "TileMaps" );
        Start();
    }

    public void Start() {
        tileMaps.GenerateNodes();
    }
}
