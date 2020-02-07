using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketApp.Battle
{
    public interface IUse
    {
        void Use(BattleGladiator player, BattleGladiator target);
    }
}
