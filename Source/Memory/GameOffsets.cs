using System;
using System.Runtime.InteropServices;
using Rage;
using SLAPI.Utils;

namespace SLAPI.Memory;

internal static class GameOffsets
{
    public static int CVehicle_AudVehicleAudioEntity { get; private set; }
    public static int CVehicle_SequentialSirenPressesOffset { get; private set; }
    public static int audVehicleAudioEntity_SirenStateOffset { get; private set; }
    public static int audVehicleAudioEntity_SirenTimeOffset { get; private set; }
    public static int audVehicleAudioEntity_LastSirenChangeTimeOffset { get; private set; }
    public static int audVehicleAudioEntity_SirenSoundsOffset { get; private set; }

    public static bool Init()
    {
#if DEBUG
        "Memory Offsets:".ToLog();
#endif

        var address = Game.FindPattern("48 8B 8B ?? ?? 00 00 E8 ?? ?? ?? ?? 48 8B 8B 70 13 00 00");
        if (AssertAddress(address, nameof(CVehicle_AudVehicleAudioEntity)))
        {
            CVehicle_AudVehicleAudioEntity = Marshal.ReadInt32(address + 3);
        }

        address = Game.FindPattern("44 88 A7 ?? ?? 00 00 44 88 AF ?? ?? 00 00 EB 07");
        if (AssertAddress(address, nameof(CVehicle_SequentialSirenPressesOffset)))
        {
            CVehicle_SequentialSirenPressesOffset = Marshal.ReadInt32(address + 3);
        }

        address = Game.FindPattern("C7 87 ?? ?? 00 00 02 00 00 00 E9 26 FE FF FF");
        if (AssertAddress(address, nameof(audVehicleAudioEntity_SirenStateOffset)))
        {
            audVehicleAudioEntity_SirenStateOffset = Marshal.ReadInt32(address + 2);
        }

        address = Game.FindPattern("81 BF ?? ?? 00 00 5E 01 00");
        if (AssertAddress(address, nameof(audVehicleAudioEntity_SirenTimeOffset)))
        {
            audVehicleAudioEntity_SirenTimeOffset = Marshal.ReadInt32(address + 2);
        }

        address = Game.FindPattern("89 87 ?? ?? 00 00 E9 F4 03 00 00");
        if (AssertAddress(address, nameof(audVehicleAudioEntity_LastSirenChangeTimeOffset)))
        {
            audVehicleAudioEntity_LastSirenChangeTimeOffset = Marshal.ReadInt32(address + 2);
        }

        address = Game.FindPattern("75 6D 48 8D B1 ?? ?? 00 00");
        if (AssertAddress(address, nameof(audVehicleAudioEntity_SirenSoundsOffset)))
        {
            audVehicleAudioEntity_SirenSoundsOffset = Marshal.ReadInt32(address + 5);
        }

        return !_anyAssertFailed;
    }

    private static bool _anyAssertFailed;
    private static bool AssertAddress(IntPtr address, string name)
    {
        if (address != IntPtr.Zero)
        {
#if DEBUG
            $"  {name}: {address}".ToLog();
#endif
            return true;
        }

        $"ERROR: Incompatible game version, couldn't find {name} instance.".ToLog();
        _anyAssertFailed = true;
        return false;
    }
}