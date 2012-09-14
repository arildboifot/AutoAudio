using System;
using System.Collections.Generic;
using System.Linq;
using AutoAudio.Interfaces;

namespace AutoAudio.Impl
{
    class PlaybackDeviceProvider : IPlaybackDeviceProvider
    {
        private static IList<PlaybackDevice> _deviceCache; 

        public IList<PlaybackDevice> GetPlaybackDevices()
        {
            //if (_deviceCache != null)
            //{
            //    return _deviceCache;
            //}

            //var objSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_SoundDevice");
            //ManagementObjectCollection objCollection = objSearcher.Get();

            //int id = 0;
            //var result = new List<PlaybackDevice>();
            //foreach (ManagementObject obj in objCollection)
            //{
            //    var name = obj.Properties.Cast<PropertyData>().Where(x => x.Name == "Name").Select(x => x.Value).FirstOrDefault();
            //    result.Add(new PlaybackDevice(id, (string)name));

            //    id++;
            //}

            //_deviceCache = new List<PlaybackDevice>();
            //_deviceCache = result;

            //return result;

            var devices = SoundConfig.ListDevices();

            return devices.Select(x => new PlaybackDevice(x.Id, x.Name)).ToList();
        }

        public string GetPlaybackDeviceName(int playbackDeviceId)
        {
            return GetPlaybackDevices().Where(x => x.Id == playbackDeviceId).Select(x => x.Name).FirstOrDefault();
        }

        public int GetDefaultDeviceId()
        {
            //SoundConfig.GetCurrentPlaybackDev
            //var defaultDevice = AudioInterface.GetDefaultPlaybackDevice();
            //return new PlaybackDevice(defaultDevice.ID, defaultDevice.Name);

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