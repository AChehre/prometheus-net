using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Prometheus.HttpClientMetrics
{
    /// <summary>
    /// Maps an HTTP route parameter name to a Prometheus label name.
    /// </summary>
    /// <remarks>
    /// Typically, the parameter name and the label name will be equal.
    /// The purpose of this is to enable capture of route parameters that conflict with built-in label names like "method" (HTTP method).
    /// </remarks>
    public sealed class HttpClientParameterMapping
    {
        /// <summary>
        /// Name of the HTTP route parameter.
        /// </summary>
        public string ParameterName { get; }

        /// <summary>
        /// Name of the Prometheus label.
        /// </summary>
        public string LabelName { get; }

        public Func<HttpRequestMessage, string, string> LabelValue { get; }
        public HttpClientParameterMapping(string name, Func<HttpRequestMessage,  string, string> labelValue)
        {
            Collector.ValidateLabelName(name);

            ParameterName = name;
            LabelValue = labelValue;
            LabelName = name;
        }

        public HttpClientParameterMapping(string parameterName, string labelName, Func<HttpRequestMessage, string, string> labelValue)
        {
            Collector.ValidateLabelName(labelName);

            ParameterName = parameterName;
            LabelName = labelName;
            LabelValue = labelValue;
        }

        //public static implicit operator HttpRouteParameterMapping(string name) => new HttpRouteParameterMapping(name);
    }
}
