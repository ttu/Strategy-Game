using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public class QuadTreeNode<T> where T : class, IQuadObject
    {
        // Tree knows max amount for objects
        public List<T> Objects = new List<T>();

        public Rectangle Bounds
        { get; set; }

        // All nodes have 4 child nodes
        public QuadTreeNode<T>[] Nodes = new QuadTreeNode<T>[4];

        public QuadTreeNode(Rectangle bounds)
        {
            Bounds = bounds;
        }

        public QuadTreeNode(float x, float y, float width, float height)
            : this(new Rectangle(x, y, width, height))
        { }

        public bool HasChildNodes()
        {
            return Nodes[0] != null;
        }
    }

    /// <summary>
    /// Interface for object that can be placed in QuadTree
    /// </summary>
    public interface IQuadObject
    {
        Rectangle Bounds { get; }
        event EventHandler BoundsChanged;
    }
}
