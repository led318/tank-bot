using System.Configuration;

namespace FormUI.Infrastructure
{
    public static class AppSettings
    {
        public static int PredictionDepth { get; set; }
        public static int StuckAiPredictionDepth { get; set; }
        public static int MyShotPredictionDepth { get; set; }
        public static int MyMovePredictionDepth { get; set; }
        public static int EnemyTankMovePredictionDepth { get; set; }
        public static int EnemyTankShotPredictionDepth { get; set; }
        public static bool EnableEnemyTankNotForwardNearestMoves { get; set; }

        public static int IgnoreEnemyMoveDepthMoreThan { get; set; }
        public static int StuckEnemyPredictionDepth { get; set; }
        public static bool DrawBaseBorders { get; set; }
        public static bool IgnorePrizeAiTanks { get; set; }
        public static bool ChooseKillOnlyByCommandsLength { get; set; }
        public static bool StoreMySelectedKillPredictions { get; set; }

        static AppSettings()
        {
            PredictionDepth = int.Parse(ConfigurationManager.AppSettings["predictionDepth"]);
            StuckAiPredictionDepth = int.Parse(ConfigurationManager.AppSettings["stuckAiPredictionDepth"]);
            MyShotPredictionDepth = int.Parse(ConfigurationManager.AppSettings["myShotPredictionDepth"]);
            MyMovePredictionDepth = int.Parse(ConfigurationManager.AppSettings["myMovePredictionDepth"]);

            DrawBaseBorders = bool.Parse(ConfigurationManager.AppSettings["drawBaseBorders"]);
            IgnorePrizeAiTanks = bool.Parse(ConfigurationManager.AppSettings["ignorePrizeAiTanks"]);
            ChooseKillOnlyByCommandsLength = bool.Parse(ConfigurationManager.AppSettings["chooseKillOnlyByCommandsLength"]);

            EnemyTankMovePredictionDepth = int.Parse(ConfigurationManager.AppSettings["enemyTankMovePredictionDepth"]);
            EnemyTankShotPredictionDepth = int.Parse(ConfigurationManager.AppSettings["enemyTankShotPredictionDepth"]);
            EnableEnemyTankNotForwardNearestMoves = bool.Parse(ConfigurationManager.AppSettings["enableEnemyTankNotForwardNearestMoves"]);

            IgnoreEnemyMoveDepthMoreThan = int.Parse(ConfigurationManager.AppSettings["ignoreEnemyMoveDepthMoreThan"]);
            StuckEnemyPredictionDepth = int.Parse(ConfigurationManager.AppSettings["stuckEnemyPredictionDepth"]);
            StoreMySelectedKillPredictions = bool.Parse(ConfigurationManager.AppSettings["storeMySelectedKillPredictions"]);
        }
    }
}
