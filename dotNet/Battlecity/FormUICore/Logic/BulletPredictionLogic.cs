using System.Collections.Generic;
using FormUI.FieldItems;
using FormUI.FieldObjects;
using FormUI.Infrastructure;
using FormUI.Predictions;
using FormUICore.FieldItems;
using FormUICore.Predictions;

namespace FormUICore.Logic
{
    public static class BulletPredictionLogic
    {
        public static void CalculateBulletPredictions(IEnumerable<Bullet> bullets)
        {
            foreach (var bullet in bullets)
                CalculateBulletPredictions(bullet);
        }

        public static void CalculateBulletPredictions(Bullet bullet)
        {
            var predictionStartPoint = bullet.Point;
            var canNotMoveFurther = false;

            AddThisRoundPredictions(bullet);

            for (var depth = 1; depth <= AppSettings.PredictionDepth; depth++)
            {
                var nextPoints = bullet.GetNextPoints(predictionStartPoint);
                foreach (var nextPoint in nextPoints)
                {
                    var nextCell = Field.GetCell(nextPoint);
                    nextCell.AddPrediction(depth, PredictionType.Bullet, item: bullet);

                    if (nextCell.CanShootThrough)
                        predictionStartPoint = nextPoint;
                    else
                    {
                        canNotMoveFurther = true;
                        break;
                    }
                }

                if (canNotMoveFurther)
                    break;
            }
        }

        private static void AddThisRoundPredictions(Bullet bullet)
        {
            var thisCell = Field.GetCell(bullet.Point);
            thisCell.AddPrediction(0, PredictionType.Bullet, item: bullet);
        }
    }
}
