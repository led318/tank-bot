using FormUI.Predictions;

namespace FormUICore.Predictions
{
    public class MyKillPrediction : BasePrediction
    {
        public override PredictionType Type => PredictionType.MyKill;

        public BasePrediction TargetMove { get; set; }
        public MyShotPrediction MyShot { get; set; }
    }
}
