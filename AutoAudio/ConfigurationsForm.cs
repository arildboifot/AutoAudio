using System;
using System.Linq;
using System.Windows.Forms;
using AutoAudio.Configuration;
using AutoAudio.Impl;
using AutoAudio.Interfaces;

namespace AutoAudio
{
    public partial class ConfigurationsForm : Form
    {
        private bool _exiting;
        private bool _allowVisible = false;
        private readonly IPlaybackDeviceProvider _playbackDeviceProvider;
        private readonly ProcessEventContainer _processEventContainer;
        private readonly int _defaultPlaybackDevice;

        private readonly AutoSwitchConfiguration _configuration = ConfigurationProvider.CurrentInstance.Configuration;

        public ConfigurationsForm()
        {
            InitializeComponent();

            _playbackDeviceProvider = new PlaybackDeviceProvider();
            _defaultPlaybackDevice = _playbackDeviceProvider.GetDefaultDeviceId();
            _processEventContainer = new ProcessEventContainer(_playbackDeviceProvider, _configuration); 

            Initialize();
        }

        protected override void SetVisibleCore(bool value)
        {
            if (!_allowVisible)
            {
                value = false;
                if (!IsHandleCreated)
                {
                    CreateHandle();
                }
            }
            base.SetVisibleCore(value);
        }

        private void Initialize()
        {
            autoSwitchConfigurationsBindingSource.DataSource = _configuration;

            foreach (var configuration in _configuration.DeviceConfigurations)
            {
                AddItemToList(configuration);
            }


            var playbackDevices = _playbackDeviceProvider.GetPlaybackDevices();
            foreach (var device in playbackDevices)
            {
                var idx = clbFavorites.Items.Add(device);
                if (_configuration.FavoriteDevices.Any(x => x.PlaybackDeviceId == device.Id))
                {
                    clbFavorites.SetItemChecked(idx, true);
                }
            }

            _processEventContainer.Initialize();            
        }      


        private void btnAdd_Click(object sender, EventArgs e)
        {
            using(var dlg = new AddConfigurationForm())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var configuration = new DeviceConfiguration();
                    configuration.PlaybackDeviceId = dlg.SelectedPlaybackDevice.Id;
                    configuration.Process = dlg.SelectedProcess;

                    AddItemToList(configuration);
                    AddConfiguration(configuration);
                }
            }
        }

        private void AddItemToList(DeviceConfiguration configuration)
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
                var configuration = (DeviceConfiguration) selectedItem.Tag;

                lvConfigurations.Items.Remove(selectedItem);
                RemoveConfiguration(configuration);
            }
        }

        private void AddConfiguration(DeviceConfiguration configuration)
        {
            _configuration.Add(configuration);
            SaveConfiguration();
        }

        private void RemoveConfiguration(DeviceConfiguration configuration)
        {
            _configuration.Remove(configuration);
            SaveConfiguration();
        }

        private void ConfigurationsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_exiting)
            {
                _allowVisible = false;
                e.Cancel = true;
                Hide();
            }
        }      

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            OpenForm();
        }

        private void OpenForm()
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }

            _allowVisible = true;
            Show();
            Activate();            
        }

        private void CloseForm()
        {
            _allowVisible = false;
            _processEventContainer.Dispose();
            _playbackDeviceProvider.SetPlaybackDevice(_defaultPlaybackDevice);
            _exiting = true;

            Application.Exit();
        }

        private void contextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {            
            var menu = sender as ContextMenuStrip;
            if (menu != null)
            {                
                menu.Items.Clear();
                menu.Items.Add(new ToolStripMenuItem("&Open", null, (o, args) => OpenForm()));
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(new ToolStripMenuItem("&Enabled", null, (o, args) =>
                    {
                        _configuration.IsEnabled = !_configuration.IsEnabled;
                        SaveConfiguration();
                    }) { Checked = _configuration.IsEnabled });
                menu.Items.Add(new ToolStripMenuItem("&Run on startup", null, (o, args) =>
                    {
                        _configuration.RunOnStartupEnabled = !_configuration.RunOnStartupEnabled;
                        SaveConfiguration();
                    }) { Checked = _configuration.RunOnStartupEnabled });
                menu.Items.Add(new ToolStripSeparator());
                AddDevices(menu, _configuration);
                menu.Items.Add(new ToolStripMenuItem("E&xit", null, (o, args) => CloseForm()));
            }
        }

        private void AddDevices(ContextMenuStrip menu, AutoSwitchConfiguration configuration)
        {
            var deviceIds = configuration.DeviceConfigurations
                            .GroupBy(x => x.PlaybackDeviceId)
                            .Select(x => x.First())
                            .Select(x => x.PlaybackDeviceId)
                            .ToList();

            deviceIds.AddRange(configuration.FavoriteDevices.Where(x => !deviceIds.Contains(x.PlaybackDeviceId)).Select(x => x.PlaybackDeviceId));
            
            foreach (var deviceId in deviceIds)
            {
                //var deviceId = config.PlaybackDeviceId;
                var deviceName = _playbackDeviceProvider.GetPlaybackDeviceName(deviceId);

                var item = new ToolStripMenuItem(deviceName);
                item.Tag = deviceId;
                Action<int> setCheckedDevice = (id) => item.Checked = id == _playbackDeviceProvider.GetDefaultDeviceId();
                item.Click += (sender, e) =>
                {
                    _playbackDeviceProvider.SetPlaybackDevice(deviceId);
                    setCheckedDevice(deviceId);                                    
                };
                setCheckedDevice(deviceId);                
                menu.Items.Add(item);
            }

            

            if (configuration.DeviceConfigurations.Count > 0)
            {
                menu.Items.Add(new ToolStripSeparator());
            }
        }

        private void SaveConfiguration()
        {
            ConfigurationProvider.CurrentInstance.Save();
            autoSwitchConfigurationsBindingSource.DataSource = _configuration;
            autoSwitchConfigurationsBindingSource.ResetBindings(false);            
        }

        private void clbFavorites_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var list = (sender as CheckedListBox);
            if (list != null)
            {
                var item = (PlaybackDevice)list.Items[e.Index];
                var configuredItem = _configuration.FavoriteDevices.SingleOrDefault(x => x.PlaybackDeviceId == item.Id);
                if (e.NewValue == CheckState.Checked)
                {
                    if (configuredItem == null)
                    {
                        _configuration.FavoriteDevices.Add(new FavoriteDeviceConfiguration {PlaybackDeviceId = item.Id});
                    }
                }
                else
                {
                    _configuration.FavoriteDevices.Remove(configuredItem);
                }

                SaveConfiguration();
            }                
        }
    }
}
