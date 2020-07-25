using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Prometheus.HttpClientMetrics
{
    /// <summary>
    ///     This base class performs the data management necessary to associate the correct labels and values
    ///     with HttpClient metrics, depending on the options the user has provided for the HttpClient metric handler.
    ///     The following labels are supported:
    ///     'method' (HTTP request method)
    ///     'host' (The host name of  HTTP request)
    ///     'path' (The query path HTTP request)
    /// </summary>
    public abstract class HttpClientDelegatingHandlerBase<TCollector, TChild> : DelegatingHandler
        where TCollector : class, ICollector<TChild>
        where TChild : class, ICollectorChild
    {
        private readonly ICollection<HttpClientParameterMapping> _additionalHttpClientParameters;
        private readonly Dictionary<string, string> _labelToHttpClientParameterMap;
        private readonly TCollector _metric;

        protected HttpClientDelegatingHandlerBase(HttpClientMetricsOptionsBase? options,
                                                  TCollector? customMetric)
        {
            MetricFactory = Metrics.WithCustomRegistry(options?.Registry ?? Metrics.DefaultRegistry);

            _additionalHttpClientParameters =
                options?.AdditionalHttpClientParameters ?? new List<HttpClientParameterMapping>(0);

            ValidateAdditionalHttpClientParameterSet();
            _labelToHttpClientParameterMap = CreateLabelToHttpClientParameterMap();


            _metric = customMetric ?? CreateMetricInstance(HttpClientLabelNames.All);
        }

        protected HttpClientDelegatingHandlerBase(HttpMessageHandler innerHandler,
                                                  HttpClientMetricsOptionsBase? options,
                                                  TCollector? customMetric) : this(options, customMetric)
        {
            InnerHandler = innerHandler;
        }


        /// <summary>
        ///     The factory to use for creating the default metric for this handler.
        /// </summary>
        protected MetricFactory MetricFactory { get; }


        private void ValidateAdditionalHttpClientParameterSet()
        {
            var parameterNames = _additionalHttpClientParameters.Select(x => x.ParameterName).ToList();

            if (parameterNames.Distinct(StringComparer.InvariantCultureIgnoreCase).Count() != parameterNames.Count)
            {
                throw new
                    ArgumentException("The set of additional HttpClient parameters to track contains multiple entries with the same parameter name.",
                                      nameof(HttpClientMetricsOptionsBase.AdditionalHttpClientParameters));
            }

            var labelNames = _additionalHttpClientParameters.Select(x => x.LabelName).ToList();

            if (labelNames.Distinct(StringComparer.InvariantCultureIgnoreCase).Count() != labelNames.Count)
            {
                throw new
                    ArgumentException("The set of additional HttpClient parameters to track contains multiple entries with the same label name.",
                                      nameof(HttpClientMetricsOptionsBase.AdditionalHttpClientParameters));
            }

            if (HttpClientLabelNames.All.Except(labelNames, StringComparer.InvariantCultureIgnoreCase).Count() !=
                HttpClientLabelNames.All.Length)
            {
                throw new
                    ArgumentException($"The set of additional HttpClient parameters to track contains an entry with a reserved label name. Reserved label names are: {string.Join(", ", HttpClientLabelNames.All)}");
            }

            var reservedParameterNames = new[] {"action", "controller"};

            if (reservedParameterNames.Except(parameterNames, StringComparer.InvariantCultureIgnoreCase).Count() !=
                reservedParameterNames.Length)
            {
                throw new
                    ArgumentException($"The set of additional HttpClient parameters to track contains an entry with a reserved HttpClient parameter name. Reserved HttpClient parameter names are: {string.Join(", ", reservedParameterNames)}");
            }
        }

        /// <summary>
        ///     Creates the default metric instance with the specified set of labels.
        /// </summary>
        protected abstract TCollector CreateMetricInstance(string[] labelNames);

        /// <summary>
        ///     Creates the metric child instance to use for measurements.
        /// </summary>
        /// <remarks>
        ///     Internal for testing purposes.
        /// </remarks>
        protected internal TChild CreateChild(HttpRequestMessage request)
        {
            if (!_metric.LabelNames.Any())
            {
                return _metric.Unlabelled;
            }


            var labelValues = new string[_metric.LabelNames.Length];

            for (var i = 0; i < labelValues.Length; i++)
            {
                switch (_metric.LabelNames[i])
                {
                    case HttpClientLabelNames.Method:
                        labelValues[i] = request.Method.Method;
                        break;
                    case HttpClientLabelNames.Host:
                        labelValues[i] = request.RequestUri.Host;
                        break;
                    default:
                        // We validate the label set on initialization, so it must be a route parameter if we get to this point.
                        var parameterName = _labelToHttpClientParameterMap[_metric.LabelNames[i]];

                        var parameter = _additionalHttpClientParameters.Single(p => p.ParameterName == parameterName);

                        var labelValue = parameter.LabelValue(request, parameterName);

                        labelValues[i] = labelValue ?? string.Empty;
                        break;
                }
            }

            return _metric.WithLabels(labelValues);
        }

        private Dictionary<string, string> CreateLabelToHttpClientParameterMap()
        {
            var map = new Dictionary<string, string>(_additionalHttpClientParameters.Count + 2);

            // Defaults are hardcoded.
            map["method"] = "method";
            map["host"] = "host";

            // Any additional ones are merged.
            foreach (var entry in _additionalHttpClientParameters)
            {
                map[entry.LabelName] = entry.ParameterName;
            }

            return map;
        }
    }
}