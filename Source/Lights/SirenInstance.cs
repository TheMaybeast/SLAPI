using System.Runtime.InteropServices;
using Rage;
using SLAPI.Memory;

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

    public uint SirenOnTime
    {
        get => Data->SirenOnTime;
        set => Data->SirenOnTime = value;
    }
    public float SirenTimeDelta
    {
        get => Data->SirenTimeDelta;
        set => Data->SirenTimeDelta = value;
    }
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
}