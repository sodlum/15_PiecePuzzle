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
        private Node[,] currentState;
        private int width;
        private int height;

        /// <summary>
        /// Gets the current state of the NodeCollection
        /// </summary>
        public Node[,] Nodes { get { return currentState; } }

        /// <summary>
        /// Creates the NodeCollection
        /// </summary>
        /// <param name="w">total number of nodes in a row</param>
        /// <param name="h">total number of nodes in a column</param>
        public NodeCollection(int w, int h)
        {
            nodes = new Node[h, w];
            currentState = new Node[h, w];
            width = w;
            height = h;
            CreateNodes();
        }

        /// <summary>
        /// Creates nodes to represent board tiles
        /// </summary>
        private void CreateNodes()
        {
            int counter = 1;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i < height - 1 || j < width - 1)
                    {
                        Node newNode = new Node(counter, j, i);
                        nodes[i, j] = newNode;
                        currentState[i, j] = newNode.Clone();
                        counter++;
                    }
                    else
                    {
                        Node newNode = new Node(0, j, i);
                        nodes[i, j] = newNode;
                        currentState[i, j] = newNode.Clone();
                    }
                }
            }
        }

        /// <summary>
        /// Finds the empty node in the current state
        /// </summary>
        /// <returns>empty node</returns>
        public Node GetEmptyNode()
        {
            Node emptyNode = null;

            foreach (Node node in currentState)
                if (node.Value == 0)
                {
                    emptyNode = node;
                    break;
                }

            return emptyNode;
        }

        /// <summary>
        /// Finds the node with the corresponding value
        /// </summary>
        /// <param name="value">value of the node</param>
        /// <returns>node corresponding to value</returns>
        public Node GetCorrespondingNode(string value)
        {
            if (value == "0")
                return GetEmptyNode();

            Node corresponding = null;

            foreach(Node node in currentState)
                if(node.Value.ToString() == value)
                {
                    corresponding = node;
                    break;
                }

            return corresponding;
        }

        /// <summary>
        /// Randomly shuffles Nnodes in the current state
        /// </summary>
        public void ShuffleNodes()
        {
            Random rnd = new Random();

            foreach(Node node in currentState)
            {
                Node rndNode;
                do { rndNode = currentState[rnd.Next(height), rnd.Next(width)]; } while (rndNode.Equals(node));

                SwapNodes(node, rndNode);
            }

            if (!validateShuffle())
                SwapNodes(GetCorrespondingNode("1"), GetCorrespondingNode("2"));
        }

        /// <summary>
        /// Validates that the shuffle is valid and the puzzle is capable of being solved
        /// </summary>
        /// <returns>true if the puzzle is valid, false otherwise</returns>
        public bool validateShuffle()
        {
            int rowOfEmptyTile = 0;
            int inversions = 0;
            List<int> linearValues = new List<int>();

            foreach (Node node in currentState)
                linearValues.Add(node.Value);

            for (int i = 0; i < linearValues.Count; i++)
            {
                if (linearValues[i] == 0)
                {
                    rowOfEmptyTile = (i / width);
                    continue;
                }

                for (int j = i + 1; j < linearValues.Count; j++)
                {
                    if (linearValues[j] == 0)
                        continue;

                    if (linearValues[i] > linearValues[j])
                        inversions++;
                }
            }

            return ((width % 2 == 1 && inversions % 2 == 0) || (width % 2 == 0) && ((height - rowOfEmptyTile) + inversions % 2 == 1));
        }

        /// <summary>
        /// Swaps two selected nodes
        /// </summary>
        /// <param name="first">First node</param>
        /// <param name="second">Second node</param>
        private void SwapNodes(Node first, Node second)
        {
            Tuple<int, int> firstSet = GetIndexPair(first.Value.ToString());
            Tuple<int, int> secondSet = GetIndexPair(second.Value.ToString());

            Swap(firstSet.Item2,
                 firstSet.Item1,
                 secondSet.Item2,
                 secondSet.Item1);
        }

        /// <summary>
        /// Swaps the selected node with the empty node
        /// </summary>
        /// <param name="value">value of corresponding node</param>
        public void SwapWithEmptyNode(string value)
        {
            Node emptyNode = GetEmptyNode();
            Node swappingNode = GetCorrespondingNode(value);

            Tuple<int, int> indexPairOfEmptyNode = GetIndexPair("0");
            Tuple<int, int> indexPairOfSwappingNode = GetIndexPair(value);

            Swap(indexPairOfEmptyNode.Item2,
                 indexPairOfEmptyNode.Item1,
                 indexPairOfSwappingNode.Item2,
                 indexPairOfSwappingNode.Item1);
        }

        /// <summary>
        /// Perfoms the swap in the currens state of the NodeCollection
        /// </summary>
        /// <param name="fY">First node y position</param>
        /// <param name="fX">First node x position</param>
        /// <param name="sY">Second node y position</param>
        /// <param name="sX">Second node x position</param>
        private void Swap(int fY, int fX, int sY, int sX)
        {
            int tempValue = currentState[fY, fX].Value;
            currentState[fY, fX].Value = currentState[sY, sX].Value;
            currentState[sY, sX].Value = tempValue;
        }

        /// <summary>
        /// Returns the (x, y) position of the node with the corresponding value
        /// </summary>
        /// <param name="value">value of corresponding node</param>
        /// <returns>(x, y) pair</returns>
        public Tuple<int, int> GetIndexPair(string value)
        {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    if (currentState[j, i].Value.ToString() == value)
                        return Tuple.Create(i, j);

            return Tuple.Create(-1, -1);
        }

        /// <summary>
        /// Validates that the current state of the 
        /// NodeCollection is in the solved state
        /// </summary>
        /// <returns>true if the puzzle is solved, false otherwise</returns>
        public bool validateNodeOrder() 
        {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    if (currentState[j, i].Value != nodes[j, i].Value)
                        return false;
            return true;
        }

        /// <summary>
        /// Node class to represent each node in the current state of the NodeCollection
        /// </summary>
        internal class Node : ICloneable
        {
            private int value;
            private int x_pos;
            private int y_pos;

            /// <summary>
            /// Gets or sets the value of the current node
            /// </summary>
            public int Value { get { return value; } set { this.value = value; } }

            /// <summary>
            /// Gets or sets the x position of the current node
            /// </summary>
            public int X { get { return x_pos; } }

            /// <summary>
            /// Gets or sets the y position of the current node
            /// </summary>
            public int Y { get { return y_pos; } }

            /// <summary>
            /// Creates an instance of the Node class
            /// </summary>
            /// <param name="v">Node value</param>
            /// <param name="x">Node x position</param>
            /// <param name="y">Node y position</param>
            public Node(int v, int x, int y)
            {
                value = v;
                x_pos = x;
                y_pos = y;
            }

            /// <summary>
            /// Creates a deep copy of the current node
            /// </summary>
            /// <returns>copy of node</returns>
            public Node Clone() { return (Node)this.MemberwiseClone(); }

            /// <summary>
            /// Implements ICloneable.Clone()
            /// </summary>
            /// <returns>cloned object</returns>
            object ICloneable.Clone() { return Clone(); }
        }
    }
}
