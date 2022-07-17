using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.ImGuiFileDialog;

namespace AudibleCharacterStatus.Windows
{
    public class PluginConfiguriation : Window
    {
        private readonly FileDialogManager _dialogManager;

        // Methods
        public PluginConfiguriation() : base("Audible Character Status Configuration")
        {
            _dialogManager = SetupDialogManager();
            IsOpen = false;
            Size = new(810, 520);
            SizeCondition = ImGuiCond.FirstUseEver;
        }

        public override void Draw()
        {
            var deviceId = Service.Config.SoundDeviceId;
            var deviceName = "Default";
            if (deviceId > -1)
            {
                if (deviceId < WaveOut.DeviceCount)
                {
                    deviceName = WaveOut.GetCapabilities(deviceId).ProductName;
                }
                else
                {
                    Service.Config.SoundDeviceId = -1;
                }
            }
            else
            {
                Service.Config.SoundDeviceId = -1;
            }

            if (ImGui.BeginCombo("Sound Device", deviceName))
            {
                if (ImGui.Selectable("Default"))
                {
                    Service.Config.SoundDeviceId = -1;
                }

                for (int i = 0; i < WaveOut.DeviceCount; i++)
                {
                    if (ImGui.Selectable(WaveOut.GetCapabilities(i).ProductName))
                    {
                        Service.Config.SoundDeviceId = i;
                    }
                }

                ImGui.EndCombo();
            }

            var playInBackground = Service.Config.PlayInBackground;
            if (ImGui.Checkbox("Play Sound in Background##General", ref playInBackground))
            {
                Service.Config.PlayInBackground = playInBackground;
            }
            if (ImGui.IsItemHovered())
            {
                Tooltip("Allows the sound from this plugin to play when the game is in the background.");
            }
            ImGui.NewLine();

            LowHpSettings();
            LowMpSettings();

            _dialogManager.Draw();

            ImGui.NewLine();

            ImGui.Separator();
            if (ImGui.Button("Save"))
            {
                Service.Config.Save();
            }
        }
        private void LowHpSettings()
        {
            if (!ImGui.TreeNode("Low HP")) return;

            ImGui.Text("Plays a sound when HP is low, based on the set percentage.");
            var enabled = Service.Config.LowHealthSoundEnabled;
            if (ImGui.Checkbox("Enabled##LowHpSound", ref enabled))
            {
                Service.Config.LowHealthSoundEnabled = enabled;
            }

            var lowHealthSoundPath = Service.Config.LowHealthSoundPath;
            ImGui.Text("Sound File Path");

            ImGui.InputText("##LowHpPath", ref lowHealthSoundPath, 512, ImGuiInputTextFlags.ReadOnly);

            ImGui.SameLine();
            if (ImGui.Button("Browse...##LowHpPath"))
            {
                var startDir = Path.GetDirectoryName(Service.Config.LowHealthSoundPath);

                void UpdatePath(bool success, List<string> paths)
                {
                    if (success && paths.Count > 0)
                    {
                        Service.Config.LowHealthSoundPath = paths[0];
                    }
                }

                _dialogManager.OpenFileDialog("Choose an audio file for Low HP", "Audio Files{.wav,.mp3}", UpdatePath, 1, startDir);

            }

            ImGui.SameLine();
            if (ImGui.Button("Test##LowHpPath"))
            {
                SoundEngine.PlaySound(Service.Config.LowHealthSoundPath, Service.Config.LowHealthSoundVolume);
            }
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(1, 0, 0, 1), FindSoundMessage(Service.Config.LowHealthSoundPath));

            var lowHpPercent = Service.Config.LowHealthPercent;
            if (ImGui.SliderFloat("Percent To Trigger Low HP", ref lowHpPercent, 0, 100, "%.1f%%"))
            {
                Service.Config.LowHealthPercent = lowHpPercent;
            }

            var lowHealthVolume = Service.Config.LowHealthSoundVolume * 100f;
            if (ImGui.SliderFloat("Volume##LowHP", ref lowHealthVolume, 0, 100.0f, "%.1f%%"))
            {
                Service.Config.LowHealthSoundVolume = Math.Min(lowHealthVolume / 100f, 1);
            }

            var lowHealthDelay = Service.Config.LowHealthSoundDelay;
            if (ImGui.InputFloat("Loop Time##LowHP", ref lowHealthDelay, 0.05f, 0.2f))
            {
                if (lowHealthDelay < 0.05f && lowHealthDelay != -100) lowHealthDelay = 0.05f;
                Service.Config.LowHealthSoundDelay = lowHealthDelay;
            }

            if (ImGui.IsItemHovered())
            {
                Tooltip("Sets the timer for when the sound will play again. (In Seconds)\nEx: 0.4 will play the sound every 0.4 seconds.\nSet to -100 for no looping");
            }
            ImGui.TreePop();
        }
        private void LowMpSettings()
        {
            if (!ImGui.TreeNode("Low MP")) return;

            ImGui.Text("Plays a sound when MP is low, based on the set percentage.");
            var enabled = Service.Config.LowMagicSoundEnabled;
            if (ImGui.Checkbox("Enabled##LowMpSound", ref enabled))
            {
                Service.Config.LowMagicSoundEnabled = enabled;
            }

            var lowMagicSoundPath = Service.Config.LowMagicSoundPath;
            ImGui.Text("Sound File Path");

            ImGui.InputText("##LowMpPath", ref lowMagicSoundPath, 512, ImGuiInputTextFlags.ReadOnly);

            ImGui.SameLine();
            if (ImGui.Button("Browse...##LowMpPath"))
            {
                var startDir = Path.GetDirectoryName(Service.Config.LowMagicSoundPath);

                void UpdatePath(bool success, List<string> paths)
                {
                    if (success && paths.Count > 0)
                    {
                        Service.Config.LowMagicSoundPath = paths[0];
                    }
                }

                _dialogManager.OpenFileDialog("Choose an audio file for Low MP", "Audio Files{.wav,.mp3}", UpdatePath, 1, startDir);

            }

            ImGui.SameLine();
            if (ImGui.Button("Test##LowMpPath"))
            {
                SoundEngine.PlaySound(Service.Config.LowMagicSoundPath, Service.Config.LowMagicSoundVolume);
            }
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(1, 0, 0, 1), FindSoundMessage(Service.Config.LowMagicSoundPath));

            var lowMpPercent = Service.Config.LowMagicPercent;
            if (ImGui.SliderFloat("Percent To Trigger Low MP", ref lowMpPercent, 0, 100, "%.1f%%"))
            {
                Service.Config.LowMagicPercent = lowMpPercent;
            }

            var lowMagicVolume = Service.Config.LowMagicSoundVolume * 100f;
            if (ImGui.SliderFloat("Volume##LowMP", ref lowMagicVolume, 0, 100.0f, "%.1f%%"))
            {
                Service.Config.LowMagicSoundVolume = Math.Min(lowMagicVolume / 100f, 1);
            }

            var lowMagicDelay = Service.Config.LowMagicSoundDelay;
            if (ImGui.InputFloat("Loop Time##LowMP", ref lowMagicDelay, 0.05f, 0.2f))
            {
                if (lowMagicDelay < 0.05f && lowMagicDelay != -100) lowMagicDelay = 0.05f;
                Service.Config.LowMagicSoundDelay = lowMagicDelay;
            }

            if (ImGui.IsItemHovered())
            {
                Tooltip("Sets the timer for when the sound will play again. (In Seconds)\nEx: 0.4 will play the sound every 0.4 seconds.\nSet to -100 for no looping");
            }
            ImGui.TreePop();
        }

        private void Tooltip(string message)
        {
            Vector2 m = ImGui.GetIO().MousePos;
            ImGui.SetNextWindowPos(new Vector2(m.X + 20, m.Y + 20));
            ImGui.Begin("ACSTT", ImGuiWindowFlags.Tooltip | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoTitleBar);
            ImGui.Text(message);
            ImGui.End();
        }
        
        /// <summary>
        /// Returns a message depending on if an audio file is found or not, and if it is not a supported format.
        /// Supported formats are mp3, wav, and ogg.
        /// </summary>
        /// <param name="path">Path to image</param>
        /// <returns>string</returns>
        private string FindSoundMessage(string path)
        {
            if (path.IsNullOrEmpty()) return "";

            var fileFound = File.Exists(path);

            if (!fileFound) return "File not found.";

            string[] supportedImages = { ".wav", ".mp3" };

            var isImage = supportedImages.Any(ext => Path.GetExtension(path).Trim() == ext);

            return isImage ? "" : "File is not supported. Use MP3, or WAV.";
        }

        private FileDialogManager SetupDialogManager()
        {
            var fileManager = new FileDialogManager { AddedWindowFlags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking };

            // Remove Videos and Music.
            fileManager.CustomSideBarItems.Add(("Videos", string.Empty, 0, -1));

            return fileManager;
        }
    }
}
