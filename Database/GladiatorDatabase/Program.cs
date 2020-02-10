using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GladiatorDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new EditorContext())
            {
                
                Console.WriteLine("Temp Switch Meny");
                Console.WriteLine("1: User. 2: Lanistas. 3: Gladiator. 4: Items. 5: BattleResult. 6: GladiatorKills. ");
                var c = Console.ReadKey();

                switch (c.Key)
                {
                    case ConsoleKey.D1:
                        Console.WriteLine("\nUser: ");
                        Console.WriteLine("1: Create. 2: Read. 3: Update. 4: Destroy. 5: Cancel");
                        var crud = Console.ReadKey();

                        Usermanager usermanager = new Usermanager(db);
                        Lanistamanager lanistamanager = new Lanistamanager(db);
                        Gladiatormanager gladiatormanager = new Gladiatormanager(db);
                        
                        //string username1 = "Timon",Pasword = "hakuna",email = "matata@bajs";
                        //List<User> test;
                        
                        //usermanager.Create(username1,Pasword,email);
                        
                        //test = usermanager.ReadAllUser();
                        
                        /*foreach (var item in test)
                        {
                            Console.WriteLine("user:{0} Pas:{1} em:{2} id:{3}", item.UserName,item.Password,
                                item.Email,item.UserId);
                        }*/
                        
                        //User test2 = test.OrderBy(s => s.UserName).First<User>();
                        //Console.WriteLine("user:{0}", test2.UserName);
                        
                        //lanistamanager.Create("Mouse", test2.UserId);
                        //var test3 = lanistamanager.ReadAll(test2.UserId);
                        /*foreach (var item in test3)
                        {
                            Console.WriteLine("Lani:{0} User:{1} mony:{2} id:{3}", item.LanistaName,item.User.UserName,
                                item.Money,item.LanistaId);
                        }*/
                        //var test4 = test3.OrderBy(s => s.LanistaName).First<Lanista>();
                        //lanistamanager.Update(test4, "Jesus", 0);

                        //gladiatormanager.LanistaCreate(test4);
                        //var test5 = gladiatormanager.LanistaReadAll(test4);
                        /*foreach (var item in test5)
                        {
                            Console.WriteLine("Lani:{0} User:{1} GladID:{2} Morale:{3}", item.Lanista.LanistaName,
                                item.Lanista.User.UserName,
                                item.GladiatorId, item.Morale);
                        }*/
                        //gladiatormanager.Update()
                        //usermanager.Update(test2, username1, Pasword,email,false);
                        //usermanager.Destroy();


                        break;
                    case ConsoleKey.D2:
                        Console.WriteLine("Lanistas: ");
                        Console.WriteLine("1: Create. 2: Read. 3: Update. 4: Destroy.");
                        break;
                    case ConsoleKey.D3:
                        Console.WriteLine("Gladiator: ");
                        Console.WriteLine("1: Create. 2: Read. 3: Update. 4: Destroy.");
                        break;
                    case ConsoleKey.D4:
                        Console.WriteLine("Items: ");
                        Console.WriteLine("1: Create. 2: Read. 3: Update. 4: Destroy.");
                        break;
                    case ConsoleKey.D5:
                        Console.WriteLine("BattleResult: ");
                        Console.WriteLine("1: Create. 2: Read. 3: Update. 4: Destroy.");
                        break;
                    case ConsoleKey.D6:
                        Console.WriteLine("GladiatorKills: ");
                        Console.WriteLine("1: Create. 2: Read. 3: Update. 4: Destroy.");
                        break;
                }


                Console.WriteLine("**********");
                Console.ReadKey();


            }
        }
    }


    public class Usermanager
    {
        public Usermanager(EditorContext db)
        {
            DB = db;
        }

        EditorContext DB { get; set; }

        public void Create(string userName, string password, string email)
        {
            Console.WriteLine("\n*Create User*");
            var name = userName;
            var User = new User { UserName = name, Password = password, Email = email, IsAdmin = false };
            var dupUser = DB.Users.Where(s => s.UserName == name).ToList();
            var colit = dupUser.Count;
            if (colit != 0)
            {
                Console.WriteLine("\nNot a valid User Name!\n");
            }
            else
            {
                DB.Users.Add(User);
                DB.SaveChanges();
            }
        }
        public List<User> ReadAllUser()
        {
            Console.WriteLine("\n*Read AllUser*");
            var SortUsers = DB.Users.OrderBy(s => s.UserName).ToList<User>();
            return SortUsers;
        }
        public User Read(string userName)
        {
            Console.WriteLine("\n*Read User*");
            var user = DB.Users.Single<User>(s => s.UserName == userName);
            return user;
        }
        public void Update(User user, string userName, string password, string email, bool admin)
        {
            Console.WriteLine("\n*Update User*");
            user.UserName = userName;
            user.Password = password;
            user.Email = email;
            user.IsAdmin = admin;

            Console.WriteLine("Confirm [y]: ");
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Y)
            {
                DB.Users.Update(user);
                DB.SaveChanges();
            }
        }
        public void Destroy(User user)

        {
            Console.WriteLine("\n*Destroy User*");
            DB.Users.Remove(user);

            Console.WriteLine("Confirm Delete [y]: ");
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Y)
            {
                DB.SaveChanges();
            }

        }

    }
    public class Lanistamanager
    {
        public Lanistamanager(EditorContext db)
        {
            DB = db;
        }

        EditorContext DB { get; set; }

        public void Create(string lanistaName, int userId)
        {
            Console.WriteLine("\n*Create Lanista*");
            var name = lanistaName;
            var Lanista = new Lanista { LanistaName = name, UserId = userId, Money = 100 };
            DB.Lanistas.Add(Lanista);
            DB.SaveChanges();
        }
        public List<Lanista> ReadAll(int userId)
        {
            Console.WriteLine("\n*Read AllLanista*");
            var SortLanistas = DB.Lanistas.Where(s => s.UserId == userId).OrderBy(s => s.LanistaName).ToList<Lanista>();
            return SortLanistas;
        }
        public Lanista Read(string lanistaName, int userId)
        {
            var lanista = DB.Lanistas.OrderBy(s => s.UserId).Where(s => s.UserId == userId).Single<Lanista>(s => s.LanistaName == lanistaName);
            return lanista;
        }
        
        public void Update(Lanista lanista, string lanistaName, int money)
        {
            Console.WriteLine("\n*Update Lanista*");
            lanista.LanistaName = lanistaName;
            lanista.Money = money;

            Console.WriteLine("Confirm [y]: ");
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Y)
            {
                DB.Lanistas.Update(lanista);
                DB.SaveChanges();
            }
        }
        public void Destroy(Lanista lanista)
        {
            Console.WriteLine("\n*Destroy Lanista*");
            DB.Lanistas.Remove(lanista);

            Console.WriteLine("Confirm Delete [y]: ");
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Y)
            {
                DB.SaveChanges();
            }

        }

    }
    public class Gladiatormanager
    {
        public Gladiatormanager(EditorContext db)
        {
            DB = db;
        }

        EditorContext DB { get; set; }


        /*public void ShopCreate(Shop shop)
        {
            Console.WriteLine("\n*ShopCreate Gladiator*");
            var owner = shop.ShopId;
            var Gladiator = new Gladiator { ShopId = owner, Alive = true, Health = 20, Stamina = 25, Strength = 15, Defense = 0, Speed = 0, Morale = 50, Kills = 0, Loss = 0, Wins = 0 };
            DB.Gladiators.Add(Gladiator);
            DB.SaveChanges();
        }*/
        public void LanistaCreate(Lanista lanista)
        {
            Console.WriteLine("\n*LanistaCreate Gladiator*");
            var owner = lanista.LanistaId;
            var Gladiator = new Gladiator {
                LanistaId = owner,
                Alive = true,
                Health = 20, Stamina = 25, Strength = 15, Defense = 0, Speed = 0, Morale = 50, 
                Kills = 0, Loss = 0, Wins = 0 };
            DB.Gladiators.Add(Gladiator);
            DB.SaveChanges();
        }
        /*public List<Gladiator> ShopReadAll(Shop shop)
        {
            Console.WriteLine("\n*Read ShopGladiator*");
            var SortGladiatorShop = DB.Gladiators.Where(s => s.ShopId == shop.ShopId).OrderBy(s => s.GladiatorId).ToList<Gladiator>();
            return SortGladiatorShop;
        }*/
        public List<Gladiator> LanistaReadAll(Lanista lanista) 
        {
            Console.WriteLine("\n*Read LanistaGladiator*");
            var SortGladiatorLanista = DB.Gladiators.Where(s => s.LanistaId == lanista.LanistaId)
                .OrderBy(s => s.GladiatorId).ToList<Gladiator>();
            return SortGladiatorLanista;
        }
        public Gladiator Read(Lanista lanista)
        {
            var gladiator = DB.Gladiators.OrderBy(s => s.LanistaId).Where(s => s.LanistaId == lanista.LanistaId).Single<Gladiator>();
            return gladiator;
        }
        
        public void Update(Lanista lanista, Gladiator gladiator, string gladiatorName,
            bool alive, bool win, bool killed)

        {
            Console.WriteLine("\n*Update Gladiator*");

            gladiator.LanistaId = lanista.LanistaId;
            gladiator.GladiatorName = gladiatorName;

            DB.Gladiators.Update(gladiator);
            DB.SaveChanges();

            gladiator.Alive = alive;
            if(win == true) 
            {
                gladiator.Wins += 1;
            }
            else 
            {
                gladiator.Loss += 1; 
            }
            if(killed == true) 
            {
                gladiator.Kills += 1;
            }

        }
        public void Destroy(Gladiator gladiator)
        {
            Console.WriteLine("\n*Destroy Gladiator*");
            DB.Gladiators.Remove(gladiator);

            Console.WriteLine("Confirm Delete [y]: ");
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Y)
            {
                DB.SaveChanges();
            }

        }

    }

    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public virtual List<Lanista> Lanistas { get; set; }

    }

    public class Lanista
    {
        public int LanistaId { get; set; }
        public string LanistaName { get; set; }
        public int Money { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual List<Item> Items { get; set; }
        public virtual List<Gladiator> Gladiators { get; set; }

    }

    public class Gladiator
    {
        public int GladiatorId { get; set; }
        public string GladiatorName { get; set; }
        /*todo gladiator type*/

        public int Health { get; set; }
        public int Stamina { get; set; }
        public int Speed { get; set; }
        public int Morale { get; set; }
        public int Strength { get; set; }
        public int Defense { get; set; }

        public int Wins { get; set; }
        public int Loss { get; set; }
        public int Kills { get; set; }
        public bool Alive { get; set; }

        /*public int ShopId { get; set; }*/
        public virtual Shop Shop { get; set; }
        
        public int LanistaId { get; set; }
        public virtual Lanista Lanista { get; set; }
        
        /*todo fix key
         * public int BattleResultId { get; set; }
        public virtual BattleResult LastBattle { get; set; }*/
        public virtual List<Item> Equipment { get; set; }


    }

    public class Item
    {
        public int ItemId { get; set; }
        public int Type { get; set; }

        public int Health { get; set; }
        public int Stamina { get; set; }
        public int Speed { get; set; }
        public int Morale { get; set; }
        public int Strength { get; set; }
        public int Defense { get; set; }

        public virtual Shop Shop { get; set; }
        public virtual Lanista Lanista { get; set; }
        public virtual Gladiator Gladiator { get; set; }

    }

    /*
    public class GladiatorKills
    {
        public int GladiatorKillsId { get; set; }
        public int KilledId { get; set; }

        public int BattleResultId { get; set; }
        public virtual BattleResult BattleResult { get; set; }
    }

    public class BattleResult
    {
        public int BattleResultId { get; set; }
        public int WinnerId { get; set; }
        public int LoserId { get; set; }

        public virtual List<Gladiator> Gladiators { get; set; }
        public virtual List<GladiatorKills> Killed { get; set; }

    }*/

    public class Shop
    {
        public int ShopId { get; set; }

        public virtual List<Gladiator> Gladiators { get; set; }
        public virtual List<Item> Items { get; set; }
    }

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
        /*todo fix key
         * public DbSet<BattleResult> BattleResults { get; set; }
        public DbSet<GladiatorKills> GladiatorKills { get; set; }*/
        public DbSet<Shop> Shop { get; set; }
    }
}