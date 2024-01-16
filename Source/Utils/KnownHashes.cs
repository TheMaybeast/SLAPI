using System.Collections.Generic;
using System.Linq;

namespace SLAPI.Utils;

internal static class KnownHashes
{
    private static Dictionary<uint, string> HashDict = new()
    {
        // Metadata Refs
        { 0x16D47B, "blip"  },
        { 0x16ADFD, "horn"  },
        { 0x16ACFD, "policevehsirens/fucked"  }, { 0x16BD4D, "policevehsirens2/fucked"  }, { 0x16C4E1, "policevehsirens3/fucked"  }, { 0x16C6C4, "policevehsirens4/fucked"  }, { 0x16CC55, "policevehsirens5/fucked"  }, { 0x16D1E8, "policevehsirens6/fucked"  },
        { 0x16A7BE, "fucked_one_shot"  },
        { 0x16AD1E, "policevehsirens/fast"  }, { 0x16BD6E, "policevehsirens2/fast"  }, { 0x16BD8F, "policevehsirens3/fast"  }, { 0x16C18C, "policevehsirens4/fast"  }, { 0x16CBFC, "policevehsirens5/fast"  }, { 0x16D1AB, "policevehsirens6/fast"  },
        { 0x16AC68, "policevehsirens/slow"  }, { 0x16B836, "policevehsirens2/slow"  }, { 0x16BE16, "policevehsirens3/slow"  }, { 0x16C16B, "policevehsirens4/slow"  }, { 0x16C8A7, "policevehsirens5/slow"  }, { 0x16CE54, "policevehsirens6/slow"  },
        { 0x16B2C1, "horn_fast"  },
        { 0x16A04E, "horn_slow"  },
        { 0x16AD3F, "warning"  },
        
        // SoundSets
        { 0x49DF3CF8, "policevehsirens" },
        { 0x62CB156B, "policevehsirens2" },
        { 0x510C71EE, "policevehsirens3" },
        { 0xB1FCB3CD, "policevehsirens4" },
        { 0x9F4F8E73, "policevehsirens5" },
        { 0x8E53EC78, "policevehsirens6" },
    };
    private static Dictionary<string, uint> StringDict = HashDict.ToDictionary(x => x.Value, x => x.Key);
    
    public static string Parse(this uint hash) => HashDict.TryGetValue(hash, out var hashName) ? hashName : $"{hash:X}";
    public static uint Parse(this string text) => StringDict.TryGetValue(text, out var hash) ? hash : uint.Parse(text);
}