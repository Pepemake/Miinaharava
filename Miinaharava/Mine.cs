using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Miinaharava
{
    internal class Mine  // Game Logic and Mine Class are combined, 07.04.25 Problem occurs as modules use eachother so commonly, it is hard to switch parts -> game Logic
                         // Will fix later?   
    {
        private int _rivit; 
        private int _sarakkeet;  
        private bool[,] _mineField;  
        private bool[,] _flaggedField;  
        private Game _gameForm;
        private bool _isAlive;
        private Button[,] _buttonGrid;


        public Mine(int rows, int cols, Game gameForm)
        {
            _rivit = rows;
            _sarakkeet = cols;
            _gameForm = gameForm;  
            _isAlive = true;
            _flaggedField = new bool[_rivit, _sarakkeet];  
        }

        public void buttonMaker()
        {
            _buttonGrid = new Button[_rivit, _sarakkeet];
            int buttonleveys = 50;
            int buttonkorkeus = 50;
            int väli = 1;

            _mineField = new bool[_rivit, _sarakkeet];
            PlaceMines(_rivit * _sarakkeet / 6); 

            int rowxposition = 30;
            int columnyposition = 30;

         
            for (int rivi = 0; rivi < _rivit; rivi++)
            {
                for (int sarake = 0; sarake < _sarakkeet; sarake++)
                {
                    Button vertical = new Button();
                    vertical.Text = string.Empty;
                    vertical.Location = new Point(rowxposition + rivi * (buttonleveys + väli),
                                                  columnyposition + sarake * (buttonkorkeus + väli));
                    vertical.Size = new Size(buttonleveys, buttonkorkeus);

                    vertical.Tag = $"{rivi},{sarake}";  
                    vertical.Click += Button_Click;
                    vertical.MouseDown += Button_MouseDown;
                    vertical.BackColor = Color.LavenderBlush;
                    _buttonGrid[rivi, sarake] = vertical;

                    _gameForm.Controls.Add(vertical);
                    

                }
            }

        }

        private void PlaceMines(int mineCount)
        {
            Random rand = new Random();
            int placedMines = 0;

            while (placedMines < mineCount)
            {
                int row = rand.Next(0, _rivit);
                int col = rand.Next(0, _sarakkeet);

                if (!_mineField[row, col])  
                {
                    _mineField[row, col] = true;
                    placedMines++;
                }
            }
        }


        private void Button_Click(object sender, EventArgs e) // GPT Modified
        {
            if (!_isAlive) return;  

            Button clickedButton = sender as Button;

            string[] parts = clickedButton.Tag.ToString().Split(',');
            int row = int.Parse(parts[0]);
            int col = int.Parse(parts[1]);



            if (clickedButton == null) return;
            

            if (_flaggedField[row, col]) return;  

            if (_mineField[row, col])
            {
                _isAlive = false;
                _gameForm.UpdateGameStatus(false);  
                string imagePath = Path.Combine(Application.StartupPath, "Images", "MineMine.png");

                try
                {
                    if (File.Exists(imagePath))
                    {
                        Bitmap mineImage = new Bitmap(imagePath);
                        clickedButton.BackgroundImage = mineImage;
                        clickedButton.BackgroundImageLayout = ImageLayout.Stretch;
                        clickedButton.BackColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }

                MessageBox.Show("Game Over!");  // Show the game over message
                _gameForm.Close();  // Close the game window
                return;
            }        
            int nearbyMines = CountNearbyMines(row, col);
            if (nearbyMines > 0)
            {
                ShowNumberImage(clickedButton, nearbyMines);
            }
            else
            {
                bool[,] visited = new bool[_rivit, _sarakkeet];
                FloodFill(row, col, _buttonGrid, visited);
            }

            clickedButton.Enabled = false; // Disable clicked button
            clickedButton.BackColor = Color.Gainsboro;

            
            CheckForVictory();
        }

        private void ShowNumberImage(Button button, int number)
        {
            try
            {
                
                string imageFileName = "Mine" + number + ".png";
                string imagePath = Path.Combine(Application.StartupPath, "Images", imageFileName);

                
                if (File.Exists(imagePath))
                {
                    Bitmap mineImage = new Bitmap(imagePath);
                    button.BackgroundImage = mineImage;
                    button.BackgroundImageLayout = ImageLayout.Stretch;
                }
                else
                {
                    button.Text = number.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                button.Text = number.ToString();
            }
        }

        




        private void Button_MouseDown(object sender, MouseEventArgs e) // Flag Mechanism
        {
            if (e.Button == MouseButtons.Right)
            {
                Button clickedButton = sender as Button;
                if (clickedButton == null) return;

                string[] position = clickedButton.Tag.ToString().Split(',');
                int row = int.Parse(position[0]);
                int col = int.Parse(position[1]);

               
                _flaggedField[row, col] = !_flaggedField[row, col];
                string flagImagePath = Path.Combine(Application.StartupPath, "Images", "MineFlag.png");

                try
                {
                    AvaaMine(clickedButton, row, col, flagImagePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
        }

        private void AvaaMine(Button clickedButton, int row, int col, string flagImagePath) // Remove Flag 
        {
            if (_flaggedField[row, col])
            {
                if (File.Exists(flagImagePath))
                {
                    Bitmap flagImage = new Bitmap(flagImagePath);
                    clickedButton.BackgroundImage = flagImage;
                    clickedButton.BackgroundImageLayout = ImageLayout.Stretch;
                }
            }
            else
            {
                clickedButton.BackgroundImage = null;  
            }
        }

        private int CountNearbyMines(int row, int col) // Tile Number Selector
        {
            int count = 0;

            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;

                    int nr = row + dr;
                    int nc = col + dc;

                    if (nr >= 0 && nr < _rivit && nc >= 0 && nc < _sarakkeet)
                    {
                        if (_mineField[nr, nc])
                        {
                            count++;
                        }
                    }
                }
            }

            return count;
        }
        private void FloodFill(int row, int col, Button[,] buttonGrid, bool[,] visited)
        {
            // Check boundaries
            if (row < 0 || row >= _rivit || col < 0 || col >= _sarakkeet)
                return;

            // Already visited
            if (visited[row, col])
                return;

            visited[row, col] = true;

           
            if (_mineField[row, col])
                return;

            int nearbyMines = CountNearbyMines(row, col);

            if (nearbyMines > 0)
            {
                Button btn = buttonGrid[row, col];
                btn.Enabled = false;  // Disable the button
                btn.BackColor = Color.Gainsboro; 
                ShowNumberImage(btn, nearbyMines);

                return;  
            }

            Button btnEmpty = buttonGrid[row, col];
            btnEmpty.Enabled = false;
            btnEmpty.BackColor = Color.Gainsboro;

            // Recurse in 8 directions (all adjacent cells)
            FloodFill(row - 1, col, buttonGrid, visited);
            FloodFill(row + 1, col, buttonGrid, visited);
            FloodFill(row, col - 1, buttonGrid, visited);
            FloodFill(row, col + 1, buttonGrid, visited);
            FloodFill(row - 1, col - 1, buttonGrid, visited);
            FloodFill(row - 1, col + 1, buttonGrid, visited);
            FloodFill(row + 1, col - 1, buttonGrid, visited);
            FloodFill(row + 1, col + 1, buttonGrid, visited);
        }



        private void CheckForVictory() // Wincondition
        {
            bool allMinesFlagged = true;
            bool noMistakes = true;

            for (int r = 0; r < _rivit; r++)
            {
                for (int c = 0; c < _sarakkeet; c++)
                {
                    if (_mineField[r, c] && !_flaggedField[r, c])
                    {
                        allMinesFlagged = false;
                    }
                    if (!_mineField[r, c] && _flaggedField[r, c])
                    {
                        noMistakes = false;
                    }
                }
            }

            if (allMinesFlagged && noMistakes)
            {
                MessageBox.Show("Sinä Voitit!");
                _gameForm.Close();
            }
        }
    }
}