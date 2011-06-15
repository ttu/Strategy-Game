using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    // OBOSLETE: Frame is filled from QuadTree
    // Responsible for creating Frame for UI
    // Here could store frames for drawing
    public class RenderSystem
    {
        IView _view;

        Queue<List<DrawObject>> _objectQueue = new Queue<List<DrawObject>>();

        List<DrawObject> _filling = new List<DrawObject>();

        List<DrawObject> _extraObjects = new List<DrawObject>();

        public RenderSystem(IView view)
        {
            _view = view;
        }

        public void AddExtraObjectToDraw(DrawObject drawObject)
        {
            _extraObjects.Add(drawObject);
        }

        public void AddNewDrawableObject(DrawObject drawObject)
        {
            _filling.Add(drawObject);
        }


        public void SwapQueue()
        {
            // Add ready list to queueu
            _objectQueue.Enqueue(_filling);

            _filling = new List<DrawObject>();

            // Check that is not drawing
            while (_view.Drawing)
                System.Threading.Thread.Sleep(5);

            List<DrawObject> nextList = _objectQueue.Dequeue();

            _view.Draw(nextList);
           
        }

        internal void FrameReady()
        {
            //List<GameObject> objects = ObjectManager.Query(CameraSystem.ScreenRectangle);

            //List<DrawObject> drawList = new List<DrawObject>();
            //drawList.Add(_gameArea.ConvertToDrawObject(CameraSystem));

            //foreach (GameObject c in objects)
            //    drawList.Add(c.ConvertToDrawObject());

            //if (draggingObject != null)
            //    drawList.Add(draggingObject);

            //_view.Draw(drawList);
        }
    }
}
