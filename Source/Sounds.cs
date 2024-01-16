using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Rage;
using SLAPI.Memory;
using SLAPI.Utils;

namespace SLAPI;

public unsafe class Sounds
{
    // Pointer to SoundSetStruct relating to this SoundSet
    internal SoundSetStruct* SoundSetStruct;

    internal Sounds(SoundSetStruct* soundSetStruct)
    {
        SoundSetStruct = soundSetStruct;
    }
    
    // Amount of sounds
    public uint Count => SoundSetStruct->SoundCount;
    
    // Indexer to get/set sounds
    public uint this[uint scriptNameHash]
    {
        get => SoundSetStruct->GetSound(scriptNameHash);
        set => SoundSetStruct->SetSound(scriptNameHash, value);
    }
    public (uint scriptName, uint metadataRef) this[int index]
    {
        get => (SoundSetStruct->Sounds[index].Name, SoundSetStruct->Sounds[index].MetadataRef);
        set
        {
            SoundSetStruct->Sounds[index] = new TSounds()
            {
                Name = value.metadataRef,
                MetadataRef = value.scriptName
            };
        }
    }
}

public unsafe class SoundSet
{
    // Stores all SoundSets obtained/created with SLAPI
    internal static readonly List<SoundSet> SoundSets = new();
    
    public bool Vanilla { get; private set; }
    public uint NameHash { get; set; }
    public Sounds Sounds;

    internal SoundSet() {}

    public SoundSet Clone(string name = null)
    {
        // Allocates new SoundSet struct
        var soundSetPtr = Manager.Allocate<SoundSetStruct>();
        
        // Copies source into new SoundSet struct
        var source = Marshal.PtrToStructure<SoundSetStruct>((IntPtr)Sounds.SoundSetStruct);
        Marshal.StructureToPtr(source, (IntPtr)soundSetPtr, false);
        
        // Creates new empty SoundSet
        var soundSet = new SoundSet()
        {
            NameHash = Game.GetHashKey(name ?? $"new-soundset-{Game.TickCount}"),
            Sounds = new Sounds(soundSetPtr)
        };
        SoundSets.Add(soundSet);
        return soundSet;
    }

    public void Dump(bool toConsole)
    {
        $"SoundSet: {NameHash} ({NameHash.Parse()})".ToLog(toConsole: toConsole);
        $"Num of Sounds: {Sounds.Count}".ToLog(toConsole: toConsole);

        for (var i = 0; i < Sounds.Count; i++)
            $"  #{i} {Sounds[i].scriptName.Parse()}: {Sounds[i].metadataRef:X}".ToLog(toConsole: toConsole);
    }
    
    // Gets new or existing instances
    public static SoundSet Get(uint hash)
    {
        // Attempts to return existing custom instance
        var soundSet = SoundSets.FirstOrDefault(x => x.NameHash == hash);
        if (soundSet != null) return soundSet;
        
        // Attempts to return existing vanilla instance
        var audSoundSetPtr = Manager.Allocate<audSoundSet>();
        audSoundSetPtr->Init(hash);
        var soundSetPtr = (SoundSetStruct*)Marshal.ReadIntPtr((IntPtr)audSoundSetPtr);
        Manager.Destroy(audSoundSetPtr);

        // Returns null if no existing SoundSet in game memory
        if ((IntPtr)soundSetPtr == IntPtr.Zero) return null;

        // Creates new vanilla SoundSet and adds to list
        soundSet = new SoundSet()
        {
            NameHash = hash,
            Sounds = new Sounds(soundSetPtr),
            Vanilla = true
        };
        SoundSets.Add(soundSet);
        return soundSet;
    }
    public static SoundSet Get(string name) => Get(Game.GetHashKey(name));
}