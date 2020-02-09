using GamesVonKoch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
