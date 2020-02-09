using GamesVonKoch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketApp.JsonModels
{
    public class JsonPID
    { 
        public string MailType { get; set; }
        public string PId { get; set; }
        public JsonPID(PID id, string type)
        {
            PId = id.ToString();
            MailType = type.ToString();
        }
        public JsonPID(PID id) 
        {
            PId = id.ToString();
            MailType = "none";
        }
        public JsonPID()
        { }
    }
}
