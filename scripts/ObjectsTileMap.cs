using Godot;
using System;

public partial class ObjectsTileMap : TileMapLayer
{
    [Signal] public delegate void GroupSwitchedEventHandler( uint group ); // TODO Move to Universal Place ( Level ? )


    private void ButtonSwitch( uint group, Vector2 position, bool active ) {
        Vector2I cell = LocalToMap( position );
        Vector2I atlasCoords = GetCellAtlasCoords( cell );
        atlasCoords.Y = (int)( 2 * group + ( active ? 1 : 0 ) );
        SetCell( cell, 2, atlasCoords );
        EmitSignal( SignalName.GroupSwitched, group );
    }

    private void OnGroupSwitched( uint group ) {
        //
    }
}
