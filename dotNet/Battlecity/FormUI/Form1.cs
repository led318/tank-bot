using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using API;
using API.Components;
using FormUI.Controls;
using FormUI.FieldItems;
using FormUI.FieldObjects;
using FormUI.Infrastructure;
using FormUI.Logic;
using FormUI.Predictions;
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

        private Stopwatch _stopWatch = new Stopwatch();

        public MyPictureBox[,] _field = new MyPictureBox[Constants.FieldWidth, Constants.FieldHeight];

        public Form1()
        {
            _serverURL = ConfigurationManager.AppSettings["serverURL"];
            _testServerURL = ConfigurationManager.AppSettings["testServerURL"];
            _isProd = bool.Parse(ConfigurationManager.AppSettings["isProd"]);

            InitializeComponent();
            //Task.Run(InitSettings);

            InitFieldPanel();
            InitPredictionCheckboxes();

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
            catch (Exception)
            {

            }
        }

        private void InitPredictionCheckboxes()
        {
            var predictionTypes = Enum.GetValues(typeof(PredictionType));

            var heightStep = 20;
            var height = 0;

            foreach (PredictionType type in predictionTypes)
            {
                var attribute = type.GetAttributeOfType<IsDefaultSelectedAttribute>();

                var checkbox = new CheckBox();
                checkbox.Text = type.ToString();
                checkbox.Width = 300;
                checkbox.Top = height;
                checkbox.Checked = attribute?.Selected ?? false;
                checkbox.ForeColor = BasePrediction.GetBorderColor(type) ?? Color.Black;

                height += heightStep;

                Controls.Add(checkbox);

                PredictionSettings.Init(type, checkbox);
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



        delegate string SetBoardCallback(Board board); //todo: change return type to string or Element[]

        private string SetBoard(Board board)
        {
            var result = string.Empty;

            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.label1.InvokeRequired)
            {
                SetBoardCallback d = new SetBoardCallback(SetBoard);
                var invokeResult = this.Invoke(d, new object[] { board });
                if (invokeResult is string invokeResultStr)
                {
                    result = invokeResultStr;
                }
            }
            else
            {
                _stopWatch.Restart();

                //this.label1.Text = board.ToString();
                State.SetThisRound(new Round(board));
                
                if (State.GameIsRunning)
                {
                    //todo: perform calculations
                    var task = new Task(CalculationLogic.PerformCalculations);
                    task.Start();
                    task.Wait();

                    for (var i = 0; i < Constants.FieldWidth; i++)
                    {
                        for (var j = 0; j < Constants.FieldHeight; j++)
                        {
                            _field[i, j].Change();
                        }
                    }

                    if (State.IsMyShotThisRound)
                    {
                        State.ThisRound.MyTank.Shot();
                        result = $"{Direction.Act}";
                    }
                }

                _stopWatch.Stop();
                label1.Text = $"{_stopWatch.ElapsedMilliseconds}ms";
                label2.Text = State.ThisRound.MyTank == null 
                    ? string.Empty 
                    : $"Me: {State.ThisRound.MyTank.Point}. Predictions: {Field.GetMyMovePredictionsCount()}";
            }

            return result;
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
