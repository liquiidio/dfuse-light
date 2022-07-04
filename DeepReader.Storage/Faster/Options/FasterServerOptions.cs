namespace DeepReader.Storage.Faster.Options
{
    public class FasterServerOptions : FasterStandaloneOptions, IFasterStorageOptions
    {
        public string IpAddress { get; set; } // IP address of server
        public int BlockStorePort { get; set; } // Port of server
        public int TransactionStorePort { get; set; } // Port of server
        public int ActionStorePort { get; set; } // Port of server
        public int AbiStorePort { get; set; } // Port of server
        public bool DisablePubSub { get; set; }
        public long PubSubPageSizeBytes { get; set; } = 512;
    }
}
