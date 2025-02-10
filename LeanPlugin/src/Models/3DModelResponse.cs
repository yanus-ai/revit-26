using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YANUS_Connector.Models
{
    public class ResponseMsg
    {
        public string message { get; set; }
        public string messageText { get; set; }
        public string link { get; set; }
    }

    public class ModelResponse
    {
        public string status { get; set; }
        public ResponseMsg response { get; set; }
    }
}
