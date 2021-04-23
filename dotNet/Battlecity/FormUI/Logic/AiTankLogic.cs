using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormUI.FieldItems.Tank;
using FormUI.Infrastructure;
using FormUI.Predictions;

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

                var prevRoundNearTanks = State.PrevRound.AiTanks
                    .Where(t => nearPoints.Contains(t.Point)).ToList();

                AiTank foundPrevRoundNearTank = null;

                if (prevRoundNearTanks.Count() == 1)
                {
                    foundPrevRoundNearTank = prevRoundNearTanks.First();
                }
                else
                {
                    foundPrevRoundNearTank = prevRoundNearTanks
                        .FirstOrDefault(p => p.GetNextPoints(p.Point).First() == aiPrizeTank.Point);
                }


                if (foundPrevRoundNearTank != null)
                {
                    aiPrizeTank.CurrentDirection = foundPrevRoundNearTank.CurrentDirection;
                }
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

                    var prevRoundNearPrizeTanks = State.PrevRound.AiPrizeTanks
                        .Where(t => nearPoints.Contains(t.Point)).ToList();

                    AiTank foundPrevRoundNearPrizeTank = null;

                    if (prevRoundNearPrizeTanks.Count() == 1)
                    {
                        foundPrevRoundNearPrizeTank = prevRoundNearPrizeTanks.First();
                    }
                    else
                    {
                        foundPrevRoundNearPrizeTank = prevRoundNearPrizeTanks
                            .FirstOrDefault(p => p.GetNextPoints(p.Point).First() == aiTank.Point);
                    }

                    if (foundPrevRoundNearPrizeTank != null)
                    {
                        State.ThisRound.AddAiPrizeTank(aiTank, foundPrevRoundNearPrizeTank);
                    }
                }
            }


            PredictionLogic.CalculateMobilePredictions(State.ThisRound.AiTanks, PredictionType.AiMove, x => x.CanMove);

            PredictionLogic.CalculateTanksShotPredictions(State.ThisRound.AiTanks, PredictionType.AiShot, AppSettings.MyShotPredictionDepth);
        }
    }
}
