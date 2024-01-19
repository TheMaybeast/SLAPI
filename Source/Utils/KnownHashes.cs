using System.Collections.Generic;
using System.Linq;
using Rage;

namespace SLAPI.Utils;

public enum HashType
{
    MetadataRef,
    ScriptName,
    SoundSet,
}

public static class KnownHashes
{
    private static Dictionary<uint, string> MetadataRefs = new()
    {
        { 0x16D47B, "blip" },
        { 0x16ADFD, "horn" },
        { 0x16ACFD, "policevehsirens/fucked" }, { 0x16BD4D, "policevehsirens2/fucked" },
        { 0x16C4E1, "policevehsirens3/fucked" }, { 0x16C6C4, "policevehsirens4/fucked" },
        { 0x16CC55, "policevehsirens5/fucked" }, { 0x16D1E8, "policevehsirens6/fucked" },
        { 0x16A7BE, "fucked_one_shot" },
        { 0x16AD1E, "policevehsirens/fast" }, { 0x16BD6E, "policevehsirens2/fast" },
        { 0x16BD8F, "policevehsirens3/fast" }, { 0x16C18C, "policevehsirens4/fast" },
        { 0x16CBFC, "policevehsirens5/fast" }, { 0x16D1AB, "policevehsirens6/fast" },
        { 0x16AC68, "policevehsirens/slow" }, { 0x16B836, "policevehsirens2/slow" },
        { 0x16BE16, "policevehsirens3/slow" }, { 0x16C16B, "policevehsirens4/slow" },
        { 0x16C8A7, "policevehsirens5/slow" }, { 0x16CE54, "policevehsirens6/slow" },
        { 0x16B2C1, "horn_fast" },
        { 0x16A04E, "horn_slow" },
        { 0x16AD3F, "warning" },
    };
    private static Dictionary<string, uint> MetadataRefInverse = MetadataRefs.ToDictionary(x => x.Value, x => x.Key);
    
    private static Dictionary<uint, string> ScriptNames = new()
    {
        { 0x26D96F23, "blip" },
        { 0x3E34681F, "horn" },
        { 0x4DD32855, "fucked" },
        { 0x5417639D, "fucked_one_shot" },
        { 0x64B8047E, "fast" },
        { 0xC4EE147F, "slow" },
        { 0xCB4A8157, "horn_fast" },
        { 0xD91C6C0F, "horn_slow" },
        { 0xE91F183C, "warning" },
    };
    private static Dictionary<string, uint> ScriptNamesInverse = ScriptNames.ToDictionary(x => x.Value, x => x.Key);

    private static Dictionary<uint, string> SoundSets = new()
    {
        { 0x49DF3CF8, "policevehsirens" },
        { 0x62CB156B, "policevehsirens2" },
        { 0x510C71EE, "policevehsirens3" },
        { 0xB1FCB3CD, "policevehsirens4" },
        { 0x9F4F8E73, "policevehsirens5" },
        { 0x8E53EC78, "policevehsirens6" },
    };
    private static Dictionary<string, uint> SoundSetsInverse = SoundSets.ToDictionary(x => x.Value, x => x.Key);
    
    public static string HashParse(this uint hash, HashType hashType)
    {
        return hashType switch
        {
            HashType.MetadataRef => MetadataRefs.TryGetValue(hash, out var metadataRefName)
                ? metadataRefName
                : $"{hash:X}",
            HashType.ScriptName => ScriptNames.TryGetValue(hash, out var scriptName) ? scriptName : $"{hash:X}",
            HashType.SoundSet => SoundSets.TryGetValue(hash, out var soundSetName) ? soundSetName : $"{hash:X}",
            _ => $"{hash:X}"
        };
    }

    public static uint HashParse(this string name, HashType hashType)
    {
        return hashType switch
        {
            HashType.MetadataRef => MetadataRefInverse.TryGetValue(name, out var metadataRef)
                ? metadataRef
                : Game.GetHashKey(name),
            HashType.ScriptName => ScriptNamesInverse.TryGetValue(name, out var scriptName)
                ? scriptName
                : Game.GetHashKey(name),
            HashType.SoundSet => SoundSetsInverse.TryGetValue(name, out var soundSet)
                ? soundSet
                : Game.GetHashKey(name),
            _ => Game.GetHashKey(name)
        };
    }
}