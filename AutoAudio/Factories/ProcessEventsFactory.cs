using AutoAudio.Impl;
using AutoAudio.Interfaces;

namespace AutoAudio.Factories
{
    public class ProcessEventsFactory
    {
        public static IProcessEvents Create()
        {
            var playbackDeviceProvider = PlaybackDevicesFactory.Create();
            return new ProcessEvents(playbackDeviceProvider);
        }
    }
}