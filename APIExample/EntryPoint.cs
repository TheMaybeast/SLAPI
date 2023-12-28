using Rage;
using Rage.Attributes;

using SLAPI;
using SLAPI.Memory;

[assembly: Plugin("SLAPI Example", PrefersSingleInstance = true, ShouldTickInPauseMenu = true)]
namespace APIExample;

public class EntryPoint
{
    public static void Main()
    {
        SLAPI.Main.Initialize();

        while (true)
        {
            GameFiber.Yield();

            var veh = Game.LocalPlayer.Character.CurrentVehicle;
            if (!veh || !veh.HasSiren) continue;

            var hornString = Functions.GetHornStatus(veh) ? "~g~Enabled" : "~r~Disabled";

            var sirenState = Functions.GetSirenState(veh);
            var sirenString = sirenState switch
            {
                eSirenState.OFF => "~r~Off",
                eSirenState.SLOW => "~g~Slow",
                eSirenState.FAST => "~g~Fast",
                eSirenState.WARNING => "~g~Warning",
                _ => "~r~Off",
            };

            Game.DisplaySubtitle(
                $"Horn: {hornString}\n~w~State: {sirenString}\n~w~Time: {Functions.GetSirenTime(veh)}\n~w~LT Time: {Functions.GetSirenLastChangeTime(veh)}\n~w~SoundSet: {Functions.GetVehicleSoundSet(veh).NameHash}");
        }

    }

    private static void OnUnload(bool isTerminating) => SLAPI.Main.Terminate();

    [ConsoleCommand]
    public static void SetVehicleSoundSet(string soundSetName) =>
        Functions.SetVehicleSoundSet(Game.LocalPlayer.Character.CurrentVehicle, soundSetName);

    [ConsoleCommand]
    public static void DumpVehicleSoundSet() =>
        Functions.GetVehicleSoundSet(Game.LocalPlayer.Character.CurrentVehicle).DumpToLog(true);

    [ConsoleCommand]
    public static void GetSound(string scriptName) => Game.Console.Print(
        $"Sound {scriptName}: {Functions.GetVehicleSoundSet(Game.LocalPlayer.Character.CurrentVehicle).GetSound(scriptName)}");

    [ConsoleCommand]
    public static void SetSound(string scriptName, uint metadataRef) => Functions
        .GetVehicleSoundSet(Game.LocalPlayer.Character.CurrentVehicle).SetSound(scriptName, metadataRef);
}