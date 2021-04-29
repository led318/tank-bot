using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using API;
using API.Components;
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

        #region KeyControl
        private string _separator = ",";
        private readonly List<Direction> _keyControlDirections = new List<Direction>();

        public List<Direction> KeyControlCommand => _keyControlDirections.ToList();
        public string KeyControlCommandText => string.Join(_separator, _keyControlDirections);

        public bool IsKeyControl => _keyControlDirections.Any();
        #endregion KeyControl

        public Form1()
        {
            InitializeComponent();
            Activated += Form1_Activated;

            keyControlTextBox.KeyUp += Form1_KeyDown;

            Text = AppSettings.IsProd ? AppSettings.User.ToString() : "Test";

            Task.Run(InitSettings);


            InitFieldPanel();
            InitPredictionCheckboxes();

            // Creating custom AI client
            var bot = new YourSolver(AppSettings.IsProd ? AppSettings.ServerURL : AppSettings.TestServerURL);
            bot.RoundCallbackHandler += SetBoard;

            Logger.LoggerTextBox = logTextBox;

            ChangeFieldItem += Form1_ChangeFieldItem;

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
                        var response = serializer.Deserialize<ServerSettingsModel[]>(jsonReader);
                        if (response.Any())
                        {
                            ServerSettings.Settings = response[0];
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

        #region ChangeFieldItem
        event EventHandler ChangeFieldItem;
        class ChangeFieldItemEvent : EventArgs
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        private void Form1_ChangeFieldItem(object sender, EventArgs e)
        {
            var data = e as ChangeFieldItemEvent;
            if (InvokeRequired)
                BeginInvoke(new Action(() => { ChangeFieldItemAction(data); }));
            else
                ChangeFieldItemAction(data);
        }

        private void ChangeFieldItemAction(ChangeFieldItemEvent data)
        {
            _field[data.X, data.Y].Change();
        }
        #endregion ChangeFieldItem

        #region InitFieldItem

        private void InitFieldPanel()
        {
            InitFieldItem += Form1_InitFieldItem;

            fieldPanel.Width = Constants.FieldWidth * Constants.CellSize;
            fieldPanel.Height = Constants.FieldHeight * Constants.CellSize;

            Parallel.For(0, Constants.FieldWidth, (i) =>
            {
                Parallel.For(0, Constants.FieldHeight, (j) =>
                {
                    if (InitFieldItem != null)
                        InitFieldItem(null, new InitFieldItemEvent { X = i, Y = j });
                });
            });
        }

        event EventHandler InitFieldItem;
        class InitFieldItemEvent : EventArgs
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        private void Form1_InitFieldItem(object sender, EventArgs e)
        {
            var data = e as InitFieldItemEvent;
            if (InvokeRequired)
                BeginInvoke(new Action(() => { InitFieldItemAction(data); }));
            else
                InitFieldItemAction(data);
        }

        private readonly object _initFieldItemLock = new Object();

        private void InitFieldItemAction(InitFieldItemEvent data)
        {
            var i = data.X;
            var j = data.Y;

            var pictureBox = new MyPictureBox(Field.Cells[i, j]);
            pictureBox.Width = Constants.CellSize;
            pictureBox.Height = Constants.CellSize;

            //var imgStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FormUICore.Resources.NONE.png");
            //pictureBox.BackgroundImage = new Bitmap(imgStream);
            //_dictionary[element] = new Bitmap(imgStream);

            pictureBox.BackgroundImage = Image.FromFile("./Sprites/NONE.png");


            pictureBox.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox.Left = i * Constants.CellSize;
            pictureBox.Top = fieldPanel.Height - ((j + 1) * Constants.CellSize);

            _field[i, j] = pictureBox;

            lock (_initFieldItemLock)
            {
                fieldPanel.Controls.Add(pictureBox);
            }
        }

        #endregion InitFieldItem

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

                    State.SetThisRound(board);

                    if (State.GameIsRunning)
                    {
                        //todo: perform calculations
                        Task.Run(CalculationLogic.PerformCalculations).Wait();

                        for (var i = 0; i < Constants.FieldWidth; i++)
                        {
                            Parallel.For(0, Constants.FieldHeight, (j) =>
                            {
                                if (ChangeFieldItem != null)
                                    ChangeFieldItem(null, new ChangeFieldItemEvent { X = i, Y = j });
                            });
                        }

                        if (IsKeyControl)
                        {
                            State.ThisRound.CurrentMoveCommands.Clear();
                            State.ThisRound.CurrentMoveCommands.AddRange(KeyControlCommand);
                            ResetKeyControls();
                        }

                        resultString = State.ThisRound.CurrentMoveCommandString;
                    }
                    else
                    {
                        Field.Reset(true);
                        TargetLogLogic.Clear();
                    }

                    _stopWatch.Stop();
                    label1.Text = $"{_stopWatch.ElapsedMilliseconds}ms";
                    label2.Text = State.ThisRound.MyTank == null
                        ? string.Empty
                        : GetMyStateString();
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
        
        private void Form1_Activated(object sender, EventArgs e)
        {
            keyControlTextBox.Focus();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessKey(e.KeyCode);
            keyControlTextBox.Text = KeyControlCommandText;
        }

        public void ResetKeyControls()
        {
            _keyControlDirections.Clear();
        }

        private void ProcessKey(Keys key)
        {
            if (IsKeyControl)
                ResetKeyControls();

            switch (key)
            {
                case Keys.Left:
                    _keyControlDirections.Add(Direction.Left);
                    break;

                case Keys.Right:
                    _keyControlDirections.Add(Direction.Right);
                    break;

                case Keys.Up:
                    _keyControlDirections.Add(Direction.Up);
                    break;

                case Keys.Down:
                    _keyControlDirections.Add(Direction.Down);
                    break;

                case Keys.Space:
                    _keyControlDirections.Add(Direction.Act);
                    break;
            }
        }
    }
}
