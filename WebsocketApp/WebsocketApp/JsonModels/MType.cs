using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketApp.JsonModels
{
    public class MType
    {
        public string MailType { get; set; }
        public MType()
        { }
        public MType(string type)
        {
            MailType = type;
        }
    }
}
