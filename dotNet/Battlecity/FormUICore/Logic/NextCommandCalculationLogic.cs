using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormUI.FieldItems;
using FormUI.FieldObjects;
using FormUI.Infrastructure;
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
                var directionMyKills = orderedMyKillPredictions.Where(x => x.Commands.StartWith(directions)).ToList();
                if (directionMyKills.Any())
                {
                    var prediction = directionMyKills.FirstOrDefault();

                    SetCurrentMove(prediction);

                    return true;
                }

                var directionDefaultTargetPredictions = orderedDefaultTargetPredictions.Where(x => x.Commands.StartWith(directions)).ToList();
                if (directionDefaultTargetPredictions.Any())
                {
                    var prediction = directionDefaultTargetPredictions.FirstOrDefault();

                    SetCurrentMove(prediction);

                    return true;
                }
            }

            return false;
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

                if (cell.IsIce)
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

            var myKillPredictions = Field.GetPredictions(x => x.MyKillPredictions)
                .Select(x => (MyKillPrediction)x)
                .OrderBy(x => x.Depth)
                .ThenBy(x => x.Commands.Count())
                .ToList();

            result.AddRange(myKillPredictions);

            return result;


            //if (AppSettings.ChooseKillOnlyByCommandsLength)
            //{
            //    var minCommandsCount = myKillPredictions.Min(x => x.Commands.RoundsCount());
            //    var minCommandsKills = myKillPredictions.Where(x => x.Commands.RoundsCount() == minCommandsCount).ToList();

            //    //var minMovesCount = minDepthKills.Min(x => x.Commands.Count);
            //    //var minMovesKills = minDepthKills.Where(x => x.Commands.Count == minMovesCount).ToList();

            //    //nearestKill = minCommandsKills.First();
            //}
            //else
            //{
            //    var minDepth = myKillPredictions.Min(x => x.Depth);
            //    var minDepthKills = myKillPredictions.Where(x => x.Depth == minDepth).ToList();

            //    var minMovesCount = minDepthKills.Min(x => x.Commands.Count);
            //    var minMovesKills = minDepthKills.Where(x => x.Commands.Count == minMovesCount).ToList();

            //    //nearestKill = minMovesKills.First();
            //}


            //return result;
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


        public static bool ProcessMyKill()
        {
            var myKillBasePredictions = !IsMyCellSafe()
                ? Field.GetPredictions(x => x.MyKillPredictions)
                    .Where(x => !((MyKillPrediction)x).Commands.IsSingleAct()).ToList()
                : Field.GetPredictions(x => x.MyKillPredictions);

            if (!myKillBasePredictions.Any())
                return false;

            var myKillPredictions = myKillBasePredictions.Select(x => (MyKillPrediction)x).ToList();

            MyKillPrediction nearestKill;

            if (AppSettings.ChooseKillOnlyByCommandsLength)
            {
                var minCommandsCount = myKillPredictions.Min(x => x.Commands.Count());
                var minCommandsKills = myKillPredictions.Where(x => x.Commands.Count() == minCommandsCount).ToList();

                //var minMovesCount = minDepthKills.Min(x => x.Commands.Count);
                //var minMovesKills = minDepthKills.Where(x => x.Commands.Count == minMovesCount).ToList();

                nearestKill = minCommandsKills.First();
            }
            else
            {
                var minDepth = myKillPredictions.Min(x => x.Depth);
                var minDepthKills = myKillPredictions.Where(x => x.Depth == minDepth).ToList();

                var minMovesCount = minDepthKills.Min(x => x.Commands.Count);
                var minMovesKills = minDepthKills.Where(x => x.Commands.Count == minMovesCount).ToList();

                nearestKill = minMovesKills.First();
            }

            var sb = new StringBuilder();
            sb.AppendLine($"SelectedKill: {nearestKill.Point}");
            sb.AppendLine($"MyShot: {nearestKill.MyShot.Depth}");
            sb.AppendLine($"TargetMove: {nearestKill.TargetMove.Depth}");

            if (AppSettings.StoreMySelectedKillPredictions)
                Field.GetCell(nearestKill.Point).Predictions.MySelectedKillPredictions.Add(nearestKill);

            Logger.Append(sb.ToString());

            SetCurrentMove(nearestKill);
            return true;
        }

        [Obsolete]
        public static void SetCurrentMove(BasePrediction prediction)
        {
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
