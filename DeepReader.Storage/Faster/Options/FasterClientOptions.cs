namespace DeepReader.Storage.Faster.Options
{
    public class FasterClientOptions : IFasterStorageOptions
    {
        public string IpAddress { get; set; } // IP address of server
        public int Port { get; set; } // Port of server
    }
}
