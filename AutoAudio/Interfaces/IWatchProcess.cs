using System.Collections.Generic;
using AutoAudio.Impl;

namespace AutoAudio.Interfaces
{
    public interface IWatchProcess
    {
        IList<WatchProcess> GetProcesses();
    }
}