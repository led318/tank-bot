using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormUICore.Predictions;

namespace FormUI.Predictions
{
    public class MyShotPrediction : BasePrediction
    {
        public override PredictionType Type => PredictionType.MyShot;
    }
}
