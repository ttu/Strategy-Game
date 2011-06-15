using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public class Character : GameObject
    {
        #region FIELDS

        public int ID;

        public Behavior Behavior;

        // Enemy
        public GameObject Target;

        // No shooting yet so if target is set by user then follow targets location
        public bool TargetSetByUser { get; set; }

        // Location where moving to
        public Point MoveTo;

       

        public bool Charge;

        public List<Character> InCombatWith = new List<Character>();

        // Empty bands, happy ranger with no friends or no enemies
        public CharacterGroup Own = new CharacterGroup();
        public CharacterGroup Enemies = new CharacterGroup();

        public BasicSkills BasicSkills;

        public Weapon CC_weapon;
        public Weapon SS_weapon;

        public Armor Armor;

        // Wounded is out of game, but not dead
        public bool Wounded;
        // Dead is dead
        public bool Dead;

        #region STATISTICS

        public List<Character> Wounds = new List<Character>();
        public List<Character> Kills = new List<Character>();

        public List<Character> WoundsAssist = new List<Character>();
        public List<Character> KillsAssist = new List<Character>();

        #endregion

        #endregion

        #region PROPERTIES

        public bool InCombat
        {
            get { if (InCombatWith.Count > 0) return true; return false; }
        }

        #endregion

        public Character()
        {
            Components.Add(new BehaviorComponent(this));
            Components.Add(new MoveComponent(this));
            Components.Add(new AttackComponent(this));
        }

        public Character(BehaviorType behavior)
        {
            Components.Add(new BehaviorComponent(this, behavior));
            Components.Add(new MoveComponent(this));
            Components.Add(new AttackComponent(this));
        }

        // Add to band and set other as enemy
        public void Initialize(CharacterGroup own, CharacterGroup enemy)
        {
            Own = own;
            Own.Add(this);

            Enemies = enemy;
        }

        public override void Update(long elapsedTime, int gamespeed)
        {
            // TODO: These could be components
            // So change from is-a to has-a
            foreach (BaseComponent component in Components)
                component.Update(elapsedTime, gamespeed);

            //UpdateBehavior();
            //((MoveComponent)Components[0]).Update(elapsedTime, gamespeed);
            //MoveTurn(elapsedTime, gamespeed);
            //FightTurn();

            base.Update(elapsedTime, gamespeed);
        }

       

        internal void TakeDamage(int damage)
        {
            BasicSkills.Wounds -= damage;

            if (BasicSkills.Wounds == 0)
            {
                BlockMove = false;
                Wounded = true;
                SetImage("Test.png");
                QuadTree.Remove(this);
            }
            else if (BasicSkills.Wounds < 0)
            {
                BlockMove = false;
                Dead = true;
                SetImage("Test.png");
                QuadTree.Remove(this);
            }
        }

        /*
        
        public void SetBehavior(BehaviorType behavior)
        {
            switch (behavior)
            {
                case BehaviorType.FastAttack:
                    this.Behavior = new FastAttack(this);
                    break;
                case BehaviorType.SmartAttack:
                    this.Behavior = new SmartAttack(this);
                    break;
                case BehaviorType.StandAndDefend:
                    this.Behavior = new StandAndDefend(this);
                    break;
                case BehaviorType.EvadeAndShoot:
                    this.Behavior = new EvadeAndShoot(this);
                    break;
                case BehaviorType.Flee:
                    this.Behavior = new Flee(this);
                    break;
                case BehaviorType.NoAI:
                    this.Behavior = new NoAI(this);
                    break;
                default:
                    this.Behavior = new StandAndDefend(this);
                    break;
            }
        }
         * 
        public void UpdateBehavior()
        {
            // Check if need to change behaviour

            // Update target
            Behavior.Update();
        }

        
        public void MoveTurn(double elapsedTime, int gameSpeed)
        {
            if (InCombat || MoveTo == null)
            {
                //UpdateView();
                return;
            }

            // Test if straight line of sight
            List<GameObject> objectsInTheWay = QuadTree.Query(Location, MoveTo);

            if (objectsInTheWay.Count > 0)
            {
                // Find closest
                double minValue = double.MaxValue;
                GameObject closest = null;

                foreach (GameObject go in objectsInTheWay)
                {
                    if (go == this || go == Target)
                        continue;
                    // TODO: Size should also matter, not only center point
                    double value = MathHelper.Length(Location, go.Location);
                    if (value < minValue)
                        closest = go;
                }

                // If closest is null, then found maybe own object
                if (closest != null)
                {

                    // Evade from x or from y
                    // Find closest corner
                    MoveTo = getNewMoveTo(Location, MoveTo, closest);

                    if (MoveTo == null)
                    {
                        //UpdateView();
                        return;
                    }
                }
            }

            Point velocity = MathHelper.Velocity(Location, MoveTo);

            // Move to velocity so distance is own radius + target radius 
            float distanceToStay = Target == null ? 0 : Radius + Target.Radius;

            if (Target != null && Target.GetType() == typeof(Character) && MathHelper.Length(Location, Target.Location) - (BasicSkills.Move * elapsedTime / (1000 / gameSpeed)) - distanceToStay - 2 <= 0)
            {
                // Move
                Charge = true;

                Location.X = MoveTo.X - (velocity.X * distanceToStay);
                Location.Y = MoveTo.Y - (velocity.Y * distanceToStay);

                ((Character)Target).InCombatWith.Add(this);
                this.InCombatWith.Add((Character)Target);
            }
            else
            {
                move(elapsedTime, gameSpeed, velocity);
            }

            // Update QuadTree
            base.LocationChanged();

            //UpdateView();
        }

        private void move(double elapsedTime, int gameSpeed, Point velocity)
        {
            float x = Location.X + velocity.X * BasicSkills.Move * (float)elapsedTime / (1000 / gameSpeed);
            float y = Location.Y + velocity.Y * BasicSkills.Move * (float)elapsedTime / (1000 / gameSpeed);
            Point futurePoint = new Point(x, y);

            List<GameObject> collisionList = QuadTree.Query(RectangleFromPoint(futurePoint));

            // Whill always find own object so ignore it
            if (collisionList.Count <= 1)
            {
                // Move closer
                Location.X = x;
                Location.Y = y;

                // Reached MoveTo, set it to null
                if (MathHelper.Length(Location, MoveTo) - (BasicSkills.Move * elapsedTime / (1000 / gameSpeed)) - 1 < 0)
                    MoveTo = null;
            }
            else
            {
                // Move somewhere else
                // Turn velocity 90 degrees
                Point newVelocity = new Point(velocity.X, velocity.Y);

                 x = Location.X + velocity.X * BasicSkills.Move * (float)elapsedTime / (1000 / gameSpeed);
                 y = Location.Y + velocity.Y * BasicSkills.Move * (float)elapsedTime / (1000 / gameSpeed);
                 futurePoint = new Point(x, y);

               collisionList = QuadTree.Query(RectangleFromPoint(futurePoint));

                // Whill always find own object so ignore it
                if (collisionList.Count <= 1)
                {
                    // Move closer
                    Location.X = x;
                    Location.Y = y;
                    return;
                }

                newVelocity = new Point(velocity.X, velocity.Y*-1);

                x = Location.X + velocity.X * BasicSkills.Move * (float)elapsedTime / (1000 / gameSpeed);
                y = Location.Y + velocity.Y * BasicSkills.Move * (float)elapsedTime / (1000 / gameSpeed);
                futurePoint = new Point(x, y);

                collisionList = QuadTree.Query(RectangleFromPoint(futurePoint));

                // Whill always find own object so ignore it
                if (collisionList.Count <= 1)
                {
                    // Move closer
                    Location.X = x;
                    Location.Y = y;
                    return;
                }

                newVelocity = new Point(velocity.X * -1, velocity.Y);

                x = Location.X + velocity.X * BasicSkills.Move * (float)elapsedTime / (1000 / gameSpeed);
                y = Location.Y + velocity.Y * BasicSkills.Move * (float)elapsedTime / (1000 / gameSpeed);
                futurePoint = new Point(x, y);

                collisionList = QuadTree.Query(RectangleFromPoint(futurePoint));

                // Whill always find own object so ignore it
                if (collisionList.Count <= 1)
                {
                    // Move closer
                    Location.X = x;
                    Location.Y = y;
                    return;
                }

                newVelocity = new Point(velocity.X * -1, velocity.Y * -1);

                x = Location.X + velocity.X * BasicSkills.Move * (float)elapsedTime / (1000 / gameSpeed);
                y = Location.Y + velocity.Y * BasicSkills.Move * (float)elapsedTime / (1000 / gameSpeed);
                futurePoint = new Point(x, y);

                collisionList = QuadTree.Query(RectangleFromPoint(futurePoint));

                // Whill always find own object so ignore it
                if (collisionList.Count <= 1)
                {
                    // Move closer
                    Location.X = x;
                    Location.Y = y;
                    return;
                }
            }
        }

        private Point getNewMoveTo(Point location, Point movingTo, GameObject closest)
        {
            Point retVal = null;

            double minValue = double.MaxValue;
            double dist;
            List<GameObject> objectsInTheWay = new List<GameObject>();

            objectsInTheWay = QuadTree.Query(location, closest.Rectangle.UpLeft);
            if (objectsInTheWay.Count <= 1)
            {
                if (objectsInTheWay[0] != null && objectsInTheWay[0] == this)
                {
                    dist = MathHelper.Length(movingTo, closest.Rectangle.UpLeft);
                    if (dist < minValue)
                    {
                        minValue = dist;
                        retVal = new Point(closest.Rectangle.UpLeft.X - Radius * 4, closest.Rectangle.UpLeft.Y - Radius * 4);
                    }
                }
            }

            objectsInTheWay = QuadTree.Query(location, closest.Rectangle.UpRight);
           if (objectsInTheWay.Count <= 1)
            {
                if (objectsInTheWay[0] != null && objectsInTheWay[0] == this)
                {
                    dist = MathHelper.Length(movingTo, closest.Rectangle.UpRight);
                    if (dist < minValue)
                    {
                        minValue = dist;
                        retVal = new Point(closest.Rectangle.UpRight.X + Radius * 4, closest.Rectangle.UpRight.Y - Radius * 4);
                    }
                }
            }

            objectsInTheWay = QuadTree.Query(location, closest.Rectangle.DownLeft);
            if (objectsInTheWay.Count <= 1)
            {
                if (objectsInTheWay[0] != null && objectsInTheWay[0] == this)
                {
                    dist = MathHelper.Length(movingTo, closest.Rectangle.DownLeft);
                    if (dist < minValue)
                    {
                        minValue = dist;
                        retVal = new Point(closest.Rectangle.DownLeft.X - Radius * 4, closest.Rectangle.DownLeft.Y + Radius * 4);
                    }
                }
            }

            objectsInTheWay = QuadTree.Query(location, closest.Rectangle.DownRight);
          if (objectsInTheWay.Count <= 1)
            {
                if (objectsInTheWay[0] != null && objectsInTheWay[0] == this)
                {
                    dist = MathHelper.Length(movingTo, closest.Rectangle.DownRight);
                    if (dist < minValue)
                    {
                        minValue = dist;
                        retVal = new Point(closest.Rectangle.DownRight.X + Radius * 4, closest.Rectangle.DownRight.Y + Radius * 4);
                    }
                }
            }

            return retVal;
        }

        public void FightTurn()
        {
            if (InCombat)
            {
                if (Target == null && InCombatWith.Count > 0)
                    Target = InCombatWith[0];

                Character target = Target as Character;
                GameRules.CloseCombat(this, target);
            }
        }
        */

    }
}
