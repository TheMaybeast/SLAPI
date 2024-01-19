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

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct SirenInstanceData
{
    public uint SirenOnTime;
    public float SirenTimeDelta;
    public int LastSirenBeat;
    
    public fixed uint LastSirenRotationBit[20];
    public fixed uint SirenRotationStart[20];
    public fixed uint LastSirenFlashBit[20];
    public fixed uint SirenFlashStart[20];
    public fixed uint LastSirenLightBit[4];
    public fixed uint SirenLightStart[4];
    public fixed float LightIntensity[4];

    public uint SirenRotating;
    public uint SirenFlashing;
    public uint SirenLightOn;
}