using System.Collections.Generic;
using AutoAudio.Impl;

namespace AutoAudio.Interfaces
{
    public interface IPlaybackDeviceProvider
    {
        IList<PlaybackDevice> GetPlaybackDevices();
        string GetPlaybackDeviceName(int playbackDeviceId);
        int GetDefaultDeviceId();
        void SetPlaybackDevice(int playbackDeviceId);
    }
}