using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketApp.DbModels
{
    public class DbMail
    {
        public long PId { get; set; }
        public DbType Type { get; set; }
        public DbAction Action { get; set; }
        public dynamic Content { get; set; }
    }
    public enum DbType
    {
        User,
        Lanista,
        Gladiator,
        Item
    }
    public enum DbAction
    {
        Create,
        Read, ReadAll,
        Update,
        Destroy
    }
}
