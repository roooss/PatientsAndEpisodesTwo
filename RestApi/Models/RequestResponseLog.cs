using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestApi.Models
{
    public class RequestResponseLog
    {
        public long ID { get; set; }
        public string Url { get; set; }
        public string RequestType { get; set; }
        public string RequestHeader { get; set; }
        public string RequestBody { get; set; }
        public string IPAddress { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public DateTime RequestDate { get; set; }
        public string ResponseHeader { get; set; }
        public string ResponseBody { get; set; }
        public DateTime ResponseDate { get; set; }
    }
}