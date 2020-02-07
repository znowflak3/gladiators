using System;
using System.Collections.Generic;

/*
    namespace BattlePrototype
    {
        public class Battle
        {
            private Queue<Gladiator> bQueue = new Queue<Gladiator>();
            private Gladiator Cestus;
            private Gladiator Retiarius;
            public Battle()
            {
                Cestus = new Gladiator();
                Cestus.Name = "Cestus";
                Cestus.Strength += 10;
                Retiarius = new Gladiator();
                Retiarius.Name = "Retiarius";
                Retiarius.Health += 50;


            }
            public void Start()
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Northpole Gladiators");
                bool isRunning = true;
                while (isRunning)
                {
                    if (bQueue.Count == 0) DecideTurn();

                    var player = bQueue.Dequeue();
                    Play(player);
                }

            }
            private void Play(Gladiator player)
            {
                Console.Clear();
                var target = player.Name == "Cestus" ? Retiarius : Cestus;
                Console.WriteLine($"{player.Name} Turn - Choose skill");
                Console.WriteLine($"{player.Name} health: {player.Health} ----- {target.Name} health: {target.Health}");

                /*
                 * 
              ^
              |
             _|_ 0
              |-( )-O
               (   )


               
                int skillIndex = 0;
                foreach (Skill skill in player.Skills)
                {
                    Console.WriteLine($"{skillIndex}. {skill.Name}");
                    skillIndex++;
                }
                int choice = -1;
                while (choice < 0 || choice > player.Skills.Count - 1)
                {
                    choice = int.Parse(Console.ReadKey().KeyChar.ToString());
                    Console.WriteLine(choice);
                }

                //TODO-Target Enemy
                Console.WriteLine($"{player.Name} targets {target.Name}");

                //count down 1 turn on all buffs player have;
                if (player.Buffs.Count > 0)
                {
                    foreach (Buff buff in player.Buffs)
                    {
                        buff.Turns--;
                    }
                }
                switch (player.Skills[choice].TargetType)
                {
                    case TargetType.Self:
                        player.Skills[choice].Use(player, player);
                        break;
                    case TargetType.Enemy:
                        player.Skills[choice].Use(player, target);
                        break;
                    case TargetType.Friendly:
                        //TODO Implement target system and a skill vith TargetType Friendly
                        break;
                    default:
                        break;
                }
                //deactive then remove buff if the turns == zero
                if (player.Buffs.Count > 0)
                {
                    foreach (Buff buff in player.Buffs)
                    {
                        if (buff.Turns < 1)
                        {
                            buff.DeActivate(player);
                        }
                    }
                    player.Buffs.RemoveAll(x => x.Turns <= 0);
                }
                Console.ReadKey();
            }
            private void DecideTurn()
            {
                int rnd = new Random().Next(0, 10);
                if (rnd > 5)
                {
                    bQueue.Enqueue(Cestus);
                    bQueue.Enqueue(Retiarius);
                }
                else if (rnd < 6)
                {
                    bQueue.Enqueue(Retiarius);
                    bQueue.Enqueue(Cestus);
                }
            }
        }
        
        
        
        
       
        
        
        
        
    }
}
*/
