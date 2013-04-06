using System;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Microsoft.Win32;

namespace AutoAudio.Configuration
{
    public class ConfigurationProvider
    {        
        private static readonly ConfigurationProvider Instance = new ConfigurationProvider();

        public AutoSwitchConfiguration Configuration { get; private set; }

        public ConfigurationProvider()
        {
            Load();
            
            Configuration.IsRunOnStartupEnabled += (sender, args) => SetStartup();
        }

        public static ConfigurationProvider CurrentInstance
        {
            get { return Instance; }
        }

        public void Load()
        {
            if (!File.Exists(ConfigurationFile))
            {
                Configuration = new AutoSwitchConfiguration();
            }
            else
            {
                try
                {
                    using (var fs = new FileStream(ConfigurationFile, FileMode.Open, FileAccess.Read))
                    {
                        var serializer = CreateSerializer();
                        Configuration = (AutoSwitchConfiguration) serializer.ReadObject(fs);
                    }
                } 
                catch(Exception ex)
                {
                    Console.WriteLine("Load failed: {0}", ex);
                    Configuration = new AutoSwitchConfiguration();
                }
            }
        }

        public string ConfigurationFile
        {
            get
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var path = Path.Combine(appData, "AutoAudio");
                var configFile = Path.Combine(path, "AutoSwitchConfigurations.xml");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return configFile;
            }
        }

        public void Save()
        {
            using(var fs = new FileStream(ConfigurationFile, FileMode.Create, FileAccess.ReadWrite))
            {
                var serializer = CreateSerializer();
                serializer.WriteObject(fs, Configuration);
            }
        }

        public void SetStartup()
        {
            SetStartup(Configuration.RunOnStartupEnabled, Constants.ApplicationName, Application.ExecutablePath);
        }

        private static void SetStartup(bool enable, string applicationName, string path)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(Constants.StartupRegistryKey, true))
            {
                if (key != null)
                {
                    if (enable)
                        key.SetValue(applicationName, path);
                    else
                        key.DeleteValue(applicationName, false);
                }
            }
        }

        private DataContractSerializer CreateSerializer()
        {
            return new DataContractSerializer(typeof(AutoSwitchConfiguration));
        }
    }
}
