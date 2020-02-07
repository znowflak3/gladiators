using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketApp.JsonModels
{
    public class GameAction
    {
        public string PId { get; set; }
        public string Action { get; set; }
        public string Target { get; set; }
        public string TurnCount { get; set; }
    }
}
