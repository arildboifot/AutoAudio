using System;
using System.IO;
using System.Runtime.Serialization;

namespace AutoAudio.Configuration
{
    public class ConfigurationProvider
    {
        private const string ConfigurationFile = "AutoSwitchConfigurations.xml";
        private static readonly ConfigurationProvider Instance = new ConfigurationProvider();

        public AutoSwitchConfigurations Configuration { get; private set; }

        public ConfigurationProvider()
        {
            Load();
        }

        public static ConfigurationProvider CurrentInstance
        {
            get { return Instance; }
        }

        public void Load()
        {
            if (!File.Exists(ConfigurationFile))
            {
                Configuration = new AutoSwitchConfigurations();
            }
            else
            {
                try
                {
                    using (var fs = new FileStream(ConfigurationFile, FileMode.Open, FileAccess.Read))
                    {
                        var serializer = CreateSerializer();
                        Configuration = (AutoSwitchConfigurations) serializer.ReadObject(fs);
                    }
                } 
                catch(Exception ex)
                {
                    Console.WriteLine("Load failed: {0}", ex);
                    Configuration = new AutoSwitchConfigurations();
                }
            }
        }

        public void Save()
        {
            using(var fs = new FileStream(ConfigurationFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var serializer = CreateSerializer();
                serializer.WriteObject(fs, Configuration);
            }
        }

        private DataContractSerializer CreateSerializer()
        {
            return new DataContractSerializer(typeof(AutoSwitchConfigurations));
        }
    }
}