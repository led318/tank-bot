using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using API;
using FormUI.Controls;
using FormUI.FieldItems;
using FormUI.FieldObjects;
using FormUI.Infrastructure;
using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace FormUI
{
    public partial class Form1 : Form
    {
        private static string _serverURL;
        private static string _testServerURL;
        private static bool _isProd;
        private static string _settingsURL = "https://epam-botchallenge.com/codenjoy-balancer/rest/game/settings/get";

        public MyPictureBox[,] _field = new MyPictureBox[Constants.FieldWidth, Constants.FieldHeight];

        public Form1()
        {
            _serverURL = ConfigurationManager.AppSettings["serverURL"];
            _testServerURL = ConfigurationManager.AppSettings["testServerURL"];
            _isProd = bool.Parse(ConfigurationManager.AppSettings["isProd"]);

            InitializeComponent();
            InitSettings();

            InitFieldPanel();

            // Creating custom AI client
            var bot = new YourSolver(_isProd ? _serverURL : _testServerURL);
            bot.RoundCallbackHandler += SetBoard;

            // Starting thread with playing game
            Task.Run(bot.Play);

            
        }

        private void InitSettings()
        {
            try
            {
                var httpClient = new HttpClient();
                var httpResponse = httpClient.GetAsync(_settingsURL).Result;
                httpResponse.EnsureSuccessStatusCode();

                if (httpResponse.Content is object && httpResponse.Content.Headers.ContentType.MediaType == "application/json")
                {
                    var contentStream = httpResponse.Content.ReadAsStream();

                    using var streamReader = new StreamReader(contentStream);
                    using var jsonReader = new JsonTextReader(streamReader);

                    var serializer = new JsonSerializer();

                    try
                    {
                        var response = serializer.Deserialize<SettingsModel[]>(jsonReader);
                        if (response.Any())
                        {
                            Settings.Get = response[0];
                        }
                    }
                    catch (JsonReaderException)
                    {
                        
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        private void InitFieldPanel()
        {
            fieldPanel.Width = Constants.FieldWidth * Constants.CellSize;
            fieldPanel.Height = Constants.FieldHeight * Constants.CellSize;

            for (var i = 0; i < Constants.FieldWidth; i++)
            {
                for (var j = 0; j < Constants.FieldHeight; j++)
                {
                    var pictureBox = new MyPictureBox(Field.Cells[i, j]);
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
                State.SetCurrentRound(new Round(board));

                for (var i = 0; i < Constants.FieldWidth; i++)
                {
                    for (var j = 0; j < Constants.FieldHeight; j++)
                    {
                        _field[i, j].Change();
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
