using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public class CameraSystem : BaseSystem
    {
        private Size _gameArea;
        private GameObject _target;

        private float _zoom = 2;

        public CameraSystem(Size gameView, Size gameArea)
        {
            SysType = SystemType.CameraSystem;

            _gameArea = gameArea;

            _width = (int)gameView.Width;
            _height = (int)gameView.Height;

            // StartLocation is center
            Center = new Point(gameArea.Width / 2, gameArea.Height / 2);
            TargetLocation = new Point(gameArea.Width / 2, gameArea.Height / 2);

            // Count zoom so as much as possible is on view in the beginning
            Zoom = (_gameArea.Width / _width > _gameArea.Height / _height ? _gameArea.Height / _height : gameArea.Width / _width);
        }

        public float Zoom
        { 
            get { return _zoom; }
            set
            {
                // Zoom factor must be greater than 0
                if (value > 0)
                {
                    float oldValue = _zoom;
                    // Check new location so won't be out of bounds
                   _zoom = value;
                   if (!checkZoom(Center))
                       _zoom = oldValue;                
                }           
            }       
        }

        // Check that after zoom view is still in game area
        // If not, move it so it is
        private bool checkZoom(Point point)
        {
            if (Width > _gameArea.Width || Height > _gameArea.Height)
                return false;

            if (point.X - Width / 2 < 0)
                point.X = Width / 2;
            if (point.X + Width / 2 > _gameArea.Width)
                point.X = _gameArea.Width - Width / 2;
            if (point.Y - Height / 2 < 0)
                point.Y = Height / 2;
            if (point.Y + Height / 2 > _gameArea.Height)
                point.Y = _gameArea.Height - Height / 2;

           TargetLocation = point;
           return true;
        }

        // Follow this target
        public GameObject Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
                TargetLocation = value.Location;
            }
        }

        // Location where camera is moving
        public Point TargetLocation;      
        // Current center
        public Point Center;

        // Views center point (changes with zoom)
        public Point OwnCenter
        {
            get 
            {
                return new Point(_width / 2, _height / 2);
            }
        }

        // Original sizes
        private int _width;
        private int _height;

        // New sizes (zoom factor included)
        public int Width 
        { 
            get { return (int)(_width * Zoom); } 
        }

        public int Height 
        { 
            get { return (int)(_height * Zoom); } 
        }

        public Rectangle ScreenRectangle
        {
            get { return new Rectangle(UpLeft, new Size(Width, Height)); }
        }

        public Point UpLeft
        {
            get
            {
                return new Point(Center.X - Width / 2, Center.Y - Height / 2);
            }
        }

        public Point DownRight
        {
            get
            {
                return new Point(Center.X + Width / 2, Center.Y + Height / 2);
            }
        }

        // Get coordinates of x and in y view related to area
        public Point GetAreaCoordinates(int viewX, int viewY)
        {
            return new Point(UpLeft.X + (viewX * Zoom), UpLeft.Y + (viewY * Zoom));
        }

        // TODO: Fix ifs etc.
        // Move cameras current location closer to target location
        internal void MoveLocation(double elapsedTime, int gameSpeed)
        {
            Point velocity = MathHelper.Velocity(Center, TargetLocation);

            if (velocity.X == 0 && velocity.Y == 0)
                return;

            // Check that doesn't move out of game area
            double timeFactor = 1 * elapsedTime / (1000 / gameSpeed);

            if (!nextMoveInsideArea(velocity, timeFactor))
                return;

            if (MathHelper.Length(Center, TargetLocation) - timeFactor <= 0)
            {
                Center.X = TargetLocation.X;
                Center.Y = TargetLocation.Y;
            }
            else
            {
                // Move closer
                Center.X += velocity.X * (float)timeFactor;
                Center.Y += velocity.Y * (float)timeFactor;
            }
        }

        private bool nextMoveInsideArea(Point velocity, double timeFactor)
        {
            float x = Center.X + velocity.X * (float)timeFactor;
            float y = Center.Y + velocity.Y * (float)timeFactor;

            if (x - Width / 2 < 0)
                return false;
            if (x + Width / 2 > _gameArea.Width)
                return false;
            if (y - Height / 2 < 0)
                return false;
            if (y + Height / 2 > _gameArea.Height)
                return false;

            return true;
        }

        // Move camera's target location from current to current.x & current.y + y
        // Move always 10% of screen with one click
        internal void MoveTargetLocation(int x, int y)
        {
            SetTargetLocation((int)Center.X + (x*this.Width / 10), (int)Center.Y + (y * this.Height / 10));
        }

        // Set new location (x,y) as TargetLocation
        internal void SetTargetLocation(int x, int y)
        {     
            TargetLocation = new Point(x, y);
        }
    }
}
