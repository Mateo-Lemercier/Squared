using Godot;
using System;
using System.IO;
using System.Runtime.InteropServices;
// TODO Should be using BinaryWriter & BinaryReader

public class LSBSteganography
{
    // public static Image Encode<T>( Image image, T structObject ) where T : struct {
    //     byte[] imageData   = image.GetData();
    //     byte[] structBytes = StructToBytes( structObject );
    //     uint   startIndex  = 0;
    //     foreach ( byte structByte in structBytes ) {
    //         startIndex = EncodeByte( imageData, structByte, startIndex );
    //     }
    //     return Image.CreateFromData( image.GetWidth(), image.GetHeight(), image.HasMipmaps(), image.GetFormat(), imageData );
    // }

    // public static T Decode<T>( Image image ) where T : struct {
    //     int    structSize  = Marshal.SizeOf( typeof(T) );
    //     byte[] imageData   = image.GetData();
    //     byte[] structBytes = new byte[structSize];
    //     uint   startIndex  = 0;
    //     for ( uint byteIndex = 0; byteIndex < structSize; byteIndex++ ) {
    //         startIndex = DecodeByte( imageData, structBytes, byteIndex, startIndex );
    //     }
    //     return BytesToStruct<T>( structBytes );
    // }


    public static uint EncodeByte( byte[] imageData, byte byteObject, uint imageIndex ) { // TODO Make private when EncodeInt is removed
        imageData[imageIndex+0] = (byte)( ( imageData[imageIndex+0] & 0b11111110 ) | ( ( byteObject >> 7 ) & 0b1 ) );
        imageData[imageIndex+1] = (byte)( ( imageData[imageIndex+1] & 0b11111110 ) | ( ( byteObject >> 6 ) & 0b1 ) );
        imageData[imageIndex+2] = (byte)( ( imageData[imageIndex+2] & 0b11111110 ) | ( ( byteObject >> 5 ) & 0b1 ) );
        imageData[imageIndex+3] = (byte)( ( imageData[imageIndex+3] & 0b11111110 ) | ( ( byteObject >> 4 ) & 0b1 ) );
        imageData[imageIndex+4] = (byte)( ( imageData[imageIndex+4] & 0b11111110 ) | ( ( byteObject >> 3 ) & 0b1 ) );
        imageData[imageIndex+5] = (byte)( ( imageData[imageIndex+5] & 0b11111110 ) | ( ( byteObject >> 2 ) & 0b1 ) );
        imageData[imageIndex+6] = (byte)( ( imageData[imageIndex+6] & 0b11111110 ) | ( ( byteObject >> 1 ) & 0b1 ) );
        imageData[imageIndex+7] = (byte)( ( imageData[imageIndex+7] & 0b11111110 ) | ( ( byteObject >> 0 ) & 0b1 ) );
        return imageIndex+8;
    }

    public static uint DecodeByte( byte[] imageData, byte[] byteArray, uint byteIndex, uint imageIndex ) { // TODO Make private when DecodeInt is removed
        byteArray[byteIndex] = 0;
        byteArray[byteIndex] |= (byte)( ( imageData[imageIndex+0] & 0b1 ) << 7 );
        byteArray[byteIndex] |= (byte)( ( imageData[imageIndex+1] & 0b1 ) << 6 );
        byteArray[byteIndex] |= (byte)( ( imageData[imageIndex+2] & 0b1 ) << 5 );
        byteArray[byteIndex] |= (byte)( ( imageData[imageIndex+3] & 0b1 ) << 4 );
        byteArray[byteIndex] |= (byte)( ( imageData[imageIndex+4] & 0b1 ) << 3 );
        byteArray[byteIndex] |= (byte)( ( imageData[imageIndex+5] & 0b1 ) << 2 );
        byteArray[byteIndex] |= (byte)( ( imageData[imageIndex+6] & 0b1 ) << 1 );
        byteArray[byteIndex] |= (byte)( ( imageData[imageIndex+7] & 0b1 ) << 0 );
        return imageIndex+8;
    }

    public static uint EncodeInt( byte[] imageData, int intObject, uint imageIndex = 0 ) { // TODO Remove when BinaryWriter implemented
        imageIndex = EncodeByte( imageData, (byte)( intObject >> 24 ), imageIndex );
        imageIndex = EncodeByte( imageData, (byte)( intObject >> 16 ), imageIndex );
        imageIndex = EncodeByte( imageData, (byte)( intObject >> 8  ), imageIndex );
        imageIndex = EncodeByte( imageData, (byte)( intObject >> 0  ), imageIndex );
        return imageIndex;
    }

    public static uint DecodeInt( byte[] imageData, int[] intArray, uint intIndex, uint imageIndex ) { // TODO Remove when BinaryReader implemented
        byte[] byteArray = [ 0, 0, 0, 0 ];
        imageIndex = DecodeByte( imageData, byteArray, 0, imageIndex );
        imageIndex = DecodeByte( imageData, byteArray, 1, imageIndex );
        imageIndex = DecodeByte( imageData, byteArray, 2, imageIndex );
        imageIndex = DecodeByte( imageData, byteArray, 3, imageIndex );
        intArray[intIndex] = 0;
        intArray[intIndex] |= byteArray[0] << 24;
        intArray[intIndex] |= byteArray[1] << 16;
        intArray[intIndex] |= byteArray[2] << 8;
        intArray[intIndex] |= byteArray[3] << 0;
        return imageIndex;
    }


    private static byte[] StructToBytes<T>( T structObject ) where T : struct {
        int    structSize = Marshal.SizeOf( typeof(T) );
        nint   pointer    = IntPtr.Zero;
        byte[] bytes      = new byte[structSize];
        try {
            pointer = Marshal.AllocHGlobal( structSize );
            Marshal.StructureToPtr( structObject, pointer, true );
            Marshal.Copy( pointer, bytes, 0, structSize );
            return bytes;
        }
        finally {
            if ( pointer != IntPtr.Zero )
                Marshal.FreeHGlobal( pointer );
        }
    }

    private static T BytesToStruct<T>( byte[] bytes ) where T : struct {
        int  structSize = Marshal.SizeOf( typeof(T) );
        nint pointer    = IntPtr.Zero;
        try {
            pointer = Marshal.AllocHGlobal( structSize );
            Marshal.Copy( bytes, 0, pointer, structSize );
            return (T)Marshal.PtrToStructure( pointer, typeof(T) );
        }
        finally {
            if ( pointer != IntPtr.Zero )
                Marshal.FreeHGlobal( pointer );
        }
    }
}
