using GamesVonKoch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WebsocketApp.JsonModels;

namespace WebsocketApp
{
    public static partial class Actors
    {
        public static ActorMeth SessionManager()
        {
            Queue<PID> playQueue = new Queue<PID>();
            Dictionary<PID, PID> clientToGames = new Dictionary<PID, PID>();

            ActorMeth behaviour = (rt, self, _, msg) =>
            {
                switch (msg.mtype)
                {
                    case Symbol.QueueGame:
                        PID key = new PID(long.Parse(msg.content.PId));
                        if (!playQueue.Contains(key))
                        {
                            playQueue.Enqueue(key);
                            Response response = new Response()
                            {
                                MailType = "queuegame",
                                MailResponse = "inqueue"
                            };
                            string json = JsonSerializer.Serialize(response);
                            byte[] buffer = Encoding.UTF8.GetBytes(json);
                            rt.GetWebSocket(key).SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                        else if (playQueue.Contains(key))
                        {
                            Response response = new Response()
                            {
                                MailType = "queuegame",
                                MailResponse = "allreadyinqueue"
                            };
                            string json = JsonSerializer.Serialize(response);
                            byte[] buffer = Encoding.UTF8.GetBytes(json);
                            rt.GetWebSocket(key).SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                        if (playQueue.Count > 1)
                        {
                            var playerOne = playQueue.Dequeue();
                            var playerTwo = playQueue.Dequeue();
                            PID[] players = new PID[] { playerOne, playerTwo };
                            var gameManager_pid = rt.SpawnLink(null, GameManager(self));
                            clientToGames.Add(playerOne, gameManager_pid);
                            clientToGames.Add(playerTwo, gameManager_pid);
                            rt.Send(gameManager_pid, new Mail(Symbol.Init, players));
                        }
                        break;
                    case Symbol.GameAction:
                        if (clientToGames.ContainsKey(new PID(long.Parse(msg.content.PId))))
                        {
                            var to = clientToGames[new PID(long.Parse(msg.content.PId))];
                            rt.Send(to, msg);
                        }
                        break;
                    case Symbol.Killed:
                        foreach (PID pid in msg.content)
                        {
                            clientToGames.Remove(pid);
                        }
                        break;
                    default:
                        break;
                }
                return null;
            };

            return behaviour;
        }
    }
}
