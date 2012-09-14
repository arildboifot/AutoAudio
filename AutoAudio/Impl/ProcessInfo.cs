namespace AutoAudio.Impl
{
    public class ProcessInfo
    {
        public string Path { get; set; }
        public string CommandLine { get; set; }

        public ProcessInfo() { }

        public ProcessInfo(string path, string commandLine) : this()
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