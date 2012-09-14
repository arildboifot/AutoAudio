using System;
using System.Windows.Forms;
using AutoAudio.Impl;
using AutoAudio.Interfaces;

namespace AutoAudio
{
    public partial class AddConfigurationForm : Form
    {
        private readonly IPlaybackDeviceProvider _playbackDeviceProvider;

        public AddConfigurationForm()
        {
            InitializeComponent();

            _playbackDeviceProvider = new PlaybackDeviceProvider();

            Initialize();
        }

        private void Initialize()
        {
            var playbackDevices = _playbackDeviceProvider.GetPlaybackDevices();
            foreach(var device in playbackDevices)
            {
                cbPlaybackDevices.Items.Add(device);
            }

            var processes = ProcessHelper.GetProcesses();
            foreach(var process in processes)
            {
                cbProcesses.Items.Add(process.Name);
            }
        }

        public PlaybackDevice SelectedPlaybackDevice
        {
            get { return (PlaybackDevice)cbPlaybackDevices.SelectedItem; }
        }

        public string SelectedProcess
        {
            get { return cbProcesses.Text; }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (SelectedPlaybackDevice == null)
            {
                MessageBox.Show("Please select a playback device", "Missing playback device", MessageBoxButtons.OK);
            }
            else if (string.IsNullOrWhiteSpace(SelectedProcess))
            {
                MessageBox.Show("Please select a process", "Missing process", MessageBoxButtons.OK);
            }
            else
                DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
