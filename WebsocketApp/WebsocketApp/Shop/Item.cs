using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketApp.Shop
{
    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public Item(string name, string desc, int cost)
        {
            Name = name;
            Description = desc;
            Cost = cost;
        }
    }
    public enum ItemType
    {
        Gladius,
        Armor
    }
}
