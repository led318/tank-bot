using System.Linq;
using FormUI.Infrastructure;
using FormUI.Predictions;
using FormUICore.Logic;
using FormUICore.Predictions;

namespace FormUI.Logic
{
    public static class AiTankLogic
    {
        public static void CalculateAiPrizeTanks()
        {
            var aiPrizeTanks = State.ThisRound.AiPrizeTanks;

            if (!aiPrizeTanks.Any() || !State.HasPrevRound)
                return;

            foreach (var aiPrizeTank in aiPrizeTanks)
            {
                var nearPoints = aiPrizeTank.Point.GetNearPoints();
                var prevRoundNearTanks = State.PrevRound.AiTanks.Where(t => nearPoints.Contains(t.Point)).ToList();
                var foundPrevRoundNearTank = prevRoundNearTanks.Count == 1
                    ? prevRoundNearTanks.First()
                    : prevRoundNearTanks.FirstOrDefault(p => p.GetNextPoints(p.Point).First() == aiPrizeTank.Point);

                if (foundPrevRoundNearTank != null)
                    aiPrizeTank.Direction = foundPrevRoundNearTank.Direction;
            }
        }

        public static void CalculateAiTanks()
        {
            var aiTanks = State.ThisRound.AiTanks;
            if (!State.ThisRound.AiPrizeTanks.Any() && State.HasPrevRound)
            {
                foreach (var aiTank in aiTanks)
                {
                    var nearPoints = aiTank.Point.GetNearPoints(includeThis: true);
                    var prevRoundNearPrizeTanks = State.PrevRound.AiPrizeTanks.Where(t => nearPoints.Contains(t.Point)).ToList();
                    var foundPrevRoundNearPrizeTank = prevRoundNearPrizeTanks.Count == 1
                        ? prevRoundNearPrizeTanks.First()
                        : prevRoundNearPrizeTanks.FirstOrDefault(p => p.GetNextPoints(p.Point).First() == aiTank.Point);

                    if (foundPrevRoundNearPrizeTank != null)
                        State.ThisRound.AddAiPrizeTank(aiTank, foundPrevRoundNearPrizeTank);
                }
            }

            PredictionLogic.CalculateMobilePredictions(State.ThisRound.AiTanks, PredictionType.AiMove, x => x.CanMove);
            CalculateStuckAiMovePredictions();
            PredictionLogic.CalculateTanksShotPredictions(State.ThisRound.AiTanks, PredictionType.AiShot, AppSettings.MyShotPredictionDepth);
        }

        private static void CalculateStuckAiMovePredictions()
        {
            var stuckAiTanks = State.ThisRound.AiTanks.Where(x => x.IsStuck).ToList();
            PredictionLogic.CalculateStuckPosition(stuckAiTanks, PredictionType.AiMove, AppSettings.StuckAiPredictionDepth);
        }
    }
}
