using System;
using System.Management;
using AutoAudio.Interfaces;

namespace AutoAudio.Impl
{
    public class ProcessEvents : IProcessEvents, IDisposable
    {
        // The dot in the scope means use the current machine
        private const string Scope = @"\\.\root\CIMV2";

        private readonly IPlaybackDeviceProvider _playbackDeviceProvider;
        private int _switchToPlaybackDevice;
        private int _defaultPlaybackDevice;
        private ManagementEventWatcher _startWatcher;
        private ManagementEventWatcher _endWatcher;

        private bool _hasStarted;

        public ProcessEvents(IPlaybackDeviceProvider playbackDeviceProvider)
        {
            _playbackDeviceProvider = playbackDeviceProvider;
        }

        public void RegisterSwitchForProcess(string processName, int switchToPlaybackDevice, int defaultPlaybackDevice)
        {
            _switchToPlaybackDevice = switchToPlaybackDevice;
            _defaultPlaybackDevice = defaultPlaybackDevice;

            _startWatcher = WatchForProcessStart(processName);
            _endWatcher = WatchForProcessEnd(processName);
        }

        public void StopWatch()
        {
            if (_startWatcher != null)
            {
                _startWatcher.Stop();
            }

            if (_endWatcher != null)
            {
                _endWatcher.Stop();
            }
        }

        private ManagementEventWatcher WatchForProcessStart(string processName)
        {
            string queryString =
                "SELECT TargetInstance" +
                "  FROM __InstanceCreationEvent " +
                "WITHIN  10 " +
                " WHERE TargetInstance ISA 'Win32_Process' " +
                "   AND TargetInstance.Name = '" + processName + "'";

            // The dot in the scope means use the current machine
            

            // Create a watcher and listen for events
            var watcher = new ManagementEventWatcher(Scope, queryString);
            watcher.EventArrived += ProcessStarted;
            watcher.Start();
            return watcher;
        }

        private ManagementEventWatcher WatchForProcessEnd(string processName)
        {
            string queryString =
                "SELECT TargetInstance" +
                "  FROM __InstanceDeletionEvent " +
                "WITHIN  10 " +
                " WHERE TargetInstance ISA 'Win32_Process' " +
                "   AND TargetInstance.Name = '" + processName + "'";

            // Create a watcher and listen for events
            var watcher = new ManagementEventWatcher(Scope, queryString);
            watcher.EventArrived += ProcessEnded;
            watcher.Start();
            return watcher;
        }

        private void ProcessStarted(object sender, EventArrivedEventArgs e)
        {
            var targetInstance = (ManagementBaseObject) e.NewEvent.Properties["TargetInstance"].Value;
            string processName = targetInstance.Properties["Name"].Value.ToString();
            Console.WriteLine(String.Format("{0} process started", processName));

            _hasStarted = true;
            _playbackDeviceProvider.SetPlaybackDevice(_switchToPlaybackDevice);
        }

        private void ProcessEnded(object sender, EventArrivedEventArgs e)
        {
            if (_hasStarted)
            {
                var targetInstance = (ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value;
                string processName = targetInstance.Properties["Name"].Value.ToString();
                Console.WriteLine(String.Format("{0} process ended", processName));

                _playbackDeviceProvider.SetPlaybackDevice(_defaultPlaybackDevice);
                _hasStarted = false;
            }
        }

        public void Dispose()
        {
            if (_startWatcher != null)
            {
                _startWatcher.Stop();
                _startWatcher.Dispose();
                _startWatcher = null;
            }

            if (_endWatcher != null)
            {
                _endWatcher.Stop();
                _endWatcher.Dispose();
                _endWatcher = null;
            }
        }
    }
}