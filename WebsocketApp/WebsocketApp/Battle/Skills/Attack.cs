using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketApp.Battle.Skills
{
    public class Attack : Skill
    {
        public Attack()
        {
            Name = "Attack";
            Description = "Basic attack";
            TargetType = TargetType.Enemy;

        }
        public override void Use(BattleGladiator player, BattleGladiator target)
        {
            int damage = player.Strength - target.Defense;
            if (damage < 0) damage = 0;
            target.Health -= damage;
            Console.WriteLine($"{player.Name} attacks {target.Name} for {damage}dmg");
        }
    }
}
