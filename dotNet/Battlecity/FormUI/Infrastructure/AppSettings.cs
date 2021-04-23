using System.Configuration;

namespace FormUI.Infrastructure
{
    public static class AppSettings
    {
        public static int PredictionDepth { get; set; }
        public static int MyShotPredictionDepth { get; set; }
        public static int MyMovePredictionDepth { get; set; }
        public static int EnemyTankMovePredictionDepth { get; set; }
        public static int EnemyTankShotPredictionDepth { get; set; }
        public static bool DrawBaseBorders { get; set; }

        static AppSettings()
        {
            PredictionDepth = int.Parse(ConfigurationManager.AppSettings["predictionDepth"]);
            MyShotPredictionDepth = int.Parse(ConfigurationManager.AppSettings["myShotPredictionDepth"]);
            MyMovePredictionDepth = int.Parse(ConfigurationManager.AppSettings["myMovePredictionDepth"]);

            DrawBaseBorders = bool.Parse(ConfigurationManager.AppSettings["drawBaseBorders"]);
            EnemyTankMovePredictionDepth = int.Parse(ConfigurationManager.AppSettings["enemyTankMovePredictionDepth"]);
            EnemyTankShotPredictionDepth = int.Parse(ConfigurationManager.AppSettings["enemyTankShotPredictionDepth"]);
        }
    }
}
