using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketApp.Battle
{
    public class Skill : IUse
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public TargetType TargetType { get; set; }
        public virtual void Use(BattleGladiator player, BattleGladiator target)
        {

        }
    }
}
