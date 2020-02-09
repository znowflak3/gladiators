using GamesVonKoch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WebsocketApp.Battle;
using WebsocketApp.Battle.Skills;
using WebsocketApp.JsonModels;

namespace WebsocketApp.Services
{
    public static class GameManagerService
    {
        public static GameReturn GetReturnMessage(BattleGladiator gladiator, int turnCount)
        {
            var result = new GameReturn()
            {
                MailType = "gamereturn",
                GOneHealth = gladiator.Health.ToString(),
                GTwoHealth = gladiator.Health.ToString(),
                //Turn = "gladiatorOne",
                TurnCount = turnCount.ToString(),
                Winner = "None",
                Buffs = new List<string>()
            };
            
            foreach (Buff buff in gladiator.Buffs)
            {
                result.Buffs.Add(buff.Name.ToLower());
            }
            return result;
        }
        public static GameStart GetStartMessage(BattleGladiator player, BattleGladiator enemy, PID turn)
        {
            var result = new GameStart()
            {
                MailType = "gamestart",
                PlayerName = player.Name,
                PlayerHealth = player.Health.ToString(),
                PlayerSkills = new List<string>(),
                EnemyName = enemy.Name,
                EnemyHealth = enemy.Health.ToString(),
                Turn = turn.ToString()
                
            };

            foreach (Skill skill in player.Skills)
            {
                result.PlayerSkills.Add(skill.Name.ToLower());
            }

            return result;
        }
        public static void SendStartMessage(WebSocket socket, GameStart msg)
        {
            string json = JsonSerializer.Serialize(msg);
            WebSocketClient.SendMessage(socket, json);
        }
        public static void SendReturnMessage(WebSocket socket, GameReturn msg)
        {
            string json = JsonSerializer.Serialize(msg);
            WebSocketClient.SendMessage(socket, json);
        }
    }
}
