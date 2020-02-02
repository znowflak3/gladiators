using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketApp
{
    
        public class User
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
            public virtual List<Lanista> Lanistas { get; set; }

        }

        public class Lanista
        {
            public int LanistaId { get; set; }
            public string LanistaName { get; set; }

            public int UserId { get; set; }
            public virtual User User { get; set; }
            public virtual List<Item> Items { get; set; }
            public virtual List<Gladiator> Gladiators { get; set; }

        }

        public class Gladiator
        {
            public int GladiatorId { get; set; }
            public struct Stats
            {
                public int Health;
                public int Stamina;
                public int Speed;
                public int Morale;
                public int Strength;
                public int Defense;
            }
            public int Wins { get; set; }
            public int Loss { get; set; }
            public int Kills { get; set; }
            public bool Alive { get; set; }

            public int LanistaId { get; set; }
            public virtual Lanista Lanista { get; set; }
            public int BattleId { get; set; }
            public virtual BattleResult LastBattle { get; set; }
            public virtual List<Item> Equipment { get; set; }


        }

        public class Item
        {
            public int ItemId { get; set; }
            public int Type { get; set; }

            public struct Stats
            {
                public int Health;
                public int Stamina;
                public int Speed;
                public int Morale;
                public int Strength;
                public int Defense;
            }
            public virtual Lanista Lanista { get; set; }
            public virtual Gladiator Gladiator { get; set; }

        }


        public class GladiatorKills
        {
            public int GladiatorKillsId { get; set; }
            public int KilledId { get; set; }

            public int BattleId { get; set; }
            public virtual BattleResult BattleResult { get; set; }
        }

        public class BattleResult
        {
            public int BattleResultId { get; set; }
            public int WinnerId { get; set; }
            public int LoserId { get; set; }

            public virtual List<Gladiator> Gladiators { get; set; }
            public virtual List<GladiatorKills> Killed { get; set; }

        }
        /*
        public class EditorContext : DbContext
        {
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GladiatorDB;");
            }

            public DbSet<User> Users { get; set; }
            public DbSet<Lanista> Lanistas { get; set; }
            public DbSet<Gladiator> Gladiators { get; set; }
            public DbSet<Item> Items { get; set; }
            public DbSet<BattleResult> BattleResults { get; set; }
            public DbSet<GladiatorKills> GladiatorKills { get; set; }

        }*/
    
}
