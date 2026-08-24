using Godot;
using System;

public partial class PlatformTileMap : TileMapLayer
{
    private void OnGroupSwitched( uint group ) {
        int SourceID = (int)( 3 + 2 * group );
        TileSet.SetSourceId( SourceID, 0 );
        TileSet.SetSourceId( SourceID+1, SourceID );
        TileSet.SetSourceId( 0, SourceID+1 );
    }
}
