using Rage;
using SLAPI.Memory;
using SLAPI.Utils;

namespace SLAPI;

public unsafe class SoundSetWrapper
{
    internal SoundSet* SoundSet;
    public uint NameHash;

    public void DumpToLog(bool toConsole = false)
    {
        $"SoundSet: {NameHash} ({(ulong)SoundSet:X})".ToLog(toConsole: toConsole);
        $"  Class: {SoundSet->Class}".ToLog(toConsole: toConsole);
        $"  Flags: {SoundSet->Flags:X}".ToLog(toConsole: toConsole);
        $"  SoundCount: {SoundSet->SoundCount}".ToLog(toConsole: toConsole);

        for (var i = 0; i < SoundSet->SoundCount; i++)
            $"  #{i} {SoundSet->Sounds[i].MetadataRef:X} ({SoundSet->Sounds[i].Name:X})".ToLog(toConsole: toConsole);
    }

    public uint GetSound(uint scriptNameHash)
    {
        for (var i = 0; i < SoundSet->SoundCount; i++)
        {
            if (SoundSet->Sounds[i].Name == scriptNameHash)
                return SoundSet->Sounds[i].MetadataRef;
        }

        return 0;
    }
    public uint GetSound(string scriptName) => GetSound(Game.GetHashKey(scriptName));

    public void SetSound(uint scriptNameHash, uint metadataRef)
    {
        for (var i = 0; i < SoundSet->SoundCount; i++)
        {
            if (SoundSet->Sounds[i].Name == scriptNameHash)
                SoundSet->Sounds[i] = new TSounds()
                {
                    Name = scriptNameHash,
                    MetadataRef = metadataRef
                };
        }
    }
    public void SetSound(string scriptName, uint metadataRef) => SetSound(Game.GetHashKey(scriptName), metadataRef);
}