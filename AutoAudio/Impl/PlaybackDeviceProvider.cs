using System;
using System.Collections.Generic;
using System.Linq;
using AutoAudio.Interfaces;
using NLog;

namespace AutoAudio.Impl
{
    class PlaybackDeviceProvider : IPlaybackDeviceProvider
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
            Logger.Info("Setting playback device '{0}':{1}", deviceName, playbackDeviceId); 
            SoundConfig.SetDefaultDevice(playbackDeviceId);
        }
    }
}