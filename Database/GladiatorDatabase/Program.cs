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

                        //usermanager.Create();
                        //usermanager.Read();
                        //usermanager.Update();
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
        var User = new User { UserName = name, Password = password, Email = email };
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
    public void Read()
    {   
        Console.WriteLine("\n*Read User*");
        var SortUsers = DB.Users.OrderBy(s => s.UserName);
        var check = SortUsers.Count<User>();
        if (check > 0)
        {
            foreach (var item in SortUsers)
            {
                Console.WriteLine("User: {0} UserId: {1}", item.UserName, item.UserId);
            }
        }
        else 
        { 
            Console.WriteLine("No Users!"); 
        }
    }
    public void Update(User user, string userName, string password, string email)
    {
        Console.WriteLine("\n*Update User*");
        user.UserName = userName;
        user.Password = password;
        user.Email = email;

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

    public int Health;
    public int Stamina;
    public int Speed;
    public int Morale;
    public int Strength;
    public int Defense;
    
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

    public int Health;
    public int Stamina;
    public int Speed;
    public int Morale;
    public int Strength;
    public int Defense;
    
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

public class EditorContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GladiatorDB;");
    }

    public DbSet<User> Users {get;set;}
    public DbSet<Lanista> Lanistas { get; set; }
    public DbSet<Gladiator> Gladiators { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<BattleResult> BattleResults { get; set; }
    public DbSet<GladiatorKills> GladiatorKills { get; set; }

}