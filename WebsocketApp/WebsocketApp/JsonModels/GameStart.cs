using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using WebsocketApp.Battle;

namespace WebsocketApp.JsonModels
{
    public class GameStart
    {
        public string PlayerName { get; set; }
        public string PlayerHealth { get; set; }
        public List<string> PlayerSkills { get; set; }
        public string EnemyName { get; set; }
        public string EnemyHealth { get; set; }
        public string Turn { get; set; }
        
    }
}
