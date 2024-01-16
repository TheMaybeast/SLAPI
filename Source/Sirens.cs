using SLAPI.Utils;

namespace SLAPI;

public unsafe class SirenSoundSet : SoundSet
{
    // Properties
    public uint Blip
    {
        get => Sounds.SoundSetStruct->GetSound("blip");
        set
        {
            if (Vanilla)
            {
                "Attempting to edit a vanilla SoundSet".ToLog(LogLevel.ERROR);
                return;
            }

            Sounds.SoundSetStruct->SetSound("blip", value);
        }
    }
    public uint Horn
    {
        get => Sounds.SoundSetStruct->GetSound("horn");
        set
        {
            if (Vanilla)
            {
                "Attempting to edit a vanilla SoundSet".ToLog(LogLevel.ERROR);
                return;
            }

            Sounds.SoundSetStruct->SetSound("horn", value);
        }
    }
    public uint Fucked
    {
        get => Sounds.SoundSetStruct->GetSound("fucked");
        set
        {
            if (Vanilla)
            {
                "Attempting to edit a vanilla SoundSet".ToLog(LogLevel.ERROR);
                return;
            }

            Sounds.SoundSetStruct->SetSound("fucked", value);
        }
    }
    public uint FuckedOneShot
    {
        get => Sounds.SoundSetStruct->GetSound("fucked_one_shot");
        set
        {
            if (Vanilla)
            {
                "Attempting to edit a vanilla SoundSet".ToLog(LogLevel.ERROR);
                return;
            }

            Sounds.SoundSetStruct->SetSound("fucked_one_shot", value);
        }
    }
    public uint Fast
    {
        get => Sounds.SoundSetStruct->GetSound("fast");
        set
        {
            if (Vanilla)
            {
                "Attempting to edit a vanilla SoundSet".ToLog(LogLevel.ERROR);
                return;
            }

            Sounds.SoundSetStruct->SetSound("fast", value);
        }
    }
    public uint Slow
    {
        get => Sounds.SoundSetStruct->GetSound("slow");
        set
        {
            if (Vanilla)
            {
                "Attempting to edit a vanilla SoundSet".ToLog(LogLevel.ERROR);
                return;
            }

            Sounds.SoundSetStruct->SetSound("slow", value);
        }
    }
    public uint HornFast
    {
        get => Sounds.SoundSetStruct->GetSound("horn_fast");
        set
        {
            if (Vanilla)
            {
                "Attempting to edit a vanilla SoundSet".ToLog(LogLevel.ERROR);
                return;
            }

            Sounds.SoundSetStruct->SetSound("horn_fast", value);
        }
    }
    public uint HornSlow
    {
        get => Sounds.SoundSetStruct->GetSound("horn_slow");
        set
        {
            if (Vanilla)
            {
                "Attempting to edit a vanilla SoundSet".ToLog(LogLevel.ERROR);
                return;
            }

            Sounds.SoundSetStruct->SetSound("horn_slow", value);
        }
    }
    public uint Warning
    {
        get => Sounds.SoundSetStruct->GetSound("warning");
        set
        {
            if (Vanilla)
            {
                "Attempting to edit a vanilla SoundSet".ToLog(LogLevel.ERROR);
                return;
            }

            Sounds.SoundSetStruct->SetSound("warning", value);
        }
    }
}