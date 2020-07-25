namespace Prometheus.HttpClientMetrics
{
    public sealed class HttpClientHandlerExporterOptions
    {
        public HttpClientInProgressOptions InProgress { get; set; } = new HttpClientInProgressOptions();
        public HttpClientRequestCountOptions RequestCount { get; set; } = new HttpClientRequestCountOptions();
        public HttpClientRequestDurationOptions RequestDuration { get; set; } = new HttpClientRequestDurationOptions();


        /// <summary>
        /// Adds an additional route parameter to all the HTTP metrics.
        /// 
        /// Helper method to avoid manually adding it to each one.
        /// </summary>
        public void AddRouteParameter(HttpClientParameterMapping mapping)
        {
            InProgress.AdditionalHttpClientParameters.Add(mapping);
            RequestCount.AdditionalHttpClientParameters.Add(mapping);
            RequestDuration.AdditionalHttpClientParameters.Add(mapping);
        }
    }
}