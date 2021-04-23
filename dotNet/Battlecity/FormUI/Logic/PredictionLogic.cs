using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Tank;
using FormUI.FieldObjects;
using FormUI.Infrastructure;
using FormUI.Predictions;

namespace FormUI.Logic
{
    public static class PredictionLogic
    {
        public static void CalculateStuckPosition(IEnumerable<BaseMobile> mobileItems, PredictionType type, int maxDepth)
        {
            foreach (var mobileItem in mobileItems)
            {
                var cell = Field.GetCell(mobileItem.Point);

                for (var depth = 1; depth <= maxDepth; depth++)
                {
                    cell.AddPrediction(depth, type, item: mobileItem);
                }
            }
        }

        public static void CalculateMobilePredictions(IEnumerable<BaseMobile> mobileItems, PredictionType type, Func<Cell, bool> breakCondition, bool includeFirstObstacle = false, int? depthRestriction = null)
        {
            var maxDepth = depthRestriction ?? AppSettings.PredictionDepth;

            foreach (var mobileItem in mobileItems)
            {
                var predictionStartPoint = mobileItem.Point;
                var canNotMoveFurther = false;

                for (var depth = 1; depth <= maxDepth; depth++)
                {
                    var nextPoints = mobileItem.GetNextPoints(predictionStartPoint);

                    foreach (var nextPoint in nextPoints)
                    {
                        var nextCell = Field.GetCell(nextPoint);

                        if (breakCondition(nextCell))
                        {
                            nextCell.AddPrediction(depth, type, item: mobileItem);
                            predictionStartPoint = nextPoint;
                        }
                        else
                        {
                            if (includeFirstObstacle)
                            {
                                nextCell.AddPrediction(depth, type, item: mobileItem);
                            }

                            canNotMoveFurther = true;
                            break;
                        }
                    }

                    if (canNotMoveFurther)
                        break;
                }
            }
        }

        public static void CalculateTanksShotPredictions(IEnumerable<BaseTank> tanks, PredictionType type, int maxDepth = 1)
        {
            foreach (var tank in tanks)
            {
                if (tank.CurrentDirection.HasValue)
                {
                    CalculateTankShotPredictions(tank.Point, type, tank.CurrentDirection.Value, tank, maxDepth);
                }
            }
        }

        public static void CalculateTankShotPredictions(Point point, PredictionType type, Direction direction, BaseTank tank, int maxDepth = 1, List<Direction> command = null)
        {

            var startShotPoint = point;

            var startIndex = Math.Min(-1 - tank.ShotCountdownLeft, 1);

            //if (!tank.IsShotThisRound)
            //    return;


            for (var i = startIndex; i <= Bullet.DefaultSpeed * maxDepth; i++)
            {
                var shotPoint = BaseMobile.Shift(startShotPoint, direction);
                var shotCell = Field.GetCell(shotPoint);

                var depth = (int)Math.Ceiling((decimal)i / 2);

                if (i >= -1)
                {
                    var actualDepth = Math.Max(1, tank.ShotCountdownLeft) + depth;
                    shotCell.AddPrediction(actualDepth, type, command);
                }

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
    }
}
