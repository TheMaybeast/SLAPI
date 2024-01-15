using Rage;
using Rage.Attributes;
using Rage.Native;
using SLAPI;
using SLAPI.Memory;
using SLAPI.Utils;

[assembly: Plugin("SLAPI Example", PrefersSingleInstance = true, ShouldTickInPauseMenu = false)]
namespace APIExample;

public class EntryPoint
{
    public static void Main()
    {
        _ = new SLAPIMain();
        SLVehicle.SirenBlipOnFastToggle = false;

        while (true)
        {
            GameFiber.Yield();

            var veh = Game.LocalPlayer.Character.CurrentVehicle;
            if (!veh || veh.IsDead || !veh.HasSiren) continue;

            var slVehicle = veh.GetSLVehicle();

            var hornString = slVehicle.HornStatus ? "~g~Enabled" : "~r~Disabled";

            var sirenState = slVehicle.SirenState;
            var sirenString = sirenState switch
            {
                eSirenState.OFF => "~r~Off",
                eSirenState.SLOW => "~g~Slow",
                eSirenState.FAST => "~g~Fast",
                eSirenState.WARNING => "~g~Warning",
                _ => "~r~Off",
            };

            DisplayHelp(
                $"Horn: {hornString}\n~w~State: {sirenString}\n~w~Time: {slVehicle.SirenTime}\n~w~LT Time: {slVehicle.SirenLastChangeTime}\n~w~SoundSet: {slVehicle.SirenSounds.NameHash}");
        }
    }

    private static void DisplayHelp(string text)
    {
        NativeFunction.Natives.x8509B634FBE7DA11("STRING");
        NativeFunction.Natives.x6C188BE134E074AA(text);
        NativeFunction.Natives.x238FFE5C7B0498A6(0, false, false, 1);
    }

    [ConsoleCommand]
    public static void SetVehicleSoundSet(string soundSetName)
    {
        var slVehicle = Game.LocalPlayer.Character.CurrentVehicle.GetSLVehicle();
        slVehicle.SirenSounds = SoundSet.Get(soundSetName);
    }

    [ConsoleCommand]
    public static void DumpVehicleSoundSet() =>
        Game.LocalPlayer.Character.CurrentVehicle.GetSLVehicle().DefaultSirenSounds.Dump(true);

    [ConsoleCommand]
    public static void DumpSoundSet(string soundSetName) => SoundSet.Get(soundSetName)?.Dump(true);
}