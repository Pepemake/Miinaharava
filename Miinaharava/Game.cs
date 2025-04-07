using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Miinaharava
{
    public partial class Game : Form
    {
        private int _rows;
        private int _cols;
        private PictureBox _statusPictureBox;

        public Game(int rows, int cols)
        {
            InitializeComponent();

            int buttonSize = 30;
            int spacing = 20;
            int padding = 50;
            int borderPadding = 20;
            int titleBarHeight = 20;

            int formWidth = (rows * (buttonSize + spacing)) + (3 * padding) + borderPadding;
            int formHeight = (cols * (buttonSize + spacing)) + (3 * padding) + titleBarHeight;

            this.Size = new Size(formWidth, formHeight);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MinimizeBox = true;
            this.MaximizeBox = true;
            this.StartPosition = FormStartPosition.CenterScreen;

            _statusPictureBox = new PictureBox();
            _statusPictureBox.Size = new Size(50, 50);  
            _statusPictureBox.Location = new Point(this.Width - 80, 10);  
            _statusPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;  
            this.Controls.Add(_statusPictureBox);

            string initialImagePath = Path.Combine(Application.StartupPath, "Images", "MineAlive.png");
            if (File.Exists(initialImagePath))
            {
                _statusPictureBox.Image = new Bitmap(initialImagePath);
            }


            _rows = rows;
            _cols = cols;

            Mine mineGame = new Mine(_rows, _cols, this); 
            mineGame.buttonMaker(); 
        }
        public void UpdateGameStatus(bool isAlive)
        {
            
            string imageName = isAlive ? "MineAlive.png" : "MineDead.png";
            string imagePath = Path.Combine(Application.StartupPath, "Images", imageName);

            try
            {
                if (File.Exists(imagePath))
                {
                    Bitmap statusImage = new Bitmap(imagePath);
                    _statusPictureBox.Image = statusImage;
                }
                else
                {
                    MessageBox.Show($"Image file '{imageName}' not found at the specified location: {imagePath}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading image: " + ex.Message);
            }
        }
    }
}



