using System;
using System.Runtime.InteropServices;
using Rage;
using SLAPI.Memory;
using SLAPI.Utils;

namespace SLAPI.Lights;

public unsafe class SirenInstance(Vehicle vehicle)
{
    public class SirenState
    {
        public bool Rotating;
        public bool Flashing;
        public bool On => Rotating || Flashing;
        public uint LastRotationStartTime;
        public uint LastFlashingStartTime;
        public uint LastStartTime => LastFlashingStartTime == 0 ? LastRotationStartTime : LastFlashingStartTime;
    }
    
    private SirenInstanceData* Data =>
        (SirenInstanceData*)Marshal.ReadIntPtr(vehicle.MemoryAddress + GameOffsets.CVehicle_SirenDataOffset);

    public uint SirenOnTime => Data->SirenOnTime;
    public float SirenTimeDelta => Data->SirenTimeDelta;
    public int TotalSirenBeats => Data->LastSirenBeat;
    public int CurrentSirenBeat => Data->LastSirenBeat % 32;

    public SirenState GetSirenState(int index)
    {
        return new SirenState()
        {
            Rotating = (Data->SirenRotating & (1 << index)) != 0,
            Flashing = (Data->SirenFlashing & (1 << index)) != 0,
            LastRotationStartTime = Data->SirenRotationStart[index],
            LastFlashingStartTime = Data->SirenFlashStart[index]
        };
    }
    
    public int GameTimeOffset => TotalSirenBeats < 0 ? 0 : (int)(Math.Round(SirenOnTime + SirenTimeDelta, 0) - CachedGameTime.GameTime);
    
    public void SetSirenOnTime(uint gameTime, uint threshold = 10, bool createFiberIfJustToggled = true)
    {
        if (TotalSirenBeats >= 0)
        {
            int offset = GameTimeOffset;
            uint onTime = (uint)(gameTime + offset);
            uint currentDiff = (uint)Math.Abs(SirenOnTime - onTime);

            // Siren processing breaks if on time is in the future
            // To prevent constantly resetting slightly (due to rounding error in time delta), 
            // only change if difference from current siren on time exceeds threshold
            if (onTime > CachedGameTime.GameTime || currentDiff < threshold) return;
                
            Data->SirenOnTime = onTime;
            $"Reset siren on time for 0x{vehicle.Handle.Value.ToString("X")} to {gameTime} + {offset} = {onTime}".ToLog(LogLevel.DEBUG);
        } else if (vehicle.IsSirenOn)
        {
            $"Siren is on but beats is <0. Siren may have just been toggled. Yielding one tick and trying again.".ToLog(LogLevel.DEBUG);
            GameFiber.StartNew(() => { 
                GameFiber.Yield();
                if (vehicle) SetSirenOnTime(gameTime, threshold, false);
            });
        }
    }

    public void SetSirenOnTime()
    {
        uint newOnTime = (uint)(CachedGameTime.GameTime - (32 * SirenTimeDelta / TotalSirenBeats));
        SetSirenOnTime(newOnTime);
    }
}