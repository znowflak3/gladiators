using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsocketApp.Battle;

namespace WebsocketApp.JsonModels
{
    public class GameReturn
    {
        public string GOneHealth { get; set; }
        public string GTwoHealth { get; set; }
        //the clients gladiator
        public string YourGladiator { get; set; }
        public List<string> Skills { get; set; }
        public List<string> Buffs { get; set; }
        //if it is clients turn to attack
        public string Turn { get; set; }
        public string TurnCount { get; set; }
        public string Winner { get; set; }
    }
}
