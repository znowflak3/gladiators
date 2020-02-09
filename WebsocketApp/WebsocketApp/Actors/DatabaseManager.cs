using GamesVonKoch.Core;
using GladiatorDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsocketApp.DbModels;

namespace WebsocketApp
{
    public static partial class Actors
    {
        public static ActorMeth DatabaseManager()
        {
            PID userDb_PId = new PID();

            ActorMeth behaviour = (rt, self, _, msg) =>
            {
                DbMail mail = msg.content;
                switch (msg.mtype)
                {
                    case Symbol.Init:
                        userDb_PId = rt.Spawn(null, UserDb());
                        break;
                    case Symbol.Normal:
                        //mailtype
                        switch (mail.Type)
                        {
                            case DbType.User:
                                rt.Send(userDb_PId, new Mail(Symbol.Normal, mail));
                                break;
                            case DbType.Lanista:
                                break;
                            case DbType.Gladiator:
                                break;
                            case DbType.Item:
                                break;
                            default:
                                break;
                        }
                        break;
                }
                return null;
            };
            return behaviour;
        }
        public static ActorMeth UserDb()
        {
            var userManager = new Usermanager(new EditorContext());

            ActorMeth behaviour = (rt, self, _, msg) =>
           {
               DbMail mail = msg.content;
               switch (mail.Action)
               {
                   case DbAction.Create:
                       //if()
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
