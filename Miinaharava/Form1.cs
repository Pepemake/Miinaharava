using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Miinaharava
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Game game = new Game(9, 9);
            game.Show();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Game game = new Game(16, 16);
            game.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Game game = new Game(30, 16); //Sarake, Rivi
            game.ShowDialog();
            this.Activate();
        }
    }
}
