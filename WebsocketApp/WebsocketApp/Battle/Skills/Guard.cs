using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsocketApp.Battle.Buffs;

namespace WebsocketApp.Battle.Skills
{
    public class Guard : Skill
    {
        public Guard()
        {
            Name = "Guard";
            Description = "Raises your Defense for 3 turns";
            TargetType = TargetType.Self;
        }
        public override void Use(BattleGladiator player, BattleGladiator target)
        {
            //if buff exist renew (remove and add it again)
            if (player.Buffs.Exists(x => x.Name == "Guarded"))
            {
                var buff = player.Buffs.Find(x => x.Name == "Guarded");
                buff.DeActivate(player);
                player.Buffs.Remove(buff);
                player.AddBuff(new Guarded());
                player.Buffs.Find(x => x.Name == "Guarded").Activate(player);
            }
            else
            {
                player.AddBuff(new Guarded());
                player.Buffs.Find(x => x.Name == "Guarded").Activate(player);
            }
            Console.WriteLine($"{player.Name} raises his guard");
        }
    }
}
