using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockMartianApi.Models
{
    public class Response
    {

        public string message;
        public int statusCode;
        public string? id;

        public Response(string message, int statusCode, string id)
        {
            this.message = message;
            this.statusCode = statusCode;
            this.id = id;
        }

        public Response(String message, int statusCode)
        {
            this.message = message;
            this.statusCode = statusCode;
        }
    }
}
