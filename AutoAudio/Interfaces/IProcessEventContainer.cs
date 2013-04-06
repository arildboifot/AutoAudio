using AutoAudio.Configuration;

namespace AutoAudio.Interfaces
{
    public interface IProcessEventContainer
    {
        void AddListener(DeviceConfiguration configuration);
        void Initialize();
    }
}