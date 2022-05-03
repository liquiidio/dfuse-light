namespace DeepReader.Apis.Options
{
    public class ApiOptions
    {
        public ushort MetricsPort { get; set; } = 7777;
        public string MetricsUrl { get; set; } = "/metrics";
    }
}
