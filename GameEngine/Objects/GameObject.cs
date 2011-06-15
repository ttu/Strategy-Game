using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GameEngine
{
    /// <summary>
    /// General Game Object
    /// </summary>
    public class GameObject: IQuadObject
    {
        #region REGIONS
        #region FIELDS
        #endregion
        #region PROPERTIES
        #endregion
        #region CONSTRUCTORS
        #endregion
        #region PRIVATE METHODS
        #endregion
        #region PUBLIC METHODS
        #endregion
        #endregion

        #region IQuadObject Members

        public Rectangle Bounds
        {
            get { return Rectangle; }
        }

        public event EventHandler BoundsChanged;

        #endregion

        #region FIELDS

        // TODO
        /* Order what should be drawn first
         * 1 = Moving layer
         * 2 = Dead, Injured (not bloacking objects)
         * 3 = Players, Trees, Houses (blocking objects)
         * 4 = Choose character move location, Clouds, birds, etc (not blaocking objects)
         */
        public int DrawOrder;

        // Makes just life easier when everyone knows QuadTree
        public static QuadTree<GameObject> QuadTree;

        public QuadTree<GameObject> QuadTreeProperty { get { return QuadTree; } }

        // Define what object can do
        // Needs refactoring
        public List<BaseComponent> Components = new List<BaseComponent>();

        // Location on Game Area
        public Point Location;
        // Location on Camera
        public Point LocationOnCamera;

        public Size Size;

        // Will object block movement, for now always
        public bool BlockMove = true;

        // List of all frames
        protected List<Bitmap> _frames = new List<Bitmap>();

        #endregion

        #region PROPERTIES

        public Rectangle Rectangle
        {
            get { return new Rectangle(UpLeft, Size); } 
        }

        public float Radius
        {
            get { return Size.Width / 2; }
        }

        public Point UpLeft
        {
            get { return new Point(Location.X - Size.Width / 2, Location.Y - Size.Height / 2);  }
        }

        // For now only 1 frame in use
		public Bitmap CurrentFrame
		{
			get{ return _frames[0]; }
        }

        #endregion

        public GameObject()
        {
            SetImage("Test.png");   
        }

        public void SetImage(string imageName)
        {
            _frames.Clear();
            _frames.Add(new Bitmap(System.IO.Path.Combine("Images", imageName)));
            Size.Width = _frames[0].Width;
            Size.Height = _frames[0].Height;
        }

        public void UpdateLocationOnCamera(float upLeftX, float upLeftY, float zoom)
        {
            // Even when zooming there is no more points on the view, images just get smaller, so have to divide with zoom
            LocationOnCamera = new Point((Location.X - upLeftX) / zoom, (Location.Y - upLeftY) / zoom);
        }

        public DrawObject ConvertToDrawObject(float upLeftX, float upLeftY, float zoom)
        {
            UpdateLocationOnCamera(upLeftX, upLeftY, zoom);

            Image cloneImage = null;

            lock (_frames[0])
            {
                cloneImage = _frames[0].Clone() as Image;
            }

            return new DrawObject(cloneImage, LocationOnCamera.X, LocationOnCamera.Y, zoom);
        }
    
        public void LocationChanged()
        {
            if (BoundsChanged != null)
                BoundsChanged(this, null);
        }

        public Rectangle RectangleFromPoint(Point point)
        {
            // 1.888f = 1, so add + 0.99999f to make everything over 0.1 to round up
            //return new Rectangle(new Point((int)(point.X - Size.Width / 2 + 0.999999f), (int)(point.Y - Size.Height / 2 + 0.999999f)), Size);
            return new Rectangle(new Point((point.X - Size.Width / 2f), (point.Y - Size.Height / 2f)), new Size(Size.Width,Size.Height));
        }
    }

   
}
