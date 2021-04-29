using System;
using System.Collections.Generic;
using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Tank;
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
            if (NextCommandCalculationLogic.CalculateNextCommand())
            {
                DeadZoneLogic.ResetDeadZoneIndex();
                return;
            }

            if (DeadZoneLogic.ProcessDeadZone())
                return;

            if (EnableKamikazeMode())
                return;

            if (ProcessMyTankLost())
                return;
        }

        private static bool EnableKamikazeMode()
        {
            if (State.ThisRound.MyTank == null)
                return false;

            var enemyTanks = State.ThisRound.EnemyTanks.Select(x => (BaseTank)x).ToList();
            if (ChoseKamikazeTarget(enemyTanks))
                return true;

            var aiTanks = State.ThisRound.AiTanks.Select(x => (BaseTank)x).ToList();
            if (ChoseKamikazeTarget(aiTanks))
                return true;

            return false;
        }

        private static bool ChoseKamikazeTarget(List<BaseTank> tanks)
        {
            var myTank = State.ThisRound.MyTank;
            var nearPoints = myTank.Point.GetNearPoints();

            var nearTanks = tanks.Where(x => nearPoints.Contains(x.Point)).ToList();
            BaseTank nearTank = null;
            if (nearTanks.Count() > 1)
            {
                foreach (var tank in nearTanks)
                {
                    var directionToMe = tank.Point.CalculateDirectionToPoint(myTank.Point);
                    if (tank.Direction != directionToMe)
                        continue;

                    nearTank = tank;
                }
            }

            if (nearTank == null)
                nearTank = nearTanks.FirstOrDefault();
            
            if (nearTank != null)
            {
                var direction = myTank.Point.CalculateDirectionToPoint(nearTank.Point);

                if (myTank.Direction != direction)
                    State.ThisRound.CurrentMoveCommands.Add(direction);

                State.ThisRound.CurrentMoveCommands.Add(Direction.Act);
                return true;
            }

            return false;
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

                if (!prevRoundCommands.Contains(Direction.Act))
                    prevRoundCommands.Add(Direction.Act);

                State.ThisRound.CurrentMoveCommands.AddRange(prevRoundCommands);

                return true;
            }

            return false;
        }

    }
}

