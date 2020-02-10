using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketApp.JsonModels
{
    public class AdminList
    {
        public string MailType { get; set; }
        public List<string> Users { get; set; }
    }
}
