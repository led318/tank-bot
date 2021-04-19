using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using API;
using FormUI.FieldItems;
using FormUI.Infrastructure;

namespace FormUI
{
    public partial class Form1 : Form
    {
        static string serverURL = "https://epam-botchallenge.com/codenjoy-contest/board/player/vsw86l76vx5va61b7ju4?code=8749211683513820687";

        public PictureBox[,] _field = new PictureBox[Constants.FieldWidth, Constants.FieldHeight];
        public Image[,] _fieldImages = new Image[Constants.FieldWidth, Constants.FieldHeight];

        public Form1()
        {
            InitializeComponent();
            InitFieldPanel();

            // Creating custom AI client
            var bot = new YourSolver(serverURL);
            bot.RoundCallbackHandler += SetBoard;

            // Starting thread with playing game
            Task.Run(bot.Play);

            
        }

        private void InitFieldPanel()
        {
            fieldPanel.Width = Constants.FieldWidth * Constants.CellSize;
            fieldPanel.Height = Constants.FieldHeight * Constants.CellSize;

            for (var i = 0; i < Constants.FieldWidth; i++)
            {
                for (var j = 0; j < Constants.FieldHeight; j++)
                {
                    var pictureBox = new PictureBox();
                    pictureBox.Width = Constants.CellSize;
                    pictureBox.Height = Constants.CellSize;
                    pictureBox.BackgroundImage = Image.FromFile("./Sprites/NONE.png");
                    pictureBox.BackgroundImageLayout = ImageLayout.Stretch;
                    pictureBox.Left = i * Constants.CellSize;
                    pictureBox.Top = fieldPanel.Height - ((j + 1) * Constants.CellSize);

                    _field[i, j] = pictureBox;
                    fieldPanel.Controls.Add(pictureBox);
                }
            }
        }



        delegate void SetBoardCallback(Board board); //todo: change return type to string or Element[]

        private void SetBoard(Board board)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.label1.InvokeRequired)
            {
                SetBoardCallback d = new SetBoardCallback(SetBoard);
                this.Invoke(d, new object[] { board });
            }
            else
            {
                this.label1.Text = board.ToString();
                var round = new Round(board);

                for (var i = 0; i < Constants.FieldWidth; i++)
                {
                    for (var j = 0; j < Constants.FieldHeight; j++)
                    {
                        if (_field[i, j].BackgroundImage != round.Items[i, j].Image)
                        {
                            _field[i, j].BackgroundImage = round.Items[i, j].Image;
                            _field[i, j].Refresh();
                        }
                    }
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
