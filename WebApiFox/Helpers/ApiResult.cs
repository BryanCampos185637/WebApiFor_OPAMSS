using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiFox.Helpers
{
    public class ApiResult
    {
        public bool State { get; set; } = true;
        public string Message { get; set; } = "Proceso satisfactorio.";
        public Object Data { get; set; }
        public Exception Exception { get; set; }

    }
}