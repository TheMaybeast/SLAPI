using Rage;
using System;
using System.Runtime.InteropServices;

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
            if (_location == IntPtr.Zero)
            {
                var addr = Game.FindPattern(Pattern);
                if (addr == IntPtr.Zero) return false;
                Game.LogTrivial($"Got Address: {(long)addr:X}");
                _location = addr;
            }

            Game.LogTrivial($"Previous {(long)Marshal.ReadByte(_location, Offset):X}");
            Marshal.WriteByte(_location, Offset, 0xFF);
            Game.LogTrivial($"New {(long)Marshal.ReadByte(_location, Offset):X}");
            Patched = true;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool Remove()
    {
        try
        {
            Game.LogTrivial($"Previous {(long)Marshal.ReadByte(_location, Offset):X}");
            Marshal.WriteByte(_location, Offset, 0x80);
            Game.LogTrivial($"New {(long)Marshal.ReadByte(_location, Offset):X}");
            Patched = false;
            return true;
        }
        catch
        {
            return false;
        }
    }
}