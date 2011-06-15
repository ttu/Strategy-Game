using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GameEngine
{
    /// <summary>
    /// Simple drawinfo for view
    /// </summary>
    public class DrawObject
    {
        public Image Image;
        public float X;
        public float Y;

        public float Width;
        public float Height;

        public DrawObject(Image image, float x, float y, float zoom)
        {
            Image = image;
            Width = Image.Width / zoom;
            Height = Image.Height / zoom;

            X = x;
            Y = y;
        }
    }
}
