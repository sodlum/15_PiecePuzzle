using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Puzzle_Game
{
    public partial class Puzzle : Form
    {
        private NodeCollection nodes;
        private int height;
        private int width;

        public Puzzle()
        {
            InitializeComponent();
        }

        private void Puzzle_Load(object sender, EventArgs e)
        {
            height = 0;
            width = 0;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                lblError.ResetText();
                System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex("^[0-9]+$");

                if(rgx.Matches(txtHeight.Text).Count == 0) throw new ArgumentException("Height is not a number");
                else if(rgx.Matches(txtWidth.Text).Count == 0) throw new ArgumentException("Width is not a number");

                height = int.Parse(txtHeight.Text);
                width = int.Parse(txtWidth.Text);

                if (height < 2 || height > 19) throw new ArgumentOutOfRangeException("Height must be greater than 1 and less than 20");
                else if(width < 2 || width > 19) throw new ArgumentOutOfRangeException("Width must be greater than 1 and less than 20");

                Form puzzleForm = new Form();
                puzzleForm.Size = new Size(width * 35 + 120, height * 35 + 120);
                puzzleForm.Name = "frmPuzzle";
                puzzleForm.Text = (height * width) - 1 + " Piece Puzzle";
                puzzleForm.Visible = true;

                Panel pane = new Panel();
                pane.Size = new Size(width * 35 + 5, height * 35 + 5);
                pane.Location = new Point(20, 10);
                pane.Visible = true;
                pane.BorderStyle = BorderStyle.Fixed3D;
                puzzleForm.Controls.Add(pane);

                nodes = new NodeCollection(width, height, 35);
                int counter = 1;

                for (int gridHeight = 0; gridHeight < height; gridHeight++)
                    for (int gridWidth = 0; gridWidth < width; gridWidth++)
                    {
                        if (gridWidth + gridHeight < (width - 1) + (height - 1))
                        {
                            Button newButton = new Button();
                            newButton.Name = "btnNode_" + counter;
                            newButton.Text = counter.ToString();
                            newButton.Location = new Point(gridWidth * 35, gridHeight * 35);
                            newButton.Size = new Size(35, 35);
                            newButton.Click += new EventHandler(movePiece);
                            newButton.Visible = true;
                            pane.Controls.Add(newButton);
                            nodes.AddNode(counter++, gridWidth, gridHeight, newButton.Location.X, newButton.Location.Y);
                        }
                        else nodes.AddNode(0, gridWidth, gridHeight, gridWidth * 35, gridHeight * 35);
                    }

                Button shuffleButton = new Button();
                shuffleButton.Name = "btnShuffle";
                shuffleButton.Text = "Shuffle";
                shuffleButton.Location = new Point(20, puzzleForm.Size.Height - 90);
                shuffleButton.Click += new EventHandler(shuffle);
                shuffleButton.Visible = true;
                puzzleForm.Controls.Add(shuffleButton);
                nodes.linkNodes(width, height);
            }
            catch(ArgumentOutOfRangeException aoorex) { lblError.Text = aoorex.ParamName; }
            catch(ArgumentException aex) { lblError.Text = aex.Message; }
        }

        private void shuffle(object sender, EventArgs e)
        {
            Node emptyNode = nodes.getEmptyNode();
            int selector;
            int limit;
            int gridArea = width * height;
            Random rnd = new Random();
            Control[] buttons = new Control[width * height - 1];
            (((sender as Button).Parent as Form).GetNextControl(new Panel(), true) as Panel).Controls.CopyTo(buttons, 0);

            if (gridArea < 20)
                limit = (int)Math.Pow(gridArea, 3);
            else if (gridArea < 150)
                limit = (int)Math.Pow(gridArea, 2);
            else limit = (int)Math.Pow(gridArea, 1.5);

            for(int i = 0; i < limit; i++)
            {
                selector = rnd.Next(1, 5);

                if (selector == 1)
                {
                    if (emptyNode.south != null)
                        foreach (Button btn in buttons)
                            if (btn.Text == emptyNode.south.Value.ToString())
                            {
                                btn.PerformClick();
                                break;
                            }
                }
                else if (selector == 2)
                {
                    if (emptyNode.east != null)
                        foreach (Button btn in buttons)
                            if (btn.Text == emptyNode.east.Value.ToString())
                            {
                                btn.PerformClick();
                                break;
                            }
                }
                else if (selector == 3)
                {
                    if (emptyNode.north != null)
                        foreach (Button btn in buttons)
                            if (btn.Text == emptyNode.north.Value.ToString())
                            {
                                btn.PerformClick();
                                break;
                            }
                }
                else
                {
                    if (emptyNode.west != null)
                        foreach (Button btn in buttons)
                            if (btn.Text == emptyNode.west.Value.ToString())
                            {
                                btn.PerformClick();
                                break;
                            }
                }
            }
        }

        private void movePiece(object sender, EventArgs e)
        {
            Node btnNode = new Node();
            bool update = false;

            foreach(Node node in nodes)
                if ((sender as Button).Text == node.Value.ToString())
                    btnNode = node;

            if (btnNode.south != null && btnNode.south.Value == 0)
            {
                nodes.moveNodes(btnNode, btnNode.south, 1);
                update = true;
            }
            else if (btnNode.east != null && btnNode.east.Value == 0)
            {
                nodes.moveNodes(btnNode, btnNode.east, 2);
                update = true;
            }
            else if (btnNode.north != null && btnNode.north.Value == 0)
            {
                nodes.moveNodes(btnNode, btnNode.north, 3);
                update = true;
            }
            else if (btnNode.west != null && btnNode.west.Value == 0)
            {
                nodes.moveNodes(btnNode, btnNode.west, 4);
                update = true;
            }

            if(update) updateBoard(((sender as Button).Parent as Panel), (sender as Button), btnNode, nodes.Width * nodes.Height);
        }

        private void updateBoard(Panel pane, Button btn, Node node, int buttonAmount)
        { btn.Location = new Point(node.HorizontalPosition, node.VerticalPosition); }
    }
}