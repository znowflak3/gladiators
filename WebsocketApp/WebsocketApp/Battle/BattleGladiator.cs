using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsocketApp.Battle.Skills;


namespace WebsocketApp.Battle
{
    public class BattleGladiator
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public int Stamina { get; set; }
        public int Strength { get; set; }
        public int Speed { get; set; }
        public int Moral { get; set; }
        public int Defense { get; set; }

        public List<Skill> Skills = new List<Skill>();
        public List<Buff> Buffs = new List<Buff>();
        public BattleGladiator()
        {
            this.Health = 100;
            this.Stamina = 600;
            this.Strength = 75;
            this.Speed = 0;
            this.Moral = 0;
            this.Defense = 50;

            Skills.Add(new Attack());
            Skills.Add(new Guard());
        }
        public void AddSkill(Skill skill)
        {
            Skills.Add(skill);
        }
        public void AddBuff(Buff buff)
        {
            Buffs.Add(buff);
        }
    }
}
