using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AutoAudio.Configuration;
using AutoAudio.Factories;
using AutoAudio.Interfaces;

namespace AutoAudio
{
    public partial class ConfigurationsForm : Form
    {
        private bool _exiting;
        private bool _isInitializing;
        private readonly IPlaybackDeviceProvider _playbackDeviceProvider;
        private readonly IDictionary<Guid, IProcessEvents> _processEvents = new Dictionary<Guid, IProcessEvents>();
        private readonly int _defaultPlaybackDevice;

        public ConfigurationsForm()
        {
            InitializeComponent();

            _playbackDeviceProvider = PlaybackDevicesFactory.Create();
            _defaultPlaybackDevice = _playbackDeviceProvider.GetDefaultDeviceId();

            Initialize();
        }

        private void Initialize()
        {
            _isInitializing = true;
            cbEnableAutoSwitch.Checked = ConfigurationProvider.CurrentInstance.Configuration.Enabled;
            foreach(var configuration in ConfigurationProvider.CurrentInstance.Configuration.Configurations)
            {
                AddItemToList(configuration);
            }

            ReconfigureListeners();
            //UpdateContextMenuStrip();
            _isInitializing = false;
        }

        //private void UpdateContextMenuStrip()
        //{
        //    for(int i = contextMenuStrip.Items.Count - 3; i > 0; i--)
        //    {
        //        contextMenuStrip.Items.RemoveAt(i);
        //    }

        //    var configurations = ConfigurationProvider.CurrentInstance.Configuration.Configurations;
        //    for(int i = 0; i < configurations.Count; i++)
        //    {
        //        var deviceId = configurations[i].PlaybackDeviceId;
        //        var deviceName = _playbackDeviceProvider.GetPlaybackDeviceName(configurations[i].PlaybackDeviceId);
                
        //        var item = new ToolStripMenuItem(deviceName);
        //        item.Tag = deviceId;
        //        item.Click += (sender, e) => _playbackDeviceProvider.SetPlaybackDevice(deviceId);
        //        item.Checked = deviceId == _defaultPlaybackDevice;
        //        contextMenuStrip.Items.Insert(i, item);
        //    }

        //    if (configurations.Count > 0)
        //    {
        //        contextMenuStrip.Items.Insert(configurations.Count, new ToolStripSeparator());
        //    }
        //}

        private void ReconfigureListeners()
        {
            ResetAllListeners();

            if (cbEnableAutoSwitch.Checked)
            {
                foreach (var configuration in ConfigurationProvider.CurrentInstance.Configuration.Configurations)
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

        private void AddListener(AutoSwitchConfiguration configuration)
        {
            var listener = ProcessEventsFactory.Create();
            listener.RegisterSwitchForProcess(configuration.Process, configuration.PlaybackDeviceId, _defaultPlaybackDevice);

            _processEvents.Add(configuration.Id, listener);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using(var dlg = new AddConfigurationForm())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var configuration = new AutoSwitchConfiguration();
                    configuration.PlaybackDeviceId = dlg.SelectedPlaybackDevice.Id;
                    configuration.Process = dlg.SelectedProcess;

                    AddItemToList(configuration);
                    AddConfiguration(configuration);
                }
            }
        }

        private void AddItemToList(AutoSwitchConfiguration configuration)
        {
            var playbackDeviceName = _playbackDeviceProvider.GetPlaybackDeviceName(configuration.PlaybackDeviceId);
            var listItem = lvConfigurations.Items.Add(configuration.Process);
            listItem.Tag = configuration;
            listItem.SubItems.Add(playbackDeviceName);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lvConfigurations.SelectedItems.Count == 0)
                return;

            var result = MessageBox.Show("Are you sure you want to delete the selected configuration?", "Deleted confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                var selectedItem = lvConfigurations.SelectedItems[0];
                var configuration = (AutoSwitchConfiguration) selectedItem.Tag;

                lvConfigurations.Items.Remove(selectedItem);
                RemoveConfiguration(configuration);
            }
        }

        private void AddConfiguration(AutoSwitchConfiguration configuration)
        {
            ConfigurationProvider.CurrentInstance.Configuration.Add(configuration);
            ConfigurationProvider.CurrentInstance.Save();

            ReconfigureListeners();
            //UpdateContextMenuStrip();
        }

        private void RemoveConfiguration(AutoSwitchConfiguration configuration)
        {
            ConfigurationProvider.CurrentInstance.Configuration.Remove(configuration);
            ConfigurationProvider.CurrentInstance.Save();

            ReconfigureListeners();
            //UpdateContextMenuStrip();
        }

        private void ConfigurationsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_exiting)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void cbEnableAutoSwitch_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isInitializing)
            {
                ConfigurationProvider.CurrentInstance.Configuration.Enabled = cbEnableAutoSwitch.Checked;
                ConfigurationProvider.CurrentInstance.Save();

                ReconfigureListeners();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            OpenForm();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenForm();
        }

        private void OpenForm()
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }

            Show();
            Activate();            
        }

        private void CloseForm()
        {
            ResetAllListeners();
            _playbackDeviceProvider.SetPlaybackDevice(_defaultPlaybackDevice);
            _exiting = true;

            Application.Exit();
        }
    }
}
