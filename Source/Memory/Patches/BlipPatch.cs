using Rage;
using System;
using System.Runtime.InteropServices;
using SLAPI.Utils;

namespace SLAPI.Memory.Patches;

internal static class BlipPatch
{
    private static IntPtr _location = IntPtr.Zero;

    public static bool Patched;

    private const string Pattern = "F6 83 ?? ?? 00 00 80 75 08";
    private const int Offset = 6;

    public static bool Patch()
    {
        try
        {
            "Applying Blip Patch:".ToLog();
            if (_location == IntPtr.Zero)
            {
                var addr = Game.FindPattern(Pattern);
                if (addr == IntPtr.Zero)
                {
                    "  Failed to apply patch: Location not found".ToLog(LogLevel.ERROR);
                    return false;
                }
                $"  Located @ {(long)addr:X}".ToLog();
                _location = addr;
            }

            $"  Previous bytes: {(long)Marshal.ReadByte(_location, Offset):X}".ToLog();
            Marshal.WriteByte(_location, Offset, 0xFF);
            $"  New bytes: {(long)Marshal.ReadByte(_location, Offset):X}".ToLog();
            Patched = true;
            "  Applied patch successfully!".ToLog();
            return true;
        }
        catch
        {
            "  Failed to apply patch: Unknown error".ToLog(LogLevel.ERROR);
            return false;
        }
    }

    public static bool Remove()
    {
        try
        {
            "Removing Blip Patch:".ToLog();
            $"  Previous {(long)Marshal.ReadByte(_location, Offset):X}".ToLog();
            Marshal.WriteByte(_location, Offset, 0x80);
            $"  New {(long)Marshal.ReadByte(_location, Offset):X}".ToLog();
            Patched = false;
            "   Removed patch successfully".ToLog();
            return true;
        }
        catch
        {
            "   Failed to remove patch: Unknown error".ToLog(LogLevel.ERROR);
            return false;
        }
    }
}