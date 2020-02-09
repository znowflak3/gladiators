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
    public static partial class Actors
    {
        
        

        public static ActorMeth Log()
        {
            ActorMeth behaviour = (rt, self, state, msg) =>
            {
                return null;
            };
            return behaviour;
        }

        
        public static ActorMeth ClientProxy()
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
        
    }
}
