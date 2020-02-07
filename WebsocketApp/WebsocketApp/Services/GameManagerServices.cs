﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsocketApp.Battle;
using WebsocketApp.Battle.Skills;

namespace WebsocketApp.Services
{
    public class GameManagerServices
    {
        Dictionary<string, Skill> Skills = new Dictionary<string, Skill>();
        public GameManagerServices()
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
