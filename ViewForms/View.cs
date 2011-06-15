using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using GameEngine;
using System.Collections;

namespace ViewForms
{
    public delegate void DrawView();

    public partial class View : Form, IView
    {
        DrawView _drawView;

        //public event EventHandler ViewClosing;
        private Font _font = new Font("Arial", 24);
        private Brush _brush = new SolidBrush(Color.Black);
        private Pen _pen;

        List<DrawObject> _objectsToDraw = new List<DrawObject>();

        Thread _thread;
        Game _game;
        RenderSystem _renderer;

        bool _painting;

        // These are for blocking Click event when dragging
        int clickX = 0;
        int clickY = 0;

        public View()
        {
            // Delegate for updating the view (Game is running on different thread)
            _drawView = new DrawView(this.drawView);

            _pen = new Pen(_brush, 2);

            _renderer = new RenderSystem(this);
           
            InitializeComponent();

            this.SetStyle(
           ControlStyles.UserPaint |
           ControlStyles.AllPaintingInWmPaint |
           ControlStyles.OptimizedDoubleBuffer, true);

            // Start game thread
            _game = new Game(this);
            ThreadStart start = new ThreadStart(_game.Start);
            _thread = new Thread(start);
            _thread.Start();
        }


        public bool Drawing
        {
            get { return _painting; }
        }

        public GameEngine.Size GameViewSize
        {
            get
            {
                //return new GameEngine.Size() { Width = 1200, Height = 800 };
                return new GameEngine.Size() { Width = ClientSize.Width, Height = ClientSize.Height }; 
            }
        }

        public void Draw(List<DrawObject> objectsToDraw)
        {
            _objectsToDraw = objectsToDraw;
            this.Invoke(_drawView);
        }

        private void drawView()
        {
            this.Invalidate();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _thread.Abort();
            base.OnClosing(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            _painting = true;

            // Draw camera lines
            //CameraSystem cam = _game.GetCamera();
            //e.Graphics.DrawLine(_pen, new PointF(cam.UpLeft.X, cam.UpLeft.Y), new PointF(cam.UpLeft.X + cam.Width, cam.UpLeft.Y));
            //e.Graphics.DrawLine(_pen, new PointF(cam.UpLeft.X, cam.UpLeft.Y + cam.Height), new PointF(cam.UpLeft.X + cam.Width, cam.UpLeft.Y + cam.Height));

            //e.Graphics.DrawLine(_pen, new PointF(cam.UpLeft.X, cam.UpLeft.Y), new PointF(cam.UpLeft.X, cam.UpLeft.Y + cam.Height));
            //e.Graphics.DrawLine(_pen, new PointF(cam.UpLeft.X + cam.Width, cam.UpLeft.Y), new PointF(cam.UpLeft.X + cam.Width, cam.UpLeft.Y + cam.Height));
            
            foreach (DrawObject drawObject in _objectsToDraw)
            {
                e.Graphics.DrawImage(drawObject.Image, drawObject.X - drawObject.Width / 2, drawObject.Y - drawObject.Height  / 2, drawObject.Width , drawObject.Height);
                //e.Graphics.DrawLine(_pen, new PointF(drawObject.X - drawObject.Width / 2, drawObject.Y - drawObject.Height / 2), new PointF(drawObject.X - drawObject.Width / 2 + drawObject.Width, drawObject.Y - drawObject.Height / 2));
                //e.Graphics.DrawLine(_pen, new PointF(drawObject.X - drawObject.Width / 2, drawObject.Y - drawObject.Height / 2 + drawObject.Height), new PointF(drawObject.X - drawObject.Width / 2 + drawObject.Width, drawObject.Y - drawObject.Height / 2 + drawObject.Height));

                //e.Graphics.DrawLine(_pen, new PointF(drawObject.X - drawObject.Width / 2, drawObject.Y - drawObject.Height / 2), new PointF(drawObject.X - drawObject.Width / 2, drawObject.Y - drawObject.Height / 2 + drawObject.Height));
                //e.Graphics.DrawLine(_pen, new PointF(drawObject.X - drawObject.Width / 2 + drawObject.Width, drawObject.Y - drawObject.Height / 2), new PointF(drawObject.X - drawObject.Width / 2 + drawObject.Width, drawObject.Y - drawObject.Height / 2 + drawObject.Height));
            }

            _painting = false;
        }

        

        protected override void OnClick(EventArgs e)
        {
            int x = ((MouseEventArgs)e).X;
            int y = ((MouseEventArgs)e).Y;

            // Prevent click when dragging
            if (x != clickX && y != clickY)
                return;

            _game.ClickAction(x, y);

           base.OnClick(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            _game.MouseMoveAction(e.X, e.Y);
            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            clickX = e.X;
            clickY = e.Y;

            _game.MouseDownAction(e.X, e.Y);
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            clickX = 0;
            clickY = 0;

            _game.MouseUpAction(e.X, e.Y);
            base.OnMouseUp(e);
        }

        private void View_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'w')
                _game.MoveCameraLocationAction(0, -1);
            else if (e.KeyChar == 's')
                _game.MoveCameraLocationAction(0, 1);
            else if (e.KeyChar == 'a')
                _game.MoveCameraLocationAction(-1, 0);
            else if (e.KeyChar == 'd')
                _game.MoveCameraLocationAction(1, 0);
            else if (e.KeyChar == 'n')
                _game.EndTurnAction();
            else if (e.KeyChar == 'p')
                _game.PauseAction();
            else if (e.KeyChar == 'e')
                _game.SelectNextAction();
            else if (e.KeyChar == 'q')
                _game.SelectPreviousAction();
            else if (e.KeyChar == 'f')
                _game.ZoomOutAction();
            else if (e.KeyChar == 'r')
                _game.ZoomInAction();
        }
    }
}
