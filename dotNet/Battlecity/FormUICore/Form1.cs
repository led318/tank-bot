﻿using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using API;
using FormUI;
using FormUI.FieldObjects;
using FormUI.Infrastructure;
using FormUICore.Controls;
using FormUICore.Infrastructure;
using FormUICore.Logic;
using FormUICore.Predictions;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
// ReSharper disable InconsistentNaming

namespace FormUICore
{
    public partial class Form1 : Form
    {
        private const string _settingsURL = "https://epam-botchallenge.com/codenjoy-balancer/rest/game/settings/get";

        private readonly Stopwatch _stopWatch = new Stopwatch();

        public MyPictureBox[,] _field = new MyPictureBox[Constants.FieldWidth, Constants.FieldHeight];

        public Form1()
        {
            var serverUrl = ConfigurationManager.AppSettings["serverURL"];
            var testServerUrl = ConfigurationManager.AppSettings["testServerURL"];
            var isProd = bool.Parse(ConfigurationManager.AppSettings["isProd"]);

            InitializeComponent();
            Task.Run(InitSettings);

            InitFieldPanel();
            InitPredictionCheckboxes();

            // Creating custom AI client
            var bot = new YourSolver(isProd ? serverUrl : testServerUrl);
            bot.RoundCallbackHandler += SetBoard;

            Logger.LoggerTextBox = logTextBox;

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
                    var contentStream = httpResponse.Content.ReadAsStreamAsync().Result;

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

        private delegate string SetBoardCallback(Board board);

        private string SetBoard(Board board)
        {
            var resultString = string.Empty;

            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.label1.InvokeRequired)
            {
                var d = new SetBoardCallback(SetBoard);

                try
                {
                    var invokeResult = this.Invoke(d, new object[] { board });
                    if (invokeResult is string invokeResultStr)
                    {
                        resultString = invokeResultStr;
                    }
                }
                catch (Exception)
                {
                }
            }
            else
            {
                if (checkBoxRunProcessing.Checked)
                {
                    _stopWatch.Restart();

                    //this.label1.Text = board.ToString();
                    State.SetThisRound(board);

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
                    else
                    {
                        //logTextBox.Clear();
                        Field.Reset(true);
                        TargetLog.Clear();
                    }

                    _stopWatch.Stop();
                    label1.Text = $"{_stopWatch.ElapsedMilliseconds}ms";
                    label2.Text = State.ThisRound.MyTank == null
                        ? string.Empty
                        : GetMyStateString();

                    //logTextBox.AppendText(Logger.GetLogAndClean());
                }
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
            if (State.ThisRound.CurrentMoveSelectedPrediction != null && State.ThisRound.CurrentMoveSelectedPrediction.Commands.Any())
            {
                killCommandsTrimmedStr = State.ThisRound.CurrentMoveSelectedPrediction?.CommandsText;
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
