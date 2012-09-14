using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using AutoAudio.Interfaces;

namespace AutoAudio.Impl
{
    class ProcessProvider : IWatchProcess
    {
        public IList<WatchProcess> GetProcesses()
        {
            var wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            using (var results = searcher.Get())
            {
                var query = from p in Process.GetProcesses()
                            join mo in results.Cast<ManagementObject>()
                            on p.Id equals (int)(uint)mo["ProcessId"]
                            select new
                            {
                                Process = p,
                                Path = (string)mo["ExecutablePath"],
                                CommandLine = (string)mo["CommandLine"],
                            };

                return query.Select(x => new WatchProcess(x.Path, x.CommandLine)).Where(x => !string.IsNullOrWhiteSpace(x.Path)).ToList();
            }
        }
    }
}
