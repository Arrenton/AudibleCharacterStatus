using Dalamud.Configuration;
using Dalamud.Plugin;

namespace AudibleCharacterStatus
{
    public class Configuration : IPluginConfiguration
    {
        int IPluginConfiguration.Version { get; set; }

        #region Saved configuration values

        public int SoundDeviceId { get; set; } = -1;
        public bool PlayInBackground { get; set; } = false;
        public bool ToggleCombat { get; set; } = false;

        #region Low HP

        public float LowHealthPercent { get; set; } = 25f;
        public bool LowHealthSoundEnabled { get; set; } = false;
        public float LowHealthSoundVolume { get; set; } = 1.0f;
        public string LowHealthSoundPath { get; set; } = "";
        public float LowHealthSoundDelay { get; set; } = 0.4f;

        #endregion

        #region Low MP

        public float LowMagicPercent { get; set; } = 25f;
        public bool LowMagicSoundEnabled { get; set; } = false;
        public float LowMagicSoundVolume { get; set; } = 1.0f;
        public string LowMagicSoundPath { get; set; } = "";
        public float LowMagicSoundDelay { get; set; } = 0.4f;

        #endregion

        #endregion

        private DalamudPluginInterface _pluginInterface;

        public void Initialize(DalamudPluginInterface pi)
        {
            this._pluginInterface = pi;
        }

        public void Save()
        {
            this._pluginInterface.SavePluginConfig(this);
        }
    }
}
