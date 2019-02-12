using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace AllegroREST.Models
{
    public class Response
    {
        public static Response Initalize(HttpStatusCode status, bool result, Stream stream) => new Response(status, result, stream);

        public HttpStatusCode Status { get; private set; }
        public bool ResultOk { get; private set; }
        public Stream Stream { get; private set; }


        private Response(HttpStatusCode status, bool result, Stream stream)
        {
            Status = status;
            ResultOk = result;
            Stream = stream;
        }
    }
}
