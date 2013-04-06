using System;

namespace AutoAudio.Configuration
{
    public class DeviceConfiguration
    {
        public Guid Id { get; private set; }
        public string Process { get; set; }
        public int PlaybackDeviceId { get; set; }

        public DeviceConfiguration()
        {
            Id = Guid.NewGuid();
        }
    }
}