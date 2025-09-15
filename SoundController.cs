using System.Diagnostics;
using Dalamud.Game.ClientState.Conditions;

namespace AudibleCharacterStatus
{
    public class SoundController
    {
        private static float _lowHealthSoundTime;
        private static float _lowMagicSoundTime;

        public static void Update()
        {

            UpdateTimers();

            LowHpTimer();
            LowMpTimer();
        }

        private static void LowHpTimer()
        {
            var localPlayer = Service.ClientState.LocalPlayer;
            if (localPlayer is null) return;

            if (!Service.Config.LowHealthSoundEnabled) return;

            //Do not allow sound if ToggleCombat is disabled, and the player is not in combat
            if (!Service.Config.ToggleCombat && !Service.Condition[ConditionFlag.InCombat])  return;    
            

            if (_lowHealthSoundTime is <= 0 and > -100 && localPlayer.CurrentHp <= localPlayer.MaxHp * (Service.Config.LowHealthPercent / 100f) && localPlayer.CurrentHp > 0)
            {
                SoundEngine.PlaySound(Service.Config.LowHealthSoundPath, Service.Config.LowHealthSoundVolume);
                _lowHealthSoundTime += Service.Config.LowHealthSoundDelay;
            }
            else if (localPlayer.CurrentHp > localPlayer.MaxHp * (Service.Config.LowHealthPercent / 100f) || localPlayer.CurrentHp == 0 ||
                     _lowHealthSoundTime == -100 && Service.Config.LowHealthSoundDelay != -100)
            {
                _lowHealthSoundTime = 0;
            }
        }

        private static void LowMpTimer()
        {
            var localPlayer = Service.ClientState.LocalPlayer;
            if (localPlayer is null) return;
            if (Process.GetCurrentProcess().Id != ProcessUtils.GetForegroundProcessId()) return;

            if (!Service.Config.LowMagicSoundEnabled) return;

            //Do not allow sound if ToggleCombat is disabled, and the player is not in combat
            if (!Service.Config.ToggleCombat && !Service.Condition[ConditionFlag.InCombat]) return;

            if (localPlayer.MaxMp <= 0) return;

            if (_lowMagicSoundTime is <= 0 and > -100 && localPlayer.CurrentMp <= localPlayer.MaxMp * (Service.Config.LowMagicPercent / 100f) && localPlayer.CurrentHp > 0)
            {
                SoundEngine.PlaySound(Service.Config.LowMagicSoundPath, Service.Config.LowMagicSoundVolume);
                _lowMagicSoundTime += Service.Config.LowMagicSoundDelay;
            }
            else if (localPlayer.CurrentMp > localPlayer.MaxMp * (Service.Config.LowMagicPercent / 100f) || localPlayer.CurrentHp == 0 ||
                     _lowMagicSoundTime == -100 && Service.Config.LowMagicSoundDelay != -100)
            {
                _lowMagicSoundTime = 0;
            }
        }

        private static void UpdateTimers()
        {
            if (_lowHealthSoundTime > 0)
            {
                _lowHealthSoundTime -= Service.Framework.UpdateDelta.Ticks / 10000000f;
            }

            if (_lowMagicSoundTime > 0)
            {
                _lowMagicSoundTime -= Service.Framework.UpdateDelta.Ticks / 10000000f;
            }
        }
    }
}
