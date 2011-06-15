using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GameEngine
{
    public class GameArea
    {
        public Size GameAreaSize;

        Bitmap _background;
		int _screenWidth;
		int _screenHeight;

        DrawObject _obj;
        float _x = 0;
        float _y = 0;
        float _width = 0;
        float _height = 0;

        public GameArea(int screenWidth, int screenHeight, int decreaseX, int decreaseY)
        {
			_screenWidth = screenWidth;
			_screenHeight = screenHeight;
            _background = new Bitmap(System.IO.Path.Combine("Images", "GameArea.png"));
            GameAreaSize = new Size(_background.Width - decreaseX, _background.Height - decreaseY);
        }

        public DrawObject ConvertToDrawObject(Rectangle screenRec, float centerX, float centerY)
        {
            // Always draw "full" image, so don't care about zoom
            // Check camera has moved, else use old image
            if (_width != screenRec.Width || _height != screenRec.Height || _x != screenRec.Location.X || _y != screenRec.Location.Y)
            {
                _obj = new DrawObject(CurrentView(screenRec), centerX, centerY, 1);
                _width = screenRec.Width;
                _height = screenRec.Height;
                _x = screenRec.Location.X;
                _y = screenRec.Location.Y;
            }

            return _obj;
        }

        // Crop correct piece from GameArea
        // TODO: Make this faster
        public Bitmap CurrentView(Rectangle viewRectangle)
        {
            RectangleF recF = new RectangleF(viewRectangle.Location.X, viewRectangle.Location.Y, viewRectangle.Size.Width, viewRectangle.Size.Height);

            //Bitmap CroppedBitmap = new Bitmap((int)recF.Width, (int)recF.Height);
            //Graphics G = Graphics.FromImage(CroppedBitmap);
            //G.DrawImage(_background, 0, 0, recF, GraphicsUnit.Pixel);

			Bitmap cropped = _background.Clone(recF, System.Drawing.Imaging.PixelFormat.DontCare);		
			Bitmap area = new Bitmap(cropped, _screenWidth,_screenHeight);

            return area;
        }
    }
}
