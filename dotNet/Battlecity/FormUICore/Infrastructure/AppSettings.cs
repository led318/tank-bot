using System;
using System.Configuration;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace FormUICore.Infrastructure
{
    public static class AppSettings
    {
        private const string _urlArtem = "https://epam-botchallenge.com/codenjoy-contest/board/player/vsw86l76vx5va61b7ju4?code=8749211683513820687";
        private const string _urlRuslana = "https://epam-botchallenge.com/codenjoy-contest/board/player/s7aq92okytrsnnnrb2lx?code=5922020647759530101";

        public static bool IsProd { get; set; }
        public static string ServerURL { get; set; }
        public static string TestServerURL { get; set; }
        public static User User { get; set; }

        public static int PredictionDepth { get; set; }
        public static int StuckAiPredictionDepth { get; set; }
        public static int MyShotPredictionDepth { get; set; }
        public static int MyMovePredictionDepth { get; set; }
        public static int DangerCellPredictionDepth { get; set; }
        public static int EnemyTankMovePredictionDepth { get; set; }
        public static int EnemyTankShotPredictionDepth { get; set; }
        public static bool EnableEnemyTankNotForwardNearestMoves { get; set; }

        public static int IgnoreEnemyMoveDepthMoreThan { get; set; }
        public static int StuckEnemyPredictionDepth { get; set; }
        public static bool DrawBaseBorders { get; set; }
        public static bool IgnorePrizeAiTanks { get; set; }
        public static bool ChooseKillOnlyByCommandsLength { get; set; }
        public static bool StoreMySelectedKillPredictions { get; set; }
        public static bool IceIsDangerousToStep { get; set; }
        public static bool AiShotCellIsDangerousToStep { get; set; }
        public static bool CanShotThroughPrize { get; set; }

        static AppSettings()
        {
            IsProd = bool.Parse(ConfigurationManager.AppSettings["isProd"]);
            TestServerURL = ConfigurationManager.AppSettings["testServerURL"];
            User = (User) int.Parse(ConfigurationManager.AppSettings["user"]);

            switch (User)
            {
                case User.Artem:
                    ServerURL = _urlArtem;
                    break;
                case User.Ruslana:
                    ServerURL = _urlRuslana;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }




            PredictionDepth = int.Parse(ConfigurationManager.AppSettings["predictionDepth"]);
            StuckAiPredictionDepth = int.Parse(ConfigurationManager.AppSettings["stuckAiPredictionDepth"]);
            MyShotPredictionDepth = int.Parse(ConfigurationManager.AppSettings["myShotPredictionDepth"]);
            MyMovePredictionDepth = int.Parse(ConfigurationManager.AppSettings["myMovePredictionDepth"]);
            DangerCellPredictionDepth = int.Parse(ConfigurationManager.AppSettings["dangerCellPredictionDepth"]);

            DrawBaseBorders = bool.Parse(ConfigurationManager.AppSettings["drawBaseBorders"]);
            IgnorePrizeAiTanks = bool.Parse(ConfigurationManager.AppSettings["ignorePrizeAiTanks"]);
            ChooseKillOnlyByCommandsLength = bool.Parse(ConfigurationManager.AppSettings["chooseKillOnlyByCommandsLength"]);

            EnemyTankMovePredictionDepth = int.Parse(ConfigurationManager.AppSettings["enemyTankMovePredictionDepth"]);
            EnemyTankShotPredictionDepth = int.Parse(ConfigurationManager.AppSettings["enemyTankShotPredictionDepth"]);
            EnableEnemyTankNotForwardNearestMoves = bool.Parse(ConfigurationManager.AppSettings["enableEnemyTankNotForwardNearestMoves"]);

            IgnoreEnemyMoveDepthMoreThan = int.Parse(ConfigurationManager.AppSettings["ignoreEnemyMoveDepthMoreThan"]);
            StuckEnemyPredictionDepth = int.Parse(ConfigurationManager.AppSettings["stuckEnemyPredictionDepth"]);
            StoreMySelectedKillPredictions = bool.Parse(ConfigurationManager.AppSettings["storeMySelectedKillPredictions"]);
            IceIsDangerousToStep = bool.Parse(ConfigurationManager.AppSettings["iceIsDangerousToStep"]);
            AiShotCellIsDangerousToStep = bool.Parse(ConfigurationManager.AppSettings["aiShotCellIsDangerousToStep"]);
            CanShotThroughPrize = bool.Parse(ConfigurationManager.AppSettings["canShotThroughPrize"]);
        }
    }

    public enum User
    {
        Artem = 1,
        Ruslana = 2
    }
}
