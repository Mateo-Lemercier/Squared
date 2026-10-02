using Godot;
using System;

[Tool]
public partial class LevelEditor : Control
{
    [ExportGroup( "Export as Image" )]
    [Export]
    private string ExportPath = "res://exportedLevel.png";
    [ExportToolButton( "   Export      " )]
    private Callable ExportToolButton => Callable.From( ExportAsImage );

    [ExportGroup( "Import as Image" )]
    [Export(PropertyHint.File, "*.png")]
    private string ImportPath = "res://exportedLevel.png";
    [ExportToolButton( "   Import      " )]
    private Callable ImportToolButton => Callable.From( ImportAsImage );

    public Level level;


    public override void _Ready() {
        // level = GetNode<Level>( "../Level" );
    }


    public void ExportAsImage() {
        Image levelImage = GetViewport().GetTexture().GetImage();
        byte[] imageData = levelImage.GetData();

        uint imageIndex = 0;
        imageIndex = EncodeTileMap( imageData, level.tileMaps.BackDecorations,  imageIndex );
        imageIndex = EncodeTileMap( imageData, level.tileMaps.Platforms,        imageIndex );
        imageIndex = EncodeTileMap( imageData, level.tileMaps.Objects,          imageIndex );
        imageIndex = EncodeTileMap( imageData, level.tileMaps.FrontDecorations, imageIndex );

        levelImage.SetData( levelImage.GetWidth(), levelImage.GetHeight(), levelImage.HasMipmaps(), levelImage.GetFormat(), imageData );
        levelImage.SavePng( ExportPath );
    }

    public void ImportAsImage() {
        Image levelImage = Image.LoadFromFile( ImportPath );
        byte[] imageData = levelImage.GetData();

        uint imageIndex = 0;
        imageIndex = DecodeTileMap( imageData, level.tileMaps.BackDecorations,  imageIndex );
        imageIndex = DecodeTileMap( imageData, level.tileMaps.Platforms,        imageIndex );
        imageIndex = DecodeTileMap( imageData, level.tileMaps.Objects,          imageIndex );
        imageIndex = DecodeTileMap( imageData, level.tileMaps.FrontDecorations, imageIndex );
    }


    private uint EncodeTileMap( byte[] imageData, TileMapLayer tileMap, uint imageIndex ) { // Temporary
        Rect2I tileMapRect = tileMap.GetUsedRect();
        imageIndex = LSBSteganography.EncodeInt( imageData, tileMapRect.Position.X, imageIndex );
        imageIndex = LSBSteganography.EncodeInt( imageData, tileMapRect.Position.Y, imageIndex );
        imageIndex = LSBSteganography.EncodeInt( imageData, tileMapRect.End.X,      imageIndex );
        imageIndex = LSBSteganography.EncodeInt( imageData, tileMapRect.End.Y,      imageIndex );

        Vector2I cell = tileMapRect.Position;
        while ( cell.Y < tileMapRect.End.Y ) {
            while ( cell.X < tileMapRect.End.X ) {
                Vector2I atlasCoords = tileMap.GetCellAtlasCoords( cell );
                int      sourceId    = tileMap.GetCellSourceId( cell );
                imageIndex = LSBSteganography.EncodeInt( imageData, atlasCoords.X, imageIndex );
                imageIndex = LSBSteganography.EncodeInt( imageData, atlasCoords.Y, imageIndex );
                imageIndex = LSBSteganography.EncodeInt( imageData, sourceId, imageIndex );
                cell.X++;
            }
            cell.X = tileMapRect.Position.X;
            cell.Y++;
        }

        return imageIndex;
    }

    private uint DecodeTileMap( byte[] imageData, TileMapLayer tileMap, uint imageIndex ) {
        int[] intArray = [ 0, 0, 0, 0 ];
        imageIndex = LSBSteganography.DecodeInt( imageData, intArray, 0, imageIndex );
        imageIndex = LSBSteganography.DecodeInt( imageData, intArray, 1, imageIndex );
        imageIndex = LSBSteganography.DecodeInt( imageData, intArray, 2, imageIndex );
        imageIndex = LSBSteganography.DecodeInt( imageData, intArray, 3, imageIndex );

        tileMap.Clear();
        Vector2I tileMapPosition = new( intArray[0], intArray[1] );
        Vector2I tileMapEnd      = new( intArray[2], intArray[3] );

        Vector2I cell = tileMapPosition;
        while ( cell.Y < tileMapEnd.Y ) {
            while ( cell.X < tileMapEnd.X ) {
                intArray = [ 0, 0, 0 ];
                imageIndex = LSBSteganography.DecodeInt( imageData, intArray, 0, imageIndex );
                imageIndex = LSBSteganography.DecodeInt( imageData, intArray, 1, imageIndex );
                imageIndex = LSBSteganography.DecodeInt( imageData, intArray, 2, imageIndex );
                Vector2I atlasCoords = new( intArray[0], intArray[1] );
                int      sourceId    = intArray[2];
                tileMap.SetCell( cell, sourceId, atlasCoords );
                cell.X++;
            }
            cell.X = tileMapPosition.X;
            cell.Y++;
        }

        return imageIndex;
    }
}
