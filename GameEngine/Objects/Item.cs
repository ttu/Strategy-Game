using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public class Item
    {
        public bool InUse;
        public int Weight;
    }

    public class ItemSet<T> : List<T> where T : Item
    {
        public T Current { get; set; }
    }

    public class Weapon : Item
    {
        public string Name;
        public int Strength;
        public int Damage;
    }

    public class Sword : Weapon
    {
        public Sword()
        {
            Name = "Sword";
            Strength = 2;
            Damage = 2;
            Weight = 1;
        }
    }

    public class Axe : Weapon
    {
        public Axe()
        {
            Name = "Axe";
            Strength = 2;
            Damage = 2;
            Weight = 1;
        }
    }

    public class Armor : Item
    {
        public int ArmorClass;
    }

}
