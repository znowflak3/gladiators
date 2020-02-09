using GamesVonKoch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsocketApp.JsonModels;

namespace WebsocketApp
{
    public static partial class Actors
    {
        public static ActorMeth Buy()
        {
            
            ActorMeth behaviour = (rt, self, _, msg) =>
            {
                StoreBuy storeMsg = msg.content;
                
                return null;
            };
            return behaviour;
        }
    }
}