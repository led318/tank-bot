﻿namespace FormUICore.Predictions
{
    public class DangerCellPrediction : BasePrediction
    {
        public override PredictionType Type => PredictionType.DangerCell;

        public bool IsCritical { get; set; }
    }
}
