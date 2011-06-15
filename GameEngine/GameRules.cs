using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public static class GameRules
    {
        static Random _random = new Random((int)DateTime.Now.Ticks);

        public static void CloseCombat(Character attacker, Character defender)
        {
            RollResult attacker_result = getCloseCombatResult(attacker, defender);
            RollResult defender_result = getCloseCombatResult(defender, attacker);

            attacker_result.Value += crowdModifier(attacker, defender);

            if (attacker_result.Lucky && !defender_result.Lucky)
            {
                Hit(attacker, defender);
            }
            else if (attacker_result.Fumble)
            {
                if (!defender_result.Fumble)
                    Hit(defender, attacker);
            }
            else if (attacker_result.Value > defender_result.Value)
            {
                Hit(attacker, defender);
            }
        }

        // One roll and lets see if it goes through..
        private static void Hit(Character hitter, Character target)
        {
            int baseModifier = 5 - hitter.CC_weapon.Strength - strengthBonus(hitter.BasicSkills.Strength);

            // Check armour
            int neededRoll = target.Armor.ArmorClass + baseModifier;
            RollResult roll = getRollResult();
            int result = roll.Value - neededRoll;

            if (!roll.Lucky && (roll.Fumble || result < 0))
                return;

           // Check wounding
           neededRoll = target.BasicSkills.Thoughness + baseModifier - woundBonus(result);
           roll = getRollResult();
           result = roll.Value - neededRoll;

           if (!roll.Lucky && ( roll.Fumble || result < 0))
               return;

           result = result == 0 ? 1 : result;

           // If lucky make double damage or at least 1
           int damage = roll.Lucky ? result * 2 : result;

           target.TakeDamage(damage);

           processDamage(hitter, target);
        }

        // If at same time in combat with many guys
        // -1 to +3
        private static int crowdModifier(Character attacker, Character defender)
        {
            int result = 0;

            // If someone is trying to hit you while you try to hit, get -1
            if (attacker.InCombatWith.Count > 1)
                result--;

            // If someone is also in combat with guy u are trying to hit, get max +3
            if (defender.InCombatWith.Count > 1)
            {
                result += defender.InCombatWith.Count >= 3 ? 3 : defender.InCombatWith.Count;
            }

            return result;
        }

        private static void processDamage(Character hitter, Character target)
        {
            if (target.Wounded || target.Dead)
            {
                if (target.Wounded)
                    hitter.Wounds.Add(target);   
                else if (target.Dead)
                    hitter.Kills.Add(target);

                hitter.Target = null;

                foreach(Character character in target.InCombatWith)
                {
                    character.InCombatWith.Remove(target);

                    if (character != hitter)
                    {
                        if (target.Wounded)
                            character.WoundsAssist.Add(target);
                        else if (target.Dead)
                            character.KillsAssist.Add(target);
                    }
                }

                target.InCombatWith.Clear();
            }
            
        }

        private static int woundBonus(int result)
        {
            return result > 3 ? (result - 3) : 0;
        }

        private static int strengthBonus(int strength)
        {
            return strength > 3 ? (strength - 3) : 0;
        }

        private static RollResult getCloseCombatResult(Character attacker, Character defender)
        {
            RollResult result  = getRollResult();

            result.Value += getCloseCombatModifier(attacker, defender) + attacker.BasicSkills.Agility;

            if (attacker.CC_weapon.Weight > attacker.BasicSkills.Strength)
                result.Value -= attacker.CC_weapon.Weight - attacker.BasicSkills.Strength;

            return result;
        }

        // TODO: Count again
        private static int getCloseCombatModifier(Character attacker, Character defender)
        {
            return attacker.BasicSkills.CloseCombat - defender.BasicSkills.CloseCombat;

        }

        private static RollResult getRollResult()
        {
            RollResult result = new RollResult();

            int roll = GetD10Roll();
            result.Value = roll;

            if (roll == 10 || roll == 1)
            {
                int newValue = _random.Next(1, 10);

                if (roll == newValue)
                {
                    if (roll == 10)
                        result.Lucky = true;
                    else
                        result.Lucky = false;
                }
            }

            return result;
        }

        public static int GetD10Roll()
        {
            return _random.Next(1, 10);
        }
    }

    public struct RollResult
    {
        public int Value;

        public bool Fumble;

        public bool Lucky;

    }
}
