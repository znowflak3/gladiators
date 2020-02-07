using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketApp.Battle.Buffs
{
    public class Guarded : Buff
    {
        private int originalDefense;
        public Guarded()
        {
            Name = "Guarded";
            Turns = 3;
        }
        public override void Activate(BattleGladiator player)
        {
            originalDefense = player.Defense;
            player.Defense += (originalDefense / 2);
        }
        public override void DeActivate(BattleGladiator player)
        {
            player.Defense -= (originalDefense / 2);
        }
    }
}
