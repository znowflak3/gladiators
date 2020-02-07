using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketApp.Battle.Skills
{
    public class SkillRepository
    {
        Dictionary<string, Skill> Skills = new Dictionary<string, Skill>();
        public SkillRepository()
        {
            Skills.Add("attack", new Attack());
            Skills.Add("guard", new Guard());
        }
        public void UseSkill(string skillName, BattleGladiator player, BattleGladiator target)
        {
            Skills[skillName].Use(player, target);
        }
    }
}
