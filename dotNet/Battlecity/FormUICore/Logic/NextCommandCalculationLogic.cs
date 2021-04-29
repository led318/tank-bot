using System;
using System.Collections.Generic;
using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldObjects;
using FormUICore.FieldObjects;
using FormUICore.Infrastructure;
using FormUICore.Predictions;

namespace FormUICore.Logic
{
    public static class NextCommandCalculationLogic
    {
        public static bool CalculateNextCommand()
        {
            if (State.ThisRound.MyTank == null)
                return false;

            var orderedMyKillPredictions = GetMyKillPredictionsOrderedByDepthThenByCommandLength();
            var orderedDefaultTargetPredictions = GetDefaultTargetPredictionsOrderedByDepth();

            if (IsMyCellSafe())
            {
                var singleActCommandPrediction = orderedMyKillPredictions.FirstOrDefault(x => x.Commands.IsSingleAct());
                if (singleActCommandPrediction != null)
                {
                    SetCurrentMove(singleActCommandPrediction);
                    return true;
                }
            }
            else
            {
                orderedMyKillPredictions = orderedMyKillPredictions.Where(x => !x.Commands.IsSingleAct()).ToList();
            }

            var evaluatedDirections = GetDirectionEvaluationsWithDangerIndex();
            var groupedEvaluatedDirections = evaluatedDirections.GroupBy(x => x.DangerIndex).OrderBy(x => x.Key).ToList();

            foreach (var directionsGroup in groupedEvaluatedDirections)
            {
                var directions = directionsGroup.Select(x => x.Direction).ToList();

                var preferredMyKill = SelectPreferredPrediction(orderedMyKillPredictions, directions);
                if (preferredMyKill != null)
                {
                    SetCurrentMove(preferredMyKill);
                    return true;
                }

                var preferredDefaultTarget = SelectPreferredPrediction(orderedDefaultTargetPredictions, directions);
                if (preferredDefaultTarget != null)
                {
                    SetCurrentMove(preferredDefaultTarget);
                    return true;
                }

            }

            return false;
        }

        private static BasePrediction SelectPreferredPrediction(IEnumerable<BasePrediction> predictions, List<Direction> directions)
        {
            var directionPredictions = predictions.Where(x => x.Commands.StartWith(directions)).ToList();
            if (directionPredictions.Any())
            {
                var minDepth = directionPredictions.Min(x => x.Depth);
                var minDepthPredictions = directionPredictions.Where(x => x.Depth == minDepth).ToList();

                if (minDepthPredictions.Count > 1)
                {
                    var minDepthStartDirections = minDepthPredictions.Select(x => x.Commands.GetStartDirection())
                        .Where(x => x.HasValue).Select(x => x.Value).Distinct().ToList();

                    var prevRoundNextDirection = GetPrevRoundNextDirection(minDepthStartDirections);
                    if (prevRoundNextDirection.HasValue)
                    {
                        var preferredPrediction = minDepthPredictions.FirstOrDefault(x => x.Commands.StartWith(prevRoundNextDirection.Value));

                        return preferredPrediction;
                    }
                }

                var prediction = minDepthPredictions.FirstOrDefault();
                return prediction;
            }

            return null;
        }

        private static Direction? GetPrevRoundNextDirection(List<Direction> availableDirections)
        {
            if (!State.HasPrevRound)
                return null;

            var prevRoundPrediction = State.PrevRound.CurrentMoveSelectedPrediction;
            if (prevRoundPrediction == null)
                return null;

            if (prevRoundPrediction.Commands.Count <= 1)
                return null;

            var prevRoundNextCommand = prevRoundPrediction.Commands[1];
            var prevRoundNextDirection = prevRoundNextCommand.FirstOrDefault(x => BaseMobile.ValidDirections.Contains(x));

            if (availableDirections.Contains(prevRoundNextDirection))
                return prevRoundNextDirection;

            return null;
        }

        private static List<DirectionEvaluation> GetDirectionEvaluationsWithDangerIndex()
        {
            var result = new List<DirectionEvaluation>();
            var myTank = State.ThisRound.MyTank;

            foreach (var direction in BaseMobile.ValidDirections)
            {
                var index = 0;

                var point = myTank.Point.Shift(direction);
                var cell = Field.GetCell(point);

                if (cell.IsWall)
                {
                    index += 1000;
                }

                var criticalDangerCount = cell.CriticalDangerCount();
                index += criticalDangerCount * 100;

                var nonCriticalDangerCount = cell.NonCriticalDangerCount();
                index += nonCriticalDangerCount * 10;

                if (AppSettings.IceIsDangerousToStep && cell.IsIce)
                    index += 5;

                if (cell.IsPrize)
                    index -= 1;

                result.Add(new DirectionEvaluation { Direction = direction, DangerIndex = index });
            }

            return result;
        }

        private static List<MyKillPrediction> GetMyKillPredictionsOrderedByDepthThenByCommandLength()
        {
            var result = new List<MyKillPrediction>();

            var allMyKillPredictions = Field.GetPredictions(x => x.MyKillPredictions);
            allMyKillPredictions = FilterOutRepeatedTargets(allMyKillPredictions);
            allMyKillPredictions = RendezvousLogic.FilterOutRendezvousPredictions(allMyKillPredictions);

            var myKillPredictions = allMyKillPredictions
                .Select(x => (MyKillPrediction)x)
                .OrderBy(x => x.Depth)
                .ThenBy(x => x.Commands.Count())
                .ToList();

            result.AddRange(myKillPredictions);

            return result;
        }

        private static List<BasePrediction> FilterOutRepeatedTargets(List<BasePrediction> allMyKillPredictions)
        {
            return allMyKillPredictions.Where(x => !TargetLogLogic.IsSameTargetMultipleRounds(x)).ToList();
        }

        private static List<BasePrediction> GetDefaultTargetPredictionsOrderedByDepth()
        {
            var result = new List<BasePrediction>();

            var defaultTargetMovePredictions = DefaultTargetLogic.GetDefaultTargetMovePredictions()
                .OrderBy(x => x.Depth)
                .ToList();

            result.AddRange(defaultTargetMovePredictions);

            return result;
        }

        public static void SetCurrentMove(BasePrediction prediction)
        {
            TargetLogLogic.Add(prediction.Point);

            State.ThisRound.CurrentMoveSelectedPrediction = prediction;

            if (!prediction.Commands.Any())
                return;

            var command = prediction.Commands[0];

            State.ThisRound.CurrentMoveCommands.AddRange(command);
        }

        private static bool IsMyCellSafe()
        {
            var myTank = State.ThisRound.MyTank;
            if (myTank == null)
                return false;

            var myCell = Field.GetCell(myTank.Point);

            //var myCellIsBulletNextRound = myCell.Predictions.BulletPredictions.Any(x => x.Depth == 1);
            //var myCellIsAiShotNextRound = myCell.Predictions.AiShotPredictions.Any(x => x.Depth == 1);
            //var myCellIsEnemyShotNextRound = myCell.Predictions.EnemyShotPredictions.Any(x => x.Depth == 1);

            var myCellIsSafe = !myCell.Predictions.DangerCellPredictions.Any(x => x.Depth == 1);

            return myCellIsSafe;
        }
    }
}
