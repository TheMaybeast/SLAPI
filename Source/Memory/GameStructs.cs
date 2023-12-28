using System;
using System.Runtime.InteropServices;
using Rage;
using SLAPI.Utils;

namespace SLAPI.Memory;

[StructLayout(LayoutKind.Sequential)]
internal struct TSounds
{
    public uint Name;
    public uint MetadataRef;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct TSoundsRaw
{
    private fixed ulong Sounds[1000];

    public TSounds this[int i]
    {
        get
        {
            var name = (uint)Sounds[i];
            var metadataRef = (uint)(Sounds[i] >> 32);
            return new TSounds
            {
                Name = name,
                MetadataRef = metadataRef
            };
        }
        set
        {
            ulong b = value.MetadataRef;
            b <<= 32;
            b |= value.Name;
            Sounds[i] = b;
        }
    }
}

[StructLayout(LayoutKind.Explicit, Pack = 1)]
internal struct SoundSet
{
    [FieldOffset(0)] public byte Class; // 1
    [FieldOffset(1)] public uint Flags; // 4
    [FieldOffset(5)] public uint SoundCount; // 4
    [FieldOffset(9)] public TSoundsRaw Sounds; // 8000

    public TSounds GetSound(uint scriptNameHash)
    {
        for (var i = 0; i < SoundCount; i++)
        {
            if (Sounds[i].Name == scriptNameHash)
                return Sounds[i];
        }

        return new TSounds();
    }
    public TSounds GetSound(string scriptName) => GetSound(Game.GetHashKey(scriptName));

    public void SetSound(uint scriptNameHash, uint metadataRef)
    {
        for (var i = 0; i < SoundCount; i++)
        {
            if (Sounds[i].Name == scriptNameHash)
                Sounds[i] = new TSounds()
                {
                    Name = scriptNameHash,
                    MetadataRef = metadataRef
                };
        }
    }
    public void SetSound(string scriptName, uint metadataRef) => SetSound(Game.GetHashKey(scriptName), metadataRef);

    public unsafe void Copy(SoundSet* source)
    {
        fixed (SoundSet* self = &this)
        {
            var size = Marshal.SizeOf(this);
            Buffer.MemoryCopy(source, self, size, size);
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct audSoundSet
{
    public SoundSet* Data;
    public uint NameHash;

    public void Init(uint soundSetNameHash)
    {
        fixed (audSoundSet* self = &this)
        {
            GameFunctions.InitAudSoundSet(self, soundSetNameHash);
        }
    }
    public void Init(string soundSetName) => Init(Game.GetHashKey(soundSetName));

    public void DumpToLog()
    {
        $"SoundSet: {NameHash}".ToLog();
        $"  Class: {Data->Class}".ToLog();
        $"  Flags: {Data->Flags:X}".ToLog();
        $"  SoundCount: {Data->SoundCount}".ToLog();

        for (var i = 0; i < Data->SoundCount; i++)
            $"  #{i} {Data->Sounds[i].MetadataRef:X} ({Data->Sounds[i].Name:X})".ToLog();
    }
}

public enum SirenState
{
    OFF = 0,
    SLOW = 1,
    FAST = 2,
    WARNING = 3
}