using Dalamud.Utility;
using NAudio.Vorbis;
using NAudio.Wave;
using NLayer.NAudioSupport;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace AudibleCharacterStatus
{
    public static class SoundEngine
    {
        static Mp3FileReaderBase.FrameDecompressorBuilder mp3Builder = new Mp3FileReaderBase.FrameDecompressorBuilder(wf => new Mp3FrameDecompressor(wf));
        // Copied from PeepingTom plugin
        public static string PlaySound(string path, float volume = 1.0f, bool waitForMessage = false)
        {
            string message = "";
            if (path.IsNullOrEmpty() || !File.Exists(path)) return "No such file exists";

            string fileType = Path.GetExtension(path);
            if (fileType != ".mp3" && fileType != ".wav" && fileType != ".ogg") {
                Service.PluginLog.Debug($"Bad file type: {fileType}");
                return "Unsupported File Type"; 
            }

            if (Process.GetCurrentProcess().Id != ProcessUtils.GetForegroundProcessId() && !Service.Config.PlayInBackground) return "Pass";

            /*var soundDevice = Service.Config.SoundDeviceId;
            if (soundDevice < -1 || soundDevice > WaveOut.Devices)
            {
                soundDevice = -1;
            }*/

            new Thread(() => {
                WaveStream reader = null;
                try
                {
                    if (fileType == ".mp3")
                    {
                        reader = new Mp3FileReaderBase(path, mp3Builder);
                    }
                    else if (fileType == ".wav")
                    {
                        reader = new WaveFileReader(path);
                    } 
                    else if (fileType == ".ogg")
                    {
                        reader = new VorbisWaveReader(path);
                    }
                }
                catch (Exception e)
                {
                    Service.PluginLog.Error($"Could not play sound file: {e.Message}");
                    message = "Error reading file. Check the log.";
                    return;
                }

                if (reader == null) return;

                volume = Math.Max(0, Math.Min(volume, 1));

                WaveChannel32 channel = null;

                try
                {
                    channel = new(reader)
                    {
                        Volume = 1 - (float)Math.Sqrt(1 - (volume * volume)),
                        PadWithZeroes = false,
                    };
                }
                catch (Exception e)
                {
                    Service.PluginLog.Error($"Could not play sound file: {e.Message}");
                    message = "Error reading file.  Check the log.";
                    return;
                }

                using (reader)
                {
                    using var output = new WaveOutEvent
                    {
                        DeviceNumber = -1,
                    };
                    output.Init(channel);
                    output.Play();

                    message = "Pass";

                    while (output.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(100);
                    }
                }
            }).Start();

            if (waitForMessage)
            {
                int waitTicks = 0;
                while (message == "")
                {
                    if (waitTicks >= 1500) message = "??";
                    Thread.Sleep(1);
                    waitTicks++;
                }
            }

            return message;
        }
    }
}
