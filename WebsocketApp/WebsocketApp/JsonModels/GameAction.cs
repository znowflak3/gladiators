using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketApp.JsonModels
{
    public class GameAction
    {
        public string pId { get; set; }
        public string action { get; set; }
        public string target { get; set; }
        public string turnCount { get; set; }
    }
}
