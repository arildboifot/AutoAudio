using System;

namespace AutoAudio.Interfaces
{
    public interface IProcessEvents : IDisposable
    {
        void RegisterSwitchForProcess(string processName, int switchToPlaybackDevice, int defaultPlaybackDevice);
        void StopWatch();
    }
}