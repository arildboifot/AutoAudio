using System;

namespace AutoAudio.Configuration
{
    public class AutoSwitchConfiguration
    {
        public Guid Id { get; private set; }
        public string Process { get; set; }
        public int PlaybackDeviceId { get; set; }

        public AutoSwitchConfiguration()
        {
            Id = Guid.NewGuid();
        }
    }
}