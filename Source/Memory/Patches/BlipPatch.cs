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
            if (_location == IntPtr.Zero)
            {
                var addr = Game.FindPattern(Pattern);
                if (addr == IntPtr.Zero) return false;
                $"Got Address: {(long)addr:X}".ToLog(LogLevel.DEBUG);
                _location = addr;
            }

            $"Previous {(long)Marshal.ReadByte(_location, Offset):X}".ToLog(LogLevel.DEBUG);
            Marshal.WriteByte(_location, Offset, 0xFF);
            $"New {(long)Marshal.ReadByte(_location, Offset):X}".ToLog(LogLevel.DEBUG);
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
            $"Previous {(long)Marshal.ReadByte(_location, Offset):X}".ToLog(LogLevel.DEBUG);
            Marshal.WriteByte(_location, Offset, 0x80);
            $"New {(long)Marshal.ReadByte(_location, Offset):X}".ToLog(LogLevel.DEBUG);
            Patched = false;
            return true;
        }
        catch
        {
            return false;
        }
    }
}