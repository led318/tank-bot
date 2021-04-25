﻿using System;
using System.Collections.Generic;
using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldObjects;
using FormUI.Infrastructure;
using FormUI.Logic;
using FormUICore.Predictions;

// ReSharper disable InconsistentNaming

namespace FormUICore.Logic
{
    public static class CalculationLogic
    {
        private static Random _random = new Random();
        private static int _currentDefaultTargetPointIndex;
        private static readonly List<Point> _defaultTargetPoints = new List<Point> { new Point(3, 31), new Point(31, 31) };
        private static Point _currentDefaultTargetPoint => _defaultTargetPoints[_currentDefaultTargetPointIndex];

        private static readonly Point _deadZoneStart = new Point(13, 1);
        private static readonly Point _deadZoneEnd = new Point(20, 4);
        private static readonly List<Direction> _deadZoneAction = new List<Direction> { Direction.Left, Direction.Act };

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
            if (ProcessDeadZone())
                return;

            if (ProcessMyKill())
                return;

            if (ProcessDefaultTarget())
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

            SetCurrentMove(nearestKill);
            return true;
        }

        private static bool ProcessDeadZone()
        {
            if (!IsInDeadZone())

                return false;
            State.ThisRound.IsInDeadZone = true;
            State.ThisRound.CurrentMoveCommands.AddRange(_deadZoneAction);

            return true;
        }

        private static bool ProcessDefaultTarget()
        {
            CheckAndChangeDefaultTargetPoint();
            var defaultCell = Field.GetCell(_currentDefaultTargetPoint);
            var defaultCellNearestMovePrediction =
                defaultCell.Predictions.MyMovePredictions.OrderBy(x => x.Depth).FirstOrDefault();

            if (defaultCellNearestMovePrediction == null)
                return false;

            SetCurrentMove(defaultCellNearestMovePrediction);
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

            var myCellIsBulletNextRound = myCell.Predictions.BulletPredictions.Any(x => x.Depth == 1);
            var myCellIsAiShotNextRound = myCell.Predictions.AiShotPredictions.Any(x => x.Depth == 1);
            var myCellIsEnemyShotNextRound = myCell.Predictions.EnemyShotPredictions.Any(x => x.Depth == 1);

            var myCellIsDangerous = myCellIsBulletNextRound || myCellIsAiShotNextRound || myCellIsEnemyShotNextRound;

            return myCellIsDangerous;
        }

        private static void SetCurrentMove(BasePrediction prediction)
        {
            State.ThisRound.CurrentMoveSelectedPrediction = prediction;

            if (!prediction.Commands.Any())
                return;

            State.ThisRound.CurrentMoveCommands.Add(prediction.Commands[0]);

            if (prediction.Commands.Count > 1 && prediction.Commands[1] == Direction.Act)
                State.ThisRound.CurrentMoveCommands.Add(prediction.Commands[1]);
        }

        private static bool IsInDeadZone()
        {
            if (State.ThisRound.MyTank == null)
                return false;

            var myTank = State.ThisRound.MyTank;

            var isXInDeadZone = myTank.Point.X >= _deadZoneStart.X && myTank.Point.X <= _deadZoneEnd.X;
            var isYInDeadZone = myTank.Point.Y >= _deadZoneStart.Y && myTank.Point.Y <= _deadZoneEnd.Y;

            return isXInDeadZone && isYInDeadZone;
        }

        private static void CheckAndChangeDefaultTargetPoint()
        {
            if (State.ThisRound.MyTank == null)
                return;

            if (State.ThisRound.MyTank.Point == _currentDefaultTargetPoint)
                _currentDefaultTargetPointIndex = (_currentDefaultTargetPointIndex + 1) % _defaultTargetPoints.Count;
        }
    }
}
