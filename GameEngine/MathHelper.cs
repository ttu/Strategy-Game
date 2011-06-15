using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
	/*
	 * Point is class so reference is passed and camera can follow Point
	 */
    /// <summary>
    /// Point on plane.
    /// </summary>
    public class Point
    {
        public Point(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public float X;
        public float Y;
    }

    /// <summary>
    /// Represents size of object.
    /// </summary>
    public struct Size
    {
        public Size(float width, float height)
        {
            this.Width = width;
            this.Height = height;
        }

        public float Width;
        public float Height;
    }

    /// <summary>
    /// Rectangle. Location is upper left corner. Size is width and height
    /// </summary>
    public struct Rectangle
    {
        public Point Location;
        public Size Size;

        /// <summary>
        ///  Location is upper left corner. Size is width and height
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Rectangle(Point location, Size size)
        {
            Location = location;
            Size = size;
        }

        /// <summary>
        ///  Location is upper left corner. Size is width and height
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Rectangle(float x, float y, float width, float height)
        {
            Location = new Point(x, y);
            Size = new Size(width, height);
        }

        public float Width
        {
            get { return Size.Width; }
        }

        public float Height
        {
            get { return Size.Height; }
        }

        public Point UpLeft
        { get { return Location; } }

        public Point UpRight
        { get { return new Point(Location.X + Size.Width, Location.Y); } }

        public Point DownLeft
        { get { return new Point(Location.X, Location.Y + Size.Height); } }

        public Point DownRight
        { get { return new Point(Location.X + Size.Width, Location.Y + Size.Height); } }
    }

	/*
	 * Helper for counting directions and distances
	 */
    public static class MathHelper
    {
        public static Rectangle RectanglefromPoints(Point start, Point end)
        {
            // Determine up left
            float x = getUpLeftX(start, end);
            float y = getUpLeftY(start, end);

            float width = Math.Abs(start.X - end.X);
            float height = Math.Abs(start.Y - end.Y);

            return new Rectangle(x, y, width, height);
        }

        private static float getUpLeftY(Point start, Point end)
        {
            // start is more up or same
            if (start.Y <= end.Y)
            {
                return start.Y;
            }
            // end is more up
            else
            {
                return end.Y;
            }
        }

        private static float getUpLeftX(Point start, Point end)
        {
            // start is more left or same
            if (start.X <= end.X)
            {
                return start.X;
            }
            // end is more left
            else
            {
                return end.X;
            }
        }

        public static double Length(Point val1, Point val2)
        {
            double x = Math.Abs(val1.X - val2.X);
            double y = Math.Abs(val1.Y - val2.Y);

            double retVal = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

            return retVal;
        }

        public static double Length(float x, float y)
        {
            double retVal = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

            return retVal;
        }

        public static Point Velocity(Point from, Point to)
        {
            float length = (float)Length(from, to);

            if (length == 0)
                return new Point(0, 0);

            Point velocity = new Point(0,0);

            velocity.X = (to.X - from.X) / length;

            velocity.Y = (to.Y - from.Y) / length;

            return velocity;
        }

        public static bool IntersectsWith(this Rectangle rec1, Rectangle rec2)
        {
            // Totally inside
            if (rec2.Contains(rec1))
                return true;

            // Any of the corners inside
            if (rec1.Contains(rec2.UpLeft))
                return true;
            if (rec1.Contains(rec2.UpRight))
                return true;
            if (rec1.Contains(rec2.DownLeft))
                return true;
            if (rec1.Contains(rec2.DownRight))
                return true;


            // Crosses vertically
            if (rec1.UpLeft.X <= rec2.UpRight.X && rec1.UpRight.X >= rec2.UpRight.X ||
                rec1.UpLeft.X <= rec2.UpLeft.X && rec1.UpRight.X >= rec2.UpLeft.X)
            {
                if (rec1.UpLeft.Y >= rec2.UpLeft.Y && rec1.DownLeft.Y <= rec2.DownLeft.Y)
                    return true;
            }

            // Crosses horizontally
            if (rec1.UpLeft.Y <= rec2.DownLeft.Y && rec1.DownLeft.Y >= rec2.DownLeft.Y ||
                rec1.UpLeft.Y <= rec2.UpLeft.Y && rec1.DownLeft.Y >= rec2.UpLeft.Y)
            {
                if (rec1.UpLeft.X >= rec2.UpLeft.X && rec1.UpRight.X <= rec2.UpRight.X)
                    return true;
            }

            return false;
        }

        public static bool Contains(this Rectangle rec1, Rectangle rec2)
        {
            if (rec2.Location.X >= rec1.Location.X && rec2.Location.Y >= rec1.Location.Y &&
                rec2.DownRight.X <= rec1.DownRight.X && rec2.DownRight.Y <= rec1.DownRight.Y)
                return true;

            return false;
        }

        public static bool Contains(this Rectangle rectangle, Point target)
        {
            return Contains(rectangle.Location, new Point(rectangle.Location.X + rectangle.Size.Width,rectangle.Location.Y + rectangle.Size.Height), target); 
        }

        public static bool Contains(Point upLeft, Point downRight, Point target)
        {
            if (target.X >= upLeft.X && target.X <= downRight.X && target.Y >= upLeft.Y && target.Y <= downRight.Y)
                return true;

            return false;
        }
    }
}
