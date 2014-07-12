using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Puzzle_Game
{
    class PuzzleForm
    {
        private Form frmPuzzle;
        private Panel pnlContainer;
        private Button[,] buttons;
        private Control[] formControls;
        private NodeCollection nodes;
        private Timer solveTimer;
        private bool startTimer;
        private bool requiresValidation;
        private int width;
        private int height;
        
        private delegate bool NodePositions(int e, int s);

        private enum CardinalDirection
        {
            WEST = 1,
            EAST = 2,
            SOUTH = 3,
            NORTH = 4
        }

        /// <summary>
        /// Creates instance of Puzzle Game
        /// </summary>
        /// <param name="nc">NodeCollection to represent board tiles</param>
        /// <param name="w">width of the board</param>
        /// <param name="h">heigh of the board</param>
        public PuzzleForm(ref NodeCollection nc, int w, int h)
        {
            width = w;
            height = h;
            nodes = nc;
            solveTimer = new Timer();
            solveTimer.Interval = 1000;
            solveTimer.Tick += solveTimer_Tick;
            requiresValidation = false;
            startTimer = false;
            InitializeComponent();
        }

        /// <summary>
        /// Moves the clicked button to the spot of the empty tile
        /// if the empty tile is next to the clicked button
        /// </summary>
        /// <param name="sender">Clicked button</param>
        /// <param name="e">Button click event arguments</param>
        private void btnNode_Click(object sender, EventArgs e)
        {
            CardinalDirection directionOfSwap;
            if (isEmptyTileInRowOrColumn((Button)sender, out directionOfSwap))
            {
                if (startTimer)
                {
                    startTimer = false;
                    solveTimer.Enabled = true;
                }
                moveRowOrColumn((Button)sender, directionOfSwap);
            }

            if(requiresValidation)
                if (validateBoard())
                    updateTimerLabels();
        }

        private void moveRowOrColumn(Button clickedButton, CardinalDirection direction)
        {
            NodeCollection.Node correspondingNode = nodes.GetCorrespondingNode(clickedButton.Text);
            NodeCollection.Node currentNode = new NodeCollection.Node(-1,-1,-1);
            List<Button> linearValues = new List<Button>();

            foreach(Button button in buttons)
            {
                if (button != null)
                    linearValues.Add(button);
            }

            do
            {
                NodeCollection.Node emptyNode = nodes.GetEmptyNode();
                NodePositions pos = CheckConditional;
                Tuple<int, int> coordinates = nodes.GetIndexPair(emptyNode.Value.ToString());
                bool moveNodes = true;
                int x = coordinates.Item1;
                int y = coordinates.Item2;

                switch (direction)
                {
                    case CardinalDirection.WEST:
                        if (!(pos(x + 1, width)))
                            currentNode = nodes.Nodes[y, x + 1].Clone();
                        else moveNodes = false;
                        break;
                    case CardinalDirection.EAST:
                        if (!(pos(x - 1, -1)))
                            currentNode = nodes.Nodes[y, x - 1].Clone();
                        else moveNodes = false;
                        break;
                    case CardinalDirection.SOUTH:
                        if (!(pos(y + 1, height)))
                            currentNode = nodes.Nodes[y + 1, x].Clone();
                        else moveNodes = false;
                        break;
                    case CardinalDirection.NORTH:
                        if (!(pos(y - 1, -1)))
                            currentNode = nodes.Nodes[y - 1, x].Clone();
                        else moveNodes = false;
                        break;
                }

                if (moveNodes)
                {
                    Button currentButton = (from button in linearValues where button.Text == currentNode.Value.ToString() select button).FirstOrDefault();

                    nodes.SwapWithEmptyNode(currentNode.Value.ToString());
                    updateClickedTile(currentButton, direction);
                }

            } while (correspondingNode.Value != 0);
            
        }

        /// <summary>
        /// Determines if the clicked button is in the
        /// same row or colums as the empty tile
        /// </summary>
        /// <param name="clickedButton">Clicked button</param>
        /// <param name="direction">out param to provide the direction of the empty tile
        /// or 0 if the empty tile is not in the same row or column</param>
        /// <returns>true if the emtpy tile is in the same row or column of the clicked button</returns>
        private bool isEmptyTileInRowOrColumn(Button clickedButton, out CardinalDirection direction)
        {
            NodeCollection.Node emptyNode = nodes.GetEmptyNode();
            NodeCollection.Node correspondingNode = nodes.GetCorrespondingNode(clickedButton.Text);
            NodePositions pos = CheckConditional;

            if (pos(emptyNode.Y, correspondingNode.Y))
            {
                if (emptyNode.X < correspondingNode.X)
                    direction = CardinalDirection.WEST;
                else direction = CardinalDirection.EAST;
            }
            else if (pos(emptyNode.X, correspondingNode.X))
            {
                if (emptyNode.Y < correspondingNode.Y)
                    direction = CardinalDirection.SOUTH;
                else direction = CardinalDirection.NORTH;
            }
            else direction = 0;

            return direction != 0;
        }

        /// <summary>
        /// Time Tracker for puzzle
        /// </summary>
        private void updateTimerLabels()
        {
            solveTimer.Enabled = false;
            requiresValidation = false;
            startTimer = false;
            Control lblTimer = formControls.Where((Control x) => x.Name == "lblTimer").FirstOrDefault();
            Control lblPrevTime = formControls.Where((Control x) => x.Name == "lblPrevTime").FirstOrDefault();
            int timeInSeconds = int.Parse(new Regex("\\d+").Match(lblTimer.Text).Value);
            lblPrevTime.Text = "Prev Time: " + timeInSeconds;
        }

        /// <summary>
        /// Updates the clicked tile to move it the location of the empty tile
        /// </summary>
        /// <param name="tile">Clicked button</param>
        /// <param name="direction">direction in which to move the clicked button</param>
        private void updateClickedTile(Button tile, CardinalDirection direction)
        {
            switch (direction)
            {
                case CardinalDirection.WEST:
                    tile.Location = new Point(tile.Location.X - 35, tile.Location.Y);
                    break;
                case CardinalDirection.EAST:
                    tile.Location = new Point(tile.Location.X + 35, tile.Location.Y);
                    break;
                case CardinalDirection.SOUTH:
                    tile.Location = new Point(tile.Location.X, tile.Location.Y - 35);
                    break;
                case CardinalDirection.NORTH:
                    tile.Location = new Point(tile.Location.X, tile.Location.Y + 35);
                    break;
            }
        }

        /// <summary>
        /// Redraws all buttons on the board to reflect 
        /// the current state of the NodeCollection
        /// </summary>
        private void updateEntireBoard()
        {
            NodeCollection.Node[,] board = nodes.Nodes;

            frmPuzzle.Controls.Remove(pnlContainer);

            //foreach (Control control in pnlContainer.Controls)
            //    pnlContainer.Controls.Remove(control);

            pnlContainer = new Panel();
            pnlContainer.Size = new Size(width * 35 + 5, height * 35 + 5);
            pnlContainer.Name = "pnlContainer";
            pnlContainer.Location = new Point(20, 10);
            pnlContainer.BorderStyle = BorderStyle.Fixed3D;

            buttons = new Button[width, height];

            foreach(NodeCollection.Node node in board)
            {
                Tuple<int, int> indexes = nodes.GetIndexPair(node.Value.ToString());

                if (node.Value != 0)
                {
                    Button newButton = new Button();
                    newButton.Name = "btnNode_" + node.Value;
                    newButton.Text = node.Value.ToString();
                    newButton.Location = new Point(node.X * 35, node.Y * 35);
                    newButton.Size = new Size(35, 35);
                    newButton.Visible = true;
                    newButton.Click += btnNode_Click;
                    buttons[indexes.Item1, indexes.Item2] = newButton;
                }
            }

            BuildForm();
        }

        /// <summary>
        /// Checks if two numbers are equal
        /// </summary>
        /// <param name="e">left of operator</param>
        /// <param name="s">right of operator</param>
        /// <returns>true if e and s are equal</returns>
        private bool CheckConditional(int e, int s) { return e == s; }

        /// <summary>
        /// Resets the time tracker labels on the board
        /// </summary>
        private void resetTimer()
        {
            solveTimer.Enabled = false;
            Control lblTimer = formControls.Where((Control x) => x.Name == "lblTimer").FirstOrDefault();
            lblTimer.Text = "Timer: 0";
        }

        /// <summary>
        /// Shuffles the tiles on the board to a completely random state
        /// </summary>
        /// <param name="sender">Shuffle button</param>
        /// <param name="e">Button click event arguments</param>
        private void btnShuffle_Click(object sender, EventArgs e)
        {
            startTimer = true;
            requiresValidation = true;
            resetTimer();
            shuffle();
            updateEntireBoard();
        }

        /// <summary>
        /// Shuffles the nodes in the NodeCollection
        /// </summary>
        private void shuffle() { nodes.ShuffleNodes(); }

        /// <summary>
        /// Validates that the puzzle is solved
        /// </summary>
        /// <returns>true if the puzzle is solved, false otherwise</returns>
        private bool validateBoard() { return nodes.validateNodeOrder(); }

        /// <summary>
        /// Timer tick event for time tracking label
        /// </summary>
        /// <param name="sender">Timer control</param>
        /// <param name="e">Timer tick event arguments</param>
        private void solveTimer_Tick(object sender, EventArgs e)
        {
            Control lblTimer = formControls.Where((Control x) => x.Name == "lblTimer").FirstOrDefault();
            int time = int.Parse(new Regex("\\d+").Match(lblTimer.Text).ToString());
            lblTimer.Text = "Timer: " + (time + 1);
        }

        /// <summary>
        /// stops the timer to prevent error on exit
        /// </summary>
        /// <param name="sender">form control</param>
        /// <param name="e">Form close event arguments</param>
        private void Puzzle_Close(object sender, EventArgs e) { solveTimer.Enabled = false; }

        /// <summary>
        /// Creates the board controls
        /// </summary>
        private void InitializeComponent()
        {
            // Form
            frmPuzzle = new Form();
            frmPuzzle.Size = new Size(width * 35 + 120, height * 35 + 120);
            frmPuzzle.Name = "frmPuzzle";
            frmPuzzle.Text = (height * width) - 1 + " Piece Puzzle";
            frmPuzzle.FormClosing += Puzzle_Close;

            // Panel
            pnlContainer = new Panel();
            pnlContainer.Size = new Size(width * 35 + 5, height * 35 + 5);
            pnlContainer.Name = "pnlContainer";
            pnlContainer.Location = new Point(20, 10);
            pnlContainer.BorderStyle = BorderStyle.Fixed3D;

            // Buttons
            buttons = new Button[width, height];
            int counter = 1;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i < height - 1 || j < width - 1)
                    {
                        Button newButton = new Button();
                        newButton.Size = new Size(35, 35);
                        newButton.Name = "btnNode_" + counter;
                        newButton.Text = counter.ToString();
                        newButton.Location = new Point(j * 35, i * 35);
                        newButton.Click += btnNode_Click;
                        buttons[j, i] = newButton;
                        counter++;
                    }
                }
            }

            // Timer Label
            Label lblTimer = new Label();
            lblTimer.Name = "lblTimer";
            lblTimer.Text = "Timer: 0";
            lblTimer.Location = new Point(frmPuzzle.Size.Width - 90, 20);
            lblTimer.Visible = true;

            // Previous Time Label
            Label lblPrevTime = new Label();
            lblPrevTime.Name = "lblPrevTime";
            lblPrevTime.Text = "";
            lblPrevTime.Location = new Point(frmPuzzle.Size.Width - 95, 50);
            lblPrevTime.Visible = true;

            // Shuffle Button
            Button btnShuffle = new Button();
            btnShuffle.Name = "btnShuffle";
            btnShuffle.Text = "Shuffle";
            btnShuffle.Location = new Point(20, frmPuzzle.Size.Height - 90);
            btnShuffle.Click += btnShuffle_Click;

            frmPuzzle.Controls.Add(lblTimer);
            frmPuzzle.Controls.Add(lblPrevTime);
            frmPuzzle.Controls.Add(btnShuffle);

            BuildForm();
        }

        /// <summary>
        /// Builds and displays the form
        /// </summary>
        private void BuildForm()
        {
            foreach (Button button in buttons)
                if (button != null)
                {
                    button.Visible = true;
                    pnlContainer.Controls.Add(button);
                }

            pnlContainer.Visible = true;

            frmPuzzle.Controls.Add(pnlContainer);
            frmPuzzle.Visible = true;

            formControls = new Control[frmPuzzle.Controls.Count];
            frmPuzzle.Controls.CopyTo(formControls, 0);
        }
    }
}
