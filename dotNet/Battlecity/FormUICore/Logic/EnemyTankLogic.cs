using System.Linq;
using FormUI.FieldItems;
using FormUI.FieldItems.Tank;
using FormUI.FieldObjects;
using FormUI.Infrastructure;
using FormUI.Logic;
using FormUI.Predictions;

namespace FormUICore.Logic
{
    public static class EnemyTankLogic
    {
        public static void CalculateEnemyTanks()
        {
            foreach (var enemyTank in State.ThisRound.EnemyTanks)
                CalculateEnemyTankShotPredictions(enemyTank);

            CalculateNotStuckEnemyMovePredictions();
            CalculateStuckEnemyMovePredictions();
        }

        private static void CalculateNotStuckEnemyMovePredictions()
        {
            var notStuckEnemyTanks = State.ThisRound.EnemyTanks.Where(x => !x.IsStuck).ToList();
            PredictionLogic.CalculateMobilePredictions(notStuckEnemyTanks, PredictionType.EnemyMove, x => x.CanMove, depthRestriction: AppSettings.EnemyTankMovePredictionDepth);
        }

        private static void CalculateStuckEnemyMovePredictions()
        {
            var stuckEnemyTanks = State.ThisRound.EnemyTanks.Where(x => x.IsStuck).ToList();
            PredictionLogic.CalculateStuckPosition(stuckEnemyTanks, PredictionType.EnemyMove, AppSettings.StuckEnemyPredictionDepth);
        }

        private static void CalculateEnemyTankShotPredictions(BaseTank tank)
        {
            if (tank.IsStuck)
                return;

            if (tank.CurrentDirection.HasValue)
                PredictionLogic.CalculateTankShotPredictions(tank.Point, PredictionType.EnemyShot, tank.CurrentDirection.Value, tank, AppSettings.EnemyTankShotPredictionDepth);

            var directions = BaseMobile.ValidDirections;
            foreach (var direction in directions)
            {
                var point = tank.Point.Shift(direction);
                var cell = Field.GetCell(point);
                if (cell.CanMove)
                    PredictionLogic.CalculateTankShotPredictions(point, PredictionType.EnemyShot, direction, tank, AppSettings.EnemyTankShotPredictionDepth);
            }
        }
    }
}
