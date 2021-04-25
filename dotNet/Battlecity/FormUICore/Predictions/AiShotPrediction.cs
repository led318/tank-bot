using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormUICore.Predictions;

namespace FormUI.Predictions
{
    public class AiShotPrediction : BasePrediction
    {
        public override PredictionType Type => PredictionType.AiShot;
    }
}
