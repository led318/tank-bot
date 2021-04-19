using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Demo;

namespace FormUI
{
    public partial class Form1 : Form
    {
        static string serverURL = "https://epam-botchallenge.com/codenjoy-contest/board/player/vsw86l76vx5va61b7ju4?code=8749211683513820687";

        public Form1()
        {
            InitializeComponent();

            // Creating custom AI client
            var bot = new YourSolver(serverURL);
            bot.RoundCallbackHandler += SetText;

            // Starting thread with playing game
            Task.Run(bot.Play);

            
        }

        delegate void SetTextCallback(string text);

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.label1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.label1.Text = text;
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
