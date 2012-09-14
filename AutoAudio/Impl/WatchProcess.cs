namespace AutoAudio.Impl
{
    public class WatchProcess
    {
        public string Path { get; set; }
        public string CommandLine { get; set; }

        public WatchProcess() { }

        public WatchProcess(string path, string commandLine) : this()
        {
            Path = path;
            CommandLine = commandLine;
        }

        public string Name
        {
            get { return System.IO.Path.GetFileName(Path); }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}