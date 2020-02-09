using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketApp.JsonModels
{
    public class StoreItems
    {
        public string MailType { get; set; }
        public List<Shop.Item> Items { get; set; }
    }
}
