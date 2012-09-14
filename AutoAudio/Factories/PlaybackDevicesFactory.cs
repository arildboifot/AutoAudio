using AutoAudio.Impl;
using AutoAudio.Interfaces;

namespace AutoAudio.Factories
{
    public class PlaybackDevicesFactory
    {
        public static IPlaybackDeviceProvider Create()
        {
            //return new PlaybackDevicesMock();
            return new PlaybackDeviceProvider();            
        }
    }
}