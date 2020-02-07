using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketApp.JsonModels
{
    public class GameReturn
    {
        public string GOneHealth { get; set; }
        public string GTwoHealth { get; set; }
        //the clients gladiator
        public string YourGladiator { get; set; }
        //if it is clients turn to attack
        public string YourTurn { get; set; }
        public string TurnCount { get; set; }
    }
}
