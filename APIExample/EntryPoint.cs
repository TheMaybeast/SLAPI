using Rage;
using Rage.Attributes;

using SLAPI;
using SLAPI.Memory;

[assembly: Plugin("SLAPI Example", PrefersSingleInstance = true, ShouldTickInPauseMenu = true)]
namespace APIExample
{
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
                    SirenState.OFF => "~r~Off",
                    SirenState.SLOW => "~g~Slow",
                    SirenState.FAST => "~g~Fast",
                    SirenState.WARNING => "~g~Warning",
                    _ => "~r~Off",
                };

                Game.DisplaySubtitle(
                    $"Horn: {hornString}\n~w~State: {sirenString}\n~w~Time: {Functions.GetSirenTime(veh)}\n~w~LT Time: {Functions.GetSirenLastChangeTime(veh)}\n~w~SoundSet: {Functions.GetVehicleSoundSet(veh).NameHash}");
            }

        }

        private static void OnUnload(bool isTerminating) => SLAPI.Main.Terminate();

        [ConsoleCommand]
        public static void InitSirenSoundSet(string soundSetName)
        {
            var soundSet = Functions.GetSoundSet(soundSetName);
            Functions.SetVehicleSoundSet(Game.LocalPlayer.Character.CurrentVehicle, soundSet);
        }
    }
}