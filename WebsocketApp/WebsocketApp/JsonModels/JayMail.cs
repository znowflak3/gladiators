using GamesVonKoch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketApp.JsonModels
{
    public class JayMail
    {
        public Symbol MailType { get; set; }
        public object Content { get; set; }
    }
}
