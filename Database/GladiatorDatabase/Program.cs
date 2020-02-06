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
                        usermanager.Read();
                        usermanager.Update();
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

                 
                /*
                 * en komentar
                Console.WriteLine("User Name:");
                var name = Console.ReadLine();
                
                var User = new User { UserName = name };
                db.Users.Add(User);
                db.SaveChanges();

                Console.WriteLine("Lanista Name:");
                name = Console.ReadLine();
                
                var Lanista = new Lanista { LanistaName = name };
                db.Lanistas.Add(Lanista);
                db.SaveChanges();

                var test = from s in db.Lanistas orderby s.LanistaName select s;
                 
                foreach (var item in test)
                {
                    Console.WriteLine(item.LanistaName);
                    Console.WriteLine(item.LanistaId);
                };*/
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
    
    public void Create()
    {
        Console.WriteLine("\n*Create User*");
        Console.WriteLine("User Name: ");
        var name = Console.ReadLine();
        var User = new User { UserName = name };
        DB.Users.Add(User);
        DB.SaveChanges();
    }
    public void Read() 
    {   
        Console.WriteLine("\n*Read User*");
        Console.WriteLine("User Name: ");
        var SortUsers = from s in DB.Users orderby s.UserName select s;
        
        foreach (var item in SortUsers)
        {
            Console.WriteLine("User: {0} UserId: {1}", item.UserName, item.UserId);
        };
    }
    public void Update()
    {
        Console.WriteLine("\n*Update User*");
        Console.WriteLine("User Name: ");
        var name = Console.ReadLine();
        var dupUserName = DB.Users.Where(s => s.UserName == name).ToList();
        User user;
        
        if ((dupUserName.Count != 1) && (dupUserName.Count > 0)) 
        { 
            Console.WriteLine("\nNot Unik User Name.");
            Console.WriteLine("\nEnter Id: ");
            var check = int.TryParse(Console.ReadLine(), out int userid);
            if (check) 
            {

                user = DB.Users.Find(userid);
            }
            else
            {
                Console.WriteLine("\nInvalid Id Input!\n");
            }           
        }
        else if (dupUserName.Count == 1)
        {

            user = dupUserName.Single();

        }
        else
        {
            Console.WriteLine("\nInvalid Name Input!\n");
            
        }


        Console.WriteLine("\nUser Name: ");
        var input = Console.ReadLine();
        user.UserName = input;
        Console.WriteLine("\nUser Password: ");
        DB.Users.Find(update).Password = Console.ReadLine();
        Console.WriteLine("\nUser e-mail: ");
        DB.Users.Find(update).Email = Console.ReadLine();
        
        Console.WriteLine("Confirm [y]: ");
        var key = Console.ReadKey();
        if(key.Key == ConsoleKey.Y) 
        {
            DB.Users.Update(DB.Users.Find(update));
            DB.SaveChanges();
        }


    }
    public void Destroy()
    {
        Console.WriteLine("\n*Destroy User*");
        Console.WriteLine("\nUser Name: ");
        var username = Console.ReadLine();
        Console.WriteLine("\nUser ID: ");
        int.TryParse(Console.ReadLine(), out int userid);

        
        
        

        
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