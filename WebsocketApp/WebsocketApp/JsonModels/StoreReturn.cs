using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketApp.JsonModels
{
    public class StoreReturn
    {
        public string MailType { get; set; }
        public bool Rejected { get; set; }
        public string item { get; set; }
    }
}
