using System;
using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldObjects;
using FormUI.Infrastructure;
using FormUI.Logic;

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
            if (NextCommandCalculationLogic.ProcessMyKill())
                return;

            //if (NextCommandCalculationLogic.CalculateNextCommand())
            //    return;

            if (DeadZoneLogic.ProcessDeadZone())
                return;

            if (DefaultTargetLogic.ProcessDefaultTarget())
                return;

            if (ProcessMyTankLost())
                return;
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

    }
}

