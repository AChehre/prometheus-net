using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Prometheus.HttpClientMetrics
{
    public class RouteHttpClientParameterMapping
    {
        public HttpClientParameterMapping Create()
        {

            var parameterName = "route";
            var labelName = "route";
            Func<HttpRequestMessage, string, string> labelValue = (HttpRequestMessage request, string parameterName) =>
            {
                if (parameterName == "route")
                {


                }
                return string.Empty;
            };

            var httpParameter =  new HttpClientParameterMapping(parameterName,labelName, labelValue);

            return httpParameter;

        }
    }
}
