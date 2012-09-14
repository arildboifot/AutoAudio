using System;
using System.Collections.Generic;
using System.Linq;
using AutoAudio.Interfaces;

namespace AutoAudio.Impl
{
    class PlaybackDeviceProvider : IPlaybackDeviceProvider
    {
        public IList<PlaybackDevice> GetPlaybackDevices()
        {
            var devices = SoundConfig.ListDevices();

            return devices.Select(x => new PlaybackDevice(x.Id, x.Name)).ToList();
        }

        public string GetPlaybackDeviceName(int playbackDeviceId)
        {
            return GetPlaybackDevices().Where(x => x.Id == playbackDeviceId).Select(x => x.Name).FirstOrDefault();
        }

        public int GetDefaultDeviceId()
        {
            return SoundConfig.GetDefaultDevice().Id;
        }

        public void SetPlaybackDevice(int playbackDeviceId)
        {
            var deviceName = GetPlaybackDeviceName(playbackDeviceId);
            Console.WriteLine("Settings playback device: " + deviceName);
            SoundConfig.SetDefaultDevice(playbackDeviceId);
        }
    }
}