using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace AutoAudio.Configuration
{
    public class AutoSwitchConfigurations
    {
        public bool Enabled { get; set; }
        public bool RunOnStartupEnabled { get; set; }
        public List<AutoSwitchConfiguration> Configurations { get; set; }
        

        public AutoSwitchConfigurations()
        {
            Configurations = new List<AutoSwitchConfiguration>();
        }

        public void Add(AutoSwitchConfiguration configuration)
        {
            Configurations.Add(configuration);
        }

        public void Remove(AutoSwitchConfiguration configuration)
        {
            var configurationToRemove = Configurations.FirstOrDefault(x => x.Id == configuration.Id);
            if (configurationToRemove != null)
            {
                Configurations.Remove(configurationToRemove);
            }
        }
    }
}