using GamesVonKoch.Core;
using GladiatorDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebsocketApp.JsonModels;
using WebsocketApp.Services;

namespace WebsocketApp
{
    public static partial class Actors
    {
        public static ActorMeth Shop()
        {
            List<Shop.Item> ItemShop = new List<Shop.Item>();
            ItemShop.Add(new Shop.Item("Gladius", "A mighty Sword", 100));
            ItemShop.Add(new Shop.Item("Gladius", "A mighty Sword", 100));
            ItemShop.Add(new Shop.Item("Gladius", "A mighty Sword", 100));
            ItemShop.Add(new Shop.Item("Gladius", "A mighty Sword", 100));
            ItemShop.Add(new Shop.Item("Gladius", "A mighty Sword", 100));

            ActorMeth behaviour = (rt, self, _, msg) =>
            {
                
                switch (msg.mtype)
                {
                    case Symbol.Items:
                        StoreItems items = new StoreItems()
                        {
                            MailType = "items",
                            Items = ItemShop
                        };
                        string json = JsonSerializer.Serialize(items);
                        WebSocketClient.SendMessage(rt.GetWebSocket(new PID(long.Parse(msg.content.PId))), json);
                        break;
                    case Symbol.Buy:
                        break;
                }
                return null;
            };
            return behaviour;
        }
    }
}