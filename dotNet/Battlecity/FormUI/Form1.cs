using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Net.Http;
using System.Text;
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
            var resultString = string.Empty;

            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.label1.InvokeRequired)
            {
                SetBoardCallback d = new SetBoardCallback(SetBoard);

                try
                {
                    var invokeResult = this.Invoke(d, new object[] { board });
                    if (invokeResult is string invokeResultStr)
                    {
                        resultString = invokeResultStr;
                    }
                }
                catch (Exception e)
                {
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
                    Task.Run(CalculationLogic.PerformCalculations).Wait();

                    for (var i = 0; i < Constants.FieldWidth; i++)
                    {
                        for (var j = 0; j < Constants.FieldHeight; j++)
                        {
                            _field[i, j].Change();
                        }
                    }


                    //if (State.IsMyShotThisRound)
                    //{
                    //    State.ThisRound.MyTank.Shot();
                    //    result = $"{Direction.Act}";
                    //}

                    resultString = State.ThisRound.CurrentMoveCommandString;
                }

                _stopWatch.Stop();
                label1.Text = $"{_stopWatch.ElapsedMilliseconds}ms";
                label2.Text = State.ThisRound.MyTank == null 
                    ? string.Empty 
                    : GetMyStateString();
            }

            return resultString;
        }

        private string GetMyStateString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Me: {State.ThisRound.MyTank.Point}");
            sb.AppendLine($"MyMovePredictions: {Field.GetMyMovePredictionsCount()}");
            sb.AppendLine($"MyShotPredictions: {Field.GetMyShotPredictionsCount()}");
            sb.AppendLine($"CurrentMove: {State.ThisRound.CurrentMoveCommandString}");
            sb.AppendLine($"Kill: {State.ThisRound.CurrentMoveSelectedPrediction?.Point}");
            sb.AppendLine($"KillCommands: {Environment.NewLine}{FormatCommandString()}");

            if (State.ThisRound.IsInDeadZone)
            {
                sb.AppendLine("Dead zone!!!");
            }

            return sb.ToString();
        }

        private string FormatCommandString()
        {
            var killCommandsTrimmedStr = string.Empty;
            if (!string.IsNullOrEmpty(State.ThisRound.CurrentMoveSelectedPrediction?.CommandsText))
            {
                var breakLimit = 7;
                var commands = State.ThisRound.CurrentMoveSelectedPrediction.Commands;
                if (commands.Count > breakLimit)
                {
                    var currentIndex = 0;
                    while (currentIndex < commands.Count)
                    {
                        var subList = commands.Skip(currentIndex).Take(breakLimit).ToList();
                        killCommandsTrimmedStr += string.Join(",", subList) + Environment.NewLine;
                        currentIndex += breakLimit;
                    }
                }
                else
                {
                    killCommandsTrimmedStr = State.ThisRound.CurrentMoveSelectedPrediction?.CommandsText;
                }
            }

            return killCommandsTrimmedStr;
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
