using System;
using System.Collections.Generic;
using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Tank;
using FormUI.FieldObjects;
using FormUI.Infrastructure;
using FormUI.Predictions;
using FormUICore.FieldItems;
using FormUICore.Infrastructure;
using FormUICore.Predictions;

namespace FormUICore.Logic
{
    public static class EnemyTankPredictionLogic
    {
        public static void CalculateEnemyTanks()
        {
            CalculateEnemyTankShotPredictions();
            CalculateNotStuckEnemyMovePredictions();
            CalculateStuckEnemyMovePredictions();
        }

        #region EnemyShot

        private static void CalculateEnemyTankShotPredictions()
        {
            foreach (var enemyTank in State.ThisRound.EnemyTanks)
                CalculateEnemyTankShotPredictions(enemyTank);
        }

        private static void CalculateEnemyTankShotPredictions(EnemyTank tank)
        {
            if (tank.IsStuck)
            {
                if (!tank.IsShoting || !tank.Direction.HasValue)
                    return;

                CalculateEnemyShotForDirection(tank.Point, tank.Direction.Value, tank);

                return;
            }

            var directions = BaseMobile.ValidDirections;
            foreach (var direction in directions)
            {
                CalculateEnemyShotForDirection(tank.Point, direction, tank);

                var point = tank.Point.Shift(direction);
                var cell = Field.GetCell(point);
                if (cell.CanMove)
                    CalculateEnemyShotForDirection(point, direction, tank);
            }
        }

        private static void CalculateEnemyShotForDirection(Point point, Direction direction, EnemyTank tank)
        {
            var startShotPoint = point;

            //var startIndex = Math.Min(-1 - tank.ShotCountdownLeft, 1);
            var startIndex = 1;

            for (var i = startIndex; i <= Bullet.DefaultSpeed * AppSettings.EnemyTankShotPredictionDepth; i++)
            {
                var shotPoint = BaseMobile.Shift(startShotPoint, direction);
                var shotCell = Field.GetCell(shotPoint);

                if (shotCell.IsWall)
                    break;

                //var depth = (int)Math.Ceiling((decimal)i / 2);
                //var actualDepth = Math.Max(1, tank.ShotCountdownLeft) + depth;

                var depth = (int)Math.Ceiling((decimal)i / 2);

                var shotCountDownLeft = tank.ShotCountdownLeft;
                var actualDepth = shotCountDownLeft + depth;

                shotCell.AddPrediction(actualDepth, PredictionType.EnemyShot);

                if (shotCell.CanShootThrough)
                {
                    startShotPoint = shotPoint;
                }
                else
                {
                    break;
                }
            }
        }


        #endregion EnemyShot

        #region EnemyMove

        private static void CalculateStuckEnemyMovePredictions()
        {
            var stuckEnemyTanks = State.ThisRound.EnemyTanks.Where(x => x.IsStuck).ToList();
            PredictionLogic.CalculateStuckPosition(stuckEnemyTanks, PredictionType.EnemyMove, AppSettings.StuckEnemyPredictionDepth);
        }

        private static void CalculateNotStuckEnemyMovePredictions()
        {
            var notStuckEnemyTanks = State.ThisRound.EnemyTanks.Where(x => !x.IsStuck).ToList();
            CalculateEnemyMovePredictions(notStuckEnemyTanks);
        }

        private static void CalculateEnemyMovePredictions(IEnumerable<EnemyTank> enemyTanks)
        {
            foreach (var enemyTank in enemyTanks)
            {
                CalculateEnemyMovePredictions(enemyTank);
            }
        }

        private static void CalculateEnemyMovePredictions(EnemyTank enemyTank)
        {
            var thisCell = Field.GetCell(enemyTank.Point);
            thisCell.AddPrediction(1, PredictionType.EnemyMove, item: enemyTank);

            CalculateNotForwardNearestMoves(enemyTank);

            var predictionStartPoint = enemyTank.Point;

            for (var depth = 1; depth <= AppSettings.EnemyTankMovePredictionDepth; depth++)
            {
                var nextPoint = enemyTank.GetNextPoints(predictionStartPoint).First();
                var nextCell = Field.GetCell(nextPoint);

                if (nextCell.CanMove)
                {
                    nextCell.AddPrediction(depth, PredictionType.EnemyMove, item: enemyTank);
                    predictionStartPoint = nextPoint;
                }
            }
        }

        private static void CalculateNotForwardNearestMoves(EnemyTank enemyTank)
        {
            if (!AppSettings.EnableEnemyTankNotForwardNearestMoves)
                return;

            var notForwardDirections = BaseMobile.ValidDirections.Where(x => x != enemyTank.Direction).ToList();

            foreach (var direction in notForwardDirections)
            {
                var nextPoint = enemyTank.GetNextPoints(enemyTank.Point, direction).First();
                var nextCell = Field.GetCell(nextPoint);

                if (nextCell.CanMove)
                    nextCell.AddPrediction(1, PredictionType.EnemyMove, item: enemyTank);
            }
        }

        #endregion EnemyMove
    }
}
