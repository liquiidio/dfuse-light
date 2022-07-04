namespace DeepReader.Storage.Faster.Options
{
    public class FasterServerOptions : FasterStandaloneOptions, IFasterStorageOptions
    {
        public string IpAddress { get; set; } // IP address of server
        public int Port { get; set; } // Port of server
        public bool DisablePubSub { get; set; }
        public long PubSubPageSizeBytes { get; set; } = 4;
    }
}
