using System;
using System.Runtime.InteropServices;
using Rage;
using SLAPI.Utils;

namespace SLAPI.Memory;

internal static unsafe class GameFunctions
{
    public delegate void InitAudSoundSetDelegate(audSoundSet* soundSet, uint soundSetNameHash);
    public delegate bool IsHornOnDelegate(IntPtr vehicle);

    public static InitAudSoundSetDelegate InitAudSoundSet { get; private set; }
    public static IsHornOnDelegate IsHornOn { get; private set; }

    public static bool Init()
    {
#if DEBUG
        "Memory Functions:".ToLog();
#endif

        var address = Game.FindPattern("E8 ?? ?? ?? ?? 4C 39 6D E0");
        if (AssertAddress(address, nameof(InitAudSoundSet)))
        {
            InitAudSoundSet = Marshal.GetDelegateForFunctionPointer<InitAudSoundSetDelegate>(address);
        }

        address = Game.FindPattern("48 8B 89 ?? 09 00 00 48 85 C9 0F 85");
        if (AssertAddress(address, nameof(IsHornOn)))
        {
            IsHornOn = Marshal.GetDelegateForFunctionPointer<IsHornOnDelegate>(address);
        }

        return !_anyAssertFailed;
    }

    private static bool _anyAssertFailed;
    private static bool AssertAddress(IntPtr address, string name)
    {
        if (address != IntPtr.Zero)
        {
#if DEBUG
            $"  {name} @ {address}".ToLog();
#endif
            return true;
        }

        $"ERROR: Incompatible game version, couldn't find {name} instance.".ToLog();
        _anyAssertFailed = true;
        return false;
    }
}