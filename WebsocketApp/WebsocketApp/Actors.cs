using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GamesVonKoch.Core;
using WebsocketApp.JsonModels;
using WebsocketApp.Battle;
using System.Threading;
using WebsocketApp.Services;
using WebsocketApp.Battle.Skills;

namespace WebsocketApp
{
    public static class Actors
    {
        public static ActorMeth ClientProxy()
        {
            ActorMeth behaviour = (rt, self, _, msg) =>
            {
                return null;
            };
            return behaviour;
        }
        public static ActorMeth SessionManager()
        {
            Queue<PID> playQueue = new Queue<PID>();
            Dictionary<PID, PID> clientToGames = new Dictionary<PID, PID>();

            ActorMeth behaviour = (rt, self, _, msg) =>
            {
                switch (msg.mtype)
                {
                    case Symbol.QueueGame:
                        PID key = new PID(long.Parse(msg.content.pId));
                        if (!playQueue.Contains(key))
                        {
                            playQueue.Enqueue(key);
                        }
                        if (playQueue.Count > 1)
                        {
                            var playerOne = playQueue.Dequeue();
                            var playerTwo = playQueue.Dequeue();
                            PID[] players = new PID[] { playerOne, playerTwo };
                            var gameManager_pid = rt.SpawnLink(null, GameManager());
                            clientToGames.Add(playerOne, gameManager_pid);
                            clientToGames.Add(playerTwo, gameManager_pid);
                            rt.Send(gameManager_pid, new Mail(Symbol.Init, players));
                        }
                        break;
                    case Symbol.GameAction:
                        var to = clientToGames[new PID(long.Parse(msg.content.pId))];
                        rt.Send(to, msg);
                        break;
                    default:
                        break;
                }
                return null;
            };

            return behaviour;
        }
        public static ActorMeth SessionRelay()
        {
            ActorMeth behaviour = (rt, self, _, msg) =>
            {
                return null;
            };
            return behaviour;
        }
        public static ActorMeth GameManager()
        {
            BattleGladiator gladiatorOne = new BattleGladiator();
            BattleGladiator gladiatorTwo = new BattleGladiator();

            int turnCount = 0;

            PID playerOne = new PID();
            PID playerTwo = new PID();

            bool isFinished = false;

            ActorMeth behaviour = (rt, self, _, msg) =>
            {
                switch (msg.mtype)
                {
                    case Symbol.Init:
                        playerOne = msg.content[0];
                        playerTwo = msg.content[1];

                        var pOneMsg = new GameReturn()
                        {
                            GOneHealth = gladiatorOne.Health.ToString(),
                            GTwoHealth = gladiatorTwo.Health.ToString(),
                            YourGladiator = "gladiatorOne",
                            YourTurn = "gladiatorOne",
                            TurnCount = turnCount.ToString()
                        };

                        var pTwoMsg = new GameReturn()
                        {
                            GOneHealth = gladiatorOne.Health.ToString(),
                            GTwoHealth = gladiatorTwo.Health.ToString(),
                            YourGladiator = "gladiatorTwo",
                            YourTurn = "gladiatorTwo",
                            TurnCount = turnCount.ToString()
                        };

                        byte[] buffer;
                        string json = JsonSerializer.Serialize(pOneMsg);

                        buffer = Encoding.UTF8.GetBytes(json);
                        WebSocket socketOne = rt.GetWebSocket(playerOne);
                        socketOne.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                        json = JsonSerializer.Serialize(pTwoMsg);
                        buffer = Encoding.UTF8.GetBytes(json);
                        WebSocket socketTwo = rt.GetWebSocket(playerTwo);
                        socketTwo.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                        break;
                    case Symbol.GameAction:
                        GameAction gAction = msg.content;
                        var skills = new SkillRepository();
                        
                        if ((turnCount & 1) == 0)// gladiator a
                        {
                            if (new PID(long.Parse(gAction.PId)).ToString() == playerOne.ToString())
                            {
                                skills.UseSkill(gAction.Action, gladiatorOne, gladiatorTwo);
                            }
                            else
                            {
                                //send back error or msg to sync?
                            }
                        }
                        else // gladiator b
                        {
                            if (new PID(long.Parse(gAction.PId)).ToString() == playerOne.ToString())
                            { 
                                skills.UseSkill(gAction.Action, gladiatorTwo, gladiatorOne); 
                            }
                            else
                            {
                                //send back error or msg to sync?
                            }
                        }
                        turnCount++;
                        break;
                    default:
                        break;
                }
                if (!isFinished)
                {

                }
                return null;
            };

            return behaviour;
        }

        public static ActorMeth Log()
        {
            ActorMeth behaviour = (rt, self, state, msg) =>
            {
                return null;
            };
            return behaviour;
        }

        public static ActorMeth Database()
        {
            ActorMeth behaviour = (rt, self, _, msg) =>
            {
                return null;
            };
            return behaviour;
        }
        public static ActorMeth Login()
        {
            ActorMeth behaviour = (rt, self, _, msg) =>
            {
                switch (msg.mtype)
                {
                    case Symbol.Authorize:
                        //check if user exist
                        //check if password matches.
                        //add socket to socketDictionary.
                        break;
                    case Symbol.CreateUser:

                        break;
                    default:
                        break;
                }
                return null;
            };
            return behaviour;
        }
        public static ActorMeth Echo()
        {
            ActorMeth behaviour = (rt, self, _, msg) =>
            {
                if (msg.mtype == Symbol.Echo)
                {
                    byte[] buffer;
                    string json = JsonSerializer.Serialize<JsonPID>(msg.content);
                    buffer = Encoding.UTF8.GetBytes(json);
                    PID webSocketKey = new PID(long.Parse(msg.content.pId));
                    WebSocket socket = rt.GetWebSocket(webSocketKey);
                    socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                return null;
            };
            return behaviour;
        }
    }
}
