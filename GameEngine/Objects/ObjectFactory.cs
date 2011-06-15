using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    /*
     * This class is for creating most common character templates 
     */
    public static class ObjectFactory
    {
        public static GameObject House()
        {
            GameObject go = new GameObject();
            go.SetImage("House.png");

            return go;
        }

        public static GameObject FenceVertical()
        {
            GameObject go = new GameObject();
            go.SetImage("Fence_vertical.png");

            return go;
        }

        public static GameObject FenceHorizontal()
        {
            GameObject go = new GameObject();
            go.SetImage("Fence_horizontal.png");

            return go;
        }

        public static Character GetPeasant(int index, BehaviorType bh)
        {
            Character c = new Character(bh);
            c.BasicSkills = new BasicSkills(4, 2, 2, 2, 2, 2, 2, 1, 1, 1);
            c.CC_weapon = new Axe();
            c.Armor = None();
            if (index == 0)
                c.SetImage("Square1.png");
            else
                c.SetImage("Square2.png");
            return c;
        }

        public static Character GetCommoner(int index)
        {
            Character c = new Character();
            c.BasicSkills = new BasicSkills(5, 2, 3, 3, 3, 3, 3, 2, 2, 1);
            c.CC_weapon = new Sword();
            c.Armor = GetShield();
            if (index == 0)
                c.SetImage("Square1.png");
            else
                c.SetImage("Square2.png");
            return c;
        }

        public static Character GetKnight(int index, BehaviorType bh)
        {
            Character c = new Character(bh);
            c.BasicSkills = new BasicSkills(5, 3, 6, 3, 4, 4, 4, 6, 8, 2);
            c.CC_weapon = new Sword();
            c.Armor = FullPlate();
            if (index == 0)
                c.SetImage("Square1.png");
            else
                c.SetImage("Square2.png");
            return c;
        }

        public static Character GetWarrior(int index)
        {
            Character c = new Character();
            c.BasicSkills = new BasicSkills(6, 3, 4, 3, 4, 4, 4, 4, 6, 2);
            c.CC_weapon = new Sword();
            c.Armor = FullPlate();
            if (index == 0)
                c.SetImage("Square1.png");
            else
                c.SetImage("Square2.png");
            return c;
        }

        public static Armor ChainMail()
        {
            Armor a = new Armor();
            a.ArmorClass = 4;
            return a;
        }

        public static Armor FullPlate()
        {
            Armor a = new Armor();
            a.ArmorClass = 6;
            return a;
        }

        public static Armor GetShield()
        {
            Armor a = new Armor();
            a.ArmorClass = 2;
            return a;
        }

        public static Armor None()
        {
            Armor a = new Armor();
            a.ArmorClass = 0;
            return a;
        }

    }
}
