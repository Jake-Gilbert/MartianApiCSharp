using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockMartianApi.Models
{
    internal class Response
    {

        public string message;
        public int statusCode;

        public Response(string message, int statusCode)
        {
            this.message = message;
            this.statusCode = statusCode;
        }
    }
}
