using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public enum BehaviorType
    {
        FastAttack,
        SmartAttack,
        StandAndDefend,
        EvadeAndShoot,
        Flee,
        NoAI
    }

    public abstract class Behavior
    {
        protected Character character;

        public Behavior(Character character)
        {
            this.character = character;
        }

        public virtual void Update()
        {
            character.Target = null;
            character.MoveTo = null;
        }
    }

    public class NoAI : Behavior
    {
        public NoAI(Character character)
            : base(character)
        { }

        public override void Update()
        {
            if (character.Target != null)
            {
                if (character.TargetSetByUser)
                    character.MoveTo = character.Target.Location;
            }
        }
    }

    // Choose closet enemy
    public class FastAttack : Behavior
    { 
        public FastAttack(Character character):base(character)
        { }

        public override void Update()
        {
            base.Update();

            double maxValue = double.MaxValue;

            foreach (Character enemy in character.Enemies)
            {
                if (enemy.Dead || enemy.Wounded)
                    continue;

                double value = MathHelper.Length(base.character.Location, enemy.Location);

                if (value < maxValue)
                {
                    maxValue = value;
                    base.character.Target = enemy;
                    base.character.MoveTo = enemy.Location;
                }
            }
        }
    }


    public class SmartAttack : Behavior
    { 
        public SmartAttack(Character character):base(character)
        {}

        public override void Update()
        {
            base.Update();

            double maxValue = double.MaxValue;

            foreach (Character enemy in character.Enemies)
            {
                if (enemy.Dead || enemy.Wounded)
                    continue;

                double value = MathHelper.Length(character.Location, enemy.Location);

                // TODO: Fix calculation
                value += Math.Abs(enemy.BasicSkills.FearValue - character.BasicSkills.Will);

                if (value < maxValue)
                {
                    maxValue = value;
                    character.Target = enemy;
                    base.character.MoveTo = enemy.Location;
                }
            }
        }
    }

    public class StandAndDefend : Behavior
    {
        public StandAndDefend(Character character):base(character)
        {}

        public override void Update()
        {
            base.Update();

            double maxValue = double.MaxValue;

            foreach (Character enemy in character.Enemies)
            {
                if (enemy.Dead || enemy.Wounded)
                    continue;

                double value = MathHelper.Length(base.character.Location, enemy.Location);

                if (value < maxValue)
                {
                    maxValue = value;
                    base.character.Target = enemy;
                    // No moving, just stand
                    //base.character.MoveTo = enemy.Location;
                }
            }

        }
    }

    public class EvadeAndShoot : Behavior
    { 
     public EvadeAndShoot(Character character):base(character)
        {}
    }

    // Evade closest enemy
    public class Flee : Behavior
    {
        public Flee(Character character)
            : base(character)
        {}

        public override void Update()
        {
            base.Update();

            int maxValue = int.MaxValue;

            foreach (Character enemy in character.Enemies)
            {
                if (enemy.Dead || enemy.Wounded)
                    continue;

                int value = (int)MathHelper.Length(base.character.Location, enemy.Location);

                if (value < maxValue)
                {
                    maxValue = value;
                    base.character.Target = enemy;
                    // TODO: Try to get as far as possible
                    base.character.MoveTo = new Point(0, 0);
                }
            }
        }
    }
}
