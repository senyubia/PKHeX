using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Stores the selected facial appearances of the player.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class PlayerAppearance9 : SaveBlock<SAV9SV>
{
    public PlayerAppearance9(SAV9SV sav, SCBlock block) : base(sav, block.Data) { }
    public ulong SkinColor     { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x00)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x00), value); }
    public ulong LipColor      { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x08)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x08), value); }
    public ulong ColorContacts { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x10)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x10), value); }
    public ulong EyeShape      { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x18)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x18), value); }
    public ulong EyebrowColor  { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x20)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x20), value); }
    public ulong EyebrowShape  { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x28)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x28), value); }
    public ulong _30           { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x30)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x30), value); }
    public ulong EyelashColor  { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x38)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x38), value); }
    public ulong EyelashShape1 { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x40)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x40), value); }
    public ulong EyelashShape2 { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x48)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x48), value); }
    public ulong Mouth         { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x50)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x50), value); }
    public ulong BeautySpot    { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x58)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x58), value); }
    public ulong Freckles      { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x60)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x60), value); }
    public ulong HairColor     { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x68)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x68), value); }
}
