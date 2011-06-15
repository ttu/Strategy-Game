using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public class MoveComponent : BaseComponent
    {
        // Location where moving to
        public Point MoveTo;

        public MoveComponent(Character owner): base(owner)
        {}

        public override void Update(long elapsedTime, int gamespeed)
        {
            MoveTurn(elapsedTime, gamespeed);
        }

        public void MoveTurn(long elapsedTime, int gameSpeed)
        {
            if (!owner.BlockMove) return;

            if (owner.InCombat || owner.MoveTo == null)
            {
                //UpdateView();
                return;
            }

            // Test if straight line of sight
            List<GameObject> objectsInTheWay = owner.QuadTreeProperty.Query(owner.Location, owner.MoveTo);

            if (objectsInTheWay.Count > 0)
            {
                // Find closest
                double minValue = double.MaxValue;
                GameObject closest = null;

                foreach (GameObject go in objectsInTheWay)
                {
                    if (go == owner || go == owner.Target)
                        continue;
                    // TODO: Size should also matter, not only center point
                    double value = MathHelper.Length(owner.Location, go.Location);
                    if (value < minValue)
                        closest = go;
                }

                // If closest is null, then found maybe own object
                if (closest != null)
                {

                    // Evade from x or from y
                    // Find closest corner
                    owner.MoveTo = getNewMoveTo(owner.Location, owner.MoveTo, closest);

                    if (owner.MoveTo == null)
                    {
                        //UpdateView();
                        return;
                    }
                }
            }

            Point velocity = MathHelper.Velocity(owner.Location, owner.MoveTo);

            // Move to velocity so distance is own radius + owner.Target radius 
            float distanceToStay = owner.Target == null ? 0 : owner.Radius + owner.Target.Radius;

            if (owner.Target != null && owner.Target.GetType() == typeof(Character) && MathHelper.Length(owner.Location, owner.Target.Location) - (owner.BasicSkills.Move * elapsedTime / (1000 / gameSpeed)) - distanceToStay - 2 <= 0)
            {
                // Move
                owner.Charge = true;

                owner.Location.X = owner.MoveTo.X - (velocity.X * distanceToStay);
                owner.Location.Y = owner.MoveTo.Y - (velocity.Y * distanceToStay);

                ((Character)owner.Target).InCombatWith.Add(owner);
                owner.InCombatWith.Add((Character)owner.Target);
            }
            else
            {
                Move(elapsedTime, gameSpeed, velocity);
            }

            // Update owner.QuadTree
            base.owner.LocationChanged();

            //UpdateView();
        }

        private void Move(double elapsedTime, int gameSpeed, Point velocity)
        {
            float x = owner.Location.X + velocity.X * owner.BasicSkills.Move * (float)elapsedTime / (1000 / gameSpeed);
            float y = owner.Location.Y + velocity.Y * owner.BasicSkills.Move * (float)elapsedTime / (1000 / gameSpeed);
            Point futurePoint = new Point(x, y);

            List<GameObject> collisionList = owner.QuadTreeProperty.Query(owner.RectangleFromPoint(futurePoint));

            // Whill always find own object so ignore it
            if (collisionList.Count <= 1)
            {
                // Move closer
                owner.Location.X = x;
                owner.Location.Y = y;

                // Reached owner.MoveTo, set it to null
                if (MathHelper.Length(owner.Location, owner.MoveTo) - (owner.BasicSkills.Move * elapsedTime / (1000 / gameSpeed)) - 1 < 0)
                    owner.MoveTo = null;
            }
            else
            {
                // Move somewhere else
                // Turn velocity 90 degrees
                Point newVelocity = new Point(velocity.X, velocity.Y);

                x = owner.Location.X + velocity.X * owner.BasicSkills.Move * (float)elapsedTime / (1000 / gameSpeed);
                y = owner.Location.Y + velocity.Y * owner.BasicSkills.Move * (float)elapsedTime / (1000 / gameSpeed);
                futurePoint = new Point(x, y);

                collisionList = owner.QuadTreeProperty.Query(owner.RectangleFromPoint(futurePoint));

                // Whill always find own object so ignore it
                if (collisionList.Count <= 1)
                {
                    // Move closer
                    owner.Location.X = x;
                    owner.Location.Y = y;
                    return;
                }

                newVelocity = new Point(velocity.X, velocity.Y * -1);

                x = owner.Location.X + velocity.X * owner.BasicSkills.Move * (float)elapsedTime / (1000 / gameSpeed);
                y = owner.Location.Y + velocity.Y * owner.BasicSkills.Move * (float)elapsedTime / (1000 / gameSpeed);
                futurePoint = new Point(x, y);

                collisionList = owner.QuadTreeProperty.Query(owner.RectangleFromPoint(futurePoint));

                // Whill always find own object so ignore it
                if (collisionList.Count <= 1)
                {
                    // Move closer
                    owner.Location.X = x;
                    owner.Location.Y = y;
                    return;
                }

                newVelocity = new Point(velocity.X * -1, velocity.Y);

                x = owner.Location.X + velocity.X * owner.BasicSkills.Move * (float)elapsedTime / (1000 / gameSpeed);
                y = owner.Location.Y + velocity.Y * owner.BasicSkills.Move * (float)elapsedTime / (1000 / gameSpeed);
                futurePoint = new Point(x, y);

                collisionList = owner.QuadTreeProperty.Query(owner.RectangleFromPoint(futurePoint));

                // Whill always find own object so ignore it
                if (collisionList.Count <= 1)
                {
                    // Move closer
                    owner.Location.X = x;
                    owner.Location.Y = y;
                    return;
                }

                newVelocity = new Point(velocity.X * -1, velocity.Y * -1);

                x = owner.Location.X + velocity.X * owner.BasicSkills.Move * (float)elapsedTime / (1000 / gameSpeed);
                y = owner.Location.Y + velocity.Y * owner.BasicSkills.Move * (float)elapsedTime / (1000 / gameSpeed);
                futurePoint = new Point(x, y);

                collisionList = owner.QuadTreeProperty.Query(owner.RectangleFromPoint(futurePoint));

                // Whill always find own object so ignore it
                if (collisionList.Count <= 1)
                {
                    // Move closer
                    owner.Location.X = x;
                    owner.Location.Y = y;
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

            objectsInTheWay = owner.QuadTreeProperty.Query(location, closest.Rectangle.UpLeft);
            if (objectsInTheWay.Count <= 1)
            {
                if (objectsInTheWay[0] != null && objectsInTheWay[0] == owner)
                {
                    dist = MathHelper.Length(movingTo, closest.Rectangle.UpLeft);
                    if (dist < minValue)
                    {
                        minValue = dist;
                        retVal = new Point(closest.Rectangle.UpLeft.X - owner.Radius * 4, closest.Rectangle.UpLeft.Y - owner.Radius * 4);
                    }
                }
            }

            objectsInTheWay = owner.QuadTreeProperty.Query(location, closest.Rectangle.UpRight);
            if (objectsInTheWay.Count <= 1)
            {
                if (objectsInTheWay[0] != null && objectsInTheWay[0] == owner)
                {
                    dist = MathHelper.Length(movingTo, closest.Rectangle.UpRight);
                    if (dist < minValue)
                    {
                        minValue = dist;
                        retVal = new Point(closest.Rectangle.UpRight.X + owner.Radius * 4, closest.Rectangle.UpRight.Y - owner.Radius * 4);
                    }
                }
            }

            objectsInTheWay = owner.QuadTreeProperty.Query(location, closest.Rectangle.DownLeft);
            if (objectsInTheWay.Count <= 1)
            {
                if (objectsInTheWay[0] != null && objectsInTheWay[0] == owner)
                {
                    dist = MathHelper.Length(movingTo, closest.Rectangle.DownLeft);
                    if (dist < minValue)
                    {
                        minValue = dist;
                        retVal = new Point(closest.Rectangle.DownLeft.X - owner.Radius * 4, closest.Rectangle.DownLeft.Y + owner.Radius * 4);
                    }
                }
            }

            objectsInTheWay = owner.QuadTreeProperty.Query(location, closest.Rectangle.DownRight);
            if (objectsInTheWay.Count <= 1)
            {
                if (objectsInTheWay[0] != null && objectsInTheWay[0] == owner)
                {
                    dist = MathHelper.Length(movingTo, closest.Rectangle.DownRight);
                    if (dist < minValue)
                    {
                        minValue = dist;
                        retVal = new Point(closest.Rectangle.DownRight.X + owner.Radius * 4, closest.Rectangle.DownRight.Y + owner.Radius * 4);
                    }
                }
            }

            return retVal;
        }


    
    }
}
