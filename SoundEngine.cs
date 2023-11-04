using Dalamud.Utility;
using NAudio.Wave;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace AudibleCharacterStatus
{
    public static class SoundEngine
    {
        // Copied from PeepingTom plugin
        public static void PlaySound(string path, float volume = 1.0f)
        {
            if (path.IsNullOrEmpty() || !File.Exists(path)) return;

            if (Process.GetCurrentProcess().Id != ProcessUtils.GetForegroundProcessId() && !Service.Config.PlayInBackground) return;

            var soundDevice = Service.Config.SoundDeviceId;
            if (soundDevice < -1 || soundDevice > WaveOut.DeviceCount)
            {
                soundDevice = -1;
            }

            new Thread(() => {
                WaveStream reader;
                try
                {
                    reader = new AudioFileReader(path);
                }
                catch (Exception e)
                {
                    Service.PluginLog.Error($"Could not play sound file: {e.Message}");
                    return;
                }

                volume = Math.Max(0, Math.Min(volume, 1));

                using WaveChannel32 channel = new(reader)
                {
                    Volume = 1 - (float)Math.Sqrt(1 - (volume * volume)),
                    PadWithZeroes = false,
                };

                using (reader)
                {
                    using var output = new WaveOutEvent
                    {
                        DeviceNumber = soundDevice,
                    };
                    output.Init(channel);
                    output.Play();

                    while (output.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(500);
                    }
                }
            }).Start();
        }
    }
}
