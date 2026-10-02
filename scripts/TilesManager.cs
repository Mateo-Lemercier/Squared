using Godot;
using System;

[Tool]
public partial class TilesManager : Node2D
{
    public class SourceIds {
        public const           int   Empty              = 0;

        public const           int   Platforms          = 1;
        public const           int   PlatformsActive1   = 2;
        public const           int   PlatformsInactive1 = 3;
        public const           int   PlatformsActive2   = 4;
        public const           int   PlatformsInactive2 = 5;
        public static readonly int[] PlatformsActive    = [ PlatformsActive1, PlatformsActive2 ];
        public static readonly int[] PlatformsInactive  = [ PlatformsInactive1, PlatformsInactive2 ];

        public const           int   Objects            = 1;
        public const           int   ObjectsActive1     = 2;
        public const           int   ObjectsInactive1   = 3;
        public const           int   ObjectsActive2     = 4;
        public const           int   ObjectsInactive2   = 5;
        public static readonly int[] ObjectsActive      = [ ObjectsActive1, ObjectsActive2 ];
        public static readonly int[] ObjectsInactive    = [ ObjectsInactive1, ObjectsInactive2 ];
    }

    [ExportToolButton( "Toggle Group 1" )]
    private Callable ToggleGroup1ToolButton => Callable.From( DEBUG_OnToggleGroup1 );
    [ExportToolButton( "Toggle Group 2" )]
    private Callable ToggleGroup2ToolButton => Callable.From( DEBUG_OnToggleGroup2 );

    public TileMapLayer BackDecorations;
    public TileMapLayer Objects;
    public TileMapLayer Platforms;
    public TileMapLayer FrontDecorations;

    private PackedScene ButtonScene;
    private PackedScene CrateScene;


    public override void _Ready() {
        BackDecorations  = GetNode<TileMapLayer>( "BackDecorations" );
        Objects          = GetNode<TileMapLayer>( "Objects" );
        Platforms        = GetNode<TileMapLayer>( "Platforms" );
        FrontDecorations = GetNode<TileMapLayer>( "FrontDecorations" );

        ButtonScene = GD.Load<PackedScene>( "res://scenes/button.tscn" );
        CrateScene  = GD.Load<PackedScene>( "res://scenes/crate.tscn" );
    }

    public void GenerateNodes() {
        Godot.Collections.Array<Vector2I> cells;

        cells = Objects.GetUsedCells();
        foreach ( Vector2I cell in cells ) {
            Vector2I atlasCoords = Objects.GetCellAtlasCoords( cell );
            switch ( atlasCoords.Y ) {
                case 15: GenerateButton( cell, atlasCoords ); break;
                case 16: FixAndGenerateButton( cell, atlasCoords ); break;
                default: break;
            }
            switch ( atlasCoords ) {
                case Vector2I(  1, 14 ): GenerateCrate( cell, atlasCoords ); break;
                case Vector2I(  6, 14 ): GenerateCrate( cell, atlasCoords ); break;
                case Vector2I( 11, 14 ): GenerateCrate( cell, atlasCoords ); break;
                default: break;
            }
        }

        // cells = Platforms.GetUsedCells();
        // foreach ( Vector2I cell in cells ) {
            // Vector2I atlasCoords = Platforms.GetCellAtlasCoords( cell );
            // switch ( atlasCoords ) {
                // case Vector2I( 0, 0 ): break;
                // default: GenerateCrate( cell ); break;
            // }
        // }
    }

    private void FixAndGenerateButton( Vector2I cell, Vector2I atlasCoords ) {
        atlasCoords.Y--;
        Objects.SetCell( cell, SourceIds.Objects, atlasCoords );
        GenerateButton( cell, atlasCoords );
    }

    private void GenerateButton( Vector2I cell, Vector2I atlasCoords ) {
        uint group = (uint)atlasCoords.X / 5;
        atlasCoords.X %= 5;
        atlasCoords.Y  = 0;

        Vector2I collisionShapeOffset = new Vector2I(
            0,
            4 * ( 1 - atlasCoords.X / 4 )
        );

        Button button = ButtonScene.Instantiate<Button>();
        GetNode( "../TileMapsNodes" ).AddChild( button );
        button.Position = Objects.MapToLocal( cell );
        button.RotationDegrees = 180.0f - 90.0f * ( atlasCoords.X % 4 );
        button.GetNode<Node2D>( "CollisionShape2D" ).Position = collisionShapeOffset;
        button.group = group;
        button.Switch += OnButtonChange;
    }

    private void GenerateCrate( Vector2I cell, Vector2I atlasCoords ) {
        int group = atlasCoords.X / 5;
        atlasCoords.X = 0;
        atlasCoords.Y = 0;

        int sourceId = Objects.GetCellSourceId( cell );

        Crate crate = CrateScene.Instantiate<Crate>();
        GetNode( "../TileMapsNodes" ).AddChild( crate );
        crate.Position = Objects.MapToLocal( cell );
        Sprite2D sprite = crate.GetNode<Sprite2D>( "Sprite2D" );
        sprite.Frame = group;
        switch ( sourceId ) {
            case SourceIds.ObjectsInactive1: crate.SwitchTangibility(); break;
            case SourceIds.ObjectsInactive2: crate.SwitchTangibility(); break;
            default: break;
        }
        GetNode<Level>( ".." ).ToggleGroup += crate.OnToggleGroup;
        Objects.SetCell( cell );
    }


    private void OnButtonChange( uint group, Vector2 position, bool active ) {
        Vector2I cell        = Objects.LocalToMap( position );
        Vector2I atlasCoords = Objects.GetCellAtlasCoords( cell );
        atlasCoords.Y = active ? 16 : 15;
        Objects.SetCell( cell, SourceIds.Objects, atlasCoords );
        GetNode( ".." ).EmitSignal( Level.SignalName.ToggleGroup, group );
    }


    private void DEBUG_OnToggleGroup1() { OnToggleGroup(1); }
    private void DEBUG_OnToggleGroup2() { OnToggleGroup(2); }
    private void OnToggleGroup( uint group ) {
        group--;
        SwitchTileSetSources( Objects.TileSet,   SourceIds.ObjectsActive[group],   SourceIds.ObjectsInactive[group]   );
        SwitchTileSetSources( Platforms.TileSet, SourceIds.PlatformsActive[group], SourceIds.PlatformsInactive[group] );
    }

    private void SwitchTileSetSources( TileSet tileSet, int id1, int id2 ) {
        int ___ = SourceIds.Empty;
        tileSet.SetSourceId( id1, ___ );
        tileSet.SetSourceId( id2, id1 );
        tileSet.SetSourceId( ___, id2 );
    }
}
