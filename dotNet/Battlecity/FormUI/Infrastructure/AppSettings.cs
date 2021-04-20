using System.Configuration;

namespace FormUI.Infrastructure
{
    public static class AppSettings
    {
        public static int PredictionDepth { get; set; }
        public static int EnemyTanksPredictionDepth { get; set; }
        public static bool DrawBaseBorders { get; set; }

        static AppSettings()
        {
            PredictionDepth = int.Parse(ConfigurationManager.AppSettings["predictionDepth"]);
            DrawBaseBorders = bool.Parse(ConfigurationManager.AppSettings["drawBaseBorders"]);
            EnemyTanksPredictionDepth = int.Parse(ConfigurationManager.AppSettings["enemyTanksPredictionDepth"]);
        }
    }
}
