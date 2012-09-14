namespace AutoAudio.Impl
{
    public class PlaybackDevice
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public PlaybackDevice(int id, string name) 
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}