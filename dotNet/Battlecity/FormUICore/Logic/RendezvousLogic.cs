using System.Collections.Generic;
using System.Linq;
using FormUI.FieldItems.Tank;
using FormUICore.Infrastructure;
using FormUICore.Predictions;
// ReSharper disable InconsistentNaming

namespace FormUICore.Logic
{
    public static class RendezvousLogic
    {
        private const int _rendezvousMin = 3;
        private const int _rendezvousMax = 4;

        public static List<BasePrediction> FilterOutRendezvousPredictions(List<BasePrediction> predictions)
        {
            PopulatePredictionsIsRendezvous(predictions);
            
            var result = predictions.Where(x => !x.IsRendezvous).ToList();

            return result;
        }

        private static void PopulatePredictionsIsRendezvous(List<BasePrediction> predictions)
        {
            if (!State.HasPrevRound)
                return;

            if (State.PrevRound.CurrentMoveSelectedPrediction == null)
                return;

            var prevPrediction = State.PrevRound.CurrentMoveSelectedPrediction;
            if (prevPrediction.Commands.Count < _rendezvousMin || prevPrediction.Commands.Count > _rendezvousMax)
                return;

            var prevCommandText = NormalizeCommandText(prevPrediction.CommandsText);

            var rendezvousPrediction = predictions.FirstOrDefault(x =>
                CheckIfPredictionIsRendezvous(x, prevCommandText));

            if (rendezvousPrediction?.Item != null)
            {
                if (rendezvousPrediction.Item is AiTank rendezvousTarget)
                {
                    var sameTargetPredictions = predictions.Where(x => x.Item.Point == rendezvousTarget.Point).ToList();
                    foreach (var sameTargetPrediction in sameTargetPredictions)
                        sameTargetPrediction.IsRendezvous = true;
                }
            }
        }

        private static bool CheckIfPredictionIsRendezvous(BasePrediction prediction, string prevCommandText)
        {
            if (prediction.Commands.Count < _rendezvousMin || prediction.Commands.Count > _rendezvousMax)
                return false;

            var thisCommandText = NormalizeCommandText(prediction.CommandsText);
            return prevCommandText == thisCommandText;
        }

        private static string NormalizeCommandText(string commandText)
        {
            return commandText.Replace("|", "_").Replace(",", "_");
        }
    }
}
