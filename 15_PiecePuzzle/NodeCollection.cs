using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle_Game
{
    class NodeCollection
    {
        public Node[,] nodes;
        public Node[,] currentState;
        private int width;
        private int height;

        public Node[,] Nodes { get { return currentState; } set { currentState = value; } }

        public NodeCollection(int w, int h)
        {
            nodes = new Node[h, w];
            currentState = new Node[h, w];
            width = w;
            height = h;
            CreateNodes();
        }

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
                    rowOfEmptyTile = (i / height);
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

            return ((width % 2 == 1 && inversions % 2 == 0) || (width % 2 == 0) && ((height - rowOfEmptyTile) % 2 == 1) == (inversions % 2 == 0));
        }

        private void SwapNodes(Node first, Node second)
        {
            Tuple<int, int> firstSet = GetIndexPair(first.Value.ToString());
            Tuple<int, int> secondSet = GetIndexPair(second.Value.ToString());

            Swap(firstSet.Item2,
                 firstSet.Item1,
                 secondSet.Item2,
                 secondSet.Item1);
        }

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

        private void Swap(int fY, int fX, int sY, int sX)
        {
            int tempValue = currentState[fY, fX].Value;
            currentState[fY, fX].Value = currentState[sY, sX].Value;
            currentState[sY, sX].Value = tempValue;
        }

        private Tuple<int, int> GetIndexPair(string value)
        {
            int y = currentState.GetLength(0);
            int x = currentState.GetLength(1);

            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                    if (currentState[j, i].Value.ToString() == value)
                        return Tuple.Create(i, j);

            return Tuple.Create(-1, -1);
        }

        internal class Node : ICloneable
        {
            private int value;
            private int x_pos;
            private int y_pos;

            public int Value { get { return value; } set { this.value = value; } }
            public int X { get { return x_pos; } }
            public int Y { get { return y_pos; } }

            public Node(int v, int x, int y)
            {
                value = v;
                x_pos = x;
                y_pos = y;
            }

            public Node Clone() { return (Node)this.MemberwiseClone(); }
            object ICloneable.Clone() { return Clone(); }
        }
    }
}
