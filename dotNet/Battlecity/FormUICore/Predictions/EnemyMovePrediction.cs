using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormUICore.Predictions;

namespace FormUI.Predictions
{
    public class EnemyMovePrediction : BasePrediction
    {
        public override PredictionType Type => PredictionType.EnemyMove;
    }
}
