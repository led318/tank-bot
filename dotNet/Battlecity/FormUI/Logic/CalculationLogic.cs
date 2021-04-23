using System;
using System.Collections.Generic;
using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Tank;
using FormUI.FieldObjects;
using FormUI.Infrastructure;
using FormUI.Predictions;

namespace FormUI.Logic
{
    public static class CalculationLogic
    {
        private static Point _defaultTargetPoint = new Point(17, 31);

        public static void PerformCalculations()
        {
            WaterLogic.PopulateHiddenWater();
            TreeLogic.PopulateItemsUnderTrees();

            ShotCountdownLogic.PopulateShotCountdownsFromPrevRound();
            PerformTick();

            AiTankLogic.CalculateAiPrizeTanks();
            AiTankLogic.CalculateAiTanks();
            EnemyTankLogic.CalculateEnemyTanks();
            BulletLogic.CalculateBullets();

            MyTankLogic.CalculateMyTankData();

            CalculateCurrentMove();
        }

        private static void PerformTick()
        {
            foreach (var baseItem in Field.AllItems)
            {
                baseItem.Tick();
            }
        }

        private static void CalculateCurrentMove()
        {
            var kills = Field.GetPredictions(x => x.MyKillPredictions);
            if (kills.Any())
            {
                var minDepth = kills.Min(x => x.Depth);
                var minDepthKills = kills.Where(x => x.Depth == minDepth).ToList();

                var minMovesCount = minDepthKills.Min(x => x.Commands.Count);
                var minMovesKills = minDepthKills.Where(x => x.Commands.Count == minMovesCount).ToList();

                var nearestKill = minMovesKills.First();

                SetCurrentMove(nearestKill);
                return;
            }

            var defaultCell = Field.GetCell(_defaultTargetPoint);
            var defaultCellNearestMovePrediction =
                defaultCell.Predictions.MyMovePredictions.OrderBy(x => x.Depth).FirstOrDefault();

            if (defaultCellNearestMovePrediction != null)
            {
                SetCurrentMove(defaultCellNearestMovePrediction);
                return;
            }
        }

        private static void SetCurrentMove(BasePrediction prediction)
        {
            State.ThisRound.CurrentMoveSelectedPrediction = prediction;

            if (prediction.Commands.Any())
            {
                State.ThisRound.CurrentMoveCommand.Add(prediction.Commands[0]);

                if (prediction.Commands.Count > 1 && prediction.Commands[1] == Direction.Act)
                {
                    State.ThisRound.CurrentMoveCommand.Add(prediction.Commands[1]);
                }
            }
        }
    }
}
