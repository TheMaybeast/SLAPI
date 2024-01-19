using Rage;
using Rage.Attributes;
using SLAPI;
using SLAPI.Memory;
using SLAPI.Utils;

[assembly: Plugin("SLAPI Example", PrefersSingleInstance = true, ShouldTickInPauseMenu = false)]
namespace APIExample;

public class EntryPoint
{
    public static void Main()
    {
        _ = new SLAPIMain()
        {
            SirenBlipPatch = false
        };

        while (true)
        {
            GameFiber.Yield();

            var veh = Game.LocalPlayer.Character.CurrentVehicle;
            if (!veh || veh.IsDead || !veh.HasSiren) continue;

            var slVehicle = veh.GetSLVehicle();

            var hornString = slVehicle.Sounds.HornStatus ? "~g~Enabled" : "~r~Disabled";

            var sirenState = slVehicle.Sounds.SirenState;
            var sirenString = sirenState switch
            {
                eSirenState.OFF => "~r~Off",
                eSirenState.SLOW => "~g~Slow",
                eSirenState.FAST => "~g~Fast",
                eSirenState.WARNING => "~g~Warning",
                _ => "~r~Off",
            };

            var text = $"~w~Horn: {hornString}\n";
            text += $"~w~State: {sirenString}\n";
            text += $"~w~Times: {slVehicle.Sounds.SirenTime} (Siren) / {slVehicle.Lights.SirenInstance.SirenTimeDelta} (Lights)\n";
            text += $"~w~LT Time: {slVehicle.Sounds.SirenLastChangeTime}\n";
            text += $"~w~SoundSet: {slVehicle.Sounds.SirenSounds?.NameHash.HashParse(HashType.SoundSet)}\n";
            var lightSirenState = slVehicle.Lights.SirenInstance.GetSirenState(0);
            var status = lightSirenState.On ? "~g~Enabled~w~" : "~r~Disabled~w~";
            text += $"Siren 1: {status} | LT: {lightSirenState.LastStartTime}";
            Game.DisplayHelp(text);
        }
    }

    [ConsoleCommand]
    public static void SetVehicleSoundSet(string soundSetName)
    {
        var slVehicle = Game.LocalPlayer.Character.CurrentVehicle.GetSLVehicle();
        slVehicle.Sounds.SirenSounds = SoundSet.Get(soundSetName); 
    }

    [ConsoleCommand]
    public static void DumpVehicleSoundSet() =>
        Game.LocalPlayer.Character.CurrentVehicle.GetSLVehicle().Sounds.DefaultSirenSounds?.Dump(true);

    [ConsoleCommand]
    public static void DumpSoundSet(string soundSetName) => SoundSet.Get(soundSetName)?.Dump(true);

    [ConsoleCommand]
    public static void ResetVehicleSoundSet()
    {
        var slVehicle = Game.LocalPlayer.Character.CurrentVehicle.GetSLVehicle();
        slVehicle.Sounds.SirenSounds = slVehicle.Sounds.DefaultSirenSounds;
    }
}