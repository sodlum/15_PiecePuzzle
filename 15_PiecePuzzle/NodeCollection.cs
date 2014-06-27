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

        /// <summary>
        /// Gets the width of the current board in play
        /// </summary>
        public int Width { get { return width; } }

        /// <summary>
        /// Gets the height of the current board in play
        /// </summary>
        public int Height { get { return height; } }

        /// <summary>
        /// Gets the total count of tiles on the board
        /// </summary>
        public int Count { get { return count; } }

        /// <summary>
        /// Initializes the current node collection with the width, height, and size of tiles
        /// </summary>
        /// <param name="w">width of the current board</param>
        /// <param name="h">height of the current board</param>
        /// <param name="size">size of each side of the tiles on the current board</param>
        public NodeCollection(int w, int h, int size)
        {
            width = w;
            height = h;
            buttonSize = size;
            nodes = new Node[h, w];
            count = 0;
        }

        /// <summary>
        /// Adds Node to NodeCollection
        /// </summary>
        /// <param name="identifier">Tile number of the node</param>
        /// <param name="w">horizontal position of the node in the NodeCollection</param>
        /// <param name="h">vertical postion of the node in the NodeCollection</param>
        /// <param name="x">position along the x-axis where the corresponding tile sits</param>
        /// <param name="y">position along the y-axis where the corresponding tile sits</param>
        public void AddNode(int identifier, int w, int h, int x, int y)
        {
            nodes[h, w] = new Node(identifier, x, y);
            count++;
        }

        /// <summary>
        /// Finds the node representing the emtpy tile
        /// </summary>
        /// <returns>Node representing the empty tile</returns>
        public Node getEmptyNode()
        {
            Node emptyNode = null;

            foreach (Node node in nodes)
                if (node.Value == 0)
                    emptyNode = node;

            return emptyNode;
        }

        /// <summary>
        /// Links all Nodes in the NodeCollection together based on cardinal direction
        /// </summary>
        public void linkNodes()
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

        /// <summary>
        /// Iterates through each Node in the NodeCollection
        /// </summary>
        /// <returns>Iterator of the current Node</returns>
        public IEnumerator<Node> GetEnumerator()
        {
            foreach (Node node in nodes)
                yield return node;
        }

        /// <summary>
        /// Updates pointers surrounding the empty node and the node that was moved
        /// </summary>
        /// <param name="movedNode">Node that was moved to the empty tile position</param>
        /// <param name="emptyNode">The new empty tile position</param>
        /// <param name="direction">Direction in which movedNode traveled</param>
        public void updateSurroundingPointers(Node movedNode, Node emptyNode, int direction)
        {
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

        /// <summary>
        /// Updates pointers for the node being moved and the emtpy node
        /// </summary>
        /// <param name="movingNode">Node being moved</param>
        /// <param name="emptyNode">Current empty Node</param>
        /// <param name="direction">Direction in which movingNode will travel</param>
        public void moveNodes(Node movingNode, Node emptyNode, int direction)
        {
            Node tempEmptyNode = (emptyNode.Clone() as Node);
            Node tempMovingNode = (movingNode.Clone() as Node);

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

        /// <summary>
        /// Validates the order of the nodes to ensure they are in the solved state
        /// </summary>
        /// <returns>true if the nodes are in the proper order, false otherwise</returns>
        public bool validateNodeOrder()
        {
            Node emptyNode = getEmptyNode();
            
            if (emptyNode.south != null || emptyNode.east != null)
                return false;
            
            Node currentNode = emptyNode.Clone();

            int wIndex = width - 1;
            int hIndex = height - 1;
            bool goLeft = true;

            if(height % 2 != 0)
            {
                while (wIndex >= 0 && hIndex >= 0)
                {
                    if(goLeft)
                    {
                        if (currentNode.Value != nodes[hIndex, wIndex].Value)
                            return false;

                        if(wIndex == 0)
                        {
                            goLeft = false;
                            if(hIndex != 0)
                                currentNode = currentNode.north.Clone();
                            hIndex--;
                        }
                        else
                        {
                            currentNode = currentNode.west.Clone();
                            wIndex--;
                        }
                    }
                    else
                    {
                        if (currentNode.Value != nodes[hIndex, wIndex].Value)
                            return false;

                        if(wIndex == width - 1)
                        {
                            goLeft = true;
                            if(hIndex != 0)
                                currentNode = currentNode.north.Clone();
                            hIndex--;
                        }
                        else
                        {
                            currentNode = currentNode.east.Clone();
                            wIndex++;
                        }
                    }
                }
            }
            else
            {
                while (wIndex < width && hIndex >= 0)
                {
                    if (goLeft)
                    {
                        if (currentNode.Value != nodes[hIndex, wIndex].Value)
                            return false;

                        if (wIndex == 0)
                        {
                            goLeft = false;
                            if(hIndex != 0)
                                currentNode = currentNode.north.Clone();
                            hIndex--;
                        }
                        else
                        {
                            currentNode = currentNode.west.Clone();
                            wIndex--;
                        }
                    }
                    else
                    {
                        if (currentNode.Value != nodes[hIndex, wIndex].Value)
                            return false;

                        if (wIndex == width - 1)
                        {
                            goLeft = true;
                            if(hIndex != 0)
                                currentNode = currentNode.north.Clone();
                            hIndex--;
                        }
                        else
                        {
                            currentNode = currentNode.east.Clone();
                            wIndex++;
                        }
                    }
                }
            }

            return true;
        }
    }

    class Node : ICloneable
    {
        public Node north;
        public Node south;
        public Node east;
        public Node west;
        private int value;
        private int x_pos;
        private int y_pos;

        /// <summary>
        /// Gets or sets the value of the current Node
        /// </summary>
        public int Value 
        { 
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Gets or sets the current Node position along the x-axis of the corresponding tile
        /// </summary>
        public int HorizontalPosition 
        { 
            get { return x_pos; }
            set { x_pos = value; }
        }

        /// <summary>
        /// Gets or sets the current Node position along the y-axis of the corresponding tile
        /// </summary>
        public int VerticalPosition 
        { 
            get { return y_pos; }
            set { y_pos = value; }
        }

        /// <summary>
        /// Creates an empty Node
        /// </summary>
        public Node() { value = 0; }

        /// <summary>
        /// Creates a new Node representing a board tile
        /// </summary>
        /// <param name="val">Value of the corresponding tile</param>
        /// <param name="x">Position along the x-axis of the corresponding tile</param>
        /// <param name="y">Position along the y-axis of the corresponding tile</param>
        public Node(int val, int x, int y) 
        { 
            value = val;
            x_pos = x;
            y_pos = y;
        }

        /// <summary>
        /// Clones the current Node
        /// </summary>
        /// <returns>deep copy of the current Node</returns>
        object ICloneable.Clone()
        {
            return this.Clone();
        }

        /// <summary>
        /// Clones the current Node
        /// </summary>
        /// <returns>deep copy of the current Node</returns>
        public Node Clone()
        {
            return (Node)this.MemberwiseClone();
        }
    }
}
