using RestApi.Models;
using RestApi.ServiceClasses;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace RestApi.Handlers
{
    public class RequestResponseLogHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // log request body
            string requestBody = await request.Content.ReadAsStringAsync();
            // Trace.WriteLine(requestBody);

            RequestResponseLog item = new RequestResponseLog();

            item.RequestDate = DateTime.Now;
            item.RequestType = request.Method.Method;
            item.Url = request.RequestUri.ToString();
            item.IPAddress = HttpContext.Current.Request.UserHostAddress;
            item.Controller = (string)request.GetRouteData().Values["controller"];
            item.Action = (string)request.GetRouteData().Values["action"];
            item.RequestHeader = GetHeadersString(request.Headers.GetEnumerator());

            // let other handlers process the request
            var result = await base.SendAsync(request, cancellationToken);

            // once response body is ready, log it
            var responseBody = await result.Content.ReadAsStringAsync();

            item.ResponseDate = DateTime.Now;
            item.ResponseHeader = GetHeadersString(result.Headers.GetEnumerator());
            item.ResponseBody = responseBody;

            LoggingService.WriteToXmlLog(item);

            return result;
        }

        private static string GetHeadersString(IEnumerator<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            StringBuilder sb = new StringBuilder();

            var enumeratedHeaders = headers;
            while (enumeratedHeaders.MoveNext())
            {
                string tempValues = GetStrings(enumeratedHeaders.Current.Value.GetEnumerator());
                sb.AppendFormat("{0}={1};", enumeratedHeaders.Current.Key, tempValues);
            }

            return sb.ToString();
        }

        private static string GetStrings(IEnumerator<string> enumerator)
        {
            StringBuilder sb = new StringBuilder();

            while (enumerator.MoveNext())
            {
                sb.AppendFormat("{0} ", enumerator.Current);
            }

            return sb.ToString().TrimEnd(new char[] { ' ' });
        }
    }
}