using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle_Game
{
    class NodeCollection
    {
        private Node[,] nodes;
        private int count;
        private int width;
        private int height;
        private int buttonSize;

        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public int Count { get { return count; } }

        public NodeCollection(int w, int h, int size)
        {
            width = w;
            height = h;
            buttonSize = size;
            nodes = new Node[h, w];
            count = 0;
        }

        public void AddNode(int identifier, int w, int h, int x, int y)
        {
            nodes[h, w] = new Node(identifier, x, y);
            count++;
        }

        public void linkNodes(int width, int height)
        {
            for(int h = 0; h < height; h++)
            {
                for(int w = 0; w < width; w++)
                {
                    if(w + h == 0)
                    {
                        nodes[h, w].north = null;
                        nodes[h, w].west = null;
                        nodes[h, w].east = nodes[h, w + 1];
                        nodes[h, w].south = nodes[h + 1, w];
                    }
                    else if(h == 0 && w < width - 1)
                    {
                        nodes[h, w].north = null;
                        nodes[h, w].west = nodes[h, w - 1];
                        nodes[h, w].east = nodes[h, w + 1];
                        nodes[h, w].south = nodes[h + 1, w];
                    }
                    else if(h == 0 && w == width - 1)
                    {
                        nodes[h, w].north = null;
                        nodes[h, w].west = nodes[h, w - 1];
                        nodes[h, w].east = null;
                        nodes[h, w].south = nodes[h + 1, w];
                    }
                    else if(w == 0 && h == height - 1)
                    {
                        nodes[h, w].north = nodes[h - 1, w];
                        nodes[h, w].west = null;
                        nodes[h, w].east = nodes[h, w + 1];
                        nodes[h, w].south = null;
                    }
                    else if(w == 0 && h < height - 1)
                    {
                        nodes[h, w].north = nodes[h - 1, w];
                        nodes[h, w].west = null;
                        nodes[h, w].east = nodes[h, w + 1];
                        nodes[h, w].south = nodes[h + 1, w];
                    }
                    else if(w == width - 1 && h < height - 1)
                    {
                        nodes[h, w].north = nodes[h - 1, w];
                        nodes[h, w].west = nodes[h, w - 1];
                        nodes[h, w].east = null;
                        nodes[h, w].south = nodes[h + 1, w];
                    }
                    else if(w < width - 1 && h == height - 1)
                    {
                        nodes[h, w].north = nodes[h - 1, w];
                        nodes[h, w].west = nodes[h, w - 1];
                        nodes[h, w].east = nodes[h, w + 1];
                        nodes[h, w].south = null;
                    }
                    else if(w < width - 1 && h < height - 1)
                    {
                        nodes[h, w].north = nodes[h - 1, w];
                        nodes[h, w].west = nodes[h, w - 1];
                        nodes[h, w].east = nodes[h, w + 1];
                        nodes[h, w].south = nodes[h + 1, w];            
                    }
                    else if(w == width - 1 && h == height - 1)
                    {
                        nodes[h, w].north = nodes[h - 1, w];
                        nodes[h, w].west = nodes[h, w - 1];
                        nodes[h, w].east = null;
                        nodes[h, w].south = null;
                    }
                }
            }
        }

        public IEnumerator<Node> GetEnumerator()
        {
            foreach (Node node in nodes)
                yield return node;
        }

        public void updateSurroundingPointers(Node movedNode, Node emptyNode, int direction)
        {
            Node tempEmptyNode = new Node(emptyNode);
            Node tempMovedNode = new Node(movedNode);

            if(direction % 2 == 1)
            {
                if(movedNode.east != null && emptyNode.east != null)
                {
                    emptyNode.east.west = emptyNode;
                    movedNode.east.west = movedNode;
                }

                if(movedNode.west != null && emptyNode.west != null)
                {
                    emptyNode.west.east = emptyNode;
                    movedNode.west.east = movedNode;
                }

                if(direction == 1)
                {
                    if (emptyNode.north != null)
                        emptyNode.north.south = emptyNode;

                    if (movedNode.south != null)
                        movedNode.south.north = movedNode;
                }
                else
                {
                    if (emptyNode.south != null)
                        emptyNode.south.north = emptyNode;

                    if (movedNode.north != null)
                        movedNode.north.south = movedNode;
                }
            }
            else
            {
                if(movedNode.north != null && emptyNode.north != null)
                {
                    emptyNode.north.south = emptyNode;
                    movedNode.north.south = movedNode;
                }

                if(movedNode.south != null && emptyNode.south != null)
                {
                    emptyNode.south.north = emptyNode;
                    movedNode.south.north = movedNode;
                }

                if (direction == 2)
                {
                    if (emptyNode.west != null)
                        emptyNode.west.east = emptyNode;
                    if (movedNode.east != null)
                        movedNode.east.west = movedNode;
                }
                else
                {
                    if (emptyNode.east != null)
                        emptyNode.east.west = emptyNode;
                    if (movedNode.west != null)
                        movedNode.west.east = movedNode;
                }
            }
        }

        public void moveNodes(Node movingNode, Node emptyNode, int direction)
        {
            Node tempEmptyNode = new Node(emptyNode);
            Node tempMovingNode = new Node(movingNode);

            switch (direction)
            {
                case 1: // moving down
                    emptyNode.north = tempMovingNode.north;
                    emptyNode.west = tempMovingNode.west;
                    emptyNode.east = tempMovingNode.east;
                    emptyNode.south = movingNode;
                    movingNode.north = emptyNode;
                    movingNode.west = tempEmptyNode.west;
                    movingNode.east = tempEmptyNode.east;
                    movingNode.south = tempEmptyNode.south;
                    movingNode.VerticalPosition += buttonSize;
                    emptyNode.VerticalPosition += buttonSize * -1;
                    break;
                case 2: // moving right
                    emptyNode.north = tempMovingNode.north;
                    emptyNode.west = tempMovingNode.west;
                    emptyNode.east = movingNode;
                    emptyNode.south = tempMovingNode.south;
                    movingNode.north = tempEmptyNode.north;
                    movingNode.west = emptyNode;
                    movingNode.east = tempEmptyNode.east;
                    movingNode.south = tempEmptyNode.south;
                    movingNode.HorizontalPosition += buttonSize;
                    emptyNode.HorizontalPosition += buttonSize * -1;
                    break;
                case 3: // moving up
                    emptyNode.north = movingNode;
                    emptyNode.west = tempMovingNode.west;
                    emptyNode.east = tempMovingNode.east;
                    emptyNode.south = tempMovingNode.south;
                    movingNode.north = tempEmptyNode.north;
                    movingNode.west = tempEmptyNode.west;
                    movingNode.east = tempEmptyNode.east;
                    movingNode.south = emptyNode;
                    movingNode.VerticalPosition += buttonSize * -1;
                    emptyNode.VerticalPosition += buttonSize;
                    break;
                case 4: // moving left
                    emptyNode.north = tempMovingNode.north;
                    emptyNode.west = movingNode;
                    emptyNode.east = tempMovingNode.east;
                    emptyNode.south = tempMovingNode.south;
                    movingNode.north = tempEmptyNode.north;
                    movingNode.west = tempEmptyNode.west;
                    movingNode.east = emptyNode;
                    movingNode.south = tempEmptyNode.south;
                    movingNode.HorizontalPosition += buttonSize * -1;
                    emptyNode.HorizontalPosition += buttonSize;
                    break;
            }

            updateSurroundingPointers(movingNode, emptyNode, direction);
        }
    }

    class Node
    {
        public Node north;
        public Node south;
        public Node east;
        public Node west;
        private int value;
        private int x_pos;
        private int y_pos;

        public int Value 
        { 
            get { return this.value; }
            set { this.value = value; }
        }

        public int HorizontalPosition 
        { 
            get { return x_pos; }
            set { x_pos = value; }
        }

        public int VerticalPosition 
        { 
            get { return y_pos; }
            set { y_pos = value; }
        }

        public Node() { value = 0; }

        public Node(int val, int x, int y) 
        { 
            value = val;
            x_pos = x;
            y_pos = y;
        }

        public Node(Node node)
        {
            this.value = node.value;
            this.x_pos = node.x_pos;
            this.y_pos = node.y_pos;
            this.north = node.north;
            this.south = node.south;
            this.east = node.east;
            this.west = node.west;
        }
    }
}
