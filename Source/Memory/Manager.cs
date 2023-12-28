﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SLAPI.Utils;

namespace SLAPI.Memory;

public static class Manager
{
    private static List<IntPtr> AllocatedMemory = new();

    public static unsafe T* Allocate<T>() where T: struct
    {
        var tempStruct = new T();
        var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(tempStruct));
#if DEBUG
        $"Allocated {typeof(T).Name} at {ptr}".ToLog();
#endif
        Marshal.StructureToPtr(tempStruct, ptr, false);
        AllocatedMemory.Add(ptr);
        return (T*)ptr;
    }

    public static unsafe void Destroy<T>(T* targetPtr) where T : struct
    {
        var ptr = (IntPtr)targetPtr;
#if DEBUG
        $"Attempting to free {typeof(T).Name} at {ptr}".ToLog();
#endif
        if (!AllocatedMemory.Contains(ptr)) return;
        Marshal.FreeHGlobal(ptr);
        AllocatedMemory.Remove(ptr);
#if DEBUG
        $"Freed {typeof(T).Name} at {ptr}".ToLog();
#endif
    }

    internal static void Cleanup()
    {
        foreach (var ptr in AllocatedMemory)
        {
            Marshal.FreeHGlobal(ptr);
#if DEBUG
            $"Freed managed memory at {ptr}".ToLog();
#endif
        }
    }
}