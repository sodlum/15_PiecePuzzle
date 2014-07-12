using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Puzzle_Game
{
    public partial class Puzzle : Form
    {
        private PuzzleForm frmPuzzle;
        private NodeCollection nodes;

        /// <summary>
        /// Constructor for puzzle generator
        /// </summary>
        public Puzzle() { InitializeComponent(); }

        /// <summary>
        /// Method stub for puzzle load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Puzzle_Load(object sender, EventArgs e) { }

        /// <summary>
        /// Validates input on main form and creates a grid based
        /// slidng puzzle game with supplied dimensions.
        /// </summary>
        /// <param name="sender">Object representing the Generate Puzzle button</param>
        /// <param name="e">Event Arguments for Button.Click event</param>
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            int width = 0, height = 0;

            try
            {
                if (!int.TryParse(txtHeight.Text, out height))
                    throw new ArgumentException("Height must be a number");
                else if (height < 2 || height > 19)
                    throw new ArgumentOutOfRangeException("Height must be within range 2 - 19 inclusive");

                if (!int.TryParse(txtWidth.Text, out width))
                    throw new ArgumentException("Width must be a number");
                else if (width < 2 || width > 19)
                    throw new ArgumentOutOfRangeException("Width must be within range 2 - 19 inclusive");

                lblError.Text = "";
            
                nodes = new NodeCollection(width, height);
                frmPuzzle = new PuzzleForm(ref nodes, width, height);
            }
            catch (ArgumentOutOfRangeException orex) { lblError.Text = orex.ParamName; }
            catch (ArgumentException aex) { lblError.Text = aex.Message; }
        }
    }
}