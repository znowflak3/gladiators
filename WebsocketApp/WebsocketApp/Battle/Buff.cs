using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketApp.Battle
{
    public class Buff
    {
        public string Name { get; set; }
        public int Turns { get; set; }
        public virtual void Activate(BattleGladiator player)
        {
            throw new NotImplementedException();
        }
        public virtual void DeActivate(BattleGladiator player)
        {
            throw new NotImplementedException();
        }
    }
}
