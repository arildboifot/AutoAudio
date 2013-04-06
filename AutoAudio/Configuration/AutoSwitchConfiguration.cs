using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoAudio.Configuration
{
    public class EnabledEventArgs : EventArgs
    {
        public bool IsEnabled { get; private set; }

        public EnabledEventArgs(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }
    }    

    public class AutoSwitchConfiguration
    {
        private bool _isEnabled;
        private bool _runOnStartupEnabled;

        public List<DeviceConfiguration> DeviceConfigurations { get; set; }

        public event EventHandler<EnabledEventArgs> IsEnabledChanged;
        public event EventHandler<EnabledEventArgs> IsRunOnStartupEnabled;
        public event EventHandler<EventArgs> DeviceConfigurationsChanged;

        public AutoSwitchConfiguration()
        {
            DeviceConfigurations = new List<DeviceConfiguration>();
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnIsEnabledChanged(new EnabledEventArgs(value));
            }
        }

        public bool RunOnStartupEnabled
        {
            get { return _runOnStartupEnabled; }
            set
            {
                _runOnStartupEnabled = value;
                OnIsRunOnStartupEnabled(new EnabledEventArgs(value));
            }
        }

        public void Add(DeviceConfiguration configuration)
        {
            DeviceConfigurations.Add(configuration);
            OnDeviceConfigurationsChanged(new EventArgs());
        }

        public void Remove(DeviceConfiguration configuration)
        {
            var configurationToRemove = DeviceConfigurations.FirstOrDefault(x => x.Id == configuration.Id);
            if (configurationToRemove != null)
            {
                DeviceConfigurations.Remove(configurationToRemove);
                OnDeviceConfigurationsChanged(new EventArgs());
            }
        }

        public void OnIsEnabledChanged(EnabledEventArgs e)
        {
            EventHandler<EnabledEventArgs> handler = IsEnabledChanged;
            if (handler != null) handler(this, e);
        }

        public void OnIsRunOnStartupEnabled(EnabledEventArgs e)
        {
            EventHandler<EnabledEventArgs> handler = IsRunOnStartupEnabled;
            if (handler != null) handler(this, e);
        }

        public void OnDeviceConfigurationsChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = DeviceConfigurationsChanged;
            if (handler != null) handler(this, e);
        }
    }
}