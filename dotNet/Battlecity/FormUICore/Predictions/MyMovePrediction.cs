using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormUICore.Predictions;

namespace FormUI.Predictions
{
    public class MyMovePrediction : BasePrediction
    {
        public override PredictionType Type => PredictionType.MyMove;
    }
}
