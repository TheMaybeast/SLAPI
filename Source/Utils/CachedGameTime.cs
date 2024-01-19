﻿using Rage;

namespace SLAPI.Utils;

internal static class CachedGameTime
{
    private static uint lastGameTime = Game.GameTime;

    public static uint GameTime => lastGameTime;

    internal static void Process()
    {
        while(true)
        {
            lastGameTime = Game.GameTime;
            GameFiber.Yield();
        }
    }
}