using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public class QuadTree<T> where T : class, IQuadObject
    {
        Size _minLeafSize;
        int _maxChildObjects = 1;

        // Bounds changed event requires this, or we would need to check evert node to find object
        private Dictionary<T, QuadTreeNode<T>> objectToNodeLookup = new Dictionary<T, QuadTreeNode<T>>();

        private object syncLock = new object();

        QuadTreeNode<T> _root;

        public QuadTree(Size minLeafSize, Rectangle rect)
        {
            _minLeafSize = minLeafSize;

            _root = new QuadTreeNode<T>(rect);
        }

        public void Insert(T newObject)
        {
            insert(_root, newObject);
        }

        public void Remove(T oldObject)
        {
            QuadTreeNode<T> node = objectToNodeLookup[oldObject];
            removeNodeObject(node, oldObject);
        }

        private void insert(QuadTreeNode<T> node, T newObject)
        {
            lock (syncLock)
            {
                if (!node.Bounds.Contains(newObject.Bounds))
                    throw new Exception("This should not happen, child does not fit within node bounds");

                // Check creation of new nodes
                if (!node.HasChildNodes() && node.Objects.Count >= _maxChildObjects)
                {
                    InitChildNodes(node);

                    List<T> childObjects = new List<T>(node.Objects);
                    List<T> childsToRelocate = new List<T>(node.Objects.Count);

                    // Loop all childs and relocate those to new nodes
                    foreach (T obj in childObjects)
                    {
                        foreach (QuadTreeNode<T> childNode in node.Nodes)
                        {
                            // This should never happen, cos we just made new nodes..
                            if (childNode == null) continue;

                            // Check if old child object fits to new nodes
                            if (childNode.Bounds.Contains(obj.Bounds))
                            {
                                childsToRelocate.Add(obj);
                                break;
                            }
                        }
                    }

                    foreach (T obj in childsToRelocate)
                    {
                        removeNodeObject(node, obj);
                        insert(node, obj);
                    }
                }

                // Check if will be inserted to child nodes
                if (node.HasChildNodes())
                {
                    foreach (QuadTreeNode<T> childNode in node.Nodes)
                    {
                        if (childNode != null)
                        {
                            if (childNode.Bounds.Contains(newObject.Bounds))
                            {
                                insert(childNode, newObject);
                                return;
                            }
                        }
                    }
                }

                // Insert to this current node
                insertNodeObject(node, newObject);
            }
        }

        private void removeNodeObject(QuadTreeNode<T> node, T obj)
        {
            objectToNodeLookup.Remove(obj);
            node.Objects.Remove(obj);
            obj.BoundsChanged -= new EventHandler(newNode_BoundsChanged);
        }

        private void insertNodeObject(QuadTreeNode<T> node, T newObject)
        {
            objectToNodeLookup.Add(newObject, node);
            node.Objects.Add(newObject);
            newObject.BoundsChanged += new EventHandler(newNode_BoundsChanged);
        }

        void newNode_BoundsChanged(object sender, EventArgs e)
        {
            lock (syncLock)
            {
                T obj = sender as T;

                QuadTreeNode<T> node = objectToNodeLookup[obj];
                // Check if moved out of node
                if (!node.Bounds.Contains(obj.Bounds))
                {
                    // Look for new home
                    removeNodeObject(node, obj);
                    Insert(obj);
                }
            }
        }

        public void InitChildNodes(QuadTreeNode<T> node)
        {
            // TODO: Should minimun size be used? or always just half half half half forever (as long as can fit objects)

            float halfWidth = (node.Bounds.Width / 2f);
            float halfHeight = (node.Bounds.Height / 2f);

            node.Nodes[0] = new QuadTreeNode<T>(new Rectangle(node.Bounds.Location, new Size(halfWidth, halfHeight)));
            node.Nodes[1] = new QuadTreeNode<T>(new Rectangle(new Point(node.Bounds.Location.X, node.Bounds.Location.Y + halfHeight), new Size(halfWidth, halfHeight)));
            node.Nodes[2] = new QuadTreeNode<T>(new Rectangle(new Point(node.Bounds.Location.X + halfWidth, node.Bounds.Location.Y), new Size(halfWidth, halfHeight)));
            node.Nodes[3] = new QuadTreeNode<T>(new Rectangle(new Point(node.Bounds.Location.X + halfWidth, node.Bounds.Location.Y + halfHeight), new Size(halfWidth, halfHeight)));
        }

        public List<T> Query(Rectangle bounds)
        {
            lock (syncLock)
            {
                List<T> results = new List<T>();

                if (_root != null)
                    Query(bounds, _root, results);

                return results;
            }
        }

        public List<T> Query(Point start, Point end)
        {
            lock (syncLock)
            {
                List<T> results = new List<T>();
                List<T> rayResults = new List<T>();

                Rectangle rect = MathHelper.RectanglefromPoints(start, end);

                if (_root != null)
                    Query(rect, _root, results);

                processResults(start, end, results, rayResults);

                return rayResults;
            }
        }

        private void processResults(Point start, Point end, List<T> results, List<T> rayResults)
        {
            Point vel = MathHelper.Velocity(start, end);

            int len = (int)MathHelper.Length(start, end);

            List<Point> points = new List<Point>();

            for (int i = 1; i <= len; i++)
            {
                Point newPoint = new Point(start.X + i * vel.X, start.Y + i * vel.Y);
                points.Add(newPoint);
            }

            foreach (T obj in results)
            {
                foreach (Point point in points)
                {
                    if (obj.Bounds.Contains(point))
                    {
                        if (!rayResults.Contains(obj))
                            rayResults.Add(obj);
                        continue;
                    }
                }
            }
        }

        private void Query(Rectangle bounds, QuadTreeNode<T> node, List<T> results)
        {
            if (node == null) return;

            if (bounds.IntersectsWith(node.Bounds))
            {
                foreach (T quadObject in node.Objects)
                {
                    if (bounds.IntersectsWith(quadObject.Bounds))
                        results.Add(quadObject);
                }

                foreach (QuadTreeNode<T> childNode in node.Nodes)
                {
                    Query(bounds, childNode, results);
                }
            }
        }
    }
}
