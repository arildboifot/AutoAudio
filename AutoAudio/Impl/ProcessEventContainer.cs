using System;
using System.Collections.Generic;
using AutoAudio.Configuration;
using AutoAudio.Interfaces;

namespace AutoAudio.Impl
{
    public class ProcessEventContainer : IProcessEventContainer, IDisposable
    {
        private bool _isDisposed;

        private readonly IPlaybackDeviceProvider _playbackDeviceProvider;
        private readonly AutoSwitchConfiguration _configuration;
        private readonly int _defaultPlaybackDevice;
        private readonly IDictionary<Guid, IProcessEvents> _processEvents = new Dictionary<Guid, IProcessEvents>();

        public ProcessEventContainer(IPlaybackDeviceProvider playbackDeviceProvider, AutoSwitchConfiguration configuration)
        {
            _playbackDeviceProvider = playbackDeviceProvider;
            _configuration = configuration;
            _defaultPlaybackDevice = _playbackDeviceProvider.GetDefaultDeviceId();

            _configuration.IsEnabledChanged += (sender, args) => ReconfigureListeners();
            _configuration.DeviceConfigurationsChanged += (sender, args) => ReconfigureListeners();
        }

        public void AddListener(DeviceConfiguration configuration)
        {
            var listener = new ProcessEvents(_playbackDeviceProvider);
            listener.RegisterSwitchForProcess(configuration.Process, configuration.PlaybackDeviceId, _defaultPlaybackDevice);

            _processEvents.Add(configuration.Id, listener);
        }

        public void Initialize()
        {
            ReconfigureListeners();
        }

        private void ReconfigureListeners()
        {
            ResetAllListeners();

            if (_configuration.IsEnabled)
            {
                foreach (var configuration in _configuration.DeviceConfigurations)
                {
                    AddListener(configuration);
                }
            }
        }

        private void ResetAllListeners()
        {
            foreach (var listener in _processEvents)
            {
                listener.Value.Dispose();
            }

            _processEvents.Clear();
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                ResetAllListeners();
                _isDisposed = true;
            }            
        }
    }
}