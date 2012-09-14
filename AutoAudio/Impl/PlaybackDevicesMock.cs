using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoAudioUi.Impl
{
    //class PlaybackDevicesMock : IPlaybackDeviceProvider
    //{
    //    private readonly List<PlaybackDevice> _devices;

    //    public PlaybackDevicesMock()
    //    {
    //        _devices = new List<PlaybackDevice>();
    //        _devices.Add(new PlaybackDevice(0, "Test 1"));
    //        _devices.Add(new PlaybackDevice(1, "Test 2"));            
    //    }

    //    public IList<PlaybackDevice> GetPlaybackDevices()
    //    {

    //        return _devices;
    //    }

    //    public string GetPlaybackDeviceName(int playbackDeviceId)
    //    {
    //        return _devices.Where(x => x.Id == playbackDeviceId).Select(x => x.Name).FirstOrDefault();
    //    }

    //    public int GetDefaultDeviceId()
    //    {
    //        return 0;
    //    }

    //    public void SetPlaybackDevice(int playbackDeviceId)
    //    {
    //        Console.WriteLine("Setting playback device: " + playbackDeviceId);
    //    }
    //}
}