using GamesVonKoch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketApp.JsonModels
{
    public class JsonPID
    { 
        public string PId { get; set; }
        public JsonPID(PID id) 
        {
            PId = id.ToString();
        }
        public JsonPID()
        { }
    }
}
