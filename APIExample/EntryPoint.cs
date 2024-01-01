﻿using Rage;
using Rage.Attributes;
using Rage.Native;
using SLAPI;
using SLAPI.Memory;

[assembly: Plugin("SLAPI Example", PrefersSingleInstance = true, ShouldTickInPauseMenu = false)]
namespace APIExample;

public class EntryPoint
{
    public static void Main()
    {
        _ = new SLAPIMain();

        while (true)
        {
            GameFiber.Yield();

            var veh = Game.LocalPlayer.Character.CurrentVehicle;
            if (!veh || veh.IsDead || !veh.HasSiren) continue;

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

            DisplayHelp(
                $"Horn: {hornString}\n~w~State: {sirenString}\n~w~Time: {Functions.GetSirenTime(veh)}\n~w~LT Time: {Functions.GetSirenLastChangeTime(veh)}\n~w~SoundSet: {Functions.GetVehicleSoundSet(veh).NameHash}");
        }
    }

    private static void DisplayHelp(string text)
    {
        NativeFunction.Natives.x8509B634FBE7DA11("STRING");
        NativeFunction.Natives.x6C188BE134E074AA(text);
        NativeFunction.Natives.x238FFE5C7B0498A6(0, false, false, 1);
    }

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