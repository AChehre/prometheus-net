using System.Collections.Generic;

namespace Prometheus.HttpClientMetrics
{
    public abstract class HttpClientMetricsOptionsBase
    {
        public bool Enabled { get; set; } = true;

        /// <summary>
        ///     Additional route parameters to include beyond the defaults (controller/action).
        ///     This may be useful if you have, for example, a "version" parameter for API versioning.
        /// </summary>
        /// <remarks>
        ///     Metric labels are automatically added for these parameters, unless you provide your
        ///     own metric instance in the options (in which case you must add the required labels).
        /// </remarks>
        public List<HttpClientParameterMapping> AdditionalHttpClientParameters { get; set; } =
            new List<HttpClientParameterMapping>();

        /// <summary>
        ///     Allows you to override the registry used to create the default metric instance.
        /// </summary>
        public CollectorRegistry? Registry { get; set; }
    }
}