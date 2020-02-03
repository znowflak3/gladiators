using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamesVonKoch.DbModels;
using Microsoft.EntityFrameworkCore.SqlServer;


namespace WebsocketApp.Context
{
    public class DatabaseRepository : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Lanista> Lanisters { get; set; }
        public DbSet<Gladiator> Gladiators { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<GladiatorKills> GladiatorKills { get; set; }
        public DbSet<BattleResult> BattleResults { get; set; }

        protected override void OnConfiguration(DbContextOptionsBuilder optionsBuilder) 
        {
            optionsBuilder.UseSqlServer();
        }
    }
}
