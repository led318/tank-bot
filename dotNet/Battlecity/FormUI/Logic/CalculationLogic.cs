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
        public static void PerformCalculations()
        {
            WaterLogic.PopulateHiddenWater();
            TreeLogic.PopulateBulletsUnderTrees();
            TreeLogic.PopulateAiTanksUnderTrees();

            ShotCountdownLogic.PopulateShotCountdownsFromPrevRound();
            PerformTick();

            AiTankLogic.CalculateAiPrizeTanks();
            AiTankLogic.CalculateAiTanks();
            EnemyTankLogic.CalculateEnemyTanks();
            BulletLogic.CalculateBullets();

            MyTankLogic.CalculateMyMovePredictions();
            MyTankLogic.CalculateMyShotPredictions();
        }

        private static void PerformTick()
        {
            foreach (var baseItem in Field.AllItems)
            {
                baseItem.Tick();
            }
        }
    }
}
