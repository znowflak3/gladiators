using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsocketApp.Battle;
using WebsocketApp.Battle.Skills;
using WebsocketApp.JsonModels;

namespace WebsocketApp.Services
{
    public class GameManagerService
    {
        public GameReturn GetReturnMessage(BattleGladiator gladiator)
        {
            var result = new GameReturn()
            {
                GOneHealth = gladiator.Health.ToString(),
                GTwoHealth = gladiator.Health.ToString(),
                //YourGladiator = "gladiatorOne",
                //YourTurn = "gladiatorOne",
                //TurnCount = turnCount.ToString(),
                Skills = new List<string>(),
                Buffs = new List<string>()
            };
            foreach (Skill skill in gladiator.Skills)
            {
                result.Skills.Add(skill.Name.ToLower());
            }
            foreach (Buff buff in gladiator.Buffs)
            {
                result.Buffs.Add(buff.Name.ToLower());
            }
            return result;
        }
    }
}
