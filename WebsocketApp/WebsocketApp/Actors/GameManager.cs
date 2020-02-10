using GamesVonKoch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsocketApp.Battle;
using WebsocketApp.Battle.Skills;
using WebsocketApp.JsonModels;
using WebsocketApp.Services;

namespace WebsocketApp
{
    public static partial class Actors
    {
        public static ActorMeth GameManager(PID pid)
        {
            BattleGladiator gladiatorOne = new BattleGladiator();
            gladiatorOne.Name = "gladiatorOne";
            BattleGladiator gladiatorTwo = new BattleGladiator();
            gladiatorTwo.Name = "gladiatorTwo";

            int turnCount = 0;
            PID parent = pid;
            PID playerOne = new PID();
            PID playerTwo = new PID();

            bool isFinished = false;


            ActorMeth behaviour = (rt, self, _, msg) =>
            {
                GameReturn pOneMsg = new GameReturn();
                GameReturn pTwoMsg = new GameReturn();


                switch (msg.mtype)
                {
                    case Symbol.Init:
                        playerOne = msg.content[0];
                        playerTwo = msg.content[1];

                        var startOneMsg = GameManagerService.GetStartMessage(gladiatorOne, gladiatorTwo, playerOne);
                        GameManagerService.SendStartMessage(rt.GetWebSocket(playerOne), startOneMsg);

                        var startTwoMsg = GameManagerService.GetStartMessage(gladiatorTwo, gladiatorOne, playerOne);
                        GameManagerService.SendStartMessage(rt.GetWebSocket(playerTwo), startTwoMsg);
                        break;
                    case Symbol.GameAction:
                        GameAction gAction = msg.content;
                        var skills = new SkillRepository();

                        if ((turnCount & 1) == 0)// gladiator a
                        {
                            if (new PID(long.Parse(gAction.PId)).ToString() == playerOne.ToString())
                            {
                                skills.UseSkill(gAction.Action, gladiatorOne, gladiatorTwo);
                                //deactive then remove buff if the turns == zero
                                if (gladiatorOne.Buffs.Count > 0)
                                {
                                    foreach (Buff buff in gladiatorOne.Buffs)
                                    {
                                        buff.Turns--;
                                        if (buff.Turns <= 0)
                                            buff.DeActivate(gladiatorOne);
                                    }
                                    gladiatorOne.Buffs.RemoveAll(x => x.Turns <= 0);
                                }
                            }
                            else
                            {
                                //send back msg to sync?
                            }

                        }
                        else if ((turnCount & 1) != 0) // gladiator b
                        {
                            if (new PID(long.Parse(gAction.PId)).ToString() == playerTwo.ToString())
                            {
                                skills.UseSkill(gAction.Action, gladiatorTwo, gladiatorOne);
                                if (gladiatorTwo.Buffs.Count > 0)
                                {
                                    foreach (Buff buff in gladiatorTwo.Buffs)
                                    {
                                        buff.Turns--;
                                        if (buff.Turns <= 0)
                                            buff.DeActivate(gladiatorTwo);
                                    }
                                    gladiatorTwo.Buffs.RemoveAll(x => x.Turns <= 0);
                                }
                            }
                            else
                            {
                                //send back msg to sync?
                            }
                        }
                        turnCount++;
                        //send synced userdata back to user
                        pOneMsg = GameManagerService.GetReturnMessage(gladiatorOne, turnCount);
                        pTwoMsg = GameManagerService.GetReturnMessage(gladiatorTwo, turnCount);
                        if ((turnCount & 1) == 0)
                        {
                            pOneMsg.Turn = playerOne.ToString();
                            pTwoMsg.Turn = playerOne.ToString();
                            pOneMsg.GOneHealth = gladiatorOne.Health.ToString();
                            pTwoMsg.GOneHealth = gladiatorTwo.Health.ToString();
                            pOneMsg.GTwoHealth = gladiatorTwo.Health.ToString();
                            pTwoMsg.GTwoHealth = gladiatorOne.Health.ToString();
                        }
                        else if ((turnCount & 1) != 0)
                        {
                            pOneMsg.Turn = playerTwo.ToString();
                            pTwoMsg.Turn = playerTwo.ToString();
                            pOneMsg.GOneHealth = gladiatorOne.Health.ToString();
                            pTwoMsg.GOneHealth = gladiatorTwo.Health.ToString();
                            pOneMsg.GTwoHealth = gladiatorTwo.Health.ToString();
                            pTwoMsg.GTwoHealth = gladiatorOne.Health.ToString();
                        }
                        if (!isFinished)
                        {
                            if (gladiatorOne.Health <= 0)
                            {
                                pOneMsg.Winner = gladiatorTwo.Name;
                                pTwoMsg.Winner = gladiatorTwo.Name;
                                isFinished = true;
                            }
                            if (gladiatorTwo.Health <= 0)
                            {
                                pOneMsg.Winner = gladiatorOne.Name;
                                pTwoMsg.Winner = gladiatorOne.Name;
                                isFinished = true;
                            }
                        }

                        GameManagerService.SendReturnMessage(rt.GetWebSocket(playerOne), pOneMsg);
                        GameManagerService.SendReturnMessage(rt.GetWebSocket(playerTwo), pTwoMsg);

                        if (isFinished)
                        {
                            PID[] players = new PID[] { playerOne, playerTwo };
                            rt.Send(parent, new Mail(Symbol.Killed, players));
                            rt.Die();
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
