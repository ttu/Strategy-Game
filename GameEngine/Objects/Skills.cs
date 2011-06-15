using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public class BasicSkills
    {
        public BasicSkills(int move, int actions, int closecombat, int shootingskill, int agility, int thoughness, int strength, int fearvalue, int will, int wounds)
        {
            Move = move;
            Actions = actions;
            CloseCombat = closecombat;
            ShootingSkill = shootingskill;
            Agility = agility;
            Thoughness = thoughness;
            Strength = strength;
            FearValue = fearvalue;
            Will = will;
            Wounds = wounds;
        }

        public int Move { get; set; }
        public int Actions { get; set; }
        public int CloseCombat { get; set; }
        public int ShootingSkill { get; set; }
        public int Agility { get; set; }
        public int Thoughness { get; set; }
        public int Strength { get; set; }
        public int FearValue { get; set; }
        public int Will { get; set; }
        public int Wounds { get; set; }
    }
}
