using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldObjects;
using FormUI.Infrastructure;
using FormUI.Logic;
using FormUICore.Infrastructure;
using FormUICore.Predictions;

// ReSharper disable InconsistentNaming

namespace FormUICore.Logic
{
    public static class CalculationLogic
    {
        private static readonly Random _random = new Random();

        public static void PerformCalculations()
        {
            WaterLogic.PopulateHiddenWater();
            IceLogic.PopulateHiddenIce();
            TreeLogic.PopulateItemsUnderTrees();

            ShotCountdownLogic.PopulateShotCountdownsFromPrevRound();
            PerformTick();

            BulletLogic.CalculateBullets();

            AiTankLogic.CalculateAiPrizeTanks();
            AiTankLogic.CalculateAiTanks();
            EnemyTankPredictionLogic.CalculateEnemyTanks();

            DangerCellLogic.CalculateDangerCells();

            MyTankPredictionLogic.CalculateMyTankData();

            CalculateCurrentMove();
        }

        private static void PerformTick()
        {
            foreach (var baseItem in Field.AllItems)
                baseItem.Tick();
        }

        private static void CalculateCurrentMove()
        {
            if (ProcessMyKill())
                return;

            if (DeadZoneLogic.ProcessDeadZone())
                return;

            if (DefaultTargetLogic.ProcessDefaultTarget())
                return;

            if (ProcessMyTankLost())
                return;
        }

        private static bool ProcessMyKill()
        {
            var myKillBasePredictions = CheckIfMyCellIsDangerous()
                ? Field.GetPredictions(x => x.MyKillPredictions)
                    .Where(x => !((MyKillPrediction)x).Commands.IsSingleAct()).ToList()
                : Field.GetPredictions(x => x.MyKillPredictions);

            if (!myKillBasePredictions.Any())
                return false;

            var myKillPredictions = myKillBasePredictions.Select(x => (MyKillPrediction)x).ToList();

            MyKillPrediction nearestKill;

            if (AppSettings.ChooseKillOnlyByCommandsLength)
            {
                var minCommandsCount = myKillPredictions.Min(x => x.Commands.RoundsCount());
                var minCommandsKills = myKillPredictions.Where(x => x.Commands.RoundsCount() == minCommandsCount).ToList();

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

        private static bool ProcessMyTankLost()
        {
            if (State.HasPrevRound && State.ThisRound.MyTank == null)
            {
                var prevRoundCommands = State.PrevRound.CurrentMoveCommands.ToList();

                if (!prevRoundCommands.Any(x => BaseMobile.ValidDirections.Contains(x)))
                {
                    var randomDirectionIndex = _random.Next(0, 3);
                    var randomDirection = BaseMobile.ValidDirections[randomDirectionIndex];
                    prevRoundCommands.Add(randomDirection);
                }

                if(!prevRoundCommands.Contains(Direction.Act))
                    prevRoundCommands.Add(Direction.Act);

                State.ThisRound.CurrentMoveCommands.AddRange(prevRoundCommands);

                return true;
            }

            return false;
        }

        private static bool CheckIfMyCellIsDangerous()
        {
            var myTank = State.ThisRound.MyTank;
            if (myTank == null)
                return false;

            var myCell = Field.GetCell(myTank.Point);

            //var myCellIsBulletNextRound = myCell.Predictions.BulletPredictions.Any(x => x.Depth == 1);
            //var myCellIsAiShotNextRound = myCell.Predictions.AiShotPredictions.Any(x => x.Depth == 1);
            //var myCellIsEnemyShotNextRound = myCell.Predictions.EnemyShotPredictions.Any(x => x.Depth == 1);

            var myCellIsDangerous = myCell.Predictions.DangerCellPredictions.Any(x => x.Depth == 1);

            return myCellIsDangerous;
        }

        public static void SetCurrentMove(BasePrediction prediction)
        {
            State.ThisRound.CurrentMoveSelectedPrediction = prediction;

            if (!prediction.Commands.Any())
                return;

            State.ThisRound.CurrentMoveCommands.Add(prediction.Commands[0]);

            if (prediction.Commands.Count > 1 && prediction.Commands[1] == Direction.Act)
                State.ThisRound.CurrentMoveCommands.Add(prediction.Commands[1]);
        }
    }
}

