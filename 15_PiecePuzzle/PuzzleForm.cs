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
        public Timer solveTimer;
        private int width;
        private int height;

        private delegate bool NodePositions(int e, int s);

        public Form Puzzle { get { return frmPuzzle; } }
        public Panel ButtonContainer { get { return pnlContainer; } }
        public Button this[int x, int y] { get { return buttons[x, y]; } }

        public PuzzleForm(ref NodeCollection nc, int w, int h)
        {
            width = w;
            height = h;
            nodes = nc;
            solveTimer = new Timer();
            solveTimer.Interval = 1000;
            solveTimer.Tick += solveTimer_Tick;
            InitializeComponent();
        }

        private void btnNode_Click(object sender, EventArgs e)
        {
            int directionOfSwap;
            if(isNeighbouringEmptyTile((Button)sender, out directionOfSwap))
            {
                solveTimer.Enabled = true;
                nodes.SwapWithEmptyNode((sender as Button).Text);
                updateClickedTile((Button)sender, directionOfSwap);
            }
        }

        private void updateClickedTile(Button tile, int direction)
        {
            switch (direction)
            {
                case 1:
                    tile.Location = new Point(tile.Location.X - 35, tile.Location.Y);
                    break;
                case 2:
                    tile.Location = new Point(tile.Location.X + 35, tile.Location.Y);
                    break;
                case 3:
                    tile.Location = new Point(tile.Location.X, tile.Location.Y - 35);
                    break;
                case 4:
                    tile.Location = new Point(tile.Location.X, tile.Location.Y + 35);
                    break;
            }
        }

        private void updateEntireBoard()
        {
            NodeCollection.Node[,] board = nodes.Nodes;

            frmPuzzle.Controls.Remove(pnlContainer);

            pnlContainer = new Panel();
            pnlContainer.Size = new Size(width * 35 + 5, height * 35 + 5);
            pnlContainer.Name = "pnlContainer";
            pnlContainer.Location = new Point(20, 10);
            pnlContainer.BorderStyle = BorderStyle.Fixed3D;

            foreach(NodeCollection.Node node in board)
            {
                if (node.Value != 0)
                {
                    Button newButton = new Button();
                    newButton.Name = "btnNode_" + node.Value;
                    newButton.Text = node.Value.ToString();
                    newButton.Location = new Point(node.X * 35, node.Y * 35);
                    newButton.Size = new Size(35, 35);
                    newButton.Visible = true;
                    newButton.Click += btnNode_Click;
                    pnlContainer.Controls.Add(newButton);
                }
            }

            BuildForm(false);
        }

        private bool CheckConditional(int e, int s) { return e == s; }

        private bool isNeighbouringEmptyTile(Button clickedButton, out int direction)
        {
            NodeCollection.Node emptyNode = nodes.GetEmptyNode();
            NodeCollection.Node correspondingNode = nodes.GetCorrespondingNode(clickedButton.Text);
            NodePositions pos = CheckConditional;

            if (pos(emptyNode.X + 1, correspondingNode.X) && pos(emptyNode.Y, correspondingNode.Y))
                direction = 1;
            else if (pos(emptyNode.X - 1, correspondingNode.X) && pos(emptyNode.Y, correspondingNode.Y))
                direction = 2;
            else if (pos(emptyNode.X, correspondingNode.X) && pos(emptyNode.Y + 1, correspondingNode.Y))
                direction = 3;
            else if (pos(emptyNode.X, correspondingNode.X) && pos(emptyNode.Y - 1, correspondingNode.Y))
                direction = 4;
            else
                direction = 0;

            return direction != 0;
        }

        private void resetTimer()
        {
            solveTimer.Enabled = false;
            Control lblTimer = formControls.Where((Control x) => x.Name == "lblTimer").FirstOrDefault();
            Control lblPrevTime = formControls.Where((Control x) => x.Name == "lblPrevTime").FirstOrDefault();
            lblTimer.Text = "Timer: 0";
            lblPrevTime.Text = "";
        }

        private void btnShuffle_Click(object sender, EventArgs e)
        {
            resetTimer();
            shuffle();
            updateEntireBoard();
        }

        private void shuffle() { nodes.ShuffleNodes(); }

        private void solveTimer_Tick(object sender, EventArgs e)
        {
            Control lblTimer = formControls.Where((Control x) => x.Name == "lblTimer").FirstOrDefault();
            int time = int.Parse(new Regex("\\d+").Match(lblTimer.Text).ToString());
            lblTimer.Text = "Timer: " + (time + 1);
        }

        private void Puzzle_Close(object sender, EventArgs e) { solveTimer.Enabled = false; }

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

            BuildForm(true);
        }

        private void BuildForm(bool addButtons)
        {
            if (addButtons)
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
