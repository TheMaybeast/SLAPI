using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SLAPI.Utils;

namespace SLAPI.Memory;

public static class Manager
{
    private static readonly List<IntPtr> AllocatedMemory = new();

    public static unsafe T* Allocate<T>() where T: struct
    {
        var tempStruct = new T();
        var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(tempStruct));
        $"Allocated {typeof(T).Name} @ {(ulong)ptr:X}".ToLog();
        Marshal.StructureToPtr(tempStruct, ptr, false);
        AllocatedMemory.Add(ptr);
        return (T*)ptr;
    }

    public static unsafe void Destroy<T>(T* targetPtr) where T : struct
    {
        var ptr = (IntPtr)targetPtr;
        if (!AllocatedMemory.Contains(ptr)) return;
        Marshal.FreeHGlobal(ptr);
        AllocatedMemory.Remove(ptr);
        $"Freed {typeof(T).Name} @ {(ulong)ptr:X}".ToLog();
    }
}
