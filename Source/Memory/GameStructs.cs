using System;
using System.Runtime.InteropServices;
using Rage;

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
internal struct SoundSetStruct
{
    [FieldOffset(0)] public byte Class;
    [FieldOffset(1)] public uint Flags;
    [FieldOffset(5)] public uint SoundCount;
    [FieldOffset(9)] public TSoundsRaw Sounds;

    public SoundSetStruct()
    {
        Class = 32;
        Flags = 0xAAAAAAAA;
        SoundCount = 9;

        Sounds = new TSoundsRaw
        {
            [0] = new TSounds
            {
                Name = 0x26D96F23,
                MetadataRef = 0
            },
            [1] = new TSounds
            {
                Name = 0x3E34681F,
                MetadataRef = 0
            },
            [2] = new TSounds
            {
                Name = 0x4DD32855,
                MetadataRef = 0
            },
            [3] = new TSounds
            {
                Name = 0x5417639D,
                MetadataRef = 0
            },
            [4] = new TSounds
            {
                Name = 0x64B8047E,
                MetadataRef = 0
            },
            [5] = new TSounds
            {
                Name = 0xC4EE147F,
                MetadataRef = 0
            },
            [6] = new TSounds
            {
                Name = 0xCB4A8157,
                MetadataRef = 0
            },
            [7] = new TSounds
            {
                Name = 0xD91C6C0F,
                MetadataRef = 0
            },
            [8] = new TSounds
            {
                Name = 0x26D96F23,
                MetadataRef = 0
            },
            [9] = new TSounds
            {
                Name = 0xE91F183C,
                MetadataRef = 0
            }
        };
    }

    public unsafe void Copy(SoundSetStruct* source)
    {
        fixed (SoundSetStruct* self = &this)
        {
            var size = Marshal.SizeOf(this);
            Buffer.MemoryCopy(source, self, size, size);
        }
    }

    public uint GetSound(uint scriptNameHash)
    {
        for (var i = 0; i < SoundCount; i++)
        {
            if (Sounds[i].Name == scriptNameHash)
                return Sounds[i].MetadataRef;
        }

        return 0;
    }
    public uint GetSound(string scriptName) => GetSound(Game.GetHashKey(scriptName));
    
    public bool SetSound(uint scriptNameHash, uint soundMetadataRef)
    {
        for (var i = 0; i < SoundCount; i++)
        {
            if (Sounds[i].Name == scriptNameHash)
            {
                Sounds[i] = new TSounds()
                {
                    Name = scriptNameHash,
                    MetadataRef = soundMetadataRef
                };
                return true;
            }
        }

        return false;
    }
    public bool SetSound(string scriptName, uint soundMetadataRef) => SetSound(Game.GetHashKey(scriptName), soundMetadataRef);
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct audSoundSet
{
    public SoundSetStruct* Data;
    public uint NameHash;

    public void Init(uint soundSetNameHash)
    {
        fixed (audSoundSet* self = &this)
        {
            GameFunctions.InitAudSoundSet(self, soundSetNameHash);
        }
    }
    public void Init(string soundSetName) => Init(Game.GetHashKey(soundSetName));
}

public enum eSirenState
{
    OFF = 0,
    SLOW = 1,
    FAST = 2,
    WARNING = 3
}