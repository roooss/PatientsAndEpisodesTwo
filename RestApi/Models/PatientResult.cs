using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestApi.Models
{
    public class PatientResult
    {
        public bool IsSuccessful { get; set; }

        public string Message { get; set; }

        public ResultStatusEnum ResultStatus { get; set; }

        public Patient Patient { get; set; }
    }
}