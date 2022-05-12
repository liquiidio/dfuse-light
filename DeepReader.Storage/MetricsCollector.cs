using System.Timers;

namespace DeepReader.Storage
{
    public class MetricsCollector
    {
        private System.Timers.Timer _timer;
        public event EventHandler? CollectMetricsHandler;

        public MetricsCollector()
        {
            _timer = new System.Timers.Timer(2000);
            _timer.Elapsed += CollectObservableMetrics;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void CollectObservableMetrics(object? sender, ElapsedEventArgs e)
        {
            if (CollectMetricsHandler is not null)
                CollectMetricsHandler(this, null!);
        }
    }
}