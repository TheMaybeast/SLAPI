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

                /*if (Game.IsKeyDown(Keys.F5))
                    instance.audSoundSet->DumpToLog();
                if (Game.IsKeyDown(Keys.F6))
                {
                    var soundSet = Manager.Allocate<audSoundSet>();
                    soundSet->Init("policevehsirens2");
                    var sound = soundSet->Data->GetSound("slow").MetadataRef;
                    instance.audSoundSet->Data->SetSound("slow", sound);
                    Manager.Destroy(soundSet);
                }*/

                Game.DisplaySubtitle(
                    $"Horn: {hornString}\n~w~State: {sirenString}\n~w~Time: {Functions.GetSirenTime(veh)}\n~w~LT Time: {Functions.GetSirenLastChangeTime(veh)}\n~w~SoundSet: {Functions.GetVehicleSoundSet(veh).NameHash}");
            }

        }

        private static void OnUnload(bool isTerminating) => SLAPI.Main.Terminate();

        /*[ConsoleCommand]
        public static void DumpSirenSoundSet() =>
            Game.LocalPlayer.Character.CurrentVehicle.GetSLInstance().audSoundSet->DumpToLog();

        

        [ConsoleCommand]
        public static void ResetSirenSoundSet() =>
            Game.LocalPlayer.Character.CurrentVehicle.GetSLInstance().Reset();*/

        [ConsoleCommand]
        public static void InitSirenSoundSet(string soundSetName)
        {
            var soundSet = Functions.GetSoundSet(soundSetName);
            Functions.SetVehicleSoundSet(Game.LocalPlayer.Character.CurrentVehicle, soundSet);
        }
    }
}